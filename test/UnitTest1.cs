using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnionUtil;
namespace Test;

using static Result;
using static MutableTag;
using static UnionImplOptions;

public enum MutableTag { Int = 9, Double = 1, Vector3 = -3 }

[Tagged<MutableTag>]
[UnionImpl(NullableEnabled)]
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
         { Tag: Double, Double: var d } => ((int)d),
         { Tag: Vector3, Vector3: var v } => (int)v.X,
         { Tag: Int, Int: var i } => i,
         { Tag: null } => 0,
         _ => throw new(),
      };
   }
   [Fact]
   public void Boxed() {
      BoxedResult<int, Exception> r = 1;
      Assert.Equal(Ok, r.Tag);
      Assert.True(r.HoldsType<int>());
      r.HoldsType<int>();
      Assert.False(r.HoldsType<Exception>());
      Assert.Equal(1, r.Ok);
      r.Ok += 123;
      Assert.ThrowsAny<InvalidOperationException>(() => r.Err);
      Assert.Equal(124, r.Ok);
      r.Err = new("abc");
      Assert.True(r.HoldsType<Exception>());
      Assert.False(r.HoldsType<int>());
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
      Sbo23<int, double, Exception> sbo23 = 0.5;
      Union<int, double, Exception> x = 0.5;
      var e = (double)x;
      Assert.Equal(32, Unsafe.SizeOf<Sbo23<int, double, Exception>>());
      Assert.Null(sbo23._box);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo23._sbo.Data));
      Assert.Equal(2, sbo23._index);

      Sbo55<int, double, Exception> sbo55 = 0.5;
      Assert.Equal(64, Unsafe.SizeOf<Sbo55<int, double, Exception>>());
      Assert.Null(sbo55._box);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo55._sbo.Data));
      Assert.Equal(2, sbo55._index);

      Sbo15<int, double, Exception> sbo15 = 0.5;
      Assert.Equal(24, Unsafe.SizeOf<Sbo15<int, double, Exception>>());
      Assert.Null(sbo15._box);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo15._sbo.Data));
      Assert.Equal(2, sbo15._index);

      Sbo7<int, double, Exception> sbo7 = 0.5;
      Assert.Equal(16, Unsafe.SizeOf<Sbo7<int, double, Exception>>());
      Assert.NotNull(sbo7._box);
      Assert.True(sbo7.HoldsType<double>());
      Assert.True(sbo7.TryGetValue(out double d));
      Assert.Equal(0.5, d);
      Assert.Equal(2, sbo7._index);
      sbo7.SetValue(1);
      Assert.Equal(1, Unsafe.As<byte, int>(ref sbo7._sbo.Data));
      Assert.Equal(1, sbo7._index);
   }
}
