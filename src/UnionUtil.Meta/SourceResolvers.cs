using System.Buffers;
namespace UnionUtil.Meta;

using static Config;

using static UnionImplOptions;
readonly record struct ResolvedSource(string HintName, StringBuilder Source);
static class SourceResolvers {
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
   const string genericHelpers = $"global::UnionUtil.OpenGenericHelpers";
   static string GenericHelpersBox(string type, string arg) => $"{genericHelpers}.Box<{type}>({arg})";
   static string GenericHelpersSboBox(string type, string sboType, string sbo, string obj, string arg)
      => $"{genericHelpers}.Box<{type},{sboType}>(ref {@unsafe}.AsRef(in {sbo}), ref {obj}, {arg})";
   static string GenericHelpersRef(string type, string arg) => $"{genericHelpers}.Ref<{type}>(ref {arg})";
   static string GenericHelpersSboRef(string type, string sboType, string sbo, string obj) => $"{genericHelpers}.Ref<{type},{sboType}>(ref {sbo}, ref {obj})";
   static string GenericHelpersGet(string type, string arg) => $"{genericHelpers}.Get<{type}>({arg})";
   static string GenericHelpersSboGet(string type, string sboType, string sbo, string obj) => $"{genericHelpers}.Get<{type},{sboType}>(ref {@unsafe}.AsRef(in {sbo}), {obj})";
   static string FieldName(in StorageEntry s) {
      return $"_{s.TypeIndex}";
   }
   public static ResolvedSource ResolveUnionImplSource(UnionImplResult args) {
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
      if (!opts.Has(ImplementAdapter) && !opts.Has(ImplementInterface)) goto skip_interface_implementations;
      sb.Append(':');
      if (opts.Has(ImplementInterface)) {
         sb.Append($"{unionUtil}.{Config.UnionType.InterfaceName}<{string.Join(",", entries.Select(e => e.TypeName))}>,");
      }
      if (opts.Has(ImplementAdapter)) {
         sb.Append($"{unionUtil}.{Config.AdapterName},");
      }
      --sb.Length;
   skip_interface_implementations:
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
               public {e.TypeName} {FieldName(e)};
         """);
      sb.AppendLine("   }");
      writeField(overlappedType, overlappedField, "default");
      foreach (var e in toOverlap) {
         getValueExprs = [.. getValueExprs, (e.TypeIndex, $"{overlappedField}.{FieldName(e)}")];
         writeConstructor(e, $"{overlappedField} = new() {{{FieldName(e)} = v}};");
         writeTryGetValue(e, $"v = {overlappedField}.{FieldName(e)};");
         writeSetValue(e, $"{overlappedField}.{FieldName(e)}");
         writeProperties(e, $"{overlappedField}.{FieldName(e)}");
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
            if (TSbo is null) writeConstructor(e, $"{objectField} = {GenericHelpersBox(T, "v")}");
            else writeConstructor(e, $"{GenericHelpersSboBox(T, TSbo, sboField, objectField, "v")}");
         } else writeConstructor(e, $"{objectField} = v;");
         var getExpr = e.Kind switch {
            Kind.Reference or Kind.Interface => $"{@unsafe}.As<{T}>({objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{@unsafe}.Unbox<{T}>({objectField}!)",
            _ => TSbo is not null ? GenericHelpersSboGet(T, TSbo, sboField, objectField) : GenericHelpersGet(T, objectField),
         };
         var refExpr = e.Kind switch {
            Kind.Interface or Kind.Reference => $"{@unsafe}.As<object?,{T}>(ref {objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{@unsafe}.Unbox<{T}>({objectField}!)",
            _ => TSbo is not null ? GenericHelpersSboRef(T, TSbo, sboField, objectField) : GenericHelpersRef(T, objectField),
         };
         writeProperties(e, getExpr);
         writeTryGetValue(e, $"v = {getExpr};");
         writeSetValue(e, refExpr);
         if (args.SmallBufferOptimized.Tag is Sbo.Disabled) {
            clearExprs = [.. clearExprs, (e.TypeIndex, $"{objectField} = null")];
         }
         getValueExprs = [.. getValueExprs, (e.TypeIndex, TSbo is not null ? GenericHelpersSboGet(e.TypeName, TSbo, sboField, objectField) : objectField)];
      }
   skip_to_box:
      StorageEntry[] sequential = [.. entries.Where(static e => e.Strategy is Strategy.Sequential)];
      if (sequential.Length is 0) goto skip_sequential;
      foreach (var e in sequential) {
         var T = e.TypeName;
         var holder = FieldName(e);
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
      if (!opts.Has(WithHoldsTypeMethod)) goto skip_include_holds_type_method;
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
      if (!opts.Has(ImplementAdapter)) goto skip_implement_visitor;
      const string adapter = $"{unionUtil}.{Config.AdapterName}";
      var canHoldTypeExpr = string.Join("||", entries.Select(static e => $"typeof(Tx) == typeof({e.TypeName})"));
      sb.AppendLine($$"""
         [{{aggressiveInlining}}]
         bool {{adapter}}.HoldsType<Tx>() => {{indexField}} switch {
      """);
      foreach (var e in entries) sb.AppendLine($"      {e.TypeIndex} => typeof(Tx) == typeof({e.TypeName}),");
      sb.AppendLine($$"""
            _ => false,
         };
      """);
      sb.AppendLine($$"""
            [{{aggressiveInlining}}]
            bool {{adapter}}.CanHoldType<Tx>() => {{canHoldTypeExpr}};
            bool {{adapter}}.IsReadOnly => {{(isReadOnly ? "true" : "false")}};
            bool {{adapter}}.IsNullable => {{(isNullable ? "true" : "false")}};
            object? {{adapter}}.Value => Value;
            bool {{adapter}}.HasValue => {{(isNullable ? "HasValue" : "true")}};
            int {{adapter}}.TypeCount => {{entries.Length}};
            [{{aggressiveInlining}}]
            bool {{adapter}}.TrySetValue<Tx>(Tx value) {
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
            bool {{adapter}}.TryGetValue<Tx>(out Tx value) {
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
            bool {{adapter}}.TryClearValue() {
               {{(isReadOnly ? "return false;" : "ClearValue(); return true;")}}
            }
         """);
   skip_implement_visitor:
      sb.Append('}');
      var hintName = args.T;
      if (args.Namespace is not null) hintName = $"{args.Namespace}.{args.T}.g";
      hintName = hintName.Replace("global::", "").Replace("<", "{").Replace(">", "}").Replace(" ", "");
      return new(hintName, sb);
   }
}
