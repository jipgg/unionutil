#pragma warning disable CS8618
using static UnionUtil.ThrowHelpers;
using UnionUtil.UnionTypeExtensions;
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

[GenerateUnion(EnableUnionTypeInterface | BoxUnconstrainedGenerics), SmallBufferOptimized]
partial struct UnionUtilBoxVariant<T1, T2, T3, T4, T5, T6, T7, T8>;

[GenerateUnion(EnableUnionTypeInterface)]
readonly partial struct UnionUtilSequentialVariant<T1, T2, T3, T4, T5, T6, T7, T8>;


// quick trivial comparison between commonly used discriminated union libraries
[MemoryDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class MatchVariants {
   [Params(1, 6, 8)]
   public int TN;
   DunetVariant<int, double, float, uint, nuint, Exception, Int128, bool> _dunet;
   UnionUtilBoxVariant<int, double, float, uint, nuint, Exception, Int128, bool> _unionUtilBox;
   UnionUtilSequentialVariant<int, double, float, uint, nuint, Exception, Int128, bool> _unionUtilSeq;
   OneOf.OneOf<int, double, float, uint, nuint, Exception, Int128, bool> _oneOf;

   [GlobalSetup]
   public void Setup() {
      switch (TN) {
         case 1:
            int i = 123;
            _dunet = i;
            _unionUtilBox = i;
            _unionUtilSeq = i;
            _oneOf = i;
            break;
         case 6:
            Exception ex = new("eadwadwadawdjfawjfawf");
            _dunet = ex;
            _unionUtilBox = ex;
            _unionUtilSeq = ex;
            _oneOf = ex;
            break;
         case 8:
            bool b = true;
            _dunet = b;
            _unionUtilBox = b;
            _unionUtilSeq = b;
            _oneOf = b;
            break;
         default:
            throw new();
      }
   }
   [Benchmark]
   public int UnionUtil_Seq_Match() {
      return _unionUtilSeq.Switch(
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
   public int UnionUtil_Seq_Match_Static() {
      return _unionUtilSeq.Switch(
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
   
   public int UnionUtil_Seq_switch() {
      if (_unionUtilSeq.TryGetValue(out int _)) return TN * 1;
      if (_unionUtilSeq.TryGetValue(out double _)) return TN * 2;
      if (_unionUtilSeq.TryGetValue(out float _)) return TN * 3;
      if (_unionUtilSeq.TryGetValue(out uint _)) return TN * 4;
      if (_unionUtilSeq.TryGetValue(out nuint _)) return TN * 5;
      if (_unionUtilSeq.TryGetValue(out Exception _)) return TN * 6;
      if (_unionUtilSeq.TryGetValue(out Int128 _)) return TN * 7;
      if (_unionUtilSeq.TryGetValue(out bool _)) return TN * 8;
      ThrowInvalidOperation();
      return default!;
   }

   [Benchmark]
   public int UnionUtil_Box_Match() {
      return _unionUtilBox.Switch(
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
   public int UnionUtil_Box_Match_Static() {
      return _unionUtilBox.Switch(
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
   public int UnionUtil_Box_switch() {
      if (_unionUtilBox.TryGetValue(out int _)) return TN * 1;
      if (_unionUtilBox.TryGetValue(out double _)) return TN * 2;
      if (_unionUtilBox.TryGetValue(out float _)) return TN * 3;
      if (_unionUtilBox.TryGetValue(out uint _)) return TN * 4;
      if (_unionUtilBox.TryGetValue(out nuint _)) return TN * 5;
      if (_unionUtilBox.TryGetValue(out Exception _)) return TN * 6;
      if (_unionUtilBox.TryGetValue(out Int128 _)) return TN * 7;
      if (_unionUtilBox.TryGetValue(out bool _)) return TN * 8;
      ThrowInvalidOperation();
      return default!;
   }
   [Benchmark]
   public int OneOf_Match() {
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
   public int OneOf_Match_Static() {
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
   public int Dunet_Match() {
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
   public int Dunet_Match_Static() {
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
         _ => ThrowInvalidOperation<int>(),
      };
   }
}
