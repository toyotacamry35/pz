// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class ProxyStat
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var ValueCache__rnd = (float)(__random__.Next(1000) / 100.0);
                ValueCache = ValueCache__rnd;
            }

            {
                var LimitMinCache__rnd = (float)(__random__.Next(1000) / 100.0);
                LimitMinCache = LimitMinCache__rnd;
            }

            {
                var LimitMaxCache__rnd = (float)(__random__.Next(1000) / 100.0);
                LimitMaxCache = LimitMaxCache__rnd;
            }

            {
                var allValuesStatType = System.Enum.GetValues(typeof(Assets.Src.Aspects.Impl.Stats.StatType));
                var valueStatType = (Assets.Src.Aspects.Impl.Stats.StatType)allValuesStatType.GetValue(__random__.Next() % allValuesStatType.Length);
                var StatType__rnd = valueStatType;
                StatType = StatType__rnd;
            }
        }
    }
}