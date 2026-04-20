namespace UnionUtil.Meta;

static class Diagnostics {
   static string MakeId(string name) => $"{nameof(UnionUtil)}_{name}";
   const string prefix = "UnionUtil_";

   public static DiagnosticDescriptor MissingUnionImpl => new(
      $"{nameof(MissingUnionImpl)}",
      "missing UnionImpl marker",
      "'{0}' does nothing without marking with 'UnionUtil.UnionImplAttribute'",
      "UnionUtil",
      DiagnosticSeverity.Warning,
      true
   );
   public static DiagnosticDescriptor MissingTypesMarker => new(
      $"{nameof(MissingTypesMarker)}",
      "missing types marker",
      "types must be marked with 'IUnion<...T>' or 'UnionAttribute<...T>'",
      "UnionUtil",
      DiagnosticSeverity.Error,
      true
   );
   public static DiagnosticDescriptor BadTagEnumLength => new(
      $"{nameof(BadTagEnumLength)}",
      "bad tag enum length",
      "length of '{0}' does not match type count of '{1}'",
      "UnionUtil",
      DiagnosticSeverity.Error,
      true
   );
   public static DiagnosticDescriptor MissingPartialKeyword => new(
      $"{nameof(MissingPartialKeyword)}",
      "missing partial keyword",
      "type is missing partial specifier",
      "UnionUtil",
      DiagnosticSeverity.Warning,
      true
   );
   public static DiagnosticDescriptor WillNeverHoldType => new(
      $"{nameof(WillNeverHoldType)}",
      "will never hold type",
      "will never hold type '{0}'",
      "UnionUtil",
      DiagnosticSeverity.Warning,
      true
   );

}
