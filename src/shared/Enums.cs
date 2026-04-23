using System;
namespace UnionUtil;

[Flags]
public enum UnionImplOptions : uint {
   Default = 0,
   IncludeHoldsTypeMethod = 1 << 0,
   BoxOpenGenerics = 1 << 1,
   BoxManagedStructs = 1 << 2,
   NullableEnabled = 1 << 3,
   ReadOnlyEnabled = 1 << 4,
   NoImplicitConversions = 1 << 5,
   ExplicitlyConvertibleToValue = 1 << 6,
};
public static class UnionImplOptionsExtenions {
   extension(UnionImplOptions opts) {
      public bool Enabled(UnionImplOptions opt) {
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
