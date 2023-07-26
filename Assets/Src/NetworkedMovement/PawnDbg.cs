using System;
using System.Collections.Generic;
using System.Text;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.NetworkedMovement.MoveActions;
using ColonyShared.SharedCode.Aspects.Locomotion;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using Src.Locomotion;
using Src.Locomotion.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Src.NetworkedMovement
{
    /// <summary>
    /// All PZ-13761 (PZ-13568) debug logic moved out of Pawn.cs to separate file.
    /// </summary>
    public partial class Pawn
    {
        #region PZ-13568:

        // Used only on server. So some fields could be invalid on Cl-s.
        private bool Dbg_PZ_13761_Enabled => GlobalConstsHolder.Dbg_PZ_13761_DebuggingEnabled;
        Vector3 Dbg_PZ_13761_Dbg_Data_AwakePos;
        float Dbg_PZ_13761_Dbg_Data_AwakeTime;
        private PZ_13761_Dbg_Data _dbg_PZ_13761_Dbg_Data;
        internal PZ_13761_Dbg_Data Dbg_PZ_13761_Dbg_Data
        {
            get
            {
                ///#PZ-13761: #Dbg: #Tmp:
                if (Dbg_PZ_13761_Enabled && IsServer && _dbg_PZ_13761_Dbg_Data == null)
                    Logger.If(LogLevel.Error)
                        ?.Message($"#PZ-13761: Dbg_PZ_13761_Dbg_Data == null !! @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ({EntityId})")
                        .UnityObj(gameObject)
                        .Write();

                return _dbg_PZ_13761_Dbg_Data;
            }
            set => _dbg_PZ_13761_Dbg_Data = value;
        }
        
        // private PZ_13761_Dbg_Data Dbg_PZ_13761_Dbg_Data => Dbg_PZ_13761_Dbg_Data_Enabled 
        //     ? ((_dbg_PZ_13761_Dbg_Data != null) 
        //         ? _dbg_PZ_13761_Dbg_Data.GetSelfIfActive 
        //         : (_dbg_PZ_13761_Dbg_Data = new PZ_13761_Dbg_Data()).GetSelfIfActive) 
        //     : null;
        // private PZ_13761_Dbg_Data _dbg_PZ_13761_Dbg_Data;
        public bool DBG_Forced_DAMP_PZ_13761; ///#PZ-13761: #Tmp #Dbg
        internal /*struct*/class PZ_13761_Dbg_Data
        {
            /// <summary>
            /// Исторически разные наборы д-х (в т.ч. и по полноте):
            /// </summary>
            internal const int Mode = 2;
            readonly bool EnableLogZoneBorders = GlobalConstsDef.DebugFlagsGetter.IsPZ_13761_EnableLogZoneBorders(GlobalConstsHolder.GlobalConstsDef);
            //Oleg Labutin, QA 14:11
            //  1 точка Savannah(528.12 102.21 -872.26)
            //  2 точка Savannah(832.97 124.86 -246.10)
            //private static readonly Vector2 LogZoneBorderMin = new Vector2(-870, 530); //MobZone:(160, 120);//Svnh:(-870, 530); //(z, x)
            //private static readonly Vector2 LogZoneBorderMax = new Vector2(-250, 830); //MobZone:(220, 180);//Svnh:(-250, 830); //(z, x)
            private static readonly Vector2 LogZoneBorderMin = new Vector2(GlobalConstsHolder.GlobalConstsDef.PZ_13761_LogZoneBorderMin_X,
                                                                           GlobalConstsHolder.GlobalConstsDef.PZ_13761_LogZoneBorderMin_Y); //MobZone:(160, 120);//Svnh:(-870, 530); //(z, x)
            private static readonly Vector2 LogZoneBorderMax = new Vector2(GlobalConstsHolder.GlobalConstsDef.PZ_13761_LogZoneBorderMax_X,
                                                                           GlobalConstsHolder.GlobalConstsDef.PZ_13761_LogZoneBorderMax_Y); //MobZone:(220, 180);//Svnh:(-250, 830); //(z, x)
            private static readonly Vector2 LogZoneBorderSize = LogZoneBorderMax - LogZoneBorderMin;
            private readonly Rect LogZoneBorder = new Rect(LogZoneBorderMin, LogZoneBorderSize);
            private Pawn _pawn;

            // Logged to separate file:
            [NotNull] public static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(PZ_13761_Dbg_Data));

            internal Vector3 PosAtAwake;
            internal float AwakeTime;
            internal Vector3 PosGotServer;
            internal float GotServerTime;
            internal Vector3? PrevNMAPos;

            private bool _isActive;
            PZ_13761_Dbg_Data GetSelfIfActive => (_isActive) ? this : null;

            /// <summary>
            /// Есть 2 listener'а, вставленные в LocoPplne, кот. записывают сюда время, когда поймали в vars'ах флаг Teleport
            /// </summary>
            float lastBeforeInterpolTeleportTime = -1;
            float lastAfterInterpolTeleportTime = -1;

            // RingBuffer<(SimulationLevel, float/*Timr.time*/)> _lastSimulLvlsBuffer;
            // RingBuffer<(Type, float/*Timr.time*/)> _lastMoveActionsBuffer;
            // RingBuffer<(PZ_13761_Dbg_PositionsAndMNAData, float/*Timr.time*/)> _lastPositionsAndNMADataBuffer;
            RingBuffer<SimulLvlAndTime> _lastSimulLvlsBuffer = new RingBuffer<SimulLvlAndTime>(RingBuffSize);
            RingBuffer<TypeAndTime> _lastMoveActionsBuffer   = new RingBuffer<TypeAndTime>(RingBuffSize);
            RingBuffer<StringAndTime> _lastSMStatesBuffer    = new RingBuffer<StringAndTime>(RingBuffSize);
            RingBuffer<PZ_13761_Dbg_PositionsAndMNAData> _lastPositionsAndNMADataBuffer     = new RingBuffer<PZ_13761_Dbg_PositionsAndMNAData>(RingBuffSize);
            RingBuffer<PZ_13761_Dbg_PositionsAndMNAData_2> _lastPositionsAndNMADataBuffer_2 = new RingBuffer<PZ_13761_Dbg_PositionsAndMNAData_2>(RingBuffSize);
            RingBuffer<PointAndTime> _bufferNMAWarpTeleport           = new RingBuffer<PointAndTime>(RingBuffSize);
            RingBuffer<PointAndTime> _bufferNMAWarpOutOfNavMesh       = new RingBuffer<PointAndTime>(RingBuffSize);
            RingBuffer<PointAndTime> _bufferNMASetNextPosOffMeshLink  = new RingBuffer<PointAndTime>(RingBuffSize);
            RingBuffer<NMAMoveDatasAndTime> _bufferNmaMoves           = new RingBuffer<NMAMoveDatasAndTime>(100);
            RingBuffer<CalculatePathFailedData> _bufferFailedCalcPath = new RingBuffer<CalculatePathFailedData>(100);
            RingBuffer<float> _bufferLocoUpdTime = new RingBuffer<float>(10);
            RingBuffer<float> _bufferMADUpdTime = new RingBuffer<float>(10);

            const int NumOfLocoNodesThatCommitToLocoVarsAfterAlarm = 6;
            List<LocoVarsAndTime> _locoVarsAfterAlarm = new List<LocoVarsAndTime>(5* NumOfLocoNodesThatCommitToLocoVarsAfterAlarm);

            internal Guid EntId = Guid.Empty;

            private const int RingBuffSize = 20;
            int _alarmTimesCounter = 3; // how many times 1 mob logs after it's broken

            // --- help data ----------------------
            const float SaveDataTimeStep = 1f;
            private MoveActionsDoer _mad;
            internal NavMeshPath LastPath = new NavMeshPath(); // needed only to avoid allocations on every CalcPath call

            // --- Privates: ----------------------
            private NmaObserver _nmaObserver;
            private NavMeshAgent _nma;
            private Transform _go;
            internal bool PZ_13761_InitLocoServ_calledAfter_GotServ;

            // --- Methods: -----------------------
            // internal void Init(MoveActionsDoer mad)
            // {
            //     // _lastSimulLvlsBuffer           = new RingBuffer<SimulLvlAndTime>(RingBuffSize);
            //     // _lastMoveActionsBuffer         = new RingBuffer<TypeAndTime>(RingBuffSize);
            //     // _lastPositionsAndNMADataBuffer = new RingBuffer<PZ_13761_Dbg_PositionsAndMNAData>(RingBuffSize);
            //     _mad = mad;
            //     // lastBeforeInterpolTeleportTime = -1;
            //     // lastAfterInterpolTeleportTime = -1;
            // }
            internal PZ_13761_Dbg_Data(MoveActionsDoer mad, NavMeshAgent nma, Transform goTransform, float awakeTime, Vector3 awakePos, Pawn pawn)
            {
                _mad = mad;
                PosAtAwake = awakePos;
                AwakeTime = awakeTime;
                _nma = nma;
                _go = goTransform;
                _pawn = pawn;
                _nmaObserver = new NmaObserver(nma);

                _mad.ActionStarted += OnActionStarted;
                _mad.CurrActionWillBeFinished += OnActionFinished;

                if (EnableLogZoneBorders)
                {
                    if (LogZoneBorderMin == Vector2.zero || LogZoneBorderMax == Vector2.zero || LogZoneBorderSize == Vector2.zero)
                        throw new Exception("(EnableLogZoneBorders && (LogZoneBorderMin == Vector3.zero || LogZoneBorderMin == Vector3.zero))");
                    if (LogZoneBorderMin.x >= LogZoneBorderMax.x || LogZoneBorderMin.y >= LogZoneBorderMax.y)
                        throw new Exception("(LogZoneBorderMin.x >= LogZoneBorderMax.x || LogZoneBorderMin.y >= LogZoneBorderMax.y).");
                }
            }

            internal void SubscribeToLocoNMANode(LocomotionNavMeshAgent lnma)
            {
                lnma.WarpTeleportEvent          += OnWarpTeleportEvent;
                lnma.WarpOutOfNavMeshEvent      += OnWarpOutOfNavMeshEvent;
                lnma.SetNextPosOffMeshLinkEvent += OnSetNextPosOffMeshLinkEvent;
            }

            internal float _lastTimeDbgDataSaved;
            internal bool ShouldSave(float TimeTimeNow, bool forced = false)
            {
                if (EnableLogZoneBorders)
                {
                    if ( !LogZoneBorder.Contains(new Vector2(_go.position.z,      _go.position.x)) 
                      && !LogZoneBorder.Contains(new Vector2(_nma.nextPosition.z, _nma.nextPosition.x)) )
                        return false;
                }

                switch (Mode)
                {
                    case 1: 
                        if (ShouldSave_1(TimeTimeNow)) 
                            return true;
                        break;
                    case 2:
                        if (ShouldSave_2(TimeTimeNow))
                            return true;
                        break;

                    default: throw new ArgumentException($"Unmaintained Mode == {Mode}");
                }

                return forced;
            }
            // For Mode==1
            internal bool ShouldSave_1(float TimeTimeNow) => (_alarmTimesCounter >= 0) && (_lastTimeDbgDataSaved + SaveDataTimeStep <= TimeTimeNow);
            // For Mode==2
            internal bool ShouldSave_2(float TimeTimeNow) => (_alarmTimesCounter >= 0) && ((_lastTimeDbgDataSaved + SaveDataTimeStep <= TimeTimeNow) 
                                                                                        || _nmaObserver.ShouldSave());
            
            private const int AlarmConditionsCounterDefaultVal = 3;
            private const float SampleToNavMeshR = 3f;
            private int _alarmConditionsCounter = AlarmConditionsCounterDefaultVal;
            private const float AlarmWhenDistGoToNMAGreaterThanSqr = 100 * 100;
            private bool ShouldAlarm(PZ_13761_Dbg_PositionsAndMNAData data)
            {
                NavMeshHit noNeedOut;
                if (!data._pathExist || !data._agentIsOnNavMesh || !NavMesh.SamplePosition(data._goPos, out noNeedOut, SampleToNavMeshR, NavMesh.AllAreas) 
                    || (data._goPos - data._agentPos).sqrMagnitude > AlarmWhenDistGoToNMAGreaterThanSqr)
                    --_alarmConditionsCounter;
                else
                {
                    // reset counter:
                    _alarmConditionsCounter = AlarmConditionsCounterDefaultVal;
                    DumpAndCleanLocoVarsAfterAlarm(); //На старте однажды срабатывает декремент --> без этой строки в лог выбрасывается ненужная куча д-х: по 30 записей из `LocoVarsAfterAlarm` на кажд.моба. 
                }

                return _alarmConditionsCounter < 0;
            }
            private bool ShouldAlarm(PZ_13761_Dbg_PositionsAndMNAData_2 data)
            {
                NavMeshHit noNeedOut;
                if (!data._pathExist || !data._nma_isOnNavMesh || !NavMesh.SamplePosition(data._goPos, out noNeedOut, SampleToNavMeshR, NavMesh.AllAreas)
                    || (data._goPos - data._nma_Pos).sqrMagnitude > AlarmWhenDistGoToNMAGreaterThanSqr)
                {
                    --_alarmConditionsCounter;
                    ShouldCollectLocoVars = true;
                }
                else
                {
                    // reset counter:
                    _alarmConditionsCounter = AlarmConditionsCounterDefaultVal;
                    /// Боюсь потерять важные д-е, поэтому пока комменчу:
                    ///  DumpAndCleanLocoVarsAfterAlarm(); //На старте однажды срабатывает декремент --> без этой строки в лог выбрасывается ненужная куча д-х: по 30 записей из `LocoVarsAfterAlarm` на кажд.моба.
                }

                return _alarmConditionsCounter < 0;
            }

            // For Mode == 1
            internal void Save(PZ_13761_Dbg_PositionsAndMNAData data, bool DBG_Forced_DAMP_PZ_13761 = false)
            {
                var shouldAlarm = ShouldAlarm(data);
                data.AlarmConditionsMetBackCounter = _alarmConditionsCounter;
                _lastPositionsAndNMADataBuffer.Add(data);
                
                if (shouldAlarm || DBG_Forced_DAMP_PZ_13761)
                { 
                    // should Alarm:
                    NavMeshHit noNeedOut;
                    var msg = new StringBuilder($"Id:{EntId}. PosAtAwake: {PosAtAwake}, t:{AwakeTime}, PosGotServer: {PosGotServer}, t:{GotServerTime};  " +
                                                $"PrevNMAPos:{(PrevNMAPos.HasValue ? PrevNMAPos.Value.ToString() : "null")};  " +
                                                $"lastBeforeInterpolTeleportTime:{lastBeforeInterpolTeleportTime}, lastAfterInterpolTeleportTime:{lastAfterInterpolTeleportTime};  " +
                                                $"locoBroken: t:{_locomotionBrokenTime}, msg:\"{_locomotionBrokenMsg}\". " +

                                                $"\n_lastSimulLvlsBuf: {_lastSimulLvlsBuffer.ItemsToStringByLines()};  " +
                                                $"\n_lastMoveActionsBuf: {_lastMoveActionsBuffer.ItemsToStringByLines()};  " +
                                                
                                                $"\n_bufferNMAWarpTeleport: {_bufferNMAWarpTeleport.ItemsToStringByLines()};  " +
                                                $"\n_bufferNMAWarpOutOfNavMesh: {_bufferNMAWarpOutOfNavMesh.ItemsToStringByLines()};  " +
                                                $"\n_bufferNMASetNextPosOffMeshLink: {_bufferNMASetNextPosOffMeshLink.ItemsToStringByLines()};  " +
                                                
                                                $"\n_lastPositionsAndNMADataBuffer: {_lastPositionsAndNMADataBuffer.ItemsToStringByLines()}." +
                                                $"\nAlarmReason: !pthExst:{data._pathExist} || !OnNavMesh:{data._agentIsOnNavMesh} " +
                                                $"|| !SamplePosition:{NavMesh.SamplePosition(data._goPos, out noNeedOut, SampleToNavMeshR, NavMesh.AllAreas)} " +
                                                $"|| goToNmaDist:{(data._goPos - data._agentPos).magnitude} >100.");

                    PZ_13761_Dbg_Data.Logger.IfError()?.Message(msg.ToString()).Write();
                    --_alarmTimesCounter;
                    Clean();
                }

                PrevNMAPos = data._agentPos;
                _lastTimeDbgDataSaved = Time.time;
            }
            // For Mode == 2
            internal void Save(PZ_13761_Dbg_PositionsAndMNAData_2 data, bool forcedDamp = false, string forcedAlarmCauser = null)
            {
                var shouldAlarm = forcedDamp || ShouldAlarm(data);
                data.AlarmCounter = _alarmConditionsCounter;
                _lastPositionsAndNMADataBuffer_2.Add(data);
                
                if (shouldAlarm || forcedDamp)
                { 
                    // should Alarm:
                    NavMeshHit noNeedOut;
                    var msg = new StringBuilder($"2.Id:{EntId}. PosAtAwake: {PosAtAwake}, t:{AwakeTime}, PosGotServer: {PosGotServer}, t:{GotServerTime};  " +
                                                $"PrevNMAPos:{(PrevNMAPos.HasValue ? PrevNMAPos.Value.ToString() : "null")};  " +
                                                $"lastBeforeInterpolTeleportTime:{lastBeforeInterpolTeleportTime}, lastAfterInterpolTeleportTime:{lastAfterInterpolTeleportTime};  " +
                                                $"locoBroken: t:{_locomotionBrokenTime}, msg:\"{_locomotionBrokenMsg}\". " +
                                                $"\n_lastSimulLvlsBuf: {_lastSimulLvlsBuffer.ItemsToStringByLines()};  " +
                                                $"\n_lastMoveActionsBuf: {_lastMoveActionsBuffer.ItemsToStringByLines()};  " +
                                                $"\n_lastSMStatesBuffer: {_lastSMStatesBuffer.ItemsToStringByLines()};  " +
                                                $"\n_LocoVarsAfterAlarm: {_locoVarsAfterAlarm.ItemsToStringByLines()};  " +

                                                $"\n_bufferNMAWarpTeleport: {_bufferNMAWarpTeleport.ItemsToStringByLines()};  " +
                                                $"\n_bufferNMAWarpOutOfNavMesh: {_bufferNMAWarpOutOfNavMesh.ItemsToStringByLines()};  " +
                                                $"\n_bufferNMASetNextPosOffMeshLink: {_bufferNMASetNextPosOffMeshLink.ItemsToStringByLines()};  " +
                                                $"\n_bufferNmaMoves: {_bufferNmaMoves.ItemsToStringByLines()};  " +
                                                $"\n_bufferFailedCalcPath: {_bufferFailedCalcPath.ItemsToStringByLines()};  " +
                                                $"\n_bufferLocoUpdTime: {_bufferLocoUpdTime.ItemsToStringByLines()};  " +
                                                $"\n_bufferMADUpdTime: {_bufferMADUpdTime.ItemsToStringByLines()};  " +

                                                $"\n_lastPositionsAndNMADataBuffer: {_lastPositionsAndNMADataBuffer_2.ItemsToStringByLines()}." +
                                                $"\nAlarmReason: !pthExst:{data._pathExist} " +
                                                    $"|| !OnNavMesh:{data._nma_isOnNavMesh} " +
                                                    $"|| !SamplePosition:{NavMesh.SamplePosition(data._goPos, out noNeedOut, SampleToNavMeshR, NavMesh.AllAreas)} " +
                                                    $"|| goToNmaDist:{(data._goPos - data._nma_Pos).magnitude} >100, " +
                                                    $"|| forcedDamp:{forcedDamp}(\"{(forcedDamp ? forcedAlarmCauser : null)}\")." +
                                                $"\nPZ_13761_InitLocoServ_calledAfter_GotServ:{PZ_13761_InitLocoServ_calledAfter_GotServ}");
                    PZ_13761_Dbg_Data.Logger.IfError()?.Message(msg.ToString()).Write();
                    --_alarmTimesCounter;
                    Clean();
                }

                PrevNMAPos = data._nma_Pos;
                _lastTimeDbgDataSaved = Time.time;
            }
            internal void SimulLvlChanged(SimulationLevel simulationLevel) => _lastSimulLvlsBuffer.Add(new SimulLvlAndTime(simulationLevel, Time.time));
            internal void OnActionStarted()  => _lastMoveActionsBuffer.Add(new TypeAndTime(_mad.CurrentAction?.GetType(), Time.time, true));
            internal void OnActionFinished() => _lastMoveActionsBuffer.Add(new TypeAndTime(_mad.CurrentAction?.GetType(), Time.time, false));
            internal void OnSMStateStarted()  => _lastSMStatesBuffer.Add(new StringAndTime(_pawn._stateMachineS.CurrentStateName, Time.time, true));
            internal void OnSMStateFinished() => _lastSMStatesBuffer.Add(new StringAndTime(_pawn._stateMachineS.CurrentStateName, Time.time, false));

            private bool ShouldCollectLocoVars;
            public bool ShouldSaveLocoVarsAfterAlarm() => ShouldCollectLocoVars;
            internal void SaveLocoVarsAfterAlarmIfShould(/*ref*/ LocomotionVariables vars, Type nodeType)
            {
                if (!ShouldSaveLocoVarsAfterAlarm())
                    return;

                _locoVarsAfterAlarm.Add(new LocoVarsAndTime(vars, nodeType, Time.time));
                if (_locoVarsAfterAlarm.Count == _locoVarsAfterAlarm.Capacity)
                    DumpAndCleanLocoVarsAfterAlarm();
            }
            private void DumpAndCleanLocoVarsAfterAlarm()
            {
                if (_locoVarsAfterAlarm.Count == 0)
                    return;
                PZ_13761_Dbg_Data.Logger.IfWarn()?.Message($"_LocoVarsAfterAlarm[{_locoVarsAfterAlarm.Capacity}]: {_locoVarsAfterAlarm.ItemsToStringByLines()}.").Write();
                _locoVarsAfterAlarm.Clear();
                ShouldCollectLocoVars = false;
            }

            internal void OnGotTeleportBeforeInterpolator() => lastBeforeInterpolTeleportTime = Time.time;
            internal void OnGotTeleportAfterInterpolator()  => lastAfterInterpolTeleportTime = Time.time;

            internal void OnWarpTeleportEvent         (LocomotionVector point) => _bufferNMAWarpTeleport         .Add(new PointAndTime(point, Time.time));
            internal void OnWarpOutOfNavMeshEvent     (LocomotionVector point) => _bufferNMAWarpOutOfNavMesh     .Add(new PointAndTime(point, Time.time));
            internal void OnSetNextPosOffMeshLinkEvent(LocomotionVector point) => _bufferNMASetNextPosOffMeshLink.Add(new PointAndTime(point, Time.time));
            internal void OnNMAMoveEvent(NMAMoveType moveType, (NMAMoveData, NMAMoveData) datas)
            {
                _bufferNmaMoves.Add(new NMAMoveDatasAndTime(moveType, datas.Item1, datas.Item2, Time.time));
                switch (moveType)
                {
                    case NMAMoveType.LNMA_WarpA:        //Fall into next case is Ok.
                    case NMAMoveType.LNMA_WarpB:        //Fall into next case is Ok.
                    case NMAMoveType.LNMA_SetNxtPosA:   //Fall into next case is Ok.
                    case NMAMoveType.LNMA_SetNxtPosB:
                        _pawn.DBG_Forced_DAMP_PZ_13761 = true;
                        break;
                    //No default is Ok.
                }
            }
            internal void OnCalcPathFailedEvent(Vector3 from, Vector3 to, int mask, int agntTypeId) => _bufferFailedCalcPath.Add(new CalculatePathFailedData(from, to, mask, agntTypeId, Time.time));
            internal void OnLocoUpdEvent() => _bufferLocoUpdTime.Add(Time.time);
            internal void OnMADUpdEvent() => _bufferMADUpdTime.Add(Time.time);

            string _locomotionBrokenMsg = null;
            float _locomotionBrokenTime = -1;
            internal void OnLocomotionBroken(string msg)
            {
                if (_locomotionBrokenMsg != null)
                    return;

                _locomotionBrokenMsg = msg;
                _locomotionBrokenTime = Time.time;
            }
            void Clean()
            {
                _locomotionBrokenMsg = null;
                _lastMoveActionsBuffer.Clear();
                _lastSMStatesBuffer.Clear();
                _lastPositionsAndNMADataBuffer.Clear();
                _lastPositionsAndNMADataBuffer_2.Clear();
                _lastSimulLvlsBuffer.Clear();
                _bufferNMAWarpTeleport.Clear();
                _bufferNMAWarpOutOfNavMesh.Clear();
                _bufferNMASetNextPosOffMeshLink.Clear();
                _bufferNmaMoves.Clear();
                _bufferFailedCalcPath.Clear();
                _bufferLocoUpdTime.Clear();
                _bufferMADUpdTime.Clear();
            }

            struct TypeAndTime
            {
                internal Type Type;
                internal float Time;
                internal bool IsStart; //false if finished;

                internal TypeAndTime(Type type, float time, bool isStart)
                {
                    Type = type;
                    Time = time;
                    IsStart = isStart;
                }
                public override string ToString() => $"{Time}: {Type.Name} ({(IsStart ? "ON" : "Off")})";
            }
            struct StringAndTime
            {
                internal string String;
                internal float Time;
                internal bool IsStart; //false if finished;

                internal StringAndTime(string s, float time, bool isStart)
                {
                    String = s;
                    Time = time;
                    IsStart = isStart;
                }
                public override string ToString() => $"{Time}: {String} ({(IsStart ? "ON" : "Off")})";
            }
            struct LocoVarsAndTime
            {
                internal LocomotionVariables Vars;
                internal Type NodeType;
                internal float Time;

                internal LocoVarsAndTime(LocomotionVariables vars, Type nodeType, float time)
                {
                    Vars = vars;
                    NodeType = nodeType;
                    Time = time;
                }
                public override string ToString() => $"{Time}: (({NodeType.Name})): {Vars}.";
            }

            struct SimulLvlAndTime
            {
                internal SimulationLevel Lvl;
                internal float Time;

                internal SimulLvlAndTime(SimulationLevel lvl, float time)
                {
                    Lvl = lvl;
                    Time = time;
                }
                public override string ToString() => $"{Time}: {Lvl}";
            }
            struct PointAndTime
            {
                internal LocomotionVector Point;
                internal float Time;

                internal PointAndTime(LocomotionVector point, float time)
                {
                    Point = point;
                    Time = time;
                }
                public override string ToString() => $"{Time}: {Point.ToWorld().ToUnity()}";
            }

            public struct NMAMoveDatasAndTime
            {
                NMAMoveType _moveType;
                NMAMoveData _data1;
                NMAMoveData _data2;
                float _time;

                public NMAMoveDatasAndTime(NMAMoveType moveType, NMAMoveData data1, NMAMoveData data2, float time)
                {
                    _moveType = moveType;
                    _data1 = data1;
                    _data2 = data2;
                    _time = time;
                }

                public override string ToString()
                {
                    return $"[{_time}]: (({_moveType})) [1]: {_data1};  \n[2]: {_data2}.";
                }
            }

            public struct CalculatePathFailedData
            {
                Vector3 _agntPos;
                Vector3 _rchblPoint;
                int _agntAreaMask;
                int _agntTypeID;

                float _time;

                public CalculatePathFailedData(Vector3 agntPos, Vector3 rchblPoint, int agntAreaMask, int agntTypeID, float time)
                {
                    _agntPos = agntPos;
                    _rchblPoint = rchblPoint;
                    _agntAreaMask = agntAreaMask;
                    _agntTypeID = agntTypeID;
                    _time = time;
                }

                public override string ToString()
                {
                    return $"[{_time}] _agPos: {_agntPos} --> rchblPnt: {_rchblPoint} // areaMask:{_agntAreaMask}, tId:{_agntTypeID}.";
                }
            }

            internal struct PZ_13761_Dbg_PositionsAndMNAData
            {
                internal Vector3 _virtualBodyPos;
                internal Vector3 _goPos;
                internal bool    _agentEnabled;
                internal bool    _agentIsOnNavMesh;
                internal bool    _agentHasPath;
                internal Vector3 _agentPos;
                internal Vector3 _agentSteeringTarget;
                internal bool _pathExist;
                internal int AlarmConditionsMetBackCounter;
                internal float _time;

                internal PZ_13761_Dbg_PositionsAndMNAData(Vector3 virtualBodyPos, Vector3 goPos, bool agentEnabled, bool agentIsOnNavMesh, bool agentHasPath, Vector3 agentPos, Vector3 agentSteeringTarget, bool pathExist, float time)
                {
                    _virtualBodyPos   = virtualBodyPos;
                    _goPos            = goPos;
                    _agentEnabled     = agentEnabled;
                    _agentIsOnNavMesh = agentIsOnNavMesh;
                    _agentHasPath     = agentHasPath;
                    _agentPos         = agentPos;
                    _agentSteeringTarget = agentSteeringTarget;
                    _pathExist        = pathExist;
                    _time             = time;
                    AlarmConditionsMetBackCounter = -1;
                }

                public override string ToString()
                {
                    return $"[{_time}]: AlarmCounter:{AlarmConditionsMetBackCounter}.  _vBPos:{_virtualBodyPos}({(_agentPos - _virtualBodyPos).magnitude}), _goPos:{_goPos}({(_agentPos - _goPos).magnitude}), _agentPos:{_agentPos}, _agentSteeringTarget:{_agentSteeringTarget};  " +
                           $"_agent: Enbld:{_agentEnabled}, OnNavMesh:{_agentIsOnNavMesh}, HasPath:{_agentHasPath}.  _pathExist:{_pathExist}";
                }
            }
            internal struct PZ_13761_Dbg_PositionsAndMNAData_2
            {
                // internal Vector3 _virtualBodyPos;
                internal Vector3 _goPos;
                // internal bool    _agentEnabled;
                // internal bool    _agentIsOnNavMesh;
                // internal bool    _agentHasPath;
                // internal Vector3 _agentPos;
                // internal Vector3 _agentSteeringTarget;

                /// <summary>
                /// Is attempt to calc path from curr NMA pos to prev. success?
                /// </summary>
                internal bool _pathExist;
                internal int AlarmCounter;
                internal float _time;

                // -- NmaData: --
                #region NmaData

                internal bool _nma_enabled;
                internal bool _nma_isOnNavMesh;
                internal bool _nma_isOnOffMeshLink;
                internal bool _nma_isStopped;
                internal bool _nma_hasPath;
                internal bool _nma_pathPending;
                internal bool _nma_isPathStale;
                internal NavMeshPathStatus _nma_pathStatus;
                
                internal float   _nma_speed;
                internal Vector3 _nma_Pos;
                internal Vector3 _nma_steeringTarget;
                internal Vector3 _nma_velocity;
                // private NavMeshPath path;

                #endregion NmaData


                internal PZ_13761_Dbg_PositionsAndMNAData_2(Vector3 goPos, bool nmaEnabled, bool nmaIsOnNavMesh, bool nmaOnOffMeshLink, bool nmaStopped, bool nmaHasPath, bool nmaPathPending, bool nmaPthStale, 
                                                          NavMeshPathStatus nmaPthStatus, Vector3 nmaPos, Vector3 nmaSteeringTarget, float nmaSpeed, Vector3 nmaVelo, bool pathExist, float time)
                {
                    _goPos            = goPos;                  //V

                    _nma_enabled         = nmaEnabled;
                    _nma_isOnNavMesh     = nmaIsOnNavMesh;
                    _nma_isOnOffMeshLink = nmaOnOffMeshLink;
                    _nma_isStopped       = nmaStopped;
                    _nma_hasPath         = nmaHasPath;
                    _nma_pathPending     = nmaPathPending;
                    _nma_isPathStale     = nmaPthStale;
                    _nma_pathStatus      = nmaPthStatus;
                    _nma_Pos             = nmaPos;              //V
                    _nma_steeringTarget  = nmaSteeringTarget;   //V
                    _nma_speed           = nmaSpeed;
                    _nma_velocity        = nmaVelo;
                    
                    _pathExist        = pathExist;
                    _time             = time;           //V
                    AlarmCounter = -1;                  //V
                }

                public override string ToString()
                {
                    return $"[{_time}]: AlarmCounter:{AlarmCounter}.  _pathExist:{_pathExist}.  _goPos:{_goPos}({(_nma_Pos - _goPos).magnitude}), _nma_Pos:{_nma_Pos}, _nma_steeringTarget:{_nma_steeringTarget};  " +
                           $"NMA: Spd:{_nma_speed}, V:{_nma_velocity}\n{_nma_enabled}\t:Enbld, \n{_nma_isOnNavMesh}\t:OnNavMesh, \n{_nma_isOnOffMeshLink}\t:OffMeshLnk, \n{_nma_isStopped}\t:Stopped,  \n{_nma_hasPath}\t:HasPath, " +
                           $"\n{_nma_pathPending}\t:Pth.Pending, \n{_nma_isPathStale}\t:Pth.Stale, " +
                           $"\nPth.Status: {_nma_pathStatus}.";
                }
            }

            internal class NmaObserver
            {
                private NavMeshAgent _nma;

                // -- NmaData: --
                #region NmaData

                private bool _enabled;
                private bool _isOnNavMesh;
                private bool _isOnOffMeshLink;
                private bool _isStopped;
                private bool _hasPath;
                private bool _pathPending;
                private bool _isPathStale;
                private NavMeshPathStatus _pathStatus;

                // private float speed;
                // private Vector3 _nextPosition;
                // private Vector3 _steeringTarget;
                // private Vector3 _velocity;
                // private NavMeshPath path;

                #endregion NmaData

                internal NmaObserver(NavMeshAgent nma)
                {
                    _nma = nma;
                }
                internal bool ShouldSave()
                {
                    var result = NmaDataChanged();
                    SaveNmaData();
                    return result;
                }

                // --- Privates: ---------------------
                void SaveNmaData()
                {
                    _enabled         = _nma.enabled        ;
                    _isOnNavMesh     = _nma.isOnNavMesh    ;
                    _isOnOffMeshLink = _nma.isOnOffMeshLink;
                    _isStopped       = _nma.isStopped      ;
                    _hasPath         = _nma.hasPath        ;
                    _pathPending     = _nma.pathPending    ;
                    _isPathStale     = _nma.isPathStale    ;
                    _pathStatus      = _nma.pathStatus     ;
                }
                bool NmaDataChanged()
                {
                    return
                    _enabled         != _nma.enabled         ||
                    _isOnNavMesh     != _nma.isOnNavMesh     ||
                    _isOnOffMeshLink != _nma.isOnOffMeshLink ||
                    _isStopped       != _nma.isStopped       ||
                    _hasPath         != _nma.hasPath         ||
                    _pathPending     != _nma.pathPending     ||
                    _isPathStale     != _nma.isPathStale     ||
                    _pathStatus      != _nma.pathStatus      ;
                }
            }

        }

        private class DebugTeleportFlagListener : ILocomotionPipelineCommitNode
        {
            public bool IsReady => true;
            internal Action OnGotTeleportFlag;

            internal DebugTeleportFlagListener(Action onGotTeleportFlag)
            {
                OnGotTeleportFlag = onGotTeleportFlag;
            }
            public void Commit(ref LocomotionVariables inVars, float dt)
            {
                if (inVars.Flags.Any(LocomotionFlags.Teleport))
                    OnGotTeleportFlag?.Invoke();
            }
        }

    #endregion PZ-13568:
    }
}
