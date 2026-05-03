#nullable enable
using System;
using System.Runtime.CompilerServices;
namespace UnionUtil {
using static MethodImplOptions;
public interface IUnionTypeArguments<T1>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1> : Attribute;
public readonly struct FromTypeArgument1 {
   public static readonly FromTypeArgument1 Value = default;
}
public interface IUnionTypeArguments<T1, T2>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2> : Attribute;
public readonly struct FromTypeArgument2 {
   public static readonly FromTypeArgument2 Value = default;
}
public interface IUnionTypeArguments<T1, T2, T3>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2, T3> : Attribute;
public readonly struct FromTypeArgument3 {
   public static readonly FromTypeArgument3 Value = default;
}
public interface IUnionTypeArguments<T1, T2, T3, T4>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2, T3, T4> : Attribute;
public readonly struct FromTypeArgument4 {
   public static readonly FromTypeArgument4 Value = default;
}
public interface IUnionTypeArguments<T1, T2, T3, T4, T5>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2, T3, T4, T5> : Attribute;
public readonly struct FromTypeArgument5 {
   public static readonly FromTypeArgument5 Value = default;
}
public interface IUnionTypeArguments<T1, T2, T3, T4, T5, T6>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2, T3, T4, T5, T6> : Attribute;
public readonly struct FromTypeArgument6 {
   public static readonly FromTypeArgument6 Value = default;
}
public interface IUnionTypeArguments<T1, T2, T3, T4, T5, T6, T7>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2, T3, T4, T5, T6, T7> : Attribute;
public readonly struct FromTypeArgument7 {
   public static readonly FromTypeArgument7 Value = default;
}
public interface IUnionTypeArguments<T1, T2, T3, T4, T5, T6, T7, T8>;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionTypeArgumentsAttribute<T1, T2, T3, T4, T5, T6, T7, T8> : Attribute;
public readonly struct FromTypeArgument8 {
   public static readonly FromTypeArgument8 Value = default;
}
namespace UnionTypeExtensions {
public static class UnionTypeSwitchExpressionExtensions {
   extension<TUnion>(TUnion u) where TUnion : IUnionType {
      [MethodImpl(AggressiveInlining)]
      public R Switch<[HoldableAttribute(unique: true)] T1, R>(Func<T1, R> f1, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, R>(Func<T1, R> f1, Func<T2, R> f2, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, [HoldableAttribute(unique: true)] T3, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, [HoldableAttribute(unique: true)] T3, [HoldableAttribute(unique: true)] T4, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, [HoldableAttribute(unique: true)] T3, [HoldableAttribute(unique: true)] T4, [HoldableAttribute(unique: true)] T5, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<R>? _ = null) {
         if (u.TryGetValue(out T1 v1)) return f1(v1);
         if (u.TryGetValue(out T2 v2)) return f2(v2);
         if (u.TryGetValue(out T3 v3)) return f3(v3);
         if (u.TryGetValue(out T4 v4)) return f4(v4);
         if (u.TryGetValue(out T5 v5)) return f5(v5);
         if (_ is not null) return _();
         return ThrowHelpers.ThrowInvalidOperation<R>();
      }
      [MethodImpl(AggressiveInlining)]
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, [HoldableAttribute(unique: true)] T3, [HoldableAttribute(unique: true)] T4, [HoldableAttribute(unique: true)] T5, [HoldableAttribute(unique: true)] T6, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<R>? _ = null) {
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
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, [HoldableAttribute(unique: true)] T3, [HoldableAttribute(unique: true)] T4, [HoldableAttribute(unique: true)] T5, [HoldableAttribute(unique: true)] T6, [HoldableAttribute(unique: true)] T7, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<R>? _ = null) {
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
      public R Switch<[HoldableAttribute(unique: true)] T1, [HoldableAttribute(unique: true)] T2, [HoldableAttribute(unique: true)] T3, [HoldableAttribute(unique: true)] T4, [HoldableAttribute(unique: true)] T5, [HoldableAttribute(unique: true)] T6, [HoldableAttribute(unique: true)] T7, [HoldableAttribute(unique: true)] T8, R>(Func<T1, R> f1, Func<T2, R> f2, Func<T3, R> f3, Func<T4, R> f4, Func<T5, R> f5, Func<T6, R> f6, Func<T7, R> f7, Func<T8, R> f8, Func<R>? _ = null) {
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
   }
}
}// namespace UnionTypeExtensions
}// namespace UnionUtil
