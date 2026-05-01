using UnionUtil;
namespace Test.MustCompile;
using static UnionImplOptions;

[UnionImpl(FieldVisibility = Visibility.Public)]
sealed partial class SealedClass : ICanHold<int, float, object, List<object>>;


[UnionImpl(EnableReadOnly), CanHold<int, float>]
partial struct ReadonlyStruct;

[UnionImpl(BoxOpenGenerics), SmallBufferOptimized]
readonly partial struct ReadonlyStructSbo<T, U> : ICanHold<T, U>;

[UnionImpl(BoxOpenGenerics)]
readonly partial struct ReadonlyStruct<T, U> : ICanHold<T, U>;

[UnionImpl]
readonly partial struct ReadonlyStructSequential<T, U> : ICanHold<T, U>;
