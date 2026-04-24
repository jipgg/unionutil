namespace UnionUtil.Meta;

static class Result {
   public const bool Ok = true;
   public const bool Err = false;
}
readonly struct FromError;
readonly struct Result<T, E> {
   public readonly T? Val;
   public readonly E? Err;
   public readonly bool Ok;
   public Result(in T value) {
      Ok = true;
      Val = value;
      Err = default;
   }
   public static implicit operator Result<T, E>(in T value) => new(in value);
   public Result(in FromError _, E error) {
      Ok = false;
      Val = default;
      Err = error;
   }
   public static implicit operator bool(in Result<T, E> r) => r.Ok;
}
static class TypeSymbolExtensions {
   static bool IsInUnionUtil(ITypeSymbol? symbol) {
      if (symbol is null) return false;
      if (symbol.ContainingNamespace.IsGlobalNamespace) return false;
      return symbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace
         && symbol.ContainingNamespace?.MetadataName == "UnionUtil";
   }
   extension(ITypeSymbol symbol) {
      public (ImmutableArray<ITypeSymbol>, bool ok) ResolveUnionTypeArgs() {
         var attr = symbol.GetAttributes()
            .SingleOrDefault(static e => IsInUnionUtil(e.AttributeClass)
                  && e.AttributeClass?.Name is Config.UnionType.AttributeName);
         if (attr?.AttributeClass is INamedTypeSymbol a) {
            return (a.TypeArguments, true);
         }
         var inter = symbol.Interfaces
            .SingleOrDefault(static e => IsInUnionUtil(e)
                  && e.Arity is not 0
                  && e.Name is Config.UnionType.InterfaceName);
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
