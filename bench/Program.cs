global using BenchmarkDotNet.Attributes;
global using System.Runtime.CompilerServices;
global using BenchmarkDotNet.Jobs;
global using static System.Runtime.CompilerServices.MethodImplOptions;
global using UnionUtil;
using BenchmarkDotNet.Running;
// BenchmarkRunner.Run(typeof(Program).Assembly);
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
   .Run();
