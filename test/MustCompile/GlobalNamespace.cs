using UnionUtil;

[UnionImpl(UnionImplOptions.EnableNullable | UnionImplOptions.EnableReadOnly)]
readonly partial struct GlobalMustCompile: IUnion<int, float, double, nint, IEnumerable<object>>;
