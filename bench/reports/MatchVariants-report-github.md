```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]    : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0 : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3

Job=.NET 10.0  Runtime=.NET 10.0  

```
| Method           | TN | Mean        | Error     | StdDev    | Code Size | Gen0   | Allocated |
|----------------- |--- |------------:|----------:|----------:|----------:|-------:|----------:|
| OneOf            | 1  |   0.0000 ns | 0.0000 ns | 0.0000 ns |     272 B |      - |         - |
| UnionUtil_switch | 6  |   0.4255 ns | 0.0178 ns | 0.0166 ns |     285 B |      - |         - |
| OneOf            | 6  |   0.4465 ns | 0.0087 ns | 0.0073 ns |     265 B |      - |         - |
| UnionUtil        | 1  |   0.4751 ns | 0.0389 ns | 0.0345 ns |     605 B |      - |         - |
| UnionUtil_switch | 8  |   0.4779 ns | 0.0216 ns | 0.0180 ns |     296 B |      - |         - |
| OneOf            | 8  |   0.4907 ns | 0.0167 ns | 0.0139 ns |     283 B |      - |         - |
| UnionUtil_switch | 1  |   0.4919 ns | 0.0673 ns | 0.0630 ns |     300 B |      - |         - |
| UnionUtil        | 8  |   0.9434 ns | 0.0148 ns | 0.0124 ns |     767 B |      - |         - |
| UnionUtil        | 6  |   1.0181 ns | 0.0958 ns | 0.0896 ns |     694 B |      - |         - |
| Dunet_switch     | 1  |   1.0560 ns | 0.0969 ns | 0.0906 ns |     376 B |      - |         - |
| Dunet_switch     | 6  |   1.8242 ns | 0.0112 ns | 0.0088 ns |     302 B |      - |         - |
| OneOf_Static     | 1  |   1.9582 ns | 0.0846 ns | 0.0750 ns |   1,304 B |      - |         - |
| UnionUtil_Static | 6  |   2.2040 ns | 0.0395 ns | 0.0369 ns |   1,729 B |      - |         - |
| OneOf_Static     | 6  |   2.3510 ns | 0.0441 ns | 0.0391 ns |   1,327 B |      - |         - |
| UnionUtil_Static | 1  |   2.5094 ns | 0.0483 ns | 0.0451 ns |   1,694 B |      - |         - |
| OneOf_Static     | 8  |   2.6223 ns | 0.0161 ns | 0.0135 ns |   1,326 B |      - |         - |
| Dunet_switch     | 8  |   2.7725 ns | 0.0245 ns | 0.0218 ns |     261 B |      - |         - |
| UnionUtil_Static | 8  |   3.0016 ns | 0.0210 ns | 0.0187 ns |   1,756 B |      - |         - |
| Dunet_Static     | 6  |   9.1623 ns | 0.0233 ns | 0.0207 ns |   1,239 B |      - |         - |
| Dunet_Static     | 8  |   9.1739 ns | 0.0149 ns | 0.0116 ns |   1,239 B |      - |         - |
| Dunet_Static     | 1  |   9.9762 ns | 0.1714 ns | 0.1519 ns |   1,239 B |      - |         - |
| Dunet            | 6  |  95.5081 ns | 0.6593 ns | 0.5844 ns |     716 B | 0.0815 |     512 B |
| Dunet            | 8  |  97.2573 ns | 0.5678 ns | 0.4741 ns |     714 B | 0.0815 |     512 B |
| Dunet            | 1  | 111.5397 ns | 1.5462 ns | 1.3707 ns |     711 B | 0.0815 |     512 B |
