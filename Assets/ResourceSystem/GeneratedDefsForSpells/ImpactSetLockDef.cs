using SharedCode.Wizardry;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.AccessRights;

namespace GeneratedDefsForSpells
{
    public class ImpactSetLockDef : SpellImpactDef
    {
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<AccessPredicateDef> Predicate { get; set; }
    }
}
