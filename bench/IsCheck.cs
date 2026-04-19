
public enum DenseCaseTag { T1, T2, T3 }
[UnionImpl(FieldVisibility = Visibility.Internal), Tagged<DenseCaseTag>]
public partial struct DenseCase<_T1, _T2, _T3> : IUnion<_T1, _T2, _T3>;

public enum SparseCaseTag { T1 = 123, T2 = -23, T3 = 5 }
[UnionImpl(FieldVisibility = Visibility.Internal), Tagged<SparseCaseTag>]
public partial struct SparseCase<_T1, _T2, _T3> : IUnion<_T1, _T2, _T3>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class IsCheck {
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
   public bool IsT() {
      return _dense.Is<int>();
   }

   [Benchmark]
   public bool IsIndex() {
      return _dense.Is(1);
   }

   [Benchmark]
   public bool IsTag_Dense() {
      return _dense.Tag is DenseCaseTag.T1;
   }
   [Benchmark]
   public bool IsTag_Sparse() {
      return _sparse.Tag is SparseCaseTag.T1;
   }

   [Benchmark]
   public bool IsT_OpenGeneric() {
      return CheckIs<int, float, object, int>(ref _dense);
   }

   [Benchmark]
   public bool IsT_OpenGeneric_Mismatch() {
      return CheckIs<int, float, object, float>(ref _dense);
   }

   [Benchmark]
   public bool IsIndex_OpenGeneric() {
      return CheckIndex<int, float, object>(ref _dense, 1);
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   static bool CheckIs<T1, T2, T3, TQuery>(ref DenseCase<T1, T2, T3> union) {
      return union.Is<TQuery>();
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   static bool CheckIndex<T1, T2, T3>(ref DenseCase<T1, T2, T3> union, byte index) {
      return union.Is(index);
   }
}
