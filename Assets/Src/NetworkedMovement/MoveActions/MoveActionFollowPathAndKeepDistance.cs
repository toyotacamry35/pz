using System;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using GeneratedDefsForSpells;
using Src.Locomotion;
using UnityEngine;
using UnityEngine.AI;
using static Assets.Src.NetworkedMovement.MoveActions.MoveActionHelper;
using static Assets.Src.NetworkedMovement.MoveActions.MoveActionResult;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    internal class MoveActionFollowPathAndKeepDistance : MoveAction
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly FollowPathHelper _followPath;
        private readonly TeleportHelper _teleport;
        private readonly MoveEffectDef.RotationType _rotationType;
        private readonly MoveEffectDef.MoveModifier _moveModifier;
        private readonly float _acceptedRangeMin;
        private readonly float _acceptedRangeMax;
        private readonly float _speedFactor;
        private readonly MobLocomotionReactions _reactions;
        private readonly IMoveActionPosition _self;
        private readonly IMoveActionPosition _target;
        private readonly long _targetNotReachableTimeout;
        private Vector3 _destinationPoint = InvalidVector;
        private Vector3 _reachablePoint = InvalidVector;
        private Vector3 _randomUnitVec;
        private bool _targetWasReached;
        private long _targetNotReachableAt = -1;

        ///#PZ-13761: #Dbg: 
        private Pawn _Dbg_pawn;

#if UNITY_EDITOR
        private readonly Color _debugColor = Color.cyan;
#endif

        public MoveActionFollowPathAndKeepDistance(
            [NotNull] NavMeshAgent agent,
            IMoveActionPosition self,
            IMoveActionPosition target,
            MoveEffectDef.RotationType rotationType,
            MoveEffectDef.MoveModifier moveModifier,
            float acceptedRangeMin,
            float acceptedRangeMax,
            float speedFactor,
            Guid entityId,
            MobLocomotionReactions reactions,
            MobMoveActionsConstantsDef constants,
            Pawn dbg_pawn
        )
            : base(entityId)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            _self = self;
            _target = target;
            _acceptedRangeMin = Mathf.Min(acceptedRangeMin, acceptedRangeMax);
            _acceptedRangeMax = Mathf.Max(acceptedRangeMin, acceptedRangeMax);
            _rotationType = rotationType;
            _moveModifier = moveModifier;
            _speedFactor = speedFactor;
            _reactions = reactions;
            _followPath = new FollowPathHelper(agent, constants, entityId, dbg_pawn);
            _teleport = new TeleportHelper(agent, constants, entityId);
            _targetNotReachableTimeout = SyncTime.FromSeconds(constants.FollowPathTargetNotReachableTimeout);
            _Dbg_pawn = dbg_pawn;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "FollowPathAndKeepDistanceToTarget:{0} AcceptedRange:[{1},{2}] RotationType:{3} MoveModifier:{5} SpeedFactor:{4}", 
                _target, _acceptedRangeMin, _acceptedRangeMax, _rotationType, _speedFactor, _moveModifier)
                .Write();

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor, 2f);
#endif
        }

        public override bool Init()
        {
            _reactions.Jumped += _followPath.OnJumpStarted;
            _reactions.Landed += _followPath.OnLanded;
            _followPath.Reset();
            _randomUnitVec = UnityEngine.Random.onUnitSphere.SetY(0);
            return _target.Valid;
        }

        public override void Dispose()
        {
            _reactions.Jumped -= _followPath.OnJumpStarted;
            _reactions.Landed -= _followPath.OnLanded;
            _followPath.Reset();
        }
        
        public override MoveActionResult Tick(Pawn.SimulationLevel simulationLevel)
        {
            if (!_target.Valid)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Target lost!",  _self.Position).Write();
                return Failed;
            }

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor);
#endif

            var targetPosition = _target.Position;
            var newDestinationPoint = targetPosition;
            var toDestination = newDestinationPoint - _self.Position;
            var toDestinationDistanceSqr = toDestination.sqrMagnitude;
            var targetTooClose = toDestinationDistanceSqr < _acceptedRangeMin.Sqr();
            if (targetTooClose)
                newDestinationPoint -= (toDestinationDistanceSqr > 0.1f ? toDestination.normalized : _randomUnitVec) * (_acceptedRangeMin + _acceptedRangeMax) * 0.5f;
            var needRepath = _followPath.CheckNeedRepath(_self.Position, _destinationPoint, newDestinationPoint);
            if (needRepath)
                _reachablePoint = InvalidVector;
            var targetReached = CheckDestinationReached(_self.Position, targetPosition, _reachablePoint, _acceptedRangeMax);
            
            if (targetReached && !targetTooClose)
            {
                if (!_targetWasReached)
                {
                    _followPath.Reset();
                    _teleport.Reset();
                    _targetWasReached = true;
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Target reached! SelfPosition:{0} TargetPosition:{1} Distance:{2}",  _self.Position, targetPosition, (_self.Position - targetPosition).magnitude).Write();
                }
            }
            else
            {
                if (simulationLevel > Pawn.SimulationLevel.Faraway)
                {
                    _teleport.Reset();

                    if (_followPath.Status == FollowPathStatus.NotStarted && _targetNotReachableAt == -1 || needRepath)
                    {
                        _targetWasReached = false;
                        _destinationPoint = newDestinationPoint;
                        if (_followPath.Start(_destinationPoint, out _reachablePoint))
                            _targetNotReachableAt = -1;
                        else
                        if (_targetNotReachableAt == -1)
                            _targetNotReachableAt = SyncTime.NowUnsynced;
                    }
                    if (_followPath.Status == FollowPathStatus.Finished)
                    {
                        if (!_targetWasReached)
                        {
                            _targetWasReached = true;
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "End of path. SelfPosition:{0} TargetPosition:{1} Distance:{2} Remaining:{3}",  _self.Position, targetPosition, (_self.Position - targetPosition).magnitude, _followPath.RemainingDistance).Write();
                        }
                    }
                    if (_followPath.Status == FollowPathStatus.Invalid)
                    {
                        if (_targetNotReachableAt == -1)
                        {
                            _targetNotReachableAt = SyncTime.NowUnsynced;
                            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Path is Invalid. SelfPosition:{0} TargetPosition:{1} Distance:{2}",  _self.Position, targetPosition, (_self.Position - targetPosition).magnitude).Write();
                        }
                    }

                    _followPath.Trace();
                }
                else
                {
                    // _followPath.ResetPath(); пока не сработал телепорт продолжаем двигаться по уже построенному пути, если он был построен

                    if (!_teleport.Engaged || needRepath)
                    {
                        _targetWasReached = false;
                        _destinationPoint = newDestinationPoint;
                        if (!_teleport.Engage(_destinationPoint, out _reachablePoint))
                            return Failed;
                    }
                }
            }

            if (_targetNotReachableAt != -1 && SyncTime.NowUnsynced - _targetNotReachableAt > _targetNotReachableTimeout)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Target not reachable for {3} sec. SelfPosition:{0} TargetPosition:{1} Distance:{2}",  _self.Position, targetPosition, (_self.Position - targetPosition).magnitude, SyncTime.ToSeconds(SyncTime.NowUnsynced - _targetNotReachableAt)).Write();
                return Failed;
            }

            return Running;
        }

        public override void GetLocomotionInput(InputState<MobInputs> input)
        {
            ///#PZ-13761: #remove_it: if (!_teleport.SetupInputState(input))
                _followPath.SetupInputState(input, _self.Position, _target.Position, _rotationType, _moveModifier, _speedFactor);
        }
    }
}
