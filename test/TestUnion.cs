using UnionUtil;
namespace Test;

public static class TestUnion {
   public static void Reassign<TUnion, [CanHold] T, [CanHold] X>(ref TUnion u, T v, X y) where TUnion : IUnionType {
      Assert.True(TUnion.CanHold<T>());
      Assert.True(u.TrySetValue(v));
      Assert.True(u.Holds<T>());
      Assert.Equal(v, u.Value);
      Assert.True(u.HasValue);
   }
   public static void Equal<TUnion, [CanHold] T>(ref TUnion u, T v) where TUnion : IUnionType {
      Assert.True(TUnion.CanHold<T>() && u.Holds<T>());
      Assert.True(u.TryGetValue(out T x));
      Assert.Equal(v, x);
   }
   public readonly record struct Metadata(
      bool BoxesOpenGenerics,
      bool BoxesManagedStructs,
      bool IsReadOnly,
      bool IsNullable,
      int SmallBufferSize,
      int TypeCount
   );
   public static void AssertSameMetadata<TUnion>(in TUnion u, Metadata expected) where TUnion: IUnionType {
      Assert.Equal(expected.BoxesOpenGenerics, TUnion.BoxesOpenGenerics);
      Assert.Equal(expected.BoxesManagedStructs, TUnion.BoxesManagedStructs);
      Assert.Equal(expected.IsReadOnly, TUnion.IsReadOnly);
      Assert.Equal(expected.IsNullable, TUnion.IsNullable);
      Assert.Equal(expected.SmallBufferSize, TUnion.SmallBufferSize);
      Assert.Equal(expected.TypeCount, TUnion.TypeCount);
   }
}
