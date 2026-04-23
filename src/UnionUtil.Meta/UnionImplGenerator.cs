#pragma warning disable CS8524
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace UnionUtil.Meta;

using static UnionImplOptions;
using static SymbolDisplayFormat;

[Generator(LanguageNames.CSharp)]
public sealed class UnionImplGenerator : IIncrementalGenerator {
   public void Initialize(IncrementalGeneratorInitializationContext ctx) {
      var provider = ctx.SyntaxProvider.ForAttributeWithMetadataName($"{nameof(UnionUtil)}.{nameof(UnionImplAttribute)}",
               static (node, token) => node switch {
                  ClassDeclarationSyntax { AttributeLists.Count: > 0 } => true,
                  StructDeclarationSyntax { AttributeLists.Count: > 0 } => true,
                  _ => false,
               }, Resolve).Where(e => e != default);
      ctx.RegisterSourceOutput(provider, static (ctx, res) => {
         if (res.err is not Unresolved.Default) goto report_error;
         if (res.ok is null) return;
         GenerateSource(ctx, res.ok);
         return;
      report_error:
         var msg = $"{res.err}";
         var desc = new DiagnosticDescriptor(msg, msg, msg, "Generator", DiagnosticSeverity.Error, true);
         ctx.ReportDiagnostic(Diagnostic.Create(desc, Location.None));
         return;
      });
   }
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
   sealed record Resolved(
      string? Namespace,
      string TypeName,
      string T,
      SyntaxKind Kind,
      UnionImplOptions Options,
      Storage StorageTypes,
      Tagged? Tagged,
      SmallBufferOptimized SmallBufferOptimized,
      Visibility FieldVisibility
   );
   enum Unresolved {
      Default = default,
      MissingConstructorArgument,
      MissingTypesMarker,
      MissingPartialKeyword,
   }
   static (Resolved? ok, Unresolved err) Resolve(GeneratorAttributeSyntaxContext ctx, CancellationToken token) {
      if (ctx.TargetSymbol is not INamedTypeSymbol symbol) return default;
      var node = (TypeDeclarationSyntax)ctx.TargetNode;
      if (!node.Modifiers.Any(SyntaxKind.PartialKeyword)) {
         return (null, Unresolved.MissingPartialKeyword);
      }
      var attr = ctx.Attributes[0];
      var symbolAttributes = symbol.GetAttributes();
      if (attr.ConstructorArguments.Length is 0) {
         return (null, Unresolved.MissingConstructorArgument);
      }
      var opts = (UnionImplOptions)attr.ConstructorArguments[0].Value!;
      var boxGenerics = opts.HasFlag(UnionImplOptions.BoxOpenGenerics);
      var boxStructs = opts.HasFlag(BoxManagedStructs);
      bool? mutable = opts.HasFlag(UnionImplOptions.ReadOnlyEnabled) ? false : null;
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
      if (opts.HasFlag(UnionImplOptions.ReadOnlyEnabled)) goto skip_readonly;
      if (ctx.TargetNode is not StructDeclarationSyntax sds) goto skip_readonly;
      if (!sds.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)) goto skip_readonly;
      opts |= UnionImplOptions.ReadOnlyEnabled;
   skip_readonly:
      const string unionTypesName = Meta.Configuration.UnionType.Name;
      var unionTypes = symbol.Interfaces
         .Where(e => e.Name is $"I{unionTypesName}")
         .FirstOrDefault();
      StorageEntry[] resolvedTypeArgs;
      if (unionTypes is not null) goto resolve_union_types;
      unionTypes = symbol.GetAttributes()
         .Where(e => e.AttributeClass?.Name is $"{unionTypesName}Attribute")
         .Select(e => e.AttributeClass)
         .FirstOrDefault();
      if (unionTypes is null) {
         return (null, Unresolved.MissingTypesMarker);
      }
   resolve_union_types:
      var targs = unionTypes.TypeArguments;
      resolvedTypeArgs = new StorageEntry[targs.Length];
      for (int i = 0; i < targs.Length; ++i) {
         int index = i + 1;
         var curr = targs[i];
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
            // to resolve base
            if (curr is INamedTypeSymbol { IsReferenceType: true } named) {
               INamedTypeSymbol? @base = symbol;
               while ((@base = @base.BaseType) != null) {
                  if (SymbolEqualityComparer.Default.Equals(@base, named)) {
                     kind = Kind.Interface;
                     break;
                  }
               }
            }
         }
         var strategy = kind switch {
            Kind.Reference or Kind.Interface => Strategy.Box,
            Kind.Unmanaged => Strategy.Overlap,
            Kind.Value => boxStructs ? Strategy.Box : Strategy.Sequential,
            Kind.Open => boxGenerics ? Strategy.Box : Strategy.Sequential,
            _ => throw new InvalidOperationException(),
         };
         resolvedTypeArgs[i] = new(index, curr.ToDisplayString(FullyQualifiedFormat), kind, strategy);
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
      if (names.Length != resolvedTypeArgs?.Length) {
         return (null, Unresolved.MissingTypesMarker);
      }
      var tagName = (string)tagged.ConstructorArguments[0].Value!;
      var tagEnum = tag.ToDisplayString(FullyQualifiedFormat);
      resolvedTagged = new Tagged(tagEnum, tagName, names, isDense);
   skip_tagged:
      var ns = symbol.ContainingNamespace;
      return (new Resolved(
         Options: opts,
         TypeName: symbol.Name,
         T: symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
         Kind: ctx.TargetNode.Kind(),
         Namespace: ns.IsGlobalNamespace ? null : ns.ToDisplayString(),
         StorageTypes: new(resolvedTypeArgs),
         Tagged: resolvedTagged,
         FieldVisibility: attr.NamedArguments
            .Where(static e => e.Key is nameof(UnionImplAttribute.FieldVisibility))
            .Select(static e => (Visibility)e.Value.Value!)
            .SingleOrDefault(),
         SmallBufferOptimized: sbo
      ), default);
   }

   static void GenerateSource(SourceProductionContext ctx, UnionImplGenerator.Resolved args) {
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
      const string invalidOperationException = "global::System.InvalidOperationException";
      const string unionUtil = "global::UnionUtil";
      const string iSmallBUffer = $"{unionUtil}.ISmallBuffer";
      const string iUnion = $"{unionUtil}.IUnion";
      const string genericHelpers = $"global::UnionUtil.OpenGenericHelpers";
      static string ghBox(string type, string arg) => $"{genericHelpers}.Box<{type}>({arg})";
      static string ghSboBox(string type, string sboType, string sbo, string obj, string arg)
         => $"{genericHelpers}.Box<{type},{sboType}>(ref {@unsafe}.AsRef(in {sbo}), ref {obj}, {arg})";
      static string ghRef(string type, string arg) => $"{genericHelpers}.Ref<{type}>(ref {arg})";
      static string ghSboRef(string type, string sboType, string sbo, string obj) => $"{genericHelpers}.Ref<{type},{sboType}>(ref {sbo}, ref {obj})";
      static string ghGet(string type, string arg) => $"{genericHelpers}.Get<{type}>({arg})";
      static string ghSboGet(string type, string sboType, string sbo, string obj) => $"{genericHelpers}.Get<{type},{sboType}>(ref {@unsafe}.AsRef(in {sbo}), {obj})";
      static string fieldName(in StorageEntry s) {
         return $"_{s.TypeIndex}";
      }

      var sb = new StringBuilder(2048);
      var opts = args.Options;
      var isReadOnly = opts.Enabled(ReadOnlyEnabled);
      var isNullable = opts.Enabled(NullableEnabled);
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
      sb.Append(args.T).Append(" : ");
      sb.Append($"{iUnion}<{string.Join(",", entries.Select(e => e.TypeName))}>, {iUnion}");
      sb.AppendLine(" {");
      var visibility = args.FieldVisibility.Keyword;
      var readonlyFieldMod = isReadOnly ? " readonly" : " ";
      (int, string)[] getValueExprs = [];
      (int, string)[] clearExprs = [];
      void writeConstructor(in StorageEntry arg, string assign) {
         sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public {{args.TypeName}}({{arg.TypeName}} v) {
               {{assign}};
               {{indexField}} = {{arg.TypeIndex}};
            }
         """);
         if (opts.Enabled(NoImplicitConversions)) return;
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
         if (opts.Enabled(ExplicitlyConvertibleToValue) is false) return;
         if (arg.Kind is Kind.Interface) return;
         sb.AppendLine($$"""
            public static explicit operator {{arg.TypeName}}({{args.T}} u) {
               return u.TryGetValue(out {{arg.TypeName}} v) ? v : throw new();
            }
         """);
      }
      void writeSetValue(in StorageEntry arg, string assign) {
         if (isReadOnly) return;
         sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            public void SetValue({{arg.TypeName}} v) {
               if ({{indexField}} is {{arg.TypeIndex}}) {
                  {{assign}} = v;
                  return;
               }
               ClearValue();
               {{assign}} = v;
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
                  if ({{indexField}} != {{e.TypeIndex}}) throw new {{invalidOperationException}}($"type index is {{{indexField}}} ({{tagged[e.TypeIndex]}}).");
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
      StorageEntry[] toOverlap = [.. entries.Where(static e => e.Strategy == Strategy.Overlap)];
      StorageEntry[] toBox = [.. entries.Where(static e => e.Strategy is Strategy.Box)];
      bool sboEnabled = args.SmallBufferOptimized.Tag is not Sbo.Disabled && toBox.Any(static e => e.Kind is Kind.Open or Kind.Value && e.Strategy is Strategy.Box);
      // setting up the fields first
      if (toBox.Length is not 0) writeField("object?", objectField, "default");
      writeField("byte", indexField);
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
      if (toOverlap.Length is 0) goto skip_to_overlap;
      sb.AppendLine($$"""
            [{{interopServices}}.StructLayout({{interopServices}}.LayoutKind.Explicit)]
            {{visibility}} struct {{overlappedType}} {
         """);
      foreach (var e in toOverlap) sb.AppendLine($"""
               [{interopServices}.FieldOffset(0)]
               public {e.TypeName} {fieldName(e)};
         """);
      sb.AppendLine("   }");
      writeField(overlappedType, overlappedField, "default");
      foreach (var e in toOverlap) {
         getValueExprs = [.. getValueExprs, (e.TypeIndex, $"{overlappedField}.{fieldName(e)}")];
         writeConstructor(e, $"{overlappedField} = new() {{{fieldName(e)} = v}};");
         writeTryGetValue(e, $"v = {overlappedField}.{fieldName(e)};");
         writeSetValue(e, $"{overlappedField}.{fieldName(e)}");
         writeProperties(e, $"{overlappedField}.{fieldName(e)}");
      }
   skip_to_overlap:
      if (toBox.Length is 0) goto skip_to_box;
      if (args.SmallBufferOptimized.Tag is Sbo.Size && TSbo is fallbackSboType) {
         var sbo = args.SmallBufferOptimized;
         sb.AppendLine($$"""
            [{{compilerServices}}.InlineArray({{sbo.Size}})]
            {{visibility}} struct {{fallbackSboType}}: {{iSmallBUffer}} {
               byte _element0;
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
         if (e.Kind is Kind.Open && e.Strategy is Strategy.Box) {
            if (TSbo is null) writeConstructor(e, $"{objectField} = {ghBox(T, "v")}");
            else writeConstructor(e, $"{ghSboBox(T, TSbo, sboField, objectField, "v")}");
         } else writeConstructor(e, $"{objectField} = v;");
         var getExpr = e.Kind switch {
            Kind.Reference or Kind.Interface => $"{@unsafe}.As<{T}>({objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{@unsafe}.Unbox<{T}>({objectField}!)",
            _ => TSbo is not null ? ghSboGet(T, TSbo, sboField, objectField) : ghGet(T, objectField),
         };
         var refExpr = e.Kind switch {
            Kind.Interface or Kind.Reference => $"{@unsafe}.As<object?,{T}>(ref {objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{@unsafe}.Unbox<{T}>({objectField}!)",
            _ => TSbo is not null ? ghSboRef(T, TSbo, sboField, objectField) : ghRef(T, objectField),
         };
         writeProperties(e, getExpr);
         writeTryGetValue(e, $"v = {getExpr};");
         writeSetValue(e, refExpr);
         if (args.SmallBufferOptimized.Tag is Sbo.Disabled) {
            clearExprs = [.. clearExprs, (e.TypeIndex, $"{objectField} = null")];
         }
         getValueExprs = [.. getValueExprs, (e.TypeIndex, TSbo is not null ? ghSboGet(e.TypeName, TSbo, sboField, objectField) : objectField)];
      }
   skip_to_box:
      StorageEntry[] sequential = [.. entries.Where(static e => e.Strategy is Strategy.Sequential)];
      if (sequential.Length is 0) goto skip_sequential;
      foreach (var e in sequential) {
         var T = e.TypeName;
         var holder = fieldName(e);
         sb.AppendLine($"  {visibility}{readonlyFieldMod} {T} {holder} = default!;");
         getValueExprs = [.. getValueExprs, (e.TypeIndex, holder)];
         writeConstructor(e, $"{holder} = v;");
         writeTryGetValue(e, $"v = {holder};");
         if (isReadOnly is false) {
            writeSetValue(e, holder);
            clearExprs = [.. clearExprs, (e.TypeIndex, $"{holder} = default!")];
         }
         writeProperties(e, holder);
      }
   skip_sequential:
      var obj = isNullable ? "object?" : "object";
      sb.AppendLine($"  public{ro} {obj} Value => {indexField} switch {{");
      foreach (var e in getValueExprs) sb.AppendLine($"    {e.Item1} => {e.Item2}!,");
      if (isNullable) sb.AppendLine($"    _ => null,");
      else sb.AppendLine($"    _ => throw new global::System.InvalidOperationException($\"type index was {{{indexField}}}\")");
      sb.AppendLine("  };");
      if (isNullable) {
         sb.AppendLine($"  public{ro} bool HasValue => {indexField} != 0;");
      }
      if (!opts.Enabled(IncludeHoldsTypeMethod)) goto skip_include_holds_type_method;
      sb.AppendLine($$"""
         [{{aggressiveInlining}}]
         public{{ro}} bool HoldsType<Type>() => {{indexField}} switch {
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
      if (clearExprs.Length is 0) {
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
               _ => throw new {{invalidOperationException}}("invalid tag " + {{indexField}}.ToString()),
            };
         }
      """);
   skip_tag_property:
      sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            bool {{iUnion}}.TryGetValue<Type>(out Type value) {
               switch ({{indexField}}) {
         """);
      foreach (var e in entries) {
         sb.AppendLine($$"""
                  case {{e.TypeIndex}}:
                     if (typeof(Type) != typeof({{e.TypeName}})) goto default;
                     value = default!;
                     return TryGetValue(out {{@unsafe}}.As<Type, {{e.TypeName}}>(ref value));
         """);
      }
      sb.AppendLine($$"""
                  default:
                     value = default!;
                     return false;
               }
            }
         """);
      sb.Append('}');
      var hintName = args.T;
      if (args.Namespace is not null) hintName = $"{args.Namespace}.{args.T}.g";
      hintName = hintName.Replace("global::", "").Replace("<", "{").Replace(">", "}").Replace(" ", "");
      ctx.AddSource(hintName, sb.ToString());
   }
}
