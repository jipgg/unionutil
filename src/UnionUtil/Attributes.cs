using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable IDE1006,CS9113
namespace UnionUtil;

[AttributeUsage(AttributeTargets.Assembly)]
sealed class UnionTypesConfigAttribute(int arity, string? @namespace, string name) : Attribute;

[AttributeUsage(AttributeTargets.Field)]
public sealed class NullTagAttribute : Attribute;

public enum Visibility : int { Private = 0, Internal = 1, Public = 2 }
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class TaggedAttribute<Tag>(string propertyName = "Tag") : Attribute where Tag : struct, Enum;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionImplAttribute : Attribute {
   public bool BoxOpenGenerics { get; init; }
   public bool BoxManagedStructs { get; init; }
   public bool Nullable { get; init; }
   public bool ReadOnly { get; init; }
   public Visibility FieldVisibility { get; init; }
}


