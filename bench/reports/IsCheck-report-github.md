```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                   | Mean      | Error     | StdDev    | Median    | Code Size | Allocated |
|------------------------- |----------:|----------:|----------:|----------:|----------:|----------:|
| Field                    | 0.0089 ns | 0.0210 ns | 0.0176 ns | 0.0000 ns |      11 B |         - |
| IsT                      | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns |      38 B |         - |
| IsIndex                  | 0.0154 ns | 0.0348 ns | 0.0309 ns | 0.0009 ns |      11 B |         - |
| IsTag_Dense              | 0.0052 ns | 0.0170 ns | 0.0151 ns | 0.0000 ns |      13 B |         - |
| IsTag_Sparse             | 0.0708 ns | 0.0619 ns | 0.0636 ns | 0.0531 ns |     493 B |         - |
| IsT_OpenGeneric          | 1.0130 ns | 0.0465 ns | 0.0435 ns | 1.0253 ns |      61 B |         - |
| IsT_OpenGeneric_Mismatch | 1.2595 ns | 0.1045 ns | 0.1431 ns | 1.2228 ns |      61 B |         - |
| IsIndex_OpenGeneric      | 1.0336 ns | 0.0850 ns | 0.0795 ns | 1.0218 ns |      44 B |         - |
