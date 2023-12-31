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
    public class WorldCoordinatorNodeServiceEntityChainProxy : BaseChainEntity
    {
        public WorldCoordinatorNodeServiceEntityChainProxy(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity entity): base(entity)
        {
        }

        public WorldCoordinatorNodeServiceEntityChainProxy(SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public WorldCoordinatorNodeServiceEntityChainProxy RequestLoginToMap(ChainArgument<SharedCode.Entities.Service.MapLoginMeta> loginMeta)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (loginMeta is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)loginMeta).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Service.MapLoginMeta)loginMeta);
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

        public WorldCoordinatorNodeServiceEntityChainProxy RequestLogoutFromMap(ChainArgument<System.Guid> userId, ChainArgument<bool> terminal)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (userId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)userId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)userId);
                if (terminal is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)terminal).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)terminal);
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

        public WorldCoordinatorNodeServiceEntityChainProxy CancelMapRequest(ChainArgument<System.Guid> requestId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (requestId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)requestId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)requestId);
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

        public WorldCoordinatorNodeServiceEntityChainProxy UpdateMapQueue()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public WorldCoordinatorNodeServiceEntityChainProxy GetMapIdByUserId(ChainArgument<System.Guid> userId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (userId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)userId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)userId);
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

        public WorldCoordinatorNodeServiceEntityChainProxy GlobalNotification(ChainArgument<string> notificationText)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (notificationText is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)notificationText).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)notificationText);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 5, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public WorldCoordinatorNodeServiceEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public WorldCoordinatorNodeServiceEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public WorldCoordinatorNodeServiceEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static WorldCoordinatorNodeServiceEntityChainProxy Chain(this SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity entity)
        {
            return new WorldCoordinatorNodeServiceEntityChainProxy(entity);
        }

        public static WorldCoordinatorNodeServiceEntityChainProxy ContinueChain(this SharedCode.Entities.Service.IWorldCoordinatorNodeServiceEntity entity, IChainedEntity fromChain)
        {
            return new WorldCoordinatorNodeServiceEntityChainProxy(entity, fromChain);
        }
    }
}