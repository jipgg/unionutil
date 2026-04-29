#pragma warning disable IDE0034
using Union = Union<uint, ulong, System.UInt128>;
using BoxUnion7 = BoxUnion7<uint, ulong, System.UInt128>;
using BoxUnion23 = BoxUnion23<uint, ulong, System.UInt128>;
using BoxUnion0 = BoxUnion0<uint, ulong, System.UInt128>;
[UnionImpl(ImplementFromIndexConstructors)]
public partial struct Union<T, U, V>;
[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors), SmallBufferOptimized(23)]
public partial struct BoxUnion23<T, U, V>;

[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors), SmallBufferOptimized(7)]
public partial struct BoxUnion7<T, U, V>;
[UnionImpl(BoxOpenGenerics | ImplementFromIndexConstructors)]
public partial struct BoxUnion0<T, U, V>;

[MemoryDiagnoser, DisassemblyDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net10_0)]
public class Constructor {
   [Benchmark]
   public Union Seq_WithSentinel_U32() {
      return new(default(FromIndex1), 1230);
   }
   [Benchmark]
   public Union Seq_U32() {
      return new(1230);
   }
   [Benchmark]
   public Union Seq_Implicit_U32() {
      return 1230;
   }
   [Benchmark]
   public BoxUnion23 Box23_WithSentinel_U32() {
      return new(default(FromIndex1), 1230);
   }
   [Benchmark]
   public BoxUnion23 Box23_U32() {
      return new(1230);
   }
   [Benchmark]
   public BoxUnion23 Box23_Implicit_U32() {
      return 1230;
   }
   [Benchmark]
   public BoxUnion7 Box7_WithSentinel_U32() {
      return new(default(FromIndex1), 1230);
   }
   [Benchmark]
   public BoxUnion7 Box7_U32() {
      return new(1230);
   }
   [Benchmark]
   public BoxUnion7 Box7_Implicit_U32() {
      return 1230;
   }
   [Benchmark]
   public BoxUnion0 Box0_WithSentinel_U32() {
      return new(default(FromIndex1), 1230);
   }
   [Benchmark]
   public BoxUnion0 Box0_U32() {
      return new(1230);
   }
   [Benchmark]
   public BoxUnion0 Box0_Implicit_U32() {
      return 1230;
   }

   [Benchmark]
   public Union Seq_WithSentinel_U64() {
      return new(default(FromIndex2), 1230);
   }
   [Benchmark]
   public Union Seq_U64() {
      return new(1230ul);
   }
   [Benchmark]
   public Union Seq_Implicit_U64() {
      return 1230ul;
   }
   [Benchmark]
   public BoxUnion23 Box23_WithSentinel_U64() {
      return new(default(FromIndex2), 1230);
   }
   [Benchmark]
   public BoxUnion23 Box23_U64() {
      return new(1230ul);
   }
   [Benchmark]
   public BoxUnion23 Box23_Implicit_U64() {
      return 1230ul;
   }
   [Benchmark]
   public BoxUnion7 Box7_WithSentinel_U64() {
      return new(default(FromIndex2), 1230);
   }
   [Benchmark]
   public BoxUnion7 Box7_U64() {
      return new(1230ul);
   }
   [Benchmark]
   public BoxUnion7 Box7_Implicit_U64() {
      return 1230ul;
   }
   [Benchmark]
   public BoxUnion0 Box0_WithSentinel_U64() {
      return new(default(FromIndex2), 1230);
   }
   [Benchmark]
   public BoxUnion0 Box0_U64() {
      return new(1230ul);
   }
   [Benchmark]
   public BoxUnion0 Box0_Implicit_U64() {
      return 1230ul;
   }

   [Benchmark]
   public Union Seq_WithSentinel_U128() {
      return new(default(FromIndex3), 1230);
   }
   [Benchmark]
   public Union Seq_U128() {
      return new(new UInt128(0, 1230));
   }
   [Benchmark]
   public Union Seq_Implicit_U128() {
      return new UInt128(0, 1230);
   }
   [Benchmark]
   public BoxUnion23 Box23_WithSentinel_U128() {
      return new(default(FromIndex3), 1230);
   }
   [Benchmark]
   public BoxUnion23 Box23_U128() {
      return new(new UInt128(0, 1230));
   }
   [Benchmark]
   public BoxUnion23 Box23_Implicit_U128() {
      return new UInt128(0, 1230);
   }
   [Benchmark]
   public BoxUnion7 Box7_WithSentinel_U128() {
      return new(default(FromIndex3), 1230);
   }
   [Benchmark]
   public BoxUnion7 Box7_U128() {
      return new(new UInt128(0, 1230));
   }
   [Benchmark]
   public BoxUnion7 Box7_Implicit_U128() {
      return new UInt128(0, 1230ul);
   }
   [Benchmark]
   public BoxUnion0 Box0_WithSentinel_U128() {
      return new(default(FromIndex3), 1230);
   }
   [Benchmark]
   public BoxUnion0 Box0_U128() {
      return new(new UInt128(0, 1230));
   }
   [Benchmark]
   public BoxUnion0 Box0_Implicit_U128() {
      return new UInt128(0, 1230);
   }
}
