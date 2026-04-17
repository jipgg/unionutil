using System.Numerics;
using System.Runtime.CompilerServices;
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

      string x = r switch {
         { Tag: Err, Err: var e } => e.Message,
         { Tag: Ok, Ok: var k } => k.ToString(),
         _ => throw new(),
      };
      Assert.Equal("abc", x);
   }
}
