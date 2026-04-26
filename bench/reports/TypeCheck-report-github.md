```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                       | _value  | Mean      | Error     | StdDev    | Median    | Code Size | Allocated |
|----------------------------- |-------- |----------:|----------:|----------:|----------:|----------:|----------:|
| Tag_Dense                    | 0.12345 | 0.0000 ns | 0.0000 ns | 0.0000 ns | 0.0000 ns |      13 B |         - |
| HoldsType_UnionType          | 1234    | 0.0006 ns | 0.0017 ns | 0.0014 ns | 0.0000 ns |      38 B |         - |
| TryGetValue                  | 1234    | 0.0007 ns | 0.0021 ns | 0.0016 ns | 0.0000 ns |      15 B |         - |
| HoldsType                    | 0.12345 | 0.0007 ns | 0.0021 ns | 0.0018 ns | 0.0000 ns |      41 B |         - |
| Field                        | 0.12345 | 0.0009 ns | 0.0029 ns | 0.0025 ns | 0.0000 ns |      11 B |         - |
| Tag_Dense                    | 1234    | 0.0021 ns | 0.0069 ns | 0.0054 ns | 0.0000 ns |      13 B |         - |
| Tag_Sparse                   | 1234    | 0.0022 ns | 0.0053 ns | 0.0045 ns | 0.0000 ns |     493 B |         - |
| HoldsType                    | 1234    | 0.0024 ns | 0.0052 ns | 0.0046 ns | 0.0000 ns |      38 B |         - |
| Field                        | 1234    | 0.0043 ns | 0.0105 ns | 0.0087 ns | 0.0000 ns |      11 B |         - |
| HoldsType_UnionType          | 0.12345 | 0.0044 ns | 0.0120 ns | 0.0100 ns | 0.0000 ns |      41 B |         - |
| Tag_Sparse                   | 0.12345 | 0.0136 ns | 0.0253 ns | 0.0225 ns | 0.0002 ns |     484 B |         - |
| TryGetValue                  | 0.12345 | 0.0248 ns | 0.0033 ns | 0.0025 ns | 0.0234 ns |      15 B |         - |
| HoldsType_UnionType_Preboxed | 0.12345 | 8.2246 ns | 0.0166 ns | 0.0138 ns | 8.2182 ns |     254 B |         - |
| HoldsType_UnionType_Preboxed | 1234    | 8.2282 ns | 0.0200 ns | 0.0167 ns | 8.2229 ns |     254 B |         - |
