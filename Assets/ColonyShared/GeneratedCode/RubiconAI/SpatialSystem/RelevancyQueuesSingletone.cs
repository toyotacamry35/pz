using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.ColonyShared.SharedCode.Utils.Statistics;
using ColonyShared.SharedCode.Utils;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using ActionType = Assets.Src.RubiconAI.AIWorld.ActionType;

namespace Assets.Src.SpatialSystem
{
    public struct ActionWithType
    {
        public int Index;
        public Func<Task> Action;
        public float Time;
    }

    public class HashSetAndQueue
    {
        // Used to avoid multiple enqueued actions for 1 mob
        public readonly HashSet<int> HashSet = new HashSet<int>();
        // 'cos of using hashsets max queue elems count in worst case could be == num.of mobs
        public readonly Queue<ActionWithType> Queue = new Queue<ActionWithType>();
    }

    public class RelevancyQueuesArray
    {
        /// <summary>
        /// #Note: work with every entry (HashSetAndQueue) under lock by its ref.
        /// </summary>
        protected readonly List<HashSetAndQueue> _array = new List<HashSetAndQueue>((int)ActionType.Max + 1);
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public RelevancyQueuesArray()
        {
            var maxActType = (int)ActionType.Max;
            for (int actType = 0; actType <= maxActType; ++actType)
            {
                _array.Add(new HashSetAndQueue());
                //d.Add((0, 0, 0));
            }

            //sw.Start();
        }

        // --- API: --------------------------------------------------------
        public void AddToQueue(int index, Func<Task> a, ActionType type)
        {
            if (a == null)
            {
                Logger.IfError()?.Message("Action add queue is null").Write();
                return;
            }

            var hashSetAndQueue = _array[(int)type];
            lock (hashSetAndQueue)
            {
                if (hashSetAndQueue.HashSet.Add(index))
                {
                    hashSetAndQueue.Queue.Enqueue(new ActionWithType { Action = a, Index = index, Time = SyncTime.NowUnsynced });
                }
            }
        }

        //Stopwatch sw = new Stopwatch();
        //List<(long, int, int)> d = new List<(long, int, int)>();
        HashSet<int> _addedLegionaries = new HashSet<int>();
        List<SuspendingAwaitable> _processTasksCache = new List<SuspendingAwaitable>();
        public struct ProcessingStatus
        {
            public int HFFarAway { get; set; }
            public int HFCommon { get; set; }
            public int FarAway { get; set; }
            public int Common { get; set; }
            public int Unseen { get; set; }

            internal void Add(ProcessingStatus status)
            {
                HFFarAway += status.HFFarAway;
                HFCommon += status.HFCommon;
                FarAway += status.FarAway;
                Common += status.Common;
                Unseen += status.Unseen;

            }
        }
        public async Task<ProcessingStatus> AsyncProcessQueues(Dictionary<ActionType, int> actionsPerFrame)
        {
            _processTasksCache.Clear();
            _addedLegionaries.Clear();
            ProcessingStatus status = new ProcessingStatus();
            // iterate by queues:
            for (int actTypeIndx = 0; actTypeIndx < _array.Count; ++actTypeIndx)
            {
                //var begin = sw.ElapsedMilliseconds;
                // perform actions:
                var apf = actionsPerFrame[(ActionType)actTypeIndx];
                if (EntitytObjectsUnitySpawnService.SpawnService != null && ((ActionType)actTypeIndx) == ActionType.ThinkFar)
                    apf = 1;

                var hashSetAndQ = _array[actTypeIndx];
                //int a = hashSetAndQ.HashSet.Count;
                for (int i = 0; i < apf; ++i)
                {
                    ActionWithType awt;
                    lock (hashSetAndQ)
                    {
                        if (hashSetAndQ.Queue.Count == 0)
                            break;
                        awt = hashSetAndQ.Queue.Dequeue();
                        hashSetAndQ.HashSet.Remove(awt.Index);
                    }
                    if (_addedLegionaries.Add(awt.Index))
                        _processTasksCache.Add(AsyncUtils.RunAsyncTask(awt.Action));
                    switch ((ActionType)actTypeIndx)
                    {
                        case ActionType.ThinkFar:
                            status.FarAway++;
                            break;
                        case ActionType.ThinkFarHighFreq:
                            status.HFFarAway++;
                            break;
                        case ActionType.ThinkClose:
                            status.Common++;
                            break;
                        case ActionType.ThinkCloseHighFreq:
                            status.HFCommon++;
                            break;
                        case ActionType.ThinkUnseen:
                            status.Unseen++;
                            break;
                    }
                }

                //d[actTypeIndx] = (sw.ElapsedMilliseconds - begin, a, apf);
            }
            //Logger.IfError()?.Message($"Tick {string.Join(",", _addedLegionaries.Select(x=>x).OrderBy(x=>x))}").Write();
            await SuspendingAwaitable.WhenAll(_processTasksCache);
            return status;
            //var end = sw.ElapsedMilliseconds;
            //Logger.IfError()?.Message($"{nameof(AsyncProcessQueues)}: {string.Join(";\t", d.Select(v => $"{v.Item1} ms.: {v.Item2}, -{v.Item3}"))}").Write();
        }
    }
}
