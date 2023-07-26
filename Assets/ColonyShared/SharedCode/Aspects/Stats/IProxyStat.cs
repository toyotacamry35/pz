using GeneratorAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.Serializers.Protobuf;
using System.Threading.Tasks;

namespace Src.Aspects.Impl.Stats
{
    [ProtoContract]
    [GenerateDeltaObjectCode]
    public interface IProxyStat : IStat, IDeltaObject
    {
        [BsonIgnore, ReplicationLevel(ReplicationLevel.ClientFull)] float ValueCache { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] Task ProxySubscribe(PropertyAddress containerAddress);
    }
}
