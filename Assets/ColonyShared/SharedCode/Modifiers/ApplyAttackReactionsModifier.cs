using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects;
using SharedCode.Utils;

namespace ColonyShared.SharedCode.Modifiers
{
    public static partial class SpellModifierExtensions
    {
        public static readonly IEqualityComparer<AttackActionDef> _ReactionsComparer = EqualityComparerFactory.Create<AttackActionDef>((x,y) => x != null && y != null && x.When == y.When && x.IdResource == y.IdResource);

        public static ResultHolder ApplyAttackReactionsModifier(this IReadOnlyList<AttackModifierDef> modifiers, IReadOnlyList<AttackActionDef> reactions, AttackActionTarget target)
        {
            if (modifiers == null)
                return new ResultHolder(reactions);

            // LINQ-free programming! only for! only hardcore! :)
            using (var reactionsModifiers = PooledArray<AttackActionsModifierDef>.Create(modifiers.Count, false))
            {
                int reactionsModifiersCount = 0;
                for (int i = 0, modifiersCount = modifiers.Count; i < modifiersCount; ++i)
                {
                    var modifier = modifiers[i];
                    if (modifier is AttackActionsModifierDef reactionModifier && reactionModifier.Target == target)
                        reactionsModifiers.Array[reactionsModifiersCount++] = reactionModifier;
                }

                if (reactionsModifiersCount == 0)
                    return new ResultHolder(reactions);
                
                var output = PooledArray<AttackActionDef>.Create(reactions.Count + reactionsModifiersCount);
                int outputCount = 0;
                for (int reactionIdx = 0, reactionsCount = reactions.Count; reactionIdx < reactionsCount; ++reactionIdx)
                {
                    var reaction = reactions[reactionIdx];
                    bool remove = false;
                    for (int i = 0; !remove && i < reactionsModifiersCount; ++i)
                    {
                        var modifier = reactionsModifiers.Array[i];
                        if (_ReactionsComparer.Equals(modifier.RemoveAction, reaction) || _ReactionsComparer.Equals(modifier.AddAction, reaction))
                        {
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Remove reaction | Reaction:{reaction} Modifier:{modifier.____GetDebugShortName()}").Write();
                            remove = true;
                        }
                    }
                    if (!remove)
                        output.Array[outputCount++] = reaction;
                }

                for (int i = 0; i < reactionsModifiersCount; ++i)
                {
                    var modifier = reactionsModifiers.Array[i];
                    if (modifier.AddAction != null)
                    {
                        bool skip = false;
                        for (int j = 0; !skip && j < outputCount; ++j)
                        {
                            var reaction = output.Array[j];
                            if (_ReactionsComparer.Equals(modifier.AddAction, reaction))
                            {
                                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Duplicate reaction detected | Reaction:{reaction} Modifier:{modifier.____GetDebugShortName()}").Write();
                                skip = true;
                            }
                        }
                        if (!skip)
                        {
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add reaction | Reaction:{modifier.AddAction.Target} Modifier:{modifier.____GetDebugShortName()}").Write();
                            output.Array[outputCount++] = modifier.AddAction;
                        }
                    }
                }

                return new ResultHolder(output, outputCount);
            }
        }
        
        
        public readonly struct ResultHolder : IDisposable
        {
            private readonly bool _needDispose;
            private readonly PooledArray<AttackActionDef> _array;
            private readonly IReadOnlyList<AttackActionDef> _collection;

            public IReadOnlyList<AttackActionDef> Collection => _collection;

            public ResultHolder(IReadOnlyList<AttackActionDef> list)
            {
                _array = default;
                _needDispose = false;
                _collection = list;
            }

            public ResultHolder(PooledArray<AttackActionDef> array, int count)
            {
                _array = array;
                _needDispose = true;
                _collection = array.GetSegment(0, count);
            }

            public void Dispose()
            {
                if (_needDispose)
                    _array.Dispose();
            }
        }
    }
}