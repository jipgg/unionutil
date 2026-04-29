namespace UnionUtil;

static class CanHoldTypesConfig {
   public const int End = 16;
}
[VariadicRange(2, CanHoldTypesConfig.End, ExpansionType.Arity)]
public interface ICanHoldTypes<T1>;
[VariadicRange(2, CanHoldTypesConfig.End,
   ExpansionType.Arity)]
[AttributeUsage(
   AttributeTargets.Class |
   AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1> : Attribute;

// [VariadicRange(2, CanHoldTypesConfig.End,
//    ExpansionType.Name)]
// public interface IHasTypeCount1;

[VariadicRange(2, CanHoldTypesConfig.End,
   ExpansionType.Name)]
public readonly struct FromIndex1;
