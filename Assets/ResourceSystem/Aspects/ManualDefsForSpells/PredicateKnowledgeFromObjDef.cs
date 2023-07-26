using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Assets.Src.Predicates
{
    public class PredicateKnowledgeFromObjDef : SpellPredicateDef
    {
        public ResourceRef<SpellEntityDef> Caster { get; set; }
        public ResourceRef<SpellEntityDef> KnowledgeHolder { get; set; }
    }
}

