using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;

namespace GeneratedCode.DeltaObjects
{
    public partial class TimeSyncerEntity : IHookOnStart, IHookOnDestroy, IHookOnUnload, IHookOnReplicationLevelChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        const int _sendTimePeriodMs = 2000;
        const int _updateTimePeriodMs = 666;

        private CancellationTokenSource _cts;

        public Task OnStart()
        {
            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            var typeName = TypeName;
            var id = Id;
            var repository = EntitiesRepository;
            AsyncUtils.RunAsyncTask(() => PeriodicUpdate(id, typeName, token, repository), repository);
            return Task.CompletedTask;
        }


        private async Task PeriodicUpdate(Guid id, string typeName, CancellationToken token, IEntitiesRepository repository)
        {
            while (true)
            {
                try
                {
                    await Task.Delay(_sendTimePeriodMs, token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (token.IsCancellationRequested || repository.StopToken.IsCancellationRequested)
                    return;

                try
                {
                    using (var wrapper = await repository.Get<ITimeSyncerEntity>(id))
                    {
                        var timeEntity = wrapper?.Get<ITimeSyncerEntity>(id);
                        if (timeEntity == null)
                            continue;
                        await timeEntity.UpdateTime();
                    }
                }
                catch (Exception e)
                {
                    Log.Logger.IfError()?.Message(e, "TimeSyncerEntityImplementation {0} {1} master exception", typeName, id).Write();
                    AsyncUtils.RunAsyncTask(async () => { await PeriodicUpdate(id, typeName, token, repository); }, repository);
                    throw;
                }
            }
        }

        private void DestroyImpl()
        {
            if (_cts == null)
            {
                Logger.IfError()?.Message("_cts == null").Write();
                return;
            }

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        public Task OnDestroy()
        {
            DestroyImpl();
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            DestroyImpl();
            return Task.CompletedTask;
        }


        private async Task PeriodicReplicationLevelChanged(Guid id, string typeName, CancellationToken token, CancellationToken repositoryStopToken, IEntitiesRepository repository)
        {
            var timeSyncPingRelaxation = Constants.WorldConstants.TimeSyncPingRelaxation;
            while (true)
            {
                try
                {
                    await Task.Delay(_updateTimePeriodMs, token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (token.IsCancellationRequested || repositoryStopToken.IsCancellationRequested)
                    return;

                try
                {
                    using (var wrapper = await repository.Get<ITimeSyncerEntity>(id))
                    {
                        var timeEntity = wrapper?.Get<ITimeSyncerEntityClientBroadcast>(id);
                        if (timeEntity == null)
                            continue;
                        var lastSyncAskedLocalUtcTime = SyncTime.NowUnsynced;
                        var latestServerTime = await timeEntity.Ping();
                        long newDeltaVal = latestServerTime - (lastSyncAskedLocalUtcTime + (SyncTime.NowUnsynced - lastSyncAskedLocalUtcTime) / 2);

                        if (SyncTime.DeltaToServer != 0)
                        {
                            SyncTime.DeltaToServer = (long)(SyncTime.DeltaToServer * timeSyncPingRelaxation + newDeltaVal * (1 - timeSyncPingRelaxation));
                        }
                        else
                        {
                            SyncTime.DeltaToServer = newDeltaVal;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Logger.IfError()?.Message("TimeSyncerEntityImplementation {0} {1} replication exception {2}", typeName, id, e.ToString()).Write();
                    AsyncUtils.RunAsyncTask(async () => { await PeriodicReplicationLevelChanged(id, typeName, token, repositoryStopToken, repository); }, repository);
                    throw;
                }
            }
        }

        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            if (ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) &&
                !ReplicationMaskUtils.IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            {
                if(_cts != null)
                {
                    Logger.IfError()?.Message("_cts != null").Write();
                    return;
                }
                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;
                var repositoryStopToken = EntitiesRepository.StopToken;
                var repository = EntitiesRepository;
                var id = Id;
                var typeName = TypeName;
                AsyncUtils.RunAsyncTask(async () => { await PeriodicReplicationLevelChanged(id, typeName, token, repositoryStopToken, repository); }, repository);
            }
            else if (ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.ClientBroadcast) &&
                !ReplicationMaskUtils.IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ReplicationLevel.Master))
            {
                if (_cts == null)
                {
                    Logger.IfError()?.Message("_cts == null").Write();
                    return;
                }
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        public Task<bool> UpdateTimeImpl()
        {
            LastServerTime = SyncTime.NowUnsynced;
            SyncTime.DeltaToServer = 0;
            return Task.FromResult(true);
        }

        public Task<long> PingImpl()
        {
            return Task.FromResult(SyncTime.NowUnsynced);
        }
    }
}
