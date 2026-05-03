#pragma warning disable CS8618
global using UnionUtil.Meta;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;

[MemoryDiagnoser]
public class GeneratorBenchmark {
   CSharpCompilation _compilation;
   CSharpGeneratorDriver _driver;

   [GlobalSetup]
   public void Setup() {
      SyntaxTree[] syntaxTrees = [
         CSharpSyntaxTree.ParseText(Sources.Trivial),
         CSharpSyntaxTree.ParseText(Sources.NonTrivial),
      ];
      _compilation = CSharpCompilation.Create("Bench", syntaxTrees);

      _driver = CSharpGeneratorDriver.Create(new UnionGenerator());
   }

   [Benchmark]
   public GeneratorDriverRunResult Run() {
      _driver.RunGeneratorsAndUpdateCompilation(_compilation, out _, out var _);
      return _driver.GetRunResult();
   }
}
