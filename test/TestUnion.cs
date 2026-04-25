using UnionUtil;
namespace Test;

public static class TestUnion {
   public static void Xy<[FromUnion] T>() {
   }
   public static void Reassign<TUnion, [FromUnion] T, [FromUnion] X>(ref TUnion u, T v, X y) where TUnion : IUnion {
      Assert.True(TUnion.CanHoldType<T>());
      Xy<T>();
      Assert.True(u.TrySetValue(v));
      Assert.True(u.HoldsType<T>());
      Assert.Equal(v, u.Value);
      Assert.True(u.HasValue);
   }
   public static void Equal<TUnion, [FromUnion] T>(ref TUnion u, T v) where TUnion : IUnion {
      Assert.True(TUnion.CanHoldType<T>() && u.HoldsType<T>());
      Assert.True(u.TryGetValue(out T x));
      Assert.Equal(v, x);
   }
}
