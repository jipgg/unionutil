using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
namespace UnionUtil;
using static MethodImplOptions;

public sealed class InvalidValueAccessException<TValue> : InvalidOperationException {
   public InvalidValueAccessException() : base() { }
   public InvalidValueAccessException(string message) : base(message) { }
   public override string Message => $"Invalid value access ({typeof(TValue).FullName}). {base.Message}";
}
public sealed class InvalidTypeIndexException(byte typeIndex) : InvalidOperationException {
   public byte TypeIndex { get; } = typeIndex;
   public override string Message => $"Invalid type index '{TypeIndex}'";
}

public static class ThrowHelpers {
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowInvalidOperationException() => throw new InvalidOperationException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowInvalidOperationException(string message) => throw new InvalidOperationException(message);
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowInvalidOperationException<T>() => throw new InvalidOperationException();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowInvalidOperationException<T>(string message) => throw new InvalidOperationException(message);
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static void ThrowInvalidValueAccessException<TValue>() => throw new InvalidValueAccessException<TValue>();
   [DoesNotReturn, MethodImpl(NoInlining)]
   public static T ThrowInvalidValueAccessException<TValue, T>() => throw new InvalidValueAccessException<TValue>();

}
