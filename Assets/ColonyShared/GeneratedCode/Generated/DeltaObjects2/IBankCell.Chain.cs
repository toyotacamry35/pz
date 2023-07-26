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
    public class BankCellChainProxy : BaseChainEntity
    {
        public BankCellChainProxy(SharedCode.Entities.IBankCell entity): base(entity)
        {
        }

        public BankCellChainProxy(SharedCode.Entities.IBankCell entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public BankCellChainProxy GetOpenOuterRef(ChainArgument<ResourceSystem.Utils.OuterRef> oref)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (oref is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)oref).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (ResourceSystem.Utils.OuterRef)oref);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 0, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public BankCellChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public BankCellChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public BankCellChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static BankCellChainProxy Chain(this SharedCode.Entities.IBankCell entity)
        {
            return new BankCellChainProxy(entity);
        }

        public static BankCellChainProxy ContinueChain(this SharedCode.Entities.IBankCell entity, IChainedEntity fromChain)
        {
            return new BankCellChainProxy(entity, fromChain);
        }
    }
}