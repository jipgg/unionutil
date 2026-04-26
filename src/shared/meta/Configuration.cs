namespace UnionUtil.Meta;

public static class Config {
   public static class UnionType {
      public const int Arity = 16;
      public const string HasAtleastTypeCountName = "IHasAtleastTypeCount";
      public const string HasTypeCountName = "IHasTypeCount";
      public const string InterfaceName = "ICanHoldTypes";
      public const string AttributeName = "CanHoldTypesAttribute";
      public const string Namespace = nameof(UnionUtil);
   }
}
