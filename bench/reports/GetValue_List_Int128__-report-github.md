```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                         | Mean      | Error     | StdDev    | Code Size | Allocated |
|------------------------------- |----------:|----------:|----------:|----------:|----------:|
| Field                          |  1.859 ns | 0.1003 ns | 0.0938 ns |      14 B |         - |
| Field_ReadOnly                 |  1.859 ns | 0.1350 ns | 0.1263 ns |      14 B |         - |
| Value                          |  5.656 ns | 0.0578 ns | 0.0513 ns |     982 B |         - |
| Value_ReadOnly                 |  5.669 ns | 0.1204 ns | 0.1126 ns |     982 B |         - |
| TryGetValue                    |  1.558 ns | 0.0767 ns | 0.0717 ns |      32 B |         - |
| TryGetValue_ReadOnly           |  1.792 ns | 0.1044 ns | 0.0926 ns |      32 B |         - |
| TryGetValue_UnionType_Preboxed | 14.263 ns | 0.0399 ns | 0.0333 ns |     494 B |         - |
| Value_UnionType_Preboxed       |  5.372 ns | 0.1460 ns | 0.1366 ns |     817 B |         - |
| TryGetValue_UnionType          |  8.615 ns | 0.1253 ns | 0.1172 ns |     455 B |         - |
| Value_UnionType                |  9.065 ns | 0.0854 ns | 0.0667 ns |     880 B |         - |
