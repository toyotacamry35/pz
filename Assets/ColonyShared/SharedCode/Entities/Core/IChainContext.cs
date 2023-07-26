using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using ProtoBuf;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities.Core
{
    [GenerateDeltaObjectCode]
    public interface IChainContext: IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        IDeltaDictionary<string, ChainContextValueWrapper> Data { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)]
        ValueTask<TryGetContextValueResult> TryGetContextValue(string key);

        [ReplicationLevel(ReplicationLevel.Master)]
        ValueTask SetContextValue(string key, object value);

        [ReplicationLevel(ReplicationLevel.Master)]
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        ValueTask<IChainContext> CloneChainContext();
    }

    [ProtoContract]
    public class TryGetContextValueResult
    {
        [ProtoMember(1)]
        public bool Result { get; set; }

        [ProtoMember(2, AsReference = false, DynamicType = true)]
        public object Value { get; set; }
    }

    [ProtoContract]
    public class ChainContextValueWrapper
    {
        [ProtoMember(1, AsReference = false, DynamicType = true)]
        public object Value { get; set; }
    }
}
