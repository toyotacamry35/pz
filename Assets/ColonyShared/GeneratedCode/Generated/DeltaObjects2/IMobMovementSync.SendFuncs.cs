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
    public partial class MobMovementSync
    {
        internal static class SendFuncs
        {
            public static ValueTask SetMovementData(SharedCode.MovementSync.MovementData data, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MovementSync.MovementData)data);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> SetPathFindingOwnerRepositoryId(System.Guid repositoryId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)repositoryId);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask UpdateMovement(SharedCode.MovementSync.MobMovementStatePacked state, long counter, bool important, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MovementSync.MobMovementStatePacked)state);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (long)counter);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)important);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask StopMovement(SharedCode.Wizardry.SpellId spellId, GeneratedDefsForSpells.MoveEffectDef moveEffectDef, bool success, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Wizardry.SpellId)spellId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (GeneratedDefsForSpells.MoveEffectDef)moveEffectDef);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)success);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask InvokeSetDebugMobPositionLoggingEvent(bool enabledStatus, bool dump, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)enabledStatus);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)dump);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            internal static ValueTask On__SyncMovementUnreliable(SharedCode.MovementSync.MobMovementStatePacked arg, IDeltaObject __entity__, IEnumerable<INetworkProxy> networkProxies, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 5, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.MovementSync.MobMovementStatePacked)arg);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 5, SharedCode.Network.MessageSendOptions.Unreliable, networkProxies, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            internal static ValueTask OnSetDebugMobPositionLoggingEvent(bool arg1, bool arg2, IDeltaObject __entity__, IEnumerable<INetworkProxy> networkProxies, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 6, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)arg1);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (bool)arg2);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(SharedCode.MovementSync.IMobMovementSync), 6, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxies, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}