namespace UnionUtil.Meta;

public static class Config {
   public const string AdapterName = "IUnionAdapter";
   public static class UnionType {
      public const int Arity = 16;
      public const string InterfaceName = "IUnion";
      public const string AttributeName = "UnionAttribute";
      public const string Namespace = nameof(UnionUtil);
   }
}
