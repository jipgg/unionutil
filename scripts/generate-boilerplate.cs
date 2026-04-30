#!/bin/env dotnet
using System.Runtime.CompilerServices;
using System.Text;

const int N = 16;
var outputDir = GetFilePath().ParentPath.ParentPath / "src/shared/generated";
Directory.CreateDirectory(outputDir);

var sb = new StringBuilder(2048);
sb.AppendLine("""
   #nullable enable
   using System;
   using System.Runtime.CompilerServices;
   namespace UnionUtil;
   using static MethodImplOptions;
   """);
for (int i = 1; i <= N; ++i) {
   var Ts = string.Join(", ", Enumerable.Range(1, i).Select(e => $"T{e}"));
   sb.AppendLine($$"""
   public interface ICanHoldTypes<{{Ts}}>;
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
   public sealed class CanHoldTypesAttribute<{{Ts}}> : Attribute;
   public readonly struct FromIndex{{i}} {
      public static readonly FromIndex{{i}} Value = default;
   }
   """);
}
sb.AppendLine("""
   public static class SwitchExpressionCompatibilityExtensions {
      extension<TUnion>(TUnion u) where TUnion : IUnionType {
   """);
for (var n = 1; n <= N; n++) {
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
File.WriteAllText(outputDir / "generated-boilerplate.g.cs", sb.ToString());

Console.WriteLine("Done.");

static string GetFilePath([CallerFilePath] string filePath = default!) {
   return filePath;
}
static class PathExtensions {
   extension(ReadOnlySpan<char> s) {
      public static string operator /(ReadOnlySpan<char> basePath, ReadOnlySpan<char> path) {
         return Path.Join(basePath, path);
      }
      public ReadOnlySpan<char> ParentPath => Path.GetDirectoryName(s);
   }
}
