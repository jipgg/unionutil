[MemoryDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class TypeCheck {
   [Params(1234, 0.12345f)]
   public object? _value;
   public DenseCase<int, float, List<double>> _dense;
   public IUnionType _preboxed = default!;
   public SparseCase<int, float, List<double>> _sparse;

   [GlobalSetup]
   public void Setup() {
      if (_value is int i) {
         _dense = i;
         _sparse = i;
         _preboxed = _dense;
      } else if (_value is float f) {
         _dense = f;
         _sparse = f;
         _preboxed = _dense;
      } else {
         throw new();
      }
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
   public bool HoldsType_UnionType() {
      static bool generic<T>(ref T v) where T: IUnionType {
         return v.HoldsType<int>();
      }
      return generic(ref _dense);
   }
   [Benchmark]
   public bool HoldsType_UnionType_Preboxed() {
      return _preboxed.HoldsType<int>();
   }

}
public enum DenseCaseTag { T1, T2, T3 }
[GenerateUnion(EnableGenericHoldsTypeMethod | EnableUnionTypeInterface,
   FieldVisibility = Visibility.Internal),
   Tagged<DenseCaseTag>]
public partial struct DenseCase<_T1, _T2, _T3> : IUnionTypeArguments<_T1, _T2, _T3>;

public enum SparseCaseTag { T1 = 123, T2 = -23, T3 = 5 }
[GenerateUnion(EnableGenericHoldsTypeMethod | EnableUnionTypeInterface,
   FieldVisibility = Visibility.Internal),
   Tagged<SparseCaseTag>]
public partial struct SparseCase<_T1, _T2, _T3> : IUnionTypeArguments<_T1, _T2, _T3>;

