using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode;
using Assets.ColonyShared.GeneratedCode.Impacts;
using Assets.ColonyShared.GeneratedCode.Impacts.ShapeUtils;
using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.GeneratedCode.Shared.Aspects;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Aspects.Misc;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using NLog;
using ResourcesSystem.Loader;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Utils.DebugCollector;
using Src.ManualDefsForSpells;

namespace GeneratedCode.Aspects.Combat
{
    public class AttackDoerServerAnimationTrajectory : IAttackDoer, IDisposable
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private const string TrajectoriesRoot = "/UtilPrefabs/Trajectories/Attacks";
        private static BoxShapeDef _defaultWeaponCollider = new BoxShapeDef
        {
            Position = Vector3.zero,
            Rotation = Vector3.forward,
            Extents = new Vector3(0.025f, 0.025f, 0.025f)
        };

        private readonly GameObjectMarkerDef _body; 
        private readonly OuterRef<IEntity> _ownerRef;
        private readonly IEntitiesRepository _repository;
        private readonly ConcurrentDictionary<SpellPartCastId, AttackSession> _attacks = new ConcurrentDictionary<SpellPartCastId, AttackSession>();

        public static string GetTrajectoriesDir(GameObjectMarkerDef bodyMarker) => $"{TrajectoriesRoot}/{bodyMarker.____GetDebugRootName()}";
        public static string GetTrajectoryName(AnimationStateDef animation) => animation.____GetDebugRootName();
        public static string GetTrajectoryPath(GameObjectMarkerDef bodyMarker, AnimationStateDef animation) => $"{GetTrajectoriesDir(bodyMarker)}/{GetTrajectoryName(animation)}";

