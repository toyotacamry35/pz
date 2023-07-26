using System;
using Assets.Src.NetworkedMovement;
using Assets.Src.RubiconAI;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using Src.Locomotion;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public class TeleportHelper
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(TeleportHelper));
        
        [NotNull] private readonly NavMeshAgent _agent;
        [NotNull] private readonly MobMoveActionsConstantsDef _constants;
        private readonly Guid _entityId;
        private Vector3 _destinationPoint;
        private long _teleportTime;
        private bool _teleported;

        public TeleportHelper([NotNull] NavMeshAgent agent, [NotNull] MobMoveActionsConstantsDef constants, Guid entityId)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            if (constants == null) throw new ArgumentNullException(nameof(constants));
            _agent = agent;
            _constants = constants;
            _entityId = entityId;
        }

        public bool Engaged => _teleportTime > 0;

        public bool TimeForTeleport => _teleportTime > 0 && SyncTime.InThePast(_teleportTime);
        
        public bool TeleportAllowed => !AIWorld.RelevancyGrid.ContainsKey(_destinationPoint.ToShared());
        
        public bool Engage(Vector3 destinationPoint, out Vector3 reachablePoint)
        {
            bool foundReachable = MoveActionHelper.FindReachablePoint(destinationPoint, _agent, _constants.DestinationPointAdjustmentRadius, out reachablePoint);
            ///#PZ-13761: #Dbg: Teleport should not be called, other than once just after mob is born (Vitaly said)
            {
                Logger.IfDebug()?.Message/*Warn*/($"#PZ-13761: TeleportHelper.Engage is called ! ({_entityId})  " +
                                                     $"NMA.Pos:{_agent.nextPosition} --> destination:{destinationPoint} (reachable:{foundReachable} - {reachablePoint})" +
                                                     $"\nNMA: enbld:{_agent.enabled}, onNavMesh:{_agent.isOnNavMesh}, onOffMeshLnk:{_agent.isOnOffMeshLink}, stopped:{_agent.isStopped}, " +
                                                     $"hasPth:{_agent.hasPath}, PthPending:{_agent.pathPending}, PthStale:{_agent.isPathStale}, PthStatus:{_agent.pathStatus}, Velo:{_agent.velocity}." +
                                                     $"  [ 2019.11: Say to @a.Kislun || @Vitaly if you see it in other cases, than: \"Teleport should not be called, other than once just after mob is born\"]")
                    .Write();
                // reachablePoint = new Vector3(7, 8, 9); // to distinguish this case from others (if mobs 'll be at strange coors)
                // return false;
            }

            if (!foundReachable)
            {
                if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(_entityId, "Destination point {0} is not on NavMesh nor near it",  destinationPoint).Write();
                return false;
            }

            if (Logger.IsDebugEnabled && (destinationPoint - reachablePoint).sqrMagnitude > 0.01)
                Logger.IfDebug()?.Message("Destination point shifted: {0} -> {1}", destinationPoint, reachablePoint).Write();
            
            _destinationPoint = reachablePoint;
            if(_teleportTime <= 0)
                _teleportTime = SyncTime.Now + SyncTime.FromSeconds(Random.Range(_constants.FollowPathTeleportDelayMin, _constants.FollowPathTeleportDelayMax)); // 10 ..50 sec
            Logger.IfDebug()?.Message(_entityId, "Teleport engaged from {0} to {1} after {2} second",  _agent.nextPosition, _destinationPoint, SyncTime.ToSeconds(_teleportTime - SyncTime.Now)).Write();
            return true;
        }

        public void Reset()
        {
            _teleportTime = 0;
            _teleported = false;
        }

        public bool SetupInputState(InputState<MobInputs> input)
        {
            if (TimeForTeleport && TeleportAllowed)
            {
                if (!_teleported)
                {
                    _teleported = true;
                    Logger.IfDebug()?.Message(_entityId, "Teleport to {0}",  _destinationPoint).Write();
                }
                
                var target = LocomotionHelpers.WorldToLocomotionVector(_destinationPoint);
                input[MobInputs.TargetPointXY] = target.Horizontal;
                input[MobInputs.TargetPointV] = target.Vertical;
                input[MobInputs.Teleport] = true;
                return true;
            }
            return false;
        }
    }
}
