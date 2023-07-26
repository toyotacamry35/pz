using System;

namespace SharedCode.Utils.DebugCollector
{
    public readonly struct Event
    {
        public readonly int Uid;
        public readonly EventType Type;
        public readonly string Name;
        public readonly Guid Entity;
        public readonly ulong BgnTime;
        public readonly ulong EndTime;

        public Event(int uid, EventType type, string name, Guid entity, ulong bgn, ulong end)
        {
            Uid = uid;
            BgnTime = bgn;
            EndTime = end;
            Type = type;
            Name = name;
            Entity = entity;
        }
    }

    public enum EventType
    {
        Instant,
        Interval,
    }
}