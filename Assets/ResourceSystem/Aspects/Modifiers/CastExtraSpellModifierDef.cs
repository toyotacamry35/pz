using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace ResourceSystem.Aspects
{
    public class CastExtraSpellModifierDef : SpellModifierDef
    {
        public ResourceRef<SpellDef> Spell;
    }
}