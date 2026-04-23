namespace UnionUtil;

using static UnionImplOptions;
using static FileLocalConstants;

file static class FileLocalConstants {
   public const UnionImplOptions ValueUnionOptions = IncludeHoldsTypeMethod | ExplicitlyConvertibleToValue;
   public const UnionImplOptions UnionOptions = IncludeHoldsTypeMethod | ExplicitlyConvertibleToValue | BoxOpenGenerics;
}

[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2> : IUnion<T1, T2>;
[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2, T3, T4> : IUnion<T1, T2, T3, T4>;
[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2, T3, T4, T5> : IUnion<T1, T2, T3, T4, T5>;
[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2, T3, T4, T5, T6> : IUnion<T1, T2, T3, T4, T5, T6>;
[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2, T3, T4, T5, T6, T7> : IUnion<T1, T2, T3, T4, T5, T6, T7>;
[UnionImpl(ValueUnionOptions)]
public partial struct ValueUnion<T1, T2, T3, T4, T5, T6, T7, T8> : IUnion<T1, T2, T3, T4, T5, T6, T7, T8>;

[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2> : IUnion<T1, T2>;
[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2, T3> : IUnion<T1, T2, T3>;
[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2, T3, T4> : IUnion<T1, T2, T3, T4>;
[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2, T3, T4, T5> : IUnion<T1, T2, T3, T4, T5>;
[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2, T3, T4, T5, T6> : IUnion<T1, T2, T3, T4, T5, T6>;
[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2, T3, T4, T5, T6, T7> : IUnion<T1, T2, T3, T4, T5, T6, T7>;
[UnionImpl(UnionOptions), SmallBufferOptimized]
public partial struct Union<T1, T2, T3, T4, T5, T6, T7, T8> : IUnion<T1, T2, T3, T4, T5, T6, T7, T8>;
