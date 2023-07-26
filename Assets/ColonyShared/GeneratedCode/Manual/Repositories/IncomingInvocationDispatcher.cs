using GeneratedCode.EntitySystem;
using GeneratedCode.Network;
using GeneratedCode.Network.Statistic;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Network;
using System;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using SharedCode.Repositories;
using SharedCode.Refs;

namespace GeneratedCode.HandlerProxyImplementations
{
    internal static class IncomingInvocationDispatcher
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private class FuncInfo : IFuncInfo
        {

            private readonly ReplicaTypeRegistry.IncomingMessageFunc func;
            private readonly IEntitiesRepository repo;
            private readonly INetworkProxy proxy;
            private readonly Guid callback;
            private readonly byte[] data;
            private readonly int offset;
            private readonly PropertyAddress id;
            private readonly Guid transactionId;
            private readonly Guid migrationId;
            private readonly IDeltaObject obj;

            internal FuncInfo(ReplicaTypeRegistry.IncomingMessageFunc func, IEntitiesRepository repo, INetworkProxy proxy, Guid callback, byte[] data, int offset, PropertyAddress id, Guid transactionId, Guid migrationId, IDeltaObject obj)
            {
                this.func = func;
                this.repo = repo;
                this.proxy = proxy;
                this.callback = callback;
                this.data = data;
                this.offset = offset;
                this.id = id;
                this.transactionId = transactionId;
                this.migrationId = migrationId;
                this.obj = obj;
            }
            public void Run()
            {
                func(repo, proxy, callback, data, offset, id, obj, transactionId, migrationId);
            }
        }

        public static IFuncInfo OnMessageReceived(IEntitiesRepository repo, INetworkProxy proxy, Guid callback, byte msgId, byte[] data, int offset, Guid migrationId)
            => OnMessageReceived(repo, proxy, callback, msgId, data, offset, Guid.Empty, migrationId);
        public static IFuncInfo OnMessageReceived(IEntitiesRepository repo, INetworkProxy proxy, Guid callback, byte msgId, byte[] data, int offset, Guid transactionId, Guid migrationId)
        {
            var serializer = proxy.Serializer;
            var id = RpcHelper.DeserializeObjectId(serializer, data, ref offset);

            var entityType = ReplicaTypeRegistry.GetTypeById(id.EntityTypeId);

            var entityRef = (IEntityRefExt)((IEntitiesRepositoryExtension)repo).GetRef(id.EntityTypeId, id.EntityId);
            if(entityRef == null)
            {
                Logger.IfWarn()?.Message("Target entity {0} {1} not found for rpc {2}", entityType, id.EntityId, msgId).Write();
                return null;
            }

            var obj = EntityPropertyResolver.Resolve<IDeltaObject>(entityRef.GetEntity(), id);
            var typeId = obj.TypeId;
            var type = ReplicaTypeRegistry.GetTypeById(typeId);

            (var dispatcher, var messageName) = ReplicaTypeRegistry.GetDispatcher(type, msgId);
            Statistics<RpcNetworkStatistics>.Instance.ReceiveBytes(type, msgId, data.Length);

            var fi = new FuncInfo(dispatcher, repo, proxy, callback, data, offset, id, transactionId, migrationId, obj);
            return fi;
        }
    }
}
