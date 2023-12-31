// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using System;
using SharedCode.Network;
using System.Threading.Tasks;
using GeneratedCode.EntitySystem;
using SharedCode.EntitySystem;
using System.Collections.Generic;

namespace GeneratedCode.DeltaObjects
{
    public partial class SceneEntity
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> Spawn(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.ISceneEntity), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> Despawn(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.ISceneEntity), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> SetLoadableObj(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> obj, System.Guid fromStatic, SharedCode.Utils.Vector3 wsPos, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)obj);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)fromStatic);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Utils.Vector3)wsPos);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.ISceneEntity), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> RemoveObject(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> obj, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)obj);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.ISceneEntity), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> LoadEntity(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> obj, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)obj);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.ISceneEntity), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> FinishLoading(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 5, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.ISceneEntity), 5, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class EventPoint
    {
        internal static class SendFuncs
        {
            public static ValueTask<SharedCode.Entities.GameObjectEntities.IEntityObjectDef> LoadEvent(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<SharedCode.Entities.GameObjectEntities.IEntityObjectDef>(__buffer__, offset, typeof(GeneratedCode.MapSystem.IEventPoint), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> AssignEvent(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> newEvent, SharedCode.Entities.GameObjectEntities.EventInstanceDef eventDef, Scripting.ScriptingContext ctx, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)newEvent);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.GameObjectEntities.EventInstanceDef)eventDef);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Scripting.ScriptingContext)ctx);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.IEventPoint), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> RemoveEvent(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.IEventPoint), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class Storyteller
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> Tick(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.IStoryteller), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> RegisterFromStaticScene(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.IStoryteller), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}

namespace GeneratedCode.DeltaObjects
{
    public partial class EventInstance
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> Stop(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(GeneratedCode.MapSystem.IEventInstance), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}