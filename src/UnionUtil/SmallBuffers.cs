using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
namespace UnionUtil;

using static MethodImplOptions;

public interface ISmallBuffer {
   abstract static int Size { get; }
   [UnscopedRef]
   ref byte Data { get; }
}

[InlineArray(7)]
public struct SmallBuffer7 : ISmallBuffer {
   byte _element0;
   public static int Size {
      [MethodImpl(AggressiveInlining)]
      get => 7;
   }
   [UnscopedRef]
   public ref byte Data {
      [MethodImpl(AggressiveInlining)]
      get => ref _element0;
   }
}
[InlineArray(15)]
public struct SmallBuffer15 : ISmallBuffer {
   byte _element0;

   public static int Size {
      [MethodImpl(AggressiveInlining)]
      get => 15;
   }
   [UnscopedRef]
   public ref byte Data {
      [MethodImpl(AggressiveInlining)]
      get => ref _element0;
   }
}
[InlineArray(23)]
public struct SmallBuffer23 : ISmallBuffer {
   byte _element0;

   public static int Size {
      [MethodImpl(AggressiveInlining)]
      get => 23;
   }
   [UnscopedRef]
   public ref byte Data {
      [MethodImpl(AggressiveInlining)]
      get => ref _element0;
   }
}

