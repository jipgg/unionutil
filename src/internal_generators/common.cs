using System.Collections.Immutable;
using System.Runtime.CompilerServices;
namespace UnionUtil;

sealed record Problem(
   Location Location,
   string Message,
   DiagnosticSeverity Severity = DiagnosticSeverity.Error
);

static class Extensions {
   public static T? Named<T>(this AttributeData attr, string name) {
      var x = attr.NamedArguments.Where(e => e.Key == name).ToArray();
      if (x.Length is 0) return default;
      return ((T?)x[0].Value.Value);
   }
   public static (T[]?, Problem?) NamedArray<T>(this AttributeData attr, string name) {
      var x = attr.NamedArguments.Where(e => e.Key == name).ToArray();
      if (x.Length is 0) return default;
      var results = new T[x[0].Value.Values.Length];
      for (int i = 0; i < results.Length; ++i) {
         var casted = (T?)x[0].Value.Values[i].Value;
         if (casted is not T ok) {
            return (null, new(
               attr.AttributeClass!.Locations.First(),
               $"bad named values in '{name}'"
            ));
         }
         results[i] = ok;
      }
      return (results, null);
   }

}
