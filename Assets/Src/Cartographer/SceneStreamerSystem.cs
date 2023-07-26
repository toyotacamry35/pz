using UnityEngine;
using System;
using SharedCode.Aspects.Cartographer;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeneratedCode.Manual.Repositories;
using UnityUpdate;
using Assets.TerrainBaker;
using Core.Environment.Logging.Extension;

namespace Assets.Src.Cartographer
{
    public class SceneStreamerSystem : ISceneStreamerInterface
    {
        private static float loadSceneRangeCheat { get; set; } = 0.0f;
        private static bool loadAllCheat { get; set; } = false;
        private static bool checkPositionCheat { get; set; } = false;
        private static bool debugModeCheat { get; set; } = false; // do not forget to create debugTimer if you want to turn on cheat from code
        private static bool debugModeVerboseCheat { get; set; } = false;
        private static Stopwatch debugTimer = null;
        private static bool showBackgroundTerrainCheat { get; set; } = true;
        private static bool showBackgroundObjectsCheat { get; set; } = true;
        private static bool showTerrainCheat { get; set; } = true;
        private static bool showFoliageCheat { get; set; } = true;
        private static bool showObjectsCheat { get; set; } = true;
        private static bool showEffectsCheat { get; set; } = true;
        
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Bounds WorldBounds => worldBounds;
        
        public static void SetDebugReportCheat()
        {
            //private BackgroundCell[] backgroundCells = null;
            //private RectInt backgroundCellsBounds = new RectInt(0, 0, 0, 0);
            //private bool backgroundInitialized = false;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, begin debug report").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                Logger.IfInfo()?.Message($"Initialized: {streamer.Initialized}, Mode:  {streamer.Mode}, SceneCollection: {(streamer.SceneCollection != null ? "OK" : "NULL")}, SceneStreamer: {(streamer.SceneStreamer != null ? "OK" : "NULL")}, check range: {(streamer.SceneStreamer != null ? streamer.SceneStreamer.CheckRange.ToString() : "unknown")}").Write();
                Logger.IfInfo()?.Message($"Iteration: {streamer.checkIteration}").Write();
                Logger.IfInfo()?.Message($"Importance radius: {Mathf.Sqrt(streamer.importanceRadius2)}, load radius: {Mathf.Sqrt(streamer.loadRadius2)}, unload radius: {Mathf.Sqrt(streamer.unloadRadius2)}, effective indices: {streamer.effectiveIndices}").Write();
                Logger.IfInfo()?.Message($"Scene cells size: {streamer.sceneCellsSize}").Write();
                Logger.IfInfo()?.Message($"Scene cells start: {streamer.sceneCellsStart}").Write();
                Logger.IfInfo()?.Message($"Scene cells bounds: {streamer.sceneCellsBounds}").Write();
                Logger.IfInfo()?.Message($"LoadScene frames interval: {streamer.loadSceneFramesInterval}, unloadScene frames interval: {streamer.unloadSceneFramesInterval}").Write();
                Logger.IfInfo()?.Message($"StatusChanged callback: {(streamer.StatusChanged != null ? "OK" : "NULL")}").Write();
                var buffer = new StringBuilder();
                if (streamer.players != null)
                {
                    var playerIndex = 0;
                    Logger.IfInfo()?.Message($"Players: {streamer.players.Count}").Write();
                    foreach (var player in streamer.players)
                    {
                        Logger.IfInfo()?.Message($"Player, index: {playerIndex}, key: {player.Key}, id: {player.Value.Id}, to remove: {player.Value.ToRemove}, pos: {player.Value.Position}, rect: {player.Value.PreviousEffectiveRect}, important: {player.Value.LoadImportantScenes.Count}, load: {player.Value.LoadScenes.Count}").Write();
                        MakeSceneNamesReport(buffer, player.Value.LoadImportantScenes, streamer.sceneCells);
                        Logger.IfInfo()?.Message($"Player, index: {playerIndex}, important load: {buffer.ToString()}").Write();
                        MakeSceneNamesReport(buffer, player.Value.LoadScenes, streamer.sceneCells);
                        Logger.IfInfo()?.Message($"Player, index: {playerIndex}, load: {buffer.ToString()}").Write();
                        ++playerIndex;
                    }
                }
                if (streamer.sceneCells != null)
                {
                    Logger.IfInfo()?.Message($"Scenes: {streamer.sceneCells.Length}").Write();
                    var states = new int[(int)(SceneCellState.Count)];
                    foreach (var sceneCell in streamer.sceneCells)
                    {
                        states[(int)sceneCell.State] += 1;
                    }
                    for (var stateIndex = 0; stateIndex < states.Length; ++stateIndex)
                    {
                        Logger.IfInfo()?.Message($"{(SceneCellState)(stateIndex)}: {states[stateIndex]}").Write();
                    }
                    for (var state = SceneCellState.Unloaded; state < SceneCellState.Count; ++state)
                    {
                        foreach (var sceneCell in streamer.sceneCells)
                        {
                            if (sceneCell.State == state)
                            {
                                Logger.IfInfo()?.Message($"Scene, state: {sceneCell.State}, name: {sceneCell.SceneName}, index: {sceneCell.Index}, center: {sceneCell.Center}, deinitialized: {sceneCell.Deinitialized}, player loads: {sceneCell.playerLoads.Count}, player locks: {sceneCell.playerLocks.Count}, iteration: {sceneCell.CheckIteration}").Write();
                                MakePlayersReport(buffer, sceneCell.playerLoads);
                                Logger.IfInfo()?.Message($"Scene, state: {sceneCell.State}, name: {sceneCell.SceneName}, index: {sceneCell.Index}, player loads: {buffer.ToString()}").Write();
                                MakePlayersReport(buffer, sceneCell.playerLocks);
                                Logger.IfInfo()?.Message($"Scene, state: {sceneCell.State}, name: {sceneCell.SceneName}, index: {sceneCell.Index}, player locks: {buffer.ToString()}").Write();
                            }
                        }
                    }
                }
                if (streamer.scenesQueue != null)
                {
                    Logger.IfInfo()?.Message($"Queue, important to load: {streamer.scenesQueue.ImportantToLoadCount}, to load: {streamer.scenesQueue.ToLoadCount}, to unload: {streamer.scenesQueue.ToUnloadCount}").Write();
                    MakeSceneNamesReport(buffer, streamer.scenesQueue.importantToLoad, streamer.sceneCells);
                    Logger.IfInfo()?.Message($"Queue, important to load: {buffer.ToString()}").Write();
                    MakeSceneNamesReport(buffer, streamer.scenesQueue.toLoad, streamer.sceneCells);
                    Logger.IfInfo()?.Message($"Queue, to load: {buffer.ToString()}").Write();
                    MakeSceneNamesReport(buffer, streamer.scenesQueue.toUnload, streamer.sceneCells);
                    Logger.IfInfo()?.Message($"Queue, to unload: {buffer.ToString()}").Write();
                    Logger.IfInfo()?.Message($"Operations: {streamer.operations.Count}").Write();
                }
                if (streamer.operations != null)
                {
                    var operationIndex = 0;
                    foreach (var operation in streamer.operations)
                    {
                        Logger.IfInfo()?.Message($"Operation, index: {operationIndex}, is done: {operation.Key.isDone}, progress: {operation.Key.progress}, scene: {operation.Value.SceneName}, state: {operation.Value.State}").Write();
                        ++operationIndex;
                    }
                }
                if (streamer.awaitReady != null)
                {
                    var awaitReadyIndex = 0;
                    foreach (var awaitReady in streamer.awaitReady)
                    {
                        Logger.IfInfo()?.Message($"Scene waiting ro ready, index: {awaitReadyIndex}, scene key: {awaitReady.Key}, scene: {awaitReady.Value.SceneName}, state: {awaitReady.Value.State}").Write();
                        ++awaitReadyIndex;
                    }
                }
                if (streamer.alreadyReady != null)
                {
                    var alreadyReadyIndex = 0;
                    foreach (var alreadyReady in streamer.alreadyReady)
                    {
                        Logger.IfInfo()?.Message($"Scene already ready, index: {alreadyReadyIndex}, scene key: {alreadyReady}").Write();
                        ++alreadyReadyIndex;
                    }
                }
            }
            else
            {
                Logger.IfInfo()?.Message($"Can't find streamer!").Write();
            }
            Logger.IfInfo()?.Message($"SceneStreamerSystem, end debug report").Write();
        }

