using System.Runtime.CompilerServices;
using System.Collections.Immutable;
using System.Diagnostics;
namespace UnionUtil.Internal.Meta;

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

   static Resolved Resolve(Compilation compilation, CancellationToken token) {
      var attr = compilation.Assembly.GetAttributes()
         .Where(static e => e.AttributeClass is { } c
            && c.Name is "UnionTypesConfigAttribute")
         .FirstOrDefault();
      if (attr is null) return default;

      var arity = (int)attr.ConstructorArguments[0].Value!;
      var @namespace = (string?)attr.ConstructorArguments[1].Value;
      if (attr.ConstructorArguments[2].Value is not string name) {
         throw new("name is null");
      }
      return new(arity, false, @namespace, name);
   }

   static bool Filter(SyntaxNode node, CancellationToken token) {
      if (node is not AttributeListSyntax als) return false;
      return als.Target?.Identifier.Kind() is SyntaxKind.AssemblyKeyword;
   }

   static void GenerateSource(SourceProductionContext ctx, Resolved ok) {
      const string T = "T";
      var typeParamsLength = ok.Arity * $"{T}X,".Length + 2;
      var sb = new StringBuilder(1024);
      sb.AppendLine("#nullable enable");
      sb.AppendLine("#pragma warning disable CS9113");
      if (ok.Namespace is string ns) {
         sb.AppendLine($"namespace {ns};");
      }
      sb.AppendLine($$"""
      public interface I{{ok.Name}} {
         bool TryGetValue<T>(out T value);
      }
      """);
      var typeParams = new StringBuilder(typeParamsLength);
      for (int n = 1; n <= ok.Arity; ++n) {
         typeParams.Append('<');
         static string ty(int i) => $"{T}{i}";
         for (int i = 1; i <= n; ++i) {
            typeParams.Append($"{ty(i)},");
         }
         typeParams[typeParams.Length - 1] = '>';
         sb.AppendLine($$"""
            public interface I{{ok.Name}}{{typeParams}} {
            """);
         for (int i = 1; i <= n; ++i) {
            sb.AppendLine($"""
               bool TryGetValue(out {ty(i)} value);
            """);
         }
         sb.AppendLine("}");
         if (ok.SkipAttributes) goto next;
         const string system = "global::System";
         const string attributeUsage = $"[{system}.AttributeUsage({system}.AttributeTargets.Struct | {system}.AttributeTargets.Class, AllowMultiple = false)]";
         sb.AppendLine(attributeUsage);
         sb.AppendLine($"public sealed class {ok.Name}Attribute{typeParams} : {system}.Attribute;");
      next:
         typeParams.Clear();
         continue;
      }

      var hintName = ok.Namespace != null
         ? $"{ok.Namespace}.{ok.Name}"
         : ok.Name;
      ctx.AddSource($"{hintName}{{{ok.Arity}}}.g", sb.ToString());
   }
}
