using System.Runtime.CompilerServices;
namespace UnionUtil.Meta;

[Generator]
public sealed class Generator : IIncrementalGenerator {
   const string MetadataName = "UnionUtil.UnionImplAttribute";
   public void Initialize(IncrementalGeneratorInitializationContext ctx) {
      var provider = ctx.SyntaxProvider
         .ForAttributeWithMetadataName(MetadataName, Predicate, Transform)
         .Where(static t => t != default);
      ctx.RegisterSourceOutput(provider, Register);
   }
   static bool Predicate(SyntaxNode node, CancellationToken token) {
      return node switch {
         ClassDeclarationSyntax { AttributeLists.Count: > 0 } => true,
         StructDeclarationSyntax { AttributeLists.Count: > 0 } => true,
         _ => false,
      };
   }
   static SymbolDisplayFormat Format =>
      SymbolDisplayFormat.FullyQualifiedFormat
      .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Included);

   static readonly DiagnosticDescriptor Desc = new(
      id: "UnionUtil",
      title: "UnionUtil error",
      messageFormat: "{0}",
      category: "Usage", defaultSeverity: DiagnosticSeverity.Error,
      isEnabledByDefault: true
   );
   static (Ok? ok, Problem[] err) Transform(GeneratorAttributeSyntaxContext ctx, CancellationToken token) {
      if (ctx.TargetSymbol is not INamedTypeSymbol symbol) {
         return default;
      }
      var attr = ctx.Attributes[0];
      T? getNamed<T>(string name) {
         var x = attr.NamedArguments.Where(e => e.Key == name).ToArray();
         if (x.Length is 0) return default;
         return ((T?)x[0].Value.Value);
      }
      var boxGenerics = getNamed<bool?>("BoxGenerics") ?? false;
      var boxStructs = getNamed<bool?>("BoxManagedStructs") ?? false;
      var compilation = ctx.SemanticModel.Compilation;
      var inter = symbol.Interfaces.Where(e => e.Name is "UnionTypes").FirstOrDefault();
      if (inter is null) {
         return (null, [new(symbol.Locations.First(), "must tag types by implementing Discriminate.Types")]);
      }
      var args = new Arg[inter.TypeArguments.Length];
      for (int i = 0; i < inter.TypeArguments.Length; ++i) {
         int index = i + 1;
         var x = inter.TypeArguments[i];
         Kind kind;
         string name;
         if (x is ITypeParameterSymbol tps) {
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
            kind = x switch {
               { TypeKind: TypeKind.Interface } => Kind.Interface,
               { IsAbstract: true } => Kind.Interface,
               { IsUnmanagedType: true } => Kind.Unmanaged,
               { IsValueType: true } => Kind.Value,
               { IsReferenceType: true } => Kind.Reference,
               _ => default,
            };
            if (x is INamedTypeSymbol {IsReferenceType: true} named) {
               INamedTypeSymbol? @base = symbol;
               while ((@base = @base.BaseType) != null) {
                  if (SymbolEqualityComparer.Default.Equals(@base, named)) {
                     kind = Kind.Interface;
                     break;
                  }
               }
            }
            name = x.ToDisplayString(Format);
         }
         var strategy = kind switch {
            Kind.Reference or Kind.Interface => Strategy.Object,
            Kind.Unmanaged => Strategy.Overlap,
            Kind.Value => boxStructs ? Strategy.Object : Strategy.Sequential,
            Kind.Generic => boxGenerics ? Strategy.Object : Strategy.Sequential,
            _ => throw new InvalidOperationException(),
         };
         args[i] = new(index, name, kind, strategy);
      }
      var ns = symbol.ContainingNamespace;
      var transformed = new Ok(
         TypeArgs: new(args: args),
         Name: new(
            Namespace: ns.IsGlobalNamespace ? null : ns.ToDisplayString(),
            Type: symbol.Name,
            Params: [.. symbol.TypeParameters.Select(e => e.Name)]
         ),
         Kind: ctx.TargetNode.Kind(),
         Mutable: getNamed<bool?>("Mutable") ?? false,
         Nullable: getNamed<bool?>("Nullable") ?? false,
         Visibility: getNamed<int?>("FieldVisibility") switch {
            1 => "internal",
            2 => "public",
            _ => "private",
         }
      );
      return (transformed, []);
   }

   static void Register(SourceProductionContext ctx, (Ok? ok, Problem[] err) res) {
      const string compilerServices = "global::System.Runtime.CompilerServices";
      const string aggressiveInlining = $"{compilerServices}.MethodImpl({compilerServices}.MethodImplOptions.AggressiveInlining)";
      var (ok, err) = res;
      if (err.Length > 0) {
         foreach (var e in err) {
            var diag = Diagnostic.Create(Desc, e.Location, e.Message);
            ctx.ReportDiagnostic(diag);
         }
         return;
      }
      var sb = new StringBuilder(1024);
      sb.AppendLine("#nullable enable");
      if (ok!.Name.Namespace is string ns) {
         sb.AppendLine($"namespace {ns};");
      }
      sb.Append("partial ");
      string ro;
      switch (ok.Kind) {
         case SyntaxKind.StructDeclaration:
            sb.Append("struct ");
            ro = " readonly";
            break;
         case SyntaxKind.ClassDeclaration:
            sb.Append("class ");
            ro = " ";
            break;
         default:
            throw new InvalidOperationException();
      }
      sb.Append(ok.Name.GenericName()).AppendLine("{");
      var args = ok.TypeArgs.args;
      var vis = ok.Visibility;
      var mut = ok.Mutable ? " " : " readonly";
      const string objectField = "_object";
      const string unmanagedField = "_overlapped";
      const string unmanagedType = "Overlapped";
      const string indexField = "_index";
      (int, string)[] getters = [];
      string value(in Arg arg) => $"_{arg.index}";
      void writeCtor(in Arg arg, string assign) {
         sb.AppendLine($"  [{aggressiveInlining}]");
         sb.AppendLine($"  public {ok.Name.Type}({arg.type} v) {{");
         sb.AppendLine($"    {assign}");
         sb.AppendLine($"    {indexField} = {arg.index};");
         sb.AppendLine("  }");
         if (arg.kind is not Kind.Interface) {
            sb.AppendLine($"  public static implicit operator {ok.Name.GenericName()}({arg.type} v) => new(v);");
         }
      }
      void writeTryGetValue(Arg arg, string assign) {
         sb.AppendLine($"  public{ro} bool TryGetValue(out {arg.type} v) {{");
         sb.AppendLine($"    if ({indexField} == {arg.index}) {{");
         sb.AppendLine($"      {assign}");
         sb.AppendLine($"      return true;");
         sb.AppendLine($"    }} else {{");
         sb.AppendLine($"      v = default!;");
         sb.AppendLine($"      return false;");
         sb.AppendLine("    }");
         sb.AppendLine("  }");
      }
      void writeSetValue(Arg arg, string assign) {
         if (!ok.Mutable) return;
         sb.AppendLine($"  public void SetValue({arg.type} v) {{");
         sb.AppendLine($"    {assign}");
         sb.AppendLine($"    {indexField} = {arg.index};");
         sb.AppendLine("  }");
      }
      Arg[] overlap = [.. args.Where(static e => e.strategy == Strategy.Overlap)];
      if (overlap.Length > 0) {
         const string interop = "global::System.Runtime.InteropServices";
         sb.AppendLine($"  [{interop}.StructLayout({interop}.LayoutKind.Explicit)]");
         sb.AppendLine($"  {vis} struct {unmanagedType}{{");
         var fieldVis = vis is "private" ? "internal" : vis;
         foreach (var e in overlap) {
            sb.AppendLine($"    [{interop}.FieldOffset(0)]");
            sb.AppendLine($"    {fieldVis} {e.type} {value(e)};");
         }
         sb.AppendLine("  }");
         sb.AppendLine($"  {vis}{mut} {unmanagedType} {unmanagedField} = default;");
         foreach (var e in overlap) {
            getters = [.. getters, (e.index, $"{unmanagedField}.{value(e)}")];
            writeCtor(e, $"{unmanagedField} = new() {{{value(e)} = v}};");
            writeTryGetValue(e, $"v = {unmanagedField}.{value(e)};");
            writeSetValue(e, $"{unmanagedField}.{value(e)} = v;");
         }
      }
      Arg[] @object = [.. args.Where(static e => e.strategy is Strategy.Object)];
      if (@object.Length > 0) {
         const string @unsafe = $"{compilerServices}.Unsafe";
         sb.AppendLine($"  {vis}{mut} object? {objectField} = default;");
         foreach (var e in @object) {
            writeCtor(e, $"{objectField} = v;");
            var get = e.kind switch {
               Kind.Reference => $"{@unsafe}.As<{e.type}>({objectField}!)",
               Kind.Value => $"{@unsafe}.Unbox<{e.type}>({objectField}!)",
               _ => $"({e.type}){objectField}!",
            };
            getters = [.. getters, (e.index, objectField)];
            writeTryGetValue(e, $"v = {get};");
            writeSetValue(e, $"{objectField} = v;");
         }
      }
      Arg[] sequential = [.. args.Where(static e => e.strategy is Strategy.Sequential)];
      if (sequential.Length > 0) {
         foreach (var e in sequential) {
            var holder = value(e);
            sb.AppendLine($"  {vis}{mut} {e.type} {holder} = default!;");
            getters = [.. getters, (e.index, holder)];
            writeCtor(e, $"{holder} = v;");
            writeTryGetValue(e, $"v = {holder};");
            writeSetValue(e, $"{holder} = v;");
         }
      }
      sb.AppendLine($"  {vis}{mut} byte {indexField};");
      var obj = ok.Nullable ? "object?" : "object";
      sb.AppendLine($"  public object? Value => {indexField} switch {{");
      foreach (var e in getters) sb.AppendLine($"    {e.Item1} => {e.Item2},");
      if (ok.Nullable) sb.AppendLine($"    _ => null,");
      else sb.AppendLine($"    _ => throw new InvalidOperationException($\"type index was {{{indexField}}}\")");
      sb.AppendLine("  };");
      if (ok.Nullable) {
         sb.AppendLine($"  public bool HasValue => {indexField} != 0;");
      }
      sb.Append('}');
      ctx.AddSource(ok.Name.HintName(), sb.ToString());
   }

}
sealed record Problem(Location Location, string Message);
sealed record Ok(Args TypeArgs, Name Name, SyntaxKind Kind, bool Mutable, string Visibility, bool Nullable);
enum Strategy { Object, Sequential, Overlap };
enum Kind : byte { Unmanaged, Generic, Reference, Value, Interface };
readonly struct Arg(int index, string name, Kind kind, Strategy strategy) : IEquatable<Arg> {
   public readonly int index = index;
   public readonly string type = name;
   public readonly Kind kind = kind;
   public readonly Strategy strategy = strategy;
   public bool Equals(Arg other) => type == other.type && strategy == other.strategy;
}
readonly struct Args(Arg[] args) : IEquatable<Args> {
   public readonly Arg[] args = args;
   public bool Equals(Args other) => args.AsSpan().SequenceEqual(other.args);
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
      var sb = new StringBuilder();
      sb.Append(Type).AppendGenerics(this);
      return sb.ToString();
   }
}
static class Extensions {
   public static StringBuilder AppendGenerics(this StringBuilder sb, Name name) {
      if (name.Params.Length == 0) return sb;
      sb.Append('<');
      foreach (var e in name.Params) {
         sb.Append(e).Append(',');
      }
      sb[sb.Length - 1] = '>';
      return sb;
   }
   public static StringBuilder AppendNamespacePrefix(this StringBuilder sb, Name name) {
      sb.Append("global::");
      if (name.Namespace != null) {
         sb.Append(name.Namespace).Append('.');
      }
      return sb;
   }
}

