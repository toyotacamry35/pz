using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Wizardry;
using ColonyShared.ManualDefsForSpells;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using JetBrains.Annotations;
using NLog;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using UnityEngine;
using SVector3 = SharedCode.Utils.Vector3;

namespace Src.Effects
{
    [UsedImplicitly, PredictableEffect]
    public class EffectDirectMotion : IClientOnlyEffectBinding<EffectDirectMotionDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        public async ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDirectMotionDef indef)
        {
            //Logger.IfError()?.Message($"EFFECT ATTACH {DateTime.Now}").Write();
            //Logger.IfError()?.Message($"{repo.TryGetLockfree<IHasMobMovementSyncAlways>(cast.Caster, ReplicationLevel.Always)?.MovementSync.PathFindingOwnerRepositoryId == repo.Id} {repo.TryGetLockfree<IMobMovementSyncAlways>(cast.Caster, ReplicationLevel.Always)?.PathFindingOwnerRepositoryId} {repo.Id}").Write();
            Guid pathGuid = default;
            using (var cnt = await repo.Get(cast.Caster))
            {
                pathGuid = cnt.Get<IHasMobMovementSyncAlways>(cast.Caster, ReplicationLevel.Always)
                    ?.MovementSync.PathFindingOwnerRepositoryId ?? default;
                
            }
            if (!cast.OnClientWithAuthority() && !(pathGuid == repo.Id))
                return;
            var def = indef;
            using (var cnt = await repo.Get(cast.Caster))
            {
                var locoOwner = cnt.Get<IHasLocomotionOwnerClientBroadcast>(cast.Caster, ReplicationLevel.ClientBroadcast)?.LocomotionOwner;
                var isValid = await locoOwner.IsValid();
                if (locoOwner != null && isValid)
                {
                    var loc = locoOwner.Locomotion;
                    var dmp = locoOwner.DirectMotionProducer;
                    var gp = locoOwner.GuideProvider;
                    var mover = def.Mover != null ? CreateMover(dmp, def.Mover, cast) : null;
                    var rotator = def.Rotator != null ? CreateRotator(dmp, def.Rotator, cast, gp) : null;
                    dmp.AddOrder(cast.WordCastId(def), mover, rotator);
                }
                else
                    if(Logger.IsWarnEnabled) Logger.IfWarn()?.Message($"Direct Motion Producer not found on {cast.GetCaster()} or invalid ({locoOwner?.IsValid()})").Write();
            }
        }

        public async ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectDirectMotionDef def)
        {
            cast.AssertIfNull(nameof(cast));
            repo.AssertIfNull(nameof(repo));

            Guid pathGuid = default;
            using (var cnt = await repo.Get(cast.Caster))
            {
                pathGuid = cnt.Get<IHasMobMovementSyncAlways>(cast.Caster, ReplicationLevel.Always)
                    ?.MovementSync.PathFindingOwnerRepositoryId ?? default;
            }
            if (!cast.OnClientWithAuthority() && !(pathGuid == repo.Id))
                return;
            using (var cnt = await repo.Get(cast.Caster))
            {
                var hasLocomotion = cnt.Get<IHasLocomotionOwnerClientBroadcast>(cast.Caster, ReplicationLevel.ClientBroadcast)?.LocomotionOwner;
                if (hasLocomotion != null)
                {
                    var dmp = hasLocomotion.DirectMotionProducer;
                    dmp?.RemoveOrder(cast.WordCastId(def));
                }
            }
        }

        private IMover CreateMover(IDirectMotionProducer dmp, EffectDirectMotionDef.MoverDef indef, SpellWordCastData cast)
        {
            switch (indef)
            {
                case EffectDirectMotionDef.NullMoverDef _:
                    return dmp.NoMovement();
                case EffectDirectMotionDef.AnimatorMoverDef def:
                    return dmp.AnimatorMovement(def.Factor);
                case EffectDirectMotionDef.CurveMoverDef def:
                {
                    return dmp.CurveMovement(def.Curve, def.VerticalCurve, def.Direction.Target.GetVector(cast), def.AdjustTime ? SyncTime.ToSeconds(cast.WordTimeRange.Duration) : 0, def.Factor);
                }
                default:
                    throw new NotSupportedException(indef.GetType().Name);
            }
        }

        private IRotator CreateRotator(IDirectMotionProducer dmp, EffectDirectMotionDef.RotatorDef indef, SpellWordCastData cast, IGuideProvider guideProvider)
        {
            switch (indef)
            {
                case EffectDirectMotionDef.NullRotatorDef _:
                    return dmp.NoRotation();
                case EffectDirectMotionDef.LookAtRotatorDef def:
                {
                    var proxy = new HasLookAnchorProxy(def.Target.Target?.GetGo(cast));
                    Func<SVector3?> targetFn = () => proxy.GetAnchor().Anchor;
                    return def.Time > 0 ? dmp.LookAtWithTime(targetFn, def.Time) : dmp.LookAtFixed(targetFn);
            }
                case EffectDirectMotionDef.BindToCameraRotatorDef def:
                    return dmp.DirectionWithSpeed(() => guideProvider?.Guide, def.Speed * SharedHelpers.Deg2Rad);
                case EffectDirectMotionDef.HardBindToCameraRotatorDef _:
                    return dmp.DirectionFixed(() => guideProvider?.Guide);
                default:
                    throw new NotSupportedException(indef.GetType().Name);
            }
        }

        private class HasLookAnchorProxy
        {
            private readonly GameObject _go;
            private IHasLookAtAnchor _hasLookAtAnchor;

            public HasLookAnchorProxy(GameObject go)
            {
                _go = go;
            }

            public IHasLookAtAnchor GetAnchor()
            {
                if (_hasLookAtAnchor == null)
                {
                    if (_go)
                    {
                        _hasLookAtAnchor = _go.GetComponentInChildren<IHasLookAtAnchor>() ?? new TransformAnchor(_go);
                    }
                    else
                    {
                        _hasLookAtAnchor = new EmptyAnchor();
                    }
                }
                return _hasLookAtAnchor; 
            }

            private class TransformAnchor : IHasLookAtAnchor
            {
                private readonly GameObject _go;
                public TransformAnchor(GameObject go) => _go = go;
                public SVector3? Anchor => _go ? (SVector3?)_go.transform.position : null;
            }

            private class EmptyAnchor : IHasLookAtAnchor
            {
                public SVector3? Anchor => null;
            }
        }
    }
}