[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
public class InitAndSwitch {
   [Benchmark]
   public int SpeculativeUnionImplementation() {
      SpeculativeUnionImplementation<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark]
   public int ReadOnlyGeneratedSequential() {
      ReadOnlyGeneratedSequential<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark]
   public int GeneratedSequential() {
      GeneratedSequential<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark]
   public int GeneratedBoxed() {
      GeneratedBoxed<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark]
   public int GeneratedRawBoxed() {
      GeneratedWhereClass<Box<int>, Box<double>, Box<Something>> @union = new Box<Something>(new(1, 2));

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out Box<int> i)) return i.Item;
      else if (@union.TryGetValue(out Box<double> d)) return (int)d.Item;
      else if (@union.TryGetValue(out Box<Something> s)) return (int)(s.Item.Int + s.Item.Double);
      else throw new();
   }
   [Benchmark]
   public int GeneratedStatically() {
      GeneratedStatically @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }

}

public readonly struct SpeculativeUnionImplementation<T1, T2, T3> {
   readonly object? _value;
   readonly int _tag;
   public object? Value {
      [MethodImpl(AggressiveInlining)]
      get => _tag switch {
         1 => (T1)_value!,
         2 => (T2)_value!,
         3 => (T3)_value!,
         _ => null,
      };
   }
   public SpeculativeUnionImplementation(T1 v) {
      _value = v;
      _tag = 1;
   }
   public SpeculativeUnionImplementation(T2 v) {
      _value = v;
      _tag = 2;
   }
   public SpeculativeUnionImplementation(T3 v) {
      _value = v;
      _tag = 3;
   }
   public static implicit operator SpeculativeUnionImplementation<T1, T2, T3>(T1 v) => new(v);
   public static implicit operator SpeculativeUnionImplementation<T1, T2, T3>(T2 v) => new(v);
   public static implicit operator SpeculativeUnionImplementation<T1, T2, T3>(T3 v) => new(v);
   public bool HasValue {
      [MethodImpl(AggressiveInlining)]
      get => _tag is 0;
   }
   public bool TryGetValue(out T1 v) {
      if (_tag is 1) {
         v = (T1)_value!;
         return true;
      }
      v = default!;
      return false;
   }
   public bool TryGetValue(out T2 v) {
      if (_tag is 2) {
         v = (T2)_value!;
         return true;
      }
      v = default!;
      return false;
   }
   public bool TryGetValue(out T3 v) {
      if (_tag is 1) {
         v = (T3)_value!;
         return true;
      }
      v = default!;
      return false;
   }
}
[UnionImpl(Nullable = true)]
public readonly partial struct ReadOnlyGeneratedSequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Nullable = true)]
public partial struct GeneratedSequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Nullable = true, BoxOpenGenerics = true, BoxManagedStructs = true)]
public partial struct GeneratedBoxed<T1, T2, T3> : IUnion<T1, T2, T3>;

[UnionImpl(Nullable = true)]
public partial struct GeneratedWhereClass<T1, T2, T3> : IUnion<T1, T2, T3>
where T1 : class where T2 : class where T3 : class;

[UnionImpl(Nullable = true), Union<int, double, Something>]
public partial struct GeneratedStatically;

public sealed class Box<T>(T item) where T : struct {
   public T Item = item;
}
public readonly record struct Something(int Int, double Double);

