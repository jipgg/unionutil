using static UnionUtil.UnionImplOptions;
[UnionImpl(ImplementAdapter | BoxOpenGenerics, FieldVisibility = Visibility.Internal)]
[SmallBufferOptimized]
partial struct BasicUnion<T, U, V, W, X, Y, Z>;
[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class SetValue {
#nullable disable
   double[] _lastType;
   byte _firstType;
   BasicUnion<byte, double, int[], Int128, int, float, double[]> _union;
#nullable restore

   [GlobalSetup]
   public void Setup() {
      _union = 123;
      _lastType = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
      _firstType = 0b10101010;
   }

   [Benchmark(Description = "SetValue,First")]
   public void SetValueMethod() {
      _union.SetValue(_firstType);
   }
   [Benchmark(Description = $"IUnionVisitor.TrySetValue<T>,First")]
   public void TrySetValueT() {
      var u = (IUnionAdapter)_union;
      u.TrySetValue(_firstType);
   }
   [MethodImpl(AggressiveInlining)]
   static void Generic<U, T>(ref U u, T value) where U : IUnionAdapter {
      u.TrySetValue(value);
   }
   [Benchmark(Description = "TUnionVisitor.TrySetValue<T>,First")]
   public void TrySetValueTGeneric() {
      Generic(ref _union, _firstType);
   }
   [Benchmark(Description = "SetValue,Last")]
   public void SetValueMethodWorst() {
      _union.SetValue(_lastType);
   }
   [Benchmark(Description = $"IUnionVisitor.TrySetValue<T>,Last")]
   public void TrySetValueTWorst() {
      var u = (IUnionAdapter)_union;
      u.TrySetValue(_lastType);
   }
   [Benchmark(Description = "TUnionVisitor.TrySetValue<T>,Last")]
   public void TrySetValueTGenericWorst() {
      Generic(ref _union, _lastType);
   }
}
