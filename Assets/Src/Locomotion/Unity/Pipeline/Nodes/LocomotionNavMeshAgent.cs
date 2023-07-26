using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Interfaces;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Locomotion.Utils;
using Assets.Src.Tools;
using ColonyShared.SharedCode.Aspects.Locomotion;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using SharedCode.Utils;
using UnityEngine.AI;
using UnityEngine;
using static Src.Locomotion.DebugTag;
using static Src.Locomotion.LocomotionDebug;
using static Src.Locomotion.Unity.VectorConvert;
using static Src.Locomotion.Unity.VectorUtils;
using Vector3 = UnityEngine.Vector3;


namespace Src.Locomotion.Unity
{
    /// <summary>
    /// It's the locomotion wrapper over Unity NavMeshAgent (farther "nma"). Here we do 2 things:
    ///   1) we set nma `.speed`, `.isStopped`, ... or just call `.Move()` here
    ///   2) we set loco-vars position by nma.
    /// // In FollowPath mode we control only speed. Destination & path are set out of this class (see `FollowPathHelper`)
    /// </summary>
    // There is nothing to clear on Clean_Soft (see `Pawn.CleanLocomotionServer_Soft`), except _agent state, but it's reseted at `Pawn.ApplySimLvlServer` (I hope, its enough)
    public class LocomotionNavMeshAgent : ILocomotionPipelinePassNode
    {
        public static readonly LocomotionLogger Logger = LocomotionLogger.GetLogger("LocomotionNavMesh");
        [NotNull] internal static readonly NLog.Logger UnityLogger = LogManager.GetCurrentClassLogger();

        private readonly NavMeshAgent _agent;
        private readonly ICurveLoggerProvider _curveLogProv;

        ///#PZ-9704: #Dbg:
        //public CycleList<NavMeshAgentDebugData> DbgLast100Datas;
        public List<NavMeshAgentDebugData> DbgLast100Datas;
        
        ///#PZ-13568: #Dbg:
        Action<NMAMoveType, (NMAMoveData, NMAMoveData)> _onNMAMoveEventHndlr;
        NLog.Logger PZ_13761_Logger;
        bool Dbg_PZ_13761_Enabled => GlobalConstsHolder.Dbg_PZ_13761_DebuggingEnabled;

        public LocomotionNavMeshAgent(
            [NotNull] NavMeshAgent agent,
            Guid dbg_entityId,
            [CanBeNull] ICurveLoggerProvider curveLogProv,
            Action<NMAMoveType, (NMAMoveData, NMAMoveData)> onNMAMoveEventHndlr,
            Func<bool> shouldSaveLocoVars,
            Action<LocomotionVariables, Type> saveLocoVarsCallback,
            NLog.Logger pZ_13761_Logger)
        {
            if (agent == null) throw new ArgumentNullException(nameof(agent));
            _agent = agent;
            _agent.updatePosition = false; // выключаем, только для того чтобы объект мог двигаться по вертикали (прыгать) 
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.acceleration = 1000000;
            _agent.autoTraverseOffMeshLink = false;
            _agent.autoBraking = false;

            _curveLogProv = curveLogProv;
            _dbg_entityId = dbg_entityId;
            _onNMAMoveEventHndlr = onNMAMoveEventHndlr;

            ShouldSaveLocoVars = shouldSaveLocoVars;
            SaveLocoVarsCallback = saveLocoVarsCallback;
            PZ_13761_Logger = pZ_13761_Logger;
        }

        ///#Dbg
        readonly Guid _dbg_entityId;

        ///#PZ-13761: #Dbg:
        public event Action<LocomotionVector> WarpTeleportEvent;
        public event Action<LocomotionVector> WarpOutOfNavMeshEvent;
        public event Action<LocomotionVector> SetNextPosOffMeshLinkEvent;
        ///#PZ-13568: #Dbg:
        protected Func<bool> ShouldSaveLocoVars;
        protected Action<LocomotionVariables, Type> SaveLocoVarsCallback;

