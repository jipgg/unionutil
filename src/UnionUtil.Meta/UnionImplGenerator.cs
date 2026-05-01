#pragma warning disable CS8524
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SpanUtility;
namespace UnionUtil.Meta;

using static UnionImplOptions;
using static SymbolDisplayFormat;

enum Strategy : byte { Box, Sequential, Overlap };
enum Kind : byte { Unmanaged, Open, Reference, Value, Interface };
readonly record struct StorageEntry(int TypeIndex, string TypeName, Kind Kind, Strategy Strategy);
readonly struct Storage(StorageEntry[] entries) : IEquatable<Storage> {
   public readonly StorageEntry[] Entries = entries;
   public bool Equals(Storage other) => Entries.AsSpan().SequenceEqual(other.Entries);
}
enum Sbo : byte { Disabled = default, Size = 1, Name = 2 }
readonly record struct SmallBufferOptimized(Sbo Tag, uint Size, string Name);
readonly record struct Tagged(string EnumTypeName, string PropertyName, string[] Names, bool IsDense) {
   public string? this[int typeIndex] => Names[(int)typeIndex - 1];
}
sealed record UnionImplResult(
   string? Namespace,
   string TypeName,
   string T,
   SyntaxKind Kind,
   UnionImplOptions Options,
   Storage StorageTypes,
   Tagged? Tagged,
   SmallBufferOptimized SmallBufferOptimized,
   Visibility FieldVisibility,
   bool Ok = true
);

