#pragma warning disable CS9113
using UnionUtil;
using static UnionUtil.Meta.Configuration;

[assembly: UnionTypesConfig(UnionType.Arity, nameof(UnionUtil), UnionType.Name)]

[AttributeUsage(AttributeTargets.Assembly)]
sealed class UnionTypesConfigAttribute(int arity, string? @namespace, string name) : Attribute;
