#pragma warning disable CS8618
global using System.Collections.Immutable;
global using UnionUtil.Meta;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.Diagnostics;
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

      _driver = (CSharpGeneratorDriver)CSharpGeneratorDriver.Create(new UnionImplGenerator());
   }

   [Benchmark]
   public GeneratorDriverRunResult Run() {
      _driver.RunGeneratorsAndUpdateCompilation(_compilation, out _, out var diagnostics);
      return _driver.GetRunResult();
   }
}
