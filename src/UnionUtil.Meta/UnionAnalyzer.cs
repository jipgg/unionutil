using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Buffers;
using SpanUtility;
using UnionUtil.Internal;
namespace UnionUtil.Meta;

using static SyntaxKind;

// hot garbage code, should probably be refactored eventually
// can probably be optimized quite a bit aswell
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnionAnalyzer : DiagnosticAnalyzer {
   public override void Initialize(AnalysisContext ctx) {
      ctx.EnableConcurrentExecution();
      ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
      ctx.RegisterSyntaxNodeAction(UnionDeclaration, StructDeclaration, ClassDeclaration);
      ctx.RegisterSyntaxNodeAction(HoldsTypeMethod, InvocationExpression);
      ctx.RegisterSyntaxNodeAction(GenericUnionMethod, InvocationExpression);
      ctx.RegisterSyntaxNodeAction(CanHoldValidation, SyntaxKind.MethodDeclaration);
   }
   static readonly DiagnosticDescriptor MissingUnionImplMarker = new(
      "UU0001",
      $"missing {nameof(GenerateUnionAttribute)} marker",
      $"'{{0}}' does nothing without marking with '{nameof(GenerateUnionAttribute)}'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static readonly DiagnosticDescriptor TypesCouldNotBeInferred = new(
      "UU0002",
      "types could not be inferred",
      $"could not infer types, mark them with 'I{MetaConfiguration.TypeMarkerName}' or '{MetaConfiguration.TypeMarkerName}Attribute'",
      "Usage",
      DiagnosticSeverity.Error,
      true
   );
   static readonly DiagnosticDescriptor TagEnumNotExhaustive = new(
      "UU0003",
      "tag enum is not exhaustive",
      "length of '{0}' does not match the count of possible types the union can hold",
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
   static readonly DiagnosticDescriptor CanNeverHoldType = new(
      "UU0005",
      "can never hold type",
      "target union can never hold type '{0}'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static readonly DiagnosticDescriptor TypeMustBeUnique = new(
      "UU0008",
      "type is not unique",
      "target union type is not unique '{0}'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );

   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
      MissingUnionImplMarker,
      TypesCouldNotBeInferred,
      TagEnumNotExhaustive,
      MissingPartialKeyword,
      CanNeverHoldType,
      CanHoldBadSource,
      CanHoldNoUnionSource,
      TypeMustBeUnique,
   ];

   static void HoldsTypeMethod(SyntaxNodeAnalysisContext ctx) {
      var sm = ctx.SemanticModel;
      var invocation = (InvocationExpressionSyntax)ctx.Node;
      if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess) return;
      if (memberAccess.Name.Identifier.Text is not "Holds") return;
      if (sm.GetTypeInfo(memberAccess.Expression).Type is not INamedTypeSymbol symbol) return;
      var (typeArgs, _) = symbol.ResolveUnionTypeArgs();
      if (typeArgs.Length is 0) return;
      switch (memberAccess.Name) {
         case GenericNameSyntax syntax when syntax.Arity is 1:
            var typeStx = syntax.TypeArgumentList.Arguments[0];
            var type = sm.GetTypeInfo(typeStx).Type;
            if (type is ITypeParameterSymbol or null) return;
            foreach (var e in typeArgs) {
               if (e is ITypeParameterSymbol tp) {
                  switch (tp) {
                     case { HasValueTypeConstraint: true }:
                        if (type.IsValueType) return;
                        continue;
                     case { HasConstructorConstraint: true }:
                        if (type.IsValueType) return;
                        continue;
                     case { HasNotNullConstraint: true }:
                        if (type.NullableAnnotation is not NullableAnnotation.Annotated) return;
                        continue;
                     case { HasReferenceTypeConstraint: true }:
                        if (type.IsReferenceType) return;
                        continue;
                     case { HasUnmanagedTypeConstraint: true }:
                        if (type.IsUnmanagedType) return;
                        continue;
                     default:
                        return;
                  }
               }
               if (SymbolEqualityComparer.Default.Equals(type, e)) return;
            }
            ctx.ReportDiagnostic(Diagnostic.Create(CanNeverHoldType, typeStx.GetLocation(), type));
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
         .Where(Helpers.IsUnionUtil)
         .ToArray();
      var interfaces = symbol.Interfaces.Where(Helpers.IsUnionUtil).ToArray();
      SymbolData? generateUnion = null;
      SymbolData? taggedSymbol = null;
      SymbolData? sboSymbol = null;
      SymbolData? unionTypeArguments = null;
      var (typeArgs, typeArgsOk) = symbol.ResolveUnionTypeArgs();
      foreach (var e in attributes) {
         var loc = e.ApplicationSyntaxReference?
            .GetSyntax(ctx.CancellationToken)
            .GetLocation() ?? symbol.Locations.First();
         switch (e.AttributeClass!.Name) {
            case nameof(GenerateUnionAttribute):
               generateUnion = new(e.AttributeClass, loc);
               break;
            case nameof(TaggedAttribute<>):
               taggedSymbol = new(e.AttributeClass, loc);
               break;
            case nameof(SmallBufferOptimizedAttribute):
               sboSymbol = new(e.AttributeClass, loc);
               break;
            case $"{MetaConfiguration.TypeMarkerName}Attribute":
               unionTypeArguments = new(e.AttributeClass, loc);
               break;
         }
      }
      if (generateUnion is SymbolData impl) goto unionimpl_not_null;
      void diagnoseMissing(in SymbolData? s) {
         if (s is not SymbolData sd) return;
         ctx.ReportDiagnostic(Diagnostic.Create(MissingUnionImplMarker, sd.Loc, sd.Sym));
      }
      diagnoseMissing(taggedSymbol);
      diagnoseMissing(sboSymbol);
      diagnoseMissing(unionTypeArguments);
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
      method = method.GetConstructedReducedFrom() ?? method;
      var original = method.OriginalDefinition;

      var typeParams = original.TypeParameters;
      var typeArgs = method.TypeArguments;

      if (original.ContainingType.IsExtension) {
         typeParams = [.. original.ContainingType.TypeParameters, .. typeParams];
         typeArgs = [.. method.ContainingType.TypeArguments, .. typeArgs];
      }
      if (typeParams.Length == 0) return;

      using var unionParams = new SpanDictionary<int, ITypeParameterSymbol>(typeParams.Length);
      using var canHoldParams = new SpanList<(int canHoldIdx, int unionIdx, AttributeData attrData, bool unique)>(typeParams.Length);

      const int unresolved = -1;

      for (int i = 0; i < typeParams.Length; i++) {
         var tp = typeParams[i];

         bool isUnion = false;
         foreach (var c in tp.ConstraintTypes) {
            if (c is not INamedTypeSymbol named) continue;
            if (!Helpers.IsUnionUtil(named)) continue;
            if (named.MetadataName is not nameof(IUnionType)) continue;
            unionParams.Add(i, tp);
            isUnion = true;
            break;
         }
         if (isUnion) continue;

         AttributeData? canHoldAttr = null;
         foreach (var attr in tp.GetAttributes()) {
            if (!Helpers.IsUnionUtil(attr)) continue;
            if (attr.AttributeClass!.MetadataName is not nameof(HoldableAttribute)) continue;
            canHoldAttr = attr;
            break;
         }
         if (canHoldAttr is null) continue;
         canHoldParams.Add((i, unresolved, canHoldAttr, default));
      }

      if (unionParams.Count is 0 || canHoldParams.Count is 0) return;

      foreach (ref var u in canHoldParams) {
         var sourceName = (string?)u.attrData.ConstructorArguments[0].Value;
         u.unique = (bool)u.attrData.ConstructorArguments[1].Value!;
         if (sourceName is null) {
            // only valid if theres exactly 1 union param
            if (unionParams.Count != 1) continue;
            foreach (var (idx, _) in unionParams) {
               u.unionIdx = idx;
               break;
            }
            continue;
         }
         foreach (var (idx, tp) in unionParams) {
            if (tp.Name != sourceName) continue;
            u.unionIdx = idx;
            break;
         }
      }
      var uniqueArgs = new SpanList<(int unionIdx, ITypeSymbol type)>(canHoldParams.Count);
      foreach (var (canHoldIdx, unionIdx, _, unique) in canHoldParams) {
         if (unionIdx is unresolved) continue;
         var resolvedUnion = typeArgs[unionIdx];
         var resolvedArg = typeArgs[canHoldIdx];
         if (resolvedUnion is ITypeParameterSymbol || resolvedArg is ITypeParameterSymbol) continue;
         var (memberTypes, ok) = resolvedUnion.ResolveUnionTypeArgs();
         if (!ok || memberTypes.Length == 0) continue;
         if (memberTypes.Any(static m => m is ITypeParameterSymbol)) continue;

         bool found = false;
         foreach (var member in memberTypes) {
            if (SymbolEqualityComparer.Default.Equals(resolvedArg, member)) {
               found = true;
               break;
            }
         }

         Location loc = invocation.GetLocation();
         int methodTypeParamIdx = original.ContainingType.IsExtension
            ? canHoldIdx - original.ContainingType.TypeParameters.Length
            : canHoldIdx;
         int argOffset = method.IsExtensionMethod && method.ReducedFrom is null ? 1 : 0;

         static bool usesTypeParam(ITypeSymbol paramType, int typeParamIdx, IMethodSymbol method) {
            if (paramType is ITypeParameterSymbol tps
               && tps.DeclaringMethod is not null
               && SymbolEqualityComparer.Default.Equals(tps.DeclaringMethod, method)
               && tps.Ordinal == typeParamIdx) {
               return true;
            }
            if (paramType is INamedTypeSymbol named) {
               foreach (var typeArg in named.TypeArguments) {
                  if (usesTypeParam(typeArg, typeParamIdx, method)) return true;
               }
            }
            return false;
         }

         for (int pi = 0; pi < original.Parameters.Length; pi++) {
            var paramType = original.Parameters[pi].Type;
            if (usesTypeParam(paramType, methodTypeParamIdx, original)) {
               int argIdx = pi - argOffset;
               if (argIdx >= 0 && argIdx < invocation.ArgumentList.Arguments.Count) {
                  loc = invocation.ArgumentList.Arguments[argIdx].Expression.GetLocation();
               }
               break;
            }
         }

         if (!found) {
            ctx.ReportDiagnostic(Diagnostic.Create(CanNeverHoldType, loc, resolvedArg));
            return;
         }

         if (unique) {
            foreach (var (otherUnionId, type) in uniqueArgs) {
               if (otherUnionId != unionIdx) continue;
               if (SymbolEqualityComparer.Default.Equals(type, resolvedArg)) {
                  ctx.ReportDiagnostic(Diagnostic.Create(TypeMustBeUnique, loc, resolvedArg));
                  continue;
               }
            }
            uniqueArgs.Add(new(unionIdx, resolvedArg));
         }
      }
   }
   static readonly DiagnosticDescriptor CanHoldNoUnionSource = new(
      "UU0006",
      "CanHold has no union source",
      $"'{{0}}' is marked with {nameof(HoldableAttribute)} but no unique {nameof(IUnionType)}-constrained type parameter exists",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );

   static readonly DiagnosticDescriptor CanHoldBadSource = new(
      "UU0007",
      "CanHold references invalid type parameter",
      $"'{{0}}' references type parameter '{{1}}' which does not exist or is not constrained to {nameof(IUnionType)}",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );

   static void CanHoldValidation(SyntaxNodeAnalysisContext ctx) {
      var node = (MethodDeclarationSyntax)ctx.Node;
      var sm = ctx.SemanticModel;

      if (sm.GetDeclaredSymbol(node, ctx.CancellationToken) is not IMethodSymbol method) return;
      if (!method.IsGenericMethod) return;
      var typeParams = method.TypeParameters;
      if (method.ContainingType.IsExtension) {
         typeParams = [.. method.ContainingType.TypeParameters, .. typeParams];
      }
      var unionIndices = new SpanList<int>(stackalloc int[method.TypeParameters.Length]);

      for (int i = 0; i < typeParams.Length; i++) {
         var tp = typeParams[i];
         foreach (var c in tp.ConstraintTypes) {
            if (c is INamedTypeSymbol named
               && Helpers.IsUnionUtil(named)
               && named.Name is nameof(IUnionType)
               && named.Arity == 0) {
               unionIndices.Add(i);
               break;
            }
         }
      }
      for (int i = 0; i < typeParams.Length; i++) {
         var tp = typeParams[i];
         var canHold = tp.GetAttributes()
            .FirstOrDefault(a => Helpers.IsUnionUtil(a) && a.AttributeClass!.Name is nameof(HoldableAttribute));

         if (canHold is null) continue;

         var loc = canHold.ApplicationSyntaxReference?
            .GetSyntax(ctx.CancellationToken)
            .GetLocation() ?? tp.Locations.FirstOrDefault() ?? node.GetLocation();

         if (canHold.ConstructorArguments.Length > 0
            && canHold.ConstructorArguments[0].Value is string sourceName) {
            bool found = false;
            for (int j = 0; j < unionIndices.Count; j++) {
               if (typeParams[unionIndices[j]].Name == sourceName) {
                  found = true;
                  break;
               }
            }
            if (!found) {
               ctx.ReportDiagnostic(Diagnostic.Create(CanHoldBadSource, loc, tp.Name, sourceName));
            }
         } else {
            if (unionIndices.Count is not 1) {
               ctx.ReportDiagnostic(Diagnostic.Create(CanHoldNoUnionSource, loc, tp.Name));
            }
         }
      }
   }
}
