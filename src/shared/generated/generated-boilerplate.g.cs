#nullable enable
using System;
using System.Runtime.CompilerServices;
namespace UnionUtil;
using static MethodImplOptions;
public interface ICanHoldTypes<T1>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1> : Attribute;
public readonly struct FromIndex1 {
   public static readonly FromIndex1 Value = default;
}
public interface ICanHoldTypes<T1, T2>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2> : Attribute;
public readonly struct FromIndex2 {
   public static readonly FromIndex2 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3> : Attribute;
public readonly struct FromIndex3 {
   public static readonly FromIndex3 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4> : Attribute;
public readonly struct FromIndex4 {
   public static readonly FromIndex4 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5> : Attribute;
public readonly struct FromIndex5 {
   public static readonly FromIndex5 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6> : Attribute;
public readonly struct FromIndex6 {
   public static readonly FromIndex6 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7> : Attribute;
public readonly struct FromIndex7 {
   public static readonly FromIndex7 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8> : Attribute;
public readonly struct FromIndex8 {
   public static readonly FromIndex8 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Attribute;
public readonly struct FromIndex9 {
   public static readonly FromIndex9 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Attribute;
public readonly struct FromIndex10 {
   public static readonly FromIndex10 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Attribute;
public readonly struct FromIndex11 {
   public static readonly FromIndex11 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Attribute;
public readonly struct FromIndex12 {
   public static readonly FromIndex12 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Attribute;
public readonly struct FromIndex13 {
   public static readonly FromIndex13 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : Attribute;
public readonly struct FromIndex14 {
   public static readonly FromIndex14 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : Attribute;
public readonly struct FromIndex15 {
   public static readonly FromIndex15 Value = default;
}
public interface ICanHoldTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : Attribute;
public readonly struct FromIndex16 {
   public static readonly FromIndex16 Value = default;
}
public static class SwitchExpressionCompatibilityExtensions {
   extension<TUnion>(TUnion u) where TUnion : IUnionType {
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, R>(Func<T1, R> f1, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, R>(Func<T1, R> f1, Func<T2, R> f2, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, [CanHold(unique: true)] T11, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<T11, R> f11, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (u.TryGetValue(out T11 v11)) return f11(v11);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, [CanHold(unique: true)] T11, [CanHold(unique: true)] T12, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<T11, R> f11, Func<T12, R> f12, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (u.TryGetValue(out T11 v11)) return f11(v11);
         if (u.TryGetValue(out T12 v12)) return f12(v12);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, [CanHold(unique: true)] T11, [CanHold(unique: true)] T12, [CanHold(unique: true)] T13, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<T11, R> f11, Func<T12, R> f12, Func<T13, R> f13, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (u.TryGetValue(out T11 v11)) return f11(v11);
         if (u.TryGetValue(out T12 v12)) return f12(v12);
         if (u.TryGetValue(out T13 v13)) return f13(v13);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, [CanHold(unique: true)] T11, [CanHold(unique: true)] T12, [CanHold(unique: true)] T13, [CanHold(unique: true)] T14, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<T11, R> f11, Func<T12, R> f12, Func<T13, R> f13, Func<T14, R> f14, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (u.TryGetValue(out T11 v11)) return f11(v11);
         if (u.TryGetValue(out T12 v12)) return f12(v12);
         if (u.TryGetValue(out T13 v13)) return f13(v13);
         if (u.TryGetValue(out T14 v14)) return f14(v14);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, [CanHold(unique: true)] T11, [CanHold(unique: true)] T12, [CanHold(unique: true)] T13, [CanHold(unique: true)] T14, [CanHold(unique: true)] T15, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<T11, R> f11, Func<T12, R> f12, Func<T13, R> f13, Func<T14, R> f14, Func<T15, R> f15, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (u.TryGetValue(out T11 v11)) return f11(v11);
         if (u.TryGetValue(out T12 v12)) return f12(v12);
         if (u.TryGetValue(out T13 v13)) return f13(v13);
         if (u.TryGetValue(out T14 v14)) return f14(v14);
         if (u.TryGetValue(out T15 v15)) return f15(v15);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[CanHold(unique: true)] T1, [CanHold(unique: true)] T2, [CanHold(unique: true)] T3, [CanHold(unique: true)] T4, [CanHold(unique: true)] T5, [CanHold(unique: true)] T6, [CanHold(unique: true)] T7, [CanHold(unique: true)] T8, [CanHold(unique: true)] T9, [CanHold(unique: true)] T10, [CanHold(unique: true)] T11, [CanHold(unique: true)] T12, [CanHold(unique: true)] T13, [CanHold(unique: true)] T14, [CanHold(unique: true)] T15, [CanHold(unique: true)] T16, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<T9, R> f9, Func<T10, R> f10, Func<T11, R> f11, Func<T12, R> f12, Func<T13, R> f13, Func<T14, R> f14, Func<T15, R> f15, Func<T16, R> f16, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (u.TryGetValue(out T6 v6)) return f6(v6);
         if (u.TryGetValue(out T7 v7)) return f7(v7);
         if (u.TryGetValue(out T8 v8)) return f8(v8);
         if (u.TryGetValue(out T9 v9)) return f9(v9);
         if (u.TryGetValue(out T10 v10)) return f10(v10);
         if (u.TryGetValue(out T11 v11)) return f11(v11);
         if (u.TryGetValue(out T12 v12)) return f12(v12);
         if (u.TryGetValue(out T13 v13)) return f13(v13);
         if (u.TryGetValue(out T14 v14)) return f14(v14);
         if (u.TryGetValue(out T15 v15)) return f15(v15);
         if (u.TryGetValue(out T16 v16)) return f16(v16);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
   }
}
