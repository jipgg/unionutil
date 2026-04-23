using UnionUtil;

[UnionImpl(UnionImplOptions.NullableEnabled | UnionImplOptions.ReadOnlyEnabled)]
readonly partial struct GlobalMustCompile: IUnion<int, float, double, nint, IEnumerable<object>>;
