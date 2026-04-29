using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace UnionUtil;

using static MethodImplOptions;

public static class ThrowHelpers {
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowUnreachable() => throw new UnreachableException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowUnreachable(string? message) => throw new UnreachableException(message);

   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowUnreachable<T>() => throw new UnreachableException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowUnreachable<T>(string? message) => throw new UnreachableException(message);

   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowInvalidOperation() => throw new InvalidOperationException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowInvalidOperation(string message) => throw new InvalidOperationException(message);

   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowInvalidOperation<T>() => throw new InvalidOperationException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowInvalidOperation<T>(string message) => throw new InvalidOperationException(message);
}
