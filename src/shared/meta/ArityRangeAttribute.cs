#pragma warning disable IDE1006,CS9113
namespace UnionUtil;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
sealed class ArityRangeAttribute(int start, int count): Attribute;
