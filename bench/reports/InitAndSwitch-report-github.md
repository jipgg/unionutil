```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method                    | Job            | Runtime        | Mean      | Error     | StdDev    | Median    | Gen0   | Allocated |
|-------------------------- |--------------- |--------------- |----------:|----------:|----------:|----------:|-------:|----------:|
| &#39;speculative union&#39;       | .NET 10.0      | .NET 10.0      | 5.9207 ns | 0.0509 ns | 0.0451 ns | 5.9018 ns | 0.0051 |      32 B |
| generic,sequential        | .NET 10.0      | .NET 10.0      | 0.0148 ns | 0.0192 ns | 0.0180 ns | 0.0020 ns |      - |         - |
| generic,sequential,tagged | .NET 10.0      | .NET 10.0      | 0.0121 ns | 0.0164 ns | 0.0145 ns | 0.0076 ns |      - |         - |
| generic,boxed             | .NET 10.0      | .NET 10.0      | 7.2218 ns | 0.0928 ns | 0.0822 ns | 7.1938 ns | 0.0051 |      32 B |
| monomorphized             | .NET 10.0      | .NET 10.0      | 0.0207 ns | 0.0291 ns | 0.0272 ns | 0.0073 ns |      - |         - |
| &#39;speculative union&#39;       | NativeAOT 10.0 | NativeAOT 10.0 | 1.1461 ns | 0.0086 ns | 0.0076 ns | 1.1433 ns |      - |         - |
| generic,sequential        | NativeAOT 10.0 | NativeAOT 10.0 | 0.1019 ns | 0.0194 ns | 0.0182 ns | 0.1011 ns |      - |         - |
| generic,sequential,tagged | NativeAOT 10.0 | NativeAOT 10.0 | 0.2535 ns | 0.0389 ns | 0.0520 ns | 0.2627 ns |      - |         - |
| generic,boxed             | NativeAOT 10.0 | NativeAOT 10.0 | 4.2726 ns | 0.0787 ns | 0.0615 ns | 4.2553 ns | 0.0051 |      32 B |
| monomorphized             | NativeAOT 10.0 | NativeAOT 10.0 | 0.0065 ns | 0.0098 ns | 0.0092 ns | 0.0000 ns |      - |         - |
