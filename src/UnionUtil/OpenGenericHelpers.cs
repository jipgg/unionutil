using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
namespace UnionUtil;

using static MethodImplOptions;

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
   public unsafe static void Box<T>(Span<byte> sbo, ref object? obj, T v) {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         var size = Unsafe.SizeOf<T>();
         if (size <= sbo.Length) {
            var copied = MemoryMarshal
               .CreateSpan(ref Unsafe.As<T, byte>(ref v), size)
               .TryCopyTo(sbo);
            Debug.Assert(copied);
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
   public static T Get<T>(ReadOnlySpan<byte> sbo, object? v) {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() <= sbo.Length) {
            return Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(sbo));
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
   public static ref T Ref<T>(ReadOnlySpan<byte> sbo, ref object? v) {
      if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         if (Unsafe.SizeOf<T>() <= sbo.Length) {
            return ref Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(sbo));
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
