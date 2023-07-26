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
    internal class MoveActionFollowPath : MoveAction
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(MoveActionFollowPath));
        
        private readonly FollowPathHelper _followPath;
        ///#PZ-13761: TODO: remove usings of it:
        private readonly TeleportHelper _teleport;
        private readonly MoveEffectDef.RotationType _rotationType;
        private readonly MoveEffectDef.MoveModifier _moveModifier;
        private readonly float _acceptedRange;
        private readonly float _speedFactor;
        private readonly MobLocomotionReactions _reactions;
        private readonly IMoveActionPosition _self;
        private readonly IMoveActionPosition _target;
        private Vector3 _destinationPoint = InvalidVector;
        private Vector3 _reachablePoint = InvalidVector;

        ///#PZ-13761: #Dbg: 
        private Pawn _Dbg_pawn;

#if UNITY_EDITOR
        private readonly Color _debugColor = Color.blue;
#endif

        public MoveActionFollowPath(
            [NotNull] NavMeshAgent agent,
            IMoveActionPosition self,
            IMoveActionPosition target,
            MoveEffectDef.RotationType rotationType,
            MoveEffectDef.MoveModifier moveModifier,
            float acceptedRange,
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
            _acceptedRange = acceptedRange;
            _rotationType = rotationType;
            _moveModifier = moveModifier;
            _speedFactor = speedFactor;
            _reactions = reactions;
            _followPath = new FollowPathHelper(agent, constants, entityId, dbg_pawn);
            _teleport = new TeleportHelper(agent, constants, entityId);
            _Dbg_pawn = dbg_pawn;
            Logger.IfDebug()?.Message(EntityId, "FollowPathToTarget:{0} AcceptedRange:{1} RotationType:{2} MoveModifier:{4} SpeedFactor:{3}", _target, _acceptedRange, _rotationType, _speedFactor, _moveModifier)
                .Write();

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor, 2f);
#endif
        }

        public override bool Init()
        {
            _reactions.Jumped += _followPath.OnJumpStarted; // Allocation: but we can't do better
            _reactions.Landed += _followPath.OnLanded;
            _followPath.Reset();
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
                Logger.IfDebug()?.Message(EntityId, "Target is invalid!")
                    .Write();
                return Failed;
            }

#if UNITY_EDITOR
            if (DbgMode)
                _target?.DebugDrawTargetPosition(_debugColor, 2f);
#endif

            var targetPosition = _target.Position;
            var needRepath = _followPath.CheckNeedRepath(_self.Position, _destinationPoint, targetPosition);
            if (needRepath)
                _reachablePoint = InvalidVector;
            var targetReached = CheckDestinationReached(_self.Position, targetPosition, _reachablePoint, _acceptedRange);

            if (targetReached)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Target reached! Current position:{0} Target position:{1} Distance:{2}",  _self.Position, targetPosition, (_self.Position - targetPosition).magnitude).Write();
                return Finished;
            }
            else
            if (simulationLevel > Pawn.SimulationLevel.Faraway)
            {
                _teleport.Reset();

                if (_followPath.Status == FollowPathStatus.NotStarted || needRepath)
                {
                    _destinationPoint = targetPosition;
                    if (!_followPath.Start(_destinationPoint, out _reachablePoint))
                        return Failed;
                }
                if (_followPath.Status == FollowPathStatus.Finished)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "End of path. _self.Position:{0} DestinationPoint:{1} Distance:{2} Remaining:{3}",  _self.Position, targetPosition, (_self.Position - targetPosition).magnitude, _followPath.RemainingDistance).Write();
                    return Finished;
                }
                if (_followPath.Status == FollowPathStatus.Invalid)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, "Path is Invalid").Write();
                    return Failed;
                }
                
                _followPath.Trace();
            }
            else
            {
                // _followPath.ResetPath(); пока не сработал телепорт продолжаем двигаться по уже построенному пути, если он был построен
                
                if (!_teleport.Engaged || needRepath)
                {
                    _destinationPoint = targetPosition;
                    if (!_teleport.Engage(_destinationPoint, out _reachablePoint))
                        return Failed;
                }
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
