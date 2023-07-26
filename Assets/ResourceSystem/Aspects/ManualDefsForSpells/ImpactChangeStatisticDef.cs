using Assets.ColonyShared.SharedCode.Aspects.Statictic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactChangeStatisticDef : SpellImpactDef
    {
        public ResourceRef<StatisticType> Statistic { get; set; }
        public ResourceRef<StatisticType> TargetType { get; set; }
        public float Value { get; set; }
    }
}
