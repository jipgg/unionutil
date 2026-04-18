using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
namespace UnionUtil;

using static MethodImplOptions;

public interface ISmallBuffer {
   abstract static int Size { get; }
   [UnscopedRef]
   ref byte Data { get; }
}
public static class OpenGenericHelpers {
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
   public static void Box<T, TSbo>(ref TSbo sbo, ref object? obj, T v) where TSbo : ISmallBuffer, allows ref struct {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         var size = Unsafe.SizeOf<T>();
         if (size <= TSbo.Size) {
            Unsafe.WriteUnaligned(ref sbo.Data, v);
            return;
         }
         obj = new Boxed<T>(v);
         return;
      }
      if (typeof(T).IsValueType) {
         obj = new Boxed<T>(v);
         return;
      }
      obj = v;
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
   public static T Get<T, TSbo>(ref TSbo sbo, object? v) where TSbo : ISmallBuffer, allows ref struct {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() <= TSbo.Size) {
            return Unsafe.ReadUnaligned<T>(ref sbo.Data);
         }
         Debug.Assert(v is Boxed<T>);
         return Unsafe.As<Boxed<T>>(v).Item;
      }
      if (typeof(T).IsValueType) {
         Debug.Assert(v is Boxed<T>);
         return Unsafe.As<Boxed<T>>(v).Item;
      }
      return Unsafe.As<object?, T>(ref v);
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
   public static ref T Ref<T, TSbo>(ref TSbo sbo, ref object? v) where TSbo : ISmallBuffer, allows ref struct {
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
}
