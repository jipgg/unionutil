```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]     : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method        | Mean       | Error     | StdDev     | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|-------------- |-----------:|----------:|-----------:|------:|--------:|--------:|-------:|----------:|------------:|
| NoChange      |   6.561 μs | 0.0922 μs |  0.0862 μs |  1.00 |    0.02 |  0.8469 |      - |    5.2 KB |        1.00 |
| UnrelatedEdit |  99.374 μs | 1.8787 μs |  1.6654 μs | 15.15 |    0.31 |  8.7891 |      - |  56.89 KB |       10.93 |
| RelevantEdit  | 467.418 μs | 9.2218 μs | 19.8508 μs | 71.25 |    3.13 | 35.1563 | 7.8125 | 234.84 KB |       45.13 |
