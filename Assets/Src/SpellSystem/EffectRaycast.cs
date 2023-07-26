using System.Linq;
using Assets.Src.Shared;
using System.Threading.Tasks;
using Assets.Src.Camera;
using Assets.Src.Doll;
using Assets.Src.Shared.Impl;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using Src.Effects.Specific;
using UnityEngine;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using Assets.Src.Tools;
using JetBrains.Annotations;
using SharedCode.Serializers;

namespace Assets.Src.Impacts
{
    [UsedImplicitly, PredictableEffect]
    public class EffectRaycast : IClientOnlyEffectBinding<EffectRaycastDef>
    {
        private readonly Vector3 _pointView = new Vector3(0.5f, 0.5f, 0);
        private static readonly NLog.Logger Logger = LogManager.GetLogger("EffectRaycast");

        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectRaycastDef indef)
        {
            var def = indef;
            var casterRef = cast.Caster;
            if (def.Caster.Target != null)
                casterRef = await def.Caster.Target.GetOuterRef(cast, repo);

            if(!casterRef.IsValid)
                casterRef = cast.Caster;

            await UnityQueueHelper.RunInUnityThread(() =>
            {
                var casterGo = GameObjectCreator.ClusterSpawnInstance.GetImmediateObjectFor(casterRef);
                var weapon = casterGo.GetComponentsInChildren<VisualSlot>().FirstOrDefault(x => x.IsInUse)?.AttachedObj;
                var shotMarker = weapon
                    ? weapon
                        .GetComponentsInChildren<MarkerOnWeapon>()
                        .Where(x => x.Id == MarkerOnWeapon.Identifier.Shot)
                        .Select(x => x.transform)
                        .FirstOrDefault()
                    : null;

                var camera = GameCamera.Camera;
                if (camera != null)
                {
                    Ray ray = camera.ViewportPointToRay(_pointView);
                    ray.origin = ray.GetPoint(Vector3.Dot((shotMarker ?? casterGo.transform).position - ray.origin, ray.direction));

                    Vector3 hitPoint;
                    var targetRef = default(OuterRef<IEntityObject>);
                    var ignoreTriggers = Physics.queriesHitTriggers;
                    try
                    {
                        Physics.queriesHitTriggers = false;
                        RaycastHit hit;
                        if (Physics.Raycast(ray.origin, ray.direction, out hit, PhysicsChecker.CheckDistance(def.Distance, def.ToString()), PhysicsLayers.BulletMaskEx))
                        {
                            hitPoint = hit.point;
                            var target = hit;
                            var targetGo = target.collider.gameObject;
                            var targetEgo = targetGo.GetComponent<EntityGameObject>();
                            if (targetEgo)
                                targetRef = targetEgo.OuterRef;
                        }
                        else
                            hitPoint = ray.GetPoint(def.Distance);
                    }
                    finally
                    {
                        Physics.queriesHitTriggers = ignoreTriggers;
                    }

                    if (cast.IsSlave && cast.SlaveMark.OnClient)
                    {
                        if (def.ShotFX.Target && shotMarker)
                        {
                            var startPoint = shotMarker.position;
                            var shotRay = new Ray(startPoint, (ray.GetPoint(def.Distance) - startPoint).normalized);
                            var endPoint = shotRay.GetPoint(Vector3.Dot(hitPoint - shotRay.origin, shotRay.direction));
                            var shotRot = Quaternion.LookRotation(shotRay.direction, shotMarker.up);
                            var shotInstance = GameObjectCreator.CreateGameObject(def.ShotFX.Target, shotMarker.position, shotRot);
                            var raycastShotFx = shotInstance.GetComponent<RaycastShotFX>();
                            if (raycastShotFx)
                            {
                                raycastShotFx.StartPoint = startPoint;
                                raycastShotFx.EndPoint = endPoint;
                            }
                        }

                        var muzzleMarker = weapon
                            ? weapon.GetComponentsInChildren<MarkerOnWeapon>()
                                .Where(x => x.Id == MarkerOnWeapon.Identifier.Muzzle)
                                .Select(x => x.transform)
                                .FirstOrDefault()
                            : null;

                        if (def.MuzzleFX.Target && muzzleMarker)
                        {
                            GameObjectCreator.CreateGameObject(def.MuzzleFX.Target, muzzleMarker.position, muzzleMarker.rotation);
                        }
                    }

                    if (targetRef.IsValid)
                    {
                        AsyncUtils.RunAsyncTask(async () =>
                        {
                            using (var wrapper = await repo.Get(cast.Wizard.TypeId, cast.Wizard.Guid))
                            {
                                var aggressorWizard = wrapper.Get<IWizardEntityClientFull>(cast.Wizard.TypeId, cast.Wizard.Guid, ReplicationLevel.ClientFull);
                                if (aggressorWizard != null)
                                {
                                    foreach (var spell in def.AppliedSpells)
                                    {
                                        await aggressorWizard.CastSpell(new SpellCastWithTarget(null) { Def = spell, Target = new OuterRef<IEntity>(targetRef.Guid, targetRef.TypeId) });
                                    }
                                }
                            }
                        }, repo);
                    }
                }
            });

            return;
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectRaycastDef def)
        {
            return new ValueTask();
        }
    }
}