using System;
namespace UnionUtil;

[Flags]
public enum UnionImplOptions : uint {
   Default = 0,
   IncludeHoldsType = 1 << 0,
   BoxOpenGenerics = 1 << 1,
   BoxManagedStructs = 1 << 2,
   Nullable = 1 << 3,
   ReadOnly = 1 << 4,
   NoImplicitConversions = 1 << 5,
};
public enum Visibility { Private, Internal, Public }
