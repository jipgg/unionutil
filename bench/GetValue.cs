[UnionImpl(UnionImplOptions.ImplementAdapter, FieldVisibility = Visibility.Internal)]
public partial struct BasicUnion<T, U, V, W> : IUnion<T, U, V, W>;
[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
[GenericTypeArguments(typeof(int))]
[GenericTypeArguments(typeof(Int128))] [GenericTypeArguments(typeof(List<Int128>))]
public class GetValue<T> {
   public BasicUnion<T, double, int[], double[]> _union;

   [GlobalSetup]
   public void Setup() {
      object v = typeof(T) switch {
         var t when t == typeof(int) => 123,
         var t when t == typeof(Int128) => new Int128(1, 2),
         var t when t == typeof(List<Int128>) => new List<Int128>(Enumerable.Range(0, 10).Select(e => new Int128((ulong)e, (ulong)e))),
         _ => throw new(),
      };
      _union = (T)v;
   }

   [Benchmark]
   public T? FieldAccess() {
      if (_union._index is not 1) return default;
      else return _union._1;
   }
   [Benchmark]
   public T? ValueProperty() {
      if (_union.Value is not T v) return default;
      else return v;
   }
   [Benchmark]
   public T? TryGetValue() {
      if (!_union.TryGetValue(out T v)) return default;
      else return v;
   }
   [Benchmark(Description = "IUnionVisitor.TryGetValue<T>")]
   public T? TryGetValueT() {
      var u = (IUnionAdapter)_union;
      if (!u.TryGetValue(out T v)) return default;
      else return v;
   }
   [Benchmark(Description = "TUnionVisitor.TryGetValue<T>")]
   public T? TryGetValueTGeneric() {
      [MethodImpl(AggressiveInlining)]
      static T? Generic<U>(ref U u) where U : IUnionAdapter {
         if (!u.TryGetValue(out T v)) return default;
         else return v;
      }
      return Generic(ref _union);
   }
}
