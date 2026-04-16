# UnionUtil
Experimental source generator utilities for the upcoming `union`s language feature in C#15.
## Main feature set
* Generated unions are congruent with the current compiler feature in the .NET11 preview
* Concrete `unmanaged` types always share the same storage memory
* Configurability of whether certain types should be stored sequentially or boxed
* Optimized boxing access and overwrite where applicable.
* Optionally tag the union with the `Tagged<TEnum>` attribute for labelled properties.

# Current limitatios

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
