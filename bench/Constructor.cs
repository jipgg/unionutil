[UnionImpl(ImplementFromIndexConstructors)]
public partial struct Union<T, U>;
[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors), SmallBufferOptimized(23)]
public partial struct BoxUnion<T, U>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class Constructor {

   [Benchmark]
   public Union<int, double> Seq_WithSentinel() {
      return new(FromIndex2.Value, 1230.0);
   }
   [Benchmark]
   public Union<int, double> Seq() {
      return new(1230.0);
   }
   [Benchmark]
   public Union<int, double> Seq_Implicit() {
      return 1230.0;
   }
   [Benchmark]
   public BoxUnion<int, double> Box_WithSentinel() {
      return new(FromIndex2.Value, 1230.0);
   }
   [Benchmark]
   public BoxUnion<int, double> Box() {
      return new(1230.0);
   }
   [Benchmark]
   public BoxUnion<int, double> Box_Implicit() {
      return 1230.0;
   }
}