        public AttackDoerServerAnimationTrajectory(OuterRef<IEntity> ownerRef, IEntitiesRepository repository, GameObjectMarkerDef bodyMarker)
        {
            _ownerRef = ownerRef.IsValid ? ownerRef : throw new ArgumentException(nameof(ownerRef));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _body = bodyMarker ?? throw new ArgumentNullException(nameof(bodyMarker));

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cont = await _repository.Get(_ownerRef))
                    await cont.Get<IHasAttackEngineClientFull>(_ownerRef, ReplicationLevel.ClientFull).AttackEngine.SetAttackDoer(this);
            }, _repository);
        }

        public void StartServerSideAttack(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, List<ResourceRef<GameObjectMarkerDef>> trajectoryMarkers, AnimationStateDef animation, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef)
        {
            if (trajectoryMarkers == null || trajectoryMarkers.Count == 0)
            {
                Logger.IfError()?.Message("StartAttack trajectory markers is null or empty {0}. Fill in spell", def).Write();
                return;
            }

            foreach (var trajectoryMarker in trajectoryMarkers)
                AsyncUtils.RunAsyncTask(async () =>
                {
                    try
                    {
                        await StartAttackInternal(attackId, def, colliderMarker, trajectoryMarker.Target, _body, animation, timeRange, currentTime, spellStartTime, spellDuration, targetRef);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Write(_ => _ + e + _ownerRef.Guid + ("Attack", def) + ("TrajectoryMarker", trajectoryMarker.Target) + ("ColliderMarker", colliderMarker) + ("Animation", animation));
                    }
                });
        }

        public void StartClientSideAttack(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, object animKey, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef)
        {
            throw new InvalidOperationException("This method must not be called on the server attack doer");
        }
        
        private BoxShapeDef getCasterCollider(IEntityObjectDef caster, GameObjectMarkerDef colliderMarker)
        {
            var attackCollider = caster.AttackCollider.Target;
            if (attackCollider == null)
                return _defaultWeaponCollider;

            var collider = attackCollider.DefaultCollider.Target;
            if (colliderMarker != default)
                if (attackCollider.CustomColliders.TryGetValue(colliderMarker, out var customCollider))
                    collider = customCollider.Target;

            return collider == default ? _defaultWeaponCollider : collider;
        }

        private BoxShapeDef getWeaponCollider(ItemResource itemDef, IEntityObjectDef caster, GameObjectMarkerDef colliderMarker)
        {
            var weaponDef = itemDef?.WeaponDef.Target;
            if (weaponDef == null)
                return getCasterCollider(caster, colliderMarker);

            var attackCollider = weaponDef.AttackCollider.Target;
            if (attackCollider == null)
                return getCasterCollider(caster, colliderMarker);

            var collider = attackCollider.DefaultCollider.Target;
            if (colliderMarker != default)
                if (attackCollider.CustomColliders.TryGetValue(colliderMarker, out var customCollider))
                    collider = customCollider.Target;

            return collider == default ? getCasterCollider(caster, colliderMarker) : collider;
        }

        private TrajectoryAnimationSetDef getTrajectoryAnimationSetDef(AnimationStateDef animation, GameObjectMarkerDef bodyMarker)
        {
            if (bodyMarker == null) throw new ArgumentNullException(nameof(bodyMarker));
            if (animation == null) throw new ArgumentNullException(nameof(animation));
            return GameResourcesHolder.Instance.LoadResource<TrajectoryAnimationSetDef>(GetTrajectoryPath(bodyMarker, animation));
        }

        private async Task StartAttackInternal(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, GameObjectMarkerDef trajectoryMarker, GameObjectMarkerDef bodyMarker, AnimationStateDef animation, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef)
        {
            Logger.IfDebug()?.Write("StartAttackInternal", _=>_ +  ("AttackId", attackId) + ("AttackDef", def?.____GetDebugAddress()) + ("ColliderMarker", colliderMarker?.____GetDebugAddress()) + ("TrajectoryMarker", trajectoryMarker?.____GetDebugAddress()) + ("Animation", animation?.____GetDebugAddress()) + ("BodyMarker", bodyMarker.____GetDebugAddress()) + ("TimeRange", timeRange) + ("CurrenTime", currentTime) + ("SpellStart", spellStartTime) + ("SpellDuration", spellDuration) + ("Target", targetRef));

            //var sw = new Stopwatch();
            //sw.Start();
            var attack = new AttackSession(attackId, def, new AttackAnimationInfo() { SpeedFactor = 1, AnimationOffset = 0, State = null, StartTime = currentTime });
            
            _attacks.TryAdd(attackId, attack);

            var timeOffset = timeRange - spellStartTime;
            float trajectoryStartSec = SyncTime.ToSecondsSafe(timeOffset.Start);
            float trajectoryEndSec = SyncTime.ToSecondsSafe(timeOffset.Finish);

            var trajectoryAnimationSetDef = getTrajectoryAnimationSetDef(animation, bodyMarker);
            var trajectory = trajectoryAnimationSetDef.Trajectories.SingleOrDefault(x => x.Key.Target == trajectoryMarker).Value.Target ?? throw new NullReferenceException($"Trajectory for {trajectoryMarker} not found in {trajectoryAnimationSetDef}");
            var timeFactor = spellDuration == 0 ? 1 : spellDuration / trajectory.Duration;
            Logger.IfDebug()?.Write(_ => _ + ("Trajectory", trajectoryAnimationSetDef.____GetDebugAddress()) + ("Start", trajectoryStartSec) + ("End", trajectoryEndSec) + ("SpellDuration", spellDuration) + ("TrjDuration", trajectory.Duration) + ("TimeFactor", timeFactor));
            var spaceId = await ImpactInShape.GetWorldSpaceId(_ownerRef, _repository);

            float checkVisibilityRadius = trajectory.BoundingSphereRadius + 5f;//Проверяем область видимости в радиусе максимальной анимационной точки + 5 метров (считаем что этом максимальный радиус моба/игрока)

            var needUpdateCasterAndTargetsData = true;
            Transform casterTransform = default;
            var casterAndTargetsAround = ShapeQuerySystem.GetEntitiesInRadiusVisibilityData(spaceId, _ownerRef, checkVisibilityRadius, _repository);
            if (targetRef != default)//Если явно указан таргет, то бьем только его
                casterAndTargetsAround.RemoveAll((x, y) => x != _ownerRef && x != targetRef);

            if (casterAndTargetsAround.Count <= 1)//Нет целей для атаки
                return;

            var casterAndTargetsTransform = new Dictionary<OuterRef<IEntity>, Transform>();
            BoxShapeDef weaponCollider = default;
            float boundingRadius = 0;
            foreach (var pair in casterAndTargetsAround)
                casterAndTargetsTransform.Add(pair.Key, default);

            var targetsBoundingRadius = new Dictionary<OuterRef<IEntity>, float>();
            foreach (var pair in casterAndTargetsAround)
                if (pair.Key != _ownerRef)
                {
                    var targetColliders = casterAndTargetsAround[pair.Key].Def.Colliders;
                    if (targetColliders != null && targetColliders.Count > 0)
                        targetsBoundingRadius.Add(pair.Key, targetColliders.Max(x => x.Target.GetBoundingRadius() + x.Target.Position.Length));
                    else
                        targetsBoundingRadius.Add(pair.Key, 0);
                }

            var toRemove = new List<OuterRef<IEntity>>();

            for (int i = 0; i < trajectory.Keys.Count; i++)
            {
                if (_attacks.Count == 0)
                    break;

                var frame = trajectory.Keys[i];
                var frameTime = frame.Time * timeFactor;

                if (frameTime > trajectoryEndSec)
                    break;

                if (frameTime >= trajectoryStartSec)
                {
                    if (needUpdateCasterAndTargetsData)
                    {
                        var casterWeapon = await FillWorldObjectsTransformAndGetCasterWeapon(casterAndTargetsTransform, _ownerRef, _repository);
                        weaponCollider = getWeaponCollider(casterWeapon, casterAndTargetsAround[_ownerRef].Def, colliderMarker);
                        boundingRadius = trajectory.BoundingSphereRadius + weaponCollider.Extents.Length * 2;
                        casterTransform = casterAndTargetsTransform[_ownerRef];
                        needUpdateCasterAndTargetsData = false;
                    }

                    var weaponColliderCenterPosition = casterTransform.Position + casterTransform.Rotation * (frame.Position + frame.Rotation * weaponCollider.Position);
                    var weaponColliderRotation = casterTransform.Rotation * frame.Rotation;

#if UNITY_EDITOR
                    var casterColliders = casterAndTargetsAround[_ownerRef].Def.Colliders;
                    {
                        //CASTER
                        foreach (var casterCollider in casterColliders)
                        {
                            if (casterCollider.Target is SphereShapeDef)
                            {
                                var casterDef = new SphereShapeDef();
                                casterDef.Position = casterTransform.Position + casterTransform.Rotation * casterCollider.Target.Position;
                                casterDef.Radius = ((SphereShapeDef) casterCollider).Radius;
                                EntitytObjectsUnitySpawnService.SpawnService.DrawShape(casterDef, Color.black, 0.1f);
                            }
                        }
                    }

                    {
                        //FRAME POINT
                        var frameDef = new BoxShapeDef();
                        frameDef.Position = casterTransform.Position + casterTransform.Rotation * frame.Position;
                        frameDef.Extents = new Vector3(0.015f, 0.05f, 0.015f);
                        frameDef.Rotation = (casterTransform.Rotation * frame.Rotation).eulerAngles;
                        EntitytObjectsUnitySpawnService.SpawnService.DrawShape(frameDef, Color.red, 0.1f);
                    }
#endif
                    toRemove.Clear();
                    foreach (var pair in casterAndTargetsAround)
                    {
                        if (pair.Key == _ownerRef)
                            continue;

                        var targetTransform = casterAndTargetsTransform[pair.Key];
                        var boundingTargetRadius = targetsBoundingRadius[pair.Key];
                        if (Vector3.GetSqrDistance(targetTransform.Position, casterTransform.Position) > (boundingRadius + boundingTargetRadius) * (boundingRadius + boundingTargetRadius))
                            continue;

                        var targetColliders = pair.Value.Def.Colliders;
                        if (targetColliders != null && targetColliders.Count > 0)
                            foreach (var targetCollider in targetColliders)
                            {
                                var intersect = false;
                                var targetCenterPosition =
                                    targetTransform.Position + targetTransform.Rotation * targetCollider.Target.Position;
                                if (targetCollider.Target is SphereShapeDef)
                                {
                                    intersect = GeometryHelpers.CheckIntersectOBBandSphere(weaponColliderCenterPosition,
                                        weaponCollider.Extents, weaponColliderRotation, targetCenterPosition,
                                        ((SphereShapeDef) targetCollider.Target).Radius);
                                }

                                if (intersect)
                                {
                                    OnAttackHitDetected(attackId, pair.Key);
                                    toRemove.Add(pair.Key);
                                    break;
                                }
                            }
                    }

                    Update();

                    foreach (var outerRef in toRemove)
                    {
                        casterAndTargetsAround.Remove(outerRef);
                        casterAndTargetsTransform.Remove(outerRef);
                    }

                    if (casterAndTargetsAround.Count <= 1) //Не осталось целей для атаки
                        break;
                }

                if (i != trajectory.Keys.Count - 1)
                {
                    var delayTime = spellStartTime + SyncTime.FromSeconds(trajectory.Keys[i + 1].Time * timeFactor) - SyncTime.Now;
                    if (delayTime > 0)
                    {
                        needUpdateCasterAndTargetsData = true;
                        const float minDelaySeconds = 0.1f;//Апдейтимся не чаще чем 10 раз в секунду
                        await Task.Delay(Math.Max((int)delayTime, (int)SyncTime.FromSeconds(minDelaySeconds)));
                    }
                }
            }
            //sw.Stop();
            //Logger.Error("!!StartAttackInternal work time {0} sec timeOffset {1} sec trajectoryStartSec {2} trajectoryEndSec {3} spellStartTime:{4} keysList:{5}",
            //    sw.Elapsed.TotalSeconds, timeOffset, trajectoryStartSec, trajectoryEndSec, spellStartTime, keysList);
        }


        public static async ValueTask<ItemResource> FillWorldObjectsTransformAndGetCasterWeapon(Dictionary<OuterRef<IEntity>, Transform> targetsTransform, OuterRef<IEntity> casterRef, IEntitiesRepository repo)
        {
            ItemResource casterWeapon = default;
            var batch = EntityBatch.Create();
            foreach (var outerRef in targetsTransform.Keys)
                batch.Add(outerRef.TypeId, outerRef.Guid);

            using (var wrapper = await repo.Get(batch))
            {
                foreach (var outerRef in targetsTransform.Keys.ToList())
                {
                    if (outerRef == casterRef)
                    {
                        var hasDollEntity = wrapper.Get<IHasDollClientBroadcast>(outerRef, ReplicationLevel.ClientBroadcast);
                        if (hasDollEntity != null)
                            casterWeapon = (ItemResource)ClusterHelpers.GetActiveWeaponResource(hasDollEntity).Key;
                    }

                    var positioned = PositionedObjectHelper.GetPositioned(wrapper, outerRef.TypeId, outerRef.Guid);
                    if (positioned == null)
                    {
                        Logger.IfWarn()?.Message($"ImpactInShape: Unexpected - OuterRef({outerRef}) is not {nameof(IPositionedObjectClientBroadcast)}.").Write();
                        continue;
                    }

                    targetsTransform[outerRef] = positioned.Transform;


                }
            }

            return casterWeapon;
        }

        public void Dispose()
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cont = await _repository.Get(_ownerRef))
                    if (cont.TryGet<IHasAttackEngineClientFull>(_ownerRef.TypeId, _ownerRef.Guid, ReplicationLevel.ClientFull, out var hae))
                        if (hae.AttackEngine != null)
                            await cont.Get<IHasAttackEngineClientFull>(_ownerRef, ReplicationLevel.ClientFull).AttackEngine.UnsetAttackDoer(this);
            }, _repository);
        }

        public void FinishAttack(SpellPartCastId attackId, TimeRange timeRange, long currentTime)
        {
            FinishAttackInternal(attackId, timeRange, currentTime);
        }

        private void FinishAttackInternal(SpellPartCastId attackId, TimeRange timeRange, long currentTime)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Finish | TimeRange:{timeRange.ToString(currentTime)} AttackId:{attackId}").Write();
            Collect.IfActive?.EventEnd(attackId);

            if (_attacks.TryGetValue(attackId, out var attack))
            {
                //var timeOffset = timeRange - attack.AnimInfo.StartTime;
                //float trajectoryEndSec = (SyncTime.ToSecondsSafe(timeOffset.Finish) + attack.AnimInfo.AnimationOffset) * attack.AnimInfo.SpeedFactor;
                if (attack.TargetsBatch?.Count > 0)
                    PushTargets(new List<AttackSession> { attack });
                _attacks.TryRemove(attackId, out var _);
            }
        }
        
        private void OnAttackHitDetected(SpellPartCastId attackId, OuterRef<IEntity> targetRef)
        {

            var currentTime = SyncTime.Now;

            if (_attacks.Count == 0)
            {
                Logger.IfWarn()?.Message("Attack hit detected while no active attacks exists").Write();
                return;
            }


            if (_attacks.TryGetValue(attackId, out var attack))
            {
                if (attack.KnownTargets == null || !attack.KnownTargets.Contains(targetRef))
                {
                    if (attack.TargetsCount < attack.Def.TargetsLimit)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Attack Target Added | TargetRef:{targetRef} AttackId:{attackId}").Write();
                        //Collect.IfActive?.Event("AttackDoer.HitDetected", _ownerRef);
                        var targetInfo = new AttackTargetInfo
                        {
                            Target = targetRef,
                            //SubTarget = targetSubRef,
                            Timestamp = currentTime,
                            //HitPoint = hit.Point.ToShared(),
                            //HitLocalPoint = hit.LocalPoint.ToShared(),
                            //HitRotation = hit.Rotation.ToShared(),
                            //HitLocalRotation = hit.LocalRotation.ToShared(),
                            //HitObject = hit.HitObject?.transform != hit.Victim ? hit.HitObject.FullName(hit.Victim.transform) : null
                        };
                        attack.TargetsCount++;
                        (attack.TargetsBatch = attack.TargetsBatch ?? new List<AttackTargetInfo>()).Add(targetInfo);
                        (attack.KnownTargets = attack.KnownTargets ?? new HashSet<OuterRef<IEntity>>()).Add(targetRef);
                    }
                    //else
                    //    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Attack Limit exceeded | Target:{hit.Victim} TargetRef:{targetRef} Attack:{attack}").Write();
                }

            }

        }

        public void Update()
        {
            List<AttackSession> attacksToPush = null;
            var batchTime = Constants.AttackConstants.TargetsBatchTimeout;
            foreach (var attack in _attacks)
                if (attack.Value.TargetsBatch != null && attack.Value.TargetsBatch.Count > 0 && SyncTime.InThePast(attack.Value.TargetsBatch[0].Timestamp + batchTime))
                    (attacksToPush = attacksToPush ?? new List<AttackSession>()).Add(attack.Value);
            if (attacksToPush != null)
                PushTargets(attacksToPush);
        }

        private void PushTargets(List<AttackSession> attacks)
        {
            var attacksToPush = new List<AttackToPush>(attacks.Count);
            foreach (var attack in attacks)
            {
                attacksToPush.Add(new AttackToPush { Id = attack.Id, TargetsBatch = attack.TargetsBatch });
                attack.TargetsBatch = null;
            }

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cont = await _repository.Get(_ownerRef))
                {
                    var attacker = cont.Get<IHasAttackEngineClientFull>(_ownerRef, ReplicationLevel.ClientFull);
                    foreach (var attack in attacksToPush)
                    {
                        //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Push | {nameof(attack.Id)}:{attack.Id} {nameof(attack.TargetsBatch)}:({string.Join(", ", attack.TargetsBatch)})").Write();
                        //Collect.IfActive?.Event("AttackDoer.HitPushed", _ownerRef);
                        await attacker.AttackEngine.PushAttackTargets(attack.Id, attack.TargetsBatch);
                    }
                }
            }, _repository);
        }

        private class AttackSession
        {
            public readonly SpellPartCastId Id;
            public readonly AttackDef Def;
            public int TargetsCount;
            public HashSet<OuterRef<IEntity>> KnownTargets;
            public List<AttackTargetInfo> TargetsBatch;
            public AttackAnimationInfo AnimInfo;

            public AttackSession(SpellPartCastId id, AttackDef def, AttackAnimationInfo animInfo)
            {
                Id = id;
                Def = def;
                AnimInfo = animInfo;
            }
        }

        private struct AttackToPush
        {
            public SpellPartCastId Id;
            public List<AttackTargetInfo> TargetsBatch;
        }
    }
}
