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
    public partial class BuildPlace
    {
        internal static class SendFuncs
        {
            public static ValueTask<bool> RemoveDelay(System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<SharedCode.DeltaObjects.Building.BuildType, System.Guid>> elements, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<SharedCode.DeltaObjects.Building.BuildType, System.Guid>>)elements);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Building.IBuildPlace), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> Check(SharedCode.DeltaObjects.Building.BuildType type, SharedCode.Entities.Building.IPositionedBuildWrapper buildWrapper, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.DeltaObjects.Building.BuildType)type);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Building.IPositionedBuildWrapper)buildWrapper);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Building.IBuildPlace), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> Start(SharedCode.DeltaObjects.Building.BuildType type, SharedCode.Entities.Building.IPositionedBuildWrapper buildWrapper, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.DeltaObjects.Building.BuildType)type);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Building.IPositionedBuildWrapper)buildWrapper);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Building.IBuildPlace), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<bool> OnProgress(SharedCode.DeltaObjects.Building.BuildType type, System.Guid elementId, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.DeltaObjects.Building.BuildType)type);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)elementId);
                    return EntitySystem.RpcHelper.SendRequest<bool>(__buffer__, offset, typeof(SharedCode.Entities.Building.IBuildPlace), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask<SharedCode.Entities.Building.OperationResultEx> Operate(SharedCode.DeltaObjects.Building.BuildType type, System.Guid callerId, System.Guid elementId, SharedCode.Entities.Building.OperationData data, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                Guid __guid__;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillRequestHeader(serializer, __buffer__, out offset, out __guid__, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.DeltaObjects.Building.BuildType)type);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)callerId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (System.Guid)elementId);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Building.OperationData)data);
                    return EntitySystem.RpcHelper.SendRequest<SharedCode.Entities.Building.OperationResultEx>(__buffer__, offset, typeof(SharedCode.Entities.Building.IBuildPlace), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, __guid__, networkProxy, SharedCode.EntitySystem.ServerCoreRuntimeParameters.RpcTimeoutSeconds, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}