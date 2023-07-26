using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ResourceSystem.Utils;
using System;

namespace SharedCode.Utils.BsonSerialization
{

    public class ValueSerializer : SerializerBase<Value>
    {
        public static readonly int Version = 1;

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Value value)
        {
            context.Writer.WriteStartDocument();
            context.Writer.WriteName("Type");
            context.Writer.WriteString(value.ValueType.ToString());
            if(value.ValueType != Value.Type.None)
            context.Writer.WriteName("Value");
            switch (value.ValueType)
            {
                case Value.Type.None:
                    break;
                case Value.Type.Float:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Float);
                    break;
                case Value.Type.Vector2:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Vector2);
                    break;
                case Value.Type.Vector3:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Vector3);
                    break;
                case Value.Type.Quaternion:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Quaternion);
                    break;
                case Value.Type.Int:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Int);
                    break;
                case Value.Type.Long:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Long);
                    break;
                case Value.Type.ULong:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.ULong);
                    break;
                case Value.Type.Bool:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Bool);
                    break;
                case Value.Type.String:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.String);
                    break;
                case Value.Type.Color:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Color);
                    break;
                case Value.Type.DateTime:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.DateTime);
                    break;
                case Value.Type.OuterRef:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.OuterRef);
                    break;
                case Value.Type.Resource:
                    BsonSerializer.Serialize(context.Writer, Value.SupportedTypes[(int)value.ValueType].Item2, value.Resource);
                    break;
            }
            context.Writer.WriteEndDocument();
        }

        public override Value Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            context.Reader.ReadStartDocument();
            context.Reader.ReadName();
            var valueType = (Value.Type)Enum.Parse(typeof(Value.Type), context.Reader.ReadString());
            Value val = default;
            if (valueType != Value.Type.None)
                context.Reader.ReadName();
            switch (valueType)
            {
                case Value.Type.None:
                    break;
                case Value.Type.Float:
                    val = new Value(BsonSerializer.Deserialize<float>(context.Reader));
                    break;
                case Value.Type.Vector2:
                    val = new Value(BsonSerializer.Deserialize<Vector2>(context.Reader));
                    break;
                case Value.Type.Vector3:
                    val = new Value(BsonSerializer.Deserialize<Vector3>(context.Reader));
                    break;
                case Value.Type.Quaternion:
                    val = new Value(BsonSerializer.Deserialize<Quaternion>(context.Reader));
                    break;
                case Value.Type.Int:
                    val = new Value(BsonSerializer.Deserialize<int>(context.Reader));
                    break;
                case Value.Type.Long:
                    val = new Value(BsonSerializer.Deserialize<long>(context.Reader));
                    break;
                case Value.Type.ULong:
                    val = new Value(BsonSerializer.Deserialize<ulong>(context.Reader));
                    break;
                case Value.Type.Bool:
                    val = new Value(BsonSerializer.Deserialize<bool>(context.Reader));
                    break;
                case Value.Type.String:
                    val = new Value(BsonSerializer.Deserialize<string>(context.Reader));
                    break;
                case Value.Type.Color:
                    val = new Value(BsonSerializer.Deserialize<Color>(context.Reader));
                    break;
                case Value.Type.DateTime:
                    val = new Value(BsonSerializer.Deserialize<DateTime>(context.Reader));
                    break;
                case Value.Type.OuterRef:
                    val = new Value(BsonSerializer.Deserialize<OuterRef>(context.Reader));
                    break;
                case Value.Type.Resource:
                    val = new Value(BsonSerializer.Deserialize<IResource>(context.Reader));
                    break;
            }
        
            context.Reader.ReadEndDocument();
            return val;
        }


    }
}
