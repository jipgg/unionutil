using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable IDE1006,CS9113
namespace UnionUtil;

[AttributeUsage(AttributeTargets.Assembly)]
sealed class UnionTypesConfigAttribute(int arity, string? @namespace, string name) : Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class TaggedAttribute<Tag>(string propertyName = "Tag") : Attribute where Tag : struct, Enum;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class UnionImplAttribute(UnionImplOptions options = UnionImplOptions.Default) : Attribute {
   public Visibility FieldVisibility { get; init; }
}

// 7 as default so it fills out the padding of the the type index field
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class SmallBufferOptimizedAttribute(uint size = 7) : Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class SmallBufferOptimizedAttribute<TSmallBuffer>() : Attribute where TSmallBuffer : ISmallBuffer;
