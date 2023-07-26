using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.MovementSync;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonCharacterMovementStateFrameSerializer : SerializerBase<CharacterMovementStateFrame>
    {
        public static readonly int Version = 1;

        private BsonCharacterMovementStateSerializer bsonCharacterMovementStateSerializer = new BsonCharacterMovementStateSerializer();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CharacterMovementStateFrame value)
        {
            context.Writer.WriteStartDocument();

            context.Writer.WriteInt64("Counter", value.Counter);

            context.Writer.WriteName("State");
            bsonCharacterMovementStateSerializer.Serialize(context, value.State);

            context.Writer.WriteEndDocument();
        }

        public override CharacterMovementStateFrame Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            long Counter = context.Reader.ReadInt64("Counter");
            string StateName = context.Reader.ReadName();
            CharacterMovementState State = bsonCharacterMovementStateSerializer.Deserialize(context);
            context.Reader.ReadEndDocument();
            return new CharacterMovementStateFrame(Counter, State);
        }
    }
}
