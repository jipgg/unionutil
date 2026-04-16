using UnionUtil;
namespace Test.MustCompile;

[UnionImpl(
   FieldVisibility = Visibility.Public
)]
sealed partial class SealedClass : IUnion<int, float, object, List<object>>;

