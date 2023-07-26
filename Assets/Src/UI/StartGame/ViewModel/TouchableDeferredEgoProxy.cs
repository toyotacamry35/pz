using System;
using System.Threading;
using System.Threading.Tasks;
using Assets.Src.Utils;
using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace Uins
{
    public class TouchableDeferredEgoProxy<TEntity> : ITouchable<TEntity> where TEntity : class, IEntity
    {
        private CancellationTokenSource _cts;
        private TouchableEgoProxy<TEntity> _entityProxy = TouchableUtils.CreateEgoProxy<TEntity>();

        private Guid _guid;

        private Action _onConnected;
        private Action _onTimeout;
        private ReplicationLevel _replicationLevel;
        private IEntitiesRepository _repository;
        private TaskCompletionSource<bool> _tcs;
        private int _typeId;

        public void Dispose()
        {
            DisconnectInternal();

            if (_entityProxy != null)
            {
                _entityProxy.Dispose();
                _entityProxy = null;
            }

            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }

        public IDisposable Subscribe(IToucher<TEntity> toucher)
        {
            return _entityProxy.Subscribe(toucher);
        }

        public string DeepLog(string prefix)
        {
            return _entityProxy.DeepLog(prefix);
        }

        public void Connect(
            IEntitiesRepository repository,
            int typeId,
            Guid guid,
            ReplicationLevel replicationLevel,
            TimeSpan timeout,
            Action onConnected,
            Action onTimeout = null
            )
        {
            DisconnectInternal();

            _guid = guid;
            _typeId = typeId;
            _repository = repository;
            _replicationLevel = replicationLevel;
            _onConnected = onConnected;
            _onTimeout = onTimeout;

            _tcs = new TaskCompletionSource<bool>();
            _cts = new CancellationTokenSource();

            ConnectInternal(timeout);
        }

        private void ConnectInternal(TimeSpan timeout)
        {
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    try
                    {
                        await CheckEntityReplication(_cts.Token);

                        var task = await Task.WhenAny(_tcs.Task, Task.Delay(timeout, _cts.Token));
                        
                        if (task == _tcs.Task)
                        {
                            var onConnected = _onConnected;
                            _cts.Cancel();
                            
                            if (onConnected != null)
                                UnityQueueHelper.RunInUnityThreadNoWait(onConnected.Invoke);
                        }
                        else
                        {
                            var onTimeout = _onTimeout;
                            DisconnectInternal();
                            
                            if (onTimeout != null)
                                UnityQueueHelper.RunInUnityThreadNoWait(onTimeout.Invoke);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            );
        }

        private async Task CheckEntityReplication(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var wrapper = await _repository.Get(_typeId, _guid))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var entity = wrapper?.Get<TEntity>(_typeId, _guid, _replicationLevel);
                    if (entity == null)
                        //Wait Replication
                        _repository.NewEntityUploaded += OnEntitiesRepositoryEntityUpdated;
                    else
                        CreateConnection();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private Task OnEntitiesRepositoryEntityUpdated(int typeId, Guid guid)
        {
            if (_typeId == typeId && _guid.Equals(guid))
            {
                _repository.NewEntityUploaded -= OnEntitiesRepositoryEntityUpdated;
                AsyncUtils.RunAsyncTask(() => CheckEntityReplication(_cts.Token));
            }

            return Task.CompletedTask;
        }

        private void CreateConnection()
        {
            _entityProxy.Connect(
                _repository,
                _typeId,
                _guid,
                ReplicationLevel.ClientFull
            );

            _tcs.SetResult(true);
        }

        private void DisconnectInternal()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }

            if (_tcs != null)
            {
                _tcs.TrySetCanceled();
                _tcs = null;
            }

            if (_repository != null)
            {
                _repository.NewEntityUploaded -= OnEntitiesRepositoryEntityUpdated;
                _repository = null;
            }

            _onTimeout = null;
            _onConnected = null;

            _entityProxy.Disconnect();
        }

        public void Disconnect()
        {
            DisconnectInternal();
        }
    }
}