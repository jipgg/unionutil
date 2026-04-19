```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method           | Job            | Runtime        | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated |
|----------------- |--------------- |--------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
| Sequential       | .NET 10.0      | .NET 10.0      |  0.0000 ns | 0.0000 ns | 0.0000 ns |  0.0000 ns |      - |         - |
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.4866 ns | 0.0051 ns | 0.0043 ns |  0.4845 ns |      - |         - |
| Boxed            | .NET 10.0      | .NET 10.0      |  0.5219 ns | 0.0697 ns | 0.1343 ns |  0.4528 ns |      - |         - |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  1.0352 ns | 0.0026 ns | 0.0020 ns |  1.0349 ns |      - |         - |
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  1.0382 ns | 0.0080 ns | 0.0062 ns |  1.0360 ns |      - |         - |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 |  1.9753 ns | 0.0011 ns | 0.0008 ns |  1.9753 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  3.4967 ns | 0.0384 ns | 0.0321 ns |  3.4849 ns | 0.0038 |      24 B |
| Sbo15            | .NET 10.0      | .NET 10.0      |  6.2461 ns | 0.0302 ns | 0.0268 ns |  6.2308 ns |      - |         - |
| UnionBaseline    | .NET 10.0      | .NET 10.0      |  7.2136 ns | 0.0879 ns | 0.0734 ns |  7.1692 ns | 0.0038 |      24 B |
| Sbo23            | .NET 10.0      | .NET 10.0      |  7.6224 ns | 0.0178 ns | 0.0158 ns |  7.6149 ns |      - |         - |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 |  8.1928 ns | 0.0351 ns | 0.0293 ns |  8.1776 ns |      - |         - |
| BoxedTagged      | .NET 10.0      | .NET 10.0      |  8.5661 ns | 0.0210 ns | 0.0187 ns |  8.5591 ns |      - |         - |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 |  9.5801 ns | 0.0032 ns | 0.0025 ns |  9.5799 ns |      - |         - |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 10.1845 ns | 0.2008 ns | 0.1878 ns | 10.1201 ns |      - |         - |
| Sbo7             | .NET 10.0      | .NET 10.0      | 13.3071 ns | 0.1494 ns | 0.1324 ns | 13.2635 ns |      - |         - |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 14.6511 ns | 0.0367 ns | 0.0286 ns | 14.6398 ns |      - |         - |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 | 17.9449 ns | 0.1175 ns | 0.0981 ns | 17.9121 ns | 0.0153 |      96 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 | 19.0021 ns | 0.1491 ns | 0.1322 ns | 19.0402 ns | 0.0179 |     112 B |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 19.4023 ns | 0.1110 ns | 0.0927 ns | 19.3939 ns | 0.0153 |      96 B |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 20.5766 ns | 0.1799 ns | 0.1595 ns | 20.5976 ns | 0.0179 |     112 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 22.4260 ns | 0.1541 ns | 0.1442 ns | 22.4489 ns | 0.0255 |     160 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 23.8296 ns | 0.2913 ns | 0.2724 ns | 23.7013 ns | 0.0255 |     160 B |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 25.8488 ns | 0.4607 ns | 0.3847 ns | 25.7917 ns | 0.0357 |     224 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 28.9512 ns | 0.1357 ns | 0.1133 ns | 28.9002 ns | 0.0357 |     224 B |
| Sbo31            | .NET 10.0      | .NET 10.0      | 30.0070 ns | 0.0217 ns | 0.0193 ns | 30.0107 ns |      - |         - |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 34.6023 ns | 0.0065 ns | 0.0051 ns | 34.6001 ns |      - |         - |
