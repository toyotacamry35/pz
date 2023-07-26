using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Aspects.Statistic
{
    public class StatisticRoot : BaseResource
    {
        public ResourceRef<StatisticType> DamageType { get; set; }
    }
}
