```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method                      | Job            | Runtime        | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated |
|---------------------------- |--------------- |--------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
| &#39;speculative union&#39;         | .NET 10.0      | .NET 10.0      | 11.1270 ns | 0.3536 ns | 0.9914 ns | 10.7314 ns | 0.0076 |      48 B |
| generic,sequential          | .NET 10.0      | .NET 10.0      |  0.3522 ns | 0.0375 ns | 0.0313 ns |  0.3537 ns |      - |         - |
| generic,sequential,tagged   | .NET 10.0      | .NET 10.0      |  0.0676 ns | 0.0623 ns | 0.0666 ns |  0.0548 ns |      - |         - |
| generic,boxed               | .NET 10.0      | .NET 10.0      | 11.5482 ns | 0.2890 ns | 0.2704 ns | 11.4785 ns | 0.0076 |      48 B |
| generic,boxed,sbo32(fits)   | .NET 10.0      | .NET 10.0      | 19.0391 ns | 0.4350 ns | 0.4069 ns | 18.9045 ns |      - |         - |
| generic,boxed,sbo7(default) | .NET 10.0      | .NET 10.0      |  1.6443 ns | 0.0573 ns | 0.0508 ns |  1.6431 ns |      - |         - |
| monomorphized               | .NET 10.0      | .NET 10.0      |  0.5513 ns | 0.0869 ns | 0.0892 ns |  0.5124 ns |      - |         - |
| &#39;speculative union&#39;         | NativeAOT 10.0 | NativeAOT 10.0 |  1.5825 ns | 0.0524 ns | 0.0490 ns |  1.5868 ns |      - |         - |
| generic,sequential          | NativeAOT 10.0 | NativeAOT 10.0 |  1.2548 ns | 0.0667 ns | 0.0685 ns |  1.2417 ns |      - |         - |
| generic,sequential,tagged   | NativeAOT 10.0 | NativeAOT 10.0 |  1.3818 ns | 0.0566 ns | 0.0530 ns |  1.3755 ns |      - |         - |
| generic,boxed               | NativeAOT 10.0 | NativeAOT 10.0 |  7.7896 ns | 0.1149 ns | 0.1019 ns |  7.8119 ns | 0.0076 |      48 B |
| generic,boxed,sbo32(fits)   | NativeAOT 10.0 | NativeAOT 10.0 | 19.5867 ns | 0.3458 ns | 0.3235 ns | 19.6589 ns |      - |         - |
| generic,boxed,sbo7(default) | NativeAOT 10.0 | NativeAOT 10.0 |  3.2688 ns | 0.0592 ns | 0.0525 ns |  3.2789 ns |      - |         - |
| monomorphized               | NativeAOT 10.0 | NativeAOT 10.0 |  1.9096 ns | 0.0714 ns | 0.0903 ns |  1.8833 ns |      - |         - |
