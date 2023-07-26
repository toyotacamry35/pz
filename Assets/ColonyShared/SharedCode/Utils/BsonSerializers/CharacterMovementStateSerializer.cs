using ColonyShared.SharedCode.Aspects.Locomotion;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.MovementSync;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonCharacterMovementStateSerializer : SerializerBase<CharacterMovementState>
    {
        public static readonly int Version = 1;

        private BsonVector3Serializer bsonVector3Serializer = new BsonVector3Serializer();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CharacterMovementState value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("Position");
            bsonVector3Serializer.Serialize(context, value.Position);

            context.Writer.WriteName("Velocity");
            bsonVector3Serializer.Serialize(context, value.Velocity);

            context.Writer.WriteDouble("Orientation", value.Orientation);
            context.Writer.WriteInt32("Flags", (int)value.Flags);

            context.Writer.WriteEndDocument();
        }

        public override CharacterMovementState Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            string PositionName = context.Reader.ReadName();
            Vector3 Position = bsonVector3Serializer.Deserialize(context);
            string VelocityName = context.Reader.ReadName();
            Vector3 Velocity = bsonVector3Serializer.Deserialize(context);
            float Orientation = (float)context.Reader.ReadDouble("Orientation");
            LocomotionFlags Flags = (LocomotionFlags)context.Reader.ReadInt32("Flags");
            context.Reader.ReadEndDocument();
            return new CharacterMovementState(Position, Velocity, Orientation, Flags);
        }
    }
}
