#pragma warning disable CS8524
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
namespace UnionUtil.Meta;

using static UnionImplOptions;
using static SymbolDisplayFormat;

enum Strategy : byte { Box, Sequential, Overlap };
enum Kind : byte { Unmanaged, Open, Reference, Value, Interface };
readonly record struct StorageEntry(int TypeIndex, string TypeName, Kind Kind, Strategy Strategy);
readonly struct Storage(StorageEntry[] entries) : IEquatable<Storage> {
   public readonly StorageEntry[] Entries = entries;
   public bool Equals(Storage other) => Entries.AsSpan().SequenceEqual(other.Entries);
}
enum Sbo : byte { Disabled = default, Size = 1, Name = 2 }
readonly record struct SmallBufferOptimized(Sbo Tag, uint Size, string Name);
readonly record struct Tagged(string EnumTypeName, string PropertyName, string[] Names, bool IsDense) {
   public string? this[int typeIndex] => Names[(int)typeIndex - 1];
}
readonly record struct UnionImplResult(
   string? Namespace,
   string TypeName,
   string T,
   SyntaxKind Kind,
   UnionImplOptions Options,
   Storage StorageTypes,
   Tagged? Tagged,
   SmallBufferOptimized SmallBufferOptimized,
   Visibility FieldVisibility,
   bool Ok = true
);

[Generator(LanguageNames.CSharp)]
public sealed class UnionImplGenerator : IIncrementalGenerator {
   public void Initialize(IncrementalGeneratorInitializationContext ctx) {
      var provider = ctx.SyntaxProvider.ForAttributeWithMetadataName(
            $"{nameof(UnionUtil)}.{nameof(UnionImplAttribute)}",
            static (node, token) => node switch {
               ClassDeclarationSyntax { AttributeLists.Count: > 0 } => true,
               StructDeclarationSyntax { AttributeLists.Count: > 0 } => true,
               _ => false,
            }, Resolve).Where(static e => e.Ok is true);
      ctx.RegisterSourceOutput(provider, static (ctx, result) => {
         Debug.Assert(result.Ok is true);
         var (hintName, sb) = SourceResolvers.ResolveUnionImplSource(result);
         ctx.AddSource(hintName, sb.ToString());
      });
   }
   static UnionImplResult Resolve(GeneratorAttributeSyntaxContext ctx, CancellationToken token) {
      if (ctx.TargetSymbol is not INamedTypeSymbol symbol) return default;
      var node = (TypeDeclarationSyntax)ctx.TargetNode;
      if (!node.Modifiers.Any(SyntaxKind.PartialKeyword)) return default;
      var attr = ctx.Attributes[0];
      var symbolAttributes = symbol.GetAttributes();
      if (attr.ConstructorArguments.Length is 0) return default;
      var opts = (UnionImplOptions)attr.ConstructorArguments[0].Value!;
      var boxGenerics = opts.HasFlag(UnionImplOptions.BoxOpenGenerics);
      var boxStructs = opts.HasFlag(BoxManagedStructs);
      bool? mutable = opts.HasFlag(UnionImplOptions.EnableReadOnly) ? false : null;
      const string sboAttr = nameof(SmallBufferOptimizedAttribute);
      SmallBufferOptimized sbo = default;
      Debug.Assert(sbo.Tag is Sbo.Disabled);
      var sboType = symbolAttributes
         .Where(static e => e.AttributeClass?.MetadataName is $"{sboAttr}`1")
         .Select(static e => ((INamedTypeSymbol)e.AttributeClass!).TypeArguments[0])
         .FirstOrDefault();
      if (sboType is not null) {
         var sboTypeName = sboType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
         sbo = new SmallBufferOptimized(Sbo.Name, default, sboTypeName);
         goto sbo_done;
      }
      var sboSize = (uint?)symbol.GetAttributes()
         .Where(e => e.AttributeClass?.MetadataName is sboAttr)
         .SingleOrDefault()?.ConstructorArguments[0].Value ?? 0;
      if (sboSize is not 0) {
         sbo = new SmallBufferOptimized(Sbo.Size, sboSize, default!);
      }
   sbo_done:
      if (opts.Has(UnionImplOptions.EnableReadOnly)) goto skip_readonly;
      if (ctx.TargetNode is not StructDeclarationSyntax sds) goto skip_readonly; if (!sds.Modifiers.Any(SyntaxKind.ReadOnlyKeyword)) goto skip_readonly;
      opts |= UnionImplOptions.EnableReadOnly;
   skip_readonly:
      var (typeArgs, ok) = symbol.ResolveUnionTypeArgs();
      if (!ok || typeArgs.Length is 0) return default;
      var storageEntries = new StorageEntry[typeArgs.Length];
      for (int i = 0; i < typeArgs.Length; ++i) {
         int index = i + 1;
         var curr = typeArgs[i];
         Kind kind;
         if (curr is ITypeParameterSymbol tps) {
            kind = tps switch {
               { HasValueTypeConstraint: true } => Kind.Value,
               { HasReferenceTypeConstraint: true } => Kind.Reference,
               // a bit awkward. While legal for the compiler, overlapping fieldoffsets of generic
               // type arguments is not allowed by the runtime, even when constrained to unmanaged.
               // I may look into still allowing union storage for small primitivees eventually.
               { HasUnmanagedTypeConstraint: true } => Kind.Value,
               _ => Kind.Open,
            };
         } else {
            kind = curr switch {
               { TypeKind: TypeKind.Interface } => Kind.Interface,
               { IsAbstract: true } => Kind.Interface,
               { IsUnmanagedType: true } => Kind.Unmanaged,
               { IsValueType: true } => Kind.Value,
               { IsReferenceType: true } => Kind.Reference,
               _ => default,
            };
            if (symbol.IsDerivedFrom(curr)) kind = Kind.Interface;
         }
         var strategy = kind switch {
            Kind.Reference or Kind.Interface => Strategy.Box,
            Kind.Unmanaged => Strategy.Overlap,
            Kind.Value => boxStructs ? Strategy.Box : Strategy.Sequential,
            Kind.Open => boxGenerics ? Strategy.Box : Strategy.Sequential,
            _ => throw new InvalidOperationException(),
         };
         storageEntries[i] = new(index, curr.ToDisplayString(FullyQualifiedFormat), kind, strategy);
      }
      Tagged? resolvedTagged = default;
      var tagged = symbol.GetAttributes()
         .Where(e => e.AttributeClass?.MetadataName is $"{nameof(TaggedAttribute<>)}`1")
         .SingleOrDefault();
      if (tagged is null) goto skip_tagged;
      var tag = tagged.AttributeClass!.TypeArguments[0];
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
      if (names.Length < storageEntries.Length) return default;
      var tagName = (string)tagged.ConstructorArguments[0].Value!;
      var tagEnum = tag.ToDisplayString(FullyQualifiedFormat);
      resolvedTagged = new Tagged(tagEnum, tagName, names, isDense);
   skip_tagged:
      var ns = symbol.ContainingNamespace;
      return new UnionImplResult(
         Options: opts,
         TypeName: symbol.Name,
         T: symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
         Kind: ctx.TargetNode.Kind(),
         Namespace: ns.IsGlobalNamespace ? null : ns.ToDisplayString(),
         StorageTypes: new(storageEntries),
         Tagged: resolvedTagged,
         FieldVisibility: attr.NamedArguments
            .Where(static e => e.Key is nameof(UnionImplAttribute.FieldVisibility))
            .Select(static e => (Visibility)e.Value.Value!)
            .SingleOrDefault(),
         SmallBufferOptimized: sbo
      );
   }
}
