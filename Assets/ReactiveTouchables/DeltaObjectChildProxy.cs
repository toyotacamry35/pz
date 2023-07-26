using System;
using SharedCode.EntitySystem;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.Serializers;

namespace ReactivePropsNs.Touchables
{
    /// <summary>
    /// Вспомогательный класс, который содержит в себе все типовые операции, производящиеся ITouchable<TDelta> для того чтобы передать вызовы дальше.
    /// Содержит функционал отложенного подписывания для тех, кто опоздал сделать этого синхронно
    /// </summary>
    public class DeltaObjectChildProxy<TDelta> : ITouchable<TDelta>, IToucher<TDelta> where TDelta : IDeltaObject
    {
        private const bool CREATE_LOG_WARNING = false;

        private static EqualityComparer<TDelta> _equalityComparer = EqualityComparer<TDelta>.Default;

        private ThreadSafeDisposeWrapper _disposeWrapper;
        private Touchable.Proxy<TDelta> _proxy;
        /// <summary> Чтобы параллелизмы, могущие взникнуть при подписке не ломали состояния наличия _cachedChild </summary>
        private object _childLock = new object();
        /// <summary>  </summary>
        private TDelta _cachedChild = default;
        /// <summary> Признак того что entity из которой мы получаем данный DeltaObject находится на read локе, значит мы можем делать с ним всякое. </summary>
        private volatile bool _parentEntityLockedIntoChildThread = false;
        private Action<Func<Task>, IEntitiesRepository> _asyncTaskRunner;

        private string _stackTrace = null;
        private Func<string, string> _createLog;
        private Func<string, string> _reciveStreamLog;

        public DeltaObjectChildProxy(Action<Func<Task>, IEntitiesRepository> asyncTaskRunner, Func<string, string> createLog)
        {
            _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
            _proxy = new Touchable.Proxy<TDelta>(DeepLog);
            _asyncTaskRunner = asyncTaskRunner;
            if (createLog == null)
            {
                if (CREATE_LOG_WARNING)
                {
                    #pragma warning disable CS0162 // Unreachable code detected
                    _stackTrace = Tools.StackTraceLastString();
                    ReactiveLogs.Logger.IfWarn()?.Message($"Не создан формирователь лога для {GetType().Name} по адресу: \n {_stackTrace}").Write();
                    #pragma warning restore CS0162 // Unreachable code detected
                }
                _createLog = prefix => $"{prefix}{GetType().FullName}<{typeof(TDelta).Name}>{(_stackTrace != null ? $" ({_stackTrace})" : "")}" + (_reciveStreamLog != null ? '\n' + _reciveStreamLog(prefix + '\t') : "");
            }
            else
                _createLog = createLog;
        }

        public void OnAdd(TDelta deltaObject)
        {
            lock (_childLock) // Типа если дочерний элемет получен, то он и разослан, чтобы между этими действиями никто не вклинился.
            {
                if (deltaObject == null || _cachedChild != null)
                    throw new Exception($"{GetType().Name}.{nameof(OnAdd)}({deltaObject}) // Но мы сейчас в несовместимом состоянии: {_cachedChild}");
                _parentEntityLockedIntoChildThread = true;
                _cachedChild = deltaObject;
                try
                {
                    if (_cachedChild != null)
                    {
                        var container = new SyncTouchContainer<TDelta>(_cachedChild);
                        _proxy.ReceiveAddContainerChain(container);
                        container.Connect(); // Сигнал прокатится по цепочке синхронно внутри потока. Это произойдёт внутри ChildLock
                    }
                }
                catch (Exception e)
                {
                    ReactiveLogs.Logger.IfError()?.Message(e, "Обвалились при попытке передать новый элемент дальше по цепочке OnAdd.").Write();;
                }
                finally
                {
                    _parentEntityLockedIntoChildThread = false;
                }
            }
        }

