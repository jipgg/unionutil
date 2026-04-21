using UnionUtil;

[UnionImpl(UnionImplOptions.Nullable | UnionImplOptions.ReadOnly)]
readonly partial struct GlobalMustCompile: IUnion<int, float, double, nint, IEnumerable<object>>;
