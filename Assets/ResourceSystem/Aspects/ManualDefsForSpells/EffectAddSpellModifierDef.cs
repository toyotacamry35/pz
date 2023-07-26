using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.Src.ResourcesSystem.Base;
using ResourceSystem.Aspects;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectAddSpellModifierDef : SpellEffectDef
    {
        public ResourceRef<PredicateDef> Condition;
        public ResourceArray<SpellModifierDef> Modifiers;
    }
}