using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectAddTestReactionToCollectionDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public ResourceRef<SpellDef> ReactionSpell_Deprecated { get; set; }
        public List<ResourceRef<ContextualActionWithPriorityDef>> ContextualReactions { get; set; }
    }
}