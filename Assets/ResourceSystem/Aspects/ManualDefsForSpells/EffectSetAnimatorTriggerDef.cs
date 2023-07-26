using System;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.Misc;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectSetAnimatorTriggerDef : SpellEffectDef
    {
        [Obsolete] public string TriggerName { get; set; }
        public ResourceRef<AnimationParameterDef> AnimatorParameter { get; set; }
    }
}
