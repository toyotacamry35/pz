using Assets.Src.Aspects.Impl.Stats;
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
    public interface IValueStat : IStat, IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)] float Value { get; set; }
        [ReplicationLevel(ReplicationLevel.Server)] ValueTask<bool> ChangeValue(float delta);
        [ReplicationLevel(ReplicationLevel.Master)] ValueTask Copy(IValueStat valueStat); // TODOA надо  написать адекватную систему копирования состояния статов, а пока так..
    }
}
