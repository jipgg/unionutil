using UnionUtil;

[UnionImpl(Nullable = true, ReadOnly = true)]
readonly partial struct GlobalMustCompile: IUnion<int, float, double, nint, IEnumerable<object>>;
