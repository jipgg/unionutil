```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method           | Job            | Runtime        | Mean     | Error    | StdDev   | Gen0   | Allocated |
|----------------- |--------------- |--------------- |---------:|---------:|---------:|-------:|----------:|
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 | 11.66 ns | 0.106 ns | 0.100 ns | 0.0166 |     104 B |
| SequentialTagged | .NET 10.0      | .NET 10.0      | 12.48 ns | 0.137 ns | 0.122 ns |      - |         - |
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 | 17.41 ns | 0.094 ns | 0.079 ns |      - |         - |
| Sequential       | .NET 10.0      | .NET 10.0      | 18.22 ns | 0.336 ns | 0.314 ns |      - |         - |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 19.62 ns | 0.413 ns | 0.386 ns | 0.0166 |     104 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 | 21.26 ns | 0.304 ns | 0.284 ns | 0.0255 |     160 B |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 22.22 ns | 0.397 ns | 0.332 ns | 0.0357 |     224 B |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 22.53 ns | 0.329 ns | 0.308 ns | 0.0255 |     160 B |
| Sbo15            | .NET 10.0      | .NET 10.0      | 24.72 ns | 0.185 ns | 0.173 ns | 0.0255 |     160 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 26.27 ns | 0.411 ns | 0.364 ns | 0.0255 |     160 B |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 | 26.28 ns | 0.436 ns | 0.341 ns |      - |         - |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 26.55 ns | 0.489 ns | 0.457 ns | 0.0255 |     160 B |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 28.37 ns | 0.433 ns | 0.384 ns | 0.0255 |     160 B |
| Sbo31            | .NET 10.0      | .NET 10.0      | 30.49 ns | 0.086 ns | 0.067 ns | 0.0255 |     160 B |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 31.43 ns | 0.461 ns | 0.409 ns | 0.0255 |     160 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 31.91 ns | 0.252 ns | 0.197 ns | 0.0357 |     224 B |
| Sbo23            | .NET 10.0      | .NET 10.0      | 32.30 ns | 0.254 ns | 0.212 ns | 0.0255 |     160 B |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 32.76 ns | 0.692 ns | 0.741 ns | 0.0255 |     160 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 | 34.82 ns | 0.342 ns | 0.320 ns | 0.0408 |     256 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 | 35.01 ns | 0.607 ns | 0.538 ns | 0.0433 |     272 B |
| Sbo7             | .NET 10.0      | .NET 10.0      | 36.81 ns | 0.329 ns | 0.275 ns | 0.0255 |     160 B |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 37.51 ns | 0.513 ns | 0.480 ns | 0.0255 |     160 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 39.33 ns | 0.769 ns | 0.682 ns | 0.0510 |     320 B |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 47.26 ns | 0.389 ns | 0.345 ns | 0.0408 |     256 B |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 49.38 ns | 0.659 ns | 0.584 ns | 0.0433 |     272 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 53.27 ns | 0.666 ns | 0.623 ns | 0.0509 |     320 B |
