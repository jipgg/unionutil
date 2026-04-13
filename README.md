# UnionUtil
Experimental, minimally configurable source generator for discriminated union storage implementations in C#15 preview.

## Usage

Add `[UnionImpl]` to a partial class or struct implementing `UnionTypes<...>` with your variant types.

```cs
using UnionUtil;


[UnionImpl(
    BoxGenerics = false, // unconstrained generic arguments
    BoxManagedStructs = false,  // default, will store them as fields sequentially
    Mutable = false, // default, will make the fields readonly
    FieldVisibility = Visibility.Internal // mainly to inspect the union type fields + generated storage types or extend the functionality
)]
readonly struct Result<T, E>

[UnionImpl(Mutable = true)]
sealed partial class Shape : UnionTypes<Circle, Rect, Line>;
```
# Storage behavior
# `unmanaged`
Concrete types will always be compacted into a 'true' union storage using `[StructLayout]` with `[FieldOffset(0)]`.
```cs
[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Explicit)]
public struct Overlapped{
    [global::System.Runtime.InteropServices.FieldOffset(0)]
    public int _1;
    [global::System.Runtime.InteropServices.FieldOffset(0)]
    public float _2
public readonly Overlapped _overlapped = default;
```
### `where T: unmanaged`
Counterintuitively, a parameter constrained to unmanaged will follow the same rules as `where T: struct`.
The reason for this is that, even though it is completely legal for the compiler to overlap memory layout for these
just like a concrete unmanaged type, the runtime currently does not allow it. Overlapping generic fields will always result in a
`TypeLoadException`.
# `struct` (managed)
By default these will be laid out in memory sequentially. However, these can also be stored as boxed values if necessary with `MethodImpl(BoxManagedStructs = true)`.
When a generic is constrained with `where T: struct`, it will follow the same rules as concrete value types.
# `class` (reference types)
Will always be stored in a shared `object` field, as this can be trivially casted back to its more concrete type.
Where a generic is constrained with `where T: class`, it will follow the same rules as concrete reference types.
