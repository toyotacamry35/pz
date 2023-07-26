using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace GeneratedDefsForSpells
{
    public class EffectSetAnimatorReactionDirDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> Provocator { get; set; }
        public ResourceRef<SpellEntityDef> Reactive { get; set; }
        public ResourceRef<SpellVector3Def> HitDirection { get; set; }
    }
}