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
    public class SerializationTestEntityChainProxy : BaseChainEntity
    {
        public SerializationTestEntityChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity entity): base(entity)
        {
        }

        public SerializationTestEntityChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public SerializationTestEntityChainProxy SetValue(ChainArgument<int> value)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (value is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)value).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)value);
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

        public SerializationTestEntityChainProxy FillTestProperty(ChainArgument<System.Guid> id)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (id is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)id).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)id);
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

        public SerializationTestEntityChainProxy RemoveTestProperty()
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

        public SerializationTestEntityChainProxy FillTestProperty2()
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

        public SerializationTestEntityChainProxy SetTestProperty2WithNewDletaObject()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public SerializationTestEntityChainProxy FillTestProperty2FromTestProperty()
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

        public SerializationTestEntityChainProxy RemoveTestProperty2()
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
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

        public SerializationTestEntityChainProxy AddToList(ChainArgument<Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2> element)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (element is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)element).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2)element);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 7, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public SerializationTestEntityChainProxy AddToDictionary(ChainArgument<int> key, ChainArgument<Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2> element)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (key is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)key).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)key);
                if (element is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)element).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2)element);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 8, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public SerializationTestEntityChainProxy SetList(ChainArgument<SharedCode.EntitySystem.Delta.IDeltaList<Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2>> list)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (list is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)list).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.Delta.IDeltaList<Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2>)list);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 9, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public SerializationTestEntityChainProxy RemoveFromList(ChainArgument<Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2> element)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (element is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)element).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.ColonyShared.SharedCode.Entities.Test.ITestDeltaObject2)element);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 10, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public SerializationTestEntityChainProxy RemoveFromDictionary(ChainArgument<int> key)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (key is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)key).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)key);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 11, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public SerializationTestEntityChainProxy SetOnDestroy(ChainArgument<int> newValue)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (newValue is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)newValue).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)newValue);
                byte[] buffer = new byte[offset];
                Buffer.BlockCopy(__buffer__, 0, buffer, 0, offset);
                var __newBlock__ = new ChainBlockCall(__id__, 12, buffer, argumetRefs);
                validateCallBlock(__newBlock__);
                chainBatch.Chain.Add(__newBlock__);
            }
            finally
            {
                EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
            }

            return this;
        }

        public SerializationTestEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public SerializationTestEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public SerializationTestEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static SerializationTestEntityChainProxy Chain(this Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity entity)
        {
            return new SerializationTestEntityChainProxy(entity);
        }

        public static SerializationTestEntityChainProxy ContinueChain(this Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity entity, IChainedEntity fromChain)
        {
            return new SerializationTestEntityChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class SerializationTestEntity2ChainProxy : BaseChainEntity
    {
        public SerializationTestEntity2ChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity2 entity): base(entity)
        {
        }

        public SerializationTestEntity2ChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity2 entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public SerializationTestEntity2ChainProxy SetValue(ChainArgument<int> value)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (value is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)value).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)value);
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

        public SerializationTestEntity2ChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public SerializationTestEntity2ChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public SerializationTestEntity2ChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static SerializationTestEntity2ChainProxy Chain(this Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity2 entity)
        {
            return new SerializationTestEntity2ChainProxy(entity);
        }

        public static SerializationTestEntity2ChainProxy ContinueChain(this Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity2 entity, IChainedEntity fromChain)
        {
            return new SerializationTestEntity2ChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class SerializationTestEntity3ChainProxy : BaseChainEntity
    {
        public SerializationTestEntity3ChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity3 entity): base(entity)
        {
        }

        public SerializationTestEntity3ChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity3 entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public SerializationTestEntity3ChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public SerializationTestEntity3ChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public SerializationTestEntity3ChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static SerializationTestEntity3ChainProxy Chain(this Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity3 entity)
        {
            return new SerializationTestEntity3ChainProxy(entity);
        }

        public static SerializationTestEntity3ChainProxy ContinueChain(this Assets.ColonyShared.SharedCode.Entities.Test.ISerializationTestEntity3 entity, IChainedEntity fromChain)
        {
            return new SerializationTestEntity3ChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class SaveToDbEntityTestChainProxy : BaseChainEntity
    {
        public SaveToDbEntityTestChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISaveToDbEntityTest entity): base(entity)
        {
        }

        public SaveToDbEntityTestChainProxy(Assets.ColonyShared.SharedCode.Entities.Test.ISaveToDbEntityTest entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public SaveToDbEntityTestChainProxy SetTestPropertyValue(ChainArgument<int> value)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (value is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)value).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)value);
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

        public SaveToDbEntityTestChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public SaveToDbEntityTestChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public SaveToDbEntityTestChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static SaveToDbEntityTestChainProxy Chain(this Assets.ColonyShared.SharedCode.Entities.Test.ISaveToDbEntityTest entity)
        {
            return new SaveToDbEntityTestChainProxy(entity);
        }

        public static SaveToDbEntityTestChainProxy ContinueChain(this Assets.ColonyShared.SharedCode.Entities.Test.ISaveToDbEntityTest entity, IChainedEntity fromChain)
        {
            return new SaveToDbEntityTestChainProxy(entity, fromChain);
        }
    }
}