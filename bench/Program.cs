global using BenchmarkDotNet.Attributes;
global using System.Runtime.CompilerServices;
global using BenchmarkDotNet.Jobs;
global using static System.Runtime.CompilerServices.MethodImplOptions;
global using BenchmarkDotNet.Order;
global using UnionUtil;
global using static UnionUtil.UnionGeneratorOptions;
using BenchmarkDotNet.Running;
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
