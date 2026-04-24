# UnionUtil
Source generator utilities for the upcoming `union`s language feature in C#15.

> The nature of the project is quite informal at the moment with volatile changes being the norm. In the scenario where this does seem useful to more people that myself alone, i may add a stable api version and add proper diagnostics, tests and analyzers.
 
Perfectly usable in .NET10, but does not have support for the fancy switch expression for matching that is available in the .NET11 preview. In these scenarios id recommend using the `[Tagged<TEnum>]` attribute for generating named properties and using pattern matching in the switch expression to match over them (`{ Tag: Tag.X, X: var x }`).

The currently proposed default implementation of `union` types will be `readonly struct(object, int)` to my understanding, meaning it'll box value types and generics. This is arguably the best ccompromise considering the runtime limitations, but may not always be what you want.

Unions will support custom, user supplied implementations, hence the reasoning for this source generator. Mainly focusing on resolving a 'roughly' optimal memory layout by default as well as some optional parameters to finetune the generated source of the union.

## Main feature set
* Generated unions are congruent with the current compiler feature structural interface in the .NET11 preview
* Concrete `unmanaged` types always share the same storage in memory
* SBO support for avoiding boxing of small `unmanaged` types in unions with open generics
* Configurability of whether certain types should be stored sequentially or boxed
* Support for mutable union types
* Optionally tag the union with the `Tagged<TEnum>` attribute for labelled properties.
# Current limitations
## `where T: unmanaged`
While legal for the compiler, Overlapping generic fields are not allowed by the runtime, even when constrained to unmanaged. Meaning `where T: unmanaged` will follow the same rules as `where T: struct` for the time being.

For unions with `BoxOpenGenerics = true` or `BoxManagedStructs = true` you can optionally specify `[SmallBufferOptimized(uint sizeInBytes)]` as an optimization to avoid boxing whenever a generic unmanaged type's size fits into the small buffer field to avoid allocation. The default size for this SBO buffer is 7 bytes. The reasoning for this seemingly arbitrary size is that this results in the memory layout of the union to simply recycle the padding otherwise created between the `object` boxing field and the `byte` type index field, keeping the struct size the same as without the SBO in these scenarios.
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
