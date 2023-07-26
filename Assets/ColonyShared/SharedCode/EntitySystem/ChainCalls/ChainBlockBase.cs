using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using SharedCode.Entities.Core;
using SharedCode.Entities.Service;
using SharedCode.Serializers.Protobuf;

namespace SharedCode.EntitySystem.ChainCalls
{
    [ProtoContract]
    [AutoProtoIncludeSubTypes]
    [BsonKnownTypes(typeof(ChainBlockCall), typeof(ChainBlockPeriod))]
    public abstract class ChainBlockBase
    {
        public abstract Task<bool> Execute(IEntityMethodsCallsChain chainCall, IChainCallServiceEntityInternal chainCallService);

        public abstract void AppendToStringBuilder(StringBuilder sb);
    }
}
