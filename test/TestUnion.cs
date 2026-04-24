using UnionUtil;
namespace Test;

public static class TestUnion {
   public static void Reassign<TUnion, [FromUnion(nameof(TUnion))] T, X>(ref TUnion u, T v, X y) where TUnion : IUnion {
      Assert.True(u.TrySetValue(v));
      Assert.Equal(v, u.Value);
      Assert.True(u.HasValue);
   }
   public static void Equal<TUnion, T>(ref TUnion u, T v) where TUnion : IUnion {
      Assert.True(u.TryGetValue(out T x));
      Assert.Equal(v, x);
   }
}
