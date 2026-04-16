using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable IDE1006,CS9113
[assembly: UnionUtil.UnionTypesConfig(32, nameof(UnionUtil), "UnionCases")]
namespace UnionUtil;

[AttributeUsage(AttributeTargets.Assembly)]
sealed class UnionTypesConfigAttribute(int arity, string? @namespace, string name) : Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionCasesAttribute(params string[] cases) : Attribute;

public enum Visibility : int { Private, Internal, Public }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionImplAttribute : Attribute {
   public bool WithFieldAccessors { get; init; }
   public string[]? FieldAccessorNames { get; init; }
   public bool BoxOpenGenerics { get; init; }
   public bool BoxManagedStructs { get; init; }
   public bool WithSetValueOverloads { get; init; }
   public bool Nullable { get; init; }
   public bool ReadOnly { get; init; }
   public Visibility FieldVisibility { get; init; }
}


public static class GenericHelpers {
   sealed class Boxed<T>(T item) {
      public T Item = item;
   }
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static object Box<T>(T v) {
      if (v is null) throw new ArgumentNullException(nameof(v));
      if (typeof(T).IsValueType) new Boxed<T>(v);
      return v;
   }
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Get<T>(object? v) {
      if (v is null) throw new ArgumentNullException(nameof(v));
      var type = typeof(T);
      if (type.IsClass || type.IsInterface) {
         return Unsafe.As<object, T>(ref v);
      } else if (type.IsValueType) {
         return Unsafe.As<Boxed<T>>(v).Item;
      }
      throw new InvalidOperationException($"{type.FullName}");
   }
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ref T Ref<T>(ref object? v) {
      if (v is null) throw new ArgumentNullException(nameof(v));
      var type = typeof(T);
      if (type.IsClass || type.IsInterface) {
         return ref Unsafe.As<object, T>(ref v);
      } else if (type.IsValueType) {
         return ref Unsafe.As<Boxed<T>>(v).Item;
      }
      throw new InvalidOperationException($"{type.FullName}");
   }
}
