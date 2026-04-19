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
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.0244 ns | 0.0450 ns | 0.0399 ns |  0.0048 ns |      - |         - |
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  0.0449 ns | 0.0107 ns | 0.0095 ns |  0.0446 ns |      - |         - |
| Sequential       | .NET 10.0      | .NET 10.0      |  0.5184 ns | 0.0480 ns | 0.0449 ns |  0.5258 ns |      - |         - |
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  1.0060 ns | 0.0086 ns | 0.0072 ns |  1.0018 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  5.7466 ns | 0.0892 ns | 0.0745 ns |  5.7388 ns | 0.0076 |      48 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 |  6.3021 ns | 0.0573 ns | 0.0536 ns |  6.2938 ns | 0.0076 |      48 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 |  8.5915 ns | 0.0415 ns | 0.0347 ns |  8.5870 ns | 0.0153 |      96 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 |  9.7359 ns | 0.0699 ns | 0.0619 ns |  9.7637 ns | 0.0179 |     112 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 12.2590 ns | 0.2460 ns | 0.2301 ns | 12.2703 ns | 0.0076 |      48 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 12.7409 ns | 0.0486 ns | 0.0430 ns | 12.7479 ns | 0.0255 |     160 B |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 13.0339 ns | 0.0711 ns | 0.0555 ns | 13.0208 ns | 0.0076 |      48 B |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 14.4027 ns | 0.3630 ns | 0.3884 ns | 14.4160 ns | 0.0076 |      48 B |
| Sbo23            | .NET 10.0      | .NET 10.0      | 15.6169 ns | 0.1470 ns | 0.1228 ns | 15.5678 ns |      - |         - |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 16.2974 ns | 0.2551 ns | 0.2386 ns | 16.3502 ns | 0.0076 |      48 B |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 16.5790 ns | 0.0954 ns | 0.0892 ns | 16.5829 ns | 0.0357 |     224 B |
| Sbo31            | .NET 10.0      | .NET 10.0      | 16.6358 ns | 0.2646 ns | 0.2346 ns | 16.5348 ns |      - |         - |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 16.9343 ns | 0.0800 ns | 0.0709 ns | 16.8982 ns |      - |         - |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 17.0119 ns | 0.1044 ns | 0.0926 ns | 17.0022 ns | 0.0153 |      96 B |
| Sbo15            | .NET 10.0      | .NET 10.0      | 17.8932 ns | 0.3990 ns | 0.3732 ns | 17.7659 ns |      - |         - |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 17.9044 ns | 0.2224 ns | 0.2080 ns | 17.7677 ns |      - |         - |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 18.9136 ns | 0.3209 ns | 0.3001 ns | 18.7079 ns |      - |         - |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 19.3974 ns | 0.3081 ns | 0.2731 ns | 19.2666 ns | 0.0179 |     112 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 21.8977 ns | 0.1599 ns | 0.1418 ns | 21.9222 ns | 0.0255 |     160 B |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 | 23.8417 ns | 0.0269 ns | 0.0238 ns | 23.8286 ns |      - |         - |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 25.0001 ns | 0.1158 ns | 0.0967 ns | 25.0057 ns | 0.0357 |     224 B |
| Sbo7             | .NET 10.0      | .NET 10.0      | 25.0546 ns | 0.5570 ns | 0.5470 ns | 24.8506 ns |      - |         - |
