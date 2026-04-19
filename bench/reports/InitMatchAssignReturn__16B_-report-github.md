```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method           | Job            | Runtime        | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated |
|----------------- |--------------- |--------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
| Sequential       | .NET 10.0      | .NET 10.0      |  0.0062 ns | 0.0136 ns | 0.0121 ns |  0.0000 ns |      - |         - |
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.0103 ns | 0.0198 ns | 0.0176 ns |  0.0000 ns |      - |         - |
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  0.1412 ns | 0.0193 ns | 0.0181 ns |  0.1420 ns |      - |         - |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  0.9687 ns | 0.0200 ns | 0.0187 ns |  0.9635 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  6.6012 ns | 0.0243 ns | 0.0203 ns |  6.5997 ns | 0.0089 |      56 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 |  8.3531 ns | 0.1424 ns | 0.1262 ns |  8.2882 ns | 0.0102 |      64 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 |  9.7668 ns | 0.0621 ns | 0.0550 ns |  9.7647 ns | 0.0153 |      96 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 | 10.8176 ns | 0.0413 ns | 0.0323 ns | 10.8079 ns | 0.0179 |     112 B |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 11.9003 ns | 0.1183 ns | 0.0988 ns | 11.8604 ns | 0.0089 |      56 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 12.3889 ns | 0.0563 ns | 0.0526 ns | 12.3662 ns | 0.0102 |      64 B |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 13.9897 ns | 0.0578 ns | 0.0451 ns | 13.9865 ns | 0.0102 |      64 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 14.1863 ns | 0.2136 ns | 0.1998 ns | 14.2194 ns | 0.0255 |     160 B |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 14.2144 ns | 0.1542 ns | 0.1367 ns | 14.1656 ns | 0.0102 |      64 B |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 15.4154 ns | 0.0982 ns | 0.0767 ns | 15.3770 ns | 0.0102 |      64 B |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 16.8049 ns | 0.1244 ns | 0.1163 ns | 16.7885 ns | 0.0153 |      96 B |
| Sbo23            | .NET 10.0      | .NET 10.0      | 17.2181 ns | 0.0194 ns | 0.0151 ns | 17.2120 ns |      - |         - |
| Sbo31            | .NET 10.0      | .NET 10.0      | 17.3603 ns | 0.2191 ns | 0.1942 ns | 17.3249 ns |      - |         - |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 17.3695 ns | 0.1789 ns | 0.1586 ns | 17.4316 ns | 0.0357 |     224 B |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 17.3844 ns | 0.0579 ns | 0.0484 ns | 17.3686 ns |      - |         - |
| Sbo15            | .NET 10.0      | .NET 10.0      | 18.8725 ns | 0.0534 ns | 0.0417 ns | 18.8709 ns | 0.0102 |      64 B |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 19.1788 ns | 0.0733 ns | 0.0612 ns | 19.1445 ns |      - |         - |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 19.4567 ns | 0.1540 ns | 0.1365 ns | 19.4608 ns | 0.0179 |     112 B |
| Sbo7             | .NET 10.0      | .NET 10.0      | 21.6233 ns | 0.2444 ns | 0.2167 ns | 21.5199 ns | 0.0102 |      64 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 21.7303 ns | 0.1218 ns | 0.1080 ns | 21.7102 ns | 0.0255 |     160 B |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 23.4041 ns | 0.2975 ns | 0.2637 ns | 23.3416 ns | 0.0102 |      64 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 26.0183 ns | 0.0999 ns | 0.0886 ns | 26.0364 ns | 0.0357 |     224 B |
