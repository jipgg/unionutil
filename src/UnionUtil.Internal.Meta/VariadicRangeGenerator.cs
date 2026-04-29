using System.Diagnostics;
namespace UnionUtil.Internal.Meta;

[Generator]
public sealed class VariadicRangeGenerator : IIncrementalGenerator {
   public void Initialize(IncrementalGeneratorInitializationContext context) {
      var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
         $"{nameof(UnionUtil)}.{nameof(VariadicRangeAttribute)}",
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
               expansionType: (ExpansionType)args[2].Value!,
               modifiers,
               baseLists
            );
         });
      context.RegisterSourceOutput(provider, static (ctx, r) => {
         var source = new StringBuilder(10 * (r.end - r.start));
         source.AppendLine("using System;");
         source.AppendLine($"namespace {nameof(UnionUtil)};");
         for (int i = r.start; i <= r.end; ++i) {
            source.Append($"{r.modifiers} {r.name}");
            switch (r.expansionType) {
               case ExpansionType.Arity:
                  var typeParams = string.Join(", ", Enumerable
                        .Range(0, i)
                        .Select(static e => $"T{e + 1}"));
                  source.Append($"<{typeParams}>");
                  break;
               case ExpansionType.Name:
                  while (source[source.Length - 1] is >= '0' and <= '9') {
                     --source.Length;
                  }
                  source.Append(i);
                  break;
               default: throw new();
            }
            if (r.baseLists is not null) source.Append(r.baseLists);
            source.AppendLine(";");
         }
         ctx.AddSource($"{r.name}{{{r.start},{r.end},{r.expansionType}}}.g", source.ToString());
      });
   }
}
