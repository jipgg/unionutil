global using UnionUtil;
global using System.Diagnostics.CodeAnalysis;
global using static UnionUtil.UnionImplOptions;
global using System.Runtime.CompilerServices;
namespace Test;

[UnionImpl(EnableNullable | ImplementUnionInterfaces | EnableReadOnly)]
[CanHold<int, float, double>]
partial class TestCase_A;

[UnionImpl(ImplementUnionInterfaces | BoxManagedStructs | BoxOpenGenerics)]
[SmallBufferOptimized(123)]
[CanHold<int, float, Exception, double>]
partial struct TestCase_B<T> : ICanHold<int, float, Exception, T>;

[InlineArray(12)]
struct TestSmallBuffer : ISmallBuffer {
   byte _x;
   static int ISmallBuffer.Size => 12;
   [UnscopedRef]
   ref byte ISmallBuffer.Data => ref _x;
}
[UnionImpl(ImplementUnionInterfaces | BoxOpenGenerics)]
[SmallBufferOptimized<TestSmallBuffer>]
partial struct TestCase_C<T, U>;

public class GenerationCorrectness {
   [Fact]
   public void SameMetadata_A() {
      TestUnion.AssertSameMetadata(new TestCase_A(123), new(
         BoxesOpenGenerics: false,
         BoxesManagedStructs: false,
         IsReadOnly: true,
         IsNullable: true,
         SmallBufferSize: 0,
         TypeCount: 3
      ));
   }
   [Fact]
   public void SameMetadata_B() {
      TestUnion.AssertSameMetadata(new TestCase_B<double>(new Exception()), new(
         BoxesOpenGenerics: true,
         BoxesManagedStructs: true,
         IsReadOnly: false,
         IsNullable: false,
         SmallBufferSize: 123,
         TypeCount: 4
      ));
   }
   [Fact]
   public void SameMetadata_C() {
      TestUnion.AssertSameMetadata(new TestCase_C<double, int>(1), new(
         BoxesOpenGenerics: true,
         BoxesManagedStructs: false,
         IsReadOnly: false,
         IsNullable: false,
         SmallBufferSize: 12,
         TypeCount: 2
      ));
   }
}
