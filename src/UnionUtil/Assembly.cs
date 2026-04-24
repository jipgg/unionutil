#pragma warning disable CS9113
using UnionUtil;

[assembly: GenerateUnionTypesFromConfig]

[AttributeUsage(AttributeTargets.Assembly)]
sealed class GenerateUnionTypesFromConfigAttribute : Attribute;
