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
    public partial class CraftCounter
    {
        internal static class SendFuncs
        {
            public static ValueTask PreventOnCompleteEvent(IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 0, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.DeltaObjects.ICraftCounter), 0, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask OnInit(Assets.Src.Aspects.Impl.Factions.Template.QuestDef questDef, Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef counterDef, SharedCode.EntitySystem.IEntitiesRepository repository, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 1, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.QuestDef)questDef);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef)counterDef);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.IEntitiesRepository)repository);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.DeltaObjects.ICraftCounter), 1, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask OnDatabaseLoad(SharedCode.EntitySystem.IEntitiesRepository repository, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 2, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.IEntitiesRepository)repository);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.DeltaObjects.ICraftCounter), 2, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            public static ValueTask OnDestroy(SharedCode.EntitySystem.IEntitiesRepository repository, IDeltaObject __entity__, INetworkProxy networkProxy, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 3, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.EntitySystem.IEntitiesRepository)repository);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.DeltaObjects.ICraftCounter), 3, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxy, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            internal static ValueTask OnOnCounterCompleted(Assets.Src.Aspects.Impl.Factions.Template.QuestDef arg1, SharedCode.Entities.Engine.IQuestCounter arg2, IDeltaObject __entity__, IEnumerable<INetworkProxy> networkProxies, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 4, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (Assets.Src.Aspects.Impl.Factions.Template.QuestDef)arg1);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Engine.IQuestCounter)arg2);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.DeltaObjects.ICraftCounter), 4, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxies, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }

            internal static ValueTask OnOnCounterChanged(SharedCode.Entities.Engine.IQuestCounter arg, IDeltaObject __entity__, IEnumerable<INetworkProxy> networkProxies, IEntitiesRepository __repository__, Guid migrationId)
            {
                int offset;
                var serializer = ((IEntitiesRepositoryExtension)__repository__).Serializer;
                var __buffer__ = EntitySystem.RpcHelper.BufferPool.Take();
                try
                {
                    __buffer__ = EntitySystem.RpcHelper.FillSendHeader(serializer, __buffer__, out offset, 5, ref migrationId);
                    __buffer__ = EntitySystem.RpcHelper.SerializeObjectId(serializer, __buffer__, ref offset, __entity__);
                    __buffer__ = serializer.Serialize(__buffer__, ref offset, (SharedCode.Entities.Engine.IQuestCounter)arg);
                    return EntitySystem.RpcHelper.SendMessage(__buffer__, offset, typeof(GeneratedCode.DeltaObjects.ICraftCounter), 5, SharedCode.Network.MessageSendOptions.ReliableOrdered, networkProxies, false);
                }
                finally
                {
                    EntitySystem.RpcHelper.BufferPool.Return(__buffer__);
                }
            }
        }
    }
}