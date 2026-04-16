using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace UnionUtil;

[Generator(LanguageNames.CSharp)]
public sealed partial class UnionImpl : IIncrementalGenerator {
   public void Initialize(IncrementalGeneratorInitializationContext ctx) {
      var provider = ctx.SyntaxProvider.ForAttributeWithMetadataName("UnionUtil.UnionImplAttribute",
               static (node, token) => node switch {
                  ClassDeclarationSyntax { AttributeLists.Count: > 0 } => true,
                  StructDeclarationSyntax { AttributeLists.Count: > 0 } => true,
                  _ => false,
               }, Resolve).Where(static t => t != default);
      ctx.RegisterSourceOutput(provider, GenerateSource);
   }
   enum Strategy { Box, Sequential, Overlap };
   enum Kind : byte { Unmanaged, Generic, Reference, Value, Interface };
   sealed record Resolved(
      Types Types,
      Name Name,
      SyntaxKind Kind,
      bool Mutable,
      string Visibility,
      bool Nullable,
      bool WithSetValue,
      Labels Labels
   );
   readonly struct Type(int index, string name, Kind kind, Strategy strategy) : IEquatable<Type> {
      public readonly int index = index;
      public readonly string type = name;
      public readonly Kind kind = kind;
      public readonly Strategy strategy = strategy;
      public bool Equals(Type other) => type == other.type && strategy == other.strategy;
   }
   readonly record struct Labels(string[]? Cases, bool Enabled) {
      public string this[int typeIndex] => Cases?[typeIndex - 1] ?? $"Case{typeIndex}";
      public string this[in Type arg] => this[arg.index];
   }
   readonly struct Types(Type[] entries) : IEquatable<Types> {
      public readonly Type[] entries = entries;
      public bool Equals(Types other) => entries.AsSpan().SequenceEqual(other.entries);
   }

