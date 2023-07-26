using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Sessions;

namespace L10n
{
    [Localized]
    public class RealmRulesLocalizationDef : BaseResource
    {
        public LocalizedString GameTimeLabel { get; set; }
        public LocalizedString PlayersInteractionLabel { get; set; }
        public LocalizedString DeathLimitLabel { get; set; }
        public LocalizedString ExpLabel { get; set; }
        
        public LocalizedString DaysCount { get; set; }
        public Dictionary<PlayersInteraction, LocalizedString> PlayersInteraction { get; set; }
        public Dictionary<DeathLimit, LocalizedString> DeathLimit { get; set; }
        public Dictionary<ExpReward, LocalizedString> ExpReward { get; set; }
    }
}