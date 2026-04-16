using UnionUtil;

[UnionImpl(Nullable = true, ReadOnly = true)]
readonly partial struct GlobalMustCompile: IUnionCases<int, float, double, nint, IEnumerable<object>>;
