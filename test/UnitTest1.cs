using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnionUtil;
namespace Test;

using static Result;
using static MutableTag;

public enum MutableTag { Int, Double, Vector3 }

[Tagged<MutableTag>("Is")]
[UnionImpl(Nullable = true)]
[Union<int, double, Vector3>]
partial struct MutableStruct;

public enum Case { A, B, C, D, E, F, G }
[Tagged<Case>("Case"), UnionImpl(
   FieldVisibility = Visibility.Internal
)]
public partial class BasicUnion<TA, TB, TC, TD, TE, TF, TG> : IUnion<TA, TB, TC, TD, TE, TF, TG>;

public class MutableStructTests {
   [Fact]
   public void CanAssign() {
      MutableStruct m = 1;
      m.Int = 2;
      Assert.ThrowsAny<InvalidOperationException>(() => m.Double);
      Assert.Equal(2, m.Int);
      m.Double = 1;
      m.Double = 23;
      BasicUnion<int, float, double, nint, nuint, uint, object> b = 1;

      m = m switch {
         { Is: Double, Double: var d } => ((int)d),
         { Is: Vector3, Vector3: var v } => (int)v.X,
         { Is: Int, Int: var i } => i,
         { Is: null } => 0,
         _ => throw new(),
      };
   }
   [Fact]
   public void Boxed() {
      BoxedResult<int, Exception> r = 1;
      Assert.Equal(Ok, r.Tag);
      Assert.Equal(1, r.Ok);
      r.Ok += 123;
      Assert.ThrowsAny<InvalidOperationException>(() => r.Err);
      Assert.Equal(124, r.Ok);
      r.Err = new("abc");
      Assert.Equal(Err, r.Tag);
      Assert.ThrowsAny<InvalidOperationException>(() => r.Ok);
      Assert.Equal("abc", r.Err.Message);

      BoxedResult<int> r2 = r;
      Assert.Equal("abc", r.Err.Message);
      r2 = 10;
      Assert.Equal(10, r2.Ok);

      string x = r switch {
         { Tag: Err, Err: var e } => e.Message,
         { Tag: Ok, Ok: var k } => k.ToString(),
         _ => throw new(),
      };
      Assert.Equal("abc", x);
   }
   [Fact]
   public void SBOWorks() {
      Sbo16<int, double, Exception> sbo16 = 0.5;
      Assert.Equal(16, Unsafe.SizeOf<Sbo7<int, double, Exception>>());
      Assert.Null(sbo16._object);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo16._sbo.Data));
      Assert.Equal(2, sbo16._index);

      Sbo7<int, double, Exception> sbo7 = 0.5;
      Assert.Equal(16, Unsafe.SizeOf<Sbo7<int, double, Exception>>());
      Assert.NotNull(sbo7._object);
      Assert.True(sbo7.TryGetValue(out double d));
      Assert.Equal(0.5, d);
      Assert.Equal(2, sbo7._index);
      sbo7.SetValue(1);
      Assert.Equal(1, Unsafe.As<byte, int>(ref sbo7._sbo.Data));
      Assert.Equal(1, sbo7._index);
   }
}

