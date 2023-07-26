using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.ColonyShared.SharedCode.Utils.Statistics;
using Assets.Src.Lib.Extensions;
using Core.Environment.Logging.Extension;
using NLog;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.NetworkedMovement
{
    public partial class Pawn
    {
        internal static class PawnWatchDog
        {
            private const bool IsEnable = false;
            private static readonly NLog.Logger ToFileLogger = LogManager.GetLogger("PawnWatchDog_ToFile"); 

            private static readonly TimeSpan UpdateTimeStep = TimeSpan.FromSeconds(60);
            private static readonly List<Pawn> Pawns = new List<Pawn>();

            // --- API ------------------------------------------

            internal static void RegisterServerPawn(Pawn p)
            {
                if (!IsEnable || !Debug.isDebugBuild)
                    return;
                Pawns.Add(p);
                p.EnableCollectDebugData();
                StartUpdateRoutineIfShould();
            }

            internal static void UnregisterServerPawn(Pawn p)
            {
                if (!IsEnable)
                    return;
                p.DisableCollectDebugData();
                Pawns.Remove(p);
                if (Pawns.Count == 0)
                    _stopTokenSource?.Cancel();
            }

            // --- Privates: -------------------------------------
            private static Task _updateRoutineTask;
            private static CancellationTokenSource _stopTokenSource;

            static void StartUpdateRoutineIfShould()
            {
                if (_updateRoutineTask != null && !_updateRoutineTask.IsCompleted)
                    return;

                _stopTokenSource = new CancellationTokenSource();
                _updateRoutineTask = StartUpdateRoutine();
            }

            static async Task StartUpdateRoutine()
            {
                 Logger.IfDebug()?.Message("#Dbg: PawnWatchDog.StartUpdateRoutine").Write();;

                try
                {
                    await UpdateCycle();
                }
                catch (Exception e)
                {
                    if (e is TaskCanceledException)
                        Logger.IfDebug()?.Message($"`{nameof(PawnWatchDog)}` update routine task is canceled.").Write();
                    else
                    {
                        Logger.IfError()?.Message("##2 " + e).Write();
                        throw;
                    }
                }
            }

            private static int _totalN;
            private static int _offN;
            private static int _suspendedN;
            private static int _farN;
            // private static int _fullTrueN;
            private static int _fullWithEnormousDistanceN;
            // private static float _closestPlayerDistforFull_Summ;
            // private static float _closestPlayerDistforFull_Min;
            // private static float _closestPlayerDistforFull_Max;

            private static readonly MinMaxAvgRegistratorFloat FullMinMaxAvg = new MinMaxAvgRegistratorFloat();

            // UPS - Updates per second
            private static readonly MinMaxAvgRegistratorFloat OffLocoCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();
            private static readonly MinMaxAvgRegistratorFloat SuspLocoCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();
            private static readonly MinMaxAvgRegistratorFloat FarLocoCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();
            private static readonly MinMaxAvgRegistratorFloat FullLocoCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();

            private static readonly MinMaxAvgRegistratorFloat OffMoveActionsCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();
            private static readonly MinMaxAvgRegistratorFloat SuspMoveActionsCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();
            private static readonly MinMaxAvgRegistratorFloat FarMoveActionsCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();
            private static readonly MinMaxAvgRegistratorFloat FullMoveActionsCoroutineUpsMinMaxAvg = new MinMaxAvgRegistratorFloat();

            private static readonly List<Guid> _pawnsNear000 = new List<Guid>();

            static void Clean()
            {
                _totalN = _suspendedN = _farN /*= _fullTrueN*/ = _fullWithEnormousDistanceN = 0;
                // _closestPlayerDistforFull_Summ = 0f;
                // _closestPlayerDistforFull_Min = float.MaxValue;
                // _closestPlayerDistforFull_Max = -1f;
                FullMinMaxAvg.BeginNewSample();

                OffLocoCoroutineUpsMinMaxAvg.BeginNewSample();
                SuspLocoCoroutineUpsMinMaxAvg.BeginNewSample();
                FarLocoCoroutineUpsMinMaxAvg.BeginNewSample();
                FullLocoCoroutineUpsMinMaxAvg.BeginNewSample();

                OffMoveActionsCoroutineUpsMinMaxAvg.BeginNewSample();
                SuspMoveActionsCoroutineUpsMinMaxAvg.BeginNewSample();
                FarMoveActionsCoroutineUpsMinMaxAvg.BeginNewSample();
                FullMoveActionsCoroutineUpsMinMaxAvg.BeginNewSample();

                _pawnsNear000.Clear();
            }

            static async Task UpdateCycle()
            {
                while (true)
                {
                    if (LocomotionProfiler2.EnableProfile) LocomotionProfiler2.BeginSample("## PawnWatchDog.UpdateCycle");

                    if (_stopTokenSource.Token.IsCancellationRequested)
                        return;

                    Clean();

                    foreach (var pawn in Pawns)
                    {
                        if (pawn == null) //plug (pawn.transform causes NullRefException) 
                        {
                             Logger.IfError()?.Message("pawn == null").Write();;
                            continue;
                        }

                        // #order! 1
                        InspectCoroutinesUps(pawn);

                        var simulationLvl = (SimulationLevel) pawn._simulationLevelServer;

                        ++_totalN;
                        // #order! 2
                        MinMaxAvgRegistratorFloat locoCoroutineUpsMinMaxAvg = null;
                        MinMaxAvgRegistratorFloat moveActionsCoroutineUpsMinMaxAvg = null;
                        switch (simulationLvl)
                        {
                            case SimulationLevel.Off:
                                ++_offN;
                                locoCoroutineUpsMinMaxAvg = OffLocoCoroutineUpsMinMaxAvg;
                                moveActionsCoroutineUpsMinMaxAvg = OffMoveActionsCoroutineUpsMinMaxAvg;
                                break;
                            case SimulationLevel.Suspended:
                                ++_suspendedN;
                                locoCoroutineUpsMinMaxAvg = SuspLocoCoroutineUpsMinMaxAvg;
                                moveActionsCoroutineUpsMinMaxAvg = SuspMoveActionsCoroutineUpsMinMaxAvg;
                                break;
                            case SimulationLevel.Faraway:
                                ++_farN;
                                locoCoroutineUpsMinMaxAvg = FarLocoCoroutineUpsMinMaxAvg;
                                moveActionsCoroutineUpsMinMaxAvg = FarMoveActionsCoroutineUpsMinMaxAvg;
                                break;
                            case SimulationLevel.Full:
                                locoCoroutineUpsMinMaxAvg = FullLocoCoroutineUpsMinMaxAvg;
                                moveActionsCoroutineUpsMinMaxAvg = FullMoveActionsCoroutineUpsMinMaxAvg;
                                var dist = Math.Min(pawn._relevanceProviderUpdateCl.ClosestObserverDistance, pawn._relevanceProviderUpdateS.ClosestObserverDistance);
                                if (dist > GlobalConstsHolder.GlobalConstsDef.VisibilityDistance)
                                {
                                    dist = Math.Min(pawn._relevanceProviderUpdateCl.Dbg_ClosestObserverDistance_Forced_DANGER, pawn._relevanceProviderUpdateS.Dbg_ClosestObserverDistance_Forced_DANGER);
                                    if (GlobalConstsHolder.Dbg_Mobs7910 && dist > GlobalConstsHolder.GlobalConstsDef.VisibilityDistance)
                                    {
                                        ++_fullWithEnormousDistanceN;
                                        var msg = $"SimulationLevel.Full, but dist > 100: {dist}. {pawn.EntityId}";
                                        Logger.IfWarn()?.Message(msg).Write();
                                        ToFileLogger.IfWarn()?.Message(msg).Write();
                                        break;
                                    }
                                }

                                FullMinMaxAvg.RegisterNewValue(dist);
                                // ++_fullTrueN;
                                // if (dist > _closestPlayerDistforFull_Max)
                                //     _closestPlayerDistforFull_Max = dist;
                                // else if (dist < _closestPlayerDistforFull_Min)
                                // {
                                //     if (dist < 0f)
                                //         if (DbgLog.Enabled) DbgLog.Log("dist < 0f : WTF?");
                                //     _closestPlayerDistforFull_Min = dist;
                                // }
                                // 
                                // _closestPlayerDistforFull_Summ += dist;
                                break;
                            default: break;
                        }

                        locoCoroutineUpsMinMaxAvg?.RegisterNewValue(pawn.CurrLocoCoroutineCyclesPerSec);
                        moveActionsCoroutineUpsMinMaxAvg?.RegisterNewValue(pawn.CurrMoveActionsCoroutineCyclesPerSec);

                        if (Vector3Extension.Approx3(pawn.transform.position, Vector3.zero, 10f))
                            _pawnsNear000.Add(pawn.EntityId);
                    }

                    float fullAvg, fullMin, fullMax, last;
                    FullMinMaxAvg.GetSampleData(out fullMin, out fullMax, out float _, out fullAvg);
                    ToFileLogger.Trace(
                        "Total: {0}, Off: {1}, Suspended: {2}, Far: {3}, Full: {4} (BAD_full: {8}). For Full closestDist Avg / Min / Max: {5} / {6} / {7}. \t" +
                        "LocoCoroutineCyclesPerSec Avg / Min / Max: for Off:  {9} / {10} / {11}. \t" +
                        "for Suspended: {12} / {13} / {14}. \t" +
                        "for Far: {15} / {16} / {17}. \t" +
                        "for Full: {18} / {19} / {20}. \t" +
                        "MoveActionsCoroutineCyclesPerSec Avg / Min / Max: for Off: {21} / {22} / {23}. \t" +
                        "for Suspended: {24} / {25} / {26}. \t" +
                        "for Far: {27} / {28} / {29}. \t" +
                        "for Full: {30} / {31} / {32}. \t" +
                        "pawnsNear000 (N=={33}): {34}."
                        , _totalN // 0
                        , _offN // 1
                        , _suspendedN // 2
                        , _farN // 3
                        , FullMinMaxAvg.Count // , _fullTrueN    // 4
                        , fullAvg // , _closestPlayerDistforFull_Summ / _fullTrueN   // 5
                        , fullMin // , _closestPlayerDistforFull_Min // 6
                        , fullMax // , _closestPlayerDistforFull_Max // 7
                        , _fullWithEnormousDistanceN // 8
                        , OffLocoCoroutineUpsMinMaxAvg.Avg, OffLocoCoroutineUpsMinMaxAvg.Min, OffLocoCoroutineUpsMinMaxAvg.Max //  9 / 10 / 11
                        , SuspLocoCoroutineUpsMinMaxAvg.Avg, SuspLocoCoroutineUpsMinMaxAvg.Min, SuspLocoCoroutineUpsMinMaxAvg.Max // 12 / 13 / 14
                        , FarLocoCoroutineUpsMinMaxAvg.Avg, FarLocoCoroutineUpsMinMaxAvg.Min, FarLocoCoroutineUpsMinMaxAvg.Max // 15 / 16 / 17
                        , FullLocoCoroutineUpsMinMaxAvg.Avg, FullLocoCoroutineUpsMinMaxAvg.Min, FullLocoCoroutineUpsMinMaxAvg.Max // 18 / 19 / 20
                        , OffMoveActionsCoroutineUpsMinMaxAvg.Avg, OffMoveActionsCoroutineUpsMinMaxAvg.Min,
                        OffMoveActionsCoroutineUpsMinMaxAvg.Max // 21 / 22 / 23
                        , SuspMoveActionsCoroutineUpsMinMaxAvg.Avg, SuspMoveActionsCoroutineUpsMinMaxAvg.Min,
                        SuspMoveActionsCoroutineUpsMinMaxAvg.Max // 24 / 25 / 26
                        , FarMoveActionsCoroutineUpsMinMaxAvg.Avg, FarMoveActionsCoroutineUpsMinMaxAvg.Min,
                        FarMoveActionsCoroutineUpsMinMaxAvg.Max // 27 / 28 / 29
                        , FullMoveActionsCoroutineUpsMinMaxAvg.Avg, FullMoveActionsCoroutineUpsMinMaxAvg.Min,
                        FullMoveActionsCoroutineUpsMinMaxAvg.Max // 30 / 31 / 32
                        , _pawnsNear000.Count, string.Join(";  ", _pawnsNear000) // 33 / 34
                    );

                    LocomotionProfiler2.EndSample(); // "PawnWatchDog.UpdateCycle"

                    await Task.Delay(UpdateTimeStep);
                }

                // ReSharper disable once FunctionNeverReturns
            }

            // UPS - Updates per second
            private static void InspectCoroutinesUps(Pawn pawn)
            {
                InspectLocoCoroutineUps(pawn);
                InspectMoveActionsCoroutinesUps(pawn);
            }

            private static void InspectLocoCoroutineUps(Pawn pawn)
            {
                if (pawn._locoCoroutineCycleTimes.Count > 0)
                {
                    var secondsSinceFirstStamp = Time.realtimeSinceStartup - pawn._locoCoroutineCycleTimes[0];
                    pawn.CurrLocoCoroutineCyclesPerSec =
                        pawn._locoCoroutineCycleTimes.Count / secondsSinceFirstStamp; ///#PZ-7910:(8325): Is such watchDog impl-ion ok?
                }
                else
                    pawn.CurrLocoCoroutineCyclesPerSec = 0f;

                pawn._locoCoroutineCycleTimes.Clear();
            }

            private static void InspectMoveActionsCoroutinesUps(Pawn pawn)
            {
                if (pawn._moveActionsCoroutineCycleTimes.Count > 0)
                {
                    var secondsSinceFirstStamp = Time.realtimeSinceStartup - pawn._moveActionsCoroutineCycleTimes[0];
                    pawn.CurrMoveActionsCoroutineCyclesPerSec = pawn._moveActionsCoroutineCycleTimes.Count / secondsSinceFirstStamp;
                }
                else
                    pawn.CurrMoveActionsCoroutineCyclesPerSec = 0f;

                pawn._moveActionsCoroutineCycleTimes.Clear();
            }
        }
    }
}