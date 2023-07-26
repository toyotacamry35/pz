using GeneratedCode.DeltaObjects;
using SharedCode.Wizardry;
using Src.ManualDefsForSpells;

namespace ColonyShared.GeneratedCode.Combat
{
    public static class AnimationPlayId
    {
        public static SpellPartCastId CreatePlayId(SpellWordCastData cast, EffectAnimatorDef.StateDef def) => new SpellPartCastId(cast.SpellId, cast.SubSpellCount, def);
        public static SpellPartCastId CreatePlayId(SpellId spellId, int subSpellId, EffectAnimatorDef.StateDef def) => new SpellPartCastId(spellId, subSpellId, def);
    }
}