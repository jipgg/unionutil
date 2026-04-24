using System.Diagnostics;
namespace UnionUtil.Meta;

ref struct SpanBuffer<T>(Span<T> buf) {
   Span<T> _buf = buf;
   int _count;

   public readonly int Capacity => _buf.Length;
   public readonly int Count => _count;
   public readonly Span<T> Span => _buf.Slice(0, _count);

   public bool TryAdd(T v) {
      if (_count >= Capacity) return false;
      _buf[_count++] = v;
      return true;
   }
   public void Add(T v) => _buf[_count++] = v;
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
ref struct SpanDictionary<K, V>(Span<(K, V)> buf) {
   public SpanDictionary(Memory<(K, V)> mem) : this(mem.Span) { }
   public readonly struct Comparer : IComparer<(K, V)> {
      public int Compare((K, V) a, (K, V) b)
         => Comparer<K>.Default.Compare(a.Item1, b.Item1);
   }
   Span<(K, V)> _buf = buf;
   int _count = 0;
   public readonly int Count => _count;
   public readonly int IndexOf(K key) {
      return _buf.Slice(0, _count).BinarySearch(new(key, default!), default(Comparer));
   }
   public readonly bool ContainsKey(K key) {
      return IndexOf(key) >= 0;
   }
   public void Sort() {
      _buf.Slice(0, _count).Sort(default(Comparer));
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

static class TypeSymbolExtensions {
   static bool IsInUnionUtil(ITypeSymbol? symbol) {
      if (symbol is null) return false;
      if (symbol.ContainingNamespace.IsGlobalNamespace) return false;
      return symbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace
         && symbol.ContainingNamespace?.MetadataName == "UnionUtil";
   }
   extension(ITypeSymbol symbol) {
      public (ImmutableArray<ITypeSymbol>, bool ok) ResolveUnionTypeArgs() {
         var attr = symbol.GetAttributes()
            .SingleOrDefault(static e => IsInUnionUtil(e.AttributeClass)
                  && e.AttributeClass?.Name is Config.UnionType.AttributeName);
         if (attr?.AttributeClass is INamedTypeSymbol a) {
            return (a.TypeArguments, true);
         }
         var inter = symbol.Interfaces
            .SingleOrDefault(static e => IsInUnionUtil(e)
                  && e.Arity is not 0
                  && e.Name is Config.UnionType.InterfaceName);
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
