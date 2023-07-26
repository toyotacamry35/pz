using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.Aspects.Counters.Template;
using SharedCode.Aspects.Science;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class ImpactAddKnowledgeDef : SpellImpactDef, IManyRewardsSource
    {
        public ResourceRef<SpellEntityDef> Caster { get; set; }
        public ResourceRef<KnowledgeDef> Knowledge { get; set; }

        IRewardSource[] IManyRewardsSource.Rewards => Knowledge.Target.Rewards;
    }
}
