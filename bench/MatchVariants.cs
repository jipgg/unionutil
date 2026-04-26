using UnionUtil.Extensions;
using static DunetVariant<int, double, float, uint, nuint, System.Exception, System.Int128, bool>;

[Dunet.Union]
abstract partial record DunetVariant<T1, T2, T3, T4, T5, T6, T7, T8> {
   public partial record X1(T1 X);
   public partial record X2(T2 X);
   public partial record X3(T3 X);
   public partial record X4(T4 X);
   public partial record X5(T5 X);
   public partial record X6(T6 X);
   public partial record X7(T7 X);
   public partial record X8(T8 X);
}

[UnionImpl(ImplementUnionInterfaces | BoxOpenGenerics)]
partial struct UnionUtilVariant<T1, T2, T3, T4, T5, T6, T7, T8>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class MatchVariants {
   [Params(1, 6, 8)]
   public int TN;
   DunetVariant<int, double, float, uint, nuint, Exception, Int128, bool> _dunet;
   UnionUtilVariant<int, double, float, uint, nuint, Exception, Int128, bool> _unionUtil;
   OneOf.OneOf<int, double, float, uint, nuint, Exception, Int128, bool> _oneOf;

   [GlobalSetup]
   public void Setup() {
      switch (TN) {
         case 1:
            int i = 123;
            _dunet = i;
            _unionUtil = i;
            _oneOf = i;
            break;
         case 6:
            Exception ex = new("eadwadwadawdjfawjfawf");
            _dunet = ex;
            _unionUtil = ex;
            _oneOf = ex;
            break;
         case 8:
            bool b = true;
            _dunet = b;
            _unionUtil = b;
            _oneOf = b;
            break;
         default:
            throw new();
      }
   }

   [Benchmark]
   public int UnionUtil() {
      return _unionUtil.Switch(
         (int _) => TN * 1,
         (double _) => TN * 2,
         (float _) => TN * 3,
         (uint _) => TN * 4,
         (nuint _) => TN * 5,
         (Exception _) => TN * 6,
         (Int128 _) => TN * 7,
         (bool _) => TN * 8
      );
   }
   [Benchmark]
   public int UnionUtil_Static() {
      return _unionUtil.Switch(
         static (int _) => 1,
         static (double _) => 2,
         static (float _) => 3,
         static (uint _) => 4,
         static (nuint _) => 5,
         static (Exception _) => 6,
         static (Int128 _) => 7,
         static (bool _) => 8
      );
   }
   [Benchmark]
   public int UnionUtil_switch() {
      if (_unionUtil.TryGetValue(out int _)) return TN * 1;
      if (_unionUtil.TryGetValue(out double _)) return TN * 2;
      if (_unionUtil.TryGetValue(out float _)) return TN * 3;
      if (_unionUtil.TryGetValue(out uint _)) return TN * 4;
      if (_unionUtil.TryGetValue(out nuint _)) return TN * 5;
      if (_unionUtil.TryGetValue(out Exception _)) return TN * 6;
      if (_unionUtil.TryGetValue(out Int128 _)) return TN * 7;
      if (_unionUtil.TryGetValue(out bool _)) return TN * 8;
      throw new InvalidOperationException();
   }
   [Benchmark]
   public int OneOf() {
      return _oneOf.Match(
         _ => TN * 1,
         _ => TN * 2,
         _ => TN * 3,
         _ => TN * 4,
         _ => TN * 5,
         _ => TN * 6,
         _ => TN * 7,
         _ => TN * 8
      );
   }
   [Benchmark]
   public int OneOf_Static() {
      return _oneOf.Match(
         static _ => 1,
         static _ => 2,
         static _ => 3,
         static _ => 4,
         static _ => 5,
         static _ => 6,
         static _ => 7,
         static _ => 8
      );
   }
   [Benchmark]
   public int Dunet() {
      return _dunet.Match(
         _ => TN * 1,
         _ => TN * 2,
         _ => TN * 3,
         _ => TN * 4,
         _ => TN * 5,
         _ => TN * 6,
         _ => TN * 7,
         _ => TN * 8
      );
   }
   [Benchmark]
   public int Dunet_Static() {
      return _dunet.Match(
         static _ => 1,
         static _ => 2,
         static _ => 3,
         static _ => 4,
         static _ => 5,
         static _ => 6,
         static _ => 7,
         static _ => 8
      );
   }
   [Benchmark]
   public int Dunet_switch() {
      return _dunet switch {
         X1(var _) => TN * 1,
         X2(var _) => TN * 2,
         X3(var _) => TN * 3,
         X4(var _) => TN * 4,
         X5(var _) => TN * 5,
         X6(var _) => TN * 6,
         X7(var _) => TN * 7,
         X8(var _) => TN * 8,
         _ => throw new InvalidOperationException(),
      };
   }
}
