using UnionUtil;
namespace Test;

public static class TestUnion {
   public static void Reassign<TUnion, [CanHold] T, [CanHold] X>(ref TUnion u, T v, X y) where TUnion : IUnionType {
      Assert.True(TUnion.CanHoldType<T>());
      Assert.True(u.TrySetValue(v));
      Assert.True(u.HoldsType<T>());
      Assert.Equal(v, u.Value);
      Assert.True(u.HasValue);
   }
   public static void Equal<TUnion, [CanHold] T>(ref TUnion u, T v) where TUnion : IUnionType {
      Assert.True(TUnion.CanHoldType<T>() && u.HoldsType<T>());
      Assert.True(u.TryGetValue(out T x));
      Assert.Equal(v, x);
   }
}
