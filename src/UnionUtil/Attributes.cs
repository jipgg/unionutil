namespace UnionUtil;

static class CanHoldTypesConfiguration {
   public const int Start = 2;
   public const int End = 16;
}
[ArityRange(CanHoldTypesConfiguration.Start, CanHoldTypesConfiguration.End)]
public interface ICanHoldTypes<T1>;
[ArityRange(CanHoldTypesConfiguration.Start, CanHoldTypesConfiguration.End)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class CanHoldTypesAttribute<T1> : Attribute;
