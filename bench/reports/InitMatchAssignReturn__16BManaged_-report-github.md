```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method           | Job            | Runtime        | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated |
|----------------- |--------------- |--------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  0.0011 ns | 0.0026 ns | 0.0022 ns |  0.0000 ns |      - |         - |
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.4943 ns | 0.0801 ns | 0.0750 ns |  0.4900 ns |      - |         - |
| Sequential       | .NET 10.0      | .NET 10.0      |  0.5707 ns | 0.0420 ns | 0.0351 ns |  0.5688 ns |      - |         - |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  0.9471 ns | 0.0061 ns | 0.0054 ns |  0.9443 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  8.1577 ns | 0.0393 ns | 0.0348 ns |  8.1520 ns | 0.0089 |      56 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 | 13.5840 ns | 0.0381 ns | 0.0356 ns | 13.5670 ns | 0.0102 |      64 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 14.0779 ns | 0.2094 ns | 0.1959 ns | 14.0158 ns | 0.0102 |      64 B |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 14.2849 ns | 0.0305 ns | 0.0238 ns | 14.2906 ns | 0.0102 |      64 B |
| Sbo15            | .NET 10.0      | .NET 10.0      | 16.9559 ns | 0.1150 ns | 0.1019 ns | 16.9389 ns | 0.0102 |      64 B |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 17.7077 ns | 0.4142 ns | 0.4432 ns | 17.6298 ns | 0.0089 |      56 B |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 18.4836 ns | 0.0484 ns | 0.0429 ns | 18.4880 ns | 0.0102 |      64 B |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 18.5658 ns | 0.3048 ns | 0.2851 ns | 18.4575 ns | 0.0102 |      64 B |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 18.6511 ns | 0.0892 ns | 0.0696 ns | 18.6661 ns | 0.0102 |      64 B |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 18.7983 ns | 0.0545 ns | 0.0483 ns | 18.7765 ns | 0.0102 |      64 B |
| Sbo23            | .NET 10.0      | .NET 10.0      | 18.9271 ns | 0.1640 ns | 0.1535 ns | 18.8512 ns | 0.0102 |      64 B |
| Sbo31            | .NET 10.0      | .NET 10.0      | 19.2065 ns | 0.0651 ns | 0.0609 ns | 19.1761 ns | 0.0102 |      64 B |
| Sbo7             | .NET 10.0      | .NET 10.0      | 22.7104 ns | 0.1934 ns | 0.1510 ns | 22.6668 ns | 0.0102 |      64 B |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 22.7984 ns | 0.1233 ns | 0.0963 ns | 22.7579 ns | 0.0102 |      64 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 | 32.3311 ns | 0.1642 ns | 0.1536 ns | 32.3074 ns | 0.0255 |     160 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 | 34.0056 ns | 0.0923 ns | 0.0721 ns | 34.0287 ns | 0.0280 |     176 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 37.5452 ns | 0.1152 ns | 0.1077 ns | 37.5215 ns | 0.0357 |     224 B |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 39.8803 ns | 0.4312 ns | 0.4034 ns | 39.7049 ns | 0.0255 |     160 B |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 40.3249 ns | 0.1693 ns | 0.1583 ns | 40.3204 ns | 0.0459 |     288 B |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 40.9171 ns | 0.1434 ns | 0.1197 ns | 40.9355 ns | 0.0280 |     176 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 41.8957 ns | 0.2069 ns | 0.1834 ns | 41.8857 ns | 0.0357 |     224 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 46.0459 ns | 0.1578 ns | 0.1399 ns | 46.0452 ns | 0.0459 |     288 B |
