using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace UnionUtil.Internal;

using static MethodImplOptions;

public static class ThrowHelpers {
#if NET7_0_OR_GREATER
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowUnreachable() => throw new UnreachableException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowUnreachable(string? message) => throw new UnreachableException(message);

   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowUnreachable<T>() => throw new UnreachableException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowUnreachable<T>(string? message) => throw new UnreachableException(message);
#endif

#if NET5_0_OR_GREATER
   [DoesNotReturn]
#endif
   [MethodImpl(NoInlining)]
   public static void ThrowInvalidOperation() => throw new InvalidOperationException();
#if NET5_0_OR_GREATER
   [DoesNotReturn]
#endif
   [MethodImpl(NoInlining)]
   public static void ThrowInvalidOperation(string message) => throw new InvalidOperationException(message);

#if NET5_0_OR_GREATER
   [DoesNotReturn]
#endif
   [MethodImpl(NoInlining)]
   public static T ThrowInvalidOperation<T>() => throw new InvalidOperationException();
#if NET5_0_OR_GREATER
   [DoesNotReturn]
#endif
   [MethodImpl(NoInlining)]
   public static T ThrowInvalidOperation<T>(string message) => throw new InvalidOperationException(message);
}
