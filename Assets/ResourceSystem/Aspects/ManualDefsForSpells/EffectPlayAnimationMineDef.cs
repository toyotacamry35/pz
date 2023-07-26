using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.Misc;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectPlayAnimationMineDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> AnimatorOwner { get; set; }
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public bool GetFromKnowType { get; set; }
        public ResourceRef<AnimationParameterDef> TriggerName { get; set; }
        public ResourceRef<AnimationParameterDef> IntName { get; set; }
        public bool UseDetach { get; set; } = true;
    }
}
