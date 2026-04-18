using System.Diagnostics;
namespace UnionUtil;

partial class UnionImpl {
   public const string CompilerServices = "global::System.Runtime.CompilerServices";
   public const string AggressiveInlining = $"{CompilerServices}.MethodImpl({CompilerServices}.MethodImplOptions.AggressiveInlining)";
   public const string Unsafe = $"{CompilerServices}.Unsafe";
   public const string ObjectField = "_object";
   public const string OverlappedField = "_overlapped";
   public const string OverlappedType = "Overlapped";
   public const string IndexField = "_index";
   public const string CodeAnalysis = "global::System.Diagnostics.CodeAnalysis";
   public const string InteropServices = "global::System.Runtime.InteropServices";
   public const string InvalidOperationException = "global::System.InvalidOperationException";
   public const string UnionUtil = "global::UnionUtil";

   static class GenericHelpers {
      const string prefix = $"global::UnionUtil.OpenGenericHelpers";
      public static string Box(string type, string arg) => $"{prefix}.Box<{type}>({arg})";
      public static string SboBox(string type, string sbo, string obj, string arg) => $"{prefix}.Box<{type}>({sbo}, ref {obj}, {arg})";
      public static string Ref(string type, string arg) => $"{prefix}.Ref<{type}>(ref {arg})";
      public static string SboRef(string type, string sbo, string obj) => $"{prefix}.Ref<{type}>({sbo}, ref {obj})";
      public static string Get(string type, string arg) => $"{prefix}.Get<{type}>({arg})";
      public static string SboGet(string type, string sbo, string obj) => $"{prefix}.Get<{type}>({sbo}, {obj})";
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
      string ro, readonlyMethodMod;
      switch (ok.Kind) {
         case SyntaxKind.StructDeclaration:
            sb.Append("struct ");
            ro = " readonly";
            if (!ok.Mutable) readonlyMethodMod = " readonly";
            else readonlyMethodMod = " ";
            break;
         case SyntaxKind.ClassDeclaration:
            sb.Append("class ");
            ro = " ";
            readonlyMethodMod = " ";
            break;
         default:
            throw new InvalidOperationException();
      }
      sb.Append(ok.Name.GenericName()).AppendLine(" {");
      var entries = ok.TypeArgs.entries;
      var visibility = ok.Visibility;
      var readonlyFieldMod = ok.Mutable ? " " : " readonly";
      var lref = $"{(ok.Mutable ? "ref " : " ")}";
      (int, string)[] getExprs = [];
      (int, string)[] clearExprs = [];
      void writeConstructor(in TypeArg arg, string assign) {
         sb.AppendLine($$"""
            [{{AggressiveInlining}}]
            public {{ok.Name.Type}}({{arg.type}} v) {
               {{assign}};
               {{IndexField}} = {{arg.index}};
            }
         """);
         if (arg.kind is not Kind.Interface) sb.AppendLine($$"""
            [{{AggressiveInlining}}]
            public static implicit operator {{ok.Name.GenericName()}}({{arg.type}} v) => new(v);
         """);
      }
      void writeTryGetValue(in TypeArg arg, string assign) {
         sb.AppendLine($$"""
            [{{AggressiveInlining}}]
            public{{ro}} bool TryGetValue(out {{arg.type}} v) {
               if ({{IndexField}} == {{arg.index}}) {
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
            [{{AggressiveInlining}}]
            public void SetValue({{arg.type}} v) {
               if ({{IndexField}} is {{arg.index}}) {
                  {{assign}} = v;
                  return;
               }
               ClearValue();
               {{assign}} = v;
               {{IndexField}} = {{arg.index}};
            }
         """);
      }
      void writeProperties(in TypeArg e, string refExpr) {
         if (ok.Tagged is not Tagged tagged) return;
         string propertyType = $" {e.type}";
         var setter = ok.Mutable ? $$"""
               [{{AggressiveInlining}}]
               set => SetValue(value);
         """ : "\n";
         sb.AppendLine($$"""
            public {{e.type}} @{{tagged[e]}} {
               [{{AggressiveInlining}}]
              {{readonlyMethodMod}} get {
                  if ({{IndexField}} != {{e.index}}) throw new {{InvalidOperationException}}($"type index is {{{IndexField}}} ({{tagged[e]}}).");
                  return {{refExpr}};
               }
               {{setter}}
            }
         """);
      }
      TypeArg[] toOverlap = [.. entries.Where(static e => e.strategy == Strategy.Overlap)];
      if (toOverlap.Length is 0) goto skip_to_overlap;
      sb.AppendLine($$"""
            [{{InteropServices}}.StructLayout({{InteropServices}}.LayoutKind.Explicit)]
            {{visibility}} struct {{OverlappedType}} {
         """);
      var fieldVisibility = visibility is "private" ? "internal" : visibility;
      foreach (var e in toOverlap) sb.AppendLine($"""
               [{InteropServices}.FieldOffset(0)]
               {fieldVisibility} {e.type} {FieldName(e)};
         """);
      sb.AppendLine($$"""
            }
            {{visibility}}{{readonlyFieldMod}} {{OverlappedType}} {{OverlappedField}} = default;
         """);
      foreach (var e in toOverlap) {
         getExprs = [.. getExprs, (e.index, $"{OverlappedField}.{FieldName(e)}")];
         writeConstructor(e, $"{OverlappedField} = new() {{{FieldName(e)} = v}};");
         writeTryGetValue(e, $"v = {OverlappedField}.{FieldName(e)};");
         writeSetValue(e, $"{OverlappedField}.{FieldName(e)}");
         writeProperties(e, $"{OverlappedField}.{FieldName(e)}");
      }
   skip_to_overlap:
      TypeArg[] toBox = [.. entries.Where(static e => e.strategy is Strategy.Box)];
      if (toBox.Length is 0) goto skip_to_box;
      sb.AppendLine($"  {visibility}{readonlyFieldMod} object? {ObjectField} = default;");
      if (ok.Sbo is { } sbo && toBox.Any(e => e.kind is Kind.Generic)) {
         sb.AppendLine($$"""
            [{{CompilerServices}}.InlineArray({{sbo.Size}})]
            {{visibility}} struct SmallBufferStorage {
               byte _element0;
            }
            {{visibility}}{{readonlyFieldMod}} SmallBufferStorage _sbo = default;
         """);
      }
      foreach (var e in toBox) {
         if (e.kind is Kind.Generic && e.strategy is Strategy.Box) {
            if (ok.Sbo is not null) {
               writeConstructor(e, $"{GenericHelpers.SboBox(e.type, "_sbo", ObjectField, "v")}");
            } else {
               writeConstructor(e, $"{ObjectField} = {GenericHelpers.Box(e.type, "v")}");
            }
         } else writeConstructor(e, $"{ObjectField} = v;");
         var get = e.kind switch {
            Kind.Reference or Kind.Interface => $"{Unsafe}.As<{e.type}>({ObjectField}!)",
            Kind.Value or Kind.Unmanaged => $"{Unsafe}.Unbox<{e.type}>({ObjectField}!)",
            _ => ok.Sbo.HasValue ? GenericHelpers.SboGet(e.type, "_sbo", ObjectField) : GenericHelpers.Get(e.type, ObjectField),
         };
         getExprs = [.. getExprs, (e.index, ok.Sbo.HasValue ? GenericHelpers.SboGet(e.type, "_sbo", ObjectField) : ObjectField)];
         writeTryGetValue(e, $"v = {get};");
         var T = e.type;
         string refExpr;
         refExpr = e.kind switch {
            Kind.Interface or Kind.Reference => $"{Unsafe}.As<object?,{T}>(ref {ObjectField}!)",
            Kind.Value or Kind.Unmanaged => $"{Unsafe}.Unbox<{T}>({ObjectField}!)",
            _ => ok.Sbo.HasValue ? GenericHelpers.SboRef(T, "_sbo", ObjectField) : GenericHelpers.Ref(T, ObjectField),
         };
         writeSetValue(e, refExpr);
         if (!ok.Sbo.HasValue) {
            clearExprs = [.. clearExprs, (e.index, $"{ObjectField} = null")];
         }
         writeProperties(e, refExpr);
      }
   skip_to_box:
      TypeArg[] sequential = [.. entries.Where(static e => e.strategy is Strategy.Sequential)];
      if (sequential.Length is 0) goto skip_sequential;
      foreach (var e in sequential) {
         var holder = FieldName(e);
         sb.AppendLine($"  {visibility}{readonlyFieldMod} {e.type} {holder} = default!;");
         getExprs = [.. getExprs, (e.index, holder)];
         writeConstructor(e, $"{holder} = v;");
         writeTryGetValue(e, $"v = {holder};");
         if (ok.Mutable) {
            writeSetValue(e, holder);
            clearExprs = [.. clearExprs, (e.index, $"{holder} = default!")];
         }
         writeProperties(e, holder);
      }
   skip_sequential:
      sb.AppendLine($"  {visibility}{readonlyFieldMod} byte {IndexField};");
      var obj = ok.Nullable ? "object?" : "object";
      sb.AppendLine($"  public {obj} Value => {IndexField} switch {{");
      foreach (var e in getExprs) sb.AppendLine($"    {e.Item1} => {e.Item2}!,");
      if (ok.Nullable) sb.AppendLine($"    0 => null,");
      sb.AppendLine($"    _ => throw new global::System.InvalidOperationException($\"type index was {{{IndexField}}}\")");
      sb.AppendLine("  };");
      if (ok.Nullable) {
         sb.AppendLine($"  public bool HasValue => {IndexField} != 0;");
      }
      if (ok.Mutable) {
         sb.AppendLine($$"""
            [{{AggressiveInlining}}]
            public void ClearValue() {
         """);
         if (clearExprs.Length is 0) {
            sb.AppendLine("}");
            goto clear_value_done;
         }
         sb.AppendLine($$"""
               switch ({{IndexField}}) {
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
      if (ok.Tagged is { } tagged && tagged.Enum is not null) {
         var tag = tagged.Enum;
         if (ok.Nullable) tag += "?";
         sb.AppendLine($$"""
         public {{tag}} {{tagged.Name}} {
            [{{AggressiveInlining}}]
           {{readonlyMethodMod}} get => {{IndexField}} switch {
      """);
         foreach (var e in entries) sb.AppendLine($$"""
               {{e.index}} => {{tagged.Enum}}.{{tagged[e]}},
      """);
         if (ok.Nullable) sb.AppendLine($"""
               0 => null,
      """);
         var genericName = ok.Name.GenericName();
         sb.AppendLine($$"""
               _ => throw new {{InvalidOperationException}}("invalid tag " + {{IndexField}}.ToString()),
            };
         }
      """);
      }
      sb.Append('}');
      ctx.AddSource(ok.Name.HintName(), sb.ToString());
   }
}
