using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SharedCode.Entities;

namespace SharedCode.Utils.BsonSerialization
{

    public class BsonTransformSerializer : SerializerBase<Transform>
    {
        public static readonly int Version = 1;

        private BsonVector3Serializer bsonVector3Serializer = new BsonVector3Serializer();
        private BsonQuaternionSerializer bsonQuaternionSerializer = new BsonQuaternionSerializer();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Transform value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("Position");
            bsonVector3Serializer.Serialize(context, value.Position);

            context.Writer.WriteName("Scale");
            bsonVector3Serializer.Serialize(context, value.Scale);

            context.Writer.WriteName("Rotation");
            bsonQuaternionSerializer.Serialize(context, value.Rotation);
            context.Writer.WriteEndDocument();
        }

        public override Transform Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            string PositionName = context.Reader.ReadName();
            Vector3 Position = bsonVector3Serializer.Deserialize(context);
            string ScaleName = context.Reader.ReadName();
            Vector3 Scale = bsonVector3Serializer.Deserialize(context);
            string RotationName = context.Reader.ReadName();
            Quaternion Rotation = bsonQuaternionSerializer.Deserialize(context);
            context.Reader.ReadEndDocument();
            return new Transform(Position, Rotation, Scale);
        }
    }
}
