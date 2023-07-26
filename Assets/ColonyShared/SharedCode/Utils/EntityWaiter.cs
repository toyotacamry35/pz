using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

using static Assets.ColonyShared.SharedCode.Utils.EntityWaiterLogger;


namespace Assets.ColonyShared.SharedCode.Utils
{
    public interface ICancelable
    {
        bool Cancel();
    }
    internal static class EntityWaiterLogger
    {
        [NotNull] internal static readonly Logger EwLogger = LogManager.GetLogger(nameof(EntityWaiter<IEntity>));
        internal const bool Dbg = false;
        internal static void DbgLog(string s)
        {
            if (Dbg) if (Utils.DbgLog.Enabled) Utils.DbgLog.Log(s);
        }
    }
    public class EntityWaiter<TEntity> : ICancelable where TEntity : class, IEntity
    {
        private readonly Guid _guid;
        private readonly int _typeId;
        private readonly ReplicationLevel _replicationLevel;

        private IEntitiesRepository _repository;
        private CancellationTokenSource _cts;
        private TaskCompletionSource<bool> _tcs;
        private Action _doWhenGotEntity;
        private Action _onTimeout;

        public static EntityWaiter<TEntity> NewOnEntityReceivedRequest(IEntitiesRepository repository,
            int typeId,
            Guid guid,
            ReplicationLevel replicationLevel,
            TimeSpan timeout,
            Action doWhenGotEntity,
            Action onTimeout = null)
        {
            return new EntityWaiter<TEntity>(repository, typeId, guid, replicationLevel, timeout, doWhenGotEntity, onTimeout);
        }


        private EntityWaiter(IEntitiesRepository repository,
            int typeId,
            Guid guid,
            ReplicationLevel replicationLevel,
            TimeSpan timeout,
            Action doWhenGotEntity,
            Action onTimeout)
        {
            DbgLog("-0");

            _guid = guid;
            _typeId = typeId;
            _repository = repository;
            _replicationLevel = replicationLevel;
            _doWhenGotEntity = doWhenGotEntity;
            _onTimeout = onTimeout;

            DbgLog("0");

            StartWaitingEntity(timeout);
        }

        /// <summary>
        /// Все решения принимаются путём работы с _tcs и/или _cts (путём вызова их методов: .TrySetResult или .TryCancel, или ...)
        /// Всё же, что нужно сделать после принятия этих решений, делается в одном месте - тут после `WhenAny`. 
        /// </summary>
        private void /*ConnectInternal*/StartWaitingEntity(TimeSpan timeout)
        {
            AsyncUtils.RunAsyncTask(
                async () =>
                {
                    DbgLog("1");

                    _tcs = new TaskCompletionSource<bool>();
                    _cts = new CancellationTokenSource();

                    DbgLog("2");

                    await CheckEntityReplication(_cts.Token);

                    DbgLog($"3 _tcs?:{_tcs!=null}, _cts?:{_cts!=null}.");

                    var task = await Task.WhenAny(_tcs.Task, Task.Delay(timeout, _cts.Token));

                    DbgLog($"4 WhenAny done : t:{task.IsCompleted}({task.Status}){task.IsCanceled}/{task.IsFaulted} _t:{_tcs.Task.IsCompleted}({_tcs.Task.Status}){_tcs.Task.IsCanceled}/{_tcs.Task.IsFaulted}. _onTimeout!=Null:{_onTimeout}");

                    if (task.Status == TaskStatus.RanToCompletion)
                    { 
                        if (task == _tcs.Task)
                        { // Entity is ready:
                            _doWhenGotEntity?.Invoke();
                        }
                        else
                        { // Timeout expired:
                            //DbgLog("5");
                            if (EntityWaiterLogger.Dbg) if (DbgLog.Enabled) DbgLog./*LogErr*/Log(15200, $"15200::: {DateTime.UtcNow} ##DBG:  StartWaitingEntity(timeout:{timeout}). WhenAny : t:{task.IsCompleted}({task.Status}){task.IsCanceled}/{task.IsFaulted} _t:{_tcs.Task.IsCompleted}({_tcs.Task.Status}){_tcs.Task.IsCanceled}/{_tcs.Task.IsFaulted}. _onTimeout!=Null:{_onTimeout}");
                            //DbgLog("6");
                            _onTimeout?.Invoke();
                            //DbgLog("7");
                        }
                    }
                    Cancel();
                    DisposeAll();

                    DbgLog("8");
                });
        }

