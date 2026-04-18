[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
public class InitAndSwitch {
   [Benchmark(Description = "speculative union")]
   public int SpeculativeUnionImplementation() {
      SpeculativeUnionImplementation<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark(Description = "generic,sequential")]
   public int GeneratedSequential() {
      GeneratedSequential<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark(Description = "generic,sequential,tagged")]
   public int GeneratedSequentialTagged() {
      GenericTaggedSequential<int, double, Something> @union = new Something(1, 2);

      return @union switch {
         { Tag: null } => 0,
         { Tag: Tag.X1, X1: var i } => i,
         { Tag: Tag.X2, X2: var d } => (int)d,
         { Tag: Tag.X3, X3: var s } => (int)(s.Int + s.Double),
         _ => throw new(),
      };
   }
   [Benchmark(Description = "generic,boxed")]
   public int GeneratedBoxed() {
      GeneratedBoxed<int, double, Something> @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
   [Benchmark(Description = "monomorphized")]
   public int GeneratedStatically() {
      GeneratedStatically @union = new Something(1, 2);

      if (!@union.HasValue) return 0;
      else if (@union.TryGetValue(out int i)) return i;
      else if (@union.TryGetValue(out double d)) return (int)d;
      else if (@union.TryGetValue(out Something s)) return (int)(s.Int + s.Double);
      else throw new();
   }
}
public enum Tag { X1, X2, X3 }
[Tagged<Tag>, UnionImpl(Nullable = true)]
public partial struct GenericTaggedSequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Nullable = true)]
public readonly partial struct ReadOnlyGeneratedSequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Nullable = true)]
public partial struct GeneratedSequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Nullable = true, BoxOpenGenerics = true, BoxManagedStructs = true)]
public partial struct GeneratedBoxed<T1, T2, T3> : IUnion<T1, T2, T3>;

[UnionImpl(Nullable = true)]
public partial struct GeneratedWhereClass<T1, T2, T3> : IUnion<T1, T2, T3>
where T1 : class where T2 : class where T3 : class;

[UnionImpl(Nullable = true), SmallBuffer(16)]
public partial struct GeneratedSbo16<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Nullable = true), SmallBuffer(8)]
public partial struct GeneratedSbo8<T1, T2, T3> : IUnion<T1, T2, T3>;

[UnionImpl(Nullable = true), Union<int, double, Something>]
public partial struct GeneratedStatically;

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
   public bool TryGetValue(out T1 v) { if (_tag is 1) { v = (T1)_value!;
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

public sealed class Box<T>(T item) where T : struct {
   public T Item = item;
}
public readonly record struct Something(int Int, double Double);

