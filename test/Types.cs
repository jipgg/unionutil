using System.Runtime.CompilerServices;
using UnionUtil;
namespace Test;

using static MethodImplOptions;
using static UnionImplOptions;

public enum Result { Ok, Err }

[UnionImpl, CanHoldTypes<bool, Exception>, Tagged<Result>]
partial struct ResultVoid;

[Tagged<Result>, UnionImpl(FieldVisibility = Visibility.Internal)]
public partial struct Result<T, E> : ICanHoldTypes<T, E> where E : Exception;

[UnionImpl(FieldVisibility = Visibility.Internal)]
[Tagged<Result>]
public partial struct Result<T> : ICanHoldTypes<T, Exception> {
   [MethodImpl(AggressiveInlining)]
   public static implicit operator Result<T, Exception>(Result<T> result) {
      return Unsafe.As<Result<T>, Result<T, Exception>>(ref result);
   }
   [MethodImpl(AggressiveInlining)]
   public static implicit operator Result<T>(Result<T, Exception> result) {
      return Unsafe.As<Result<T, Exception>, Result<T>>(ref result);
   }
}

[Tagged<Result>, UnionImpl(
   BoxManagedStructs | BoxOpenGenerics | WithHoldsTypeMethod,
   FieldVisibility = Visibility.Internal
)]
public partial struct BoxedResult<T, E> : ICanHoldTypes<T, E> where E : Exception;

[Tagged<Result>, UnionImpl(
      BoxManagedStructs | BoxOpenGenerics | WithHoldsTypeMethod,
      FieldVisibility = Visibility.Internal
)]
public partial struct BoxedResult<T> : ICanHoldTypes<T, Exception> {
   [MethodImpl(AggressiveInlining)]
   public static implicit operator BoxedResult<T, Exception>(BoxedResult<T> result) {
      return Unsafe.As<BoxedResult<T>, BoxedResult<T, Exception>>(ref result);
   }
   [MethodImpl(AggressiveInlining)]
   public static implicit operator BoxedResult<T>(BoxedResult<T, Exception> result) {
      return Unsafe.As<BoxedResult<T, Exception>, BoxedResult<T>>(ref result);
   }
}
[UnionImpl(
   EnableNullable | BoxOpenGenerics | WithHoldsTypeMethod | ImplementCommonInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized(23)]
public partial struct Sbo23<T1, T2, T3> : ICanHoldTypes<T1, T2, T3>;
[UnionImpl(
   EnableNullable | BoxOpenGenerics | WithHoldsTypeMethod | ImplementCommonInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized(55)]
public partial struct Sbo55<T1, T2, T3> : ICanHoldTypes<T1, T2, T3>;
[UnionImpl(
   EnableNullable | BoxOpenGenerics | WithHoldsTypeMethod | ImplementCommonInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized(15)]
public partial struct Sbo15<T1, T2, T3> : ICanHoldTypes<T1, T2, T3>;
[UnionImpl(EnableNullable | BoxOpenGenerics | WithHoldsTypeMethod | ImplementCommonInterface,
   FieldVisibility = Visibility.Internal
), SmallBufferOptimized<SBO7>]
public partial struct Sbo7<T1, T2, T3> : ICanHoldTypes<T1, T2, T3>;
[InlineArray(7)]
public struct SBO7 : ISmallBuffer {
   byte _element0;

   public static int Size {
      [MethodImpl(AggressiveInlining)]
      get => 7;
   }
   [System.Diagnostics.CodeAnalysis.UnscopedRef]
   public ref byte Data {
      [MethodImpl(AggressiveInlining)]
      get => ref _element0;
   }
}