[Generator(LanguageNames.CSharp)]
public sealed class UnionImplGenerator : IIncrementalGenerator {
   static bool Predicate(SyntaxNode node, CancellationToken token) {
      if (node.Kind() is not (SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration)) {
         return false;
      }
      if (((TypeDeclarationSyntax)node).AttributeLists.Count > 0) return true;
      return false;
   }
   public void Initialize(IncrementalGeneratorInitializationContext ctx) {
      var provider = ctx.SyntaxProvider.ForAttributeWithMetadataName(
            $"{nameof(UnionUtil)}.{nameof(UnionImplAttribute)}",
            Predicate, Resolve).Where(static e => e?.Ok is true);
      ctx.RegisterSourceOutput(provider, static (ctx, result) => {
         Debug.Assert(result!.Ok is true);
         var (hintName, sb) = ResolveSource(result);
         ctx.AddSource(hintName, sb.ToString());
      });
   }
   static UnionImplResult? Resolve(GeneratorAttributeSyntaxContext ctx, CancellationToken token) {
      if (ctx.TargetSymbol is not INamedTypeSymbol symbol) return null;
      var node = (TypeDeclarationSyntax)ctx.TargetNode;
      if (!node.Modifiers.Any(SyntaxKind.PartialKeyword)) return null;
      var attr = ctx.Attributes[0];
      var symbolAttributes = symbol.GetAttributes();
      if (attr.ConstructorArguments.Length is 0) return null;
      var opts = (UnionImplOptions)attr.ConstructorArguments[0].Value!;
      var boxGenerics = opts.Has(UnionImplOptions.BoxOpenGenerics);
      var boxStructs = opts.Has(BoxManagedStructs);
      bool? mutable = opts.Has(UnionImplOptions.EnableReadOnly) ? false : null;
      const string sboAttr = nameof(SmallBufferOptimizedAttribute);
      SmallBufferOptimized sbo = default;
      Debug.Assert(sbo.Tag is Sbo.Disabled);
      var sboType = symbolAttributes
         .Where(static e => e.AttributeClass?.MetadataName is $"{sboAttr}`1")
         .Select(static e => ((INamedTypeSymbol)e.AttributeClass!).TypeArguments[0])
         .FirstOrDefault();
      if (sboType is not null) {
         var sboTypeName = sboType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
         sbo = new SmallBufferOptimized(Sbo.Name, default, sboTypeName);
         goto sbo_done;
      }
      var sboSize = (uint?)symbol.GetAttributes()
         .Where(e => e.AttributeClass?.MetadataName is sboAttr)
         .SingleOrDefault()?.ConstructorArguments[0].Value ?? 0;
      if (sboSize is not 0) {
         sbo = new SmallBufferOptimized(Sbo.Size, sboSize, default!);
      }
   sbo_done:
      if (opts.Has(UnionImplOptions.EnableReadOnly)) goto skip_infer_readonly;
      if (ctx.TargetNode is not StructDeclarationSyntax sds) goto skip_infer_readonly;
      if (!sds.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)) goto skip_infer_readonly;
      opts |= UnionImplOptions.EnableReadOnly;
   skip_infer_readonly:
      var (typeArgs, ok) = symbol.ResolveUnionTypeArgs();
      if (!ok || typeArgs.Length is 0) return default;
      var storageEntries = new StorageEntry[typeArgs.Length];
      for (int i = 0; i < typeArgs.Length; ++i) {
         int index = i + 1;
         var curr = typeArgs[i];
         Kind kind;
         if (curr is ITypeParameterSymbol tps) {
            kind = tps switch {
               { HasValueTypeConstraint: true } => Kind.Value,
               { HasReferenceTypeConstraint: true } => Kind.Reference,
               // a bit awkward. While legal for the compiler, overlapping fieldoffsets of generic
               // type arguments is not allowed by the runtime, even when constrained to unmanaged.
               // I may look into still allowing union storage for small primitivees eventually.
               { HasUnmanagedTypeConstraint: true } => Kind.Value,
               _ => Kind.Open,
            };
         } else {
            kind = curr switch {
               { TypeKind: TypeKind.Interface } => Kind.Interface,
               { IsAbstract: true } => Kind.Interface,
               { IsUnmanagedType: true } => Kind.Unmanaged,
               { IsValueType: true } => Kind.Value,
               { IsReferenceType: true } => Kind.Reference,
               _ => default,
            };
            if (symbol.IsDerivedFrom(curr)) kind = Kind.Interface;
         }
         Strategy strategy;
         switch (kind) {
            case Kind.Reference or Kind.Interface:
               strategy = Strategy.Box;
               break;
            case Kind.Unmanaged:
               if (symbol.IsGenericType) goto case Kind.Value;
               strategy = Strategy.Overlap;
               break;
            case Kind.Value:
               strategy = boxStructs ? Strategy.Box : Strategy.Sequential;
               break;
            case Kind.Open:
               strategy = boxGenerics ? Strategy.Box : Strategy.Sequential;
               break;
            default:
               throw new InvalidOperationException();
         }
         storageEntries[i] = new(index, curr.ToDisplayString(FullyQualifiedFormat), kind, strategy);
      }
      Tagged? resolvedTagged = default;
      var tagged = symbol.GetAttributes()
         .Where(e => e.AttributeClass?.MetadataName is $"{nameof(TaggedAttribute<>)}`1")
         .SingleOrDefault();
      if (tagged is null) goto skip_tagged;
      var tag = tagged.AttributeClass!.TypeArguments[0];
      var members = tag.GetMembers()
         .OfType<IFieldSymbol>()
         .Where(static e => e.HasConstantValue)
         .ToArray();
      bool isDense = true;
      for (long i = 0; i < members.Length; ++i) {
         if (Convert.ToInt64(members[i].ConstantValue) != i) {
            isDense = false;
            break;
         }
      }
      string[] names = [.. members.Select(static e => e.Name)];
      if (names.Length < storageEntries.Length) return default;
      var tagName = (string)tagged.ConstructorArguments[0].Value!;
      var tagEnum = tag.ToDisplayString(FullyQualifiedFormat);
      resolvedTagged = new Tagged(tagEnum, tagName, names, isDense);
   skip_tagged:
      var ns = symbol.ContainingNamespace;
      return new UnionImplResult(
         Options: opts,
         TypeName: symbol.Name,
         T: symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
         Kind: ctx.TargetNode.Kind(),
         Namespace: ns.IsGlobalNamespace ? null : ns.ToDisplayString(),
         StorageTypes: new(storageEntries),
         Tagged: resolvedTagged,
         FieldVisibility: attr.NamedArguments
            .Where(static e => e.Key is nameof(UnionImplAttribute.FieldVisibility))
            .Select(static e => (Visibility)e.Value.Value!)
            .SingleOrDefault(),
         SmallBufferOptimized: sbo
      );
   }
   const string compilerServices = "global::System.Runtime.CompilerServices";
   const string aggressiveInlining = $"{compilerServices}.MethodImpl({compilerServices}.MethodImplOptions.AggressiveInlining)";
   const string @unsafe = $"{compilerServices}.Unsafe";
   const string objectField = "_box";
   const string overlappedField = "_overlap";
   const string overlappedType = "OverlappedStorage";
   const string sboField = "_sbo";
   const string fallbackSboType = "SmallBufferStorage";
   const string indexField = "_index";
   const string codeAnalysis = "global::System.Diagnostics.CodeAnalysis";
   const string unscopedRef = $"{codeAnalysis}.UnscopedRef";
   const string interopServices = "global::System.Runtime.InteropServices";
   const string unionUtil = "global::UnionUtil";
   const string throwHelpers = $"{unionUtil}.ThrowHelpers";
   const string throwInvalidOperation = $"{throwHelpers}.ThrowInvalidOperation";
   const string iSmallBUffer = $"{unionUtil}.ISmallBuffer";
   const string boxed = $"{unionUtil}.Boxed";
   const string boxHelpers = $"{unionUtil}.BoxHelpers";
   static string BoxHelpersWrite(string T, string v) => $"{boxHelpers}.Write<{T}>(ref {objectField}, {v})";
   static string BoxHelpersWrite(string T, string TSmallBuffer, string v) => $"{boxHelpers}.Write<{T}, {TSmallBuffer}>(ref {@unsafe}.AsRef(in {sboField}), ref {objectField}, {v})";
   static string BoxHelpersUpdate(string T, string v) => $"{boxHelpers}.Update<{T}>(ref {objectField}, {v})";
   static string BoxHelpersUpdate(string T, string TSmallBuffer, string v) => $"{boxHelpers}.Update<{T}, {TSmallBuffer}>(ref {@unsafe}.AsRef(in {sboField}), ref {objectField}, {v})";
   static string BoxHelpersRead(string T) => $"{boxHelpers}.Read<{T}>({objectField})";
   static string BoxHelpersRead(string T, string TSmallBuffer) => $"{boxHelpers}.Read<{T}, {TSmallBuffer}>(ref {@unsafe}.AsRef(in {sboField}), {objectField})";
   static string FieldName(in StorageEntry s) {
      return $"_{s.TypeIndex}";
   }
   static ResolvedSource ResolveSource(UnionImplResult args) {
      var sb = new StringBuilder(2048);
      var opts = args.Options;
      var isReadOnly = opts.Has(EnableReadOnly);
      var isNullable = opts.Has(EnableNullable);
      sb.AppendLine("#nullable enable");
      if (args.Namespace is string ns) {
         sb.AppendLine($"namespace {ns};");
      }
      sb.Append("partial ");
      string ro;
      switch (args.Kind) {
         case SyntaxKind.StructDeclaration:
            sb.Append("struct ");
            ro = " readonly";
            break;
         case SyntaxKind.ClassDeclaration:
            sb.Append("class ");
            ro = " ";
            break;
         default:
            throw new InvalidOperationException();
      }
      var entries = args.StorageTypes.Entries;
      sb.Append(args.T);
      if (opts.Has(ImplementUnionInterfaces)) {
         sb.Append($":{unionUtil}.{nameof(IUnionType)}");
      }
      sb.AppendLine(" {");
      var visibility = args.FieldVisibility.Keyword;
      var readonlyFieldMod = isReadOnly ? " readonly" : " ";
      using var getValueExprs = new SpanList<(int, string)>(entries.Length);
      using var clearExprs = new SpanList<(int, string)>(entries.Length);
      void writeConstructor(in StorageEntry arg, string assign) {
         sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public {{args.TypeName}}({{arg.TypeName}} v) {
               {{assign}};
               {{indexField}} = {{arg.TypeIndex}};
            }
         """);
         if (!opts.Has(ImplementFromIndexConstructors)) goto skip_implement_from_index_constructors;
         sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public {{args.TypeName}}({{unionUtil}}.FromIndex{{arg.TypeIndex}} _, {{arg.TypeName}} v) {
               {{assign}};
               {{indexField}} = {{arg.TypeIndex}};
            }
         """);
      skip_implement_from_index_constructors:
         if (opts.Has(NoImplicitConversions)) return;
         if (arg.Kind is not Kind.Interface) sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public static implicit operator {{args.T}}({{arg.TypeName}} v) => new(v);
         """);
      }
      void writeTryGetValue(in StorageEntry arg, string assign) {
         sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public{{ro}} bool TryGetValue(out {{arg.TypeName}} v) {
               if ({{indexField}} == {{arg.TypeIndex}}) {
                  {{assign}}
                  return true;
               } else {
                  v = default!;
                  return false;
               }
            }
         """);
         if (opts.Has(WithExplicitConversionsToValue) is false) return;
         if (arg.Kind is Kind.Interface) return;
         sb.AppendLine($$"""
            public static explicit operator {{arg.TypeName}}({{args.T}} u) {
               return u.TryGetValue(out {{arg.TypeName}} v) ? v : {{throwInvalidOperation}}<{{arg.TypeName}}>();
            }
         """);
      }
      void writeSetValue(in StorageEntry arg, string set, string reset) {
         if (isReadOnly) return;
         sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public void SetValue({{arg.TypeName}} v) {
               if ({{indexField}} is {{arg.TypeIndex}}) {
                  {{set}};
                  return;
               }
               ClearValue();
               {{reset}};
               {{indexField}} = {{arg.TypeIndex}};
            }
         """);
      }
      void writeProperties(in StorageEntry e, string refExpr) {
         if (args.Tagged is not Tagged tagged) return;
         var setter = isReadOnly ? "\n" : $$"""
               [{{aggressiveInlining}}]
               set => SetValue(value);
         """;
         sb.AppendLine($$"""
            public{{(isReadOnly ? ro : " ")}} {{e.TypeName}} {{tagged[e.TypeIndex]}} {
               [{{aggressiveInlining}}]
              {{(isReadOnly ? " " : ro)}} get {
                  if ({{indexField}} != {{e.TypeIndex}}) {{throwInvalidOperation}}($"type index is {{{indexField}}} ({{tagged[e.TypeIndex]}})");
                  return {{refExpr}};
               }
               {{setter}}
            }
         """);
      }
      void writeField(string type, string field, string? expr = null) {
         sb.Append($"   {visibility}{readonlyFieldMod} {type} {field}");
         if (expr is not null) sb.Append(" = ").Append(expr);
         sb.AppendLine(";");
      }
      var toOverlap = new SpanList<StorageEntry>(entries.Length);
      var toBox = new SpanList<StorageEntry>(entries.Length);
      var sequential = new SpanList<StorageEntry>(entries.Length);
      foreach (var e in entries) {
         switch (e.Strategy) {
            case Strategy.Overlap:
               toOverlap.Add(e);
               break;
            case Strategy.Box:
               toBox.Add(e);
               break;
            case Strategy.Sequential:
               sequential.Add(e);
               break;
         }
      }
      bool sboEnabled = args.SmallBufferOptimized.Tag is not Sbo.Disabled && toBox.Any(static e => e.Kind is Kind.Open or Kind.Value && e.Strategy is Strategy.Box);
      if (toBox.Count is not 0) writeField("object?", objectField, "default");
      string? TSbo = default;
      if (sboEnabled) {
         var size = args.SmallBufferOptimized.Size;
         TSbo = args.SmallBufferOptimized switch {
            { Tag: Sbo.Name, Name: var tn } => (string)tn,
            { Tag: Sbo.Size, Size: 7 or 15 or 23 } => $"{unionUtil}.SmallBuffer{size}",
            _ => fallbackSboType,
         };
         writeField(TSbo, sboField, "default");
      }
      writeField("byte", indexField);
      if (toOverlap.Count is 0) goto skip_to_overlap;
      sb.AppendLine($$"""
            [{{interopServices}}.StructLayout({{interopServices}}.LayoutKind.Explicit)]
            {{visibility}} struct {{overlappedType}} {
         """);
      foreach (var e in toOverlap) sb.AppendLine($"""
               [{interopServices}.FieldOffset(0)]
               public {e.TypeName} {FieldName(e)};
         """);
      sb.AppendLine("   }");
      writeField(overlappedType, overlappedField, "default");
      foreach (var e in toOverlap) {
         var field = $"{overlappedField}.{FieldName(e)}";
         getValueExprs.Add((e.TypeIndex, field));
         writeConstructor(e, $"{overlappedField} = new() {{{FieldName(e)} = v}};");
         writeTryGetValue(e, $"v = {field};");
         writeSetValue(e, $"{field} = v", $"{field} = v");
         writeProperties(e, field);
      }
   skip_to_overlap:
      if (toBox.Count is 0) goto skip_to_box;
      if (args.SmallBufferOptimized.Tag is Sbo.Size && TSbo is fallbackSboType) {
         var sbo = args.SmallBufferOptimized;
         sb.AppendLine($$"""
            {{visibility}} struct {{fallbackSboType}}: {{iSmallBUffer}} {
         #pragma warning disable CS0169
               byte {{string.Join(",", Enumerable
                     .Range(0, (int)sbo.Size)
                     .Select(i => $"_element{i}"))}};
         #pragma warning restore CS0169
               public static int Size {
                  [{{aggressiveInlining}}]
                  get => {{sbo.Size}};
               }
               public ref byte Data {
                  [{{unscopedRef}}]
                  [{{aggressiveInlining}}]
                  get => ref _element0;
               }
            }
         """);
      }
      foreach (var e in toBox) {
         var T = (string)e.TypeName;
         string writeExpr;
         if (e.Kind is Kind.Open && e.Strategy is Strategy.Box) {
            if (TSbo is null) writeExpr = $"{BoxHelpersWrite(T, "v")}";
            else writeExpr = $"{BoxHelpersWrite(T, TSbo, "v")}";
         } else writeExpr = $"{objectField} = v;";
         writeConstructor(e, writeExpr);
         var getExpr = e.Kind switch {
            Kind.Reference or Kind.Interface => $"{@unsafe}.As<{T}>({objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{@unsafe}.Unbox<{T}>({objectField}!)",
            _ => TSbo is not null ? BoxHelpersRead(T, TSbo) : BoxHelpersRead(T),
         };
         var refExpr = e.Kind switch {
            Kind.Interface or Kind.Reference => $"{objectField} = v",
            Kind.Value or Kind.Unmanaged => $"{@unsafe}.Unbox<{T}>({objectField}!) = v",
            _ => TSbo is not null ? BoxHelpersUpdate(T, TSbo, "v") : BoxHelpersUpdate(T, "v"),
         };
         writeProperties(e, getExpr);
         writeTryGetValue(e, $"v = {getExpr};");
         writeSetValue(e, refExpr, writeExpr);
         if (args.SmallBufferOptimized.Tag is Sbo.Disabled) {
            clearExprs.Add((e.TypeIndex, $"{objectField} = null"));
         }
         getValueExprs.Add((e.TypeIndex, TSbo is not null ? BoxHelpersRead(e.TypeName, TSbo) : objectField));
      }
   skip_to_box:
      if (sequential.Count is 0) goto skip_sequential;
      foreach (var e in sequential) {
         var T = e.TypeName;
         var holder = FieldName(e);
         sb.AppendLine($"  {visibility}{readonlyFieldMod} {T} {holder} = default!;");
         getValueExprs.Add((e.TypeIndex, holder));
         writeConstructor(e, $"{holder} = v;");
         writeTryGetValue(e, $"v = {holder};");
         if (isReadOnly is false) {
            writeSetValue(e, $"{holder} = v", $"{holder} = v");
            clearExprs.Add((e.TypeIndex, $"{holder} = default!"));
         }
         writeProperties(e, holder);
      }
   skip_sequential:
      var obj = isNullable ? "object?" : "object";
      sb.AppendLine($"  public{ro} {obj} Value => {indexField} switch {{");
      foreach (var e in getValueExprs) sb.AppendLine($"    {e.Item1} => {e.Item2}!,");
      if (isNullable) sb.AppendLine($"    _ => null,");
      else sb.AppendLine($"    _ => {throwInvalidOperation}<{obj}>($\"type index was {{{indexField}}}\")");
      sb.AppendLine("  };");
      if (isNullable) {
         sb.AppendLine($"  public{ro} bool HasValue => {indexField} != 0;");
      }
      if (!opts.Has(ImplementHoldsTypeMethod)) goto skip_include_holds_type_method;
      sb.AppendLine($$"""
         [{{aggressiveInlining}}]
         public{{ro}} bool Holds<Type>() => {{indexField}} switch {
      """);
      foreach (var e in entries) sb.AppendLine($"      {e.TypeIndex} => typeof(Type) == typeof({e.TypeName}),");
      sb.AppendLine($$"""
            _ => false,
         };
      """);
   skip_include_holds_type_method:
      if (isReadOnly) goto skip_clear_value_method;
      sb.AppendLine($$"""
         [{{aggressiveInlining}}]
         public void ClearValue() {
      """);
      if (clearExprs.Count is 0) {
         sb.AppendLine("   }");
         goto skip_clear_value_method;
      }
      sb.AppendLine($$"""
            switch ({{indexField}}) {
      """);
      foreach (var e in clearExprs) sb.AppendLine($"""
               case {e.Item1}:
                  {e.Item2};
                  break;
      """);
      sb.AppendLine("""
            }
         }
      """);
   skip_clear_value_method:
      if (args.Tagged is not { } tagged) {
         goto skip_tag_property;
      }
      var @enum = (string)tagged.EnumTypeName;
      if (isNullable) @enum += "?";

      sb.AppendLine($$"""
         public{{ro}} {{@enum}} {{tagged.PropertyName}} {
            [{{aggressiveInlining}}]
      """);
      if (tagged.IsDense) {
         var expr = $"(({@enum}){indexField} - 1)";
         if (isNullable) expr = $"{indexField} is 0 ? null : {expr}";
         sb.AppendLine($$"""
               get => {{expr}};
            }
         """);
         goto skip_tag_property;
      }
      sb.AppendLine($$"""
           get => {{indexField}} switch {
      """);
      foreach (var e in entries) sb.AppendLine($$"""
               {{e.TypeIndex}} => {{tagged.EnumTypeName}}.{{tagged[e.TypeIndex]}},
      """);
      if (isNullable) sb.AppendLine($"""
               0 => null,
      """);
      sb.AppendLine($$"""
               _ => {{throwInvalidOperation}}<{{@enum}}>($"invalid tag " + {{indexField}}.ToString()),
            };
         }
      """);
   skip_tag_property:
      if (!opts.Has(ImplementUnionInterfaces)) goto skip_implement_union_type_interface;
      const string @interface = $"{unionUtil}.{nameof(IUnionType)}";
      var canHoldTypeExpr = string.Join("||", entries.Select(static e => $"typeof(Tx) == typeof({e.TypeName})"));
      sb.AppendLine($$"""
         [{{aggressiveInlining}}]
         bool {{@interface}}.Holds<Tx>() => {{indexField}} switch {
      """);
      foreach (var e in entries) sb.AppendLine($"      {e.TypeIndex} => typeof(Tx) == typeof({e.TypeName}),");
      sb.AppendLine($$"""
            _ => false,
         };
      """);
      if (isReadOnly) sb.AppendLine($"""
         static bool {@interface}.IsReadOnly => true;
      """);
      if (isNullable) sb.AppendLine($"""
         static bool {@interface}.IsNullable => true;
      """);
      if (opts.Has(BoxManagedStructs)) sb.AppendLine($"""
         static bool {@interface}.BoxesManagedStructs => true;
      """);
      if (opts.Has(BoxOpenGenerics)) sb.AppendLine($"""
         static bool {@interface}.BoxesOpenGenerics => true;
      """);
      if (args.SmallBufferOptimized.Tag is Sbo.Size) sb.AppendLine($"""
         static int {@interface}.SmallBufferSize => {args.SmallBufferOptimized.Size};
      """);
      else if (args.SmallBufferOptimized.Tag is Sbo.Name) sb.AppendLine($"""
         static int {@interface}.SmallBufferSize => {boxHelpers}.GetSmallBufferSize<{args.SmallBufferOptimized.Name}>();
      """);
      sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            static bool {{@interface}}.CanHold<Tx>() => {{canHoldTypeExpr}};
            static int {{@interface}}.TypeCount => {{entries.Length}};
            object? {{@interface}}.Value => Value;
            bool {{@interface}}.HasValue => {{(isNullable ? "HasValue" : "true")}};
            [{{aggressiveInlining}}]
            bool {{@interface}}.TrySetValue<Tx>(Tx value) {
         """);
      if (!isReadOnly) {
         foreach (var e in entries) {
            sb.AppendLine($$"""
               if (typeof(Tx) == typeof({{e.TypeName}})) {
                  SetValue({{@unsafe}}.As<Tx, {{e.TypeName}}>(ref value));
                  return true;
               }
         """);
         }
      }
      sb.AppendLine($$"""
               return false;
            }
            [{{aggressiveInlining}}]
            bool {{@interface}}.TryGetValue<Tx>(out Tx value) {
               switch ({{indexField}}) {
         """);
      foreach (var e in entries) {
         sb.AppendLine($$"""
                  case {{e.TypeIndex}}:
                     if (typeof(Tx) != typeof({{e.TypeName}})) goto default;
                     value = default!;
                     return TryGetValue(out {{@unsafe}}.As<Tx, {{e.TypeName}}>(ref value));
         """);
      }
      sb.AppendLine($$"""
                  default:
                     value = default!;
                     return false;
               }
            }
            [{{aggressiveInlining}}]
            bool {{@interface}}.TryClearValue() {
               {{(isReadOnly ? "return false;" : "ClearValue(); return true;")}}
            }
         """);
   skip_implement_union_type_interface:
      sb.Append('}');
      var hintName = args.T;
      if (args.Namespace is not null) hintName = $"{args.Namespace}.{args.T}.g";
      hintName = hintName.Replace("global::", "").Replace("<", "{").Replace(">", "}").Replace(" ", "");
      return new(hintName, sb);
   }
}
