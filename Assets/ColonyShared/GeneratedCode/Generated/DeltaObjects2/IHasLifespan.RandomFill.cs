// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class Lifespan
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var CountdownCancellationToken__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.EntitySystem.ChainCalls.ChainCancellationToken>();
                CountdownCancellationToken = CountdownCancellationToken__rnd;
            }

            {
                var allValuesOnLifespanExpired = System.Enum.GetValues(typeof(Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired));
                var valueOnLifespanExpired = (Assets.ColonyShared.SharedCode.Entities.OnLifespanExpired)allValuesOnLifespanExpired.GetValue(__random__.Next() % allValuesOnLifespanExpired.Length);
                var DoOnExpired__rnd = valueOnLifespanExpired;
                DoOnExpired = DoOnExpired__rnd;
            }

            {
                var LifespanSec__rnd = (float)(__random__.Next(1000) / 100.0);
                LifespanSec = LifespanSec__rnd;
            }

            {
                var IsLifespanExpired__rnd = __random__.Next() % 2 == 1;
                IsLifespanExpired = IsLifespanExpired__rnd;
            }

            {
                var LifespanCycleNumber__rnd = (int)__random__.Next(100);
                LifespanCycleNumber = LifespanCycleNumber__rnd;
            }

            {
                var BirthTime__rnd = (long)__random__.Next(100);
                BirthTime = BirthTime__rnd;
            }
        }
    }
}