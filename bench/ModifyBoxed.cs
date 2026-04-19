public struct MutableUnion<T1, T2, T3> {
   object? _value;
   int _tag;
   public readonly object? Value {
      [MethodImpl(AggressiveInlining)]
      get => _tag switch {
         1 => (T1)_value!,
         2 => (T2)_value!,
         3 => (T3)_value!,
         _ => null,
      };
   }
   public T1 V1 {
      [MethodImpl(AggressiveInlining)]
      readonly get {
         if (_tag is not 1) throw new();
         return (T1)_value!;
      }
      set {
         _tag = 1;
         _value = value;
      }
   }
   public T2 V2 {
      [MethodImpl(AggressiveInlining)]
      readonly get {
         if (_tag is not 2) throw new();
         return (T2)_value!;
      }
      set {
         _tag = 2;
         _value = value;
      }
   }
   public T3 V3 {
      [MethodImpl(AggressiveInlining)]
      readonly get {
         if (_tag is not 3) throw new();
         return (T3)_value!;
      }
      set {
         _tag = 3;
         _value = value;
      }
   }
   public MutableUnion(T1 v) {
      _value = v;
      _tag = 1;
   }
   public MutableUnion(T2 v) {
      _value = v;
      _tag = 2;
   }
   public MutableUnion(T3 v) {
      _value = v;
      _tag = 3;
   }
   public static implicit operator MutableUnion<T1, T2, T3>(T1 v) => new(v);
   public static implicit operator MutableUnion<T1, T2, T3>(T2 v) => new(v);
   public static implicit operator MutableUnion<T1, T2, T3>(T3 v) => new(v);
   public readonly bool HasValue {
      [MethodImpl(AggressiveInlining)]
      get => _tag is 0;
   }
   public void SetValue(T1 v) {
      _value = v;
      _tag = 1;
   }
   public void SetValue(T2 v) {
      _value = v;
      _tag = 2;
   }
   public void SetValue(T3 v) {
      _value = v;
      _tag = 3;
   }
   public readonly bool TryGetValue(out T1 v) {
      if (_tag is 1) {
         v = (T1)_value!;
         return true;
      }
      v = default!;
      return false;
   }
   public readonly bool TryGetValue(out T2 v) {
      if (_tag is 2) {
         v = (T2)_value!;
         return true;
      }
      v = default!;
      return false;
   }
   public readonly bool TryGetValue(out T3 v) {
      if (_tag is 1) {
         v = (T3)_value!;
         return true;
      }
      v = default!;
      return false;
   }
}

struct ManagedStruct {
   public object Object;
}
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
public class ModifyBoxed {
   [Params(8, 32, 128)]
   public int N;
   [Benchmark(Description = "union(int)")]
   public void Union() {
      MutableUnion<int, float, double> boxed = 1;
      for (int i = 0; i < N; ++i) {
         boxed.TryGetValue(out int x);
         boxed.SetValue(x + i);
      }
   }
   [Benchmark(Description = "generated(int)")]
   public void Generated() {
      Boxed<int, float, double> boxed = 1;
      for (int i = 0; i < N; ++i) {
         boxed.TryGetValue(out int x);
         boxed.SetValue(x + i);
      }
   }
   [Benchmark(Description = "union(struct(object))")]
   public void UnionManagedStruct() {
      MutableUnion<ManagedStruct, float, double> boxed = new ManagedStruct { Object = null! };
      for (int i = 0; i < N; ++i) {
         boxed.TryGetValue(out ManagedStruct x);
         x.Object = null!;
         boxed.SetValue(x);
      }
   }
   [Benchmark(Description = "generated(struct(object))")]
   public void GeneratedManaged() {
      Boxed<ManagedStruct, float, double> boxed = new ManagedStruct { Object = null! };
      for (int i = 0; i < N; ++i) {
         boxed.TryGetValue(out ManagedStruct x);
         x.Object = null!;
         boxed.SetValue(x);
      }
   }

   [Benchmark(Description = "union(List<int>)")]
   public void UnionClass() {
      MutableUnion<List<int>, float, double> boxed = new List<int>();
      for (int i = 0; i < N; ++i) {
         boxed.TryGetValue(out List<int> x);
         boxed.SetValue(x);
      }
   }
   [Benchmark(Description = "generated(List<int>)")]
   public void GeneratedClass() {
      Boxed<List<int>, float, double> boxed = new List<int>();
      for (int i = 0; i < N; ++i) {
         boxed.TryGetValue(out List<int> x);
         boxed.SetValue(x);
      }
   }
}
