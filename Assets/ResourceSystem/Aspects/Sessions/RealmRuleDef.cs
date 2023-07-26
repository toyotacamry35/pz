using System;
using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace SharedCode.Aspects.Sessions
{
    [Localized]
    public class RealmRuleDef : BaseResource
    {
        public LocalizedString Description { get; set; }
        public SemanticContext SemanticContext { get; set; }
    }

    public enum SemanticContext
    {
        Primary = 0,
        Info = 1,
        Success = 2,
        Danger = 3
    }

    public class CustomRealmRuleDef : BaseResource
    {
        public float DefaultValue { get; set; } = 0;
    }

    public class RealmGameTime : RealmRuleDef
    {
        public int DaysCount { get; set; }
    }
    
    public class RealmPlayersInteraction : RealmRuleDef
    {
        public PlayersInteraction PlayersInteraction { get; set; }
    }

    public enum PlayersInteraction
    {
        Hostile = 0,
        Friendly = 1,
    }

    public class RealmDeathLimit : RealmRuleDef
    {
        public DeathLimit DeathLimit { get; set; }
    }

    public enum DeathLimit
    {
        Default = 0,
        Single = 1,
    }

    public class RealmExp : RealmRuleDef
    {
        public ExpReward ExpReward { get; set; }
    }

    public enum ExpReward
    {
        Default = 0,
	Extra50Pct = 2,
        Triple = 3,
        Quintuple = 5
    }
}