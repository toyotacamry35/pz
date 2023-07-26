using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Science;

namespace GeneratedDefsForSpells
{
    public class PredicateKnowledgeDef : SharedCode.Wizardry.SpellPredicateDef
    {
        public Assets.Src.ResourcesSystem.Base.ResourceRef<SharedCode.Wizardry.SpellEntityDef> Caster { get; set; }
        public ResourceRef<KnowledgeDef> Knowledge { get; set; }
        public bool IncludeBlocked { get; set; }
    }
}
