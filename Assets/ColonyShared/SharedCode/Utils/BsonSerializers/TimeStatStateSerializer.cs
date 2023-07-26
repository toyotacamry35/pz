using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Src.Aspects.Impl.Stats.Proxy;

namespace SharedCode.Utils.BsonSerialization
{
    public class BsonTimeStatStateSerializer : SerializerBase<TimeStatState>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TimeStatState value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteDouble(nameof(TimeStatState.ChangeRateCache), value.ChangeRateCache);
            context.Writer.WriteInt64(nameof(TimeStatState.LastBreakPointTime), value.LastBreakPointTime);
            context.Writer.WriteDouble(nameof(TimeStatState.LastBreakPointValue), value.LastBreakPointValue);
            context.Writer.WriteEndDocument();
        }

        public override TimeStatState Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            float ChangeRateCache = (float)context.Reader.ReadDouble(nameof(TimeStatState.ChangeRateCache));
            long LastBreakPointTime = context.Reader.ReadInt64(nameof(TimeStatState.LastBreakPointTime));
            float LastBreakPointValue = (float)context.Reader.ReadDouble(nameof(TimeStatState.LastBreakPointValue));
            context.Reader.ReadEndDocument();
            return new TimeStatState(ChangeRateCache, LastBreakPointTime, LastBreakPointValue);
        }
    }
}
