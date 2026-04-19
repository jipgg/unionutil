```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method           | Job            | Runtime        | Mean       | Error     | StdDev    | Gen0   | Allocated |
|----------------- |--------------- |--------------- |-----------:|----------:|----------:|-------:|----------:|
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  0.0194 ns | 0.0173 ns | 0.0153 ns |      - |         - |
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.0531 ns | 0.0037 ns | 0.0033 ns |      - |         - |
| Sequential       | .NET 10.0      | .NET 10.0      |  0.5192 ns | 0.0137 ns | 0.0121 ns |      - |         - |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  0.9756 ns | 0.0073 ns | 0.0057 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  5.9218 ns | 0.0230 ns | 0.0215 ns | 0.0076 |      48 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 |  7.0400 ns | 0.0388 ns | 0.0363 ns | 0.0076 |      48 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 |  8.9490 ns | 0.1603 ns | 0.1499 ns | 0.0153 |      96 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 |  9.9362 ns | 0.2632 ns | 0.2333 ns | 0.0179 |     112 B |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 11.3645 ns | 0.0761 ns | 0.0636 ns | 0.0076 |      48 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 12.8313 ns | 0.1458 ns | 0.1293 ns | 0.0076 |      48 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 12.9761 ns | 0.1121 ns | 0.0936 ns | 0.0255 |     160 B |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 13.0232 ns | 0.1747 ns | 0.1549 ns | 0.0076 |      48 B |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 14.1269 ns | 0.0211 ns | 0.0165 ns | 0.0076 |      48 B |
| Sbo15            | .NET 10.0      | .NET 10.0      | 15.8618 ns | 0.0515 ns | 0.0430 ns |      - |         - |
| Sbo23            | .NET 10.0      | .NET 10.0      | 16.2984 ns | 0.0114 ns | 0.0095 ns |      - |         - |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 16.6910 ns | 0.2734 ns | 0.2557 ns | 0.0357 |     224 B |
| Sbo31            | .NET 10.0      | .NET 10.0      | 17.2283 ns | 0.0364 ns | 0.0304 ns |      - |         - |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 17.3332 ns | 0.0817 ns | 0.0682 ns |      - |         - |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 17.9691 ns | 0.2875 ns | 0.2549 ns |      - |         - |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 18.1750 ns | 0.0247 ns | 0.0193 ns |      - |         - |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 18.3033 ns | 0.0841 ns | 0.0787 ns | 0.0153 |      96 B |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 19.0865 ns | 0.1822 ns | 0.1615 ns | 0.0179 |     112 B |
| Sbo7             | .NET 10.0      | .NET 10.0      | 19.8601 ns | 0.0310 ns | 0.0259 ns | 0.0076 |      48 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 21.2805 ns | 0.1322 ns | 0.1172 ns | 0.0255 |     160 B |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 21.6887 ns | 0.2059 ns | 0.1720 ns | 0.0076 |      48 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 24.5065 ns | 0.2326 ns | 0.2062 ns | 0.0357 |     224 B |