        public void OnRemove(TDelta deltaObject)
        {
            lock (_childLock) // Типа если дочерний элемет получен, то он и разослан, чтобы между этими действиями никто не вклинился.
            {
                if (deltaObject != null && !_equalityComparer.Equals(deltaObject, _cachedChild))
                    throw new Exception($"{GetType().Name}.{nameof(OnRemove)}({deltaObject}) // Но мы сейчас подключены к другому объекту: {_cachedChild}");
                _parentEntityLockedIntoChildThread = true;
                try
                {
                    _cachedChild = default;
                    var container = new SyncTouchContainer<TDelta>(deltaObject);
                    _proxy.ReceiveRemoveContainerChain(container);
                    container.Connect(); // Сигнал прокатится по цепочке синхронно внутри потока.
                }
                catch (Exception e)
                {
                    ReactiveLogs.Logger.IfError()?.Message(e, "Обвалились при попытке передать новый элемент дальше по цепочке OnRemove.").Write();;
                }
                finally
                {
                    _parentEntityLockedIntoChildThread = false;
                }
                SubscribeSuspendedTouchers(); // Имеем право, потому что если мы попали сюда, значит ждать больше ничего...
            }
        }

        public IDisposable Subscribe(IToucher<TDelta> toucher)
        {
            //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(Subscribe)}({toucher}) // {nameof(_parentEntityLockedIntoChildThread)}={_parentEntityLockedIntoChildThread})");

            if (!_disposeWrapper.StartWorker())
            {
                toucher.OnCompleted();
                return EmptySubscriptionAgent<TDelta>.Instance;
            }

            // Зависит от состояния. Если мы уже подсоеденены, то откладывать новые toucher на попозже.
            TDelta suspendedConnectionRequired = default;
            IDisposable subscribtion = null;
            // Когда удастся поймать родительскую Entity на readLock тогда более нормально это сделаем.
            lock (_childLock)
            {
                if (_cachedChild != null)
                {
                    toucher.SetRequestLogHandler(_createLog);
                    if (_parentEntityLockedIntoChildThread)
                    {
                        // Если родительская Entity у нас на локе, то можно ей сразу child и отдать. Новый подписчик будет добавлен в начало цепочки и многократного сообщения не получит.
                        var subscription = _proxy.Subscribe(toucher);
                        var addContainer = new SyncTouchContainer<TDelta>(_cachedChild);
                        subscription.ReceiveAddContainerDirect(addContainer);
                        addContainer.Connect(); // Сигнал прокатится по цепочке синхронно внутри потока.
                        subscribtion = subscription;
                    }
                    else
                    {
                        // Если мы не на локе, значит подписка пришла к нам из какого-то совсем левого треда, и надо откладывать подписку до момента когда мы parentEntity поймаем на лок.
                        var subscribtionWrapper = new SuspendedDisposable();
                        suspendedTouchers.Enqueue(new Tuple<SuspendedDisposable, IToucher<TDelta>>(subscribtionWrapper, toucher));
                        if (!_suspendedTouchersConnectionAlreadyCreated)
                        {
                            suspendedConnectionRequired = _cachedChild;
                            _suspendedTouchersConnectionAlreadyCreated = true;
                        }
                        subscribtion = subscribtionWrapper;
                    }
                }
                else
                {
                    subscribtion = _proxy.Subscribe(toucher);
                }
            }

            if (!EqualityComparer<TDelta>.Default.Equals(suspendedConnectionRequired, default))
                _asyncTaskRunner(async () => await SuspendedTouchersRequireConnection(suspendedConnectionRequired),
                    suspendedConnectionRequired.EntitiesRepository);

            _disposeWrapper.FinishWorker();

            return subscribtion;
        }
        /// <summary>
        /// У этой переменной сложное значение. С одной стороны значение false означает, что для отложенных подписчиков пока не был запущен таск, который их отпроцессит.
        /// С другой стороны если будет произведён, например дисконнект, то все имеющиеся на этот момент подписчики будут сразу подписанны, и переменная сброшена,
        /// даже если таск, который должен их обработать ещё не сработал. Таким образом для следующих припозднившехся подписчиков она опять начнётся из состояния false,
        /// причём, возможно, с тем же самым значением переменной в _cachedChild, тоесть ничто не мешает иметь больше одного запущенного таска по отложенному подключению, и даже, возможно, с одинаковыми параметрами.
        /// </summary>
        private bool _suspendedTouchersConnectionAlreadyCreated = false;
        /// <summary> Список отложенных подписчиков, которые получат своё подключение попозже. </summary>
        private Queue<Tuple<SuspendedDisposable, IToucher<TDelta>>> suspendedTouchers = new Queue<Tuple<SuspendedDisposable, IToucher<TDelta>>>();
        private async Task SuspendedTouchersRequireConnection(TDelta child)
        {
            //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(SubscribeSuspendedTouchers)}({child}) // {nameof(suspendedTouchers)}.{nameof(suspendedTouchers.Count)}={suspendedTouchers.Count})");

            IEntitiesRepository Repo = child.EntitiesRepository;
            if (Repo == null)
                return;
            var (entityTypeId, entityId) = (_cachedChild.ParentTypeId, _cachedChild.ParentEntityId);
            // Нужно поймать на lock Entity
            using (var wrapper = await Repo.Get(entityTypeId, entityId))
            {
                if (wrapper != null && wrapper.IsEntityExist(entityTypeId, entityId)) // TODO проверить что в других местах когда 
                {
                    lock (_childLock)
                    {
                        if (!child.Equals(_cachedChild))
                            return;
                        if (suspendedTouchers.Count == 0)
                            return;
                        _parentEntityLockedIntoChildThread = true;
                        // Потом предать _cachedChild выбранному агенту.
                        var addSyncContainer = new SyncTouchContainer<TDelta>(child);
                        while (suspendedTouchers.Count > 0)
                        {
                            var suspended = suspendedTouchers.Dequeue();
                            var agent = _proxy.Subscribe(suspended.Item2);
                            agent.ReceiveAddContainerDirect(addSyncContainer); // try finaly предусмотренны внутри контейнера.
                            suspended.Item1.SetDisposable(agent);
                        }

                        var task = addSyncContainer.Connect();
                        // Новые подписчики, которые могут быть созданы в процессе рассылки этих сообщений получат подключение сразу и без нас, потому что мы на read локе.
                        _parentEntityLockedIntoChildThread = false;
                        _suspendedTouchersConnectionAlreadyCreated = false;
                    }
                }
                else
                {
                    lock (_childLock)
                    {
                        _suspendedTouchersConnectionAlreadyCreated = false; // Энтити сдохла, хвост облез.
                        // SubscribeSuspendedTouchers() // Ну по логике так делать надо же, не?
                        // С другой стороны то что сдохла эта entity не факт, что мы не подключились уже к какой-то другой. А SubscribeSuspendedTouchers мы сделаем после того как родительский класс поймает событие об издыхании entity
                    }
                }
            }
        }
        /// <summary> Просто почистим список ожидающих Toucher-ов, потому что ждать им больше нечего. </summary>
        private void SubscribeSuspendedTouchers()
        {
            _suspendedTouchersConnectionAlreadyCreated = false;
            //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(SubscribeSuspendedTouchers)}() // {nameof(suspendedTouchers)}.{nameof(suspendedTouchers.Count)}={suspendedTouchers.Count})");
            while (suspendedTouchers.Count > 0) // Это всегда делается внутри lock (_childLock) и это сохраняет целостность коллекции
            {
                var suspended = suspendedTouchers.Dequeue();
                suspended.Item1.SetDisposable(_proxy.Subscribe(suspended.Item2));
            }
        }

        public string StateForLog()
        {
            if (_disposeWrapper.IsDisposed)
                return "DISPOSED";
            lock (_childLock)
                 return _cachedChild == null ? "DISCONNECTED" : _cachedChild.ToString();
        }

        public void SetRequestLogHandler(Func<string, string> handler)
        {
            _reciveStreamLog = handler;
        }

        public string DeepLog(string prefix)
        {
            return _createLog(prefix);
        }

        public void OnCompleted()
        {
            Dispose();
        }
        public bool IsDisposed => _disposeWrapper.IsDisposed;
        public void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }
        private void DisposeInternal()
        {
            _cachedChild = default;
            _asyncTaskRunner = null;
            _proxy.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
