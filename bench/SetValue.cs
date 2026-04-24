using static UnionUtil.UnionImplOptions;
[UnionImpl(ImplementAdapter | BoxOpenGenerics | BoxManagedStructs, FieldVisibility = Visibility.Internal)]
[SmallBufferOptimized]
partial struct BasicUnion<T1, T2, T3, T4, T5, T6, T7, T8>;
[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class SetValue {
#nullable disable
   double[] _t8;
   byte _t1;
   Int128 _t4;
   BasicUnion<byte, double, int[], Int128, int, float, nuint, double[]> _union;
   IUnionAdapter _preboxed;
#nullable restore

   [GlobalSetup]
   public void Setup() {
      _union = 123;
      _preboxed = _union;
      _t8 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
      _t1 = 0b10101010;
      _t4 = new Int128(123, 123);
   }

   // [Benchmark]
   // public void SetValue_T1() {
   //    _union.SetValue(_t1);
   // }
   // [Benchmark]
   // public void TrySetValue_T1_Preboxed() {
   //    _preboxed.TrySetValue(_t1);
   // }
   // [Benchmark]
   // public void TrySetValue_T1() {
   //    Generic(ref _union, _t1);
   // }
   // [Benchmark]
   // public void SetValue_T8() {
   //    _union.SetValue(_t8);
   // }
   // [Benchmark]
   // public void TrySetValue_T8_Preboxed() {
   //    _preboxed.TrySetValue(_t8);
   // }
   // [Benchmark]
   // public void TrySetValue_T8() {
   //    Generic(ref _union, _t8);
   // }
   [Benchmark]
   public void SetValue_T4() {
      _union.SetValue(_t4);
   }
   [Benchmark]
   public void TrySetValue_T4_Preboxed() {
      _preboxed.TrySetValue(_t4);
   }
   [Benchmark]
   public void TrySetValue_T4() {
      Generic(ref _union, _t4);
   }
   // [Benchmark]
   // public void SetValue_SameType() {
   //    _union.SetValue(int.MaxValue);
   // }
   // [Benchmark]
   // public void TrySetValue_SameType_Preboxed() {
   //    _preboxed.TrySetValue(int.MaxValue);
   // }
   // [Benchmark]
   // public void TrySetValue_SameType() {
   //    Generic(ref _union, int.MaxValue);
   // }
   static void Generic<U, T>(ref U u, T value) where U : IUnionAdapter {
      u.TrySetValue(value);
   }
}
