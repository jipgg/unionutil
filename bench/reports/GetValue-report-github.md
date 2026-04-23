```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                | Mean       | Error     | StdDev    | Gen0   | Code Size | Allocated |
|---------------------- |-----------:|----------:|----------:|-------:|----------:|----------:|
| FieldAccess           |  0.0000 ns | 0.0000 ns | 0.0000 ns |      - |      20 B |         - |
| ValueProperty         |  9.6918 ns | 0.1195 ns | 0.0933 ns | 0.0051 |     403 B |      32 B |
| TryGetValue           |  0.4981 ns | 0.0375 ns | 0.0333 ns |      - |      24 B |         - |
| IUnion.TryGetValue&lt;T&gt; | 45.2913 ns | 0.6884 ns | 0.5748 ns | 0.0102 |     355 B |      64 B |
| TUnion.TryGetValue&lt;T&gt; |  0.5258 ns | 0.0675 ns | 0.0631 ns |      - |      82 B |         - |
