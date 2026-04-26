using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnionUtil;
namespace Test;

using static Result;
using static MutableTag;
using static UnionImplOptions;

static class MatchExtensions {
   public static R Match2<TUnion, [CanHold(unique: true)] T1, [CanHold(unique: true)] T2, R>(this TUnion u, Func<T1, R> f1, Func<T2, R> f2, Func<R>? @default = null) where TUnion : IUnionType {
      if (u.TryGetValue(out T1 v1)) return f1(v1);
      if (u.TryGetValue(out T2 v2)) return f2(v2);
      if (@default is not null) return @default();
      throw new InvalidOperationException();
   }
   extension<TUnion>(TUnion u) where TUnion : IUnionType {
      public R Match<[CanHold] T1, R>(Func<T1, R> f1, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Match<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, R>(Func<T1, R> f1, Func<T2, R> f2, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Match<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
   }
}

public enum MutableTag { Int = 9, Double = 1, Vector3 = -3 }

[Tagged<MutableTag>]
[UnionImpl(EnableNullable)]
[CanHoldTypes<int, double, Vector3>]
partial struct MutableStruct;

[UnionImpl(ImplementHoldsTypeMethod | ImplementUnionInterfaces)]
partial struct ClassUnion<T, U, V> where T : class where U : class where V : class;

[UnionImpl(BoxOpenGenerics | ImplementUnionInterfaces,
      FieldVisibility = Visibility.Internal), SmallBufferOptimized]
partial struct Union<T, U, V>;

[UnionImpl]
partial struct MutableStruct2<T> where T : struct;

public enum Case { A, B, C, D, E, F, G }
[Tagged<Case>("Case"), UnionImpl(
   FieldVisibility = Visibility.Internal
)]
public partial class BasicUnion<TA, TB, TC, TD, TE, TF, TG>;

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
   static void TestCommonInterface<TUnion, [CanHold] T, [CanHold] U>(ref TUnion u, T v, U v2) where TUnion : IUnionType where T : IEquatable<T> {
      Assert.True(u.TryGetValue(out T x));
      Assert.True(u.HoldsType<T>());
      Assert.Equal(v, x);
      Assert.True(TUnion.CanHoldType<T>());
      Assert.True(u.TrySetValue((v2)));
      Assert.True(u.TryGetValue(out U x2));
      Assert.Equal(v2, x2);
      Assert.True(TUnion.CanHoldType<U>());
      Assert.True(u.HoldsType<U>());
   }
   [Fact]
   public void SBOWorks() {
      Sbo23<int, double, Exception> sbo23 = 0.5;
      Assert.Equal(32, Unsafe.SizeOf<Sbo23<int, double, Exception>>());
      Assert.Null(sbo23._box);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo23._sbo.Data));
      Assert.Equal(2, sbo23._index);
      sbo23 = 0.5;
      TestCommonInterface(ref sbo23, 0.5, new Exception("eeeeee"));

      Sbo55<int, double, Exception> sbo55 = 0.5;
      Assert.Equal(64, Unsafe.SizeOf<Sbo55<int, double, Exception>>());
      Assert.Null(sbo55._box);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo55._sbo.Data));
      Assert.Equal(2, sbo55._index);
      sbo55 = 0.5;
      TestCommonInterface(ref sbo55, 0.5, 0.10);

      Sbo15<int, double, Exception> sbo15 = 0.5;
      Assert.Equal(24, Unsafe.SizeOf<Sbo15<int, double, Exception>>());
      Assert.Null(sbo15._box);
      Assert.Equal(0.5, Unsafe.As<byte, double>(ref sbo15._sbo.Data));
      Assert.Equal(2, sbo15._index);
      sbo15 = 0.5;
      TestCommonInterface(ref sbo15, 0.5, new Exception("eewawadaw"));

      Sbo7<int, double, Exception> sbo7 = 0.5;
      Assert.Equal(16, Unsafe.SizeOf<Sbo7<int, double, Exception>>());
      Assert.NotNull(sbo7._box);
      IUnionType sv = sbo7;
      Assert.True(sv.HoldsType<double>());
      Assert.False(sv.HoldsType<object>());
      Assert.True(sbo7.HoldsType<double>());
      Assert.True(sbo7.TryGetValue(out double d));
      Assert.Equal(0.5, d);
      Assert.Equal(2, sbo7._index);
      sbo7.SetValue(1);
      Assert.Equal(1, Unsafe.As<byte, int>(ref sbo7._sbo.Data));
      Assert.Equal(1, sbo7._index);
      sbo7 = 0.5;
      TestCommonInterface(ref sbo7, 0.5, 1.23);
   }

   [Fact]
   public void TestSetValue() {
      Union<int, Int128, double[]> u = 1;
      var str = u.Switch(
         static (int i) => $"{i}",
         static (Int128 i) => $"{i}",
         static () => "abc"
      );
      var str2 = u.Switch(
         static (Int128 i) => $"{i}",
         static (int i) => $"{i}",
         static (double[] d) => $"{d}",
         static () => "abc"
      );
      var c = u.Switch(
         static (int x) => x,
         static (double[] d) => d.Sum(),
         static (Int128 x) => ((int)x)
      );
      TestUnion.Reassign(ref u, 1, 123);
      Int128 x = new(123, 123);
      u.SetValue(x);
      Assert.True(u.TryGetValue(out Int128 u1));
      Assert.Equal(x, u1);
      u.SetValue(1);
      Assert.False(u.TryGetValue(out Int128 _));
      Assert.Equal(1, u.Value);
   }
}