        bool ILocomotionPipelinePassNode.IsReady => true;

        public void Reset(Vector3 pos)
        {
            // Наверное не нужно, т.к. агент ресетится в Pawn (sim.Lvl.Change(Dead) --> ApplSimLvlServ и сопается/перезапускается там же (_agentS.enabled = false/true)
            throw new NotImplementedException();
            
            _agent.angularSpeed = 0f;
            _agent.nextPosition = pos;
            _agent.destination = _agent.nextPosition;
         
        }

        LocomotionVariables ILocomotionPipelinePassNode.Pass(LocomotionVariables vars, float dt)
        {
            LocomotionVector newPosition = vars.Position + vars.ExtraPosition;

            if (GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef) && GlobalConstsDef.DebugFlagsGetter.IsDebugMobsHard_DANGER(GlobalConstsHolder.GlobalConstsDef))
            {
                if (false) Logger.IfDebug()?.Message($"{_dbg_entityId} | newPos:{newPosition},  = vars.Pos:{vars.Position} + vars.ExtraPos:{vars.ExtraPosition}").Write();

                //#Dbg#Hard:
                ///#PZ-10393: #Dbg#Hard:
                if (false) if (GlobalConstsDef.DebugFlagsGetter.IsDebugMobs(GlobalConstsHolder.GlobalConstsDef) && GlobalConstsDef.DebugFlagsGetter.IsDebugMobsHard_DANGER(GlobalConstsHolder.GlobalConstsDef))
                {
                    if (DbgLast100Datas == null)
                        DbgLast100Datas = new List<NavMeshAgentDebugData>(100); //new CycleList<NavMeshAgentDebugData>(100);

                    if (DbgLast100Datas.Count < 100)
                        DbgLast100Datas.Add(new NavMeshAgentDebugData(vars.Flags.Any(LocomotionFlags.FollowPath),
                                                                      vars.Flags.Any(LocomotionFlags.Teleport),
                                                                      _agent.isOnNavMesh, 
                                                                      _agent.isOnOffMeshLink,
                                                                      _agent.nextPosition, 
                                                                      LocomotionToWorldVectorAndToUnity(ref newPosition), vars.Flags));
                }
            }

