using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class PredicateIsBuildingUpgradableDef : SpellPredicateDef
    {
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public bool Inversed { get; set; }
    }
}
