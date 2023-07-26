using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using NLog;
using ResourceSystem.Utils;

namespace SharedCode.Utils.DebugCollector
{
    public static class Collect
    {
        public static readonly Collector Instance = new Collector();

        public static Collector IfActive => Instance.IsActive ? Instance : null;
        
        public static bool IsActive => Instance.IsActive;
    }

    public class Collector
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlockingCollection<UnprocessedEvent> _unprocessedEvents = new BlockingCollection<UnprocessedEvent>(1000);
        private readonly Func<ulong> _clock; //old: = () => (ulong)SyncTime.NowUnsynced;
        private readonly List<ITarget> _targets = new List<ITarget>();
        private Task _processingLoop;
        private bool _disposed;
        private int _eventId;

        internal Collector()
        {
            _clock = GlobalConstsDef.DebugFlagsGetter.IsUseSyncedTimestamperAtDebugCollector(GlobalConstsHolder.GlobalConstsDef)
                ? (Func<ulong>) (() => (ulong) SyncTime.Now)
                : () => (ulong) SyncTime.NowUnsynced; //?toAndrey?: why ulong?;
        }

        public ulong TimeNow => _clock();
        
        public void Dispose()
        {
            _disposed = true;
            _unprocessedEvents.CompleteAdding();
            _unprocessedEvents.Dispose();
        }

        public bool IsActive => _targets.Count != 0 && !_disposed;

        public void Event(string name)
            => _unprocessedEvents.TryAdd(new UnprocessedEvent(_clock(), UnprocessedEventType.Instant, name));

        public void EventBgn(string name, object causer)
            => _unprocessedEvents.TryAdd(new UnprocessedEvent(_clock(), UnprocessedEventType.IntervalBgn, name, causer));

        public void EventEnd(object causer)
            => _unprocessedEvents.TryAdd(new UnprocessedEvent(_clock(), UnprocessedEventType.IntervalEnd, null, causer));

        public void Event(string name, Guid entity)
            => _unprocessedEvents.TryAdd(new UnprocessedEvent(_clock(), UnprocessedEventType.Instant, name, entity));

        public void EventBgn(string name, Guid entity, object causer)
            => _unprocessedEvents.TryAdd(new UnprocessedEvent(_clock(), UnprocessedEventType.IntervalBgn, name, entity, causer));

        public void Event(string name, OuterRef entity) 
            => Event(name, entity.Guid);

        public void EventBgn(string name, OuterRef entity, object causer) 
            => EventBgn(name, entity.Guid, causer);

        public void RegisterTarget(ITarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            lock (_targets)
                if (!_targets.Contains(target))
                    _targets.Add(target);
            RunProcessingLoop();
        }

        public void UnregisterTarget(ITarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            lock (_targets)
                _targets.Remove(target);
        }

        private void RunProcessingLoop()
        {
            if (_processingLoop == null || _processingLoop.IsCompleted)
            {
                _processingLoop = Task.Run(() =>
                {
                    while (!_unprocessedEvents.IsCompleted)
                    {
                        try
                        {
                            ProcessEvent(_unprocessedEvents.Take());
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }
                    }
                });
            }
        }

        private void ProcessEvent(in UnprocessedEvent @event)
        {
            Logger.IfDebug()?.Message($"Process event {@event.Name}").Write();
            try
            {
                lock (_targets)
                    foreach (var target in _targets)
                        switch (@event.Type)
                        {
                            case UnprocessedEventType.Instant:
                                target.AddInstantEvent(++_eventId, @event.Name, @event.Entity, @event.Timestamp);
                                break;
                            case UnprocessedEventType.IntervalBgn:
                                target.AddIntervalBgnEvent(++_eventId, @event.Name, @event.Causer, @event.Entity, @event.Timestamp);
                                break;
                            case UnprocessedEventType.IntervalEnd:
                                target.AddIntervalEndEvent(@event.Causer, @event.Timestamp);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }
        
        private readonly struct UnprocessedEvent
        {
            public readonly ulong Timestamp;
            public readonly UnprocessedEventType Type;
            public readonly object Causer;
            public readonly string Name;
            public readonly Guid Entity;

            public UnprocessedEvent(ulong timestamp, UnprocessedEventType type, string name)
            {
                Timestamp = timestamp;
                Type = type;
                Name = name;
                Causer = null;
                Entity = Guid.Empty;
            }

            public UnprocessedEvent(ulong timestamp, UnprocessedEventType type, string name, Guid entity)
            {
                Timestamp = timestamp;
                Type = type;
                Name = name;
                Causer = null;
                Entity = entity;
            }

            public UnprocessedEvent(ulong timestamp, UnprocessedEventType type, string name, object causer)
            {
                Timestamp = timestamp;
                Type = type;
                Name = name;
                Causer = causer ?? throw new ArgumentNullException(nameof(causer));
                Entity = Guid.Empty;
            }

            public UnprocessedEvent(ulong timestamp, UnprocessedEventType type, string name, Guid entity, object causer)
            {
                Timestamp = timestamp;
                Type = type;
                Name = name;
                Causer = causer ?? throw new ArgumentNullException(nameof(causer));
                Entity = entity;
            }
        }

        private enum UnprocessedEventType
        {
            Instant,
            IntervalBgn,
            IntervalEnd
        }
    }
}