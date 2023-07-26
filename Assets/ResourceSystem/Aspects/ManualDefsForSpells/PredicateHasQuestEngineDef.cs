using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ResourceSystem.Aspects.ManualDefsForSpells
{
    public class PredicateHasQuestEngineDef : SpellPredicateDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
    }
}