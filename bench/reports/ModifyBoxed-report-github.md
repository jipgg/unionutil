```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                  | N   | Mean      | Error    | StdDev   | Code Size | Gen0   | Allocated |
|------------------------ |---- |----------:|---------:|---------:|----------:|-------:|----------:|
| Generated_ListInt       | 8   |  10.59 ns | 0.052 ns | 0.046 ns |     127 B | 0.0051 |      32 B |
| Generated_ManagedStruct | 8   |  14.04 ns | 0.035 ns | 0.031 ns |     154 B | 0.0038 |      24 B |
| Baseline_ListInt        | 8   |  16.58 ns | 0.168 ns | 0.157 ns |     206 B | 0.0051 |      32 B |
| Generated_Int           | 8   |  20.26 ns | 0.213 ns | 0.189 ns |     181 B | 0.0038 |      24 B |
| Generated_ListInt       | 32  |  21.10 ns | 0.075 ns | 0.067 ns |     127 B | 0.0051 |      32 B |
| Generated_ManagedStruct | 32  |  35.92 ns | 0.153 ns | 0.136 ns |     154 B | 0.0038 |      24 B |
| Baseline_ManagedStruct  | 8   |  48.89 ns | 0.281 ns | 0.249 ns |     151 B | 0.0306 |     192 B |
| Baseline_ListInt        | 32  |  55.15 ns | 0.183 ns | 0.153 ns |     206 B | 0.0051 |      32 B |
| Baseline_Int            | 8   |  63.85 ns | 0.262 ns | 0.204 ns |     157 B | 0.0305 |     192 B |
| Generated_Int           | 32  |  75.47 ns | 1.470 ns | 1.303 ns |     181 B | 0.0038 |      24 B |
| Generated_ListInt       | 128 |  88.05 ns | 0.591 ns | 0.462 ns |     127 B | 0.0050 |      32 B |
| Generated_ManagedStruct | 128 | 120.71 ns | 1.612 ns | 1.508 ns |     154 B | 0.0038 |      24 B |
| Baseline_ListInt        | 128 | 142.04 ns | 1.009 ns | 0.944 ns |     206 B | 0.0050 |      32 B |
| Generated_Int           | 128 | 159.78 ns | 1.621 ns | 1.516 ns |     181 B | 0.0038 |      24 B |
| Baseline_Int            | 32  | 182.37 ns | 0.529 ns | 0.442 ns |     157 B | 0.1223 |     768 B |
| Baseline_ManagedStruct  | 32  | 183.07 ns | 0.722 ns | 0.640 ns |     151 B | 0.1223 |     768 B |
| Baseline_ManagedStruct  | 128 | 669.94 ns | 3.097 ns | 2.586 ns |     151 B | 0.4892 |    3072 B |
| Baseline_Int            | 128 | 682.52 ns | 6.974 ns | 6.524 ns |     157 B | 0.4892 |    3072 B |
