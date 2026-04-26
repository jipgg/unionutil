#if !SHARED_SHARED_INCLUDE_GUARD
#define SHARED_SHARED_INCLUDE_GUARD
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System;
#pragma warning disable IDE1006,CS9113
namespace UnionUtil;

using static FileScoped;

file static class FileScoped {
   public const AttributeTargets Targets = AttributeTargets.Class | AttributeTargets.Struct;
}

#if NET7_0_OR_GREATER
/// <summary>
/// Represents a small, fixed-size buffer that can be embedded directly into a union
/// to avoid heap allocations or large struct fields.
/// </summary>
/// <remarks>
/// Implementations should provide a fixed-size storage region and expose it via <see cref="Data"/>.
/// This is typically used for small-buffer optimization (SBO) scenarios.
/// </remarks>
public interface ISmallBuffer {
   /// <summary>
   /// Gets the size in bytes of the buffer.
   /// </summary>
   abstract static int Size { get; }
   /// <summary>
   /// Returns a reference to the first byte of the buffer.
   /// </summary>
   /// <remarks>
   /// This expects the the Data to be atleast the size of <see cref="Size"/>.
   /// </remarks>
   [UnscopedRef]
   ref byte Data { get; }
}
/// <summary>
/// Enables small buffer optimization for a union using a custom buffer type.
/// This optimization will only apply if <see cref="UnionImplOptions.BoxOpenGenerics"/>
/// or <see cref="UnionImplOptions.BoxManagedStructs"/> is set.
/// </summary>
/// <typeparam name="TSmallBuffer">
/// The buffer type that provides inline storage. Must implement <see cref="ISmallBuffer"/>.
/// </typeparam>
[AttributeUsage(Targets)]
public sealed class SmallBufferOptimizedAttribute<TSmallBuffer>() : Attribute where TSmallBuffer : ISmallBuffer;
#endif

[AttributeUsage(AttributeTargets.GenericParameter)]
public sealed class CanHoldAttribute(string? typeParamNameOfUnion = null, bool unique = false) : Attribute;

/// <summary>
/// Adds a strongly-typed tag field to the generated union and
/// generates named properties.
/// </summary>
/// <typeparam name="Tag">
/// An enum type used to represent the active variant.
/// </typeparam>
/// <param name="propertyName">
/// The name of the generated property exposing the tag.
/// </param>
[AttributeUsage(Targets)]
public sealed class TaggedAttribute<Tag>(string propertyName = "Tag") : Attribute where Tag : struct, Enum;

/// <summary>
/// Marks a type as a union and configures how its implementation is generated.
/// </summary>
/// <param name="options">
/// A set of flags controlling code generation behavior.
/// </param>
/// <remarks>
/// This attribute is the primary entry point for enabling source generation of union types.
/// </remarks>
[AttributeUsage(Targets)]
public sealed class UnionImplAttribute(UnionImplOptions options = UnionImplOptions.Default) : Attribute {
   /// <summary>
   /// Sets the visibility of the generated backing fields.
   /// </summary>
   public Visibility FieldVisibility { get; init; } = Visibility.Private;
}

/// <summary>
/// Enables small-buffer optimization with a fixed inline buffer size.
/// </summary>
/// <param name="size">
/// The size of the inline buffer in bytes.
/// Defaults to 7 as a means to recycle the padding produced by the <see langword="byte"/> type index field.
/// </param>
[AttributeUsage(Targets)]
public sealed class SmallBufferOptimizedAttribute(uint size = 7) : Attribute;

