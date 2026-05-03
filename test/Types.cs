using System.Numerics;
namespace Test;

using static MethodImplOptions;
using static UnionGeneratorOptions;

public enum MutableTag { Int = 9, Double = 1, Vector3 = -3 }

public struct ManagedValue { public string Text; }

[GenerateUnion(BoxStructs | EnableUnionTypeInterface, FieldVisibility = Visibility.Internal)]
[SmallBufferOptimized(23)]
public partial struct ManagedStructUnion : IUnionTypeArguments<int, ManagedValue>;

[Tagged<MutableTag>]
[GenerateUnion(EnableNullable)]
[UnionTypeArguments<int, double, Vector3>]
partial struct MutableStruct;

[GenerateUnion(EnableGenericHoldsMethod | EnableUnionTypeInterface)]
partial struct ClassUnion<T, U, V> where T : class where U : class where V : class;

[GenerateUnion(BoxUnconstrainedGenerics | EnableUnionTypeInterface,
      FieldVisibility = Visibility.Internal), SmallBufferOptimized]
partial struct Union<T, U, V>;
[GenerateUnion(BoxUnconstrainedGenerics | EnableUnionTypeInterface,
      FieldVisibility = Visibility.Internal), SmallBufferOptimized]
readonly partial struct ReadOnlyUnion<T, U, V>;

[GenerateUnion]
partial struct MutableStruct2<T> where T : struct;

public enum Case { A, B, C, D, E, F, G }
[Tagged<Case>("Case"), GenerateUnion(
   FieldVisibility = Visibility.Internal
)]
public partial class BasicUnion<TA, TB, TC, TD, TE, TF, TG>;

public enum Result { Ok, Err }

[GenerateUnion, UnionTypeArguments<bool, Exception>, Tagged<Result>]
partial struct ResultVoid;

[Tagged<Result>, GenerateUnion(FieldVisibility = Visibility.Internal)]
public partial struct Result<T, E> : IUnionTypeArguments<T, E> where E : Exception;

[GenerateUnion(FieldVisibility = Visibility.Internal)]
[Tagged<Result>]
public partial struct Result<T> : IUnionTypeArguments<T, Exception> {
   [MethodImpl(AggressiveInlining)]
   public static implicit operator Result<T, Exception>(Result<T> result) {
      return result._index is 1 ? result._1 : Unsafe.As<Exception>(result._box!);
   }
   [MethodImpl(AggressiveInlining)]
   public static implicit operator Result<T>(Result<T, Exception> result) {
      return result._index is 1 ? result._1 : result._2;
   }
}

[Tagged<Result>, GenerateUnion(
   BoxStructs | BoxUnconstrainedGenerics | EnableGenericHoldsMethod,
   FieldVisibility = Visibility.Internal
)]
public partial struct BoxedResult<T, E> : IUnionTypeArguments<T, E> where E : Exception;

[Tagged<Result>, GenerateUnion(
      BoxStructs | BoxUnconstrainedGenerics | EnableGenericHoldsMethod,
      FieldVisibility = Visibility.Internal
)]
public partial struct BoxedResult<T> : IUnionTypeArguments<T, Exception> {
   [MethodImpl(AggressiveInlining)]
   public static implicit operator BoxedResult<T, Exception>(BoxedResult<T> result) {
      BoxedResult<T, Exception> r = default;
      r._index = result._index;
      r._box = result._box;
      return r;
   }
   [MethodImpl(AggressiveInlining)]
   public static implicit operator BoxedResult<T>(BoxedResult<T, Exception> result) {
      BoxedResult<T> r = default;
      r._index = result._index;
      r._box = result._box;
      return r;
   }
}
[GenerateUnion(
   EnableNullable | BoxUnconstrainedGenerics | EnableGenericHoldsMethod | EnableUnionTypeInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized(23)]
public partial struct Sbo23<T1, T2, T3>;
[GenerateUnion(
   EnableNullable | BoxUnconstrainedGenerics | EnableGenericHoldsMethod | EnableUnionTypeInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized(55)]
public partial struct Sbo55<T1, T2, T3>;
[GenerateUnion(
   EnableNullable | BoxUnconstrainedGenerics | EnableGenericHoldsMethod | EnableUnionTypeInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized(15)]
public partial struct Sbo15<T1, T2, T3> : IUnionTypeArguments<T1, T2, T3>;
[GenerateUnion(EnableNullable | BoxUnconstrainedGenerics | EnableGenericHoldsMethod | EnableUnionTypeInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized<SBO7>]
public partial struct Sbo7<T1, T2, T3>;

[InlineArray(7)]
public struct SBO7 : ISmallBuffer {
   byte _element0;

   public static int Size {
      [MethodImpl(AggressiveInlining)]
      get => 7;
   }
   [UnscopedRef]
   public ref byte Data {
      [MethodImpl(AggressiveInlining)]
      get => ref _element0;
   }
}
