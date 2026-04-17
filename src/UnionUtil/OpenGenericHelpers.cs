using System.Runtime.CompilerServices;
namespace UnionUtil;

public static class OpenGenericHelpers {
   sealed class Boxed<T>(T item) {
      public T Item = item;
   }
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static object? Box<T>(T v) {
      if (!typeof(T).IsValueType) return v;
      return new Boxed<T>(v);
   }
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Get<T>(object? v) {
      var type = typeof(T);
      if (type.IsClass || type.IsInterface) {
         return Unsafe.As<object?, T>(ref v);
      } else if (type.IsValueType) {
         if (v is null) throw new ArgumentNullException(nameof(v));
         return Unsafe.As<Boxed<T>>(v).Item;
      }
      throw new InvalidOperationException($"{type.FullName}");
   }
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T Ref<T>(ref object? v) {
      var type = typeof(T);
      if (type.IsClass || type.IsInterface) {
         return ref Unsafe.As<object?, T>(ref v);
      } else if (type.IsValueType) {
         if (v is null) throw new ArgumentNullException(nameof(v));
         return ref Unsafe.As<Boxed<T>>(v).Item;
      }
      throw new InvalidOperationException($"{type.FullName}");
   }
}
