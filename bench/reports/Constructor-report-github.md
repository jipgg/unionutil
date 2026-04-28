```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method                  | Mean      | Error     | StdDev    | Median    | Gen0   | Code Size | Allocated |
|------------------------ |----------:|----------:|----------:|----------:|-------:|----------:|----------:|
| Seq_Implicit_U64        | 0.0727 ns | 0.0489 ns | 0.1132 ns | 0.0000 ns |      - |      29 B |         - |
| Seq_U64                 | 0.1182 ns | 0.0544 ns | 0.1335 ns | 0.0817 ns |      - |      29 B |         - |
| Seq_U32                 | 0.1569 ns | 0.0539 ns | 0.0957 ns | 0.1563 ns |      - |      29 B |         - |
| Seq_WithSentinel_U32    | 0.2190 ns | 0.1030 ns | 0.3022 ns | 0.0277 ns |      - |      29 B |         - |
| Seq_WithSentinel_U64    | 0.2784 ns | 0.0584 ns | 0.0625 ns | 0.2588 ns |      - |      29 B |         - |
| Seq_U128                | 0.3054 ns | 0.0813 ns | 0.1963 ns | 0.2712 ns |      - |      29 B |         - |
| Seq_Implicit_U32        | 0.3405 ns | 0.1017 ns | 0.2998 ns | 0.3222 ns |      - |      29 B |         - |
| Seq_WithSentinel_U128   | 0.6154 ns | 0.0833 ns | 0.2456 ns | 0.5269 ns |      - |      29 B |         - |
| Seq_Implicit_U128       | 0.7310 ns | 0.0829 ns | 0.1802 ns | 0.7357 ns |      - |      29 B |         - |
| Box7_Implicit_U32       | 5.0965 ns | 0.0589 ns | 0.0551 ns | 5.0895 ns |      - |      54 B |         - |
| Box0_Implicit_U64       | 5.1859 ns | 0.1529 ns | 0.1430 ns | 5.2298 ns | 0.0038 |      34 B |      24 B |
| Box0_U64                | 5.3558 ns | 0.1686 ns | 0.3041 ns | 5.3584 ns | 0.0038 |      34 B |      24 B |
| Box0_U128               | 6.0255 ns | 0.1986 ns | 0.5161 ns | 5.9684 ns | 0.0051 |      40 B |      32 B |
| Box0_U32                | 6.0399 ns | 0.1367 ns | 0.1212 ns | 6.0113 ns | 0.0038 |      33 B |      24 B |
| Box7_WithSentinel_U32   | 6.0723 ns | 0.2482 ns | 0.7318 ns | 5.7565 ns |      - |      54 B |         - |
| Box7_U32                | 6.0877 ns | 0.1938 ns | 0.4607 ns | 6.0432 ns |      - |      54 B |         - |
| Box0_WithSentinel_U32   | 6.2432 ns | 0.1907 ns | 0.3981 ns | 6.2069 ns | 0.0038 |      33 B |      24 B |
| Box0_WithSentinel_U128  | 6.4102 ns | 0.2137 ns | 0.6235 ns | 6.3536 ns | 0.0051 |      40 B |      32 B |
| Box0_Implicit_U128      | 6.4554 ns | 0.2669 ns | 0.7871 ns | 6.3050 ns | 0.0051 |      40 B |      32 B |
| Box0_WithSentinel_U64   | 7.0495 ns | 0.2136 ns | 0.3852 ns | 7.0806 ns | 0.0038 |      34 B |      24 B |
| Box7_Implicit_U64       | 7.0890 ns | 0.2154 ns | 0.3538 ns | 7.0522 ns | 0.0038 |      69 B |      24 B |
| Box23_WithSentinel_U64  | 7.2321 ns | 0.2184 ns | 0.4101 ns | 7.1899 ns |      - |      60 B |         - |
| Box7_U64                | 7.2702 ns | 0.2150 ns | 0.3877 ns | 7.2303 ns | 0.0038 |      69 B |      24 B |
| Box0_Implicit_U32       | 7.3062 ns | 0.2199 ns | 0.5755 ns | 7.2549 ns | 0.0038 |      33 B |      24 B |
| Box23_Implicit_U32      | 7.3175 ns | 0.2202 ns | 0.6459 ns | 7.3812 ns |      - |      59 B |         - |
| Box23_U32               | 7.3557 ns | 0.2950 ns | 0.8652 ns | 6.9430 ns |      - |      59 B |         - |
| Box23_Implicit_U64      | 7.5932 ns | 0.2274 ns | 0.4381 ns | 7.4988 ns |      - |      60 B |         - |
| Box7_WithSentinel_U128  | 7.6415 ns | 0.2206 ns | 0.5536 ns | 7.5797 ns | 0.0051 |      75 B |      32 B |
| Box7_WithSentinel_U64   | 7.6819 ns | 0.2117 ns | 0.3870 ns | 7.6780 ns | 0.0038 |      69 B |      24 B |
| Box7_U128               | 7.7575 ns | 0.2297 ns | 0.6328 ns | 7.6135 ns | 0.0051 |      75 B |      32 B |
| Box7_Implicit_U128      | 7.8913 ns | 0.2213 ns | 0.3509 ns | 7.9224 ns | 0.0051 |      75 B |      32 B |
| Box23_U128              | 8.1629 ns | 0.2306 ns | 0.6726 ns | 8.1081 ns |      - |      65 B |         - |
| Box23_WithSentinel_U128 | 8.1820 ns | 0.2598 ns | 0.7661 ns | 8.0939 ns |      - |      65 B |         - |
| Box23_U64               | 8.2538 ns | 0.2408 ns | 0.6753 ns | 8.3062 ns |      - |      60 B |         - |
| Box23_Implicit_U128     | 8.3772 ns | 0.2313 ns | 0.3928 ns | 8.3638 ns |      - |      65 B |         - |
| Box23_WithSentinel_U32  | 8.6295 ns | 0.3069 ns | 0.9048 ns | 8.5348 ns |      - |      59 B |         - |
