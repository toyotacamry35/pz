using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Src.ContainerApis
{
    public class CraftVirtualQueue
    {
        private Dictionary<int, CraftVirtualQueueSlot> _queue = new Dictionary<int, CraftVirtualQueueSlot>(); //синхронный доступ из Unity
        private bool _isChangedCraftQueueSlots;
        private List<CraftQueueSlot> _newCraftQueueSlots = new List<CraftQueueSlot>(); //доступ из t-p
        private CraftQueueSlot _emptyCraftQueueSlot = new CraftQueueSlot();
        private CraftQueueSlotComparer _comparer = new CraftQueueSlotComparer();

        private CraftEngineQueueFullApi _craftEngineQueueFullApi;
        private bool _isBenchSequencePlayed;

        private List<AddetTaskInfo> _addedTasks = new List<AddetTaskInfo>();


        //=== Props ===========================================================

        private bool _isQueuePaused;

        private bool IsQueuePaused
        {
            get { return _isQueuePaused; }
            set
            {
                if (_isQueuePaused != value)
                {
                    _isQueuePaused = value;
                    if (_isQueuePaused)
                        OnIsQueuePausedBecomeTrue();
                }
            }
        }


        //=== Ctor ============================================================

        public CraftVirtualQueue(CraftEngineQueueFullApi craftEngineQueueFullApi)
        {
            if (!craftEngineQueueFullApi.AssertIfNull(nameof(craftEngineQueueFullApi)))
                _craftEngineQueueFullApi = craftEngineQueueFullApi;
        }


        //=== Public ==========================================================

        public void OnSubscribeRequest(int slotIndex, QueueSlotChangedDelegate onQueueSlotChanged)
        {
            var craftVirtualQueueSlot = GetCraftVirtualQueueSlot(slotIndex);
            craftVirtualQueueSlot.AddSubscriber(onQueueSlotChanged);
        }

        public void OnUnsubscribeRequest(int slotIndex, QueueSlotChangedDelegate onQueueSlotChanged)
        {
            CraftVirtualQueueSlot craftVirtualQueueSlot;
            if (!_queue.TryGetValue(slotIndex, out craftVirtualQueueSlot))
                return;

            craftVirtualQueueSlot.RemoveSubscriber(onQueueSlotChanged);
        }

        public void ChangeCraftQueueSlots(List<CraftQueueSlot> craftQueueSlots = null) //из t-p
        {
            if (craftQueueSlots != null)
                lock (_newCraftQueueSlots)
                {
                    _newCraftQueueSlots = craftQueueSlots;
                }

            _isChangedCraftQueueSlots = true;
        }

        public void OnUpdate()
        {
            if (!_isChangedCraftQueueSlots)
                return;

            bool isFirstSlotChanged = false;
            for (int i = 0, len = Mathf.Max(_queue.Count, _newCraftQueueSlots.Count); i < len; i++)
            {
                var virtualQueueSlot = GetCraftVirtualQueueSlot(i);
                UpdateSlotIfNotEquals(i < _newCraftQueueSlots.Count ? _newCraftQueueSlots[i] : _emptyCraftQueueSlot, virtualQueueSlot);

                if (virtualQueueSlot.IsChanged)
                {
                    if (i == 0)
                        isFirstSlotChanged = true;
                    virtualQueueSlot.NotifyChanges();
                }
            }

            _isChangedCraftQueueSlots = false;

            if (isFirstSlotChanged)
                BenchSequenceCheck();

            AddRemoveTasksCheck();
            IsQueuePaused = GetIsQueuePaused();
        }

        private void BenchSequenceCheck()
        {
            var firstCraftQueueSlot = GetCraftVirtualQueueSlot(0).SelfSlot;
            if (_isBenchSequencePlayed != IsActiveWorkbenchCraftQueueSlot(firstCraftQueueSlot))
                SendBenchSequenceChange(!_isBenchSequencePlayed);
        }

        private static bool IsActiveWorkbenchCraftQueueSlot(CraftQueueSlot slot)
        {
            return !slot.IsEmpty && slot.CraftRecipe.HasWorkbenchTypes && slot.IsActive;
        }

        private void AddRemoveTasksCheck()
        {
            var addedTasksIndices = _addedTasks.Select(ati => ati.InnerIndex).ToArray();
            var queueTasksIndices = _queue.Values.Where(val => !val.SelfSlot.IsEmpty).Select(val => val.SelfSlot.KeyIndex).ToArray();

            var newTasksIndices = queueTasksIndices.Except(addedTasksIndices).ToArray();
            var oldTasksIndices = addedTasksIndices.Except(queueTasksIndices).ToArray();

//            UI.CallerLog($"addedTasksIndices={addedTasksIndices.ItemsToString()}    " +
//                         $"queueTasksIndices={queueTasksIndices.ItemsToString()}\n" +
//                         $"new={newTasksIndices.ItemsToString()}   old={oldTasksIndices.ItemsToString()}"); //2del

            if (oldTasksIndices.Length > 0)
            {
                for (int i = 0; i < oldTasksIndices.Length; i++)
                    _addedTasks.Remove(_addedTasks.Find(t => t.InnerIndex == oldTasksIndices[i]));

                _craftEngineQueueFullApi.SendQueueEvent(
                    _addedTasks.Count == 0 ? CraftQueueEvent.QueueCompleted : CraftQueueEvent.TaskCompleted);
            }

            if (newTasksIndices.Length > 0)
            {
                var firstAddedRecipe = _queue.Values.FirstOrDefault()?.SelfSlot.CraftRecipe;
                var isQueueStarted = _addedTasks.Count == 0 && firstAddedRecipe != null;
                var isWorkbenchRecipeAdded = firstAddedRecipe?.WorkbenchTypes?.Where(v => v.Target != null).Any() != null;
                for (int i = 0; i < newTasksIndices.Length; i++)
                    _addedTasks.Add(new AddetTaskInfo(newTasksIndices[i]));

                if (isQueueStarted && isWorkbenchRecipeAdded) //звуков не нужно, из другого места будет активирован верстачный луп
                    return;

                _craftEngineQueueFullApi.SendQueueEvent(isQueueStarted
                    ? CraftQueueEvent.HandcraftQueueStarted
                    : CraftQueueEvent.TaskAdded);
            }
        }

        private bool GetIsQueuePaused()
        {
            return _queue.Values.Any(t => !t.SelfSlot.IsEmpty && !t.SelfSlot.IsActive);
        }

        private void OnIsQueuePausedBecomeTrue()
        {
            _craftEngineQueueFullApi.SendQueueEvent(CraftQueueEvent.QueuePaused);
        }

        private void SendBenchSequenceChange(bool newIsBenchSequencePlayed)
        {
            _isBenchSequencePlayed = newIsBenchSequencePlayed;
            _craftEngineQueueFullApi.SendQueueEvent(
                _isBenchSequencePlayed ? CraftQueueEvent.BenchSequenceStarted : CraftQueueEvent.BenchSequenceEnded);
        }


        //=== Private =========================================================

        private CraftVirtualQueueSlot GetCraftVirtualQueueSlot(int slotIndex)
        {
            CraftVirtualQueueSlot craftVirtualQueueSlot;
            if (!_queue.TryGetValue(slotIndex, out craftVirtualQueueSlot))
            {
                craftVirtualQueueSlot = new CraftVirtualQueueSlot(slotIndex);
                _queue.Add(slotIndex, craftVirtualQueueSlot);
            }

            return craftVirtualQueueSlot;
        }

        private void UpdateSlotIfNotEquals(CraftQueueSlot fromCraftQueueSlot, CraftVirtualQueueSlot toVirtualQueueSlot)
        {
            if (toVirtualQueueSlot.AssertIfNull(nameof(toVirtualQueueSlot)) ||
                fromCraftQueueSlot.AssertIfNull(nameof(fromCraftQueueSlot)))
                return;

            lock (fromCraftQueueSlot)
            {
                if (_comparer.Equals(fromCraftQueueSlot, toVirtualQueueSlot.SelfSlot))
                    return;

                toVirtualQueueSlot.Set(fromCraftQueueSlot);
            }
        }



        //=== Class ===================================================================================================

        private class CraftVirtualQueueSlot
        {
            private event QueueSlotChangedDelegate QueueSlotChanged;


            //=== Props =======================================================

            public CraftQueueSlot SelfSlot { get; } = new CraftQueueSlot();

            public int SlotIndex { get; }

            public bool IsChanged { get; private set; }


            //=== Ctor ========================================================

            public CraftVirtualQueueSlot(int index)
            {
                SlotIndex = index;
            }


            //=== Public ======================================================

            public void AddSubscriber(QueueSlotChangedDelegate onQueueSlotChanged)
            {
                QueueSlotChanged += onQueueSlotChanged;
                if (!SelfSlot.IsEmpty)
                    onQueueSlotChanged.Invoke(SlotIndex, SelfSlot);
            }

            public void RemoveSubscriber(QueueSlotChangedDelegate onQueueSlotChanged)
            {
                QueueSlotChanged -= onQueueSlotChanged;
                if (!SelfSlot.IsEmpty)
                    onQueueSlotChanged.Invoke(SlotIndex, new CraftQueueSlot());
            }

            public void NotifyChanges()
            {
                QueueSlotChanged?.Invoke(SlotIndex, SelfSlot);
                IsChanged = false;
            }

            public void Set(CraftQueueSlot newCraftQueueSlot)
            {
                SelfSlot.SetFrom(newCraftQueueSlot);
                IsChanged = true;
            }

            public override string ToString()
            {
                return $"({nameof(CraftVirtualQueueSlot)}[{SlotIndex}]: {nameof(IsChanged)}{IsChanged.AsSign()}, " +
                       $"{nameof(SelfSlot)}={SelfSlot} /cvqs)";
            }
        }

        private class AddetTaskInfo
        {
            public int InnerIndex { get; }

            public AddetTaskInfo(int innerIndex)
            {
                InnerIndex = innerIndex;
            }
        }
    }
}