using Assets.Src.Character.Events;
using Assets.Src.ManualDefsForSpells;
using Assets.Src.Shared.Impl;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    [UsedImplicitly, PredictableEffect]
    class EffectPostVisualEventVisibleOnlyToPlayer : IClientOnlyEffectBinding<EffectPostVisualEventVisibleOnlyToPlayerDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPostVisualEventVisibleOnlyToPlayerDef indef)
        {
            if (cast.OnClientWithAuthority())
            {
                var def = indef;
                GameObject castergo = null;
                GameObject targetgo = null;
                OuterRef<IEntity> targetEntityRef = default(OuterRef<IEntity>);
                VisualEventProxy vep = default(VisualEventProxy);
                Vector3 position = default(Vector3);
                Quaternion rotation = default(Quaternion);
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                {
                    castergo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(cast.Caster);
                    var castDataAsIWithTarget = cast.CastData as IWithTarget;
                    targetEntityRef = castDataAsIWithTarget?.Target ?? default(OuterRef<IEntity>);
                    if (targetEntityRef != null && targetEntityRef.IsValid)
                    {
                        targetgo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(targetEntityRef);
                    }

                    vep = castergo.GetComponent<VisualEventProxy>();
                    position = castergo.transform.position;
                    rotation = castergo.transform.rotation;

                    var evt = new VisualEvent()
                    {
                        casterRepository = repo,
                        eventType = def.TriggerName,
                        casterGameObject = castergo,
                        targetGameObject = targetgo,
                        targetEntityRef = targetEntityRef,
                        position = position,
                        rotation = rotation
                    };
                    vep.PostEvent(evt);
                });
            }
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectPostVisualEventVisibleOnlyToPlayerDef indef)
        {
            return new ValueTask();
        }
    }
}