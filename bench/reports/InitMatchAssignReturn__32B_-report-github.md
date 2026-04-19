```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method           | Job            | Runtime        | Mean       | Error     | StdDev    | Gen0   | Allocated |
|----------------- |--------------- |--------------- |-----------:|----------:|----------:|-------:|----------:|
| Sequential       | .NET 10.0      | .NET 10.0      |  0.4811 ns | 0.0180 ns | 0.0150 ns |      - |         - |
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.4850 ns | 0.0092 ns | 0.0082 ns |      - |         - |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  1.2916 ns | 0.0026 ns | 0.0022 ns |      - |         - |
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  2.1932 ns | 0.0131 ns | 0.0102 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  8.3141 ns | 0.0921 ns | 0.0769 ns | 0.0115 |      72 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 | 11.3679 ns | 0.0506 ns | 0.0423 ns | 0.0153 |      96 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 14.6926 ns | 0.3241 ns | 0.3032 ns | 0.0255 |     160 B |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 15.7642 ns | 0.0855 ns | 0.0714 ns | 0.0115 |      72 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 16.4763 ns | 0.1015 ns | 0.0900 ns | 0.0153 |      96 B |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 17.0241 ns | 0.3085 ns | 0.2885 ns | 0.0153 |      96 B |
| Sbo15            | .NET 10.0      | .NET 10.0      | 17.4192 ns | 0.0692 ns | 0.0614 ns | 0.0153 |      96 B |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 17.6707 ns | 0.0864 ns | 0.0675 ns | 0.0153 |      96 B |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 20.0351 ns | 0.4451 ns | 0.4163 ns | 0.0357 |     224 B |
| Sbo31            | .NET 10.0      | .NET 10.0      | 22.3824 ns | 0.0946 ns | 0.0885 ns | 0.0153 |      96 B |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 22.7487 ns | 0.1717 ns | 0.1522 ns | 0.0153 |      96 B |
| Sbo23            | .NET 10.0      | .NET 10.0      | 23.3916 ns | 0.2391 ns | 0.2119 ns | 0.0153 |      96 B |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 23.4050 ns | 0.4866 ns | 0.5206 ns | 0.0153 |      96 B |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 24.5316 ns | 0.5199 ns | 0.4863 ns | 0.0153 |      96 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 24.5384 ns | 0.2761 ns | 0.2305 ns | 0.0255 |     160 B |
| Sbo7             | .NET 10.0      | .NET 10.0      | 27.1842 ns | 0.1817 ns | 0.1611 ns | 0.0153 |      96 B |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 30.1022 ns | 0.6357 ns | 1.0444 ns | 0.0153 |      96 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 | 31.4541 ns | 0.7378 ns | 0.6901 ns | 0.0306 |     192 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 | 31.8843 ns | 0.5480 ns | 0.5126 ns | 0.0331 |     208 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 35.7468 ns | 0.1753 ns | 0.1554 ns | 0.0357 |     224 B |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 41.7439 ns | 0.8891 ns | 0.8316 ns | 0.0306 |     192 B |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 43.7098 ns | 0.2336 ns | 0.2071 ns | 0.0331 |     208 B |
