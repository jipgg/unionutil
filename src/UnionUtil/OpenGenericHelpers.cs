using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
namespace UnionUtil;

using static MethodImplOptions;

public static class OpenGenericHelpers {
   [MethodImpl(AggressiveInlining)]
   public static int GetSmallBufferSize<TSmallBuffer>() where TSmallBuffer : ISmallBuffer {
      return TSmallBuffer.Size;
   }
   sealed class Boxed<T>(T item) {
      public T Item = item;
   }
   [MethodImpl(AggressiveInlining)]
   static bool BoxedIsAssumed<T>() {
      return !RuntimeHelpers.IsReferenceOrContainsReferences<T>()
         || typeof(T).IsValueType;
   }
   [MethodImpl(AggressiveInlining)]
   public static object? Box<T>(T v) {
      if (BoxedIsAssumed<T>()) return new Boxed<T>(v);
      else return v;
   }
   [MethodImpl(AggressiveInlining)]
   public static void Box<T, TSbo>(ref TSbo sbo, ref object? obj, T v) where TSbo : ISmallBuffer {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         var size = Unsafe.SizeOf<T>();
         if (size <= TSbo.Size) {
            Unsafe.WriteUnaligned(ref sbo.Data, v);
            return;
         }
         goto box_value;
      }
      if (typeof(T).IsValueType) {
         obj = new Boxed<T>(v);
         return;
      }
      obj = v;
      return;
   box_value:
      obj = new Boxed<T>(v);
      return;
   }
   [MethodImpl(AggressiveInlining)]
   public static T Get<T>(object? v) {
      if (BoxedIsAssumed<T>()) {
         Debug.Assert(v is Boxed<T>);
         return Unsafe.As<Boxed<T>>(v).Item;
      } else {
         return Unsafe.As<object?, T>(ref v);
      }
   }
   [MethodImpl(AggressiveInlining)]
   public static T Get<T, TSbo>(ref TSbo sbo, object? v) where TSbo : ISmallBuffer {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() > TSbo.Size) {
            goto read_boxed_value;
         }
         return Unsafe.ReadUnaligned<T>(ref sbo.Data);
      }
      if (typeof(T).IsValueType) {
         goto read_boxed_value;
      }
      Debug.Assert(v?.GetType().IsValueType is false);
      return Unsafe.As<object?, T>(ref v);
   read_boxed_value:
      Debug.Assert(v is Boxed<T>);
      return Unsafe.As<Boxed<T>>(v).Item;
   }
   [MethodImpl(AggressiveInlining)]
   public static ref T Ref<T>(ref object? v) {
      if (BoxedIsAssumed<T>()) {
         Debug.Assert(v is Boxed<T>);
         return ref Unsafe.As<Boxed<T>>(v).Item;
      } else {
         return ref Unsafe.As<object?, T>(ref v);
      }
   }
   [MethodImpl(AggressiveInlining)]
   public static void Set<T>(ref object? v, T x) {
      if (BoxedIsAssumed<T>()) {
         Debug.Assert(v is Boxed<T>);
         Unsafe.As<Boxed<T>>(v).Item = x;
      } else {
         Unsafe.As<object?, T>(ref v) = x;
      }
   }
   [MethodImpl(AggressiveInlining)]
   public static ref T Ref<T, TSbo>(ref TSbo sbo, ref object? v) where TSbo : ISmallBuffer {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() <= TSbo.Size) {
            return ref Unsafe.As<byte, T>(ref sbo.Data);
         }
         Debug.Assert(v is Boxed<T>);
         return ref Unsafe.As<Boxed<T>>(v).Item;
      }
      if (typeof(T).IsValueType) {
         Debug.Assert(v is Boxed<T>);
         return ref Unsafe.As<Boxed<T>>(v).Item;
      }
      return ref Unsafe.As<object?, T>(ref v);
   }
   [MethodImpl(AggressiveInlining)]
   public static void Set<T, TSbo>(ref TSbo sbo, ref object? v, T x) where TSbo : ISmallBuffer {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() > TSbo.Size) goto box_value;
         Unsafe.WriteUnaligned(ref sbo.Data, x);
         return;
      }
      if (typeof(T).IsValueType) goto box_value;
      v = x;
      return;
   box_value:
      Debug.Assert(v is Boxed<T>);
      Unsafe.As<Boxed<T>>(v).Item = x;
      return;
   }
}
