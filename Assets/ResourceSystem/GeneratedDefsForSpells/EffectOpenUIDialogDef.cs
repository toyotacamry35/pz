using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.Dialog;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class EffectOpenUIDialogDef : SpellEffectDef
    {
        public ResourceRef<SpellTargetDef> Target { get; set; }
        public ResourceRef<SpellCasterDef> Caster { get; set; }
        public ResourceRef<DialogDef> Dialog { get; set; }
    }
}
 