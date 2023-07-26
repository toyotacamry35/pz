using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace Assets.Src.Aspects.Impl.Factions.Template
{
    public class FactionDef : SaveableBaseResource
    {
        public LocalizedString NameLs { get; set; }

        public LocalizedString DescriptionLs { get; set; }
        public ResourceRef<CalcerDef<bool>> Permadeath { get; set; }
        public ResourceRef<PredicateDef> CorpsesVisibleOnlyForOwner; 
        public bool UnbreakableItems;
        public bool RemoveAllQuests;
        public bool RemoveAllItems;
        public bool RemoveAllPerks;

        public ResourceRef<RelationshipRulesDef> RelationshipRules;
    }
}