using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
namespace UnionUtil;

using static ThrowHelpers;
using static MethodImplOptions;

public sealed class Boxed<T>(T value) : IEquatable<T> {
   public T Item = value;
   public bool Equals(T? other) => EqualityComparer<T>.Default.Equals(Item, other);
   public override bool Equals(object? obj) => obj is T x && Equals(x);
   public override int GetHashCode() => Item?.GetHashCode() ?? 0;
   public override string ToString() => Item?.ToString() ?? "";
   public static bool operator ==(Boxed<T> a, Boxed<T> b) => EqualityComparer<T>.Default.Equals(a.Item, b.Item);
   public static bool operator !=(Boxed<T> a, Boxed<T> b) => !(a == b);
   public static bool operator ==(Boxed<T> a, T b) => EqualityComparer<T>.Default.Equals(a.Item, b);
   public static bool operator !=(Boxed<T> a, T b) => !(a == b);
   public static bool operator ==(T a, Boxed<T> b) => EqualityComparer<T>.Default.Equals(a, b.Item);
   public static bool operator !=(T a, Boxed<T> b) => !(a == b);
}

public static class BoxHelpers {
   [MethodImpl(AggressiveInlining)]
   public static int GetSmallBufferSize<TSmallBuffer>() where TSmallBuffer : ISmallBuffer {
      return TSmallBuffer.Size;
   }
   [MethodImpl(AggressiveInlining)]
   public static void Update<T, TSmallBuffer>(ref TSmallBuffer sbo, ref object? obj, T v) where TSmallBuffer : ISmallBuffer {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() > TSmallBuffer.Size) goto update_boxed;
         Unsafe.WriteUnaligned(ref sbo.Data, v);
         return;
      }
      if (typeof(T).IsValueType) goto update_boxed;
      obj = v;
      return;
   update_boxed:
      if (obj is Boxed<T> boxed) boxed.Item = v;
      else ThrowUnreachable();
   }
   [MethodImpl(AggressiveInlining)]
   public static void Update<T>(ref object? obj, T v) {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>() || typeof(T).IsValueType) {
         if (obj is Boxed<T> boxed) {
            boxed.Item = v;
            return;
         }
         ThrowUnreachable();
      }
      if (typeof(T).IsValueType) ThrowUnreachable();
      obj = v;
   }
   [MethodImpl(AggressiveInlining)]
   public static void Write<T, TSmallBuffer>(ref TSmallBuffer sbo, ref object? obj, T v) where TSmallBuffer : ISmallBuffer {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() > TSmallBuffer.Size) goto box;
         Unsafe.WriteUnaligned(ref sbo.Data, v);
         return;
      }
      if (typeof(T).IsValueType) goto box;
      obj = v;
      return;
   box:
      obj = new Boxed<T>(v);
      return;
   }
   [MethodImpl(AggressiveInlining)]
   public static void Write<T>(ref object? obj, T v) {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() is false || typeof(T).IsValueType) {
         if (obj is null) {
            obj = new Boxed<T>(v);
            return;
         }
      }
      if (typeof(T).IsValueType) ThrowUnreachable();
      obj = v;
   }
   [MethodImpl(AggressiveInlining)]
   public static T Read<T, TSmallBuffer>(ref TSmallBuffer sbo, object? obj) where TSmallBuffer : ISmallBuffer {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() is false) {
         if (Unsafe.SizeOf<T>() > TSmallBuffer.Size) goto boxed;
         return Unsafe.ReadUnaligned<T>(ref sbo.Data);
      }
      if (typeof(T).IsValueType) goto boxed;
      return Unsafe.As<object?, T>(ref obj);
   boxed:
      if (obj is Boxed<T> boxed) return boxed.Item;
      return ThrowUnreachable<T>();
   }
   [MethodImpl(AggressiveInlining)]
   public static T Read<T>(object? obj) {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() is false || typeof(T).IsValueType) {
         if (obj is Boxed<T> boxed) return boxed.Item;
         ThrowUnreachable();
      }
      if (obj is T v) return v;
      return ThrowUnreachable<T>();
   }
}
