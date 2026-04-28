namespace UnionUtil.Internal.Meta;

[Generator]
public sealed class ArityRangeGenerator : IIncrementalGenerator {
   public void Initialize(IncrementalGeneratorInitializationContext context) {
      var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
         $"{nameof(UnionUtil)}.{nameof(ArityRangeAttribute)}",
         static (node, token) => node is TypeDeclarationSyntax { AttributeLists.Count: > 0 }
            and not RecordDeclarationSyntax or DelegateDeclarationSyntax,
         static (ctx, cancel) => {
            var node = (TypeDeclarationSyntax)ctx.TargetNode;
            var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
            var args = ctx.Attributes[0].ConstructorArguments;

            var span = TextSpan.FromBounds(node.SpanStart, node.Keyword.Span.End);
            var sourceText = ctx.SemanticModel.SyntaxTree.GetText(cancel);
            var modifiers = sourceText.ToString(span);
            var baseLists = node.BaseList is null ? null : sourceText.ToString(node.BaseList.Span);
            return (
               name: symbol.Name,
               start: (int)args[0].Value!,
               end: (int)args[1].Value!,
               modifiers,
               baseLists
            );
         });
      context.RegisterSourceOutput(provider, static (ctx, r) => {
         var source = new StringBuilder(10 * (r.end - r.start));
         source.AppendLine("using System;");
         source.AppendLine($"namespace {nameof(UnionUtil)};");
         for (int n = r.start; n <= r.end; ++n) {
            var typeParams = string.Join(", ", Enumerable.Range(0, n).Select(static i => $"T{i + 1}"));
            source.Append($"{r.modifiers} {r.name}<{typeParams}>");
            if (r.baseLists is not null) {
               source.Append(r.baseLists);
            }
            source.AppendLine(";");
         }
         ctx.AddSource($"{r.name}{{{r.start},{r.end}}}.g", source.ToString());
      });
   }
}
