using Assets.Src.Aspects.Impl;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Assets.Src.Aspects;
using JetBrains.Annotations;

namespace Assets.Src.Impacts.AnimatorImpacts
{
    [UsedImplicitly, PredictableEffect]
    public class EffectSetAnimatorTrigger : IClientOnlyEffectBinding<EffectSetAnimatorTriggerDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSetAnimatorTriggerDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                EffectSetAnimatorTriggerDef selfDef = def;
                var animator = (cast.GetCaster()?.GetComponent<IEntityPawn>()?.View.Value as IAnimatedView)?.Animator; 
                if (animator != null)
                {
                    var triggerName = selfDef.AnimatorParameter != null ? selfDef.AnimatorParameter.Target.Name : selfDef.TriggerName;
                    animator.SetTrigger(triggerName);
                }
            });

            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSetAnimatorTriggerDef def)
        {
            return new ValueTask();
        }
    }
}
