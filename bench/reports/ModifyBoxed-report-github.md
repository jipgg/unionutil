```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method                    | Job            | Runtime        | N   | Mean      | Error     | StdDev     | Median    | Gen0   | Allocated |
|-------------------------- |--------------- |--------------- |---- |----------:|----------:|-----------:|----------:|-------:|----------:|
| **union(int)**                | **.NET 10.0**      | **.NET 10.0**      | **8**   |  **55.61 ns** |  **1.233 ns** |   **3.247 ns** |  **55.52 ns** | **0.0306** |     **192 B** |
| generated(int)            | .NET 10.0      | .NET 10.0      | 8   |  19.20 ns |  0.219 ns |   0.205 ns |  19.22 ns | 0.0038 |      24 B |
| union(struct(object))     | .NET 10.0      | .NET 10.0      | 8   |  63.21 ns |  1.196 ns |   0.998 ns |  63.53 ns | 0.0305 |     192 B |
| generated(struct(object)) | .NET 10.0      | .NET 10.0      | 8   |  12.48 ns |  0.196 ns |   0.164 ns |  12.52 ns | 0.0038 |      24 B |
| union(List&lt;int&gt;)          | .NET 10.0      | .NET 10.0      | 8   |  17.59 ns |  0.101 ns |   0.089 ns |  17.60 ns | 0.0051 |      32 B |
| generated(List&lt;int&gt;)      | .NET 10.0      | .NET 10.0      | 8   |  11.34 ns |  0.091 ns |   0.080 ns |  11.35 ns | 0.0051 |      32 B |
| union(int)                | NativeAOT 10.0 | NativeAOT 10.0 | 8   |  46.87 ns |  0.512 ns |   0.479 ns |  46.94 ns | 0.0306 |     192 B |
| generated(int)            | NativeAOT 10.0 | NativeAOT 10.0 | 8   |  17.28 ns |  0.053 ns |   0.047 ns |  17.27 ns | 0.0038 |      24 B |
| union(struct(object))     | NativeAOT 10.0 | NativeAOT 10.0 | 8   |  35.63 ns |  0.157 ns |   0.140 ns |  35.61 ns | 0.0306 |     192 B |
| generated(struct(object)) | NativeAOT 10.0 | NativeAOT 10.0 | 8   |  12.53 ns |  0.032 ns |   0.030 ns |  12.53 ns | 0.0038 |      24 B |
| union(List&lt;int&gt;)          | NativeAOT 10.0 | NativeAOT 10.0 | 8   |  17.97 ns |  0.044 ns |   0.041 ns |  17.97 ns | 0.0051 |      32 B |
| generated(List&lt;int&gt;)      | NativeAOT 10.0 | NativeAOT 10.0 | 8   |  11.68 ns |  0.054 ns |   0.050 ns |  11.68 ns | 0.0051 |      32 B |
| **union(int)**                | **.NET 10.0**      | **.NET 10.0**      | **32**  | **184.32 ns** |  **0.455 ns** |   **0.380 ns** | **184.45 ns** | **0.1223** |     **768 B** |
| generated(int)            | .NET 10.0      | .NET 10.0      | 32  |  84.07 ns |  0.620 ns |   0.580 ns |  84.03 ns | 0.0038 |      24 B |
| union(struct(object))     | .NET 10.0      | .NET 10.0      | 32  | 182.35 ns |  0.171 ns |   0.143 ns | 182.37 ns | 0.1223 |     768 B |
| generated(struct(object)) | .NET 10.0      | .NET 10.0      | 32  |  33.84 ns |  0.229 ns |   0.191 ns |  33.78 ns | 0.0038 |      24 B |
| union(List&lt;int&gt;)          | .NET 10.0      | .NET 10.0      | 32  |  55.21 ns |  0.182 ns |   0.161 ns |  55.16 ns | 0.0051 |      32 B |
| generated(List&lt;int&gt;)      | .NET 10.0      | .NET 10.0      | 32  |  22.80 ns |  0.518 ns |   0.822 ns |  23.00 ns | 0.0051 |      32 B |
| union(int)                | NativeAOT 10.0 | NativeAOT 10.0 | 32  | 160.48 ns |  3.189 ns |   5.585 ns | 162.09 ns | 0.1223 |     768 B |
| generated(int)            | NativeAOT 10.0 | NativeAOT 10.0 | 32  |  64.47 ns |  0.917 ns |   1.020 ns |  64.57 ns | 0.0038 |      24 B |
| union(struct(object))     | NativeAOT 10.0 | NativeAOT 10.0 | 32  | 165.30 ns |  2.760 ns |   3.869 ns | 164.60 ns | 0.1223 |     768 B |
| generated(struct(object)) | NativeAOT 10.0 | NativeAOT 10.0 | 32  |  37.38 ns |  0.768 ns |   0.854 ns |  37.08 ns | 0.0038 |      24 B |
| union(List&lt;int&gt;)          | NativeAOT 10.0 | NativeAOT 10.0 | 32  |  63.28 ns |  0.664 ns |   0.621 ns |  63.31 ns | 0.0050 |      32 B |
| generated(List&lt;int&gt;)      | NativeAOT 10.0 | NativeAOT 10.0 | 32  |  43.90 ns |  0.862 ns |   0.885 ns |  44.04 ns | 0.0051 |      32 B |
| **union(int)**                | **.NET 10.0**      | **.NET 10.0**      | **128** | **847.44 ns** | **37.871 ns** | **111.663 ns** | **826.93 ns** | **0.4892** |    **3072 B** |
| generated(int)            | .NET 10.0      | .NET 10.0      | 128 | 249.01 ns |  7.558 ns |  22.284 ns | 250.61 ns | 0.0038 |      24 B |
| union(struct(object))     | .NET 10.0      | .NET 10.0      | 128 | 974.84 ns | 34.225 ns |  98.748 ns | 986.99 ns | 0.4883 |    3072 B |
| generated(struct(object)) | .NET 10.0      | .NET 10.0      | 128 | 161.06 ns |  4.863 ns |  14.339 ns | 158.80 ns | 0.0038 |      24 B |
| union(List&lt;int&gt;)          | .NET 10.0      | .NET 10.0      | 128 | 198.41 ns |  6.182 ns |  18.229 ns | 204.32 ns | 0.0050 |      32 B |
| generated(List&lt;int&gt;)      | .NET 10.0      | .NET 10.0      | 128 | 126.95 ns |  3.294 ns |   9.399 ns | 128.92 ns | 0.0050 |      32 B |
| union(int)                | NativeAOT 10.0 | NativeAOT 10.0 | 128 | 794.77 ns | 15.883 ns |  32.085 ns | 781.49 ns | 0.4892 |    3072 B |
| generated(int)            | NativeAOT 10.0 | NativeAOT 10.0 | 128 | 236.92 ns |  2.095 ns |   1.857 ns | 236.97 ns | 0.0038 |      24 B |
| union(struct(object))     | NativeAOT 10.0 | NativeAOT 10.0 | 128 | 742.78 ns | 12.785 ns |  15.219 ns | 740.27 ns | 0.4892 |    3072 B |
| generated(struct(object)) | NativeAOT 10.0 | NativeAOT 10.0 | 128 | 164.43 ns |  1.492 ns |   1.246 ns | 164.21 ns | 0.0038 |      24 B |
| union(List&lt;int&gt;)          | NativeAOT 10.0 | NativeAOT 10.0 | 128 | 202.66 ns |  1.985 ns |   1.857 ns | 202.70 ns | 0.0050 |      32 B |
| generated(List&lt;int&gt;)      | NativeAOT 10.0 | NativeAOT 10.0 | 128 | 126.13 ns |  1.484 ns |   1.388 ns | 126.26 ns | 0.0050 |      32 B |
