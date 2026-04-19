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
| Field                    | 0.0118 ns | 0.0177 ns | 0.0459 ns | 0.0000 ns |      11 B |         - |
| IsT                      | 0.0021 ns | 0.0086 ns | 0.0084 ns | 0.0000 ns |      38 B |         - |
| IsIndex                  | 0.0162 ns | 0.0287 ns | 0.0269 ns | 0.0000 ns |      11 B |         - |
| IsTag                    | 0.6008 ns | 0.0546 ns | 0.0456 ns | 0.5978 ns |     493 B |         - |
| IsT_OpenGeneric          | 1.5069 ns | 0.0450 ns | 0.0376 ns | 1.5004 ns |      61 B |         - |
| IsT_OpenGeneric_Mismatch | 1.0570 ns | 0.0969 ns | 0.1646 ns | 1.0189 ns |      61 B |         - |
| IsIndex_OpenGeneric      | 0.8905 ns | 0.0546 ns | 0.0484 ns | 0.9010 ns |      44 B |         - |
