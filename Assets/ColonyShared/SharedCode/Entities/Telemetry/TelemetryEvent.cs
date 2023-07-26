using ProtoBuf;
using System;
using Telemetry;

namespace SharedCode.Entities.Telemetry
{
    public enum TelemetryEventType
    {
        None,
        Hardware,
    }

    public enum TelemetryEventErrorCode
    {
        None,
        Success,
    }

    [ProtoContract]
    public class TelemetryEventResult
    {
        [ProtoMember(1)]
        public TelemetryEventType Type { get; set; } = TelemetryEventType.None;

        [ProtoMember(2)]
        public TelemetryEventErrorCode Result { get; set; } = TelemetryEventErrorCode.None;
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(HardwareEvent))]
    public class TelemetryEvent
    {
        [ProtoMember(1)]
        public string IndexName { get; set; } = "telemetry_event";

        [ProtoMember(2)]
        public TelemetryEventType Type { get; set; } = TelemetryEventType.None;

        [ProtoMember(3)]
        public DateTime Time { get; set; } = DateTime.UtcNow;

        [Nest.Keyword]
        [ProtoMember(4)]
        public Guid CharacterId { get; set; }

        [Nest.Keyword]
        [ProtoMember(5)]
        public Guid SessionId { get; set; }

        public void Index()
        {
            ElasticAccessor.Index(this, IndexName);
        }
    }

    [ProtoContract]
    public class HardwareEvent : TelemetryEvent
    {
        [Nest.Keyword]
        [ProtoMember(6)]
        public string VideoCard { get; set; }

        public HardwareEvent()
        {
            IndexName = "telemetry_event_hardware";
            Type = TelemetryEventType.Hardware;
        }
    }
}
