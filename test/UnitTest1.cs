using System.Numerics;
using UnionUtil;
namespace Test;

[UnionImpl(
   WithSetValueOverloads = true,
   Nullable = true,
   FieldVisibility = Visibility.Public,
   WithFieldAccessors = true,
   FieldAccessorNames = ["Int", "Double", "Vector3"]
)]
[UnionCases<int, double, Vector3>("Int", "Double", "Vector3")]
partial struct MutableStruct;

[UnionImpl(
   WithFieldAccessors = true,
   FieldAccessorNames = ["Ok", "Error"],
   BoxManagedStructs = true,
   BoxOpenGenerics = true,
   FieldVisibility = Visibility.Internal
), UnionCases("Ok", "Error")]
partial struct Result<T, E> : IUnionCases<T, E>;

public class MutableStructTests {
   [Fact]
   public void CanAssign() {
      MutableStruct m = 1;

      m.Int = 2;
      Assert.Throws<InvalidOperationException>(() => m.Double);
      Assert.Equal(2, m.Int);
      // Assert.True(m.TryGetValue(out int v));
      // Assert.Equal(2, v);
   }
   [Fact]
   public void Boxed() {
      Result<int, Exception> r = 1;
      Assert.True(r.IsOk);
      Assert.Equal(1, r.Ok);
      r.Ok += 123;
      Assert.Throws<InvalidOperationException>(() => r.Error);
      Assert.Equal(124, r.Ok);
   }
}
