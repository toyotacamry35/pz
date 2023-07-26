using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;

namespace ColonyShared.SharedCode.Aspects.Counters.Template
{
    public class DealDamageCounterDef : TargetedQuestCounterDef<IEntityObjectDef>
    {
        public ResourceRef<StatisticType> ObjectType { get; set; }
        public float Value { get; set; }
    }
}
