namespace Test;

public abstract class SboContractTests<TSbo> where TSbo : IUnionType {
   protected abstract TSbo WithDouble(double value);
   protected abstract TSbo WithInt(int value);
   [Test]
   public async Task Holds_Double_AfterDoubleAssignment() {
      var sbo = WithDouble(0.5);
      await Assert.That(sbo.HoldsType<double>()).IsTrue();
   }
   [Test]
   public async Task Holds_Int_ReturnsFalse_WhenDouble() {
      var sbo = WithDouble(0.5);
      await Assert.That(sbo.HoldsType<int>()).IsFalse();
   }
   [Test]
   public async Task TryGetValue_ReturnsAssignedDouble() {
      var sbo = WithDouble(0.5);
      var ok = sbo.TryGetValue(out double d);
      await Assert.That(ok).IsTrue();
      await Assert.That(d).IsEqualTo(0.5);
   }
   [Test]
   public async Task TrySetValue_ChangesCurrentType() {
      var sbo = WithDouble(0.5);
      sbo.TrySetValue(42);
      var ok = sbo.TryGetValue(out int i);
      await Assert.That(ok).IsTrue();
      await Assert.That(i).IsEqualTo(42);
   }
   [Test]
   public async Task CanHold_Double_IsTrue() => await Assert.That(TSbo.CanHoldType<double>()).IsTrue();
   [Test]
   public async Task CanHold_Int_IsTrue() => await Assert.That(TSbo.CanHoldType<int>()).IsTrue();
   [Test]
   public async Task IUnionType_Value_ReturnsCurrentValue() {
      var sbo = WithDouble(0.5);
      await Assert.That(sbo.Value).IsEqualTo(0.5);
   }
}

[InheritsTests]
public class Sbo23Tests : SboContractTests<Sbo23<int, double, Exception>> {
   protected override Sbo23<int, double, Exception> WithDouble(double v) => v;
   protected override Sbo23<int, double, Exception> WithInt(int v) => v;
   [Test]
   public async Task StructSize_Is32() =>
      await Assert.That(Unsafe.SizeOf<Sbo23<int, double, Exception>>()).IsEqualTo(32);

   [Test]
   public async Task Double_StoredInline_NullBox() {
      Sbo23<int, double, Exception> sbo = 0.5;
      await Assert.That(sbo._box).IsNull();
      await Assert.That(Unsafe.As<byte, double>(ref sbo._sbo.Data)).IsEqualTo(0.5);
      await Assert.That((int)sbo._index).IsEqualTo(2);
   }
}

[InheritsTests]
public class Sbo55Tests : SboContractTests<Sbo55<int, double, Exception>> {
   protected override Sbo55<int, double, Exception> WithDouble(double v) => v;
   protected override Sbo55<int, double, Exception> WithInt(int v) => v;

   [Test]
   public async Task StructSize_Is64() =>
      await Assert.That(Unsafe.SizeOf<Sbo55<int, double, Exception>>()).IsEqualTo(64);

   [Test]
   public async Task Double_StoredInline_NullBox() {
      Sbo55<int, double, Exception> sbo = 0.5;
      await Assert.That(sbo._box).IsNull();
      await Assert.That(Unsafe.As<byte, double>(ref sbo._sbo.Data)).IsEqualTo(0.5);
      await Assert.That((int)sbo._index).IsEqualTo(2);
   }
}

[InheritsTests]
public class Sbo15Tests : SboContractTests<Sbo15<int, double, Exception>> {
   protected override Sbo15<int, double, Exception> WithDouble(double v) => v;
   protected override Sbo15<int, double, Exception> WithInt(int v) => v;

   [Test]
   public async Task StructSize_Is24() =>
      await Assert.That(Unsafe.SizeOf<Sbo15<int, double, Exception>>()).IsEqualTo(24);

   [Test]
   public async Task Double_StoredInline_NullBox() {
      Sbo15<int, double, Exception> sbo = 0.5;
      await Assert.That(sbo._box).IsNull();
      await Assert.That(Unsafe.As<byte, double>(ref sbo._sbo.Data)).IsEqualTo(0.5);
      await Assert.That((int)sbo._index).IsEqualTo(2);
   }
}

[InheritsTests]
public class Sbo7Tests : SboContractTests<Sbo7<int, double, Exception>> {
   protected override Sbo7<int, double, Exception> WithDouble(double v) => v;
   protected override Sbo7<int, double, Exception> WithInt(int v) => v;

   [Test]
   public async Task StructSize_Is16() =>
      await Assert.That(Unsafe.SizeOf<Sbo7<int, double, Exception>>()).IsEqualTo(16);

   [Test]
   public async Task Double_DoesNotFitBuffer_IsBoxed() {
      Sbo7<int, double, Exception> sbo = 0.5;
      await Assert.That(sbo._box).IsNotNull();
      await Assert.That((int)sbo._index).IsEqualTo(2);
   }

   [Test]
   public async Task Int_FitsBuffer_ClearsBox() {
      Sbo7<int, double, Exception> sbo = 0.5;
      sbo.SetValue(1);
      await Assert.That(Unsafe.As<byte, int>(ref sbo._sbo.Data)).IsEqualTo(1);
      await Assert.That((int)sbo._index).IsEqualTo(1);
   }

   [Test]
   public async Task IUnionType_Holds_Double() {
      Sbo7<int, double, Exception> sbo = 0.5;
      IUnionType sv = sbo;
      await Assert.That(sv.HoldsType<double>()).IsTrue();
      await Assert.That(sv.HoldsType<object>()).IsFalse();
   }
}
