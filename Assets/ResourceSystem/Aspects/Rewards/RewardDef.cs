using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace ResourceSystem.Aspects.Rewards
{
    [Localized]
    public class RewardDef : SaveableBaseResource
    {
        public LocalizedString Title { get; set; }
        
        public int Experience { get; set; }
        public float ExperienceMultiplier { get; set; }
    }
}
