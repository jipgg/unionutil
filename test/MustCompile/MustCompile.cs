using UnionUtil;
namespace Test.MustCompile;

[UnionImpl(
   FieldVisibility = Visibility.Public
)]
sealed partial class SealedClass : IUnion<int, float, object, List<object>>;


[UnionImpl(ReadOnly = true), Union<int, float>]
partial struct ReadonlyStruct;
