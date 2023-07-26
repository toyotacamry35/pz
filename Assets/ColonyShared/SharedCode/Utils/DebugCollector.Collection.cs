using System;
using System.Collections.Generic;
using Assets.Src.Tools;
using Src.Locomotion;

namespace SharedCode.Utils.DebugCollector
{
    public class Collection
    {
        private readonly int _intervalDuration;
        private int _bufferCapacity;

        private readonly RingBuffer<Event> _events;
        private readonly Dictionary<object, int> _causerToIdx = new Dictionary<object, int>(); // храним direct индексы из _events 
        private readonly Dictionary<int, object> _idxToCauser = new Dictionary<int, object>(); // храним direct индексы мз _events 
        private readonly HashSet<int> _unfinishedEvents = new HashSet<int>(); // здесь тоже direct индекс из _events
        private readonly Dictionary<ulong, HashSet<int>> _intervals = new Dictionary<ulong, HashSet<int>>(); // и здесь, в hashset, тоже direct индексы из _events
        private readonly object _lock = new object();

        public Collection(int bufferCapacity, int intervalDuration = 1000)
        {
            _intervalDuration = intervalDuration;
            _events = new RingBuffer<Event>(bufferCapacity);
        }

        public bool Empty => _events.Count == 0;
        
        public IEnumerable<Event> GetEvents(ulong timeRangeBgn, ulong timeRangeEnd)
        {
            var intervalBgn = GetTimestampIntervalId(timeRangeBgn);
            var intervalEnd = GetTimestampIntervalId(timeRangeEnd) + 1;
            lock(_lock)
                for (ulong i = intervalBgn; i < intervalEnd; ++i)
                    if (_intervals.TryGetValue(i, out var interval))
                        foreach (var idx in interval)
                            yield return _events.DirectGet(idx);
        }

        public Event GetEarliestEvent()
        {
            lock (_lock)
            {
                ulong intervalKey = ulong.MaxValue;
                HashSet<int> interval = null;
                foreach (var i in _intervals)
                {
                    if (i.Value.Count > 0 && i.Key < intervalKey)
                    {
                        intervalKey = i.Key;
                        interval = i.Value;
                    }
                }
                if (interval != null)
                {
                    ulong eventTimestamp = ulong.MaxValue;
                    int eventIdx = -1;
                    foreach (int i in interval)
                    {
                        ref var ev = ref _events.DirectGet(i);
                        if (ev.BgnTime < eventTimestamp)
                        {
                            eventTimestamp = ev.BgnTime;
                            eventIdx = i;
                        }
                    }
                    if (eventIdx != -1)
                        return _events.DirectGet(eventIdx);
                }
            }
            throw new InvalidOperationException("Collection is empty");
        }

        public bool AddInstant(int eventId, string eventName, Guid entityId, ulong timestamp)
        {
            lock (_lock)
            {
                var (index, displaced) = _events.PushBackEx(new Event(eventId, EventType.Instant, eventName, entityId, timestamp, timestamp));
                if (displaced) Displaced(index);
                AddToInterval(index, timestamp);
            }
            return true;
        }

        public bool AddIntervalBgn(int eventId, string eventName, object eventCauser, Guid entityId, ulong timestamp)
        {
            lock (_lock)
            {
                var (index, displaced) = _events.PushBackEx(new Event(eventId, EventType.Interval, eventName, entityId, timestamp, ulong.MaxValue));
                if (displaced) Displaced(index);
                AddToInterval(index, timestamp);
                _causerToIdx.Add(eventCauser, index);
                _idxToCauser.Add(index, eventCauser);
                _unfinishedEvents.Add(index);
            }
            return true;
        }

        public bool AddIntervalEnd(object eventCauser, ulong timestamp)
        {
            lock (_lock)
                if (_causerToIdx.TryGetValue(eventCauser, out var index))
                {
                    var e = _events.DirectGet(index);
                    _events.DirectSet(index, new Event(e.Uid, EventType.Interval, e.Name, e.Entity, e.BgnTime, timestamp));
                    AddToInterval(index, timestamp);
                    _causerToIdx.Remove(eventCauser);
                    _idxToCauser.Remove(index);
                    _unfinishedEvents.Remove(index);
                    return true;
                }
            return false;
        }

        public void Clear()
        {
            lock (_lock)
            {
                _events.Clear();
                _intervals.Clear();
                _causerToIdx.Clear();
                _idxToCauser.Clear();
                _unfinishedEvents.Clear();
            }
        }
        
        public void FinishAllUnfinishedEvents(ulong timestamp)
        {
            lock (_lock)
            {
                foreach (var index in _unfinishedEvents)
                {
                    var e = _events.DirectGet(index);
                    _events.DirectSet(index, new Event(e.Uid, EventType.Interval, e.Name, e.Entity, e.BgnTime, timestamp));
                    AddToInterval(index, timestamp);
                    if (_idxToCauser.TryGetValue(index, out var causer))
                        _causerToIdx.Remove(causer);
                }
                _unfinishedEvents.Clear();
            }
        }

        public void UpdateUnfinishedEvents(ulong timestamp)
        {
            lock (_lock)
                foreach (var eventIdx in _unfinishedEvents)
                    AddToInterval(eventIdx, timestamp);
        }
        
        private void Displaced(int index)
        {
            _unfinishedEvents.Remove(index);

            var intervalsToRemove = new List<ulong>();
            foreach (var interval in _intervals)
                if (interval.Value.Remove(index) && interval.Value.Count == 0)
                    intervalsToRemove.Add(interval.Key);
            foreach (var i in intervalsToRemove)
                _intervals.Remove(i);

            if (_idxToCauser.TryGetValue(index, out var causer))
            {
                _idxToCauser.Remove(index);
                _causerToIdx.Remove(causer);
            }
        }

        private void AddToInterval(int idx, ulong timestamp)
        {
            var milestone = GetTimestampIntervalId(timestamp);
            _intervals.GetOrCreate(milestone).Add(idx);
        }

        private ulong GetTimestampIntervalId(ulong timestamp)
        {
            return (timestamp / (ulong) _intervalDuration);
        }
    }
}