[Flags]
/// <summary>
/// Options that control how a union is generated.
/// </summary>
public enum UnionImplOptions : uint {
   Default = 0,
   /// <summary>
   /// Adds a `HoldsType&lt;T&gt;()` method.
   /// </summary>
   ImplementHoldsTypeMethod = 1 << 0,
   /// <summary>
   /// If enabled, open generics (without generic constraints) will be stored in a shared <see langword="object"/> field.
   /// Otherwise open generics will be stored sequentially.
   /// </summary>
   /// <remarks>
   /// If <see cref="SmallBufferOptimizedAttribute"/> or <see cref="SmallBufferOptimizedAttribute{TSmallBuffer}"/> are specified,
   /// <see langword="unmanaged"/> values will not be boxed if they fit in the sbo buffer.
   /// </remarks>
   BoxOpenGenerics = 1 << 1,
   /// <summary>
   /// If enabled, managed structs (and `where T: struct`) will be stored in a shared <see langword="object"/> field.
   /// Otherwise managed structs will be stored sequentially.
   /// </summary>
   /// <remarks>
   /// If <see cref="SmallBufferOptimizedAttribute"/> or <see cref="SmallBufferOptimizedAttribute{TSmallBuffer}"/> are specified,
   /// <see langword="unmanaged"/> values will not be boxed if they fit in the sbo buffer.
   /// </remarks>
   BoxManagedStructs = 1 << 2,
   /// <summary>
   /// Will generate a HasValue property, this is congruent with the proposed nullability for C#15 unions.
   /// </summary>
   EnableNullable = 1 << 3,
   /// <summary>
   /// Mainly useful for specifying immutability on <see langword="class"/>es.
   /// </summary>
   /// <remarks>
   /// If not specified, the generator will infer it based on whether the <see langword="readonly"/> keyword is specified on the struct.
   /// </remarks>
   EnableReadOnly = 1 << 4,
   /// <summary>
   /// Disables implicit conversion operators for union variants.
   /// </summary>
   NoImplicitConversions = 1 << 5,
   /// <summary>
   /// Adds explicit conversion operators for the values it can hold.
   /// </summary>
   WithExplicitConversionsToValue = 1 << 6,

   /// <summary>
   /// Implements <see cref="IUnionType"/> and tag interfaces like IHasTypeCountN, which provides a common generic interface
   /// in a type order agnostic manner. Mainly useful in generics <code>where TUnion : <see cref="IUnionType"/></code>.
   /// </summary>
   ImplementUnionInterfaces = 1 << 7,
};
public static class UnionImplOptionsExtenions {
   extension(UnionImplOptions opts) {
      public bool Has(UnionImplOptions opt) {
         return (opts & opt) != 0;
      }
   }
}
public enum Visibility { Private, Internal, Public }
public static class VisibilityExtensions {
   extension(Visibility v) {
      public string Keyword => v switch {
         Visibility.Internal => "internal",
         Visibility.Public => "public",
         _ => "private",
      };
   }
}

/// <summary>
/// Provides a uniform interface for querying, reading, and writing the contained value generically.
/// </summary>
public interface IUnionType {
#if NET7_0_OR_GREATER
   virtual static bool BoxesOpenGenerics { get; } = false;
   virtual static bool BoxesManagedStructs { get; } = false;
   virtual static int SmallBufferSize { get; } = 0;
   /// <summary>
   /// Returns <see langword="true"/> if <typeparamref name="T"/> is among the types this union
   /// is declared to hold, regardless of its current state.
   /// </summary>
   abstract static bool CanHoldType<T>();

   /// <summary>
   /// Gets a value indicating whether the union is immutable after construction.
   /// </summary>
   virtual static bool IsReadOnly { get; } = false;

   /// <summary>
   /// Gets a value indicating whether the union permits an empty state.
   /// </summary>
   virtual static bool IsNullable { get; } = false;

   /// <summary>
   /// Gets the number of distinct types the union can hold.
   /// </summary>
   abstract static int TypeCount { get; }
#endif

   /// <summary>
   /// Returns <see langword="true"/> if the currently stored value is of type <typeparamref name="T"/>.
   /// </summary>
   bool HoldsType<T>();

   /// <summary>
   /// Gets the currently stored value boxed as <see cref="object"/>,
   /// or <see langword="null"/> if the union is empty.
   /// </summary>
   object? Value { get; }

   /// <summary>
   /// Gets a value indicating whether the union currently contains a value.
   /// </summary>
   bool HasValue { get; }

   /// <summary>
   /// Attempts to store <paramref name="value"/> in the union.
   /// </summary>
   bool TrySetValue<T>(T value);

   /// <summary>
   /// Attempts to retrieve the stored value as <typeparamref name="T"/>.
   /// </summary>
   bool TryGetValue<T>(out T value);

   /// <summary>
   /// Attempts to reset the union to an empty state.
   /// </summary>
   bool TryClearValue();
}
#endif
