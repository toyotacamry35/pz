using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Lib.Extensions;
using Assets.Src.RubiconAI;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = SharedCode.Utils.Vector3;

namespace Assets.Src.NetworkedMovement
{
    public partial class Pawn
    {
        private DateTime _lastTickLoco;
        private DateTime _lastTickMoveActions;
        private long _locoUpdatesCount;
        private bool firstDone_DBG;
        private float dbg_nextTimeLog2;
        private float dbg_deltaTimeLog2 = 3f;
        private float dbg_triggerRecalc;

        [Header(" -v-v-v-v-v-v-v-v- DEBUG -v-v-v-v-v-v-v-v- ")]
        public float ClosestObserverDistance_DBG;

        public GameObject _relativeGo_DBG;
        public float RealDist_DBG;
        public (UpdateResultMean, int) LocoTickStep_DBG;
        public Vector3Int DELTA_Cell_DBG;
        public Vector3Int RelativeGo_Cell_DBG;
        public Vector3Int ThisMob_Cell_DBG;

        [Header(" - - - - - handlers - - - - - ")]
        public bool DBG_TrackClose;

        [Header(" - - - - - other - - - - - ")]
        public GameObject Dbg_PlayerMarkerGo;

        public GameObject Dbg_KvarMarkerGo;

        [Header(" - - - - - old: - - - - - ")]
        public string SimLvl_Server;

        public string SimLvl_Cl;

        [Header(" -^-^-^-^-^-^-^-^- DEBUG -^-^-^-^-^-^-^-^- ")]
        public bool _;

        private const bool Dbg = false;
        private Vector3Int invalidVec3Int_DBG = new Vector3Int(-77, -77, -77);
        private List<DbgLoco1st100Data> _dbg_1st100LocoUpdatesDataOnServer;
        private static bool _closestDistBelow0ErrSpamPreventer;
        public bool Dbg_LogSimLvlChange;
        [SerializeField] private LocomotionDebugTrail _locomotionDebugTrail; //v
        ///#PZ-13761: #Dbg #Tmp:
        bool _pZ_13761_GotServ;
        //bool _pZ_13761_InitLocoServ_calledAfter_GotServ;
        private bool _isEnabledCollectDebugData;
        float dbg_nextTimeLog;
        float dbg_deltaTimeLog = 5f;
        internal LocomotionDamperNode _damperNode;

        private PawnPositionLoggingHandler Plh => _posLoggingHandler;
        
        private void Dbg_SimLvlChangeLog(SimulationLevel lvl, bool raise, string tag)
        {
            if (!GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef))
                return;

            if (raise)
                if (Logger.IsDebugEnabled)
                    Logger.IfDebug()?.Message($"{tag} . . . . . . . )---> {lvl}.   (dist: {_relevanceProviderUpdateCl.Dbg_ClosestObserverDistance_Forced_DANGER}/{_relevanceProviderUpdateS.Dbg_ClosestObserverDistance_Forced_DANGER})").Write();
                else if (Logger.IsDebugEnabled)
                    Logger.IfDebug()?.Message($"{tag} . x . x . x . {lvl} X==>.   (dist: {_relevanceProviderUpdateCl.Dbg_ClosestObserverDistance_Forced_DANGER}/{_relevanceProviderUpdateS.Dbg_ClosestObserverDistance_Forced_DANGER})").Write();
        }

