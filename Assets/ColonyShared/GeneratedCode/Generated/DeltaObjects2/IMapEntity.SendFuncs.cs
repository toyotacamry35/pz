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
    public partial class MapEntity
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> SetMapEntityState(SharedCode.MapSystem.MapEntityState state, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MapSystem.MapEntityState)state);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> ChangeChunkDescription(System.Guid descriptionId, SharedCode.MapSystem.MapChunkState newState, System.Guid unityRepositoryId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)descriptionId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MapSystem.MapChunkState)newState);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)unityRepositoryId);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> OnLastUserLeft(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask SpawnNewBots(System.Collections.Generic.List<System.Guid> botIds, string spawnPointTypePath, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<System.Guid>)botIds);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)spawnPointTypePath);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> TryAquireSpawnRightsForPointsSet(SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity> spawner, SharedCode.Entities.GameObjectEntities.SceneChunkDef mapSceneDef, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>)spawner);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.GameObjectEntities.SceneChunkDef)mapSceneDef);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>> GetWorldSpaceForPoint(SharedCode.Utils.Vector3 point, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 5, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Utils.Vector3)point);
                    return EntitySystem.RpcHelper.SendRequest<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 5, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> NotifyAllCharactersViaChat(string text, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 6, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)text);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 6, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            internal static ValueTask OnNewChatMessageEvent(string arg1, string arg2, IDeltaObject __entity__, IEnumerable<INetworkProxy> networkProxies, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 7, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)arg1);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (string)arg2);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MapSystem.IMapEntity), 7, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxies, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}