
public enum DenseCaseTag { T1, T2, T3 }
[UnionImpl(
   UnionImplOptions.IncludeHoldsTypeMethod,
   FieldVisibility = Visibility.Internal),
   Tagged<DenseCaseTag>]
public partial struct DenseCase<_T1, _T2, _T3> : IUnion<_T1, _T2, _T3>;

public enum SparseCaseTag { T1 = 123, T2 = -23, T3 = 5 }
[UnionImpl(
   UnionImplOptions.IncludeHoldsTypeMethod,
   FieldVisibility = Visibility.Internal),
   Tagged<SparseCaseTag>]
public partial struct SparseCase<_T1, _T2, _T3> : IUnion<_T1, _T2, _T3>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class TypeCheck {
   public DenseCase<int, float, object> _dense;
   public SparseCase<int, float, object> _sparse;

   [GlobalSetup]
   public void Setup() {
      _dense = 1;
      _sparse = 1;
   }

   [Benchmark]
   public bool Field() {
      return _dense._index is 1;
   }
   [Benchmark]
   public bool TryGetValue() {
      return _dense.TryGetValue(out int _);
   }

   [Benchmark]
   public bool HoldsType() {
      return _dense.HoldsType<int>();
   }

   [Benchmark]
   public bool Tag_Dense() {
      return _dense.Tag is DenseCaseTag.T1;
   }
   [Benchmark]
   public bool Tag_Sparse() {
      return _sparse.Tag is SparseCaseTag.T1;
   }

   [Benchmark]
   public bool HoldsType_OpenGeneric() {
      return CheckType<int, float, object, int>(ref _dense);
   }

   [Benchmark]
   public bool HoldsType_OpenGeneric_Mismatch() {
      return CheckType<int, float, object, float>(ref _dense);
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   static bool CheckType<T1, T2, T3, TQuery>(ref DenseCase<T1, T2, T3> union) {
      return union.HoldsType<TQuery>();
   }

}
