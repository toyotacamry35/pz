using System;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using GeneratedDefsForSpells;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using SVector3 = SharedCode.Utils.Vector3;
using SVector2 = SharedCode.Utils.Vector2;
using static Src.Locomotion.LocomotionHelpers;
using static Assets.Src.NetworkedMovement.MoveActions.MoveActionHelper;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public enum FollowPathStatus
    {
        NotStarted,
        Pending,
        Started,
        Finished,
        Invalid
    }

    
    public class FollowPathHelper: IResettable
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly NavMeshAgent _agent;
        private readonly Guid _entityId;
        private readonly MobMoveActionsConstantsDef _constants;
        private long _repathBaseTime = -1;
        private long _lastRepathTime = -1;
        private long _lastDebugTraceTime = -1;
        private OffLinkState _offLinkState;
        private Plane _currentOffMeshLinkFinishPlane;
        ///#PZ-13761: #Dbg: 
        private Pawn _Dbg_pawn;
        
        public FollowPathHelper([NotNull] NavMeshAgent agent, [NotNull] MobMoveActionsConstantsDef constants, Guid entityId, Pawn dbg_pawn)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (constants == null) throw new ArgumentNullException(nameof(constants));
            _agent = agent;
            _entityId = entityId;
            _constants = constants;
            _Dbg_pawn = dbg_pawn;
        }

        public FollowPathStatus Status
        {
            get
            {
                if (!_agent.hasPath)
                    return _agent.pathPending ? FollowPathStatus.Pending : FollowPathStatus.NotStarted;

                if (_agent.remainingDistance <= Mathf.Max(_agent.stoppingDistance, 0.1f))
                    return FollowPathStatus.Finished;
                
                if (_agent.pathStatus == NavMeshPathStatus.PathInvalid || _agent.pathStatus == NavMeshPathStatus.PathPartial)
                    return FollowPathStatus.Invalid;

                return FollowPathStatus.Started;
            }
        }

        public float RemainingDistance => _agent.remainingDistance;

        public void OnJumpStarted()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OnJumpStarted _offLinkState:{0}",  _offLinkState).Write();
            if(_offLinkState == OffLinkState.Jump)
                _offLinkState = OffLinkState.Jumped;
        }

        public void OnLanded()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OnLanded _offLinkState:{0}",  _offLinkState).Write();
        }
        
        public bool Start(Vector3 destinationPoint, out Vector3 reachablePoint)
        {
            if(destinationPoint == InvalidVector) throw new ArgumentException("destinationPoint is invalid", nameof(destinationPoint));
            
            reachablePoint = destinationPoint;
            
            if(_lastRepathTime != -1)
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Update path after {0} sec (path status: {1} offlink: {2})",  SyncTime.ToSeconds(SyncTime.NowUnsynced - _lastRepathTime), Status, _offLinkState).Write();
            
            _lastRepathTime = SyncTime.NowUnsynced;

            if (!_agent.enabled)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Can't calculate path due agent is disabled.").Write();
                return false;
            }            

            if (!_agent.isOnNavMesh)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Can't calculate path due agent is not on NavMesh. Agent position is: {0}",  _agent.nextPosition).Write();
                Logger.IfError()?.Message(_entityId, "Can't calculate path due agent is not on NavMesh. Agent position is: {0}", _agent.nextPosition).Write();
                return false;
            }            
            
            if (!FindReachablePoint(destinationPoint, _agent, _constants.DestinationPointAdjustmentRadius, out reachablePoint))
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Destination point {0} is not on NavMesh nor near it",  destinationPoint).Write();
                return false;
            }
            
            if (Logger.IsDebugEnabled && (destinationPoint - reachablePoint).sqrMagnitude > 0.01)
                Logger.IfDebug()?.Message(_entityId, "Destination point adjusted: {0} -> {1} ({2}m)",  destinationPoint, reachablePoint, (destinationPoint - reachablePoint).magnitude).Write();

            LocomotionNavMeshAgent.Logger.IfDebug()?.Message("2. #ResetPath.").Write();;
            _agent.ResetPath();
            _offLinkState = OffLinkState.None;

            Logger.IfDebug()?.Message("3. #SetDestination: " + reachablePoint).Write();
            // Logger.IfError()?.Message("SetDestination").Write();;
            var path = new NavMeshPath();
            _agent.ResetPath();

            ///#PZ-13568: #Dbg: #Tmp saved args:
            var from = _agent.nextPosition;
            var to = reachablePoint;
            var mask = _agent.areaMask;
            var agntTId = _agent.agentTypeID;

            ///#PZ-13568: #Dbg: #Tmp replaced by nxt line:
            ///var rv = NavMesh.CalculatePath(_agent.nextPosition, reachablePoint, new NavMeshQueryFilter() { areaMask = _agent.areaMask, agentTypeID = _agent.agentTypeID }, path);
            var rv = NavMesh.CalculatePath(from, to, new NavMeshQueryFilter() { areaMask = mask, agentTypeID = agntTId }, path);
            if (rv)
            {
                _agent.path = path;
                _calculatePathFailedCounter = default;
            }
            else if (Debug.isDebugBuild)
            {
                ///#PZ-13568: #Dbg:
                _Dbg_pawn.Dbg_PZ_13761_Dbg_Data?.OnCalcPathFailedEvent(from, to ,mask, agntTId);
                Dbg_OnCalculatePathFailed(_agent.nextPosition, reachablePoint);
            }
            //var rv = _agent.SetDestination(reachablePoint);
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "SetDestination from {2} to {0} returns {1}",  reachablePoint, rv, _agent.nextPosition).Write();
            return rv;
        }

        public void Reset()
        {
            LocomotionNavMeshAgent.Logger.IfDebug()?.Message("2. #ResetPath.").Write();;
            if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
                _agent.ResetPath();
            else
            {
                Logger.If(LogLevel.Error)
                    ?.Message($"!_agent.isActiveAndEnabled({_agent.isActiveAndEnabled}) || !_agent.isOnNavMesh({_agent.isOnNavMesh}). {_agent.gameObject.name}, _agent.pos:{_agent.transform.position}") //, prnt.pos:{_agent.transform.parent.position}");
                    .UnityObj(_agent)
                    .Write(); 
                if (false)
                    Debug.Break();
            }

            _offLinkState = OffLinkState.None;
        }
        
        public void SetupInputState(
            InputState<MobInputs> outInput, 
            Vector3 selfPosition, 
            Vector3 targetPosition, 
            MoveEffectDef.RotationType rotationType, 
            MoveEffectDef.MoveModifier modifier,
            float speedFactor)
        {
            ProcessOffMeshLink(selfPosition);
            
            SharedCode.Utils.Vector2 pathDirection;
            if (Status == FollowPathStatus.Started)
            {
                pathDirection = _offLinkState == OffLinkState.None ? 
                    WorldToLocomotionVector(_agent.steeringTarget - selfPosition).Horizontal.normalized : 
                    WorldToLocomotionVector(_agent.currentOffMeshLinkData.endPos - selfPosition).Horizontal.normalized;
            }
            else
                pathDirection = SharedCode.Utils.Vector2.zero;

            SharedCode.Utils.Vector2 guide;
            switch (rotationType)
            {
                case MoveEffectDef.RotationType.LookAtTarget:
                    guide = WorldToLocomotionVector(targetPosition - selfPosition).Horizontal.normalized;
                    break;
                case MoveEffectDef.RotationType.LookAtMoveDirection:
                    guide = pathDirection;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rotationType), rotationType, null);
            }

            outInput[MobInputs.SpeedFactor] = speedFactor;
            outInput[MobInputs.Guide] = guide;
            outInput[MobInputs.FollowPath] = true;
            if (Status == FollowPathStatus.Started)
            {
                //moved out this if: input[MobInputs.FollowPath] = true;
                switch (_offLinkState)
                {
                    case OffLinkState.None:
                    case OffLinkState.Jumped:
                        outInput[MobInputs.Move] = InverseTransformVector(pathDirection, guide);
                        outInput[MobInputs.Run] = (modifier & MoveEffectDef.MoveModifier.Run) != 0;
                        break;
                    case OffLinkState.Jump:
                        var targetPoint = WorldToLocomotionVector(_agent.currentOffMeshLinkData.endPos);
                        outInput[MobInputs.JumpToTarget] = true;
                        outInput[MobInputs.TargetPointXY] = targetPoint.Horizontal;
                        outInput[MobInputs.TargetPointV] = targetPoint.Vertical;                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public bool CheckNeedRepath(Vector3 selfPosition, Vector3 oldTargetPosition, Vector3 newTargetPosition)
        {
            if (oldTargetPosition == InvalidVector)
                return true;

            if (newTargetPosition == InvalidVector)
                return false;

            float delay;
            if (CalculateRepathDelay(selfPosition, _agent.nextPosition, oldTargetPosition, newTargetPosition, _offLinkState, out delay))
            {
                if (_repathBaseTime == -1)
                    _repathBaseTime = SyncTime.NowUnsynced + SyncTime.FromSeconds(UnityEngine.Random.value * _constants.FollowPathRepathDelayRandomisation);
                if (_repathBaseTime + SyncTime.FromSeconds(delay) < SyncTime.NowUnsynced)
                {
                    _repathBaseTime = -1;
                    return true;
                }
            }
            else
                _repathBaseTime = -1;
            return false;
        }

        private bool CalculateRepathDelay(
            Vector3 selfPosition, 
            Vector3 pathPosition, 
            Vector3 oldTargetPosition, 
            Vector3 newTargetPosition, 
            OffLinkState offLinkState, 
            out float delay)
        {
            var near = _constants.FollowPathRepathNear;
            var far = _constants.FollowPathRepathFar;
            
            var distanceToTarget = (newTargetPosition - selfPosition).magnitude;
            var factor = Mathf.InverseLerp(near.Distance, far.Distance, distanceToTarget);
            var targetShiftSqr = (newTargetPosition - oldTargetPosition).sqrMagnitude;
            var targetThresholdSqr = Mathf.Lerp(near.TargetShift, far.TargetShift, factor).Sqr();
            if (targetShiftSqr > targetThresholdSqr)
            {
                delay = Mathf.Lerp(near.Delay, far.Delay, factor);
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Target moved: {0} -> {1} Distance: {2}",  oldTargetPosition, newTargetPosition, Mathf.Sqrt(targetShiftSqr)).Write();
                return true;
            }

            if (offLinkState == OffLinkState.None)
            {
                var selfDevThresholdSqr = Mathf.Lerp(near.SelfDeviation, far.SelfDeviation, factor).Sqr();
                var selfDevSqr = (selfPosition - pathPosition).sqrMagnitude;
                if (selfDevSqr > selfDevThresholdSqr)
                {
                    delay = Mathf.Lerp(near.Delay, far.Delay, factor);
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Self deviation detected: {0}",  Mathf.Sqrt(selfDevSqr)).Write();
                    return true;
                }
            }

            delay = 0;
            return false;
        }
      
        private void ProcessOffMeshLink(Vector3 selfPosition)
        {
            if (!_agent.hasPath || !_agent.isOnOffMeshLink)
            {
                if (_offLinkState != OffLinkState.None)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OffMeshLink moving canceled. hasPath:{0} isOnOffMeshLink:{1}",  _agent.hasPath, _agent.isOnOffMeshLink).Write();
                    _offLinkState = OffLinkState.None;
                }
            }   
            else 
            {
                if (_offLinkState == OffLinkState.None)
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "OffMeshLink moving started").Write();
                    _offLinkState = OffLinkState.Jump;
                    _currentOffMeshLinkFinishPlane = _agent.currentOffMeshLinkData.OffMeshLinkFinishPlane();
                }
                else
                {
                    if (!_currentOffMeshLinkFinishPlane.GetSide(selfPosition))
                    {
                        Logger.IfDebug()?.Message(_entityId, "Stop OffMeshLink moving").Write();
                        Logger.IfDebug()?.Message("A. #CompleteOffMeshLink.").Write();
                        _agent.CompleteOffMeshLink();
                        _offLinkState = OffLinkState.None;
                    }
                }
            }
        }

        private int _calculatePathFailedCounter;
        private void Dbg_OnCalculatePathFailed(Vector3 from, Vector3 to)
        {
            int DebugLogIfFailedMoreThan = 10;
            if (++_calculatePathFailedCounter > DebugLogIfFailedMoreThan)
            {
                _Dbg_pawn.Dbg_PZ_13761_Dbg_Data_Save(true, "FPH.OnCalculatePathFailed");
                Logger.IfWarn()?.Message($"OnCalculatePathFailed {_calculatePathFailedCounter}'th time in a row. (entId:{_Dbg_pawn.EntityId}).").Write();
                _calculatePathFailedCounter = default;
            }
        }

        public void Trace()
        {
            if (Logger.IsTraceEnabled && _lastDebugTraceTime < SyncTime.NowUnsynced)
            {
                _lastDebugTraceTime = SyncTime.NowUnsynced + 1000;
                Logger.IfTrace()?.Message(_entityId, "PathStatus:{0} AgentPosition:{0} DestinationPoint:{2} RemainingDistance:{3} OffLink:{4}",
                    Status, _agent.nextPosition, _agent.pathEndPosition, _agent.remainingDistance, _offLinkState).Write();
            }
        }

        enum OffLinkState { None, Jump, Jumped }
    }
}