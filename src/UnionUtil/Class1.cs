using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Numerics;
namespace UnionUtil;

public record Abc();
public readonly record struct Xyz(object Any);
[UnionImpl(
   BoxGenerics = false,
   BoxManagedStructs = false,
   Mutable = true,
   FieldVisibility = Visibility.Public
)]
sealed partial class MyUnion2<T1, TVal> : UnionTypes<TVal, int, float, T1, Xyz, List<float>, List<int>> where TVal : struct;
public class Class1 {
   public void E() {
      MyUnion2<Abc, DateTime> x = 1.0f;
      x.SetValue(2.0f);
   }
}
[UnionImpl]
readonly partial struct Unmanager : UnionTypes<int, float, Vector3>;