            if (vars.Flags.Any(LocomotionFlags.Teleport))
            {
                WarpTeleportEvent?.Invoke(newPosition);
                WarpNavmeshAgent(newPosition);
            }
            else
            {
                if (!_agent.isOnNavMesh)
                {
                    // режим движения вне навмеша 
                    WarpOutOfNavMeshEvent?.Invoke(newPosition);
                    WarpNavmeshAgent(newPosition);
                    Logger.IfTrace()?.Message($"Out of NavMesh moving. Position:{newPosition}").Write();
                }
                else if (_agent.isOnOffMeshLink)
                {
                    // режим движения по лиинку 
                    SetNextPosOffMeshLinkEvent?.Invoke(newPosition);

                    if (Dbg_PZ_13761_Enabled)
                    { 
                        var log1 = new NMAMoveData(_agent, newPosition.ToWorld().ToUnity());

                        _agent.nextPosition = LocomotionToWorldVectorAndToUnity(ref newPosition);

                        var log2 = new NMAMoveData(_agent, NMAMoveData.NanV3);
                        ///#PZ-13568: #Dbg:
                        _onNMAMoveEventHndlr?.Invoke(NMAMoveType.SetNxtPos, (log1, log2));
                    }
                    else
                        _agent.nextPosition = LocomotionToWorldVectorAndToUnity(ref newPosition);

                    Logger.IfTrace()?.Message($"On NavMeshLink moving. Position:{newPosition}").Write();
                }
                else
                {
                    if (vars.Flags.Any(LocomotionFlags.FollowPath)) // флаг взводит SM.State.Execute
                    {
                        // Режим движения по пути - движением руководит NavMeshAgent, а мы только задаём скорость движения
                        var dir = (_agent.steeringTarget - _agent.nextPosition).normalized;
                        var v = new LocomotionVector(vars.Velocity.Horizontal);
                        _agent.speed = Vector3.Dot(LocomotionToWorldVectorAndToUnity(ref v), dir);
                        _agent.isStopped = false;
                    }
                    else if (!_agent.isStopped)
                    {
                        ///#PZ-13568:
                        //#Note: Без этой отдельной ветки "else if" (когда ".isStopped = true" было внутри просто else), получалось, что на большом dt (до 10 сек., когда моб далеко) часто бывало, что большинство пути 
                        //.. NMA нашагивал в том промежутке м/у тиками Loco, где во время второго из пары тиков уже флага FP нет и получ-ся, что мы забиваем на пройденный со времени предыд-го тика Loco путь и вместо
                        //.. того, чтобы выставить body (==>но и g.o.) в позицию NMA, мы наоборот - мували NMA обратно. - глупая работа и лишнее поле д/ ошибок позиции и расхождений NMA и body(g.o.).
                        _agent.isStopped = true;
                    }
                    else
                    {
                        var worldNewPos = LocomotionToWorldVectorAndToUnity(ref newPosition);
                        if (!_agent.nextPosition.ApproximatelyEqual(worldNewPos, 0.001f)) ///#PZ-13568: attempt to avoid .Move call to same pos. (seen in logs)
                        { 
                            var prevPos = _agent.nextPosition;

                            NMAMoveData? log2 = null;
                            if (Dbg_PZ_13761_Enabled)
                            { 
                                var log1 = new NMAMoveData(_agent, worldNewPos);

                                // Режим свободного движения - движением руководит locomotion, как обычно.
                                _agent.Move(worldNewPos - _agent.nextPosition);

                                /*var*/ log2 = new NMAMoveData(_agent, NMAMoveData.NanV3);
                                ///#PZ-13568: #Dbg:
                                _onNMAMoveEventHndlr?.Invoke(NMAMoveType.Move, (log1, log2.Value));
                            }
                            else
                                _agent.Move(worldNewPos - _agent.nextPosition);

                            ///#PZ-13568: Attempt to handle agnt moved in wierd (notOnNM, but thinks On) state:
                            // "dt > 0.5f" is needed, 'cos mobs closer to player hasn't problems with this logic, but updates much more frequently (`SamplePosition` is heavy, so shouldn't be called on frequent calls)
                            if (dt > 0.5f && !NavMesh.SamplePosition(_agent.nextPosition, out _, 0.1f, /*NavMesh.AllAreas*/_agent.areaMask))//new NavMeshQueryFilter { areaMask = 1, agentTypeID = _agent.agentTypeID }))
                            {
                                var t = Time.time;
                                var samplePrev   = NavMesh.SamplePosition(prevPos, out _, 0.1f, /*NavMesh.AllAreas*/_agent.areaMask);
                                var warpTo = prevPos;
                                bool samplePrev05 = samplePrev;
                                if (!samplePrev)
                                {
                                    samplePrev05 = NavMesh.SamplePosition(prevPos, out var hit, 0.5f, /*NavMesh.AllAreas*/_agent.areaMask);
                                    if (samplePrev05)
                                        warpTo = hit.position;
                                    if (Debug.isDebugBuild) LogToPZ_13761_Logger_Error(-1, $"[{t}] LocoNMA (!Prev): {_dbg_entityId} !NavMesh.SamplePosition(prev): 0.1:false, 0.5:{samplePrev05} after Move: prev:{prevPos} --> trgt:{worldNewPos} == curr:{_agent.nextPosition}");
                                }

                                var sample05     = NavMesh.SamplePosition(_agent.nextPosition, out _, 0.5f, /*NavMesh.AllAreas*/_agent.areaMask);
                                //PZ_13761_Logger.Error
                                if (Debug.isDebugBuild) LogToPZ_13761_Logger_Error(0, $"[{t}] LocoNMA (0): {_dbg_entityId} !NavMesh.SamplePosition after Move (0.5?:{sample05}, prev?:{samplePrev}, prev.0.5?:{samplePrev05}): prev:{prevPos} --> trgt:{worldNewPos} == curr:{_agent.nextPosition}");
                                if (!_agent.Warp(warpTo))
                                {
                                    //PZ_13761_Logger.Error
                                    if (Debug.isDebugBuild) LogToPZ_13761_Logger_Error(1, $"[{t}] LocoNMA (1): {_dbg_entityId} !_agent.Warp(warpTo:{warpTo}) after bad Move: prev:{prevPos} --> trgt:{worldNewPos} == curr:{_agent.nextPosition}");

                                    if (false) //#PZ-13568: #Tmp off, should be on if log "LocoNMA (1)" 'll be met in logs.
                                    { 
                                    var log3a = new NMAMoveData(_agent, warpTo);
                                    _onNMAMoveEventHndlr?.Invoke(NMAMoveType.LNMA_WarpA, (log2.Value, log3a));
                                    
                                    _agent.nextPosition = warpTo;
                                    NMAMoveType mt;
                                    if (!_agent.nextPosition.ApproximatelyEqual(warpTo))
                                    {
                                        PZ_13761_Logger.IfError()?.Message($"[{t}] LocoNMA (2): {_dbg_entityId} !_agent.nextPosition.ApproximatelyEqual(warpTo:{warpTo}) after set nextPos (after failed warp after bad Move): prev:{prevPos} --> trgt:{newPosition} == curr:{_agent.nextPosition}").Write();
                                        mt = NMAMoveType.LNMA_SetNxtPosA;
                                    }
                                    else
                                        mt = NMAMoveType.LNMA_SetNxtPosB;

                                    var log4 = new NMAMoveData(_agent, warpTo);
                                    _onNMAMoveEventHndlr?.Invoke(mt, (log3a, log4));
                                    }
                                }
                                else if (Dbg_PZ_13761_Enabled)
                                {
                                    var log3b = new NMAMoveData(_agent, warpTo);
                                    _onNMAMoveEventHndlr?.Invoke(NMAMoveType.LNMA_WarpB, (log2.Value, log3b));
                                }
                            }
                        }
                    }
                    var agentPosition = _agent.nextPosition.ToLocomotion();
                    //#Note: Изначально тут было так:
                    //   var vertical = newPosition.Vertical;
                    // Затем я исправил на:
                    //   var vertical = Math.Max(newPosition.Vertical, agentPosition.Vertical)
                    // , чтобы body (след-но g.o.) не проваливать под землю.
                    // (НО: обратное нельзя, т.к. мобы не смогут прыгать и падать - будем из мгновенно к земле прилеплять).
                    // Вроде помогло, но осталась проблема, что на большом тике Loco NMA убегает далеко в промежутке м/у тиками Loco. На момент очередного тика Loco флага FollowPath уже нет, поэтому двигаем не себя в позицию NMA, а наоборот.
                    // Это неважная деталь, а важно то, что из-за большого расстояния от go до NMA получ-ся , что перепад в рельефе может быть значительным. И получ-ся так:
                    // GroundSensor.DistToGround рассчит-ся в update (когда NMA бог знает где) и рассчит-ся как: body.pos - nma.pos. Нода физики отраб-ет с этим знач-ем, после чего в этом методе LocoNMA возвращаем для коммита в body position,
                    // который (если, как было, берёт Y из прилетевших vars (и даже после фикса Max(..) чинит проваливание под землю, но не чинит позиционирование body над землёй.
                    // Чиним пока костылём: опираеся на dt - если оно > 1сек, то значит это редкие тики Loco, значит никого нет рядом и безопасно для логики просто выставлять body в Y NMA.
                    // Тут видимо пострадают прыжки и падения - моб оказывается "приклеенным" к навмешу по вертикали, но для оч.далёких мобов это предположительно норм (они даже вряд ли пытаются на таком расстоянии что-то такое делать).
                    var vertical = (dt < 1f)
                        ? Math.Max(newPosition.Vertical, agentPosition.Vertical)
                        : agentPosition.Vertical;

                    newPosition = new LocomotionVector(agentPosition.Horizontal, vertical);
                    ///#PZ-13568:
                    //Возможно сэмплить целевую точку для .Move об навмеш, чтобы не выкинуть NMA куда-то.


                    Logger.IfTrace()?.Message($"NavMesh moving. Position:{newPosition}").Write();
                }
            }

