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
    public class TimeoutTestEntityChainProxy : BaseChainEntity
    {
        public TimeoutTestEntityChainProxy(GeneratedCode.EntityModel.Test.ITimeoutTestEntity entity): base(entity)
        {
        }

        public TimeoutTestEntityChainProxy(GeneratedCode.EntityModel.Test.ITimeoutTestEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public TimeoutTestEntityChainProxy LongUsage()
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

        public TimeoutTestEntityChainProxy ShortUsage()
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

        public TimeoutTestEntityChainProxy SetTestProperty(ChainArgument<int> value)
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

        public TimeoutTestEntityChainProxy AwaitWriteTimeSec(ChainArgument<float> seconds)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
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

        public TimeoutTestEntityChainProxy AwaitWriteTimeSecAndSetTestProperty(ChainArgument<float> seconds, ChainArgument<int> value)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
                if (value is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)value).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)value);
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

        public TimeoutTestEntityChainProxy AwaitReadTimeSec(ChainArgument<float> seconds)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
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

        public TimeoutTestEntityChainProxy AwaitWriteTimeSecAndCallSubTestEntityRpcWithAwait(ChainArgument<float> seconds, ChainArgument<float> subseconds, ChainArgument<int> value)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
                if (subseconds is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)subseconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)subseconds);
                if (value is IChainResult)
                    argumetRefs.Add(2, ((IChainResult)value).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)value);
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

        public TimeoutTestEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public TimeoutTestEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public TimeoutTestEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static TimeoutTestEntityChainProxy Chain(this GeneratedCode.EntityModel.Test.ITimeoutTestEntity entity)
        {
            return new TimeoutTestEntityChainProxy(entity);
        }

        public static TimeoutTestEntityChainProxy ContinueChain(this GeneratedCode.EntityModel.Test.ITimeoutTestEntity entity, IChainedEntity fromChain)
        {
            return new TimeoutTestEntityChainProxy(entity, fromChain);
        }
    }
}

namespace GeneratedCode.DeltaObjects.Chain
{
    [GeneratedCode("CodeGen", "1.0")]
    public class TimeoutSubTestEntityChainProxy : BaseChainEntity
    {
        public TimeoutSubTestEntityChainProxy(GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity entity): base(entity)
        {
        }

        public TimeoutSubTestEntityChainProxy(GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity entity, IChainedEntity fromChain): base(entity, fromChain)
        {
        }

        public TimeoutSubTestEntityChainProxy LongUsage()
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

        public TimeoutSubTestEntityChainProxy ShortUsage()
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

        public TimeoutSubTestEntityChainProxy SetTestProperty(ChainArgument<int> value)
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

        public TimeoutSubTestEntityChainProxy AwaitWriteTimeSec(ChainArgument<float> seconds)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
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

        public TimeoutSubTestEntityChainProxy AwaitWriteTimeSecAndSetTestProperty(ChainArgument<float> seconds, ChainArgument<int> value)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
                if (value is IChainResult)
                    argumetRefs.Add(1, ((IChainResult)value).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (int)value);
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

        public TimeoutSubTestEntityChainProxy AwaitReadTimeSec(ChainArgument<float> seconds)
        {
            var argumetRefs = new Dictionary<int, string>();
            var __id__ = SharedCode.EntitySystem.EntityPropertyResolvers.EntityPropertyResolver.GetPropertyAddress(__entity__);
            int offset = 0;
            var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
            try
            {
                var serializer = ServicesPool.Services.Get<SharedCode.Serializers.ISerializer>();
                if (seconds is IChainResult)
                    argumetRefs.Add(0, ((IChainResult)seconds).Key);
                else
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (float)seconds);
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

        public TimeoutSubTestEntityChainProxy Delay(float duration, bool repeat = false, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, repeat ? -1 : 0, fromUtcNow));
            return this;
        }

        public TimeoutSubTestEntityChainProxy DelayCount(float duration, int count, bool fromUtcNow = true)
        {
            chainBatch.Chain.Add(new ChainBlockPeriod(duration, count, fromUtcNow));
            return this;
        }

        public TimeoutSubTestEntityChainProxy StoreResult(string name)
        {
            ((ChainBlockCall)chainBatch.Chain.Last()).SetStoreResultKey(name);
            return this;
        }
    }

    public static partial class ChainProxyExtensions
    {
        public static TimeoutSubTestEntityChainProxy Chain(this GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity entity)
        {
            return new TimeoutSubTestEntityChainProxy(entity);
        }

        public static TimeoutSubTestEntityChainProxy ContinueChain(this GeneratedCode.EntityModel.Test.ITimeoutSubTestEntity entity, IChainedEntity fromChain)
        {
            return new TimeoutSubTestEntityChainProxy(entity, fromChain);
        }
    }
}