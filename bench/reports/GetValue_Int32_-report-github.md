```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                         | Mean       | Error     | StdDev    | Median     | Gen0   | Code Size | Allocated |
|------------------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|----------:|
| Field                          |  0.0000 ns | 0.0000 ns | 0.0000 ns |  0.0000 ns |      - |         - |         - |
| Field_ReadOnly                 |  0.0521 ns | 0.0582 ns | 0.0545 ns |  0.0416 ns |      - |      13 B |         - |
| Value                          |  9.3946 ns | 0.2039 ns | 0.1907 ns |  9.3643 ns | 0.0038 |     389 B |      24 B |
| Value_ReadOnly                 |  8.2376 ns | 0.1776 ns | 0.1661 ns |  8.2357 ns | 0.0038 |     389 B |      24 B |
| TryGetValue                    |  0.0022 ns | 0.0091 ns | 0.0085 ns |  0.0000 ns |      - |      17 B |         - |
| TryGetValue_ReadOnly           |  0.0269 ns | 0.0318 ns | 0.0282 ns |  0.0167 ns |      - |      17 B |         - |
| TryGetValue_UnionType_Preboxed | 11.0560 ns | 0.1245 ns | 0.1040 ns | 11.0597 ns |      - |     286 B |         - |
| Value_UnionType_Preboxed       |  9.5131 ns | 0.1476 ns | 0.1308 ns |  9.4704 ns | 0.0038 |     410 B |      24 B |
| TryGetValue_UnionType          |  0.0320 ns | 0.0321 ns | 0.0284 ns |  0.0264 ns |      - |      70 B |         - |
| Value_UnionType                | 10.3190 ns | 0.2498 ns | 0.2337 ns | 10.3068 ns | 0.0038 |     389 B |      24 B |
