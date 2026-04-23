[UnionImpl(FieldVisibility = Visibility.Internal)]
public partial struct BasicUnion<T, U, V, W> : IUnion<T, U, V, W>;
[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class GetValue {
   public BasicUnion<_16B, double, int[], double[]> _union;

   [GlobalSetup]
   public void Setup() {
      _union = new _16B(1, 2);
   }

   [Benchmark]
   public _16B FieldAccess() {
      if (_union._index is not 1) return default;
      else return _union._1;
   }
   [Benchmark]
   public _16B ValueProperty() {
      if (_union.Value is not _16B v) return default;
      else return v;
   }
   [Benchmark]
   public _16B TryGetValue() {
      if (!_union.TryGetValue(out _16B v)) return default;
      else return v;
   }
   [Benchmark(Description = "IUnion.TryGetValue<T>")]
   public _16B TryGetValueT() {
      var u = (IUnion)_union;
      if (!u.TryGetValue(out _16B v)) return default;
      else return v;
   }
   [Benchmark(Description = "TUnion.TryGetValue<T>")]
   public _16B TryGetValueTGeneric() {
      static _16B Generic<T>(ref T u) where T : IUnion {
         if (!u.TryGetValue(out _16B v)) return default;
         else return v;
      }
      return Generic(ref _union);
   }
}
