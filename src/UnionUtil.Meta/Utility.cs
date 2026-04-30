using System.Buffers;
using System.Diagnostics;
namespace UnionUtil.Meta;

readonly record struct ResolvedSource(string HintName, StringBuilder Source);

static class Helpers {
   public static bool IsUnionUtil(ITypeSymbol? symbol) {
      if (symbol is null) return false;
      if (symbol.ContainingNamespace.IsGlobalNamespace) return false;
      return symbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace
         && symbol.ContainingNamespace?.MetadataName == "UnionUtil";
   }
   public static bool IsUnionUtil(AttributeData? attributeData) => IsUnionUtil(attributeData?.AttributeClass);

}
static class TypeSymbolExtensions {
   const string CanHoldTypes = "CanHoldTypes";
   extension(ITypeSymbol symbol) {
      public (ImmutableArray<ITypeSymbol>, bool ok) ResolveUnionTypeArgs() {
         var attr = symbol.GetAttributes()
            .SingleOrDefault(static e => Helpers.IsUnionUtil(e.AttributeClass)
                  && e.AttributeClass?.Name is $"{CanHoldTypes}Attribute");
         if (attr?.AttributeClass is INamedTypeSymbol a) {
            return (a.TypeArguments, true);
         }
         var inter = symbol.Interfaces
            .SingleOrDefault(static e => Helpers.IsUnionUtil(e)
                  && e.Arity is not 0
                  && e.Name is $"I{CanHoldTypes}");
         if (inter is not null) {
            return (inter.TypeArguments, true);
         }
         if (symbol is INamedTypeSymbol nts) {
            return (nts.TypeArguments, true);
         }
         return ([], false);
      }
      public bool IsDerivedFrom(ITypeSymbol type) {
         if (type is not { IsReferenceType: true }) return false;
         ITypeSymbol? b = symbol;
         while ((b = b.BaseType) != null) {
            if (!SymbolEqualityComparer.Default.Equals(b, type)) {
               continue;
            }
            return true;
         }
         return false;
      }
   }
}
