using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class TouchableEgoProxy<T> : ITouchable<T> where T : class, IEntity
    {
        public event Action<Exception> Diagnostic;
        public bool LogEverything = false;
        private Exception DiagnosticInternal(Exception e, string logMsg = "")
        {
            ReactiveLogs.Logger.IfError()?.Message($"Exception: {logMsg}\n{e.Message}\n{e.StackTrace}").Write();
            var invokeList = Diagnostic;
            Diagnostic = null;
            invokeList?.Invoke(e);
            return e;
        }

        private Touchable.Proxy<T> _proxy;
        /// <summary> Передать начальное состояние если присоединяется новый слушатель. Состояние передаётся в виде нового контейнера. </summary>
        private void OnSubscribeHandler(ToucherSubscriptionAgent<T> newAgent)
        {
            if (!_disposeWrapper.StartWorker())
            {
                newAgent.Dispose(); // Возможно никогда не пригодится потому, что внутри public IDisposable Subscribe(IToucher<T> toucher) мы гарантируем, что подписка не может быть вызванная для уничтоженной прокси.
                return;
            }

            Task newBlockerTask = null;
            lock (_entityRequestLock)
            {
                if (_lastConnectContainer != null)
                {
                    // Connect
                    var closuredContainer = _containerFactory.GetNewContainer(_lastConnectContainer);
                    newAgent.ReceiveAddContainerDirect(closuredContainer);

                    Task newChainTask;
                    (newBlockerTask, newChainTask) =
                        GetTaskWithBlocker(_lastChainTask, async () => {
                            await closuredContainer.Connect();
                        });
                    _lastChainTask = newChainTask;
                }
            }

            _disposeWrapper.FinishWorker();

            newBlockerTask?.Start();
        }

        //private RootSubscriptionAgent<T> _root;

        /// <summary> Лок данных последней Entity которую мы добавили в очередь на подключение. </summary>
        private Object _entityRequestLock = new object();

        //--- Флаги состояния
        private EntityTouchContainer<T> _lastConnectContainer;

//        /// <summary> Получено ли entity в рамках Connect или попытка это сделать бесславно оборвалась.
//        /// Актуально только когда Task на Connect закончился. </summary>
//        private bool _entityIsSucessfullyConnected;

        private Task _lastChainTask;

        /// <summary> Отладочная информация </summary>
        private static int _idCounter;

        private IEntityTouchContainerFactory<T> _containerFactory;

        // Всё что нужно для потокобезопасного диспоуза
        private ThreadSafeDisposeWrapper _disposeWrapper;

        //=== Props ===========================================================

        public bool IsDisposed => _disposeWrapper.IsDisposed;

        public int Id { get; }


        //=== Ctor ============================================================

        public TouchableEgoProxy(IEntityTouchContainerFactory<T> containerFactory)
        {
            Id = ++_idCounter;
            _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
            _proxy = new Touchable.Proxy<T>(DeepLog, OnSubscribeHandler);
            _containerFactory = containerFactory;
        }

        ~TouchableEgoProxy()
        {
            if (IsDisposed)
                return;

            //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
        }

        //=== Public ==========================================================

        public void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }

        public static void ResetIdCounter()
        {
            _idCounter = 0;
        }

        public (Task blocker, Task chain) GetTaskWithBlocker(Task prev, Func<Task> next)
        {
            var blocker = new Task(() => { });
            var chain = Chain(prev, blocker, next);
            return (blocker, chain);
        }

        public void Connect(IEntitiesRepository repo, int typeId, Guid entityId, ReplicationLevel replicationLevel)
        {
            if (LogEverything)
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(Connect)}({repo}, {typeId}, {entityId}, {replicationLevel})");
            if (repo == null)
            {
                ReactiveLogs.Logger.IfError()?.Message($"No repo -- {this}").Write();
                return;
                //throw new Exception($"No repo -- {this}");
            }

            if (!_disposeWrapper.StartWorker())
                return;

            Task newBlockerTask;
            lock (_entityRequestLock)
            {
                if (_lastConnectContainer != null)
                {
                    Disconnect();
                }

                var closuredContainer = _containerFactory.GetNewContainer(repo, typeId, entityId, replicationLevel);
                _lastConnectContainer = closuredContainer;
                _proxy.ReceiveAddContainerChain(closuredContainer);
                
                repo.SubscribeOnDestroyOrUnload(typeId, entityId, OnConnectedEntityDestroyOrUnload);

                Task newChainTask;
                (newBlockerTask, newChainTask) = GetTaskWithBlocker(_lastChainTask, async () => await closuredContainer.Connect());
                _lastChainTask = newChainTask;
            }

            //Кто-то внутри может синхронно выполниться, вызвать Exception и не дать снять worker. Чтобы этого избежать Контейнер закрывает все свои внешние вызовы try-ем.
            _disposeWrapper.FinishWorker(); // Пока вот это не закончится, реального диспоуза не будет, даже если его запросили в параллельном потоке.
            newBlockerTask.Start();
        }

        public IDisposable Subscribe(IToucher<T> toucher)
        {
            if (!_disposeWrapper.StartWorker())
            {
                toucher.OnCompleted();
                return EmptySubscriptionAgent<T>.Instance;
            }

            var agent = _proxy.Subscribe(toucher);

            _disposeWrapper.FinishWorker();
            return agent;
        }

        public void Disconnect()
        {
            if (LogEverything)
                Console.WriteLine($"{GetType().Name}.{nameof(Disconnect)}()" + (_lastConnectContainer != null ? $"// {_lastConnectContainer.EntityId}" : ""));
            if (!_disposeWrapper.StartWorker())
                return;

            Task newBlockerTask = null;
            lock (_entityRequestLock)
            {
                if (_lastConnectContainer != null)
                {
                    _lastConnectContainer.Repo.UnsubscribeOnDestroyOrUnload(
                        _lastConnectContainer.TypeId, _lastConnectContainer.EntityId, OnConnectedEntityDestroyOrUnload); 
                    // Кейсом, что в неразмотанной очереди есть пара ConnectDisconnect и вдруг этой пары entity уничтожается, а мы уже здесь отписались,
                    // Не проблема, потому что в этом случае враппер будет возвращать null. Нас интересует только сдыхание той Entity, которая прямо сейчас подцеплена.
                    
                    var connectContainer = _lastConnectContainer;
                    var disconnectContainer = _containerFactory.GetNewContainer(_lastConnectContainer);
                    _proxy.ReceiveRemoveContainerChain(disconnectContainer);

                    async Task NextDisconnectTask()
                    {
                        if (connectContainer.WasSucessful)
                            await disconnectContainer.Connect();
                    }
                    _lastConnectContainer = null;
                    Task newChainTask;
                    (newBlockerTask, newChainTask) = GetTaskWithBlocker(_lastChainTask, NextDisconnectTask);
                    _lastChainTask = newChainTask;
                }
            }
            _disposeWrapper.FinishWorker(); //Кто-то внутри может синхронно выполниться, вызвать Exception и не дать снять worker. Чтобы этого избежать Контейнер закрывает все свои внешние вызовы try-ем.
            newBlockerTask?.Start();
        }

        private Task OnConnectedEntityDestroyOrUnload(int typeId, Guid guid, IEntity entityRef)
        {
            //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.ffff}] {GetType().Name}.{nameof(Disconnect)}(guid:{(_lastConnectContainer != null ? _lastConnectContainer.EntityId.ToString() : "-")}) ");
            if (!_disposeWrapper.StartWorker())
                return Task.CompletedTask;

            Task newBlockerTask = null;
            lock (_entityRequestLock)
            {
                // _repo.UnsubscribeOnDestroyOrUnload(typeId, guid, OnConnectedEntityDestroyOrUnload);
                // Мы вообще не отписываемся от этой функции, потому что Боря сказал, что еслу уж Entity сдохла, то сделала она это вместе со ссылкой на нас.
                // Вероятность коллизии EntityId в другом репозитории исчезающе мала, так что на то чтобы сверять ещё и Repo забиваем
                if (_lastConnectContainer != null && _lastConnectContainer.TypeId == typeId && _lastConnectContainer.EntityId == guid) 
                {
                    // Если мы сюда попали, значит Entity сдохло позорной смертью либо в до коннекта к нему либо после, не исключён вариант, что и во время.
                    // Значит нужно поставить в общую очередь задачку на холостой отконнект всех слушателей от этой Entity.

                    var connectContainer = _lastConnectContainer;
                    var disconnectContainer = new SyncTouchContainer<T>(null);

                    _proxy.ReceiveRemoveContainerChain(disconnectContainer);

                    async Task NextDisconnectTask()
                    {
                        if (connectContainer.WasSucessful)
                            await disconnectContainer.Connect();
                    }

                    _lastConnectContainer = null;
                    Task newChainTask;
                    (newBlockerTask, newChainTask) = GetTaskWithBlocker(_lastChainTask, NextDisconnectTask);
                    _lastChainTask = newChainTask;
                }
            }

            _disposeWrapper.FinishWorker();

            newBlockerTask?.Start();

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"({GetType().NiceName()} id={Id} ConnectContainer{(_lastConnectContainer != null).AsSign()}, " +
                   $"{nameof(IsDisposed)}{IsDisposed.AsSign()})";
        }

        public string DeepLog(string prefix)
        {
            var container = _lastConnectContainer;
            return $"{prefix}{GetType().NiceName()} {(container != null ? $"CONNECTED: type={container.TypeId}, Id={container.EntityId}, replicationLevel={container.ReplicationLevel}" : "DISCONNECTED")}";
        }

        //=== Private =========================================================
        private async Task Chain(Task prevTask, Task blocker, Func<Task> nextTaskFactory)
        {
            try
            {
                if (prevTask != null)
                    await prevTask;
            }
            catch (Exception e)
            {
                DiagnosticInternal(e);
            }

            await blocker;

            try
            {
                await nextTaskFactory();
            }
            catch (Exception e)
            {
                DiagnosticInternal(e);
            }
        }

        private void DisposeInternal()
        {
            _proxy.Dispose(); // А потом прошёл Disconnect, который отклонили воркеры да и пофиг, потому что _proxy уже в любом случае того.
            if (_lastConnectContainer != null) // Отписываемся от чтобы избавиться от ссылки на этот экземпляр снаружи. Сама по себе операция не будет выполнена, потому что все операции в OnConnectedEntityDestroyOrUnload отклонят воркеры
                _lastConnectContainer.Repo.UnsubscribeOnDestroyOrUnload(_lastConnectContainer.TypeId, _lastConnectContainer.EntityId, OnConnectedEntityDestroyOrUnload);
            GC.SuppressFinalize(this);
        }
    }
}