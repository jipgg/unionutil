using UnionUtil;
namespace Test.MustCompile;
using static UnionImplOptions;

[UnionImpl(
   FieldVisibility = Visibility.Public
)]
sealed partial class SealedClass : IUnion<int, float, object, List<object>>;


[UnionImpl(ReadOnly), Union<int, float>]
partial struct ReadonlyStruct;

[UnionImpl(BoxOpenGenerics), SmallBufferOptimized]
readonly partial struct ReadonlyStructSbo<T, U> : IUnion<T, U>;

[UnionImpl(BoxOpenGenerics)]
readonly partial struct ReadonlyStruct<T, U> : IUnion<T, U>;

[UnionImpl]
readonly partial struct ReadonlyStructSequential<T, U> : IUnion<T, U>;