        private class SceneNameComparerClass : IComparer<string>
        {
            public int Compare(string left, string right)
            {
                Vector3Int _leftPoint;
                Vector3Int _rightPoint;
                var _left = IsSceneForStreaming(left, out _leftPoint);
                var _right = IsSceneForStreaming(right, out _rightPoint);
                if (_left && _right)
                {
                    if (_leftPoint.z == _rightPoint.z)
                    {
                        return _leftPoint.x.CompareTo(_rightPoint.x);
                    }
                    else
                    {
                        return _leftPoint.z.CompareTo(_rightPoint.z);
                    }
                }
                else if (!_right && !_left)
                {
                    return left.CompareTo(right);
                }
                else
                {
                    return _left.CompareTo(_right);
                }
            }
        }

        private static SceneNameComparerClass sceneNameComparerClass = new SceneNameComparerClass();

        private static void MakeSceneNamesReport(StringBuilder buffer, HashSet<int> indices, SceneCell[] scenes)
        {
            buffer.Clear();
            var sceneNames = new List<string>();
            foreach (var index in indices)
            {
                sceneNames.Add(scenes[index].SceneName);
            }
            sceneNames.Sort(sceneNameComparerClass);
            foreach (var sceneName in sceneNames)
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(", ");
                }
                buffer.Append(sceneName);
            }
        }

        private static void MakePlayersReport(StringBuilder buffer, HashSet<Guid> players)
        {
            buffer.Clear();
            foreach (var player in players)
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(", ");
                }
                buffer.Append(player.ToString());
            }
        }

        public static void SetLoadSceneRangeCheat(float loadSceneRange)
        {
            loadSceneRangeCheat = loadSceneRange;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, LoadSceneRangeCheat: {((loadSceneRangeCheat > 0.0f) ? "true" : "false")}, range: {loadSceneRangeCheat}").Write();

            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.UpdateRadiuses();
                streamer.CheckCellsForAllPlayers();
            }
        }

        public static void SetLoadAllCheat(bool enable)
        {
            bool previousValue = loadAllCheat;
            loadAllCheat = enable;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, LoadAllCheat: {loadAllCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckChellsForLoadAllCheat();
            }
        }

        public static void SetCheckPositionCheat(bool enable)
        {
            checkPositionCheat = enable;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, CheckPositionCheat: {checkPositionCheat}").Write();
        }

        public static void SetDebugModeCheat(bool enable)
        {
            debugModeCheat = enable;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, DebugModeCheat: {debugModeCheat}").Write();

            if (debugModeCheat)
            {
                debugTimer = new Stopwatch();
            }
            else
            {
                debugTimer = null;
            }
        }

        public static void SetDebugModeVerboseCheat(bool enable)
        {
            debugModeVerboseCheat = enable;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, DebugModeVerboseCheat: {debugModeVerboseCheat}").Write();
        }

        public static void SetShowBackgroundTerrainCheat(bool show)
        {
            showBackgroundTerrainCheat = show;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, ShowBackgroundTerrainCheat: {showBackgroundTerrainCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckBackground();
            }
        }

        public static void SetShowBackgroundObjectsCheat(bool show)
        {
            showBackgroundObjectsCheat = show;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, ShowBackgroundObjectsCheat: {showBackgroundObjectsCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckBackground();
            }
        }

        public static void SetShowTerrainCheat(bool show)
        {
            showTerrainCheat = show;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, SetShowTerrainCheat: {showTerrainCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckScenes();
            }
        }

        public static void SetShowFoliageCheat(bool show)
        {
            showFoliageCheat = show;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, SetShowFoliageCheat: {showFoliageCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckScenes();
            }
        }

        public static void SetShowObjectsCheat(bool show)
        {
            showObjectsCheat = show;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, SetShowObjectsCheat: {showObjectsCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckScenes();
            }
        }

        public static void SetShowEffectsCheat(bool show)
        {
            showEffectsCheat = show;
            Logger.IfInfo()?.Message($"SceneStreamerSystem, SetShowEffectsCheat: {showEffectsCheat}").Write();
            var streamer = Streamer as SceneStreamerSystem;
            if (streamer != null)
            {
                streamer.CheckScenes();
            }
        }

        public static void SwitchShowBackgroundTerrainCheat()
        {
            showBackgroundTerrainCheat = !showBackgroundTerrainCheat;
            SetShowBackgroundTerrainCheat(showBackgroundTerrainCheat);
        }

        public static void SwitchShowBackgroundObjectsCheat()
        {
            showBackgroundObjectsCheat = !showBackgroundObjectsCheat;
            SetShowBackgroundObjectsCheat(showBackgroundObjectsCheat);
        }

        public static void SwitchShowTerrainCheat()
        {
            showTerrainCheat = !showTerrainCheat;
            SetShowTerrainCheat(showTerrainCheat);
        }

        public static void SwitchShowFoliageCheat()
        {
            showFoliageCheat = !showFoliageCheat;
            SetShowFoliageCheat(showFoliageCheat);
        }

        public static void SwitchShowObjectsCheat()
        {
            showObjectsCheat = !showObjectsCheat;
            SetShowObjectsCheat(showObjectsCheat);
        }

        public static void SwitchShowEffectsCheat()
        {
            showEffectsCheat = !showEffectsCheat;
            SetShowEffectsCheat(showEffectsCheat);
        }

        public static void ResetDebugTimer()
        {
            if (debugModeCheat)
            {
                debugTimer?.Restart();
            }
        }

        public static void DebugError(string line)
        {
            Logger.IfError()?.Message($"[SceneStreamerSystemDebugReport]\tERROR\t{line}").Write();
        }

        public static Action<bool, string> DebugReport(bool essential) => debugModeCheat && (essential || debugModeVerboseCheat) ? _debugReportInternal : null;
        
        public static void DebugReportInternal(bool time, string line)
        {
            if (time)
            {
                var milliseconds = (debugTimer?.ElapsedTicks ?? 0) * 1000.0 / Stopwatch.Frequency;
                Logger.IfError()?.Message($"[SceneStreamerSystemDebugReport]\tDEBUG\t{milliseconds} ms\t{line}").Write();
            }
            else
            {
                Logger.IfError()?.Message($"[SceneStreamerSystemDebugReport]\tDEBUG\t{line}").Write();
            }
        }

        private static readonly char[] Delimiters = new char[] { '_', '.' };
        private static readonly float rangeThreshold2 = 16.0f;

        private enum SceneCellState
        {
            Unloaded = 0,
            ToLoad = 1,
            WaitLoading = 2,
            Loaded = 3,
            ToUnload = 4,
            WaitUnloading = 5,
            Count = 6
        }

        private class Player
        {
            public Guid Id = Guid.Empty;
            public bool RecentlyAdded = true;
            public bool ToRemove = false;
            public Vector3 Position = Vector3.zero;
            public Vector2 Position2 { get { return new Vector2(Position.x, Position.z); } }
            public RectInt PreviousEffectiveRect = new RectInt(0, 0, 0, 0);
            public HashSet<int> LoadImportantScenes = new HashSet<int>();
            public HashSet<int> LoadScenes = new HashSet<int>();

            public SceneStreamerStatus GetStatus(SceneStreamerSystem streamer)
            {
                if (RecentlyAdded || (streamer == null) || !streamer.Initialized)
                {
                    return SceneStreamerStatus.Unknown;
                }
                else if (LoadImportantScenes.Count > 0)
                {
                    return SceneStreamerStatus.NotReady;
                }
                else if (LoadScenes.Count > 0)
                {
                    return SceneStreamerStatus.ImportantReady;
                }
                else
                {
                    return SceneStreamerStatus.AllReady;
                }
            }

            public bool ReadyToRemove()
            {
                return ToRemove && (LoadImportantScenes.Count == 0) && (LoadScenes.Count == 0);
            }

            public override string ToString() => $"Id:{Id}, Position: {Position}, GetStatus:{GetStatus((SceneStreamerSystem)SceneStreamerSystem.Streamer)}";
        }

        private class SceneCell
        {
            public uint CheckIteration = uint.MinValue;
            public int Index = 0;

            public SceneCellState State = SceneCellState.Unloaded;
            public Vector2 Center = Vector2.zero;
            public string SceneName = string.Empty;

            public bool Deinitialized = false;

            public HashSet<Guid> playerLoads = new HashSet<Guid>();
            public HashSet<Guid> playerLocks = new HashSet<Guid>();

            private bool CheckNoPlayerLocks()
            {
                return (playerLocks.Count == 0);
            }

            private void AddToPlayerLocks(Player player)
            {
                playerLocks.Add(player.Id);
                DebugReport(false)?.Invoke(false, $"AddToPlayerLocks(), scene: {SceneName}, state: {State}, player: {player.Id}, count: {playerLocks.Count}");
            }

            private bool RemoveFromPlayerLocks(Player player)
            {
                playerLocks.Remove(player.Id);
                DebugReport(false)?.Invoke(false, $"RemoveFromPlayerLocks(), scene: {SceneName}, state: {State}, player: {player.Id}, count: {playerLocks.Count}");
                return CheckNoPlayerLocks();
            }

            private void AddToPlayerLoad(SceneStreamerSystem streamer, Player player, bool important)
            {
                playerLoads.Add(player.Id);
                DebugReport(false)?.Invoke(false, $"AddToPlayerLoad(), scene: {SceneName}, state: {State}, playerLoads: {playerLoads.Count}");
                var oldStatus = player.GetStatus(streamer);
                if (important)
                {
                    player.LoadImportantScenes.Add(Index);
                }
                else
                {
                    player.LoadScenes.Add(Index);
                }
                var newStatus = player.GetStatus(streamer);
                DebugReport(false)?.Invoke(false, $"AddToPlayerLoad(), scene: {SceneName}, state: {State}, player: {player.Id}, important: {player.LoadImportantScenes.Count}, other: {player.LoadScenes.Count}, old: {oldStatus}, new: {newStatus}");
                streamer.CallStatusChanged(player, oldStatus, newStatus);
            }

            private void RemoveFromPlayerLoad(SceneStreamerSystem streamer, Player player)
            {
                playerLoads.Remove(player.Id);
                DebugReport(false)?.Invoke(false, $"RemoveFromPlayerLoad(), scene: {SceneName}, state: {State}, playerLoads: {playerLoads.Count}");
                var oldStatus = player.GetStatus(streamer);
                player.LoadImportantScenes.Remove(Index);
                player.LoadScenes.Remove(Index);
                var newStatus = player.GetStatus(streamer);
                DebugReport(false)?.Invoke(false, $"RemoveFromPlayerLoad(), scene: {SceneName}, state: {State}, player: {player.Id}, important: {player.LoadImportantScenes.Count}, other: {player.LoadScenes.Count}, old: {oldStatus}, new: {newStatus}");
                streamer.CallStatusChanged(player, oldStatus, newStatus);
            }

            public void RemoveFromPlayerLoads(SceneStreamerSystem streamer)
            {
                DebugReport(false)?.Invoke(false, $"RemoveFromPlayerLoads(), scene: {SceneName}, state: {State}, playerLoads: 0");
                foreach (var id in playerLoads)
                {
                    Player player = null;
                    if (streamer.players.TryGetValue(id, out player))
                    {
                        var oldStatus = player.GetStatus(streamer);
                        player.LoadImportantScenes.Remove(Index);
                        player.LoadScenes.Remove(Index);
                        var newStatus = player.GetStatus(streamer);
                        DebugReport(false)?.Invoke(false, $"RemoveFromPlayerLoads(), scene: {SceneName}, state: {State}, player: {player.Id}, important: {player.LoadImportantScenes.Count}, other: {player.LoadScenes.Count}, old: {oldStatus}, new: {newStatus}");
                        streamer.CallStatusChanged(player, oldStatus, newStatus);
                    }
                }
                playerLoads.Clear();
            }

            public void CheckForLoadAllCheat(SceneStreamerSystem streamer)
            {
                if (!loadAllCheat)
                {
                    if (CheckNoPlayerLocks())
                    {
                        if (State == SceneCellState.Loaded)
                        {
                            State = SceneCellState.ToUnload;
                            streamer.scenesQueue.AddToUnload(this);
                            DebugReport(true)?.Invoke(false, $"ADD TO UNLOAD SceneCell.CheckForLoadAllCheat(), scene: {SceneName}, state: {State}, center {Center}, iteration: {CheckIteration}, queue to unload: {streamer.scenesQueue.ToUnloadCount}");
                        }
                        else if (State == SceneCellState.ToLoad)
                        {
                            RemoveFromPlayerLoads(streamer);
                            State = SceneCellState.Unloaded;
                            streamer.scenesQueue.RemoveFromLoad(this);
                        }
                    }
                }
                else
                {
                    if (State == SceneCellState.Unloaded)
                    {
                        State = SceneCellState.ToLoad;
                        var important = false;
                        streamer.scenesQueue.AddToLoad(this, important);
                        DebugReport(true)?.Invoke(false, $"ADD TO LOAD SceneCell.CheckForLoadAllCheat(), scene: {SceneName}, state: {State}, center {Center}, iteration: {CheckIteration}, important: {important}, queue to load: {streamer.scenesQueue.ImportantToLoadCount}, {streamer.scenesQueue.ToLoadCount}");
                    }
                    else if (State == SceneCellState.ToUnload)
                    {
                        State = SceneCellState.Loaded;
                        streamer.scenesQueue.RemoveFromUnload(this);
                    }
                }
            }

            public void CheckForPlayer(SceneStreamerSystem streamer, Player player)
            {
                if (CheckIteration != streamer.checkIteration)
                {
                    if (player != null)
                    {
                        CheckIteration = streamer.checkIteration;
                        var distance2 = (Center - player.Position2).sqrMagnitude;
                        if (player.ToRemove || (distance2 > streamer.unloadRadius2))
                        {
                            if (RemoveFromPlayerLocks(player) && !loadAllCheat)
                            {
                                if (State == SceneCellState.Loaded)
                                {
                                    State = SceneCellState.ToUnload;
                                    streamer.scenesQueue.AddToUnload(this);
                                    DebugReport(true)?.Invoke(false, $"ADD TO UNLOAD SceneCell.CheckForPlayer(), scene: {SceneName}, state: {State}, player: {player.Id}, center {Center}, iteration: {CheckIteration}, queue to unload: {streamer.scenesQueue.ToUnloadCount}");
                                }
                                else if (State == SceneCellState.ToLoad)
                                {
                                    RemoveFromPlayerLoads(streamer);
                                    State = SceneCellState.Unloaded;
                                    streamer.scenesQueue.RemoveFromLoad(this);
                                }
                            }
                        }
                        else if (distance2 <= streamer.loadRadius2)
                        {
                            AddToPlayerLocks(player);
                            if ((State == SceneCellState.ToLoad) || (State == SceneCellState.WaitLoading))
                            {
                                if (distance2 <= streamer.importanceRadius2)
                                {
                                    AddToPlayerLoad(streamer, player, true);
                                    if (State == SceneCellState.ToLoad)
                                    {
                                        streamer.scenesQueue.MoveToImportant(this);
                                    }
                                }
                                else
                                {
                                    AddToPlayerLoad(streamer, player, false);
                                }
                            }
                            else if (State == SceneCellState.Unloaded)
                            {
                                State = SceneCellState.ToLoad;
                                var important = (distance2 <= streamer.importanceRadius2);
                                AddToPlayerLoad(streamer, player, important);
                                streamer.scenesQueue.AddToLoad(this, important);
                                DebugReport(true)?.Invoke(false, $"ADD TO LOAD SceneCell.CheckForPlayer(), scene: {SceneName}, state: {State}, player: {player.Id}, center {Center}, iteration: {CheckIteration}, important: {important}, queue to load: {streamer.scenesQueue.ImportantToLoadCount}, {streamer.scenesQueue.ToLoadCount}");
                            }
                            else if (State == SceneCellState.ToUnload)
                            {
                                State = SceneCellState.Loaded;
                                streamer.scenesQueue.RemoveFromUnload(this);
                            }
                        }
                    }
                }
            }
        }

        private class ScenesQueue
        {
            public HashSet<int> importantToLoad = new HashSet<int>();
            public HashSet<int> toLoad = new HashSet<int>();
            public HashSet<int> toUnload = new HashSet<int>();

            public int ImportantToLoadCount { get { return importantToLoad.Count; } }
            public int ToLoadCount { get { return toLoad.Count; } }
            public int ToUnloadCount { get { return toUnload.Count; } }

            public void MoveToImportant(SceneCell sceneCell)
            {
                toLoad.Remove(sceneCell.Index);
                importantToLoad.Add(sceneCell.Index);
            }

            public void AddToLoad(SceneCell sceneCell, bool important)
            {
                if (important)
                {
                    importantToLoad.Add(sceneCell.Index);
                }
                else
                {
                    toLoad.Add(sceneCell.Index);
                }
            }

            public void AddToUnload(SceneCell sceneCell)
            {
                toUnload.Add(sceneCell.Index);
            }

            public void RemoveFromLoad(SceneCell sceneCell)
            {
                importantToLoad.Remove(sceneCell.Index);
                toLoad.Remove(sceneCell.Index);
            }

            public void RemoveFromUnload(SceneCell sceneCell)
            {
                toUnload.Remove(sceneCell.Index);
            }

            public void Clear()
            {
                importantToLoad.Clear();
                toLoad.Clear();
                toUnload.Clear();
            }

            public bool CanLoad()
            {
                return ((importantToLoad.Count > 0) || (toLoad.Count > 0));
            }

            public bool CanLoad(bool important)
            {
                return ((important ? importantToLoad.Count : toLoad.Count) > 0);
            }

            public bool CanUnload()
            {
                return (toUnload.Count > 0);
            }

            public SceneCell TryLoad(SceneCell[] sceneCells)
            {
                if (importantToLoad.Count > 0)
                {
                    var first = importantToLoad.First();
                    importantToLoad.Remove(first);
                    return sceneCells[first];
                }
                else if (toLoad.Count > 0)
                {
                    var first = toLoad.First();
                    toLoad.Remove(first);
                    return sceneCells[first];
                }
                else
                {
                    return null;
                }
            }

            public SceneCell TryLoad(bool important, SceneCell[] sceneCells)
            {
                if (important)
                {
                    if (importantToLoad.Count > 0)
                    {
                        int first = importantToLoad.First();
                        importantToLoad.Remove(first);
                        return sceneCells[first];
                    }
                    return null;
                }
                else
                {
                    if (toLoad.Count > 0)
                    {
                        int first = toLoad.First();
                        toLoad.Remove(first);
                        return sceneCells[first];
                    }
                    return null;
                }
            }

            public SceneCell TryUnload(SceneCell[] sceneCells)
            {
                if (toUnload.Count > 0)
                {
                    int first = toUnload.First();
                    toUnload.Remove(first);
                    return sceneCells[first];
                }
                return null;
            }
        }

        private class BackgroundCell
        {
            public TerrainHolderBehaviour TerrainHolder = null;
            public GameObject Terrain = null;
            public GameObject StaticObjects = null;
            public bool Visible = true;
        }

        private Dictionary<Guid, Player> players = new Dictionary<Guid, Player>();

        private uint checkIteration = uint.MinValue;

        private float importanceRadius2 = 0.0f;
        private float loadRadius2 = 0.0f;
        private float unloadRadius2 = 0.0f;
        private Vector2Int effectiveIndices = Vector2Int.zero;

        private Vector2 sceneCellsSize = new Vector2(0.0f, 0.0f);
        private Vector2Int sceneCellsStart = new Vector2Int(0, 0);
        private RectInt sceneCellsBounds = new RectInt(0, 0, 0, 0);
        private SceneCell[] sceneCells = null;

        private BackgroundCell[] backgroundCells = null;
        private RectInt backgroundCellsBounds = new RectInt(0, 0, 0, 0);
        private bool backgroundInitialized = false;

        private ScenesQueue scenesQueue = new ScenesQueue();

        private Dictionary<AsyncOperation, SceneCell> operations = new Dictionary<AsyncOperation, SceneCell>();
        private Dictionary<string, SceneCell> awaitReady = new Dictionary<string, SceneCell>();
        private HashSet<string> alreadyReady = new HashSet<string>();


        private int loadSceneFramesInterval = 0;
        private int unloadSceneFramesInterval = 0;

        private Bounds worldBounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
        private static readonly Action<bool, string> _debugReportInternal = DebugReportInternal;

        public event Action<Guid, SceneStreamerStatus, SceneStreamerStatus> StatusChanged;

        // - Private Helpers ----------------------------------------------------------------------
        private static GameObject FindChild(GameObject gameObject, string name)
        {
            if ((gameObject != null) && !string.IsNullOrEmpty(name))
            {
                var childCount = gameObject.transform.childCount;
                if (childCount > 0)
                {
                    for (var childIndex = 0; childIndex < childCount; ++childIndex)
                    {
                        var child = gameObject.transform.GetChild(childIndex).gameObject;
                        if (child.name.Equals(name))
                        {
                            return child;
                        }
                    }
                }
            }
            return null;
        }

        private static bool IsSceneForStreaming(string sceneName, out Vector3Int coordinates)
        {
            coordinates = new Vector3Int(0, 0, 0);
            var segments = sceneName.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 1)
            {
                var coordinatesAquired = 0;
                for (var index = 1; index < segments.Length; ++index)
                {
                    var coordinate = segments[index][0];
                    var value = segments[index].Substring(1);
                    if (coordinate == 'x' || coordinate == 'X')
                    {
                        int _value;
                        if (int.TryParse(value, out _value))
                        {
                            coordinates.x = _value;
                            ++coordinatesAquired;
                        }
                    }
                    else if (coordinate == 'y' || coordinate == 'Y')
                    {
                        int _value;
                        if (int.TryParse(value, out _value))
                        {
                            coordinates.y = _value;
                            ++coordinatesAquired;
                        }
                    }
                    else if (coordinate == 'z' || coordinate == 'Z')
                    {
                        int _value;
                        if (int.TryParse(value, out _value))
                        {
                            coordinates.z = _value;
                            ++coordinatesAquired;
                        }
                    }
                }
                if (coordinatesAquired > 1)
                {
                    return true;
                }
            }
            return false;
        }

        private static Vector2 GetSceneCenter(SceneCollectionDef sceneCollection, Vector3Int coordinates)
        {
            return new Vector2((coordinates.x + 0.5f) * sceneCollection.SceneSize.x, (coordinates.z + 0.5f) * sceneCollection.SceneSize.z);
        }

        private static int GetSceneIndex(int sizeX, int x, int z)
        {
            return (sizeX * z) + x;
        }

        private static Vector2Int GetSceneIndices(int sizeX, int index)
        {
            var z = index / sizeX;
            var x = index - (z * sizeX);
            return new Vector2Int(x, z);
        }

        private static int GetSceneIndex(SceneCollectionDef sceneCollection, Vector3Int coordinates, bool check)
        {
            if (check)
            {
                if ((coordinates.x < sceneCollection.SceneStart.x) ||
                     (coordinates.z < sceneCollection.SceneStart.z) ||
                     (coordinates.x >= (sceneCollection.SceneStart.x + sceneCollection.SceneCount.x)) ||
                     (coordinates.z >= (sceneCollection.SceneStart.z + sceneCollection.SceneCount.z)))
                {
                    return -1;
                }
            }
            return GetSceneIndex(sceneCollection.SceneCount.x, coordinates.x - sceneCollection.SceneStart.x, coordinates.z - sceneCollection.SceneStart.z);
        }

        private static Vector3Int GetSceneIndices(SceneCollectionDef sceneCollection, int index)
        {
            var indices = GetSceneIndices(sceneCollection.SceneCount.x, index);
            return new Vector3Int(indices.x + sceneCollection.SceneStart.x, 0, indices.y + sceneCollection.SceneStart.z);
        }

        // - Private Methods ----------------------------------------------------------------------
        private RectInt ClampToSceneCellBounds(RectInt effectiveRect)
        {
            var xMin = Mathf.Max(effectiveRect.xMin, sceneCellsBounds.xMin);
            var xMax = Mathf.Min(effectiveRect.xMax, sceneCellsBounds.xMax);
            var yMin = Mathf.Max(effectiveRect.yMin, sceneCellsBounds.yMin);
            var yMax = Mathf.Min(effectiveRect.yMax, sceneCellsBounds.yMax);
            xMin = Mathf.Min(xMin, sceneCellsBounds.xMax);
            xMax = Mathf.Max(xMax, sceneCellsBounds.xMin);
            yMin = Mathf.Min(yMin, sceneCellsBounds.yMax);
            yMax = Mathf.Max(yMax, sceneCellsBounds.yMin);
            return new RectInt(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        private void CheckBackground()
        {
            if (backgroundInitialized && (backgroundCells != null))
            {
                foreach (var backgroundCell in backgroundCells)
                {
                    if (backgroundCell != null)
                    {
                        if (backgroundCell.Terrain != null)
                        {
                            backgroundCell.Terrain.SetActive(backgroundCell.Visible && showBackgroundTerrainCheat);
                        }
                        if (backgroundCell.StaticObjects != null)
                        {
                            backgroundCell.StaticObjects.SetActive(backgroundCell.Visible && showBackgroundObjectsCheat);
                        }
                    }
                }
            }
        }

        private void CheckScenes()
        {
            if (sceneCells != null)
            {
                foreach(var sceneCell in sceneCells)
                {
                    if ((sceneCell != null) && (sceneCell.State == SceneCellState.Loaded))
                    {
                        var scene = SceneManager.GetSceneByName(sceneCell.SceneName);
                        if (scene.IsValid() && scene.isLoaded)
                        {
                            var rootGameObjects = scene.GetRootGameObjects();
                            if ((rootGameObjects != null) && (rootGameObjects.Length > 0))
                            {
                                foreach (var rootGameObject in rootGameObjects)
                                {
                                    var sceneLoaderBehaviour = rootGameObject.GetComponent<SceneLoaderBehaviour>();
                                    if (sceneLoaderBehaviour != null)
                                    {
                                        sceneLoaderBehaviour.CheckGameObjects(showTerrainCheat, showFoliageCheat, showObjectsCheat, showEffectsCheat);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateRadiuses()
        {
            if (SceneStreamer != null)
            {
                if (loadSceneRangeCheat > 0.0f)
                {
                    CalculateRadiuses(SceneStreamer.ImportanceRange, loadSceneRangeCheat, loadSceneRangeCheat + (SceneStreamer.UnloadSceneRange - SceneStreamer.LoadSceneRange));
                }
                else
                {
                    CalculateRadiuses(SceneStreamer.ImportanceRange, SceneStreamer.LoadSceneRange, SceneStreamer.UnloadSceneRange);
                }
            }
        }

        private void CalculateRadiuses(float importanceRange, float loadSceneRange, float unloadSceneRange)
        {
            loadRadius2 = Mathf.Max(((sceneCellsSize.x * sceneCellsSize.x + sceneCellsSize.y * sceneCellsSize.y) / 4.0f) + rangeThreshold2, loadSceneRange * loadSceneRange);
            importanceRadius2 = Mathf.Min(loadRadius2, importanceRange * importanceRange);
            unloadRadius2 = Mathf.Max(loadRadius2 + rangeThreshold2, unloadSceneRange * unloadSceneRange);
            var effectiveRange = Mathf.Sqrt(unloadRadius2);
            effectiveIndices = new Vector2Int(Mathf.CeilToInt(effectiveRange / sceneCellsSize.x), Mathf.CeilToInt(effectiveRange / sceneCellsSize.y));
        }

        private List<KeyValuePair<Guid, Vector3>> BackupPlayers()
        {
            var result = new List<KeyValuePair<Guid, Vector3>>();
            foreach (var player in players)
            {
                result.Add(new KeyValuePair<Guid, Vector3>(player.Value.Id, player.Value.Position));
            }
            return result;
        }

        private void RestorePlayers(List<KeyValuePair<Guid, Vector3>> backup)
        {
            foreach (var backupPlayer in backup)
            {
                SetPosition(backupPlayer.Key, backupPlayer.Value, true);
            }
        }

        private void InitializeBackground()
        {
            ResetDebugTimer();
            var scenesFound = 0;
            if (!backgroundInitialized && (SceneCollection != null) && !string.IsNullOrEmpty(SceneCollection.BackgroundSceneName))
            {
                var scene = SceneManager.GetSceneByName(SceneCollection.BackgroundSceneName);
                if (scene.IsValid() && scene.isLoaded)
                {
                    var rootGameObjects = new List<GameObject>(scene.rootCount);
                    scene.GetRootGameObjects(rootGameObjects);
                    var backgroundRootGameObjects = new List<KeyValuePair<GameObject, Vector3Int>>();
                    var min = new Vector3Int();
                    var max = new Vector3Int();
                    var somethingFound = false;
                    foreach (var rootGameObject in rootGameObjects)
                    {
                        Vector3Int coordinates;
                        if (IsSceneForStreaming(rootGameObject.name, out coordinates))
                        {
                            if (!somethingFound)
                            {
                                min = coordinates;
                                max = coordinates;
                                somethingFound = true;
                            }
                            else
                            {
                                min.x = Mathf.Min(min.x, coordinates.x);
                                min.y = Mathf.Min(min.y, coordinates.y);
                                min.z = Mathf.Min(min.z, coordinates.z);
                                max.x = Mathf.Max(max.x, coordinates.x);
                                max.y = Mathf.Max(max.y, coordinates.y);
                                max.z = Mathf.Max(max.z, coordinates.z);
                            }
                            backgroundRootGameObjects.Add(new KeyValuePair<GameObject, Vector3Int>(rootGameObject, coordinates));
                        }
                    }
                    if (somethingFound)
                    {
                        backgroundCellsBounds = new RectInt(min.x, min.z, max.x - min.x + 1, max.z - min.z + 1);
                        backgroundCells = new BackgroundCell[backgroundCellsBounds.width * backgroundCellsBounds.height];
                        foreach (var backgroundRootGameObject in backgroundRootGameObjects)
                        {
                            var backgroundCell = new BackgroundCell();
                            backgroundCell.Visible = true;
                            var terrainCollider = FindChild(backgroundRootGameObject.Key, SceneCollection.BackgroundTerrainColliderName);
                            if (terrainCollider != null)
                            {
                                backgroundCell.TerrainHolder = terrainCollider.GetComponent<TerrainHolderBehaviour>();
                            }
                            backgroundCell.Terrain = FindChild(backgroundRootGameObject.Key, SceneCollection.BackgroundTerrainName);
                            backgroundCell.StaticObjects = FindChild(backgroundRootGameObject.Key, SceneCollection.BackgroundStaticObjectsName);
                            if ((backgroundCell.Terrain != null) || (backgroundCell.StaticObjects != null))
                            {
                                ++scenesFound;
                                var index = GetSceneIndex(backgroundCellsBounds.width,
                                                          backgroundRootGameObject.Value.x - backgroundCellsBounds.min.x,
                                                          backgroundRootGameObject.Value.z - backgroundCellsBounds.min.y);
                                backgroundCells[index] = backgroundCell;
                            }
                        }
                    }
                    backgroundInitialized = true;
                }
            }
            DebugReport(false)?.Invoke(true, $"InitializeBackground(), result: {backgroundInitialized}, scenes found: {scenesFound}, bounds: {backgroundCellsBounds} ({backgroundCellsBounds.width * backgroundCellsBounds.height})");
        }

        private void DeinitializeBackground()
        {
            if (backgroundInitialized)
            {
                backgroundCells = null;
                backgroundCellsBounds = new RectInt(0, 0, 0, 0);
                backgroundInitialized = false;
            }
        }

        private void InitializeCells()
        {
            ResetDebugTimer();

            loadSceneFramesInterval = 0;
            unloadSceneFramesInterval = 0;

            checkIteration = uint.MinValue;

            sceneCellsSize = new Vector2(SceneCollection.SceneSize.x, SceneCollection.SceneSize.z);
            sceneCellsStart = new Vector2Int(SceneCollection.SceneStart.x, SceneCollection.SceneStart.z);
            sceneCellsBounds = new RectInt(0, 0, SceneCollection.SceneCount.x, SceneCollection.SceneCount.z);
            sceneCells = new SceneCell[sceneCellsBounds.width * sceneCellsBounds.height];

            UpdateRadiuses();

            foreach (var sceneName in SceneCollection.SceneNames)
            {
                Vector3Int coordinates;
                if (IsSceneForStreaming(sceneName, out coordinates))
                {
                    SceneCell sceneCell = new SceneCell();
                    sceneCell.Index = GetSceneIndex(SceneCollection, coordinates, false);
                    sceneCell.Center = GetSceneCenter(SceneCollection, coordinates);
                    sceneCell.SceneName = sceneName;
                    sceneCells[sceneCell.Index] = sceneCell;
                    sceneCell.CheckForLoadAllCheat(this);
                }
            }

            var someScenesAreMissing = false;
            for (var sceneIndex = 0; sceneIndex < sceneCells.Length; ++sceneIndex)
            {
                if(sceneCells[sceneIndex] == null)
                {
                    var indices = GetSceneIndices(SceneCollection, sceneIndex);
                    DebugError( $"InitializeCells(), scene {indices.x} {indices.z} is missing.");
                    someScenesAreMissing = true;
                }
            }
            if (someScenesAreMissing)
            {
                throw new ArgumentException("Some scenes are missing.", "SceneCollection.SceneNames");
            }

            var min = new Vector3(
                SceneCollection.SceneStart.x * SceneCollection.SceneSize.x,
                SceneCollection.SceneStart.y * SceneCollection.SceneSize.y,
                SceneCollection.SceneStart.z * SceneCollection.SceneSize.z);
            var max = new Vector3(
                min.x + SceneCollection.SceneCount.x * SceneCollection.SceneSize.x,
                min.y + SceneCollection.SceneCount.y * SceneCollection.SceneSize.y,
                min.z + SceneCollection.SceneCount.z * SceneCollection.SceneSize.z);
            worldBounds = new Bounds((min + max) * 0.5f, min - max);

            DebugReport(true)?.Invoke(true, $"InitializeCells() WorldBounds:[{worldBounds.min} - {worldBounds.max}]");
        }

        private void DeinitializeCells()
        {
            checkIteration = uint.MinValue;

            scenesQueue.Clear();

            foreach (var operation in operations)
            {
                operation.Value.Deinitialized = true;
            }

            if (sceneCells != null)
            {
                foreach (var sceneCell in sceneCells)
                {
                    if ((sceneCell.State == SceneCellState.Loaded) || (sceneCell.State == SceneCellState.ToUnload))
                    {
                        sceneCell.State = SceneCellState.WaitUnloading;
                        sceneCell.Deinitialized = true;
                        var operation = SceneManager.UnloadSceneAsync(sceneCell.SceneName);
                        if (operation != null)
                        {
                            operations.Add(operation, sceneCell);
                            operation.completed += OnOperationCompleted;
                            if (operation.isDone)
                            {
                                OnOperationCompleted(operation);
                            }
                            DebugReport(true)?.Invoke(false, $"CALL ASYNC UNLOAD DeinitializeCells(), sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}");
                        }
                        else
                        {
                            OperationCompleted(sceneCell, false);
                            DebugError($"CALL ASYNC UNLOAD ERROR DeinitializeCells(), UnloadSceneAsync() return null, sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}");
                        }
                    }
                }
            }
            sceneCells = null;
            sceneCellsBounds = new RectInt(0, 0, 0, 0);
        }

        private void InitializeInternal()
        {
            DeinitializeInternal();
            if (CanStream)
            {
                InitializeBackground();
                InitializeCells();

                UnityUpdateDelegate.OnUpdate += OnUpdate;
            }
        }

        private void DeinitializeInternal()
        {
            RemovePlayersReadyToRemove(true);
            if (CanStream)
            {
                DeinitializeCells();
                DeinitializeBackground();

                loadRadius2 = 0.0f;
                importanceRadius2 = 0.0f;
                unloadRadius2 = 0.0f;
                effectiveIndices = Vector2Int.zero;

                loadSceneFramesInterval = 0;
                unloadSceneFramesInterval = 0;

                UnityUpdateDelegate.OnUpdate -= OnUpdate;
                Initialized = false;
            }
        }

        private void AddCheckIteration()
        {
            if (checkIteration == uint.MaxValue)
            {
                checkIteration = uint.MinValue;
                if (sceneCells != null)
                {
                    for (var index = 0; index < sceneCells.Length; ++index)
                    {
                        var sceneCell = sceneCells[index];
                        if (sceneCell != null)
                        {
                            sceneCell.CheckIteration = checkIteration;
                        }
                    }
                }
            }
            checkIteration += 1U;
        }

        private void CheckChellsForLoadAllCheat()
        {
            if (CanStream)
            {
                var count = sceneCells.Length;
                for (var index = 0; index < count; ++index)
                {
                    sceneCells[index].CheckForLoadAllCheat(this);
                }
            }
        }

        private void CheckCellsForAllPlayers()
        {
            if (CanStream && (sceneCells != null))
            {
                foreach (var player in players)
                {
                    CheckCellsForPlayer(player.Value);
                }
            }
        }

        private void CheckCellsForPlayer(Player player)
        {
            AddCheckIteration();

            var currentSceneIndex = new Vector2Int(Mathf.FloorToInt(player.Position.x / sceneCellsSize.x) - sceneCellsStart.x, Mathf.FloorToInt(player.Position.z / sceneCellsSize.y) - sceneCellsStart.y);
            var effectiveRect = new RectInt(currentSceneIndex.x - effectiveIndices.x, currentSceneIndex.y - effectiveIndices.y, effectiveIndices.x * 2 + 1, effectiveIndices.y * 2 + 1);
            effectiveRect = ClampToSceneCellBounds(effectiveRect);

            DebugReport(false)?.Invoke(false, $"CheckCells(), start iteration: {checkIteration}, player: {player.Id}, remove: {player.ToRemove}, pos: {player.Position}, rect: {effectiveRect}, prev: {player.PreviousEffectiveRect}");

            var sizeX = sceneCellsBounds.width;
            for (var x = player.PreviousEffectiveRect.xMin; x < player.PreviousEffectiveRect.xMax; ++x)
            {
                for (var y = player.PreviousEffectiveRect.yMin; y < player.PreviousEffectiveRect.yMax; ++y)
                {
                    var sceneCell = sceneCells[GetSceneIndex(sizeX, x, y)];
                    if (sceneCell != null)
                    {
                        sceneCell.CheckForPlayer(this, player);
                    }
                }
            }

            if ((effectiveRect.xMin != player.PreviousEffectiveRect.xMin) ||
                (effectiveRect.xMax != player.PreviousEffectiveRect.xMax) ||
                (effectiveRect.yMin != player.PreviousEffectiveRect.yMin) ||
                (effectiveRect.yMax != player.PreviousEffectiveRect.yMax))
            {
                for (var x = effectiveRect.xMin; x < effectiveRect.xMax; ++x)
                {
                    for (var y = effectiveRect.yMin; y < effectiveRect.yMax; ++y)
                    {
                        var sceneCell = sceneCells[GetSceneIndex(sizeX, x, y)];
                        if (sceneCell != null)
                        {
                            sceneCell.CheckForPlayer(this, player);
                        }
                    }
                }
                player.PreviousEffectiveRect = effectiveRect;
            }

            DebugReport(false)?.Invoke(false, $"CheckCells(), end iteration: {checkIteration}");
        }

        private void RemovePlayersReadyToRemove(bool force)
        {
            var playersToRemove = players.Where(player => (force || player.Value.ReadyToRemove())).ToArray();
            foreach (var playerToRemove in playersToRemove)
            {
                DebugReport(true)?.Invoke(false, $"REMOVE PLAYER, player: {playerToRemove.Value.Id}");
                players.Remove(playerToRemove.Key);
                var oldStatus = playerToRemove.Value.GetStatus(this);
                var newStatus = SceneStreamerStatus.Unknown;
                CallStatusChanged(playerToRemove.Value, oldStatus, newStatus);
            }
        }

        private void OnUpdate()
        {
            loadSceneFramesInterval = Mathf.Max(0, loadSceneFramesInterval - 1);
            unloadSceneFramesInterval = Mathf.Max(0, unloadSceneFramesInterval - 1);

            if (scenesQueue.CanLoad() && ((loadSceneFramesInterval == 0) || (Mode == SceneStreamerMode.OptimiseLoadtime)))
            {
                ResetDebugTimer();
                loadSceneFramesInterval = Mathf.Max(0, SceneStreamer.LoadSceneFramesInterval);
                var sceneCell = scenesQueue.TryLoad(sceneCells);
                sceneCell.State = SceneCellState.WaitLoading;
                var operation = SceneManager.LoadSceneAsync(sceneCell.SceneName, LoadSceneMode.Additive);
                if (operation != null)
                {
                    operations.Add(operation, sceneCell);
                    operation.completed += OnOperationCompleted;
                    if (operation.isDone)
                    {
                        OnOperationCompleted(operation);
                    }
                    DebugReport(true)?.Invoke(true, $"CALL ASYNC LOAD OnUpdate(), sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}, queue to load: {scenesQueue.ImportantToLoadCount}, {scenesQueue.ToLoadCount}");
                }
                else
                {
                    OperationCompleted(sceneCell, false);
                    DebugError($"CALL ASYNC LOAD ERROR OnUpdate(), LoadSceneAsync() return null, sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}, queue to load: {scenesQueue.ImportantToLoadCount}, {scenesQueue.ToLoadCount}");
                }
            }

            if (scenesQueue.CanUnload() && ((unloadSceneFramesInterval == 0) || (Mode == SceneStreamerMode.OptimiseLoadtime)))
            {
                ResetDebugTimer();
                unloadSceneFramesInterval = Mathf.Max(0, SceneStreamer.UnloadSceneFramesInterval);
                var sceneCell = scenesQueue.TryUnload(sceneCells);
                sceneCell.State = SceneCellState.WaitUnloading;
                var operation = SceneManager.UnloadSceneAsync(sceneCell.SceneName);
                if (operation != null)
                {
                    operations.Add(operation, sceneCell);
                    operation.completed += OnOperationCompleted;
                    if (operation.isDone)
                    {
                        OnOperationCompleted(operation);
                    }
                    DebugReport(true)?.Invoke(true, $"CALL ASYNC UNLOAD OnUpdate(), sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}, queue to unload: {scenesQueue.ToUnloadCount}");
                }
                else
                {
                    OperationCompleted(sceneCell, false);
                    DebugError($"CALL ASYNC UNLOAD ERROR OnUpdate(), UnloadSceneAsync return null, sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}, queue to unload: {scenesQueue.ToUnloadCount}");
                }
            }
        }

        private void OperationCompleted(SceneCell sceneCell, bool good)
        {
            DebugReport(true)?.Invoke(false, $"OPERATION COMPLETED OperationCompleted(), good: {good}, sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}, operations left: {operations.Count}");
            if (sceneCell.State == SceneCellState.WaitLoading)
            {
                if (good)
                {
                    awaitReady.Add(sceneCell.SceneName, sceneCell);
                    if (alreadyReady.Contains(sceneCell.SceneName))
                    {
                        alreadyReady.Remove(sceneCell.SceneName);
                        OnSceneReady(sceneCell.SceneName);
                    }
                }
                else
                {
                    OnSceneReady(sceneCell.SceneName);
                }
            }
            else if (sceneCell.State == SceneCellState.WaitUnloading)
            {
                sceneCell.State = SceneCellState.Unloaded;
                if (!sceneCell.Deinitialized)
                {
                    foreach (var player in players)
                    {
                        sceneCell.CheckForPlayer(this, player.Value);
                    }
                }
                RemovePlayersReadyToRemove(false);
            }
        }

        private void OnOperationCompleted(AsyncOperation operation)
        {
            operation.completed -= OnOperationCompleted;
            SceneCell sceneCell = null;
            if (operations.TryGetValue(operation, out sceneCell))
            {
                OperationCompleted(sceneCell, true);
                operations.Remove(operation);
            }
        }

        private void OnSceneReady(string sceneName)
        {
            SceneCell sceneCell = null;
            if (awaitReady.TryGetValue(sceneName, out sceneCell))
            {
                DebugReport(true)?.Invoke(false, $"SCENE READY OnSceneReady(), sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}, operations left: {operations.Count}");
                sceneCell.State = SceneCellState.Loaded;
                sceneCell.RemoveFromPlayerLoads(this);
                if (!sceneCell.Deinitialized)
                {
                    foreach (var player in players)
                    {
                        sceneCell.CheckForPlayer(this, player.Value);
                    }
                }
                else
                {
                    sceneCell.State = SceneCellState.WaitUnloading;
                    sceneCell.Deinitialized = true;
                    var operation = SceneManager.UnloadSceneAsync(sceneCell.SceneName);
                    if (operation != null)
                    {
                        operations.Add(operation, sceneCell);
                        operation.completed += OnOperationCompleted;
                        if (operation.isDone)
                        {
                            OnOperationCompleted(operation);
                        }
                        DebugReport(true)?.Invoke(false, $"CALL ASYNC UNLOAD OnSceneReady(), sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}");
                    }
                    else
                    {
                        OperationCompleted(sceneCell, false);
                        DebugError($"CALL ASYNC UNLOAD ERROR OnSceneReady(), UnloadSceneAsync() return null, sceneCell: {sceneCell.SceneName}, state: {sceneCell.State}, center {sceneCell.Center}");
                    }
                }
                RemovePlayersReadyToRemove(false);
                awaitReady.Remove(sceneName);
            }
            else
            {
                alreadyReady.Add(sceneName);
            }
        }

        private void CallStatusChanged(Player player, SceneStreamerStatus oldStatus, SceneStreamerStatus newStatus)
        {
            //DebugReport(false, false, $"CALL STATUS CHANGED, player: {player.Id}, old: {oldStatus}, new: {newStatus}, StatusChanged: {((StatusChanged == null) ? "NULL" : "OK")}");

            if ((player != null) && (oldStatus != newStatus) && (StatusChanged != null))
            {
                DebugReport(true)?.Invoke(false, $"CALL STATUS CHANGED, player: {player.Id}, old: {oldStatus}, new: {newStatus}");
                StatusChanged(player.Id, oldStatus, newStatus);
            }
        }

        // - Static accessor to streamer ----------------------------------------------------------
        public static ISceneStreamerInterface Streamer { get { return GameState.Instance?.SceneStreamerSystem; } }

        // - Interface ----------------------------------------------------------------------------
        public SceneStreamerMode Mode { get; set; } = SceneStreamerMode.Default;

        public bool CanStream { get { return (SceneCollection != null) && (SceneStreamer != null); } }

        public bool Initialized { get; private set; } = false;

        public SceneCollectionDef SceneCollection { get; private set; } = null;

        public SceneStreamerDef SceneStreamer { get; private set; } = null;

        public bool Initialize([CanBeNull] SceneCollectionDef sceneCollection, [CanBeNull] SceneStreamerDef sceneStreamer)
        {
            DebugReport(true)?.Invoke(false, $"INTERFACE: Initialize(), sceneCollection: {(sceneCollection == null ? "NULL" : "OK")}, sceneStreamer: {(sceneStreamer == null ? "NULL" : "OK")}");

            AsyncStackHolder.ThrowIfNotInUnityContext();

            var backup = BackupPlayers();

            DeinitializeInternal();

            SceneCollection = sceneCollection;
            SceneStreamer = sceneStreamer;
            InitializeInternal();

            Initialized = true;

            RestorePlayers(backup);

            return Initialized;
        }

        public void Deinitialize()
        {
            DebugReport(true)?.Invoke(false, $"INTERFACE: Deinitialize()");

            AsyncStackHolder.ThrowIfNotInUnityContext();

            DeinitializeInternal();

            SceneCollection = null;
            SceneStreamer = null;

            Initialized = false;
        }

        public SceneStreamerStatus GetStatus(Guid id)
        {
            AsyncStackHolder.ThrowIfNotInUnityContext();

            var returnValue = SceneStreamerStatus.Unknown;
            Player player = null;
            players.TryGetValue(id, out player);
            if (player != null)
            {
                returnValue = player.GetStatus(this);
            }

            //DebugReport(false, false, $"INTERFACE: GetStatus(), player: {id}, status: {returnValue}");

            return returnValue;
        }

        public void SetPosition(Guid id, Vector3 position, bool force)
        {
            //DebugReport(false, false, $"INTERFACE: SetPosition(), player: {id}, position: {position}, force: {force}");

            AsyncStackHolder.ThrowIfNotInUnityContext();

            Player player = null;
            var position2 = new Vector2(position.x, position.z);
            players.TryGetValue(id, out player);
            if (force ||
                (player == null) ||
                !CanStream ||
                ((position2 - player.Position2).sqrMagnitude >= (SceneStreamer.CheckRange * SceneStreamer.CheckRange)))
            {
                if (player == null)
                {
                    player = new Player();
                    player.Id = id;
                    DebugReport(true)?.Invoke(false, $"ADD PLAYER, player: {id}, position: {position}, force: {force}");
                    players.Add(id, player);
                }
                player.ToRemove = false;
                player.Position = position;
                if (CanStream && (sceneCells != null))
                {
                    CheckCellsForPlayer(player);
                }
                if (player.RecentlyAdded)
                {
                    var oldStatus = player.GetStatus(this);
                    player.RecentlyAdded = false;
                    var newStatus = player.GetStatus(this);
                    CallStatusChanged(player, oldStatus, newStatus);
                }
            }

            if (checkPositionCheat && (player != null))
            {
                var threshold = 0.5f;
                var traceDistance = 200.0f;
                var traceMask = Shared.PhysicsLayers.CheckIsGroundedMask;
                var status = player.GetStatus(this);
                RaycastHit hitInfo;
                if (!Physics.Raycast(position, Vector3.down + Vector3.up * threshold, out hitInfo, traceDistance, traceMask))
                {
                    var traceResult = "NO TERRAIN AT ALL";
                    if (Physics.Raycast(position, Vector3.up, out hitInfo, traceDistance, traceMask))
                    {
                        var distance = (hitInfo.point.y - position.y);
                        traceResult = $"{distance}";
                    }
                    DebugError($"SetPosition(), !!!Physics.Raycast() FAIL!!!: status: {status}, terrain: {traceResult}, player: {id}, position: {position}, force: {force}");
                }
            }
        }

        public bool Remove(Guid id)
        {
            DebugReport(true)?.Invoke(false, $"INTERFACE: Remove(), player: {id}");

            AsyncStackHolder.ThrowIfNotInUnityContext();

            Player player = null;
            players.TryGetValue(id, out player);
            if (player != null)
            {
                player.ToRemove = true;
                if (CanStream && (sceneCells != null))
                {
                    CheckCellsForPlayer(player);
                }
                RemovePlayersReadyToRemove(false);
                return true;
            }
            return false;
        }

        public void ShowBackground(bool show, string sceneName, TerrainBakerMaterialSupport terrain)
        {
            DebugReport(true)?.Invoke(false, $"INTERFACE: ShowBackground(), show: {show}, scene name: {sceneName}");

            AsyncStackHolder.ThrowIfNotInUnityContext();

            if (backgroundInitialized && (backgroundCells != null))
            {
                Vector3Int coordinates;
                if (IsSceneForStreaming(sceneName, out coordinates))
                {
                    if ((backgroundCellsBounds.min.x <= coordinates.x) && (backgroundCellsBounds.min.y <= coordinates.z) && (backgroundCellsBounds.max.x > coordinates.x) && (backgroundCellsBounds.max.y > coordinates.z))
                    {
                        var index = GetSceneIndex(backgroundCellsBounds.width,
                                                  coordinates.x - backgroundCellsBounds.min.x,
                                                  coordinates.z - backgroundCellsBounds.min.y);
                        var backgroundCell = backgroundCells[index];
                        if (backgroundCell != null)
                        {
                            backgroundCell.Visible = show;
                            if (backgroundCell.Terrain != null)
                            {
                                backgroundCell.Terrain.SetActive(backgroundCell.Visible && showBackgroundTerrainCheat);
                            }
                            if (backgroundCell.StaticObjects != null)
                            {
                                backgroundCell.StaticObjects.SetActive(backgroundCell.Visible && showBackgroundObjectsCheat);
                            }
                            if (backgroundCell.TerrainHolder != null)
                            {
                                backgroundCell.TerrainHolder.Terrain = terrain;
                            }
                            DebugReport(true)?.Invoke(false, $"ShowBackground(), show: {show}, scene name: {sceneName}");
                        }
                    }
                }
            }
        }

        public void ReportReady(bool ready, string sceneName)
        {
            DebugReport(true)?.Invoke(false, $"INTERFACE: ReportReady(), ready: {ready}, scene name: {sceneName}");

            AsyncStackHolder.ThrowIfNotInUnityContext();

            if (ready)
            {
                OnSceneReady(sceneName);
            }
        }
    }
}