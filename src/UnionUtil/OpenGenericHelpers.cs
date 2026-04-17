using System.Runtime.CompilerServices;
using System.Diagnostics;
namespace UnionUtil;
using static MethodImplOptions;

public static class OpenGenericHelpers {
   sealed class Boxed<T>(T item) {
      public T Item = item;
   }
   [MethodImpl(AggressiveInlining)]
   static bool BoxedIsAssumed<T>()  {
      return !RuntimeHelpers.IsReferenceOrContainsReferences<T>()
         || typeof(T).IsValueType;
   }
   [MethodImpl(AggressiveInlining)]
   public static object? Box<T>(T v) {
      if (BoxedIsAssumed<T>()) return new Boxed<T>(v);
      else return v;
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
   public static ref T Ref<T>(ref object? v) {
      if (BoxedIsAssumed<T>()) {
         Debug.Assert(v is Boxed<T>);
         return ref Unsafe.As<Boxed<T>>(v).Item;
      } else {
         return ref Unsafe.As<object?, T>(ref v);
      }
   }
}
