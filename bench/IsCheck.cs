
public enum IsCaseTag { T1, T2, T3 }
[UnionImpl(FieldVisibility = Visibility.Internal), Tagged<IsCaseTag>]
public partial struct IsCase<_T1, _T2, _T3> : IUnion<_T1, _T2, _T3>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class IsCheck {
   public IsCase<int, float, object> _value;

   [GlobalSetup]
   public void Setup() {
      _value = 1;
   }

   [Benchmark]
   public bool Field() {
      return _value._index is 1;
   }

   [Benchmark]
   public bool IsT() {
      return _value.Is<int>();
   }

   [Benchmark]
   public bool IsIndex() {
      return _value.Is(1);
   }

   [Benchmark]
   public bool IsTag() {
      return _value.Tag is IsCaseTag.T1;
   }

   [Benchmark]
   public bool IsT_OpenGeneric() {
      return CheckIs<int, float, object, int>(ref _value);
   }

   [Benchmark]
   public bool IsT_OpenGeneric_Mismatch() {
      return CheckIs<int, float, object, float>(ref _value);
   }

   [Benchmark]
   public bool IsIndex_OpenGeneric() {
      return CheckIndex<int, float, object>(ref _value, 1);
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   static bool CheckIs<T1, T2, T3, TQuery>(ref IsCase<T1, T2, T3> union) {
      return union.Is<TQuery>();
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   static bool CheckIndex<T1, T2, T3>(ref IsCase<T1, T2, T3> union, byte index) {
      return union.Is(index);
   }
}
