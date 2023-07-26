using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Character.Events;
using Assets.Src.Effects.FX;
using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using Vector3 = SharedCode.Utils.Vector3;

namespace Src.Effects.FX
{
    [UsedImplicitly, PredictableEffect]
    public class EffectFx : IClientOnlyEffectBinding<EffectFxDef>
    {
        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectFxDef def)
        {
            var ownerGo = def.Owner != null ? def.Owner.Target.GetGo(cast) : cast.GetCaster();
            var targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            var targetGo = def.Target != null ? def.Target.Target.GetGo(cast) : null;
            var parameters = await EffectPostVisualEvent.MapParameters(cast, def.Params, repo);
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var evt = new VisualEvent
                {
                    casterRepository = repo,
                    casterGameObject = ownerGo,
                    targetEntityRef = targetRef,
                    targetRepository = repo,
                    targetGameObject = targetGo,
                    position = ownerGo.transform.position,
                    rotation = ownerGo.transform.rotation,
                    parameters = parameters
                };
                VisualEffectHandlerCollection.Get(def.Fx).OnEventUpdate(def.Fx, evt);
            });
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectFxDef def)
        {
            return new ValueTask();
        }
    }
}