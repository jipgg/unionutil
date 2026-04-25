using UnionUtil;

[UnionImpl(UnionImplOptions.EnableNullable | UnionImplOptions.EnableReadOnly)]
readonly partial struct GlobalMustCompile: ICanHoldTypes<int, float, double, nint, IEnumerable<object>>;
