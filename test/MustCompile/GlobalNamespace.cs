using UnionUtil;

[UnionImpl]
readonly partial struct Global: UnionTypes<int, float, double, nint, IEnumerable<object>>;
