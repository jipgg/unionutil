using UnionUtil.Internal;

public static class Sources {

   public const string Trivial = $"""
   using {nameof(UnionUtil)};
   [{nameof(GenerateUnionAttribute)}]
   partial struct Trivial<T, U, V, W>;
   """;

   public const string NonTrivial = $$"""
   using UnionUtil;
   using System.Collections.Generic;
   using static UnionUtil.{{nameof(UnionGeneratorOptions)}};
   enum NonTrivialTag {Tag1, Tag2, Tag3, Tag4}
   [UnionImpl({{nameof(EnableUnionTypeInterface)}} | {{nameof(BoxUnconstrainedGenerics)}} | {{nameof(EnableGenericHoldsTypeMethod)}})]
   [{{nameof(TaggedAttribute<>)}}<NonTrivialTag>, {{nameof(SmallBufferOptimizedAttribute)}}(123)]
   readonly partial struct NonTivial<T1, T2, T3> : I{{MetaConfiguration.TypeMarkerName}}<T1, T2, List<T3>, double>;
   """;

   public const string Many = $"""
   using {nameof(UnionUtil)};
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many1<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many2<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many3<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many4<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many5<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many6<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many7<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many8<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many9<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many10<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many11<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many12<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many13<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many14<T, U, V, W, X, Y, Z>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many15<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many16<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many17<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many18<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many19<T, U, V, W>;
   [{nameof(GenerateUnionAttribute)}]
   partial struct Many20<T, U, V, W, X>;
   """;

   public static readonly SyntaxTree[] SyntaxTrees = [
      CSharpSyntaxTree.ParseText(Trivial),
      CSharpSyntaxTree.ParseText(NonTrivial),
   ];

}
