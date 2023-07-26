using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectCastSpellDef : SpellEffectDef
    {
        public ResourceRef<SpellDef> Spell { get; set; }
    }
}
