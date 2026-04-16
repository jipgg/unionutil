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
   public const string UnscopedRef = $"{CodeAnalysis}.UnscopedRef";
   public const string InteropServices = "global::System.Runtime.InteropServices";
   public const string InvalidOperationException = "global::System.InvalidOperationException";
   public const string UnionUtil = "global::UnionUtil";
   public const string GenericHelpers = $"{UnionUtil}.GenericHelpers";

   static string FieldName(in Type e) {
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
      var entries = ok.Types.entries;
      var visibility = ok.Visibility;
      var readonlyFieldMod = ok.Mutable ? " " : " readonly";
      var lref = $"{(ok.Mutable ? "ref " : " ")}";
      // string readonlyMethodMod = ok.Kind is SyntaxKind.StructDeclaration && !ok.Mutable ? " readonly" : " ";
      (int, string)[] getters = [];
      void writeConstructor(in Type arg, string assign) {
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
      void writeTryGetValue(in Type arg, string assign) {
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
      void writeSetValue(in Type arg, string assign) {
         if (ok.WithSetValue is false) return;
         sb.AppendLine($$"""
            [{{AggressiveInlining}}]
            public void SetValue({{arg.type}} v) {
               {{assign}}
               {{IndexField}} = {{arg.index}};
            }
         """);
      }
      void writeProperties(in Type e, string returnExpr) {
         // string propertyType = ok.Mutable ? $" ref {e.type}" : $"{readonlyMethodMod} {e.type}";
         string propertyType = ok.Mutable ? $" ref {e.type}" : $" {e.type}";
         // string propertyType = $" {e.type}";
         if (ok.Mutable) sb.AppendLine($"""
            [{UnscopedRef}]
         """);
         // var setter = ok.Mutable ? $$"""
         //
         //       set {
         //          if ({{IndexField}} is {{e.index}}) {
         //             {{returnExpr}} = value;
         //          } else {
         //             SetValue(value);
         //          }
         //       }
         // """ : "\n";
         sb.AppendLine($$"""
            public{{propertyType}} @{{ok.Labels[e]}} {
               [{{AggressiveInlining}}]
              {{readonlyMethodMod}} get {
                  if ({{IndexField}} != {{e.index}}) throw new {{InvalidOperationException}}($"type index is {{{IndexField}}} ({{ok.Labels[e]}}).");
                  {{returnExpr}};
               }
            }
            public{{readonlyMethodMod}} bool Is{{ok.Labels[e]}} {
               [{{AggressiveInlining}}]
               get => {{IndexField}} == {{e.index}};
            } 
         """);
      }
      Type[] toOverlap = [.. entries.Where(static e => e.strategy == Strategy.Overlap)];
      if (toOverlap.Length > 0) {
         sb.AppendLine($$"""
            [{{InteropServices}}.StructLayout({{InteropServices}}.LayoutKind.Explicit)]
            {{visibility}} struct {{OverlappedType}} {
         """);
         var fieldVis = visibility is "private" ? "internal" : visibility;
         foreach (var e in toOverlap) sb.AppendLine($"""
               [{InteropServices}.FieldOffset(0)]
               {fieldVis} {e.type} {FieldName(e)};
         """);
         sb.AppendLine($$"""
            }
            {{visibility}}{{readonlyFieldMod}} {{OverlappedType}} {{OverlappedField}} = default;
         """);
         foreach (var e in toOverlap) {
            getters = [.. getters, (e.index, $"{OverlappedField}.{FieldName(e)}")];
            writeConstructor(e, $"{OverlappedField} = new() {{{FieldName(e)} = v}};");
            writeTryGetValue(e, $"v = {OverlappedField}.{FieldName(e)};");
            if (ok.Mutable) writeSetValue(e, $"{OverlappedField}.{FieldName(e)} = v;");
            if (ok.Labels.Enabled) {
               var returnExpr = ok.Mutable
                  ? $"return ref {OverlappedField}.{FieldName((e))}"
                  : $"return {OverlappedField}.{FieldName(e)}";
               writeProperties(e, returnExpr);
            }
         }
      }
      Type[] toBox = [.. entries.Where(static e => e.strategy is Strategy.Box)];
      if (toBox.Length > 0) {
         sb.AppendLine($"  {visibility}{readonlyFieldMod} object? {ObjectField} = default;");
         foreach (var e in toBox) {
            if (e.kind is Kind.Generic && e.strategy is Strategy.Box) {
               writeConstructor(e, $"{ObjectField} = {GenericHelpers}.Box<{e.type}>(v)");
            } else writeConstructor(e, $"{ObjectField} = v;");
            var get = e.kind switch {
               Kind.Reference or Kind.Interface => $"{Unsafe}.As<{e.type}>({ObjectField}!)",
               Kind.Value or Kind.Unmanaged => $"{Unsafe}.Unbox<{e.type}>({ObjectField}!)",
               _ => $"{GenericHelpers}.Get<{e.type}>({ObjectField})",
            };
            getters = [.. getters, (e.index, ObjectField)];
            writeTryGetValue(e, $"v = {get};");
            if (ok.Mutable) writeSetValue(e, $"{ObjectField} = v;");
            var T = e.type;
            string returnExpr;
            if (ok.Mutable) {
               returnExpr = e.kind switch {
                  Kind.Interface or Kind.Reference => $"return ref {Unsafe}.As<{T},{T}>(ref {ObjectField}!)",
                  Kind.Value or Kind.Unmanaged => $"return ref {Unsafe}.Unbox<{T}>({ObjectField}!)",
                  _ => $"return ref {GenericHelpers}.Ref<{T}>(ref {ObjectField})",
                  // _ => $"return ref {Unsafe}.As<object?,{UnionUtil}.GenericBox<{T}>>(ref {ObjectField}).Item",
                  // _ => $"return ref global::UnionUtil.UnionHelpers.UnboxAny<{T}>(ref {ObjectField}!)",
               };
            } else {
               returnExpr = e.kind switch {
                  Kind.Interface or Kind.Reference => $"return {Unsafe}.As<{T}>({ObjectField})!",
                  Kind.Value or Kind.Unmanaged => $"return {ObjectField}.Unbox<{T}>({ObjectField}!)",
                  _ => $"return {GenericHelpers}.Get<{T}>({ObjectField})",
               };
            }
            if (ok.Labels.Enabled) {
               writeProperties(e, returnExpr);
            }
         }
      }
      Type[] sequential = [.. entries.Where(static e => e.strategy is Strategy.Sequential)];
      if (sequential.Length > 0) {
         foreach (var e in sequential) {
            var holder = FieldName(e);
            sb.AppendLine($"  {visibility}{readonlyFieldMod} {e.type} {holder} = default!;");
            getters = [.. getters, (e.index, holder)];
            writeConstructor(e, $"{holder} = v;");
            writeTryGetValue(e, $"v = {holder};");
            if (ok.Mutable) writeSetValue(e, $"{holder} = v;");
            if (ok.Labels.Enabled) {
               var returnExpr = ok.Mutable ? $"return ref {holder}" : $"return {holder}";
               writeProperties(e, returnExpr);
            }
         }
      }
      sb.AppendLine($"  {visibility}{readonlyFieldMod} byte {IndexField};");
      var obj = ok.Nullable ? "object?" : "object";
      sb.AppendLine($"  public {obj} Value => {IndexField} switch {{");
      foreach (var e in getters) sb.AppendLine($"    {e.Item1} => {e.Item2}!,");
      if (ok.Nullable) sb.AppendLine($"    _ => null,");
      else sb.AppendLine($"    _ => throw new global::System.InvalidOperationException($\"type index was {{{IndexField}}}\")");
      sb.AppendLine("  };");
      if (ok.Nullable) {
         sb.AppendLine($"  public bool HasValue => {IndexField} != 0;");
      }
      sb.Append('}');
      ctx.AddSource(ok.Name.HintName(), sb.ToString());
   }
}
