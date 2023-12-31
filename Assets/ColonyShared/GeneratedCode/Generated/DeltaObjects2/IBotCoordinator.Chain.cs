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
    public class BotCoordinatorChainProxy : BaseChainEntity
    {
        public BotCoordinatorChainProxy(GeneratedCode.EntityModel.Bots.IBotCoordinator entity): base(entity)
        {
        }

        public BotCoordinatorChainProxy(GeneratedCode.EntityModel.Bots.IBotCoordinator entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public BotCoordinatorChainProxy Register()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public BotCoordinatorChainProxy Initialize(ChainArgument<GeneratedCode.Custom.Config.MapDef> mapDef, ChainArgument<System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>> botsRefs, ChainArgument<SharedCode.AI.LegionaryEntityDef> botConfig)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (mapDef is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)mapDef).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (GeneratedCode.Custom.Config.MapDef)mapDef);
                if (botsRefs is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)botsRefs).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>)botsRefs);
                if (botConfig is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)botConfig).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.AI.LegionaryEntityDef)botConfig);
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

        public BotCoordinatorChainProxy ActivateBots(ChainArgument<System.Guid> account, ChainArgument<System.Collections.Generic.List<System.Guid>> botsIds)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (account is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)account).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)account);
                if (botsIds is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)botsIds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<System.Guid>)botsIds);
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

        public BotCoordinatorChainProxy DeactivateBots(ChainArgument<System.Guid> account)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (account is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)account).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)account);
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

        public BotCoordinatorChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public BotCoordinatorChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public BotCoordinatorChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static BotCoordinatorChainProxy Chain(this GeneratedCode.EntityModel.Bots.IBotCoordinator entity)
        {
            return new BotCoordinatorChainProxy(entity);
        }

        public static BotCoordinatorChainProxy ContinueChain(this GeneratedCode.EntityModel.Bots.IBotCoordinator entity, IChainedEntity fromChain)
        {
            return new BotCoordinatorChainProxy(entity, fromChain);
        }
    }
}