using GeneratedCode.Network;
using SharedCode.EntitySystem;
using SharedCode.Network;
using System;

namespace GeneratedCode.HandlerProxyImplementations
{
    public readonly struct ServiceHandlersProxy
    {
        private readonly IEntitiesRepository _repository;
        private readonly NetworkMessageDispatcher _networkMessageDispatcher;

        public ServiceHandlersProxy(in NetworkMessageDispatcher dispatcher, IEntitiesRepository repository)
        {
            _repository = repository;
            _networkMessageDispatcher = dispatcher;
        }

        public void OnMessageReceived(byte msgId, byte[] data, int offset, Guid migrationId, INetworkProxy proxy)
        {
            var fi = IncomingInvocationDispatcher.OnMessageReceived(_repository, proxy, proxy.RemoteRepoId, msgId, data, offset, migrationId);
            if (fi != null)
                _networkMessageDispatcher.Add(fi);
        }

        public void OnMessageReceived(byte msgId, byte[] data, int offset, Guid transactionId, Guid migrationId, INetworkProxy proxy)
        {
            var fi = IncomingInvocationDispatcher.OnMessageReceived(_repository, proxy, proxy.RemoteRepoId, msgId, data, offset, transactionId, migrationId);
            if(fi != null)
                _networkMessageDispatcher.Add(fi);
        }

        public void OnTransactionInfoReceived(RemoteCallCompletionFunc func, byte[] data, int offset)
        {
            var fi = new FuncInfoTransactionResult(func, data, offset);
            _networkMessageDispatcher.Add(fi);
        }

        private class FuncInfoTransactionResult : IFuncInfo
        {
            private readonly RemoteCallCompletionFunc func;
            private readonly byte[] data;
            private readonly int offset;

            public void Run()
            {
                func(data, offset);
            }

            public FuncInfoTransactionResult(RemoteCallCompletionFunc func, byte[] data, int offset)
            {
                this.func = func;
                this.data = data;
                this.offset = offset;
            }
        }
    }
}
