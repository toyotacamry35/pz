using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects;
using ColonyShared.SharedCode.Aspects.Combat;
using ColonyShared.SharedCode.Aspects.Misc;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ResourceSystem.Aspects;
using ResourceSystem.Entities;
using ResourceSystem.Utils;
using SharedCode.Aspects.Item.Templates;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities;
using SharedCode.Entities.Building;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;
using SharedCode.Wizardry;
using SharedCode.EntitySystem.Delta;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace GeneratedCode.DeltaObjects
{
    public partial class AttackEngine : IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(AttackEngine));
        private static readonly NLog.Logger ValidationLogger = LogManager.GetLogger("AttackValidation");

        private IAttackDoer _attackDoer;

        private IHasWizardEntity OwnerWithWizard => (parentDeltaObject as IHasWizardEntity) ?? (parentEntity as IHasWizardEntity);
        private IPositionedObject OwnerWithPosition => PositionedObjectHelper.GetPositioned(parentEntity);
        private IEntity OwnerEntity => (parentDeltaObject as IEntity) ?? (parentEntity as IEntity);

        
        public Task<bool> StartAttackImpl(SpellPartCastId attackId, long finishTime, AttackDef attackDef, IReadOnlyList<AttackModifierDef> attackModifiers)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Start Attack | {nameof(attackId)}:{attackId} {nameof(attackDef)}:{attackDef.____GetDebugAddress()} {nameof(finishTime)}:{finishTime}").Write();

