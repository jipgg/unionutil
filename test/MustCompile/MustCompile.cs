using UnionUtil;
namespace Test.MustCompile;
using static UnionImplOptions;

[UnionImpl(FieldVisibility = Visibility.Public)]
sealed partial class SealedClass : ICanHoldTypes<int, float, object, List<object>>;


[UnionImpl(EnableReadOnly), CanHoldTypes<int, float>]
partial struct ReadonlyStruct;

[UnionImpl(BoxOpenGenerics), SmallBufferOptimized]
readonly partial struct ReadonlyStructSbo<T, U> : ICanHoldTypes<T, U>;

[UnionImpl(BoxOpenGenerics)]
readonly partial struct ReadonlyStruct<T, U> : ICanHoldTypes<T, U>;

[UnionImpl]
readonly partial struct ReadonlyStructSequential<T, U> : ICanHoldTypes<T, U>;
