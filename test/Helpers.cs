namespace Test;

public static class TestUnion {
   public readonly record struct Metadata(
      bool BoxesUnconstrainedGenerics,
      bool BoxesStructs,
      bool IsReadOnly,
      bool IsNullable,
      int SmallBufferSize,
      int TypeArgumentCount
   );
   public static async Task AssertSameMetadata<TUnion>(TUnion _, Metadata expected) where TUnion : IUnionType {
      await Assert.That(TUnion.BoxesUnconstrainedGenerics).IsEqualTo(expected.BoxesUnconstrainedGenerics);
      await Assert.That(TUnion.BoxesStructs).IsEqualTo(expected.BoxesStructs);
      await Assert.That(TUnion.IsReadOnly).IsEqualTo(expected.IsReadOnly);
      await Assert.That(TUnion.IsNullable).IsEqualTo(expected.IsNullable);
      await Assert.That(TUnion.SmallBufferSize).IsEqualTo(expected.SmallBufferSize);
      await Assert.That(TUnion.TypeArgumentCount).IsEqualTo(expected.TypeArgumentCount);
   }
}