        private void LogPawnState()
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(EntityId, PawnStateString()).Write();
        }

        private string PawnStateString()
        {
            return string.Format(
                "{0}go:{1} pawn:{2} locomotionUpdate:{3} actionsUpdate:{4} collider:{5} animator:{6}", // tick:{7}",
                (IsServer ? $"simLvlS:{_simulationLevelServer} " : "") + (IsClient ? $"simLvlC:{_simulationLevelClient} " : "")
                , gameObject.activeSelf, enabled, _locomotionCoroutine != null, _moveActionsCoroutine != null, _colliderS.enabled,
                _clientView?.Animator?.enabled /*_Deprecated_moveActionsUpdateTick*/);
        }

        // --- Util types: -------------------------

        ///#PZ-13568: !!!IMPORTANT!!! Update should be On: UNCOMMENT REAL `UPDATE` HERE!:
        void Update_PZ_13568() /// Update()
        {
            if (PZ_13761_Dbg_Data.Mode == 2)
            {
                Dbg_PZ_13761_Dbg_Data_Save(DBG_Forced_DAMP_PZ_13761, nameof(DBG_Forced_DAMP_PZ_13761));
                DBG_Forced_DAMP_PZ_13761 = false;
            }
        }

        private void EnableCollectDebugData()
        {
            _locoCoroutineCycleTimes.Clear();
            _isEnabledCollectDebugData = true;
        }

        private void DisableCollectDebugData()
        {
            _isEnabledCollectDebugData = false;
            _locoCoroutineCycleTimes.Clear();
        }
        
        public void Dbg_PZ_13761_Dbg_Data_Save(bool forcedAlarm = false, string forcedAlarmCauser = null)
        {
            if (!Dbg_PZ_13761_Dbg_Data?.ShouldSave(Time.time, forcedAlarm) ?? true)
                return;
            
            var pathExist = !Dbg_PZ_13761_Dbg_Data.PrevNMAPos.HasValue // not 1st call
                            || NavMesh.CalculatePath(
                                        _agentS.nextPosition, 
                                        Dbg_PZ_13761_Dbg_Data.PrevNMAPos.Value, 
                                        new NavMeshQueryFilter() {areaMask = _agentS.areaMask, agentTypeID = _agentS.agentTypeID}, 
                                        Dbg_PZ_13761_Dbg_Data.LastPath
                                        );
            Dbg_PZ_13761_Dbg_Data.Save(new PZ_13761_Dbg_Data.PZ_13761_Dbg_PositionsAndMNAData_2(
                                                                                                goPos             : transform.position, 
                                                                                                nmaEnabled        : _agentS.enabled, 
                                                                                                nmaIsOnNavMesh    : _agentS.isOnNavMesh,
                                                                                                nmaOnOffMeshLink  : _agentS.isOnOffMeshLink,
                                                                                                nmaStopped        : _agentS.isStopped,
                                                                                                nmaHasPath        : _agentS.hasPath,
                                                                                                nmaPathPending    : _agentS.pathPending,
                                                                                                nmaPthStale       : _agentS.isPathStale,
                                                                                                nmaPthStatus      : _agentS.pathStatus,
                                                                                                nmaPos            : _agentS.nextPosition, 
                                                                                                nmaSteeringTarget : _agentS.steeringTarget,
                                                                                                nmaSpeed          : _agentS.speed,
                                                                                                nmaVelo           : _agentS.velocity,
                                                                                                pathExist         : pathExist,
                                                                                                time              : Time.time
                                                                                                ), forcedAlarm, forcedAlarmCauser);
        
        }

        public string GetDebugInfo()
        {
            return $"Pawn:: _locomotionIsBroken: {_locomotionIsBroken}\n" 
                   + $"Pawn:: _moveActionIsBroken: {_moveActionIsBroken}\n" 
                   + $"Pawn:: SimulationLvl[S]:  {(int) (SimulationLevel) _simulationLevelServer}\n"
                   + $"Pawn:: SimulationLvl(Cl):  {(int) (SimulationLevel) _simulationLevelServer}\n"
                   + $"Pawn:: ClosestObserverDistance: {_relevanceProviderUpdateCl.ClosestObserverDistance}/{_relevanceProviderUpdateS.ClosestObserverDistance}"
                   + $"Pawn:: LastTickLoco: {_lastTickLoco}\n"
                   + $"Pawn:: _locoUpdatesCount: {_locoUpdatesCount}\n"
                   + $"Pawn:: LastTickMoveActions:  {SharedHelpers.TimeStamp(_lastTickMoveActions)}\n"
                   + $"Pawn:: SM.StateName: {_stateMachineS?.CurrentStateName ?? "null"}\n"
                   + $"Pawn:: TickStep:{LocoTickStep_DBG} VisibleByCamera:{(_isVisibleByCamera.HasValue ? _isVisibleByCamera.Value.IsVisible : false)}\n "
                   + "< . . . . . . . . . . . . . . . . >\n"
                   + _moveActionsDoer?.GetDebugInfo();
        }
        
        #if UNITY_EDITOR 
        //#Dbg:
        private void OnDrawGizmosSelected()
        {
            ClosestObserverDistance_DBG = Math.Min(_relevanceProviderUpdateCl.ClosestObserverDistance, _relevanceProviderUpdateS.ClosestObserverDistance); //Может не корректно - просто заткнул от ошибки - раньше был только 1 _relevanceProviderUpdate
            RealDist_DBG = (_relativeGo_DBG == null) ? -1f : (_relativeGo_DBG.transform.position - transform.position).magnitude;

            ThisMob_Cell_DBG = AIWorld.RelevancyGrid.Dbg_GetCellByPos(transform.position.ToShared()).ToUnityVector3Int();
            RelativeGo_Cell_DBG = (_relativeGo_DBG == null)
                ? invalidVec3Int_DBG
                : AIWorld.RelevancyGrid.Dbg_GetCellByPos(_relativeGo_DBG.transform.position.ToShared()).ToUnityVector3Int();
            DELTA_Cell_DBG = (_relativeGo_DBG == null) ? invalidVec3Int_DBG : ThisMob_Cell_DBG - RelativeGo_Cell_DBG;


            if (DBG_TrackClose && ClosestObserverDistance_DBG > 0 && (_relevanceProviderUpdateCl.Dbg_ClosestObserverDistance_Forced_DANGER > 0 || _relevanceProviderUpdateS.Dbg_ClosestObserverDistance_Forced_DANGER > 0)) //Может не корректно - просто заткнул от ошибки - раньше был только 1 _relevanceProviderUpdate
            {
                if (Time.realtimeSinceStartup > dbg_nextTimeLog2)
                {
                    dbg_nextTimeLog2 = Time.realtimeSinceStartup + dbg_deltaTimeLog2;
                    if (Logger.IsDebugEnabled)
                        Logger.IfDebug()?.Message(
                            $"##((DBG_TrackClose BUT ClosestObserverDistance_DBG >= 0)): Plyr:{_relativeGo_DBG?.transform.position}, cell:{RelativeGo_Cell_DBG};  " +
                            $"Kvar:{transform.position}, cell:{ThisMob_Cell_DBG};  Dlt_Cell:{DELTA_Cell_DBG}.  " +
                            $"Clsst_Dist:{ClosestObserverDistance_DBG}, Re_Dist:{RealDist_DBG}.  LocoTickStep:{LocoTickStep_DBG}")
                            .Write();

                    if (!firstDone_DBG && _relativeGo_DBG != null)
                    {
                        Instantiate(Dbg_PlayerMarkerGo, _relativeGo_DBG.transform.position, _relativeGo_DBG.transform.rotation);
                        Instantiate(Dbg_KvarMarkerGo, transform.position, transform.rotation);
                        firstDone_DBG = true;
                        if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message(" ^ - - - - - 1stDone - - - - - ^ ").Write();;

                        dbg_triggerRecalc = Math.Min(_relevanceProviderUpdateCl.Dbg_ClosestObserverDistance_Forced_DANGER, _relevanceProviderUpdateS.Dbg_ClosestObserverDistance_Forced_DANGER); //Может не корректно - просто заткнул от ошибки - раньше был только 1 _relevanceProviderUpdate
                    }
                }
            }

            if (!GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef))
                return;
            // Draw "relevance level" borders:
            DebugExtension.DrawCircle(transform.position, Color.blue, 100f);
            DebugExtension.DrawCircle(transform.position, Color.magenta, 62.5f);
            DebugExtension.DrawCircle(transform.position, Color.white, 25f);
        }
        
        #if ONGUI
        void OnGUI1()
        #else
        void OnDisabledGUI()
        #endif
        {
            _moveActionsDoer.DrawOnGUI();
        }

        #endif
        
        
        public struct DbgLoco1st100Data
        {
            public DateTime Time;
            public Vector3? VirtBodyPos;
            public Vector3? TransformBodyPos;
            public UnityEngine.Vector3 GoPos;

            [CanBeNull]
            public Type CurrMoveActionType;

            public SimulationLevel SimLvl;

            public DbgLoco1st100Data(DateTime time, Vector3? virtBodyPos, Vector3? transformBodyPos, UnityEngine.Vector3 goPos, [CanBeNull] Type currMoveActionType,
                SimulationLevel simLvl)
            {
                Time = time;
                VirtBodyPos = virtBodyPos;
                TransformBodyPos = transformBodyPos;
                GoPos = goPos;
                CurrMoveActionType = currMoveActionType;
                SimLvl = simLvl;
            }

            public override string ToString()
            {
                return
                    $"Time: {Time};  VirtBodyPos: {VirtBodyPos?.ToString() ?? "null"};  TransformBodyPos: {TransformBodyPos?.ToString() ?? "null"};  GoPos: {GoPos};  CurrMoveActionType: {CurrMoveActionType?.Name ?? "null"};  SimLvl: {SimLvl}";
            }
        }
    }
}