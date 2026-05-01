using TUnit.Assertions;
namespace Test;

using static Result;
using static MutableTag;

public class MutableStructTests {
   [Test]
   public async Task Reassignment_UpdatesValue() {
      MutableStruct m = 1;
      m.Int = 2;
      await Assert.That(m.Int).IsEqualTo(2);
   }
   [Test]
   public async Task AccessingWrongVariant_Throws() {
      MutableStruct m = 1;
      await Assert.That(() => { _ = m.Double; }).ThrowsException();
   }
   [Test]
   public async Task SwitchExpression_MatchesCurrentVariant() {
      MutableStruct m = 1;
      m.Double = 3.0;

      MutableStruct result = m switch {
         { Tag: Double, Double: var d } => (int)d,
         { Tag: Vector3, Vector3: var v } => (int)v.X,
         { Tag: Int, Int: var i } => i,
         { Tag: null } => 0,
         _ => throw new(),
      };
      await Assert.That(result.Tag).IsEqualTo(Int);
      await Assert.That(result.Int).IsEqualTo(3);
   }
}
public class BoxedResultTests {
   [Test]
   public async Task IntAssignment_SetsOkTagAndValue() {
      BoxedResult<int, Exception> r = 1;
      await Assert.That(r.Tag).IsEqualTo(Ok);
      await Assert.That(r.Ok).IsEqualTo(1);
   }
   [Test]
   public async Task IntAssignment_HoldsInt_NotException() {
      BoxedResult<int, Exception> r = 1;
      await Assert.That(r.Holds<int>()).IsTrue();
      await Assert.That(r.Holds<Exception>()).IsFalse();
   }
   [Test]
   public async Task MutatingOk_ReflectsNewValue() {
      BoxedResult<int, Exception> r = 1;
      r.Ok += 123;
      await Assert.That(r.Ok).IsEqualTo(124);
   }
   [Test]
   public async Task AccessingErrWhenOk_Throws() {
      BoxedResult<int, Exception> r = 1;
      await Assert.That(() => { _ = r.Err; }).ThrowsException();
   }
   [Test]
   public async Task ExceptionAssignment_SetsErrTagAndValue() {
      BoxedResult<int, Exception> r = 1;
      r.Err = new("abc");
      await Assert.That(r.Tag).IsEqualTo(Err);
      await Assert.That(r.Err.Message).IsEqualTo("abc");
   }
   [Test]
   public async Task AfterErrAssignment_OkThrows_IntNotHeld() {
      BoxedResult<int, Exception> r = 1;
      r.Err = new("abc");
      await Assert.That(r.Holds<Exception>()).IsTrue();
      await Assert.That(r.Holds<int>()).IsFalse();
      await Assert.That(() => { _ = r.Ok; }).ThrowsException();
   }
   [Test]
   public async Task SwitchExpression_MatchesErrVariant() {
      BoxedResult<int, Exception> r = 1;
      r.Err = new("abc");

      string x = r switch {
         { Tag: Err, Err: var e } => e.Message,
         { Tag: Ok, Ok: var k } => k.ToString(),
         _ => throw new(),
      };

      await Assert.That(x).IsEqualTo("abc");
   }
   [Test]
   public async Task ConvertToSingleTypeResult_PreservesErr() {
      BoxedResult<int, Exception> r = 1;
      r.Err = new("abc");
      BoxedResult<int> r2 = r;
      await Assert.That(r2.Err.Message).IsEqualTo("abc");
   }
   [Test]
   public async Task SingleTypeResult_IntAssignment_ReturnsValue() {
      BoxedResult<int> r2 = 10;
      await Assert.That(r2.Ok).IsEqualTo(10);
   }
}
public class UnionSetValueTests {
   [Test]
   public async Task Switch_ReturnsValueFromCurrentVariant() {
      Union<int, Int128, double[]> u = 1;
      var result = u.Switch(
         static (int i) => $"{i}",
         static (Int128 i) => $"{i}",
         static () => "abc"
      );
      await Assert.That(result).IsEqualTo("1");
   }
   [Test]
   public async Task Switch_OrderIndependent_ReturnsCurrentVariant() {
      Union<int, Int128, double[]> u = 1;
      var result = u.Switch(
         static (Int128 i) => $"{i}",
         static (int i) => $"{i}",
         static (double[] d) => $"{d}",
         static () => "abc"
      );
      await Assert.That(result).IsEqualTo("1");
   }
   [Test]
   public async Task SetValue_Int128_TryGetValueSucceeds() {
      Union<int, Int128, double[]> u = 1;
      Int128 x = new(123, 123);
      u.SetValue(x);
      var ok = u.TryGetValue(out Int128 u1);
      await Assert.That(ok).IsTrue();
      await Assert.That(u1).IsEqualTo(x);
   }
   [Test]
   public async Task SetValue_Int_AfterInt128_TryGetInt128Fails() {
      Union<int, Int128, double[]> u = 1;
      u.SetValue(new Int128(123, 123));
      u.SetValue(1);
      var ok = u.TryGetValue(out Int128 _);
      await Assert.That(ok).IsFalse();
      await Assert.That(u.Value).IsEqualTo(1);
   }
}
