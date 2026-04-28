using System.Runtime.CompilerServices;
using Union = Union<uint, ulong, System.UInt128, U256>;
using BoxUnion7 = BoxUnion7<uint, ulong, System.UInt128, U256>;
using BoxUnion23 = BoxUnion23<uint, ulong, System.UInt128, U256>;
using BoxUnion0 = BoxUnion0<uint, ulong, System.UInt128, U256>;

[InlineArray(256)]
struct U256 {
   byte _0;
   [MethodImpl(AggressiveInlining)]
   public U256(long v) => Unsafe.WriteUnaligned(ref _0, v);
}

[UnionImpl(ImplementFromIndexConstructors)]
public partial struct Union<T, U, V, W>;
[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors), SmallBufferOptimized(23)]
public partial struct BoxUnion23<T, U, V, W>;

[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors), SmallBufferOptimized(7)]
public partial struct BoxUnion7<T, U, V, W>;
[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors)]
public partial struct BoxUnion0<T, U, V, W>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class ConstructAndCopy {
   [Params(10, 1_000, 1_000_000)]
   public int Iterations;
   [MethodImpl(NoInlining)]
   static void Copy(Union _) { }
   [MethodImpl(NoInlining)]
   static void Copy(BoxUnion7 _) { }
   [MethodImpl(NoInlining)]
   static void Copy(BoxUnion23 _) { }
   [MethodImpl(NoInlining)]
   static void Copy(BoxUnion0 _) { }
   [Benchmark(Baseline = true)]
   public void Seq_U32() {
      var u = new Union(1230);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box23_U32() {
      var u = new BoxUnion23(1230);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box7_U32() {
      var u = new BoxUnion7(1230);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box0_U32() {
      var u = new BoxUnion0(1230);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Seq_U64() {
      var u = new Union(1230ul);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box23_U64() {
      var u = new BoxUnion23(1230ul);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box7_U64() {
      var u = new BoxUnion7(1230ul);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box0_U64() {
      var u = new BoxUnion0(1230ul);
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Seq_U128() {
      var u = new Union(new UInt128(0, 1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box23_U128() {
      var u = new BoxUnion23(new UInt128(0, 1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box7_U128() {
      var u = new BoxUnion7(new UInt128(0, 1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box0_U128() {
      var u = new BoxUnion0(new UInt128(0, 1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Seq_U256() {
      var u = new Union(new U256(1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box23_U256() {
      var u = new BoxUnion23(new U256(1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box7_U256() {
      var u = new BoxUnion7(new U256(1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
   [Benchmark]
   public void Box0_U256() {
      var u = new BoxUnion0(new U256(1230));
      for (int i = 0; i < Iterations; ++i) Copy(u);
   }
}