            DebugAgent.Set(NavMeshPosition, _agent.nextPosition.ToShared());
            DebugAgent.Set(NavMeshOffLink, _agent.isOnOffMeshLink);

            ///#PZ-13568: #Dbg: #Tmp replaced by next lines:
            // return new LocomotionVariables(vars) { Position = newPosition, ExtraPosition = LocomotionVector.Zero };
            var result = new LocomotionVariables(vars) { Position = newPosition, ExtraPosition = LocomotionVector.Zero };
            if (ShouldSaveLocoVars?.Invoke() ?? false)
                SaveLocoVarsCallback(result, this.GetType());

            _curveLogProv?.CurveLogger?.AddData("NavMeshAgent.Result.Pos", SyncTime.Now, result.Position);
            
            return result;

            //Dbg:
            // var resultVars = new LocomotionVariables(vars) {Position = newPosition, ExtraPosition = LocomotionVector.Zero};
            // _curveLogProv?.CurveLogger?.IfActive?.AddData("0.4) NavMeshAgnt.Velo", SyncTime.Now, resultVars.Velocity);
            // return resultVars;
        }

        //#PZ-13568: Чтобы не засорять лог, но в то же время не терять важную инфу о том, что NMA.Move ломается, аккумулируем тут сообщения об ошибках и выдаём их партиями (c прогрессирующим объёмом аккумулятора от 1 до 100 с шагом в 10)
        Dictionary<int, List<string>> PZ_13761_Errors_Accumulator = new Dictionary<int, List<string>>();
        int PZ_13761_Errors_Accumulator_List_Capacity = 1;
        int PZ_13761_Errors_Accumulator_List_Capacity_Step = 10;
        int PZ_13761_Errors_Accumulator_List_Capacity_Max = 100;
        void LogToPZ_13761_Logger_Error(int msgType, string msg)
        {
            if (Dbg_PZ_13761_Enabled)
                PZ_13761_Logger.IfError()?.Message(msg).Write();
            else
            {
                List<string> list;
                if (!PZ_13761_Errors_Accumulator.TryGetValue(msgType, out list))
                {
                    list = new List<string>(PZ_13761_Errors_Accumulator_List_Capacity);
                    PZ_13761_Errors_Accumulator.Add(msgType, list);
                }
                list.Add(msg);
                if (list.Count == list.Capacity)
                {
                    PZ_13761_Logger.IfError()?.Message(list.ItemsToStringByLines()).Write();
                    list.Clear();
                    if (list.Capacity < PZ_13761_Errors_Accumulator_List_Capacity_Max)
                        list.Capacity += PZ_13761_Errors_Accumulator_List_Capacity_Step;
                }
            }
        }

