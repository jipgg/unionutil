using System.Runtime.CompilerServices;
using System.Buffers;
namespace UnionUtil.Meta.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeAnalyzer : DiagnosticAnalyzer {

   static string MakeId(string name) => $"{nameof(UnionUtil)}_{name}";

   static DiagnosticDescriptor MissingUnionImpl => new(
      MakeId(nameof(MissingUnionImpl)),
      "missing UnionImpl marker",
      "'{0}' does nothing without marking with 'UnionUtil.UnionImplAttribute'",
      "Usage",
      DiagnosticSeverity.Warning,
      true
   );
   static DiagnosticDescriptor TypesAlreadyMarked => new(
      MakeId(nameof(TypesAlreadyMarked)),
      "types already marked",
      "types already marked by '{0}'",
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

   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [
      MissingUnionImpl,
      TypesAlreadyMarked,
      MissingTypesMarker,
      BadTagEnumLength,
      MissingPartial,
   ];

   public override void Initialize(AnalysisContext ctx) {
      ctx.EnableConcurrentExecution();
      ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      ctx.RegisterSyntaxNodeAction(AnalyzeDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.ClassDeclaration);
   }
   static bool IsUnionUtil(ITypeSymbol? symbol) {
      if (symbol is null) return false;
      return symbol.ContainingNamespace.ContainingNamespace.IsGlobalNamespace
         && symbol.ContainingNamespace.MetadataName == "UnionUtil";
   }
   static bool IsUnionUtil(AttributeData? attributeData) => IsUnionUtil(attributeData?.AttributeClass);
   readonly record struct SymbolData(INamedTypeSymbol Sym, Location Loc);
   static void AnalyzeDeclaration(SyntaxNodeAnalysisContext ctx) {
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
      bool assignUnionChecked(INamedTypeSymbol symbol, Location loc) {
         if (unionSymbol is SymbolData sd) {
            var d = Diagnostic.Create(TypesAlreadyMarked, loc, sd.Sym.ToDisplayString());
            ctx.ReportDiagnostic(d);
            return false;
         }
         unionSymbol = new(symbol, loc);
         return true;
      }
      foreach (var e in interfaces) {
         if (e.Name is not "IUnion") continue;
         var loc = e.DeclaringSyntaxReferences.FirstOrDefault()?
            .GetSyntax(ctx.CancellationToken).GetLocation() ?? symbol.Locations.First();
         assignUnionChecked(e, loc);
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
               assignUnionChecked(e.AttributeClass, loc);
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
         ctx.ReportDiagnostic(Diagnostic.Create(MissingPartial, node.GetLocation()));
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
