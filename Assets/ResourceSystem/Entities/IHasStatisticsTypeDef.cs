using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasStatisticsTypeDef
    {
        ResourceRef<StatisticType> ObjectType { get; set; }
    }
}