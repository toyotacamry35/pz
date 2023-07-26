using Assets.Src.Aspects.Impl;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Character;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Src.Effects.AnimatorEffects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectPlayAnimationMine : IClientOnlyEffectBinding<EffectPlayAnimationMineDef>
    {
        int GetValue(SpellWordCastData cast, EffectPlayAnimationMineDef def)
        {
            Interactive interactive = def.Target.Target.GetGo(cast)?.GetComponent<Interactive>();
            if (interactive != null)
            {
                return def.GetFromKnowType ? (int)interactive.KnowType : (int)interactive.InteractionType;
            }

            return 0;
        }

        Animator GetAnimator(SpellEntityDef animationOwnerDef, SpellWordCastData cast)
        {
            var animatorOwner = animationOwnerDef?.GetGo(cast) ?? cast.GetCaster(); 
            if(!animatorOwner)
                return null;
            return (animatorOwner.GetComponent<IEntityPawn>()?.View.Value as IAnimatedView)?.Animator;
        }

        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPlayAnimationMineDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var selfDef = def;
                var animator = GetAnimator(selfDef.AnimatorOwner, cast);
                if (animator)
                {
                    if (selfDef.IntName.Target != null)
                        animator.SetInteger(selfDef.IntName.Target.Name, GetValue(cast, selfDef));
                    if (selfDef.TriggerName.Target != null)
                        animator.SetTrigger(selfDef.TriggerName.Target.Name);
                }
            });

            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPlayAnimationMineDef def)
        {
            var selfDef = def;
            if (!selfDef.UseDetach)
                return new ValueTask();

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var animator = GetAnimator(selfDef.AnimatorOwner, cast);
                if (animator != null)
                {
                    //if (charView.Animator.GetInteger(selfDef._intName.Target.Name) == GetValue(cast, selfDef))
                    if(selfDef.IntName.Target != null)
                        animator.SetInteger(selfDef.IntName.Target.Name, 0);
                }
            });

            return new ValueTask();
        }
    }
}

