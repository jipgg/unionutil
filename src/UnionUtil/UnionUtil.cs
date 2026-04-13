#pragma warning disable IDE1006
namespace UnionUtil;

file static class X {
   public const AttributeTargets Targets = AttributeTargets.Class | AttributeTargets.Struct;
   public const bool AllowMultiple = false;
}

public enum Visibility : int {
   Private = 0,
   Internal = 1,
   Public = 2,
}
[AttributeUsage(X.Targets, AllowMultiple = X.AllowMultiple)]
public sealed class UnionImplAttribute : Attribute {
   public bool BoxGenerics { get; init; }
   public bool BoxManagedStructs { get; init; }
   public bool Mutable { get; init; }
   public Visibility FieldVisibility { get; init; }
}
public interface UnionTypes<T1>;
public interface UnionTypes<T1, T2>;
public interface UnionTypes<T1, T2, T3>;
public interface UnionTypes<T1, T2, T3, T4>;
public interface UnionTypes<T1, T2, T3, T4, T5>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>;
public interface UnionTypes<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T16>;
