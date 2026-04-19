using UnionUtil;
namespace Test.MustCompile;

[UnionImpl(
   FieldVisibility = Visibility.Public
)]
sealed partial class SealedClass : IUnion<int, float, object, List<object>>;


[UnionImpl(ReadOnly = true), Union<int, float>]
partial struct ReadonlyStruct;

[UnionImpl(BoxOpenGenerics = true), SmallBufferOptimized]
readonly partial struct ReadonlyStructSbo<T, U> : IUnion<T, U>;
