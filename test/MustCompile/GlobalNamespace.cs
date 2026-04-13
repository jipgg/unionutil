using UnionUtil;

[UnionImpl(Nullable = true)]
readonly partial struct Global: UnionTypes<int, float, double, nint, IEnumerable<object>>;
