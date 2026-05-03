namespace Test.MustCompile;
using static UnionGeneratorOptions;

[GenerateUnion(FieldVisibility = Visibility.Public)]
sealed partial class SealedClass : IUnionTypeArguments<int, float, object, List<object>>;


#pragma warning disable
[GenerateUnion(EnableReadOnly), UnionTypeArguments<int, float>]
partial struct ReadonlyStruct;
#pragma warning restore

[GenerateUnion(BoxUnconstrainedGenerics), SmallBufferOptimized]
readonly partial struct ReadonlyStructSbo<T, U> : IUnionTypeArguments<T, U>;

[GenerateUnion(BoxUnconstrainedGenerics)]
readonly partial struct ReadonlyStruct<T, U> : IUnionTypeArguments<T, U>;

[GenerateUnion]
readonly partial struct ReadonlyStructSequential<T, U> : IUnionTypeArguments<T, U>;
