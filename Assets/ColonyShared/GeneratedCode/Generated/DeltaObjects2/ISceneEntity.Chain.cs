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
    public class SceneEntityChainProxy : BaseChainEntity
    {
        public SceneEntityChainProxy(GeneratedCode.MapSystem.ISceneEntity entity): base(entity)
        {
        }

        public SceneEntityChainProxy(GeneratedCode.MapSystem.ISceneEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public SceneEntityChainProxy Spawn()
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

        public SceneEntityChainProxy Despawn()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public SceneEntityChainProxy SetLoadableObj(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> obj, ChainArgument<System.Guid> fromStatic, ChainArgument<SharedCode.Utils.Vector3> wsPos)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (obj is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)obj).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)obj);
                if (fromStatic is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)fromStatic).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)fromStatic);
                if (wsPos is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)wsPos).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Utils.Vector3)wsPos);
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

        public SceneEntityChainProxy RemoveObject(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> obj)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (obj is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)obj).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)obj);
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

        public SceneEntityChainProxy LoadEntity(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> obj)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (obj is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)obj).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)obj);
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

        public SceneEntityChainProxy FinishLoading()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public SceneEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public SceneEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public SceneEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static SceneEntityChainProxy Chain(this GeneratedCode.MapSystem.ISceneEntity entity)
        {
            return new SceneEntityChainProxy(entity);
        }

        public static SceneEntityChainProxy ContinueChain(this GeneratedCode.MapSystem.ISceneEntity entity, IChainedEntity fromChain)
        {
            return new SceneEntityChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class EventPointChainProxy : BaseChainEntity
    {
        public EventPointChainProxy(GeneratedCode.MapSystem.IEventPoint entity): base(entity)
        {
        }

        public EventPointChainProxy(GeneratedCode.MapSystem.IEventPoint entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public EventPointChainProxy LoadEvent()
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

        public EventPointChainProxy AssignEvent(ChainArgument<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> newEvent, ChainArgument<SharedCode.Entities.GameObjectEntities.EventInstanceDef> eventDef, ChainArgument<Scripting.ScriptingContext> ctx)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (newEvent is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)newEvent).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)newEvent);
                if (eventDef is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)eventDef).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.GameObjectEntities.EventInstanceDef)eventDef);
                if (ctx is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)ctx).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Scripting.ScriptingContext)ctx);
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

        public EventPointChainProxy RemoveEvent()
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

        public EventPointChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public EventPointChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public EventPointChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static EventPointChainProxy Chain(this GeneratedCode.MapSystem.IEventPoint entity)
        {
            return new EventPointChainProxy(entity);
        }

        public static EventPointChainProxy ContinueChain(this GeneratedCode.MapSystem.IEventPoint entity, IChainedEntity fromChain)
        {
            return new EventPointChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class StorytellerChainProxy : BaseChainEntity
    {
        public StorytellerChainProxy(GeneratedCode.MapSystem.IStoryteller entity): base(entity)
        {
        }

        public StorytellerChainProxy(GeneratedCode.MapSystem.IStoryteller entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public StorytellerChainProxy Tick()
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

        public StorytellerChainProxy RegisterFromStaticScene()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public StorytellerChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public StorytellerChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public StorytellerChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static StorytellerChainProxy Chain(this GeneratedCode.MapSystem.IStoryteller entity)
        {
            return new StorytellerChainProxy(entity);
        }

        public static StorytellerChainProxy ContinueChain(this GeneratedCode.MapSystem.IStoryteller entity, IChainedEntity fromChain)
        {
            return new StorytellerChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class EventInstanceChainProxy : BaseChainEntity
    {
        public EventInstanceChainProxy(GeneratedCode.MapSystem.IEventInstance entity): base(entity)
        {
        }

        public EventInstanceChainProxy(GeneratedCode.MapSystem.IEventInstance entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public EventInstanceChainProxy Stop()
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

        public EventInstanceChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public EventInstanceChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public EventInstanceChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static EventInstanceChainProxy Chain(this GeneratedCode.MapSystem.IEventInstance entity)
        {
            return new EventInstanceChainProxy(entity);
        }

        public static EventInstanceChainProxy ContinueChain(this GeneratedCode.MapSystem.IEventInstance entity, IChainedEntity fromChain)
        {
            return new EventInstanceChainProxy(entity, fromChain);
        }
    }
}