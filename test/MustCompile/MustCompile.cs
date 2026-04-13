using UnionUtil;
namespace MustCompile;


[UnionImpl(
   FieldVisibility = Visibility.Public
)]
sealed partial class SealedClass : UnionTypes<int, float, object, List<object>>;

#if NET10_0_OR_GREATER
#endif
[UnionImpl(FieldVisibility = Visibility.Internal)]
readonly partial struct Result<T> : UnionTypes<T, Exception> {
   public T Ok {
      get {
         if (TryGetValue(out T ok)) return ok;
         throw new();
      }
   }
   public Exception? Ex {
      get {
         if (TryGetValue(out Exception ex)) return ex;
         return null;
      }
   }
}
[UnionImpl(FieldVisibility = Visibility.Internal)]
readonly partial struct Result<T, E> : UnionTypes<T, E> where E : Exception {
   public T Ok {
      get {
         if (TryGetValue(out T ok)) return ok;
         throw new();
      }
   }
   public E? Ex {
      get {
         if (TryGetValue(out E err)) return err;
         return null;
      }
   }
   public static implicit operator Result<T>(Result<T, E> r) {
      if (r.TryGetValue(out E ex)) {
         return new(ex);
      }
      if (!r.TryGetValue(out T ok)) throw new();
      return new(ok);
   }
}

static class Test {
   static void Block() {
      SealedClass x = 0.0f;
      Result<int, Exception> res = new InvalidOperationException();
      Global g = 1;
      if (g.HasValue) {
         var c = g.Value;
      }
   }
}
