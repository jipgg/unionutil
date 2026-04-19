```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3


```
| Method           | Job            | Runtime        | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated |
|----------------- |--------------- |--------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
| SequentialTagged | NativeAOT 10.0 | NativeAOT 10.0 |  0.0000 ns | 0.0000 ns | 0.0000 ns |  0.0000 ns |      - |         - |
| SequentialTagged | .NET 10.0      | .NET 10.0      |  0.0003 ns | 0.0012 ns | 0.0011 ns |  0.0000 ns |      - |         - |
| Sequential       | NativeAOT 10.0 | NativeAOT 10.0 |  0.0031 ns | 0.0082 ns | 0.0077 ns |  0.0000 ns |      - |         - |
| Sequential       | .NET 10.0      | .NET 10.0      |  0.0577 ns | 0.0073 ns | 0.0061 ns |  0.0551 ns |      - |         - |
| Sbo7             | .NET 10.0      | .NET 10.0      |  4.9641 ns | 0.0110 ns | 0.0086 ns |  4.9617 ns |      - |         - |
| Sbo7             | NativeAOT 10.0 | NativeAOT 10.0 |  6.4361 ns | 0.0533 ns | 0.0445 ns |  6.4150 ns |      - |         - |
| UnionBaseline    | NativeAOT 10.0 | NativeAOT 10.0 |  6.6470 ns | 0.1330 ns | 0.1244 ns |  6.6771 ns | 0.0076 |      48 B |
| Boxed            | NativeAOT 10.0 | NativeAOT 10.0 |  7.1281 ns | 0.0765 ns | 0.0716 ns |  7.1012 ns | 0.0076 |      48 B |
| Sbo23Class       | NativeAOT 10.0 | NativeAOT 10.0 |  9.1723 ns | 0.1839 ns | 0.1536 ns |  9.1509 ns | 0.0153 |      96 B |
| Sbo31Class       | NativeAOT 10.0 | NativeAOT 10.0 |  9.9176 ns | 0.1848 ns | 0.1638 ns |  9.8505 ns | 0.0179 |     112 B |
| BoxedTagged      | NativeAOT 10.0 | NativeAOT 10.0 | 10.4572 ns | 0.0841 ns | 0.0657 ns | 10.4632 ns | 0.0076 |      48 B |
| Boxed            | .NET 10.0      | .NET 10.0      | 11.0077 ns | 0.0791 ns | 0.0661 ns | 11.0271 ns | 0.0076 |      48 B |
| Sbo55Class       | NativeAOT 10.0 | NativeAOT 10.0 | 12.2674 ns | 0.0312 ns | 0.0260 ns | 12.2691 ns | 0.0255 |     160 B |
| UnionBaseline    | .NET 10.0      | .NET 10.0      | 13.0474 ns | 0.0706 ns | 0.0589 ns | 13.0380 ns | 0.0076 |      48 B |
| BoxedTagged      | .NET 10.0      | .NET 10.0      | 14.0956 ns | 0.0343 ns | 0.0268 ns | 14.0881 ns | 0.0076 |      48 B |
| Sbo80Class       | NativeAOT 10.0 | NativeAOT 10.0 | 16.5164 ns | 0.0761 ns | 0.0675 ns | 16.5245 ns | 0.0357 |     224 B |
| Sbo15            | NativeAOT 10.0 | NativeAOT 10.0 | 17.4459 ns | 0.0824 ns | 0.0731 ns | 17.4537 ns |      - |         - |
| Sbo31            | NativeAOT 10.0 | NativeAOT 10.0 | 17.7942 ns | 0.0241 ns | 0.0201 ns | 17.7853 ns |      - |         - |
| Sbo23            | NativeAOT 10.0 | NativeAOT 10.0 | 17.8591 ns | 0.1990 ns | 0.1764 ns | 17.8143 ns |      - |         - |
| Sbo23            | .NET 10.0      | .NET 10.0      | 17.8875 ns | 0.3824 ns | 0.3577 ns | 17.9303 ns |      - |         - |
| Sbo15            | .NET 10.0      | .NET 10.0      | 17.9164 ns | 0.4165 ns | 0.7402 ns | 17.9746 ns |      - |         - |
| Sbo31            | .NET 10.0      | .NET 10.0      | 19.1656 ns | 0.4558 ns | 0.4476 ns | 19.2055 ns |      - |         - |
| Sbo23Class       | .NET 10.0      | .NET 10.0      | 19.1771 ns | 0.3625 ns | 0.3214 ns | 19.1760 ns | 0.0153 |      96 B |
| Sbo31Class       | .NET 10.0      | .NET 10.0      | 20.3437 ns | 0.4224 ns | 0.3744 ns | 20.3570 ns | 0.0179 |     112 B |
| Sbo55Class       | .NET 10.0      | .NET 10.0      | 24.1567 ns | 0.2476 ns | 0.2195 ns | 24.1047 ns | 0.0255 |     160 B |
| Sbo80Class       | .NET 10.0      | .NET 10.0      | 26.6827 ns | 0.6686 ns | 0.8926 ns | 26.7350 ns | 0.0357 |     224 B |
