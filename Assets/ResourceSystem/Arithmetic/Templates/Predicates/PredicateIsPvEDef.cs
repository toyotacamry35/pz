using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Sessions;
using System.Collections.Generic;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates
{
    public class PredicateIsPvEDef : PredicateDef
    { }
    public class PredicateIsRealmDef : PredicateDef
    {
        public List<ResourceRef<RealmRulesDef>> RealmRules { get; set; } = new List<ResourceRef<RealmRulesDef>>();
    }
    public class PredicateIsHardcoreDef : PredicateDef
    {
    }
    public class PredicateCustomRealmRuleDef : PredicateDef
    {
        public ResourceRef<CustomRealmRuleDef> CustomRealmRule { get; set; }
    }
    public class CalcerCustomRealmRuleDef : CalcerDef<float>
    {
        public ResourceRef<CustomRealmRuleDef> CustomRealmRule { get; set; }
    }
}