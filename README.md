# UnionUtil
Source generator utilities for the upcoming `union`s language feature in C#15.

> The nature of the project is quite informal at the moment with volatile changes being the norm. In the scenario where this does seem useful to more people that myself alone, i may add a stable api version and add proper diagnostics, tests and analyzers.
 
Perfectly usable in .NET10, but does not have support for the fancy switch expression for matching that is available in the .NET11 preview. In these scenarios id recommend using the `[Tagged<TEnum>]` attribute for generating named properties and using pattern matching in the switch expression to match over them (`{ Tag: Tag.X, X: var x }`).

The currently proposed default implementation of `union` types will be `readonly struct(object, int)` to my understanding, meaning it'll box value types and generics. This is arguably the best ccompromise considering the runtime limitations, but may not always be what you want.

Unions will support custom, user supplied implementations, hence the reasoning for this source generator. Mainly focusing on resolving a 'roughly' optimal memory layout by default as well as some optional parameters to finetune the generated source of the union.

## Main feature set
* Generated unions are congruent with the current compiler feature structural interface in the .NET11 preview
* Concrete `unmanaged` types always share the same storage in memory
* Configurability of whether certain types should be stored sequentially or boxed
* Support for mutable union types
* Optionally tag the union with the `Tagged<TEnum>` attribute for labelled properties.
# Current limitatios
## `where T: unmanaged`
While legal for the compiler, Overlapping generic fields are not allowed by the runtime, even when constrained to unmanaged. Meaning `where T: unmanaged` will follow the same rules as `where T: struct` for the time being.
Potential workaround would be an option to generate a small buffer optimization field with a user attributed fixed size, then store small values in there and otherwise do a boxing fallback. I'd have to look into finding an acceptable solution.
# Installation
```sh
dotnet package add UnionUtil
```
or add it as a project refecence in your `.csproj`:
```csproj
  <ItemGroup>
    <ProjectReference Include="dir\to\unionutil\src\UnionUtil\UnionUtil.csproj" />
     <ProjectReference
        Include="dir\to\unionutil\src\UnionUtil.Generators\UnionUtil.Generators.csproj" 
        OutputItemType="Analyzer"
        ReferenceOutputAssembly="false"
      />
  </ItemGroup>

```
# Usage Examples
```cs
using UnionUtil;

// basic union with statically know type cases
[UnionUImpl, Union<int, double, DateTime>]
partial struct MyUnion;

// when generics are needed:
[UnionImpl(
    // whether the union should box unconstrainted generics or store them sequentially
    // default is false (sequential)
    BoxOpenGenerics = true,
    // whether managed structs should be boxed or stored sequentially.
    // this also applies to generics constrained to `struct` or `unmanaged`
    BoxManagedStructs = true,
    // mainly useful for specifying whether a class should be readonly or not.
    // when omitted it will be inferred by the `readonly` keyword being specified on structs
    // otherwise it'll be false
    ReadOnly = false,
    // will generate HasValue for a null case in the preview union feature in .NET11
    // will also alter the Tagged variant
    Nullable = true,
    // mainly useful for inspecting how the type got generated or wanting to extend the behavior
    // default is `Visibility.Internal`
    FieldVisibility = Vilisiblity.Internal
)]
partial struct MyGenericUnion<T, U, V> : IUnion<T, U, V>;

// tagged union
public enum Result { Ok, Err }
// optional propertyNamy argument
// default is "Tag"
[Tagged<Result>("Is"), UnionImpl]
partial struct Result<T> : IUnion<T, Exception>;
// usage example
Result<int> result = 1;
result.Ok += 3;
result.Err = new Exception("my exception"); // will change the tag to Err
var str = result switch {
    {Is: Result.Ok, Ok: var v} => v.ToString(),
    {Is: Result.Err, Err: var e} => e.Message,
};
// with the .NET11 preview features
var str = result switch {
    int ok => ok.ToString(),
    Exception err => err.Message,
};

```
# Benchmarks
More proper benchmarks will come once C#15 unions are in a more fleshed out state.
## InitAndSwitch `int, double, record(int, double)` (Construction + Specualtive desugared switch expression)
[Source](./bench/InitAndSwitch.cs) for specifics.
```

BenchmarkDotNet v0.15.8, Linux EndeavourOS
12th Gen Intel Core i5-1240P 0.40GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.104
  [Host]         : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  .NET 10.0      : .NET 10.0.4 (10.0.4, 42.42.42.42424), X64 RyuJIT x86-64-v3
  NativeAOT 10.0 : .NET 10.0.4, X64 NativeAOT x86-64-v3


```
| Method                         | Job            | Runtime        | Mean       | Error     | StdDev    | Median     | Gen0   | Allocated |
|------------------------------- |--------------- |--------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
| SpeculativeUnionImplementation | .NET 10.0      | .NET 10.0      |  7.2381 ns | 0.0466 ns | 0.0389 ns |  7.2236 ns | 0.0051 |      32 B |
| ReadOnlyGeneratedSequential    | .NET 10.0      | .NET 10.0      |  0.0059 ns | 0.0132 ns | 0.0110 ns |  0.0000 ns |      - |         - |
| GeneratedSequential            | .NET 10.0      | .NET 10.0      |  0.0031 ns | 0.0066 ns | 0.0058 ns |  0.0000 ns |      - |         - |
| GeneratedBoxed                 | .NET 10.0      | .NET 10.0      | 11.6213 ns | 0.0561 ns | 0.0468 ns | 11.6093 ns | 0.0051 |      32 B |
| GeneratedRawBoxed              | .NET 10.0      | .NET 10.0      |  7.1935 ns | 0.1327 ns | 0.1241 ns |  7.1621 ns | 0.0051 |      32 B |
| GeneratedStatically            | .NET 10.0      | .NET 10.0      |  0.1163 ns | 0.0477 ns | 0.0446 ns |  0.1022 ns |      - |         - |
| SpeculativeUnionImplementation | NativeAOT 10.0 | NativeAOT 10.0 |  1.2031 ns | 0.0256 ns | 0.0239 ns |  1.2007 ns |      - |         - |
| ReadOnlyGeneratedSequential    | NativeAOT 10.0 | NativeAOT 10.0 |  0.0008 ns | 0.0031 ns | 0.0033 ns |  0.0000 ns |      - |         - |
| GeneratedSequential            | NativeAOT 10.0 | NativeAOT 10.0 |  0.0989 ns | 0.0368 ns | 0.0361 ns |  0.1053 ns |      - |         - |
| GeneratedBoxed                 | NativeAOT 10.0 | NativeAOT 10.0 | 20.8178 ns | 0.4415 ns | 0.9505 ns | 20.5148 ns | 0.0051 |      32 B |
| GeneratedRawBoxed              | NativeAOT 10.0 | NativeAOT 10.0 |  5.7708 ns | 0.3616 ns | 1.0604 ns |  5.2420 ns | 0.0051 |      32 B |
| GeneratedStatically            | NativeAOT 10.0 | NativeAOT 10.0 |  0.0162 ns | 0.0167 ns | 0.0440 ns |  0.0000 ns |      - |         - |

> Most notable observation is that my [OpenGenericsHelpers](./src/UnionUtil/OpenGenericHelpers.cs) is noticably worse than i initially anticipated. I may be able to optimize this so read operation are near equivalent to `object` wrapping while still allowing to reuse the already allocated boxed structs in scenarios where they are possible. I'll likely simply go the object boxing approach for `readonly` generated types. 
