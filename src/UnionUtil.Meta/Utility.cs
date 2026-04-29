using System.Buffers;
using System.Diagnostics;
namespace UnionUtil.Meta;

readonly record struct ResolvedSource(string HintName, StringBuilder Source);

ref struct SpanList<T> : IDisposable {
   public SpanList(Span<T> buf) {
      _owner = null;
      _buf = buf;
   }
   public SpanList(IMemoryOwner<T> owner) {
      _owner = owner;
      _buf = owner.Memory.Span;
   }
   public SpanList(int capacity) : this(MemoryPool<T>.Shared.Rent(capacity)) { }
   readonly IMemoryOwner<T>? _owner;
   public readonly void Dispose() => _owner?.Dispose();
   readonly Span<T> _buf;
   int _count;

   public readonly bool IsEmpty => _count is 0;
   public readonly int Capacity => _buf.Length;
   public readonly int Count => _count;
   public readonly Span<T> Span => _count is 0 ? [] : _buf.Slice(0, _count);
   public readonly bool Contains(T v) {
      foreach (var e in Span) {
         if (EqualityComparer<T>.Default.Equals(e, v)) return true;
      }
      return false;
   }

   public bool TryAdd(T v) {
      if (_count >= Capacity) return false;
      _buf[_count++] = v;
      return true;
   }
   public readonly ref T this[int i] {
      get {
         if (i < 0 || i >= _count) throw new IndexOutOfRangeException();
         return ref _buf[i];
      }
   }
   public void Add(T v) => _buf[_count++] = v;
   public readonly bool Any(Func<T, bool> pred) {
      foreach (var e in Span) {
         if (pred(e)) return true;
      }
      return false;
   }
   public readonly Span<T>.Enumerator GetEnumerator() => _buf.Slice(0, _count).GetEnumerator();
}

static class SpanExtensions {
   extension<T>(Span<T> span) {
      public void Sort<TComparer>(TComparer comparer) where TComparer : IComparer<T> {
         for (int i = 1; i < span.Length; i++) {
            var key = span[i];
            int j = i - 1;
            while (j >= 0 && comparer.Compare(span[j], key) > 0) {
               span[j + 1] = span[j];
               j--;
            }
            span[j + 1] = key;
         }
      }
   }
}
ref struct SpanDictionary<K, V> : IDisposable {
   public SpanDictionary(Span<(K, V)> buf) => _buf = buf;
   public SpanDictionary(IMemoryOwner<(K, V)> owner) {
      _owner = owner;
      _buf = _owner.Memory.Span;
   }
   public SpanDictionary(int capacity) : this(MemoryPool<(K, V)>.Shared.Rent(capacity)) { }

   public readonly struct Comparer : IComparer<(K, V)> {
      public int Compare((K, V) a, (K, V) b)
         => Comparer<K>.Default.Compare(a.Item1, b.Item1);
   }
   readonly IMemoryOwner<(K, V)>? _owner;
   public readonly void Dispose() => _owner?.Dispose();
   readonly Span<(K, V)> _buf;
   int _count = 0;
   public readonly int Count => _count;
   public readonly int IndexOf(K key) {
      return _buf.Slice(0, _count).BinarySearch(new(key, default!), default(Comparer));
   }
   public readonly bool ContainsKey(K key) {
      return IndexOf(key) >= 0;
   }
   public readonly void Sort() {
      _buf.Slice(0, _count).Sort(default(Comparer));
   }
   public bool TryAdd(K key, V value) {
      if (IndexOf(key) >= 0) return false;
      _buf[_count++] = new(key, value);
      Sort();
      return true;
   }
   public void Add(K key, V value) {
      if (!TryAdd(key, value)) throw new();
   }
   public V this[K key] {
      readonly get {
         var i = IndexOf(key);
         if (i < 0) throw new KeyNotFoundException();
         return _buf[i].Item2;
      }
      set {
         var i = IndexOf(key);
         if (i < 0) {
            _buf[_count++] = new(key, value);
            Sort();
         } else {
            _buf[i] = new(key, value);
         }
      }
   }
   public readonly (K Key, V Value) First() => _buf[0];
   public ref struct KeysEnumerator(Span<(K, V)> span) {
      readonly Span<(K, V)> _span = span;
      int _i = -1;
      public readonly K Current => _span[_i].Item1;
      public bool MoveNext() => ++_i < _span.Length;
      public void Reset() => _i = -1;
      public readonly K First() => _span[0].Item1;
   }
   public readonly KeysEnumerator Keys => new(_buf.Slice(0, _count));
   public readonly Span<(K, V)>.Enumerator GetEnumerator() => _buf.Slice(0, _count).GetEnumerator();
}

static class Helpers {
   public static bool IsUnionUtil(ITypeSymbol? symbol) {
      if (symbol is null) return false;
      if (symbol.ContainingNamespace.IsGlobalNamespace) return false;
      return symbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace
         && symbol.ContainingNamespace?.MetadataName == "UnionUtil";
   }
   public static bool IsUnionUtil(AttributeData? attributeData) => IsUnionUtil(attributeData?.AttributeClass);

}
static class TypeSymbolExtensions {
   const string CanHoldTypes = "CanHoldTypes";
   extension(ITypeSymbol symbol) {
      public (ImmutableArray<ITypeSymbol>, bool ok) ResolveUnionTypeArgs() {
         var attr = symbol.GetAttributes()
            .SingleOrDefault(static e => Helpers.IsUnionUtil(e.AttributeClass)
                  && e.AttributeClass?.Name is $"{CanHoldTypes}Attribute");
         if (attr?.AttributeClass is INamedTypeSymbol a) {
            return (a.TypeArguments, true);
         }
         var inter = symbol.Interfaces
            .SingleOrDefault(static e => Helpers.IsUnionUtil(e)
                  && e.Arity is not 0
                  && e.Name is $"I{CanHoldTypes}");
         if (inter is not null) {
            return (inter.TypeArguments, true);
         }
         if (symbol is INamedTypeSymbol nts) {
            return (nts.TypeArguments, true);
         }
         return ([], false);
      }
      public bool IsDerivedFrom(ITypeSymbol type) {
         if (type is not { IsReferenceType: true }) return false;
         ITypeSymbol? b = symbol;
         while ((b = b.BaseType) != null) {
            if (!SymbolEqualityComparer.Default.Equals(b, type)) {
               continue;
            }
            return true;
         }
         return false;
      }
   }
}
