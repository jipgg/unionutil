using System.Runtime.CompilerServices;
using System.Buffers;
namespace UnionUtil.Meta.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnionAnalyzer : DiagnosticAnalyzer {

   static string MakeId(string name) => $"{nameof(UnionUtil)}_{name}";

   static DiagnosticDescriptor MissingUnionImpl => new(
      MakeId(nameof(MissingUnionImpl)),
      "missing UnionImpl marker",
      "'{0}' does nothing without marking with 'UnionUtil.UnionImplAttribute'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static DiagnosticDescriptor MissingTypesMarker => new(
      MakeId((nameof(MissingTypesMarker))),
      "missing types marker",
      "types must be marked with 'IUnion<...T>' or 'UnionAttribute<...T>'",
      "Usage",
      DiagnosticSeverity.Error,
      true
   );
   static DiagnosticDescriptor BadTagEnumLength => new(
      MakeId(nameof(BadTagEnumLength)),
      "bad tag enum length",
      "length of '{0}' does not match type count of '{1}'",
      "Usage",
      DiagnosticSeverity.Error,
      true
   );
   static DiagnosticDescriptor MissingPartial => new(
      MakeId(nameof(MissingPartial)),
      "missing partial specifier",
      "type is missing partial specifier",
      "Usage",
      DiagnosticSeverity.Error,
      true
   );
   static DiagnosticDescriptor WillNeverHoldType => new(
      MakeId(nameof(WillNeverHoldType)),
      "will never hold type",
      "will never hold type '{0}'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
      MissingUnionImpl,
      MissingTypesMarker,
      BadTagEnumLength,
      MissingPartial,
      WillNeverHoldType,
   ];

   public override void Initialize(AnalysisContext ctx) {
      ctx.EnableConcurrentExecution();
      ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze);
      ctx.RegisterSyntaxNodeAction(UnionDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.ClassDeclaration);
      ctx.RegisterSyntaxNodeAction(HoldsTypeMethod, SyntaxKind.InvocationExpression);
   }
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
      if (IsUnionUtil(symbol) && symbol.Name is "IUnion") {
         @interface = symbol;
      }
      if (@interface is null) {
         @interface = symbol.Interfaces
            .Where(e => IsUnionUtil(e) && e.Name is "IUnion")
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
      foreach (var e in interfaces) {
         if (e.Name is not "IUnion") continue;
         var loc = e.DeclaringSyntaxReferences.FirstOrDefault()?
            .GetSyntax(ctx.CancellationToken).GetLocation() ?? symbol.Locations.First();
         unionSymbol = new(e, loc);
      }
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
         ctx.ReportDiagnostic(Diagnostic.Create(MissingUnionImpl, sd.Loc, sd.Sym));
      }
      diagnoseMissing(taggedSymbol);
      diagnoseMissing(sboSymbol);
      diagnoseMissing(unionSymbol);
      return;
   unionimpl_not_null:
      if (!node.Modifiers.Any(SyntaxKind.PartialKeyword)) {
         ctx.ReportDiagnostic(Diagnostic.Create(MissingPartial, symbol.Locations.First()));
      }
      if (unionSymbol is null) {
         ctx.ReportDiagnostic(Diagnostic.Create(MissingTypesMarker, impl.Loc));
      }
      if (taggedSymbol is SymbolData tagged && unionSymbol is SymbolData union) {
         var tags = tagged.Sym.TypeArguments[0].GetMembers()
            .OfType<IFieldSymbol>()
            .Where(e => e.HasConstantValue)
            .ToArray();
         if (tags.Length != union.Sym.Arity) {
            ctx.ReportDiagnostic(Diagnostic.Create(BadTagEnumLength, tagged.Loc, tagged.Sym, union.Sym));
         }
      }
      return;
   }
}
