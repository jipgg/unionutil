# UnionUtil
[![NuGet Version](https://img.shields.io/nuget/v/UnionUtil?label=NuGet&logo=nuget)](https://www.nuget.org/packages/UnionUtil)
[![NuGet Downloads](https://img.shields.io/nuget/dt/UnionUtil?label=Downloads&logo=nuget)](https://www.nuget.org/packages/UnionUtil)
Source generator utilities trying to be congruent with the upcoming `union`s language feature in C#15.
Strong focus on generating 'roughly' optimal union storage layouts within the bounds of what is allowed in the runtime.
Project has been downgraded to .NET7 for compatibility.

## Motivation
This project initially started as an experiment playing around with the .NET11 union preview feature.
The currently proposed default implementation of `union` types will be `readonly struct(object, int)` to my understanding, meaning it'll box value types and generics. This is arguably the best ccompromise considering the runtime limitations, but may not always be what you want.
Unions will support custom, user supplied implementations, hence the reasoning for this source generator. Mainly focusing on resolving a 'roughly' optimal memory layout by default as well as some optional parameters to finetune the generated source of the union.


## Main feature set
* Generated unions are congruent with the current compiler feature structural interface in the .NET11 preview
* Concrete `unmanaged` types always share the same storage in memory in non-generic unions
* Small buffer optimization support to avoid boxing of small `unmanaged` types in unions with open generics
* Configurability of whether certain types should be stored sequentially or boxed
* Support for mutable union types
* Optionally tag the union with the `Tagged<TEnum>` attribute for labelled properties.
* `IUnionType` for a non-boxing, type order agnostic common interface in generic contexts.
* Analyzers and attributes for working with `IUnionType` ergonomically.
* Compatibility extension methods broadly simulating the `switch` expression semantics for older projects.
# Current limitations
## `where T: unmanaged`
While legal for the compiler, overlapping generic fields are not allowed by the runtime, even when constrained to unmanaged. Meaning `where T: unmanaged` will follow the same rules as `where T: struct` for the time being.

For unions with `BoxOpenGenerics` or `BoxManagedStructs` you can optionally specify `[SmallBufferOptimized(uint sizeInBytes)]` or `[SmallBufferOptimized<TSmallBuffer>]` as an optimization to avoid boxing whenever a generic unmanaged type's size fits into the small buffer field to avoid allocation. The default size for this SBO buffer is 7 bytes. The reasoning for this seemingly arbitrary size is that this results in the memory layout of the union to simply recycle the padding otherwise created between the `object` boxing field and the `byte` type index field, keeping the struct size the same as without the SBO in these scenarios.
# Installation
```sh
dotnet package add UnionUtil
```
or add it as a project refecence in your `.csproj`:
```csproj
  <ItemGroup>
    <ProjectReference Include="dir\to\unionutil\src\UnionUtil\UnionUtil.csproj" />
     <ProjectReference
        Include="dir\to\unionutil\src\UnionUtil.Meta\UnionUtil.Meta.csproj" 
        OutputItemType="Analyzer"
        ReferenceOutputAssembly="false"
      />
  </ItemGroup>

```
# Usage Examples
Generating new unions:
```cs
using UnionUtil;
using static UnionUtil.UnionGeneratorOptions;
// basic union with statically know type cases
[GenerateUnion, UnionTypeArguments<int, double, DateTime>]
partial struct MyUnion;
// for trivial generic cases
[GenerateUnion(EnableReadOnly | EnableNullable)]
sealed partial class MyOtherUnion<T, U, V, W>;
// for non-trivial generic cases
[GenerateUption(ImplementUnionInterfaces)] // will implement `IUnionType`
readonly partial struct Union<T, U, V> : IUnionTypeArguments<T, U, List<V>, double>;
// will box but store unmanaged values smaller than 15 bytes inside the inline buffer
[GenerateUnion(BoxOpenGenerics | WithExplicitConversionsToValue), SmallBufferOptimized(15)]
partial struct MyGenericUnion<T, U, V, W, X, Y, Z>;
// tagged union
public enum Result { Ok, Err }
[GenerateUnion, Tagged<Result>]
partial struct Result<T> : IUnionTypeArguments<T, Exception>;
```
Using the unions:
```cs
Union<int, bool, double> myUnion = 123;
// This will be the main supported compatibility switch expression syntax
// for projects targetting older .NET standards or language versions.
// requires `using UnionUtil.UnionTypeExtensions`
var asInt = myUnion.Switch( // does not care about generic type order
    (int i) => 1,
    (float f) => -1, // analyzer will emit warning that the union can never hold `float`
    (int i) => -1, // analyzer will emit warning that this was previously declared
    (double d) => 3,
    (bool b) => 2,
    () => 4 // default case
);
Result<int[]> result = [1, 2, 3];
// pattern match over tags
var str = result switch {
    {Tag: Result.Ok, Ok: var ok} => $"[{string.Join(", ", ok.Select(e => e.ToString()))}]",
    {Tag: Result.Err, Err: var err} => err.Message,
};

// working with unions generically
// `[CanHold(nameof(TUnion))] T` marks a type parameter to be analyzed at the point invocation
// and emit a warning if TUnion can not realistically ever hold T. 
static TNumber UseUnion<TUnion, [CanHold] TNumber>(in TUnion u, TNumber v)
    where TUnion: IUnionType where TNumber : INumber<TNumber>  {

    if (!TUnion.CanHold<double>()) throw new(); // inspect whether a certain type can be held by the union
    var sboSize = TUnion.SmallBufferSize; // inspect the sbo size
    var typeCount = TUnion.TypeCount; // inspect the count of types the union can hold
    var holdsType = u.Holds<TNumber>(); // check if the union holds a specific type

    if (u.TryGetValue(out TNumber n)) return n * v; // the main magic of IUnionType which implements a generic TryGetValue<T>(out T v)
    else throw new();
}

```
# Benchmarks
Benchmark reports can be found [here](./bench/reports/).
They wont always be up-to-date, nor always be a good representative for real-world code.
Mainly used to visualize how well internal implementations compare to other ones, so i can
determine which implementations are underperforming.
