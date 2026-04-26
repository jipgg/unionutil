```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                                  | Mean       | Error     | StdDev    | Median     | Code Size | Allocated |
|---------------------------------------- |-----------:|----------:|----------:|-----------:|----------:|----------:|
| SetValue_T1                             |  0.0000 ns | 0.0000 ns | 0.0000 ns |  0.0000 ns |      38 B |         - |
| TrySetValue_UnionType_T5                |  0.0004 ns | 0.0013 ns | 0.0011 ns |  0.0000 ns |      37 B |         - |
| SetValue_T5                             |  0.0010 ns | 0.0025 ns | 0.0019 ns |  0.0000 ns |      37 B |         - |
| TrySetValue_UnionType_SameType          |  0.0046 ns | 0.0101 ns | 0.0079 ns |  0.0020 ns |      92 B |         - |
| SetValue_SameType                       |  0.0060 ns | 0.0079 ns | 0.0066 ns |  0.0032 ns |      92 B |         - |
| TrySetValue_UnionType_T1                |  0.0077 ns | 0.0132 ns | 0.0111 ns |  0.0004 ns |      38 B |         - |
| TrySetValue_UnionType_T8                |  3.6662 ns | 0.1361 ns | 0.1671 ns |  3.6387 ns |      47 B |         - |
| SetValue_T8                             |  3.7988 ns | 0.1379 ns | 0.1978 ns |  3.7951 ns |      47 B |         - |
| TrySetValue_UnionType_T5_Preboxed       |  8.2346 ns | 0.0251 ns | 0.0223 ns |  8.2242 ns |     272 B |         - |
| TrySetValue_UnionType_T1_Preboxed       |  9.1354 ns | 0.0068 ns | 0.0053 ns |  9.1333 ns |     274 B |         - |
| TrySetValue_UnionType_SameType_Preboxed |  9.1491 ns | 0.0221 ns | 0.0196 ns |  9.1464 ns |     277 B |         - |
| TrySetValue_UnionType_T8_Preboxed       | 13.9648 ns | 0.2704 ns | 0.2529 ns | 13.9590 ns |     273 B |         - |
