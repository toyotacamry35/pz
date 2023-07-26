using System.Threading.Tasks;
using Assets.Src.Aspects.Impl;
using JetBrains.Annotations;
using NLog;
using UnityEngine;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using Assets.Src.Wizardry;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;

namespace Assets.Src.Impacts.AnimatorImpacts
{
    [UsedImplicitly, PredictableEffect]
    internal class EffectSetAnimatorReactionDir : IClientOnlyEffectBinding<EffectSetAnimatorReactionDirDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Animations");

        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSetAnimatorReactionDirDef def)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var selfDef = def;
                var resultVec = selfDef.HitDirection.Target.GetVec3(cast, Vector3.zero);
                if (!Mathf.Approximately(resultVec.sqrMagnitude, 1f))
                {
                    Logger.IfWarn()?.Message($"_hitDirection magn. != 1 (sqrMagn== {resultVec.sqrMagnitude}).").Write();
                    resultVec.Normalize();
                }

                var animator = (selfDef.Reactive.Target.GetGo(cast).GetComponent<IEntityPawn>()?.View.Value as IAnimatedView)?.Animator; 
                if(animator!= null)
                {
                    animator.SetFloat(Consts.AnimatorReactionDirZ, resultVec.z);
                    animator.SetFloat(Consts.AnimatorReactionDirX, resultVec.x);
                }

                Logger.IfDebug()?.Message($"ImpactSetAnimatorReactionDir.Apply. Spell: {cast.CastData.Def}, resultVec: {resultVec}").Write();   
            });

            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectSetAnimatorReactionDirDef def)
        {
            return new ValueTask();
        }
    }
}
