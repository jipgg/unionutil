using System.Runtime.CompilerServices;
using System.Buffers;
namespace UnionUtil.Meta;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnionAnalyzer : DiagnosticAnalyzer {
   public override void Initialize(AnalysisContext ctx) {
      ctx.EnableConcurrentExecution();
      ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
      ctx.RegisterSyntaxNodeAction(UnionDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.ClassDeclaration);
      ctx.RegisterSyntaxNodeAction(HoldsTypeMethod, SyntaxKind.InvocationExpression);
      ctx.RegisterSyntaxNodeAction(GenericUnionMethod, SyntaxKind.InvocationExpression);
   }
   static readonly DiagnosticDescriptor MissingUnionImplMarker = new(
      "UU0001",
      "missing UnionImpl marker",
      "'{0}' does nothing without marking with 'UnionUtil.UnionImplAttribute'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static readonly DiagnosticDescriptor TypesCouldNotBeInferred = new(
      "UU0002",
      "types could not be inferred",
      "could not infer types, mark them with 'IUnion' or 'UnionAttribute'",
      "Usage",
      DiagnosticSeverity.Error,
      true
   );
   static readonly DiagnosticDescriptor TagEnumNotExhaustive = new(
      "UU0003",
      "tag enum is not exhaustive",
      "length of '{0}' does not match the count of possible union cases",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static readonly DiagnosticDescriptor MissingPartialKeyword = new(
      "UU0004",
      "missing partial keyword",
      "type is missing partial specifier",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static readonly DiagnosticDescriptor WillNeverHoldType = new(
      "UU0005",
      "will never hold type",
      "will never hold type '{0}'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
      MissingUnionImplMarker,
      TypesCouldNotBeInferred,
      TagEnumNotExhaustive,
      MissingPartialKeyword,
      WillNeverHoldType,
   ];

   static bool IsUnionUtil(ITypeSymbol? symbol) {
      if (symbol is null) return false;
      if (symbol.ContainingNamespace.IsGlobalNamespace) return false;
      return symbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace
         && symbol.ContainingNamespace?.MetadataName == "UnionUtil";
   }
   static bool IsUnionUtil(AttributeData? attributeData) => IsUnionUtil(attributeData?.AttributeClass);
   static void HoldsTypeMethod(SyntaxNodeAnalysisContext ctx) {
      var sm = ctx.SemanticModel;
      var invocation = (InvocationExpressionSyntax)ctx.Node;
      if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;
      if (memberAccess.Name.Identifier.Text is not "HoldsType") return;
      if (sm.GetTypeInfo(memberAccess.Expression).Type is not INamedTypeSymbol symbol) return;
      INamedTypeSymbol? @interface = null;
      if (IsUnionUtil(symbol) && symbol.Name is "IUnion" && symbol.Arity is not 0) {
         @interface = symbol;
      }
      if (@interface is null) {
         @interface = symbol.Interfaces
            .Where(e => IsUnionUtil(e) && e.Name is "IUnion" && symbol.Arity is not 0)
            .FirstOrDefault();
      }
      if (@interface is null) return;
      if (@interface.TypeArguments.OfType<ITypeParameterSymbol>().Any()) return;
      switch (memberAccess.Name) {
         case GenericNameSyntax syntax when syntax.Arity is 1:
            var typeStx = syntax.TypeArgumentList.Arguments[0];
            var type = sm.GetTypeInfo(typeStx).Type;
            if (type is ITypeParameterSymbol or null) return;
            foreach (var e in @interface.TypeArguments) {
               if (e is ITypeParameterSymbol tp) {
                  switch (tp) {
                     case { HasValueTypeConstraint: true }:
                        if (!type.IsValueType) continue;
                        break;
                     case { HasConstructorConstraint: true }:
                        if (!type.IsValueType) continue;
                        break;
                     case { HasNotNullConstraint: true }:
                        if (type.NullableAnnotation is NullableAnnotation.Annotated) continue;
                        break;
                     case { HasReferenceTypeConstraint: true }:
                        if (!type.IsReferenceType) continue;
                        break;
                     case { HasUnmanagedTypeConstraint: true }:
                        if (!type.IsUnmanagedType) continue;
                        break;
                     default:
                        return;
                  }
               }
               if (SymbolEqualityComparer.Default.Equals(type, e)) return;
            }
            ctx.ReportDiagnostic(Diagnostic.Create(WillNeverHoldType, typeStx.GetLocation(), type));
            break;
         default:
            break;
      }
   }
   readonly record struct SymbolData(INamedTypeSymbol Sym, Location Loc);
   static void UnionDeclaration(SyntaxNodeAnalysisContext ctx) {
      var node = (TypeDeclarationSyntax)ctx.Node;
      var declaredSymbol = ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, ctx.CancellationToken);
      if (declaredSymbol is not INamedTypeSymbol symbol) return;
      var attributes = symbol.GetAttributes()
         .Where(IsUnionUtil)
         .ToArray();
      var interfaces = symbol.Interfaces.Where(IsUnionUtil).ToArray();
      SymbolData? unionImpl = null;
      SymbolData? taggedSymbol = null;
      SymbolData? sboSymbol = null;
      SymbolData? unionSymbol = null;
      var (typeArgs, typeArgsOk) = symbol.ResolveUnionTypeArgs();
      foreach (var e in attributes) {
         var loc = e.ApplicationSyntaxReference?
            .GetSyntax(ctx.CancellationToken)
            .GetLocation() ?? symbol.Locations.First();
         switch (e.AttributeClass!.Name) {
            case "UnionImplAttribute":
               unionImpl = new(e.AttributeClass, loc);
               break;
            case "TaggedAttribute":
               taggedSymbol = new(e.AttributeClass, loc);
               break;
            case "SmallBufferOptimizedAttribute":
               sboSymbol = new(e.AttributeClass, loc);
               break;
            case "UnionAttribute":
               unionSymbol = new(e.AttributeClass, loc);
               break;
         }
      }
      if (unionImpl is SymbolData impl) goto unionimpl_not_null;
      void diagnoseMissing(in SymbolData? s) {
         if (s is not SymbolData sd) return;
         ctx.ReportDiagnostic(Diagnostic.Create(MissingUnionImplMarker, sd.Loc, sd.Sym));
      }
      diagnoseMissing(taggedSymbol);
      diagnoseMissing(sboSymbol);
      diagnoseMissing(unionSymbol);
      return;
   unionimpl_not_null:
      if (!node.Modifiers.Any(SyntaxKind.PartialKeyword)) {
         ctx.ReportDiagnostic(Diagnostic.Create(MissingPartialKeyword, symbol.Locations.First()));
      }
      if (!typeArgsOk || typeArgs.Length is 0) {
         ctx.ReportDiagnostic(Diagnostic.Create(TypesCouldNotBeInferred, impl.Loc));
      }
      if (taggedSymbol is SymbolData tagged) {
         var tags = tagged.Sym.TypeArguments[0].GetMembers()
            .OfType<IFieldSymbol>()
            .Where(e => e.HasConstantValue)
            .ToArray();
         if (tags.Length != typeArgs.Length) {
            ctx.ReportDiagnostic(Diagnostic.Create(TagEnumNotExhaustive, tagged.Loc, tagged.Sym));
         }
      }
      return;
   }
   static void GenericUnionMethod(SyntaxNodeAnalysisContext ctx) {
      var sm = ctx.SemanticModel;
      var invocation = (InvocationExpressionSyntax)ctx.Node;

      if (sm.GetSymbolInfo(invocation, ctx.CancellationToken).Symbol is not IMethodSymbol method) return;
      if (!method.IsGenericMethod) return;

      var original = method.OriginalDefinition;

      var unionParamIndices = new Dictionary<int, int>();
      var fromUnionBindings = new Dictionary<int, int>();

      for (int i = 0; i < original.TypeParameters.Length; i++) {
         var tp = original.TypeParameters[i];
         foreach (var c in tp.ConstraintTypes) {
            if (c is INamedTypeSymbol named
               && IsUnionUtil(named)
               && named.Name is "IUnion"
               && named.Arity == 0) {
               unionParamIndices[i] = i;
               break;
            }
         }
      }

      if (unionParamIndices.Count == 0) return;

      for (int i = 0; i < original.TypeParameters.Length; i++) {
         if (unionParamIndices.ContainsKey(i)) continue;

         var tp = original.TypeParameters[i];
         var fromUnion = tp.GetAttributes()
            .FirstOrDefault(a => IsUnionUtil(a) && a.AttributeClass!.Name is "FromUnionAttribute");

         if (fromUnion is null) continue;

         if (fromUnion.ConstructorArguments.Length > 0
            && fromUnion.ConstructorArguments[0].Value is string sourceName) {
            for (int j = 0; j < original.TypeParameters.Length; j++) {
               if (original.TypeParameters[j].Name == sourceName && unionParamIndices.ContainsKey(j)) {
                  fromUnionBindings[i] = j;
                  break;
               }
            }
         } else if (unionParamIndices.Count == 1) {
            fromUnionBindings[i] = unionParamIndices.Keys.First();
         }
      }

      if (fromUnionBindings.Count == 0) return;

      foreach (var pair in fromUnionBindings) {
         var vi = pair.Key;
         var ui = pair.Value;
         var resolvedUnion = method.TypeArguments[ui];
         var resolvedValue = method.TypeArguments[vi];

         if (resolvedUnion is ITypeParameterSymbol || resolvedValue is ITypeParameterSymbol) continue;

         var (memberTypes, ok) = resolvedUnion.ResolveUnionTypeArgs();
         if (!ok || memberTypes.Length == 0) continue;
         if (memberTypes.Any(m => m is ITypeParameterSymbol)) continue;

         bool found = false;
         foreach (var member in memberTypes) {
            if (SymbolEqualityComparer.Default.Equals(resolvedValue, member)) {
               found = true;
               break;
            }
         }
         if (found) continue;

         Location loc = invocation.GetLocation();
         for (int pi = 0; pi < original.Parameters.Length; pi++) {
            if (original.Parameters[pi].Type is ITypeParameterSymbol tps
               && tps.Ordinal == vi
               && pi < invocation.ArgumentList.Arguments.Count) {
               loc = invocation.ArgumentList.Arguments[pi].Expression.GetLocation();
               break;
            }
         }

         ctx.ReportDiagnostic(Diagnostic.Create(WillNeverHoldType, loc, resolvedValue));
      }
   }
}
