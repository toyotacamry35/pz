using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class PredicateCheckIfInRangeDef : SpellPredicateDef
    {
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public float Range { get; set; }
    }
}