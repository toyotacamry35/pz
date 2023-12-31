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
    public class ChainCallServiceEntityInternalChainProxy : BaseChainEntity
    {
        public ChainCallServiceEntityInternalChainProxy(SharedCode.Entities.Service.IChainCallServiceEntityInternal entity): base(entity)
        {
        }

        public ChainCallServiceEntityInternalChainProxy(SharedCode.Entities.Service.IChainCallServiceEntityInternal entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public ChainCallServiceEntityInternalChainProxy ChainCall(ChainArgument<SharedCode.Entities.Core.IEntityMethodsCallsChain> chainCall)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (chainCall is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)chainCall).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Core.IEntityMethodsCallsChain)chainCall);
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

        public ChainCallServiceEntityInternalChainProxy AddExistingChainCalls(ChainArgument<System.Collections.Generic.List<SharedCode.Entities.Core.IEntityMethodsCallsChain>> chainCalls, ChainArgument<int> typeId, ChainArgument<System.Guid> entityId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (chainCalls is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)chainCalls).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<SharedCode.Entities.Core.IEntityMethodsCallsChain>)chainCalls);
                if (typeId is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)typeId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                if (entityId is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)entityId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 1, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public ChainCallServiceEntityInternalChainProxy ChainCallBatch(ChainArgument<System.Collections.Generic.List<SharedCode.Entities.Core.IEntityMethodsCallsChain>> chainCalls)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (chainCalls is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)chainCalls).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<SharedCode.Entities.Core.IEntityMethodsCallsChain>)chainCalls);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 2, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public ChainCallServiceEntityInternalChainProxy CancelChain(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId, ChainArgument<System.Guid> chainId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (typeId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)typeId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                if (entityId is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)entityId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                if (chainId is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)chainId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)chainId);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 3, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public ChainCallServiceEntityInternalChainProxy CancelAllChain(ChainArgument<int> typeId, ChainArgument<System.Guid> entityId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (typeId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)typeId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)typeId);
                if (entityId is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)entityId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)entityId);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 4, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public ChainCallServiceEntityInternalChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public ChainCallServiceEntityInternalChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public ChainCallServiceEntityInternalChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static ChainCallServiceEntityInternalChainProxy Chain(this SharedCode.Entities.Service.IChainCallServiceEntityInternal entity)
        {
            return new ChainCallServiceEntityInternalChainProxy(entity);
        }

        public static ChainCallServiceEntityInternalChainProxy ContinueChain(this SharedCode.Entities.Service.IChainCallServiceEntityInternal entity, IChainedEntity fromChain)
        {
            return new ChainCallServiceEntityInternalChainProxy(entity, fromChain);
        }
    }
}