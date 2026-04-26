```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                         | Mean      | Error     | StdDev    | Median    | Code Size | Gen0   | Allocated |
|------------------------------- |----------:|----------:|----------:|----------:|----------:|-------:|----------:|
| Field                          | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns |      20 B |      - |         - |
| Field_ReadOnly                 | 0.0010 ns | 0.0023 ns | 0.0019 ns | 0.0000 ns |      20 B |      - |         - |
| Value                          | 7.2035 ns | 0.0236 ns | 0.0197 ns | 7.1946 ns |     403 B | 0.0051 |      32 B |
| Value_ReadOnly                 | 8.5029 ns | 0.0167 ns | 0.0130 ns | 8.5034 ns |     403 B | 0.0051 |      32 B |
| TryGetValue                    | 0.0002 ns | 0.0007 ns | 0.0006 ns | 0.0000 ns |      24 B |      - |         - |
| TryGetValue_ReadOnly           | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns |      24 B |      - |         - |
| TryGetValue_UnionType_Preboxed | 9.1316 ns | 0.0097 ns | 0.0076 ns | 9.1289 ns |     306 B |      - |         - |
| Value_UnionType_Preboxed       | 7.7055 ns | 0.0239 ns | 0.0211 ns | 7.7068 ns |     424 B | 0.0051 |      32 B |
| TryGetValue_UnionType          | 0.0002 ns | 0.0006 ns | 0.0005 ns | 0.0000 ns |      82 B |      - |         - |
| Value_UnionType                | 9.4895 ns | 0.0855 ns | 0.0668 ns | 9.4684 ns |     403 B | 0.0051 |      32 B |
