#pragma warning disable CS8618

[MemoryDiagnoser]
public class IncrementalBenchmark {
   CSharpCompilation _compilation;
   CSharpGeneratorDriver _primed;

   [GlobalSetup]
   public void Setup() {
      _compilation = CSharpCompilation.Create("Bench", Sources.SyntaxTrees);
      var driver = CSharpGeneratorDriver.Create(new UnionImplGenerator());
      _primed = (CSharpGeneratorDriver)driver.RunGenerators(_compilation);
   }

   [Benchmark(Baseline = true)]
   public GeneratorDriverRunResult NoChange() {
      // Same compilation, nothing changed — should be near-zero work
      var driver = _primed.RunGenerators(_compilation);
      return driver.GetRunResult();
   }

   [Benchmark]
   public GeneratorDriverRunResult UnrelatedEdit() {
      // Add a file that has nothing to do with unions
      var updated = _compilation.AddSyntaxTrees(
         CSharpSyntaxTree.ParseText("class Foo { int x = 1; }"));
      var driver = _primed.RunGenerators(updated);
      return driver.GetRunResult();
   }

   [Benchmark]
   public GeneratorDriverRunResult RelevantEdit() {
      // Modify a union declaration — generator must re-run
      var newTree = CSharpSyntaxTree.ParseText(Sources.Many);
      var updated = _compilation.AddSyntaxTrees(newTree);
      var driver = _primed.RunGenerators(updated);
      return driver.GetRunResult();
   }
}
