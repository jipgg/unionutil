#pragma warning disable CS8618

[MemoryDiagnoser]
public class IncrementalBenchmark {
   CSharpCompilation _compilation;
   CSharpGeneratorDriver _primed;

   [GlobalSetup]
   public void Setup() {
      _compilation = CSharpCompilation.Create("Bench", Sources.SyntaxTrees); var driver = CSharpGeneratorDriver.Create(new UnionImplGenerator());
      _primed = (CSharpGeneratorDriver)driver.RunGenerators(_compilation);
   }

   [Benchmark(Baseline = true)]
   public GeneratorDriverRunResult NoChange() {
      var driver = _primed.RunGenerators(_compilation);
      return driver.GetRunResult();
   }

   [Benchmark]
   public GeneratorDriverRunResult UnrelatedEdit() {
      var updated = _compilation.AddSyntaxTrees(
         CSharpSyntaxTree.ParseText("class Foo { int x = 1; }"));
      var driver = _primed.RunGenerators(updated);
      return driver.GetRunResult();
   }

   [Benchmark]
   public GeneratorDriverRunResult RelevantEdit() {
      var newTree = CSharpSyntaxTree.ParseText(Sources.Many);
      var updated = _compilation.AddSyntaxTrees(newTree);
      var driver = _primed.RunGenerators(updated);
      return driver.GetRunResult();
   }
}
