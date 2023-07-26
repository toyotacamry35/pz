using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.Rewards;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactGrantAccountRewardDef : SpellImpactDef
    {
        public ResourceRef<RewardDef> Reward { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}
