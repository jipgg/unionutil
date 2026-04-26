public static class Sources {

   public const string Trivial = """
   using UnionUtil;
   [UnionImpl]
   partial struct Trivial<T, U, V, W>;
   """;

   public const string NonTrivial = """
   using UnionUtil;
   using System.Collections.Generic;
   using static UnionUtil.UnionImplOptions;
   enum NonTrivialTag {Tag1, Tag2, Tag3, Tag4}
   [UnionImpl(ImplementUnionInterfaces | BoxOpenGenerics | ImplementHoldsTypeMethod)]
   [Tagged<NonTrivialTag>, SmallBufferOptimized(123)]
   readonly partial struct NonTivial<T1, T2, T3> : ICanHoldTypes<T1, T2, List<T3>, double>;
   """;

   public const string Many = """
   using UnionUtil;
   [UnionImpl]
   partial struct Many1<T, U, V, W>;
   [UnionImpl]
   partial struct Many2<T, U, V, W>;
   [UnionImpl]
   partial struct Many3<T, U, V, W>;
   [UnionImpl]
   partial struct Many4<T, U, V, W>;
   [UnionImpl]
   partial struct Many5<T, U, V, W>;
   [UnionImpl]
   partial struct Many6<T, U, V, W>;
   [UnionImpl]
   partial struct Many7<T, U, V, W>;
   [UnionImpl]
   partial struct Many8<T, U, V, W>;
   [UnionImpl]
   partial struct Many9<T, U, V, W>;
   [UnionImpl]
   partial struct Many10<T, U, V, W>;
   [UnionImpl]
   partial struct Many11<T, U, V, W>;
   [UnionImpl]
   partial struct Many12<T, U, V, W>;
   [UnionImpl]
   partial struct Many13<T, U, V, W>;
   [UnionImpl]
   partial struct Many14<T, U, V, W, X, Y, Z>;
   [UnionImpl]
   partial struct Many15<T, U, V, W>;
   [UnionImpl]
   partial struct Many16<T, U, V, W>;
   [UnionImpl]
   partial struct Many17<T, U, V, W>;
   [UnionImpl]
   partial struct Many18<T, U, V, W>;
   [UnionImpl]
   partial struct Many19<T, U, V, W>;
   [UnionImpl]
   partial struct Many20<T, U, V, W, X>;
   """;

   public static readonly SyntaxTree[] SyntaxTrees = [
      CSharpSyntaxTree.ParseText(Trivial),
      CSharpSyntaxTree.ParseText(NonTrivial),
   ];

}
