#pragma warning disable IDE1006,CS9113
namespace UnionUtil;

enum ExpansionType { Arity, Name };
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
sealed class VariadicRangeAttribute(int start, int count, ExpansionType expansionType) : Attribute;