        private async Task CheckEntityReplication(CancellationToken cancellationToken)
        {
            DbgLog("2.1");

            try
            {
                //DbgLog.Log("2.2");
                cancellationToken.ThrowIfCancellationRequested();
                //DbgLog.Log("2.3");
                using (var wrapper = await _repository.Get(_typeId, _guid))
                {
                    //DbgLog("2.4");
                    cancellationToken.ThrowIfCancellationRequested();
                    //DbgLog.Log("2.5");
                    var entity = wrapper.Get<TEntity>(_typeId, _guid, _replicationLevel);
                    if (entity == null)
                    {
                        //DbgLog("2.6.a");
                        //Wait Replication
                        _repository.NewEntityUploaded += OnEntitiesRepositoryEntityUpdated;
                        DbgLog("2.6.a.2");
                    }
                    else
                    {
                        //DbgLog("2.6.b");
                        CallDoWhenGotEntity();
                        DbgLog("2.6.b.2");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                EwLogger.IfError()?.Message($"Check Entity Replication Cancelled {_typeId} {_guid}").Write();
            }
        }

        private void CallDoWhenGotEntity()
        {
            DbgLog("26b.1");

            if (_isCanceled)
            {
                EwLogger.IfError()?.Message("_isCanceled==true, but `CallDoWhenGotEntity` is called").Write();
                return;
            }

            DbgLog($"26b.2  _tcs?:{_tcs!=null},  _cts?:{_cts!=null}");

            if (!_tcs.TrySetResult(true))
                EwLogger.IfWarn()?.Message("_tcs.TrySetResult(true) returned false. [ Is it Ok? ]").Write();

            DbgLog("26b.3");
        }

        private async Task OnEntitiesRepositoryEntityUpdated(int typeId, Guid guid)
        {
            if (_typeId == typeId && _guid.Equals(guid))
            {
                DbgLog("::: OnEntitiesRepositoryEntityUpdated");
                _repository.NewEntityUploaded -= OnEntitiesRepositoryEntityUpdated;
                // Now we should come into "else" branch
                await CheckEntityReplication(_cts.Token);
            }
        }

        private bool _isCanceled;
        /// <summary>
        /// This method is called from manually cancel all tasks of an instance of `EnttyWaiter`, & unconditionally WhenAny of tasks timeout || successfully got entty, which it was waiting for:
        /// "if (_isCanceled)" does not guarant, we don't go-in twice, but it's not a problem to go-in twice, so I decided, `CompareExchange` is excessively here.
        /// </summary>
        /// <returns> `false` means it was already canceled </returns>
        public bool Cancel()
        {
            DbgLog("Cancel");

            if (_isCanceled)
                return false;

            //DbgLog("Cancel.1");

            if (_cts != null)
                _cts.Cancel();

            //DbgLog("Cancel.2");

            if (_tcs != null)
            {
                var tmp = _tcs.TrySetCanceled();
                DbgLog("_tcs.TrySetCanceled() == " + tmp);
            }

            _isCanceled = true;

            DbgLog("Cancel.3");

            return true;
        }

        public bool DisposeAll()
        {
            DbgLog("DisposeAll.1");

            if (_cts != null)
            {
                _cts.Dispose();
                _cts = null;
            }

            DbgLog("DisposeAll.2");

            _tcs = null;

            if (_repository != null)
            {
                _repository.NewEntityUploaded -= OnEntitiesRepositoryEntityUpdated;
                _repository = null;
            }

            _onTimeout = null;
            _doWhenGotEntity = null;

            DbgLog("DisposeAll.3");

            return true;
        }
    }
}
