#!/bin/env dotnet
#:project _shared.csproj
#:property PublishAot=true
// feels like this is the least complicated way to generate this specific static boilerplate
// Dont really need to inspect the syntax tree for this so the internal generator project was a bit overkill imo.
using System.Runtime.CompilerServices;
using static UnionUtil.Internal.MetaConfiguration;
using System.Text;
using UnionUtil;

var outputDir = GetFilePath().ParentPath.ParentPath / "src/generated";
Directory.CreateDirectory(outputDir);

var sb = new StringBuilder(2048);
sb.AppendLine("""
   #nullable enable
   using System;
   using System.Runtime.CompilerServices;
   namespace UnionUtil;
   using static MethodImplOptions;
   """);
for (int i = 1; i <= ArityCount; ++i) {
   var Ts = string.Join(", ", Enumerable.Range(1, i).Select(e => $"T{e}"));
   sb.AppendLine($$"""
   public interface I{{TypeMarkerName}}<{{Ts}}>;
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public sealed class {{TypeMarkerName}}Attribute<{{Ts}}> : Attribute;
   public readonly struct {{FromIndexName}}{{i}} {
      public static readonly {{FromIndexName}}{{i}} Value = default;
   }
   """);
}
sb.AppendLine($$"""
   public static class SwitchExpressionCompatibilityExtensions {
      extension<TUnion>(TUnion u) where TUnion : {{nameof(IUnionType)}} {
   """);
for (var n = 1; n <= ArityCount; n++) {
   var typeParams = string.Join(", ", Enumerable.Range(1, n).Select(i => $"[CanHold(unique: true)] T{i}"));
   var funcParams = string.Join(", ", Enumerable.Range(1, n).Select(i => $"Func<T{i}, R> f{i}"));

   sb.AppendLine($$"""
         [MethodImpl(AggressiveInlining)]
         public R Switch<{{typeParams}}, R>({{funcParams}}, Func<R>? _ = null) {
   """);
   for (var i = 1; i <= n; ++i) sb.AppendLine($"""
            if (u.TryGetValue(out T{i} v{i})) return f{i}(v{i});
   """);
   sb.AppendLine("""
            if (_ is not null) return _();
            return ThrowHelpers.ThrowInvalidOperation<R>();
         }
   """);
}
sb.AppendLine("""
      }
   }
   """);
await File.WriteAllTextAsync(outputDir / "GenericOverloads.g.cs", sb.ToString());

Console.WriteLine("Done.");

static string GetFilePath([CallerFilePath] string filePath = default!) => filePath;
static class PathExtensions {
   extension(ReadOnlySpan<char> s) {
      public static string operator /(ReadOnlySpan<char> basePath, ReadOnlySpan<char> path) => Path.Join(basePath, path);
      public ReadOnlySpan<char> ParentPath => Path.GetDirectoryName(s);
   }
}

