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
    public class MapEntityChainProxy : BaseChainEntity
    {
        public MapEntityChainProxy(SharedCode.MapSystem.IMapEntity entity): base(entity)
        {
        }

        public MapEntityChainProxy(SharedCode.MapSystem.IMapEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public MapEntityChainProxy SetMapEntityState(ChainArgument<SharedCode.MapSystem.MapEntityState> state)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (state is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)state).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MapSystem.MapEntityState)state);
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

        public MapEntityChainProxy ChangeChunkDescription(ChainArgument<System.Guid> descriptionId, ChainArgument<SharedCode.MapSystem.MapChunkState> newState, ChainArgument<System.Guid> unityRepositoryId)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (descriptionId is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)descriptionId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)descriptionId);
                if (newState is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)newState).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MapSystem.MapChunkState)newState);
                if (unityRepositoryId is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)unityRepositoryId).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)unityRepositoryId);
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

        public MapEntityChainProxy OnLastUserLeft()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public MapEntityChainProxy SpawnNewBots(ChainArgument<System.Collections.Generic.List<System.Guid>> botIds, ChainArgument<string> spawnPointTypePath)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (botIds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)botIds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<System.Guid>)botIds);
                if (spawnPointTypePath is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)spawnPointTypePath).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)spawnPointTypePath);
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

        public MapEntityChainProxy TryAquireSpawnRightsForPointsSet(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> spawner, ChainArgument<SharedCode.Entities.GameObjectEntities.SceneChunkDef> mapSceneDef)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (spawner is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)spawner).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)spawner);
                if (mapSceneDef is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)mapSceneDef).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.GameObjectEntities.SceneChunkDef)mapSceneDef);
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

        public MapEntityChainProxy GetWorldSpaceForPoint(ChainArgument<SharedCode.Utils.Vector3> point)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (point is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)point).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Utils.Vector3)point);
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

        public MapEntityChainProxy NotifyAllCharactersViaChat(ChainArgument<string> text)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (text is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)text).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)text);
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

        public MapEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public MapEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public MapEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static MapEntityChainProxy Chain(this SharedCode.MapSystem.IMapEntity entity)
        {
            return new MapEntityChainProxy(entity);
        }

        public static MapEntityChainProxy ContinueChain(this SharedCode.MapSystem.IMapEntity entity, IChainedEntity fromChain)
        {
            return new MapEntityChainProxy(entity, fromChain);
        }
    }
}