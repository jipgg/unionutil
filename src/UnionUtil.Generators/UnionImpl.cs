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
   readonly record struct Sbo(uint Size);
   sealed record Resolved(
      TypeArgs TypeArgs,
      Name Name,
      SyntaxKind Kind,
      bool Mutable,
      string Visibility,
      bool Nullable,
      Tagged? Tagged,
      Sbo? Sbo
   );
   readonly struct TypeArg(int index, string name, Kind kind, Strategy strategy) : IEquatable<TypeArg> {
      public readonly int index = index;
      public readonly string type = name;
      public readonly Kind kind = kind;
      public readonly Strategy strategy = strategy;
      public bool Equals(TypeArg other) => type == other.type && strategy == other.strategy;
   }
   readonly record struct Tagged(string? Enum, string? Name, string[]? Cases, bool Dense) {
      public string this[int typeIndex] => Cases?[typeIndex - 1] ?? $"Case{typeIndex}";
      public string this[in TypeArg arg] => this[arg.index];
   }
   readonly struct TypeArgs(TypeArg[] entries) : IEquatable<TypeArgs> {
      public readonly TypeArg[] entries = entries;
      public bool Equals(TypeArgs other) => entries.AsSpan().SequenceEqual(other.entries);
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
      var sboSize = (uint?)symbol.GetAttributes()
         .Where(e => e.AttributeClass?.Name is "SmallBufferOptimizedAttribute")
         .SingleOrDefault()?.ConstructorArguments[0].Value ?? 0;
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
      const string unionTypesName = "Union";
      var unionTypes = symbol.Interfaces
         .Where(e => e.Name is $"I{unionTypesName}")
         .FirstOrDefault();
      TypeArg[] resolvedTypeArgs;
      if (unionTypes is not null) goto resolve_union_types;
      unionTypes = symbol.GetAttributes()
         .Where(e => e.AttributeClass?.Name is $"{unionTypesName}Attribute")
         .Select(e => e.AttributeClass)
         .FirstOrDefault();
      if (unionTypes is null) {
         emitProblem($"Missing '{unionTypesName} interface to tag types");
         resolvedTypeArgs = [];
         goto skip_resolving_union_types;
      }
   resolve_union_types:
      var targs = unionTypes.TypeArguments;
      resolvedTypeArgs = new TypeArg[targs.Length];
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
         resolvedTypeArgs[i] = new(index, name, kind, strategy);
      }
   skip_resolving_union_types:
      Tagged? resolvedTagged = default;
      var tagged = symbol.GetAttributes()
         .Where(e => e.AttributeClass?.MetadataName is "TaggedAttribute`1")
         .SingleOrDefault();
      if (tagged is null) goto taggeds_done;
      var tag = tagged.AttributeClass!.TypeArguments[0];
      // var members = tag.GetMembers()
      //    .Where(static e => e.Kind == SymbolKind.Field);
      var members = tag.GetMembers()
         .OfType<IFieldSymbol>()
         .Where(static e => e.HasConstantValue)
         .ToArray();
      bool isDense = true;
      for (long i = 0; i < members.Length; ++i) {
         if (Convert.ToInt64(members[i].ConstantValue) != i) {
            isDense = false;
            break;
         }
      }
      string[] names = [.. members.Select(static e => e.Name)];
      if (names.Length != resolvedTypeArgs?.Length) {
         emitProblem($"length of tag enum is not the same as length of cases");
         goto taggeds_done;
      }
      var tagName = (string)tagged.ConstructorArguments[0].Value!;
      var tagEnum = tag.ToDisplayString(Format);
      resolvedTagged = new(tagEnum, tagName, names, isDense);
   taggeds_done:
      Debug.Assert(mutable.HasValue);
      var ns = symbol.ContainingNamespace;
      return (new Resolved(
         TypeArgs: new(entries: resolvedTypeArgs ?? throw new("resolvedTypes is null")),
         Name: new(
            Namespace: ns.IsGlobalNamespace ? null : ns.ToDisplayString(),
            Type: symbol.Name,
            Params: [.. symbol.TypeParameters.Select(e => e.Name)]
         ),
         Kind: ctx.TargetNode.Kind(),
         Mutable: mutable.Value,
         Nullable: attr.Named<bool?>("Nullable") ?? false,
         Tagged: resolvedTagged,
         Visibility: attr.Named<int?>("FieldVisibility") switch {
            1 => "internal",
            2 => "public",
            _ => "private",
         },
         Sbo: sboSize is not 0 ? new(sboSize) : null
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
