global using UnionUtil;
global using System.Diagnostics.CodeAnalysis;
global using static UnionUtil.UnionGeneratorOptions;
global using System.Runtime.CompilerServices;
global using TUnit.Assertions;
namespace Test;

[GenerateUnion(EnableNullable | EnableUnionTypeInterface | EnableReadOnly)]
[UnionTypeArguments<int, float, double>]
partial class TestCase_A;

[GenerateUnion(EnableUnionTypeInterface | BoxStructs | BoxUnconstrainedGenerics)]
[SmallBufferOptimized(123)]
[UnionTypeArguments<int, float, Exception, double>]
partial struct TestCase_B<T> : IUnionTypeArguments<int, float, Exception, T>;

[InlineArray(12)]
struct TestSmallBuffer : ISmallBuffer {
   byte _x;
   static int ISmallBuffer.Size => 12;
   [UnscopedRef]
   ref byte ISmallBuffer.Data => ref _x;
}
[GenerateUnion(EnableUnionTypeInterface | BoxUnconstrainedGenerics)]
[SmallBufferOptimized<TestSmallBuffer>]
partial struct TestCase_C<T, U>;

public class GenerationCorrectnessTests {
   [Test]
   public async Task SameMetadata_A() {
      await TestUnion.AssertSameMetadata(new TestCase_A(123), new(
         BoxesUnconstrainedGenerics: false,
         BoxesStructs: false,
         IsReadOnly: true,
         IsNullable: true,
         SmallBufferSize: 0,
         TypeArgumentCount: 3
      ));
   }
   [Test]
   public async Task SameMetadata_B() {
      await TestUnion.AssertSameMetadata(new TestCase_B<double>(new Exception()), new(
         BoxesUnconstrainedGenerics: true,
         BoxesStructs: true,
         IsReadOnly: false,
         IsNullable: false,
         SmallBufferSize: 123,
         TypeArgumentCount: 4
      ));
   }
   [Test]
   public async Task SameMetadata_C() {
      await TestUnion.AssertSameMetadata(new TestCase_C<double, int>(1), new(
         BoxesUnconstrainedGenerics: true,
         BoxesStructs: false,
         IsReadOnly: false,
         IsNullable: false,
         SmallBufferSize: 12,
         TypeArgumentCount: 2
      ));
   }
}
