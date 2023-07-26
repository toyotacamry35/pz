// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Logging;
using SharedCode.OurSimpleIoC;
using SharedCode.Utils;
using System.Linq;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class EntityObjectsChainProxy : BaseChainEntity
    {
        public EntityObjectsChainProxy(SharedCode.Entities.GameObjectEntities.IEntityObjects entity): base(entity)
        {
        }

        public EntityObjectsChainProxy(SharedCode.Entities.GameObjectEntities.IEntityObjects entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public EntityObjectsChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public EntityObjectsChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public EntityObjectsChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static EntityObjectsChainProxy Chain(this SharedCode.Entities.GameObjectEntities.IEntityObjects entity)
        {
            return new EntityObjectsChainProxy(entity);
        }

        public static EntityObjectsChainProxy ContinueChain(this SharedCode.Entities.GameObjectEntities.IEntityObjects entity, IChainedEntity fromChain)
        {
            return new EntityObjectsChainProxy(entity, fromChain);
        }
    }
}