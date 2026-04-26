[UnionImpl(ImplementUnionInterfaces | BoxOpenGenerics | BoxManagedStructs,
      FieldVisibility = Visibility.Internal)]
[SmallBufferOptimized]
partial struct BasicUnion<T1, T2, T3, T4, T5, T6, T7, T8>;
[MemoryDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class SetValue {
#nullable disable
   double[] _t8;
   byte _t1;
   int _t5;
   Int128 _sameType;
   BasicUnion<byte, double, int[], Int128, int, float, nuint, double[]> _union;
   IUnionType _preboxed;
#nullable restore

   [GlobalSetup]
   public void Setup() {
      _union = new Int128(123, 123);
      _preboxed = _union;
      _t8 = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
      _t1 = 0b10101010;
      _t5 = 12345;
      _sameType = new Int128(1111, 111111);
   }

   [Benchmark]
   public void SetValue_T1() {
      _union.SetValue(_t1);
   }
   [Benchmark]
   public void TrySetValue_UnionType_T1_Preboxed() {
      _preboxed.TrySetValue(_t1);
   }
   [Benchmark]
   public void TrySetValue_UnionType_T1() {
      Generic(ref _union, _t1);
   }
   [Benchmark]
   public void SetValue_T8() {
      _union.SetValue(_t8);
   }
   [Benchmark]
   public void TrySetValue_UnionType_T8_Preboxed() {
      _preboxed.TrySetValue(_t8);
   }
   [Benchmark]
   public void TrySetValue_UnionType_T8() {
      Generic(ref _union, _t8);
   }
   [Benchmark]
   public void SetValue_SameType() {
      _union.SetValue(_sameType);
   }
   [Benchmark]
   public void TrySetValue_UnionType_SameType_Preboxed() {
      _preboxed.TrySetValue(_sameType);
   }
   [Benchmark]
   public void TrySetValue_UnionType_SameType() {
      Generic(ref _union, _sameType);
   }
   [Benchmark]
   public void SetValue_T5() {
      _union.SetValue(_t5);
   }
   [Benchmark]
   public void TrySetValue_UnionType_T5_Preboxed() {
      _preboxed.TrySetValue(_t5);
   }
   [Benchmark]
   public void TrySetValue_UnionType_T5() {
      Generic(ref _union, _t5);
   }
   static void Generic<U, T>(ref U u, T value) where U : IUnionType {
      u.TrySetValue(value);
   }
}
