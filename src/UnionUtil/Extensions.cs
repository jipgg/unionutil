namespace UnionUtil.Extensions;

public static class SwitchExtensions {
   extension<TUnion>(TUnion u) where TUnion : IUnionType {
      public R Switch<[CanHold(unique: true)] T1, R>
         (Func<T1, R> f1, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
      public R Switch
         <[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, R>
         (Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<R>? @default = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (@default is not null) return @default();
         throw new InvalidOperationException();
      }
   }
}
