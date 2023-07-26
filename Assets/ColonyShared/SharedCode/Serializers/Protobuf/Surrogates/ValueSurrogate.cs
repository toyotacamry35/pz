using System;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode;
using ProtoBuf;
using ResourceSystem.Utils;
using SharedCode.Utils;

namespace SharedCode.Serializers.Protobuf.Surrogates
{
    [ProtoContract]
    public class ValueSurrogate
    {

        static ProtoBufSerializer serializer = new ProtoBufSerializer();
        [ProtoMember(1)]
        public byte[] SubData { get; set; }

        public static implicit operator ValueSurrogate(Value u)
        {
            var result = new ValueSurrogate();

            if (u.ValueType != Value.Type.None)
            {
                var __buffer__ = global::GeneratedCode.EntitySystem.RpcHelper.BufferPool.Take();
                int offset = 0;
                try
                {
                    serializer.Serialize(__buffer__, ref offset, u.ValueType);
                    switch (u.ValueType)
                    {
                        case Value.Type.None:
                            break;
                        case Value.Type.Float:
                            serializer.Serialize(__buffer__, ref offset, u.Float);
                            break;
                        case Value.Type.Vector2:
                            serializer.Serialize(__buffer__, ref offset, u.Vector2);
                            break;
                        case Value.Type.Vector3:
                            serializer.Serialize(__buffer__, ref offset, u.Vector3);
                            break;
                        case Value.Type.Int:
                            serializer.Serialize(__buffer__, ref offset, u.Int);
                            break;
                        case Value.Type.Long:
                            serializer.Serialize(__buffer__, ref offset, u.Long);
                            break;
                        case Value.Type.Bool:
                            serializer.Serialize(__buffer__, ref offset, u.Bool);
                            break;
                        case Value.Type.String:
                            serializer.Serialize(__buffer__, ref offset, u.String);
                            break;
                        case Value.Type.Color:
                            serializer.Serialize(__buffer__, ref offset, u.Color);
                            break;
                        case Value.Type.DateTime:
                            serializer.Serialize(__buffer__, ref offset, u.DateTime);
                            break;
                        case Value.Type.OuterRef:
                            serializer.Serialize(__buffer__, ref offset, u.OuterRef);
                            break;
                        case Value.Type.Resource:
                            serializer.Serialize(__buffer__, ref offset, u.Resource);
                            break;
                    }
                }
                finally
                {
                    result.SubData = new byte[offset];
                    Array.Copy(__buffer__, 0, result.SubData, 0, offset);
                    global::GeneratedCode.EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }

            }

            return result;
        }

        public static implicit operator Value(ValueSurrogate surrogate)
        {
            if (surrogate.SubData == null || surrogate.SubData.Length == 0)
                return default;
            int offset = 0;
            var valType = serializer.Deserialize<Value.Type>(surrogate.SubData, ref offset);
            Value val = default;
            switch (valType)
            {
                case Value.Type.None:
                    break;
                case Value.Type.Float:
                    val = new Value(serializer.Deserialize<float>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Vector2:
                    val = new Value(serializer.Deserialize<Vector2>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Vector3:
                    val = new Value(serializer.Deserialize<Vector3>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Int:
                    val = new Value(serializer.Deserialize<int>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Long:
                    val = new Value(serializer.Deserialize<long>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Bool:
                    val = new Value(serializer.Deserialize<bool>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.String:
                    val = new Value(serializer.Deserialize<string>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Color:
                    val = new Value(serializer.Deserialize<Color>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.DateTime:
                    val = new Value(serializer.Deserialize<DateTime>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.OuterRef:
                    val = new Value(serializer.Deserialize<OuterRef>(surrogate.SubData, ref offset));
                    break;
                case Value.Type.Resource:
                    val = new Value(serializer.Deserialize<BaseResource>(surrogate.SubData, ref offset));
                    break;
            }
            return val;
        }
    }
}
