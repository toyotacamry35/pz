using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class EffectAddContextualReactionsDef : SpellEffectDef
    {
        public ResourceRef<SpellEntityDef> Target { get; set; }
        public List<ContextualReactionData> ContextualReactionsData { get; set; }
    }

    public class ReactionProvocatorType : BaseResource
    {
    }

    public interface IContextualAction : IEquatable<IContextualAction>
    {
        int Priority { get; }

        SpellDef SpellDef { get; }

        SpellDef CheckSpellDef { get; }
    }

    public class ContextualActionWithPriorityDef : BaseResource, IContextualAction
    {
        public int _priority { get; set; }
        public ResourceRef<SpellDef> _spell { get; set; }
        public ResourceRef<SpellDef> _checkSpell { get; set; }

        public int Priority => _priority;

        public SpellDef SpellDef => _spell; 

        public SpellDef CheckSpellDef => _checkSpell;
        

        public bool Equals(IContextualAction other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _priority == other.Priority && _spell == other.SpellDef && _checkSpell == other.CheckSpellDef;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IContextualAction);
        }
    }

    
    public class ContextualActionInstance2 : IContextualAction
    {
        public int Priority { get; }

        public SpellDef SpellDef { get; }

        public SpellDef CheckSpellDef { get; }

        public ContextualActionInstance2(int priority, SpellDef spell, SpellDef checkSpell)
        {
            Priority = priority;
            SpellDef = spell;
            CheckSpellDef = checkSpell;
        }

        public bool Equals(IContextualAction other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Priority == other.Priority && SpellDef == other.SpellDef && CheckSpellDef == other.CheckSpellDef;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IContextualAction);
        }
    }

    public class ContextualReactionData : BaseResource
    {
        public ResourceRef<ReactionProvocatorType> ProvocatorTypePath { get; set; }

        public ResourceRef<ContextualActionWithPriorityDef>[] ReactionContextualActions = Array.Empty<ResourceRef<ContextualActionWithPriorityDef>>();
    }
}