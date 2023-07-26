using System;

namespace SharedCode.Utils.DebugCollector
{
    public interface ITarget
    {
        void AddInstantEvent(int eventId, string eventName, Guid entityId, ulong timestamp);
        void AddIntervalBgnEvent(int eventId, string eventName, object eventCauser, Guid entityId, ulong timestamp);
        void AddIntervalEndEvent(object eventCauser, ulong timestamp);
    }
}