// атака вполне может быть "бесконечной". например что-нибуть типа огнемёта или бензопилы, что дамажит пока нажата кнопка.
//            var currentTime = SyncTime.Now;
//            if (finishTime > currentTime + Constants.AttackConstants.AttackDurationLimit)
//            {
//                if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message($"{attackDef.____GetDebugAddress()} | Attack duration limit exceeded: ({finishTime - currentTime}) | ").Write();
//                finishTime = currentTime + Constants.WorldConstants.AttackDurationLimit;
//            }
            var currentTime = SyncTime.Now;

            CleanupExpiredData();
            if (Attacks.Count >= Constants.AttackConstants.SimultaneousAttacksLimit)
            {
                if (Logger.IsWarnEnabled) Logger.IfWarn()?.Message($"{attackDef.____GetDebugAddress()} | Simultaneous attacks limit exceeded").Write();
                return Task.FromResult(false);
            }

            // проверяем наличие целей для этой новой атаки в OrphanTargets
            int processedTargetsCount = 0;
            int targetsLimit = attackDef.TargetsLimit;
            for (int i = OrphanTargets.Count - 1; i >= 0; --i)
            {
                var orphan = OrphanTargets[i];
                if (orphan.AttackId == attackId)
                {
                    if (processedTargetsCount >= targetsLimit)
                        break;
                    processedTargetsCount++;
                    OrphanTargets.RemoveAt(i);
                    ProcessTarget(orphan.TargetInfo, attackDef, attackModifiers, currentTime);
                }
            }

            Attacks.Add(new AttackInfo(attackDef, attackId, attackModifiers, finishTime, processedTargetsCount));
            
            return Task.FromResult(true);
        }

        public Task FinishAttackImpl(SpellPartCastId attackId, long currentTime)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Finish Attack | {nameof(attackId)}:{attackId} {nameof(currentTime)}:{currentTime}").Write();

            CleanupExpiredData();
            for (var i = 0; i < Attacks.Count; ++i)
            {
                var attack = Attacks[i];
                if (attack.Id == attackId)
                {
                    if (currentTime < attack.FinishTime)
                        Attacks[i] = new AttackInfo(attack, currentTime);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }

        public Task PushAttackTargetsImpl(SpellPartCastId attackId, List<AttackTargetInfo> targets)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Push targets | {nameof(attackId)}:{attackId} {nameof(targets)}:({string.Join(", ", targets)})").Write();

            CleanupExpiredData();

            long currentTime = SyncTime.Now;
            for (var i = 0; i < Attacks.Count; ++i)
            {
                var attack = Attacks[i];
                if (attack.Id == attackId)
                {
                    var targetsLimit = attack.Def.TargetsLimit;
                    for (var t = 0; t < targets.Count; ++t)
                    {
                        if (attack.ProcessedTargetsCount >= targetsLimit)
                            break;
                        Attacks[i] = new AttackInfo(attack, attack.ProcessedTargetsCount + 1);
                        ProcessTarget(targets[t], attack.Def, attack.Modifiers, currentTime);
                    }
                    return Task.CompletedTask;
                }
            }

            long tolerance = Constants.AttackConstants.AttackTimeTolerance;
            foreach (var target in targets)
                if (!SyncTime.InThePast(target.Timestamp, currentTime - tolerance))
                    OrphanTargets.Add(new AttackOrphanTargetInfo(new AttackTargetInfo(target) { Timestamp = Min(target.Timestamp, currentTime) }, attackId));
            
            return Task.CompletedTask;
        }

        private void ProcessTarget(AttackTargetInfo target, AttackDef attackDef, IReadOnlyList<AttackModifierDef> modifiers, long currentTime)
        {
            var repo = parentEntity.EntitiesRepository;
            var positionHistory = OwnerWithPosition.History;
            AsyncUtils.RunAsyncTask(async () =>
            {
                if (await ValidateTarget(target, attackDef, positionHistory, currentTime))
                {
                    await DamagePipelineHelper.ExecuteStrike(
                        aggressorRef: new OuterRef(OwnerEntity.Id, OwnerEntity.TypeId),
                        victimInfo: target,
                        repository: repo,
                        attackTimestamp: target.Timestamp,
                        attackDesc: attackDef,
                        attackModifiers: modifiers
                    );
                }
                else if (Logger.IsDebugEnabled)
                    Logger.IfDebug()?.Message($"Attack validation failed for target:{target}").Write();
            }, repo);
        }

        private async Task<bool> ValidateTarget(AttackTargetInfo target, AttackDef attackDef, IPositionHistory ownerPositionHistory, long currentTime)
        {
            int iterations = Constants.AttackConstants.ValidationTimeIterations;
            if (iterations <= 0)
                return true;

            using (var cont = await parentEntity.EntitiesRepository.Get(target.Target.TypeId, target.Target.Guid))
            {
                var buildCollection = cont.Get<IBuildCollection>(target.Target.TypeId, target.Target.Guid);
                if (buildCollection != null)  // костыль для атаки по зданиям которые сделаны полностью ортогонально всему, что у нас есть
                {
                    var buildElement = await buildCollection.TryGetValue(BuildType.Any, target.SubTarget);
                    if (buildElement == null)
                    {
                        Logger.IfError()?.Message($"Can't get build element:{target.SubTarget} in build collection:{target.Target}").Write();
                        return false;
                    }
                    return ValidateTarget(attackDef, buildElement.PositionHistory, Constants.AttackConstants.DefaultBuildingBounds.Target, target.Timestamp, ownerPositionHistory, currentTime, 1);
                }
                else
                {
                    var targetEnt = cont.Get<IEntity>(target.Target.TypeId, target.Target.Guid);
                    var targetPositioned = PositionedObjectHelper.GetPositioned(targetEnt);
                    if (targetPositioned == null)
                    {
                        if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Target:{target.Target} is not {nameof(IPositionedObject)}").Write();
                        return false;
                    }
                    var targetObject = cont.Get<IEntityObject>(target.Target.TypeId, target.Target.Guid);
                    var targetBoundsDef = (targetObject?.Def as IHasBoundsDef)?.Bounds ?? Constants.AttackConstants.DefaultBounds.Target;
                    return ValidateTarget(attackDef, targetPositioned.History, targetBoundsDef, target.Timestamp, ownerPositionHistory, currentTime, Constants.AttackConstants.ValidationTimeIterations);
                }
            }
        }
        
        private bool ValidateTarget(AttackDef attackDef,
            IPositionHistory targetPositionHistory,
            CapsuleDef targetBoundsDef,
            long targetTimestamp,
            IPositionHistory ownerPositionHistory,
            long currentTime, 
            int iterations)
        {
            if (iterations <= 0)
                return true;

            var distanceLimit = Sqr(attackDef.DistanceLimit);
            var checkBoundsRadius = targetBoundsDef.Radius + distanceLimit;
            var checkBoundsHeight = targetBoundsDef.Height + distanceLimit * 2;
            long pingToTarget = 0; // TODO: здесь должен быть реальный ping от клиента к серверу 
            long refTimestamp = Min(currentTime, targetTimestamp);
            long minTimestamp = iterations > 1 ? refTimestamp - Constants.AttackConstants.ValidationTimeInterval / 2 : refTimestamp;
            long timeStep = iterations > 1 ? Constants.AttackConstants.ValidationTimeInterval / (iterations - 1) : 0;
            long prevTimestamp = -1;
            float minDistance = float.MaxValue;
            bool rv = false;
            for (int i = 0; !rv && i < iterations; ++i)
            {
                var timestamp = Min(minTimestamp + i * timeStep, currentTime);
                if (timestamp == prevTimestamp)
                    break;
                prevTimestamp = timestamp;
                var ownerTransform = ownerPositionHistory.GetTransformAt(timestamp);
                var targetTransform = targetPositionHistory.GetTransformAt(timestamp - pingToTarget);
                var checkBoundsCenter = new Vector3(targetTransform.Position.x, targetTransform.Position.y + targetBoundsDef.OffsetY + targetBoundsDef.Height * 0.5f, targetTransform.Position.z);
                if(CheckInsideCapsule( ownerTransform.Position, checkBoundsCenter, checkBoundsRadius, checkBoundsHeight, CapsuleDirection.Y))
                    rv = true;
                if (ValidationLogger.IsDebugEnabled)
                    minDistance = Min(DistanceToCapsule(ownerTransform.Position, checkBoundsCenter, targetBoundsDef.Radius, targetBoundsDef.Height, CapsuleDirection.Y), minDistance);
            }

            if (ValidationLogger.IsDebugEnabled)
                ValidationLogger.IfDebug()?.Message("{value}", new ValidationInfo {AttackDef = attackDef.____GetDebugAddress(), Distance = minDistance, Success = rv} ).Write();
            
            if (!rv && Logger.IsDebugEnabled) 
                Logger.IfDebug()?.Message($"Attack validation failed | Distance:{minDistance}").Write();

            return rv;
        }

        private void CleanupExpiredData()
        {
            long currentTime = SyncTime.Now;
            long currentTimeWithTolerance = currentTime - Constants.AttackConstants.AttackTimeTolerance;

            // Удаляем завершившиеся атаки 
            for (int i = Attacks.Count - 1; i >= 0; --i)
            {
                var attack = Attacks[i];
                if (SyncTime.InThePast(attack.FinishTime, currentTimeWithTolerance))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.IfDebug()?.Message($"Remove finished attack | {attack} {nameof(currentTime)}:{currentTime}").Write();
                    Attacks.RemoveAt(i);
                }
            }

            // Удаляем цели которые пришли не вовремя  
            for (int i = OrphanTargets.Count - 1; i >= 0; --i)
            {
                var orphan = OrphanTargets[i];
                if (SyncTime.InThePast(orphan.TargetInfo.Timestamp, currentTimeWithTolerance))
                {
                    if (Logger.IsDebugEnabled)
                        Logger.IfDebug()?.Message(
                            $"Remove orfaned attack targets | {orphan} {nameof(currentTime)}:{currentTime}")
                            .Write();
                    OrphanTargets.RemoveAt(i);
                }
            }
        }

        struct ValidationInfo
        {
            public bool Success { get; set; }
            public string AttackDef { get; set; }
            public float Distance { get; set; }
        }

        public IAttackDoer AttackDoer => _attackDoer;

        public Task SetAttackDoerImpl(IAttackDoer newDoer)
        {
            _attackDoer = newDoer ?? throw new ArgumentNullException(nameof(newDoer));
            return Task.CompletedTask;
        }

        public Task UnsetAttackDoerImpl(IAttackDoer oldDoer)
        {
            Interlocked.CompareExchange(ref _attackDoer, null, oldDoer ?? throw new ArgumentNullException(nameof(oldDoer)));
            return Task.CompletedTask;
        }

        public Task OnDatabaseLoad()
        {
            if (Attacks == null)
                Attacks = new DeltaList<AttackInfo>();
            if(OrphanTargets == null)
                OrphanTargets = new DeltaList<AttackOrphanTargetInfo>();
            return Task.CompletedTask;
        }
    }
}