   readonly record struct Name(string? Namespace, string Type, string[] Params) {
      public string HintName() {
         var sb = new StringBuilder();
         if (Namespace != null) {
            sb.Append(Namespace).Append('.');
         }
         sb.Append(Type);
         if (Params.Length > 0) {
            sb.Append('{').Append((string.Join(",", Params))).Append('}');
         }
         sb.Append(".g");
         return sb.ToString();
      }
      public string GenericName() {
         if (Params.Length == 0) return Type;
         var sb = new StringBuilder();
         sb.Append(Type);
         sb.Append('<');
         foreach (var e in Params) {
            sb.Append(e).Append(',');
         }
         sb[sb.Length - 1] = '>';
         return sb.ToString();
      }
   }
   static (Resolved, Problem[]) Resolve(GeneratorAttributeSyntaxContext ctx, CancellationToken token) {
      if (ctx.TargetSymbol is not INamedTypeSymbol symbol) return default;
      Problem[] problems = [];
      void emitProblem(string message, Location? location = null, DiagnosticSeverity severity = DiagnosticSeverity.Error) {
         problems = [.. problems, new(location ?? symbol.Locations.First(), message, severity)];
      }
      var attr = ctx.Attributes[0];
      var boxGenerics = attr.Named<bool?>("BoxOpenGenerics") ?? false;
      var boxStructs = attr.Named<bool?>("BoxManagedStructs") ?? false;
      var mutable = !attr.Named<bool?>("ReadOnly");
      if (mutable.HasValue) goto mutable_done;
      switch (ctx.TargetNode) {
         case StructDeclarationSyntax x:
            if (x.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)) {
               mutable = false;
               goto mutable_done;
            }
            break;
         case ClassDeclarationSyntax x:
            if (x.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)) {
               mutable = false;
               goto mutable_done;
            }
            break;
      }
      mutable = true;
   mutable_done:
      const string unionTypesName = "UnionCases";
      var unionTypes = symbol.Interfaces
         .Where(e => e.Name is $"I{unionTypesName}")
         .FirstOrDefault();
      Type[] resolvedTypes;
      if (unionTypes is not null) goto resolve_union_types;
      unionTypes = symbol.GetAttributes()
         .Where(e => e.AttributeClass?.Name is $"{unionTypesName}Attribute")
         .Select(e => e.AttributeClass)
         .FirstOrDefault();
      if (unionTypes is null) {
         emitProblem($"Missing '{unionTypesName} interface to tag types");
         resolvedTypes = [];
         goto skip_resolving_union_types;
      }
   resolve_union_types:
      var targs = unionTypes.TypeArguments;
      resolvedTypes = new Type[targs.Length];
      for (int i = 0; i < targs.Length; ++i) {
         int index = i + 1;
         var curr = targs[i];
         Kind kind;
         string name;
         if (curr is ITypeParameterSymbol tps) {
            kind = tps switch {
               { HasValueTypeConstraint: true } => Kind.Value,
               { HasReferenceTypeConstraint: true } => Kind.Reference,
               // a bit awkward. While legal for the compiler, overlapping fieldoffsets of generic
               // type arguments is not allowed by the runtime, even when constrained to unmanaged.
               // I may look into still allowing union storage for small primitivees eventually.
               { HasUnmanagedTypeConstraint: true } => Kind.Value,
               _ => Kind.Generic,
            };
            name = tps.Name;
         } else {
            kind = curr switch {
               { TypeKind: TypeKind.Interface } => Kind.Interface,
               { IsAbstract: true } => Kind.Interface,
               { IsUnmanagedType: true } => Kind.Unmanaged,
               { IsValueType: true } => Kind.Value,
               { IsReferenceType: true } => Kind.Reference,
               _ => default,
            };
            // to resolve base
            if (curr is INamedTypeSymbol { IsReferenceType: true } named) {
               INamedTypeSymbol? @base = symbol;
               while ((@base = @base.BaseType) != null) {
                  if (SymbolEqualityComparer.Default.Equals(@base, named)) {
                     kind = Kind.Interface;
                     break;
                  }
               }
            }
            name = curr.ToDisplayString(Format);
         }
         var strategy = kind switch {
            Kind.Reference or Kind.Interface => Strategy.Box,
            Kind.Unmanaged => Strategy.Overlap,
            Kind.Value => boxStructs ? Strategy.Box : Strategy.Sequential,
            Kind.Generic => boxGenerics ? Strategy.Box : Strategy.Sequential,
            _ => throw new InvalidOperationException(),
         };
         resolvedTypes[i] = new(index, name, kind, strategy);
      }
   skip_resolving_union_types:
      Labels labels = default;
      if (attr.Named<bool?>("WithFieldAccessors") is not true) {
         goto labels_done;
      }
      const string accessorNames = "FieldAccessorNames";
      var (names, err) = attr.NamedArray<string>(accessorNames);
      if (err != null) {
         problems = [.. problems, err];
      } else if (names is { } c && c.Length != resolvedTypes?.Length) {
         emitProblem($"length of {accessorNames} does not match the arity of {unionTypesName}");
      } else {
         labels = new(names, true);
      }
   labels_done:
      var ns = symbol.ContainingNamespace;
      Debug.Assert(mutable.HasValue);
      return (new Resolved(
         Types: new(entries: resolvedTypes ?? throw new("resolvedTypes is null")),
         Name: new(
            Namespace: ns.IsGlobalNamespace ? null : ns.ToDisplayString(),
            Type: symbol.Name,
            Params: [.. symbol.TypeParameters.Select(e => e.Name)]
         ),
         Kind: ctx.TargetNode.Kind(),
         WithSetValue: attr.Named<bool?>("WithSetValueOverloads") ?? false,
         Mutable: mutable!.Value,
         Nullable: attr.Named<bool?>("Nullable") ?? false,
         Labels: labels,
         Visibility: attr.Named<int?>("FieldVisibility") switch {
            1 => "internal",
            2 => "public",
            _ => "private",
         }
      ), problems);
   }
   static SymbolDisplayFormat Format =>
      SymbolDisplayFormat.FullyQualifiedFormat
      .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included);

   static readonly DiagnosticDescriptor Descriptor = new(
      id: "UnionUtil",
      title: "UnionUtil error",
      messageFormat: "{0}",
      category: "Usage", defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true
   );
}
