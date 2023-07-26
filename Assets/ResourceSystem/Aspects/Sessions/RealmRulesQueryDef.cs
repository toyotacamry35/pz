using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace SharedCode.Aspects.Sessions
{
    [Localized]
    public class RealmRulesQueryDef : SaveableBaseResource
    {
        public LocalizedString LockedDescription { get; set; }
        public int Priority { get; set; }
        public ResourceRef<RealmRulesDef> RealmRules  { get; set; }
    }
    
    public class RealmRulesQueriesConfigDef : BaseResource
    {
        public ResourceRef<RealmRulesQueriesPackDef> Develop { get; set; }
        public ResourceRef<RealmRulesQueriesPackDef> Release { get; set; }
    }
	
    public class RealmRulesQueriesPackDef : BaseResource
    {
        public ResourceRef<RealmRulesQueryDef>[] Queries { get; set; }
    }

}