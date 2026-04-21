using BenchmarkDotNet.Order;
using Opts = UnionUtil.UnionImplOptions;
public readonly record struct _1B(byte X);
public readonly record struct _4B(int X);
public readonly record struct _8B(long X);
public readonly record struct _16B(long A, long B);
public readonly record struct _32B(long A, long B, long C, long D);
public readonly record struct _64B(long A, long B, long C, long D, long E, long F, long G, long H);
public readonly record struct _16BManaged(string S, long X);        // managed struct
public sealed record Reference(int X);                                // reference type

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
[GenericTypeArguments(typeof(_1B))]
[GenericTypeArguments(typeof(_4B))]
[GenericTypeArguments(typeof(_8B))]
[GenericTypeArguments(typeof(_16B))]
[GenericTypeArguments(typeof(_32B))]
[GenericTypeArguments(typeof(_64B))]
[GenericTypeArguments(typeof(_16BManaged))]
[GenericTypeArguments(typeof(Reference))]
public class InitMatchAssignReturn<T> where T : notnull {
   T _value = default!;
   [GlobalSetup]
   public void Setup() {
      var r = Random.Shared;
      object v = typeof(T) switch {
         var t when t == typeof(_1B) => new _1B((byte)r.Next()),
         var t when t == typeof(_4B) => new _4B(r.Next()),
         var t when t == typeof(_8B) => new _8B(r.NextInt64()),
         var t when t == typeof(_16B) => new _16B(r.NextInt64(), r.NextInt64()),
         var t when t == typeof(_32B) => new _32B(r.NextInt64(), r.NextInt64(), r.NextInt64(), r.NextInt64()),
         var t when t == typeof(Something) => new Something(
            r.Next(),
            r.NextDouble(),
            r.NextInt64(),
            (ulong)r.NextInt64()
         ),
         var t when t == typeof(_64B) => new _64B(
            r.NextInt64(),
            r.NextInt64(),
            r.NextInt64(),
            r.NextInt64(),
            r.NextInt64(),
            r.NextInt64(),
            r.NextInt64(),
            r.NextInt64()
         ),
         var t when t == typeof(_16BManaged) => new _16BManaged("hello", r.Next()),
         var t when t == typeof(Reference) => new Reference(r.Next()),
         _ => throw new NotSupportedException()
      };
      _value = (T)v;
   }

   [Benchmark]
   public UnionBaseline<int, double, T> UnionBaseline() {
      UnionBaseline<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }

   [Benchmark]
   public Sequential<int, double, T> Sequential() {
      Sequential<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }

   [Benchmark]
   public SequentialTagged<int, double, T> SequentialTagged() {
      SequentialTagged<int, double, T> @union = _value;
      @union = @union switch {
          { Tag: null } => 0,
          { Tag: Tag.X1, X1: var i } => i,
          { Tag: Tag.X2, X2: var d } => (int)d,
          { Tag: Tag.X3, X3: var s } => s,
         _ => throw new(),
      };
      return @union;
   }

   [Benchmark]
   public Boxed<int, double, T> Boxed() {
      Boxed<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
   [Benchmark]
   public BoxedTagged<int, double, T> BoxedTagged() {
      BoxedTagged<int, double, T> @union = _value;
      @union = @union switch {
          { Tag: null } => 0,
          { Tag: Tag.X1, X1: var i } => i,
          { Tag: Tag.X2, X2: var d } => (int)d,
          { Tag: Tag.X3, X3: var s } => s,
         _ => throw new(),
      };
      return @union;
   }


   [Benchmark]
   public Sbo7<int, double, T> Sbo7() {
      Sbo7<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }

   [Benchmark]
   public Sbo15<int, double, T> Sbo15() {
      Sbo15<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }

   [Benchmark]
   public Sbo23<int, double, T> Sbo23() {
      Sbo23<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
   [Benchmark]
   public Sbo23Class<int, double, T> Sbo23Class() {
      Sbo23Class<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
   [Benchmark]
   public Sbo31<int, double, T> Sbo31() {
      Sbo31<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
   [Benchmark]
   public Sbo31Class<int, double, T> Sbo31Class() {
      Sbo31Class<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
   [Benchmark]
   public Sbo55Class<int, double, T> Sbo55Class() {
      Sbo55Class<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
   [Benchmark]
   public Sbo80Class<int, double, T> Sbo80Class() {
      Sbo80Class<int, double, T> @union = _value;
      if (!@union.HasValue) @union = 0;
      else if (@union.TryGetValue(out int i)) @union = i;
      else if (@union.TryGetValue(out double d)) @union = d;
      else if (@union.TryGetValue(out T s)) @union = s;
      else throw new();
      return @union;
   }
}
public enum Tag { X1, X2, X3 }
[Tagged<Tag>, UnionImpl(Opts.Nullable)]
public partial struct SequentialTagged<T1, T2, T3> : IUnion<T1, T2, T3>;
[Tagged<Tag>, UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics | Opts.BoxManagedStructs)]
public partial struct BoxedTagged<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable)]
public readonly partial struct ReadOnlyGeneratedSequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable)]
public partial struct Sequential<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics | Opts.BoxManagedStructs)]
public partial struct Boxed<T1, T2, T3> : IUnion<T1, T2, T3>;

[UnionImpl(Opts.Nullable)]
public partial struct GeneratedWhereClass<T1, T2, T3> : IUnion<T1, T2, T3>
where T1 : class where T2 : class where T3 : class;

[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(80)]
public sealed partial class Sbo80Class<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(55)]
public sealed partial class Sbo55Class<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(31)]
public sealed partial class Sbo31Class<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(31)]
public partial struct Sbo31<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(7)]
public partial struct Sbo7<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(15)]
public partial struct Sbo15<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(23)]
public partial struct Sbo23<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(Opts.Nullable | Opts.BoxOpenGenerics), SmallBufferOptimized(23)]
public sealed partial class Sbo23Class<T1, T2, T3> : IUnion<T1, T2, T3>;

public readonly record struct Something(int Int, double Double, long Long, ulong Ulong);
[UnionImpl(Opts.Nullable), Union<int, double, Something>]
public partial struct GeneratedStatically;

public readonly struct UnionBaseline<T1, T2, T3> {
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
   public UnionBaseline(T1 v) {
      _value = v;
      _tag = 1;
   }
   public UnionBaseline(T2 v) {
      _value = v;
      _tag = 2;
   }
   public UnionBaseline(T3 v) {
      _value = v;
      _tag = 3;
   }
   public static implicit operator UnionBaseline<T1, T2, T3>(T1 v) => new(v);
   public static implicit operator UnionBaseline<T1, T2, T3>(T2 v) => new(v);
   public static implicit operator UnionBaseline<T1, T2, T3>(T3 v) => new(v);
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
      if (_tag is 3) {
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

