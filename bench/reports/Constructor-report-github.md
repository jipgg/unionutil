```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method           | Mean      | Error     | StdDev    | Median    | Code Size | Allocated |
|----------------- |----------:|----------:|----------:|----------:|----------:|----------:|
| Seq_WithSentinel | 0.1116 ns | 0.0539 ns | 0.0738 ns | 0.1317 ns |      14 B |         - |
| Seq              | 0.0709 ns | 0.0552 ns | 0.1115 ns | 0.0000 ns |      14 B |         - |
| Seq_Implicit     | 0.1597 ns | 0.0785 ns | 0.1332 ns | 0.1369 ns |      14 B |         - |
| Box_WithSentinel | 8.3429 ns | 0.1236 ns | 0.1156 ns | 8.3592 ns |      66 B |         - |
| Box              | 6.7463 ns | 0.2097 ns | 0.1859 ns | 6.7185 ns |      66 B |         - |
| Box_Implicit     | 7.3057 ns | 0.1441 ns | 0.1277 ns | 7.2974 ns |      66 B |         - |
