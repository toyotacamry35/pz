using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public abstract class BaseEffectOpenUiDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpellEntityDef> Caster { get; set; }
    }
}