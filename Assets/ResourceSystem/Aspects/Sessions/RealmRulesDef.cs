using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using L10n;

namespace SharedCode.Aspects.Sessions
{
    [Localized]
    public class RealmRulesDef : SaveableBaseResource
    {
        public ResourceRef<MapDef> DefaultMap { get; set; }
        public ResourceRef<MapDef> FirstTimeMap { get; set; }
        public int MinLevel { get; set; } = 0;

        public int ExistenceHours { get; set; } = -1;
        public int CanJoinHours { get; set; } = -1;
        public float ExpMultiplier { get; set; } = 1;
        public LocalizedString Title { get; set; }
        public LocalizedString Description { get; set; }

        public List<ResourceRef<RealmRuleDef>> Rules { get; set; }
        public Dictionary<ResourceRef<CustomRealmRuleDef>, float> Values { get; set; }

        // public RealmGameTime GameTime  { get; set; }
        // public RealmPlayersInteraction PlayersInteraction  { get; set; }
        // public RealmDeathLimit DeathLimit  { get; set; }
        // public RealmExp Exp  { get; set; }

    }

    public class RealmRulesConfigDef : BaseResource
    {
        public ResourceRef<RealmRulesPackDef> Develop;
        public ResourceRef<RealmRulesPackDef> Release;
    }

    public class RealmRulesPackDef : BaseResource
    {
        public ResourceRef<RealmRulesDef>[] Rules;
    }
}