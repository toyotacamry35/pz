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
    public class RealmsCollectionEntityChainProxy : BaseChainEntity
    {
        public RealmsCollectionEntityChainProxy(SharedCode.Entities.IRealmsCollectionEntity entity): base(entity)
        {
        }

        public RealmsCollectionEntityChainProxy(SharedCode.Entities.IRealmsCollectionEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public RealmsCollectionEntityChainProxy AddRealm(ChainArgument<System.Guid> mapId, ChainArgument<SharedCode.Aspects.Sessions.RealmRulesDef> realmDef)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (mapId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)mapId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)mapId);
                if (realmDef is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)realmDef).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Aspects.Sessions.RealmRulesDef)realmDef);
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

        public RealmsCollectionEntityChainProxy RemoveRealm(ChainArgument<System.Guid> mapId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (mapId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)mapId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)mapId);
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

        public RealmsCollectionEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public RealmsCollectionEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public RealmsCollectionEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static RealmsCollectionEntityChainProxy Chain(this SharedCode.Entities.IRealmsCollectionEntity entity)
        {
            return new RealmsCollectionEntityChainProxy(entity);
        }

        public static RealmsCollectionEntityChainProxy ContinueChain(this SharedCode.Entities.IRealmsCollectionEntity entity, IChainedEntity fromChain)
        {
            return new RealmsCollectionEntityChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class RealmEntityChainProxy : BaseChainEntity
    {
        public RealmEntityChainProxy(SharedCode.Entities.IRealmEntity entity): base(entity)
        {
        }

        public RealmEntityChainProxy(SharedCode.Entities.IRealmEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public RealmEntityChainProxy TryAttach(ChainArgument<System.Guid> account)
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

        public RealmEntityChainProxy Enter(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> account)
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
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)account);
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

        public RealmEntityChainProxy Leave(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> account)
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
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)account);
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

        public RealmEntityChainProxy AddMap(ChainArgument<System.Guid> mapId, ChainArgument<SharedCode.MapSystem.MapMeta> mapMeta)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (mapId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)mapId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)mapId);
                if (mapMeta is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)mapMeta).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MapSystem.MapMeta)mapMeta);
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

        public RealmEntityChainProxy RemoveMap(ChainArgument<System.Guid> mapId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (mapId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)mapId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)mapId);
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

        public RealmEntityChainProxy SetActive(ChainArgument<bool> active)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (active is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)active).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)active);
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

        public RealmEntityChainProxy SetMapDead(ChainArgument<System.Guid> mapId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (mapId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)mapId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)mapId);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 6, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public RealmEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public RealmEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public RealmEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static RealmEntityChainProxy Chain(this SharedCode.Entities.IRealmEntity entity)
        {
            return new RealmEntityChainProxy(entity);
        }

        public static RealmEntityChainProxy ContinueChain(this SharedCode.Entities.IRealmEntity entity, IChainedEntity fromChain)
        {
            return new RealmEntityChainProxy(entity, fromChain);
        }
    }
}