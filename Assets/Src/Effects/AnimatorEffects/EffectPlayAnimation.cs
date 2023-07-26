using Assets.Src.Aspects.Impl;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using JetBrains.Annotations;

namespace Assets.Src.Effects.AnimatorEffects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectPlayAnimation : IClientOnlyEffectBinding<EffectPlayAnimationDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPlayAnimationDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                EffectPlayAnimationDef selfDef = def;
                var animator = (cast.GetCaster()?.GetComponent<IEntityPawn>()?.View.Value as IAnimatedView)?.Animator;
                if (animator)
                {
                    if (selfDef.IntName.Target != null)
                        animator.SetInteger(selfDef.IntName.Target.Name, selfDef.IntValue);
                    animator.SetTrigger(selfDef.TriggerName.Target.Name);
                }
            });
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPlayAnimationDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var selfDef = def;
                var animator = (cast.GetCaster()?.GetComponent<IEntityPawn>()?.View.Value as IAnimatedView)?.Animator;
                if (animator)
                {
                    if (selfDef.IntName.Target != null && animator.GetInteger(selfDef.IntName.Target.Name) == selfDef.IntValue)
                        animator.SetInteger(selfDef.IntName.Target.Name, 0);
                }
            });
            return new ValueTask();
        }
    }
}
