using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects.Misc;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectAnimatorSetDirectionDef : SpellEffectDef
    {
        public ResourceRef<AnimationParameterDef> ParameterX { get; set; }
        public ResourceRef<AnimationParameterDef> ParameterY { get; set; }
        public ResourceRef<AnimationParameterDef> ParameterZ { get; set; }
        public ResourceRef<SpellVector3Def> Direction { get; set; }
        public bool UseDetach { get; set; }
    }
}