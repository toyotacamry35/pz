using System.Threading.Tasks;
using Assets.Src.Aspects;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using UnityEngine;

namespace Assets.Src.Effects.AnimatorEffects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectAnimatorSetDirection: IClientOnlyEffectBinding<EffectAnimatorSetDirectionDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectAnimatorSetDirectionDef def)
        {
            var selfDef = def;
            var dir = selfDef.Direction.Target.GetVec3(cast, Vector3.zero);
            UnityQueueHelper.RunInUnityThreadNoWait(() => SetDirection(cast, selfDef, dir));
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectAnimatorSetDirectionDef def)
        {
            var selfDef = def;
            if (selfDef.UseDetach)
                UnityQueueHelper.RunInUnityThreadNoWait(() => SetDirection(cast, selfDef, Vector3.zero));
            return new ValueTask();
        }

        private static void SetDirection(SpellWordCastData cast, EffectAnimatorSetDirectionDef def, Vector3 dir)
        {
            var animator = (cast.GetCaster()?.GetComponent<IEntityPawn>()?.View.Value as IAnimatedView)?.Animator;
            if (!animator)
                return;
            if(def.ParameterX.Target!=null)
                animator.SetFloat(def.ParameterX.Target.Name, dir.x);
            if(def.ParameterY.Target!=null)
                animator.SetFloat(def.ParameterY.Target.Name, dir.y);
            if(def.ParameterZ.Target!=null)
                animator.SetFloat(def.ParameterZ.Target.Name, dir.z);
        }
    }
}