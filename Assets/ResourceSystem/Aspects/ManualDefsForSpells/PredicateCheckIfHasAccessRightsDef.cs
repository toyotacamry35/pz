using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class PredicateCheckIfHasAccessRightsDef : SpellPredicateDef
    {
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<SpellTargetDef> Target { get; set; }
    }
}