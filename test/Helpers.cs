using TUnit.Assertions;
using UnionUtil;
namespace Test;

public static class TestUnion {
   public readonly record struct Metadata(
      bool BoxesOpenGenerics,
      bool BoxesManagedStructs,
      bool IsReadOnly,
      bool IsNullable,
      int SmallBufferSize,
      int TypeCount
   );
   public static async Task AssertSameMetadata<TUnion>(TUnion _, Metadata expected) where TUnion : IUnionType {
      await Assert.That(TUnion.BoxesOpenGenerics).IsEqualTo(expected.BoxesOpenGenerics);
      await Assert.That(TUnion.BoxesManagedStructs).IsEqualTo(expected.BoxesManagedStructs);
      await Assert.That(TUnion.IsReadOnly).IsEqualTo(expected.IsReadOnly);
      await Assert.That(TUnion.IsNullable).IsEqualTo(expected.IsNullable);
      await Assert.That(TUnion.SmallBufferSize).IsEqualTo(expected.SmallBufferSize);
      await Assert.That(TUnion.TypeCount).IsEqualTo(expected.TypeCount);
   }
}
