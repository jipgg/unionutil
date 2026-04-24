using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using System.Diagnostics;
using UnionUtil.Meta;
namespace UnionUtil.Internal.Meta;
using static Config;

[Generator(LanguageNames.CSharp)]
public sealed class UnionTypesConfigGenerator : IIncrementalGenerator {
   public void Initialize(IncrementalGeneratorInitializationContext ctx) {
      ctx.RegisterSourceOutput(
         ctx.CompilationProvider.Select(Resolve),
         GenerateSource
      );
   }
   readonly record struct Resolved(
      int Arity,
      bool SkipAttributes,
      string? Namespace,
      string Name
   );

   static bool Resolve(Compilation compilation, CancellationToken token) {
      var attr = compilation.Assembly.GetAttributes()
         .Where(static e => e.AttributeClass is { } c
            && c.Name is "GenerateUnionTypesFromConfigAttribute")
         .FirstOrDefault();
      return attr is not null;
   }

   static void GenerateSource(SourceProductionContext ctx, bool ok) {
      if (!ok) return;
      const string T = "T";
      var typeParamsLength = Config.UnionType.Arity * $"{T}X,".Length + 2;
      var sb = new StringBuilder(1024);
      sb.AppendLine("#nullable enable");
      sb.AppendLine("#pragma warning disable CS9113");
      if (UnionType.Namespace is string ns) {
         sb.AppendLine($"namespace {ns};");
      }
      // sb.AppendLine($$"""
      // public interface {{Config.AdapterName}} {
      //    bool CanHoldType<T>();
      //    bool HoldsType<T>();
      //    bool IsReadOnly {get;}
      //    bool IsNullable {get;}
      //    object? Value {get;}
      //    bool HasValue {get;}
      //    int TypeCount {get;}
      //    bool TrySetValue<T>(T value);
      //    bool TryGetValue<T>(out T value);
      //    bool TryClearValue();
      // }
      // """);
      var typeParams = new StringBuilder(typeParamsLength);
      for (int n = 1; n <= UnionType.Arity; ++n) {
         typeParams.Append('<');
         static string ty(int i) => $"{T}{i}";
         for (int i = 1; i <= n; ++i) {
            typeParams.Append($"{ty(i)},");
         }
         typeParams[typeParams.Length - 1] = '>';
         sb.AppendLine($$"""
            public interface {{UnionType.InterfaceName}}{{typeParams}} {
            """);
         for (int i = 1; i <= n; ++i) {
            sb.AppendLine($"""
               bool TryGetValue(out {ty(i)} value);
            """);
         }
         sb.AppendLine("}");
         const string system = "global::System";
         const string attributeUsage = $"[{system}.AttributeUsage({system}.AttributeTargets.Struct | {system}.AttributeTargets.Class, AllowMultiple = false)]";
         sb.AppendLine(attributeUsage);
         sb.AppendLine($"public sealed class {UnionType.AttributeName}{typeParams} : {system}.Attribute;");
         typeParams.Clear();
         continue;
      }

      ctx.AddSource($"UnionTypes.g", sb.ToString());
   }
}
