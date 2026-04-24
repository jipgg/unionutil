[UnionImpl(UnionImplOptions.ImplementCommonInterface, FieldVisibility = Visibility.Internal)]
public partial struct BasicUnion<T, U, V, W>;
[UnionImpl(UnionImplOptions.ImplementCommonInterface, FieldVisibility = Visibility.Internal)]
public readonly partial struct ReadOnlyBasicUnion<T, U, V, W>;
[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
[BenchmarkCategory("GetValue")]
[GenericTypeArguments(typeof(int))]
[GenericTypeArguments(typeof(Int128))]
[GenericTypeArguments(typeof(List<Int128>))]
public class GetValue<T> {
   public BasicUnion<T, double, int[], double[]> _union;
   public ReadOnlyBasicUnion<T, double, int[], double[]> _readOnly;
   public IUnion _preboxed = default!;

   [GlobalSetup]
   public void Setup() {
      object v = typeof(T) switch {
         var t when t == typeof(int) => 123,
         var t when t == typeof(Int128) => new Int128(1, 2),
         var t when t == typeof(List<Int128>) => new List<Int128>(Enumerable.Range(0, 10).Select(e => new Int128((ulong)e, (ulong)e))),
         _ => throw new(),
      };
      _union = (T)v;
      _preboxed = _union;
      _readOnly = (T)v;
   }

   [Benchmark]
   public T? Field() {
      if (_union._index is not 1) return default;
      else return _union._1;
   }
   [Benchmark]
   public T? Field_ReadOnly() {
      if (_readOnly._index is not 1) return default;
      else return _union._1;
   }
   [Benchmark]
   public T? Value() {
      if (_union.Value is not T v) return default;
      else return v;
   }
   [Benchmark]
   public T? Value_ReadOnly() {
      if (_readOnly.Value is not T v) return default;
      else return v;
   }
   [Benchmark]
   public T? TryGetValue() {
      if (!_union.TryGetValue(out T v)) return default;
      else return v;
   }
   [Benchmark]
   public T? TryGetValue_ReadOnly() {
      if (!_readOnly.TryGetValue(out T v)) return default;
      else return v;
   }
   [Benchmark]
   public T? TryGetValue_Adapter_Preboxed() {
      if (!_preboxed.TryGetValue(out T v)) return default;
      else return v;
   }
   [Benchmark]
   public T? Value_Adapter_Preboxed() {
      if (_preboxed.Value is not T v) return default;
      else return v;
   }
   [Benchmark]
   public T? TryGetValue_Adapter() {
      static T? Generic<U>(ref U u) where U : IUnion {
         if (!u.TryGetValue(out T v)) return default;
         else return v;
      }
      return Generic(ref _union);
   }
   [Benchmark]
   public T? Value_Adapter() {
      static T? Generic<U>(ref U u) where U : IUnion {
         if (u.Value is not T v) return default;
         else return v;
      }
      return Generic(ref _union);
   }
}