        // Moved here from SpawnZone ('cos of references)
        //private const float MaxOffset = 300;
        private const float StartSampleRadius = 1;
        private const float SampleIteration = 4;
        private const float SampleRadiusGrowFactor = 2;
        //Using of these two fixes Unity `NavMesh.SamplePosition` issue:
        private const float SampleRadiusStep = 1.1f;
        private const float SampleCenterOffsetStep = 0.9f;

        public static NavMeshHit GetSurface(GameObject executor, LocomotionVector point, int areaMask = NavMesh.AllAreas) =>
            GetSurface(executor, LocomotionToWorldVectorAndToUnity(ref point), areaMask);

        public static NavMeshHit GetSurface_RES(GameObject executor, Vector3 point, int areaMask = NavMesh.AllAreas, bool silentMode = false)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, StartSampleRadius, areaMask))
                return hit;

            var sampleRadius = StartSampleRadius;
            var yDiff = StartSampleRadius;
            for (int i = 1; i <= SampleIteration; i++)
            {
                var upperSourcePoint = new Vector3(point.x, point.y + yDiff, point.z);
                if (NavMesh.SamplePosition(upperSourcePoint, out hit, sampleRadius, areaMask))
                    return hit;

                var downSourcePoint = new Vector3(point.x, point.y - yDiff, point.z);
                if (NavMesh.SamplePosition(downSourcePoint, out hit, sampleRadius, areaMask))
                    return hit;

                sampleRadius *= SampleRadiusGrowFactor;
                yDiff += sampleRadius - 0.1f;
            }
            
            if (!silentMode)
                UnityLogger.IfError()?.Message($"Failed to find NavMesh around {point} for obj {executor.name} SpawnZone (mask:{areaMask}).").UnityObj(executor).Write();

            //#no_need:
            ///if (areaMask == NavMesh.AllAreas)
            ///    return GetSurface(executor, point, 1, silentMode);
            ///else 
                return new NavMeshHit();
        }
        //#Hack fixes Unity `NavMesh.SamplePosition` bug:
        public static NavMeshHit GetSurface(GameObject executor, Vector3 point, int areaMask = NavMesh.AllAreas, bool silentMode = false)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, StartSampleRadius, areaMask))
                return hit;

            //Old: var sampleRadius = StartSampleRadius;
            //Old: var yDiff = StartSampleRadius;
            for (int i = 1; i <= SampleIteration; i++)
            {
                var sampleRadius = i * SampleRadiusStep;
                var yDiff = i * SampleCenterOffsetStep;

                var upperSourcePoint = new Vector3(point.x, point.y + yDiff, point.z);
                if (NavMesh.SamplePosition(upperSourcePoint, out hit, sampleRadius, areaMask))
                    return hit;

                var downSourcePoint = new Vector3(point.x, point.y - yDiff, point.z);
                if (NavMesh.SamplePosition(downSourcePoint, out hit, sampleRadius, areaMask))
                    return hit;

                //Old: sampleRadius *= SampleRadiusGrowFactor;
                //Old: yDiff += sampleRadius - 0.1f;
            }
            
            if (!silentMode)
                UnityLogger.IfError()?.Message($"Failed to find NavMesh around {point} for obj {executor.name} SpawnZone (mask:{areaMask}).").UnityObj(executor).Write();
            
            //#no_need:
            /// if (areaMask == NavMesh.AllAreas)
            ///{
            ///    var onlyWalkableHit = GetSurface(executor, point, 1, silentMode);
            ///    if (onlyWalkableHit.hit)
            ///        if (DbgLog.Enabled) DbgLog.LogErr("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ onlyWalkableHit.hit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            ///    return onlyWalkableHit;
            ///}
            /// else 
                return new NavMeshHit();
        }


        public static bool TryGetSurface(GameObject executor, Vector3 point, out NavMeshHit outHit, int areaMask = NavMesh.AllAreas) 
        {
            outHit = GetSurface(executor, point, areaMask, true);
            return outHit.hit;
        }

        private void WarpNavmeshAgent(LocomotionVector newPosition)
        {
            var pos = LocomotionToWorldVectorAndToUnity(ref newPosition);
            var hit = GetSurface(_agent.gameObject, pos);
            if (!hit.hit)
                throw new ArgumentException($"Can't sample point to navmesh! (pos: {LocomotionToWorldVectorAndToUnity(ref newPosition)}).  " +
                                            //$"DbgLast100Datas: {(DbgLast100Datas != null ? DbgLast100Datas.ToStringCustom(";\n") : "null")}.");
                                            $"DbgLast100Datas: {(DbgLast100Datas != null ? string.Join(";\n", DbgLast100Datas) : "null")}.");
            Logger.IfDebug()?.Message("#WARP: " + hit.position).Write();

            var log1 = new NMAMoveData(_agent, hit.position);

            _agent.Warp(hit.position);

            var log2 = new NMAMoveData(_agent, NMAMoveData.NanV3);
            ///#PZ-13568: #Dbg:
            _onNMAMoveEventHndlr?.Invoke(NMAMoveType.Warp, (log1, log2));
        }
    }


    // --- Util types: -----------------------------

    public struct NavMeshAgentDebugData
    {
        public bool IsFlag_FollowPath;
        public bool IsFlag_Teleport;
        public bool IsOnNavMesh;
        public bool IsOnOffMeshLink;
        public Vector3 CurrPos;
        public Vector3 NewPos;
        public LocomotionFlags Flags;

        public NavMeshAgentDebugData(bool isFlag_FollowPath, bool isFlag_Teleport, bool isOnNavMesh, bool isOnOffMeshLink, Vector3 currPos, Vector3 newPos, LocomotionFlags flags)
        {
            IsFlag_FollowPath = isFlag_FollowPath;
            IsFlag_Teleport = isFlag_Teleport;
            IsOnNavMesh = isOnNavMesh;
            IsOnOffMeshLink = isOnOffMeshLink;
            CurrPos = currPos;
            NewPos = newPos;
            Flags = flags;
        }

        public override string ToString()
        {
            return $"IsFlag_FollowPath: {IsFlag_FollowPath};  IsFlag_Teleport: {IsFlag_Teleport};  IsOnNavMesh: {IsOnNavMesh};  IsOnOffMeshLink: {IsOnOffMeshLink};  CurrPos: {CurrPos};  Flags: {Flags}.";
        }
    }

    ///#PZ-13568: #Dbg:
    #region #PZ-13568: #Dbg:

    /// <summary>
    /// Данные о муве (.Move(..)) NMA'нта.
    /// </summary>
    public struct NMAMoveData
    {
        Vector3 _agntNextPos;
        Vector3 _trgPos;
        Vector3 _agntDestination;
        bool _agntHasPath;
        int _pathLen;
        Vector3 _pa1st;
        Vector3 _paLast;
        //bool _agntIsStopped;

        public static Vector3 NanV3 = new Vector3(float.NaN, float.NaN, float.NaN);

        public NMAMoveData(NavMeshAgent agent, Vector3 newPos)
        {
            _agntNextPos     = agent.nextPosition;
            _agntDestination = agent.destination;
            _agntHasPath     = agent.hasPath;
            //_agntIsStopped   = agent.isStopped;

            _trgPos = newPos;

            _pathLen = -1;
            _pa1st  = NanV3;
            _paLast = NanV3;

            if (agent.path?.corners != null)
            {
                _pathLen = agent.path.corners.Length;
                if (_pathLen > 0)
                {
                    _pa1st  = agent.path.corners[0];
                    _paLast = agent.path.corners[_pathLen - 1];
                }
            }
        }

        public override string ToString()
        {
            return /*$"stopd: {_agntIsStopped},  */$"hasPth: {_agntHasPath},  pthL: {_pathLen}.  Pos: {_agntNextPos} --> {_trgPos}.  destin:{_agntDestination}.  Pth[0, Z]:[{_pa1st}, {_paLast}].";
        }
    }
    public enum NMAMoveType
    {
        Move,
        SetNxtPos,
        Warp,
        
        LNMA_WarpA,      //Warp back handling (lvl1) failed
        LNMA_WarpB,      //Warp back handling (lvl1) success
        LNMA_SetNxtPosA, //Set nextPos handling (lvl2) failed
        LNMA_SetNxtPosB, //Set nextPos handling (lvl2) success

        // from Pawn:
        P_SetNxtPos,
        P_Warp,
    }

    #endregion #PZ-13568: #Dbg:
}