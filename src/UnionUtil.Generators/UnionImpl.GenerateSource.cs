using System.Diagnostics;
namespace UnionUtil;

partial class UnionImpl {
   const string _compilerServices = "global::System.Runtime.CompilerServices";
   const string _aggressiveInlining = $"{_compilerServices}.MethodImpl({_compilerServices}.MethodImplOptions.AggressiveInlining)";
   const string _unsafe = $"{_compilerServices}.Unsafe";
   const string _objectField = "_box";
   const string _overlappedField = "_overlap";
   const string _overlappedType = "OverlappedStorage";
   const string _sboField = "_sbo";
   const string _fallbackSboType = "SmallBufferStorage";
   const string _smallBufferInterface = $"{_unionUtil}.ISmallBuffer";
   const string _indexField = "_index";
   const string _codeAnalysis = "global::System.Diagnostics.CodeAnalysis";
   const string _unscopedRef = $"{_codeAnalysis}.UnscopedRef";
   const string _interopServices = "global::System.Runtime.InteropServices";
   const string _invalidOperationException = "global::System.InvalidOperationException";
   const string _unionUtil = "global::UnionUtil";

   static class GenericHelpers {
      const string prefix = $"global::UnionUtil.OpenGenericHelpers";
      public static string Box(string type, string arg) => $"{prefix}.Box<{type}>({arg})";
      public static string SboBox(string type, string sboType, string sbo, string obj, string arg)
         => $"{prefix}.Box<{type},{sboType}>(ref {_unsafe}.AsRef(in {sbo}), ref {obj}, {arg})";
      public static string Ref(string type, string arg) => $"{prefix}.Ref<{type}>(ref {arg})";
      public static string SboRef(string type, string sboType, string sbo, string obj) => $"{prefix}.Ref<{type},{sboType}>(ref {sbo}, ref {obj})";
      public static string Get(string type, string arg) => $"{prefix}.Get<{type}>({arg})";
      public static string SboGet(string type, string sboType, string sbo, string obj) => $"{prefix}.Get<{type},{sboType}>(ref {_unsafe}.AsRef(in {sbo}), {obj})";
   }
   static string FieldName(in TypeArg e) {
      return $"_{e.index}";
   }
   static void GenerateSource(SourceProductionContext ctx, (Resolved?, Problem[]) result) {
      var (ok, err) = result;
      if (err.Length > 0) {
         foreach (var e in err) {
            var diag = Diagnostic.Create(Descriptor, e.Location, e.Message);
            ctx.ReportDiagnostic(diag);
         }
         return;
      }
      var sb = new StringBuilder(2048);
      sb.AppendLine("#nullable enable");
      if (ok!.Name.Namespace is string ns) {
         sb.AppendLine($"namespace {ns};");
      }
      sb.Append("partial ");
      string ro;
      switch (ok.Kind) {
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
      sb.Append(ok.Name.GenericName()).AppendLine(" {");
      var entries = ok.TypeArgs.entries;
      var visibility = ok.Visibility;
      var readonlyFieldMod = ok.Mutable ? " " : " readonly";
      var lref = $"{(ok.Mutable ? "ref " : " ")}";
      (int, string)[] getValueExprs = [];
      (int, string)[] clearExprs = [];
      void writeConstructor(in TypeArg arg, string assign) {
         sb.AppendLine($$"""
            [{{_aggressiveInlining}}]
            public {{ok.Name.Type}}({{arg.type}} v) {
               {{assign}};
               {{_indexField}} = {{arg.index}};
            }
         """);
         if (arg.kind is not Kind.Interface) sb.AppendLine($$"""
            [{{_aggressiveInlining}}]
            public static implicit operator {{ok.Name.GenericName()}}({{arg.type}} v) => new(v);
         """);
      }
      void writeTryGetValue(in TypeArg arg, string assign) {
         sb.AppendLine($$"""
            [{{_aggressiveInlining}}]
            public{{ro}} bool TryGetValue(out {{arg.type}} v) {
               if ({{_indexField}} == {{arg.index}}) {
                  {{assign}}
                  return true;
               } else {
                  v = default!;
                  return false;
               }
            }
         """);
      }
      void writeSetValue(in TypeArg arg, string assign) {
         if (ok.Mutable is false) return;
         sb.AppendLine($$"""
            [{{_aggressiveInlining}}]
            public void SetValue({{arg.type}} v) {
               if ({{_indexField}} is {{arg.index}}) {
                  {{assign}} = v;
                  return;
               }
               ClearValue();
               {{assign}} = v;
               {{_indexField}} = {{arg.index}};
            }
         """);
      }
      void writeProperties(in TypeArg e, string refExpr) {
         if (ok.Tagged is not Tagged tagged) return;
         var setter = ok.Mutable ? $$"""
               [{{_aggressiveInlining}}]
               set => SetValue(value);
         """ : "\n";
         sb.AppendLine($$"""
            public{{(ok.Mutable ? " " : ro)}} {{e.type}} {{tagged[e]}} {
               [{{_aggressiveInlining}}]
              {{(ok.Mutable ? ro : " ")}} get {
                  if ({{_indexField}} != {{e.index}}) throw new {{_invalidOperationException}}($"type index is {{{_indexField}}} ({{tagged[e]}}).");
                  return {{refExpr}};
               }
               {{setter}}
            }
         """);
      }
      TypeArg[] toOverlap = [.. entries.Where(static e => e.strategy == Strategy.Overlap)];
      TypeArg[] toBox = [.. entries.Where(static e => e.strategy is Strategy.Box)];
      bool sboEnabled = ok.Sbo is not null && toBox.Any(static e => e.kind is Kind.Generic or Kind.Value && e.strategy is Strategy.Box);
      // setting up the fields first
      if (toBox.Length is not 0) {
         sb.AppendLine($"   {visibility}{readonlyFieldMod} object? {_objectField} = default;");
      }
      sb.AppendLine($"   {visibility}{readonlyFieldMod} byte {_indexField};");
      string? TSbo = default;
      if (sboEnabled) {
         var size = ok.Sbo!.Value.Size;
         TSbo = size switch {
            7 or 15 or 23 => $"{_unionUtil}.SmallBuffer{size}",
            _ => _fallbackSboType,
         };
         sb.AppendLine($"   {visibility}{readonlyFieldMod} {TSbo} {_sboField} = default;");
      }
      if (toOverlap.Length is 0) goto skip_to_overlap;
      sb.AppendLine($$"""
            [{{_interopServices}}.StructLayout({{_interopServices}}.LayoutKind.Explicit)]
            {{visibility}} struct {{_overlappedType}} {
         """);
      var fieldVisibility = visibility is "private" ? "internal" : visibility;
      foreach (var e in toOverlap) sb.AppendLine($"""
               [{_interopServices}.FieldOffset(0)]
               {fieldVisibility} {e.type} {FieldName(e)};
         """);
      sb.AppendLine($$"""
            }
            {{visibility}}{{readonlyFieldMod}} {{_overlappedType}} {{_overlappedField}} = default;
         """);
      foreach (var e in toOverlap) {
         getValueExprs = [.. getValueExprs, (e.index, $"{_overlappedField}.{FieldName(e)}")];
         writeConstructor(e, $"{_overlappedField} = new() {{{FieldName(e)} = v}};");
         writeTryGetValue(e, $"v = {_overlappedField}.{FieldName(e)};");
         writeSetValue(e, $"{_overlappedField}.{FieldName(e)}");
         writeProperties(e, $"{_overlappedField}.{FieldName(e)}");
      }
   skip_to_overlap:
      if (toBox.Length is 0) goto skip_to_box;
      if (TSbo is _fallbackSboType) {
         var sbo = ok.Sbo!.Value;
         sb.AppendLine($$"""
            [{{_compilerServices}}.InlineArray({{sbo.Size}})]
            {{visibility}} struct {{_fallbackSboType}}: {{_smallBufferInterface}} {
               byte _element0;
               public static int Size {
                  [{{_aggressiveInlining}}]
                  get => {{sbo.Size}};
               }
               public ref byte Data {
                  [{{_unscopedRef}}]
                  [{{_aggressiveInlining}}]
                  get => ref _element0;
               }
            }
         """);
      }
      foreach (var e in toBox) {
         var T = e.type;
         if (e.kind is Kind.Generic && e.strategy is Strategy.Box) {
            if (TSbo is null) writeConstructor(e, $"{_objectField} = {GenericHelpers.Box(T, "v")}");
            else writeConstructor(e, $"{GenericHelpers.SboBox(T, TSbo, _sboField, _objectField, "v")}");
         } else writeConstructor(e, $"{_objectField} = v;");
         var getExpr = e.kind switch {
            Kind.Reference or Kind.Interface => $"{_unsafe}.As<{T}>({_objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{_unsafe}.Unbox<{T}>({_objectField}!)",
            _ => TSbo is not null ? GenericHelpers.SboGet(T, TSbo, _sboField, _objectField) : GenericHelpers.Get(T, _objectField),
         };
         var refExpr = e.kind switch {
            Kind.Interface or Kind.Reference => $"{_unsafe}.As<object?,{T}>(ref {_objectField}!)",
            Kind.Value or Kind.Unmanaged => $"{_unsafe}.Unbox<{T}>({_objectField}!)",
            _ => TSbo is not null ? GenericHelpers.SboRef(T, TSbo, _sboField, _objectField) : GenericHelpers.Ref(T, _objectField),
         };
         writeProperties(e, getExpr);
         writeTryGetValue(e, $"v = {getExpr};");
         writeSetValue(e, refExpr);
         if (!ok.Sbo.HasValue) {
            clearExprs = [.. clearExprs, (e.index, $"{_objectField} = null")];
         }
         getValueExprs = [.. getValueExprs, (e.index, TSbo is not null ? GenericHelpers.SboGet(e.type, TSbo, _sboField, _objectField) : _objectField)];
      }
   skip_to_box:
      TypeArg[] sequential = [.. entries.Where(static e => e.strategy is Strategy.Sequential)];
      if (sequential.Length is 0) goto skip_sequential;
      foreach (var e in sequential) {
         var T = e.type;
         var holder = FieldName(e);
         sb.AppendLine($"  {visibility}{readonlyFieldMod} {T} {holder} = default!;");
         getValueExprs = [.. getValueExprs, (e.index, holder)];
         writeConstructor(e, $"{holder} = v;");
         writeTryGetValue(e, $"v = {holder};");
         if (ok.Mutable) {
            writeSetValue(e, holder);
            clearExprs = [.. clearExprs, (e.index, $"{holder} = default!")];
         }
         writeProperties(e, holder);
      }
   skip_sequential:
      var obj = ok.Nullable ? "object?" : "object";
      sb.AppendLine($"  public{ro} {obj} Value => {_indexField} switch {{");
      foreach (var e in getValueExprs) sb.AppendLine($"    {e.Item1} => {e.Item2}!,");
      if (ok.Nullable) sb.AppendLine($"    0 => null,");
      sb.AppendLine($"    _ => throw new global::System.InvalidOperationException($\"type index was {{{_indexField}}}\")");
      sb.AppendLine("  };");
      if (ok.Nullable) {
         sb.AppendLine($"  public{ro} bool HasValue => {_indexField} != 0;");
      }
      sb.AppendLine($$"""
         [{{_aggressiveInlining}}]
         public{{ro}} bool Is<Tx>() => {{_indexField}} switch {
      """);
      foreach (var e in entries) sb.AppendLine($"      {e.index} => typeof(Tx) == typeof({e.type}),");
      sb.AppendLine($$"""
            _ => false,
         };
         [{{_aggressiveInlining}}]
         public{{ro}} bool Is(byte typeIndex) => {{_indexField}} == typeIndex;
      """);
      if (ok.Mutable) {
         sb.AppendLine($$"""
            [{{_aggressiveInlining}}]
            public void ClearValue() {
         """);
         if (clearExprs.Length is 0) {
            sb.AppendLine("}");
            goto clear_value_done;
         }
         sb.AppendLine($$"""
               switch ({{_indexField}}) {
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
      }
   clear_value_done:
      if (ok.Tagged is not { } tagged || tagged.Enum is not {} @enum) {
         goto tag_getter_done;
      }
      if (ok.Nullable) @enum += "?";

      sb.AppendLine($$"""
         public{{ro}} {{@enum}} {{tagged.Name}} {
            [{{_aggressiveInlining}}]
      """);
      if (tagged.Dense) {
         var expr = $"(({@enum}){_indexField} - 1)";
         if (ok.Nullable) expr = $"{_indexField} is 0 ? null : {expr}";
         sb.AppendLine($$"""
               get => {{expr}};
            }
         """);
         goto tag_getter_done;
      }
      sb.AppendLine($$"""
           get => {{_indexField}} switch {
      """);
      foreach (var e in entries) sb.AppendLine($$"""
               {{e.index}} => {{tagged.Enum}}.{{tagged[e]}},
      """);
      if (ok.Nullable) sb.AppendLine($"""
               0 => null,
      """);
      var genericName = ok.Name.GenericName();
      sb.AppendLine($$"""
               _ => throw new {{_invalidOperationException}}("invalid tag " + {{_indexField}}.ToString()),
            };
         }
      """);
   tag_getter_done:
      sb.Append('}');
      ctx.AddSource(ok.Name.HintName(), sb.ToString());
   }
}
