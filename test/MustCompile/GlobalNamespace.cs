using UnionUtil;

[UnionImpl(UnionImplOptions.EnableNullable | UnionImplOptions.EnableReadOnly)]
readonly partial struct GlobalMustCompile: ICanHold<int, float, double, nint, IEnumerable<object>>;
