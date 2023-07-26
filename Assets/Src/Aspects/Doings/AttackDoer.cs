using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.Damage.Templates;
using Assets.Src.Character.Events;
using Assets.Src.FX.Decals;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using Assets.Tools;
using ColonyShared.GeneratedCode.Combat;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Aspects.Misc;
using ColonyShared.SharedCode.Entities.Reactions;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ResourceSystem.Aspects.Misc;
using ResourceSystem.Reactions;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.EntitySystem;
using SharedCode.Extensions;
using SharedCode.Serializers;
using SharedCode.Utils.DebugCollector;
using Src.Locomotion.Unity;
using Src.ManualDefsForSpells;
using Src.Tools;
using UnityEngine;
using static UnityQueueHelper;

namespace Src.Aspects.Doings
{
    public class AttackDoer : IAttackDoer
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(AttackDoer));
       
        private readonly OuterRef<IHasAttackEngineClientFull> _ownerRef;
        private readonly IEntitiesRepository _repository;
        private readonly AttackEventSubscriptionHandler[] _attackHandlers;
        private readonly IAttackDoerSupport _support;
        private readonly List<AttackSession> _attacks = new List<AttackSession>();
        private readonly List<SpellPartCastId> _prematurelyAttacks = new List<SpellPartCastId>();
        private readonly Transform _body;
        private readonly GameObject _root;
        private readonly List<AttackSession> _attacksToPush = new List<AttackSession>();


        public AttackDoer(
            OuterRef<IHasAttackEngineClientFull> ownerRef,
            IEntitiesRepository repository, 
            IAttackDoerSupport support,
            IEnumerable<AttackEventSubscriptionHandler> attackHandlers,
            Transform body)
        {
            _attackHandlers = (attackHandlers ?? throw new ArgumentNullException(nameof(attackHandlers))).ToArray();
            _support = support ?? throw new ArgumentNullException(nameof(support)); 
            _ownerRef = ownerRef.IsValid ? ownerRef : throw new ArgumentException(nameof(ownerRef));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _body = body ? body : throw new ArgumentNullException(nameof(body));
            _root = _body.gameObject.GetRoot();
            foreach (var attackHandler in _attackHandlers)
                attackHandler.AttackHitDetected += OnAttackHitDetected;            
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cont = await _repository.Get(_ownerRef))
                    await cont.Get(_ownerRef, ReplicationLevel.ClientFull).AttackEngine.SetAttackDoer(this);
            }, _repository);
        }

        public void Dispose()
        {
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    using (var container = await _repository.Get(_ownerRef))
                        if (container != null)
                        {
                            var attackEngine = container.Get(_ownerRef, ReplicationLevel.ClientFull);
                            if (attackEngine != null)
                                await attackEngine.AttackEngine.UnsetAttackDoer(this);
                            else
                                Logger.IfWarn()?.Message("Can't UnsetAttackDoer No AttackEngine").Write();
                        }
                        else
                            Logger.IfWarn()?.Message("Can't UnsetAttackDoer No Owner").Write();
                },
                _repository);            
        }

        public void StartClientSideAttack(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, object animKey, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef)
        {
            RunInUnityThread(() => StartAttackInternal(attackId, def, colliderMarker, animKey, timeRange, currentTime).WrapErrors());
        }

        public void StartServerSideAttack(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, List<ResourceRef<GameObjectMarkerDef>> trajectoryMarkers, AnimationStateDef animation, TimeRange timeRange, long currentTime, long spellStartTime, float spellDuration, OuterRef<IEntity> targetRef)
        {
            throw new InvalidOperationException("This method must not be called on the client attack doer"); 
        }

        private async Task StartAttackInternal(SpellPartCastId attackId, AttackDef def, GameObjectMarkerDef colliderMarker, object animKey, TimeRange timeRange, long currentTime)
        {
            AssertInUnityThread();
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Start | TimeRange:{timeRange.ToString(currentTime)} AttackId:{attackId} AttackDef:{def.____GetDebugAddress()} PlayId:{animKey}").Write();
            Collect.IfActive?.EventBgn("AttackDoer.Attack", _ownerRef, attackId);

            var animInfoTask = _support.FetchAttackAnimationInfo(animKey);
            await Task.WhenAny(animInfoTask, Task.Delay(500));
            AssertInUnityThread();

            if (!animInfoTask.IsCompleted)
            {
                Logger.IfError()?.Message($"There are no animation play with Id:{animKey}").Write();
                throw new KeyNotFoundException($"There are no animation play with Id:{animKey}");
            }

            var attack = new AttackSession(attackId, def, animInfoTask.Result);
            var attackIndex = _attacks.Count;
           _attacks.Add(attack);

            var timeOffset = timeRange - attack.AnimInfo.StartTime + Constants.AttackConstants.AttackColliderLookAhead;
            float trajectoryStartSec = (SyncTime.ToSecondsSafe(timeOffset.Start) + attack.AnimInfo.AnimationOffset) * attack.AnimInfo.SpeedFactor;
            float trajectoryEndSec = (SyncTime.ToSecondsSafe(timeOffset.Finish) + attack.AnimInfo.AnimationOffset) * attack.AnimInfo.SpeedFactor;
            
            if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"Attack Info | Id:{attack.Id} Def:{attack.Def} AnimState:{attack.AnimInfo.State.____GetDebugAddress()} AnimStartAt:{attack.AnimInfo.StartTime} AnimOffset:{attack.AnimInfo.AnimationOffset}  AnimSpeed:{attack.AnimInfo.SpeedFactor} TimeOffset:{timeOffset} TrjStart:{trajectoryStartSec} TrjEnd:{trajectoryEndSec}").Write();
 
            foreach (var attackHandler in _attackHandlers)
            {
                if (_support.TryGetTrajectory(attack.AnimInfo.State, attackHandler.GetMarker(), out var trajectory))
                {
                    attackHandler.ActivateColliders(attackId, _body, trajectory, colliderMarker, trajectoryStartSec, trajectoryEndSec, attack.AnimInfo.SpeedFactor);
                    (attack.ActiveHandlers = attack.ActiveHandlers ?? new List<AttackEventSubscriptionHandler>()).Add(attackHandler); 
                }
            }

            if(attack.ActiveHandlers == null)
                Logger.IfError()?.Message($"No active attack handlers. AttackDef:{def} AttackId:{attackId}").Write();
 
            if (_prematurelyAttacks.Contains(attackId))
            {
                _prematurelyAttacks.Remove(attackId);
                FinishAttackInternal(attackId, attack, trajectoryEndSec);
                _attacks.RemoveAt(attackIndex);
            }
        }

        public void FinishAttack(SpellPartCastId attackId, TimeRange timeRange, long currentTime)
        {
            RunInUnityThread(() => FinishAttackSync(attackId, timeRange, currentTime));
        }

        private void FinishAttackSync(SpellPartCastId attackId, TimeRange timeRange, long currentTime)
        {
            AssertInUnityThread();
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Finish | TimeRange:{timeRange.ToString(currentTime)} AttackId:{attackId}").Write();
            Collect.IfActive?.EventEnd(attackId);

            var idx = _attacks.FindIndex(x => x.Id == attackId);
            if (idx != -1)
            {
                var attack = _attacks[idx];
                var timeOffset = timeRange - attack.AnimInfo.StartTime;
                float trajectoryEndSec = (SyncTime.ToSecondsSafe(timeOffset.Finish) + attack.AnimInfo.AnimationOffset) * attack.AnimInfo.SpeedFactor;
                FinishAttackInternal(attackId, attack, trajectoryEndSec);
                _attacks.RemoveAt(idx);
            }
            else
            {
                _prematurelyAttacks.Add(attackId);
            }
        }

        private void FinishAttackInternal(SpellPartCastId attackId, AttackSession attack, float trajectoryEndSec)
        {
            if (attack.ActiveHandlers != null)
                foreach (var attackHandler in attack.ActiveHandlers)
                    attackHandler.DeactivateColliders(attackId, trajectoryEndSec);

            if (attack.TargetsBatch?.Count > 0)
                PushTargets(new List<AttackSession> {attack});
        }
        
        private void OnAttackHitDetected(SpellPartCastId attackId, in HitInfo hit)
        {
            AssertInUnityThread();
            SharedCode.Logging.Log.AttackStopwatch.Restart();
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Attack Hit | Target:{hit.Victim?.name} AttackId:{attackId}").Write();
            
            var currentTime = SyncTime.Now;
            
            if (_attacks.Count == 0)
            {
                Logger.IfWarn()?.Message("Attack hit detected while no active attacks exists").Write();
                return;
            }

            if (hit.Victim == null)
            {
                 Logger.IfError()?.Message("hit._collidedObject is null").Write();;
                return;
            }

            OuterRef targetRef = OuterRef.Invalid;
            Guid targetSubRef = Guid.Empty;
            
            var targetComp = hit.Victim.GetComponent<IAttackTargetComponent>();
            if (targetComp != null)
            {
                targetRef = targetComp.EntityId;
                targetSubRef = targetComp.SubObjectId;
            }
            else
            {
                var targetEgo = hit.Victim.GetComponent<EntityGameObject>();
                if (targetEgo != null)
                {
                    targetRef = targetEgo.OuterRefEntity.To();
                }
            }

            if (!targetRef.IsValid)
                return;
            
            if (_ownerRef.Guid == targetRef.Guid)
                return;
            
            //TODO: raycast check must be here

            AttackSession attack = null;
            for (int i = _attacks.Count - 1; i >= 0; --i)
                if (_attacks[i].Id == attackId)
                {
                    attack = _attacks[i];
                    break;
                }

            if(attack != null)
            {
                if (attack.KnownTargets == null || !attack.KnownTargets.Contains(targetRef))
                {
                    if (attack.TargetsCount < attack.Def.TargetsLimit)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Attack Target Added | TargetRef:{targetRef} AttackId:{attackId}").Write();
                        Collect.IfActive?.Event("AttackDoer.HitDetected", _ownerRef);
                        var targetInfo = new AttackTargetInfo
                        {
                            Target = targetRef,
                            SubTarget = targetSubRef,
                            Timestamp = currentTime,
                            HitPoint = hit.Point.ToShared(),
                            HitLocalPoint = hit.LocalPoint.ToShared(),
                            HitRotation = hit.Rotation.ToShared(),
                            HitLocalRotation = hit.LocalRotation.ToShared(),
                            HitObject = hit.HitObject?.transform != hit.Victim ? hit.HitObject.FullName(hit.Victim.transform) : null
                        };
                        attack.TargetsCount++;
                        (attack.TargetsBatch = attack.TargetsBatch ?? new List<AttackTargetInfo>()).Add(targetInfo);
                        (attack.KnownTargets = attack.KnownTargets ?? new HashSet<OuterRef>()).Add(targetRef);
                    }
                    else 
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Attack Limit exceeded | Target:{hit.Victim} TargetRef:{targetRef} Attack:{attack}").Write();
                }
                
                PlaceDecails(hit);
            }

            Update();
        }

        public void Update()
        {
            var batchTime = Constants.AttackConstants.TargetsBatchTimeout;
            _attacksToPush.Clear();
            foreach (var attack in _attacks)
                if (attack.TargetsBatch != null && attack.TargetsBatch.Count > 0 && SyncTime.InThePast(attack.TargetsBatch[0].Timestamp + batchTime))
                    _attacksToPush.Add(attack);
            if (_attacksToPush.Count > 0)
                PushTargets(_attacksToPush);
        }

        private void PushTargets(List<AttackSession> attacks)
        {
            var attacksToPush = new List<AttackToPush>(attacks.Count);
            foreach (var attack in attacks)
            {
                attacksToPush.Add(new AttackToPush {Id = attack.Id, TargetsBatch = attack.TargetsBatch});
                attack.TargetsBatch = null;
            }

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var cont = await _repository.Get(_ownerRef))
                {
                    var attacker = cont.Get(_ownerRef, ReplicationLevel.ClientFull);
                    SharedCode.Logging.Log.AttackStopwatch.Milestone("Push Targets");
                    foreach (var attack in attacksToPush)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Push | {nameof(attack.Id)}:{attack.Id} {nameof(attack.TargetsBatch)}:({string.Join(", ", attack.TargetsBatch)})").Write();
                        Collect.IfActive?.Event("AttackDoer.HitPushed", _ownerRef);
                        await attacker.AttackEngine.PushAttackTargets(attack.Id, attack.TargetsBatch);
                    }
                }
            });
        }
        
        private void PlaceDecails(in HitInfo hitInfo)
        {
            // Декали
            var fxInfo = hitInfo.Victim.GetComponentInChildren<FxMetaInfo>();
            if (fxInfo && fxInfo.FxResourceSet)
            {
                var fxResourceSet = fxInfo.FxResourceSet.Get<FxResourceSetDef>();
                if (fxResourceSet != default(FxResourceSetDef))
                {
                    foreach (var fxResource in fxResourceSet._fxResources)
                        if (hitInfo.DamageType != default(DamageTypeDef) && fxResource._damageType == hitInfo.DamageType)
                            HitDecalPlacer.PlaceDecal(fxResource, hitInfo);
                }
            }
        }
        
        private class AttackSession
        {
            public readonly SpellPartCastId Id;
            public readonly AttackDef Def;
            public readonly AttackAnimationInfo AnimInfo;
            public int TargetsCount;
            public HashSet<OuterRef> KnownTargets;
            public List<AttackTargetInfo> TargetsBatch;
            public List<AttackEventSubscriptionHandler> ActiveHandlers;

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

        private static ArgTuple[] MapHitToEventParams(HitInfo hit, HitFXEventType.ParamsBundle @params)
        {
            return new[]
            {
                ArgTuple.Create(@params.HitPoint, ArgValue.Create(hit.Point.ToShared())),
                ArgTuple.Create(@params.HitRotation, ArgValue.Create(hit.Rotation.ToShared())),
                ArgTuple.Create(@params.HitLocalPoint, ArgValue.Create(hit.LocalPoint.ToShared())),
                ArgTuple.Create(@params.HitLocalRotation, ArgValue.Create(hit.LocalRotation.ToShared())),
                ArgTuple.Create(@params.HitObject, ArgValue.Create(hit.HitObject.FullName())),
                ArgTuple.Create(@params.DamageType, ArgValue.Create((BaseResource)hit.DamageType))
            };
        }
    }
}