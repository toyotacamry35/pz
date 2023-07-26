using System;
using System.Threading;
using SharedCode.EntitySystem;

namespace ReactivePropsNs.Touchables
{
    public class ToucherSubscriptionAgent<T> : IIsDisposed where T : IDeltaObject
    {
        protected ToucherSubscriptionAgent<T> PreviousInChain;

        /// <summary>
        /// Конвенция: При Dispose мы не очищаем NextInChain потому что хотим чтобы волна дошла до конца по цепочке.
        /// При этом вызов уже удалённых не происходит, потому что они имеют флаг IsDisposed и не обработают Direct вызов
        /// Вызов добавленных не происходит, потому что мы по конвенции обязуемся добавлять только в начало Chain-а, и обрабатывать эту ситуацию отдельно
        /// </summary>
        protected ToucherSubscriptionAgent<T> NextInChain;

        private Action<T> _onAdd;
        private Action<T> _onRemove;
        private Action _onCompleted;

        /// <summary>
        /// Lock-объект операций с цепочкой из владельца начала цепочки
        /// Не очищаем в Dispose(), т.к. нужна при передаче через Chain-методы даже после Dispose()
        /// </summary>
        private object _chainLock;

        // Всё что нужно для потокобезопасного диспоуза
        protected ThreadSafeDisposeWrapper _disposeWrapper;

        //=== Props ===========================================================

        public bool IsDisposed => _disposeWrapper.IsDisposed;

        public int Id { get; private set; } //DEBUG


        //=== Ctor ============================================================
        private static int _nextFreeId = 0;
        public static void ResetId()
        {
            _nextFreeId = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainLock">Объект блокирования операций изменения цепочки агентов</param>
        /// <param name="onAdd">Событие добавления Entity</param>
        /// <param name="onRemove">Событие удаления Entity</param>
        /// <param name="onCompleted"></param>
        public ToucherSubscriptionAgent(object chainLock, Action<T> onAdd = null, Action<T> onRemove = null, Action onCompleted = null)
        {
            Id = Interlocked.Increment(ref _nextFreeId);
            _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
            _chainLock = chainLock;
            _onAdd = onAdd ?? NullOnAdd;
            _onRemove = onRemove ?? NullOnRemove;
            _onCompleted = onCompleted ?? NullOnCompleted;
        }


        //=== Public ==========================================================

        /// <summary>
        /// Вставляемcя в начало цепочки
        /// </summary>
        public void InsertAfter(ToucherSubscriptionAgent<T> previous)
        {
            lock (_chainLock)
            {
                PreviousInChain = previous;
                NextInChain = previous.NextInChain;

                PreviousInChain.NextInChain = this;
                NextInChain.PreviousInChain = this;
            }
        }

        public virtual void ReceiveAddContainerChain(ITouchContainer<T> container)
        {
            ReceiveAddContainerDirect(container);
            ToucherSubscriptionAgent<T> nextInCurrentChain;
            lock (_chainLock)
            {
                nextInCurrentChain = NextInChain;
            }

            nextInCurrentChain.ReceiveAddContainerChain(container);
        }

        public void ReceiveAddContainerDirect(ITouchContainer<T> container)
        {
            if (!_disposeWrapper.StartWorker())
                return;

            container.SetCallbacks(OnAddSuccess, OnAddFail);

            _disposeWrapper.FinishWorker();
        }

        public virtual void ReceiveRemoveContainerChain(ITouchContainer<T> container)
        {
            ReceiveRemoveContainerDirect(container);

            ToucherSubscriptionAgent<T> nextInCurrentChain;
            lock (_chainLock)
            {
                nextInCurrentChain = NextInChain;
            }

            nextInCurrentChain.ReceiveRemoveContainerChain(container);
        }

        public void ReceiveRemoveContainerDirect(ITouchContainer<T> container)
        {
            if (!_disposeWrapper.StartWorker())
                return;

            container.SetCallbacks(OnRemoveSuccess, OnRemoveFail);

            _disposeWrapper.FinishWorker();
        }

        public virtual void DisposeChain()
        {
            Dispose();

            ToucherSubscriptionAgent<T> nextInCurrentChain;
            lock (_chainLock)
            {
                nextInCurrentChain = NextInChain;
            }

            nextInCurrentChain.DisposeChain();
        }

        public virtual void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }

        ~ToucherSubscriptionAgent()
        {
            if (IsDisposed)
                return;

            //ReactiveLogs.Logger.IfWarn()?.Message($"Non disposed {GetType().NiceName()} was finalized -- {this}").Write();
        }

        public override string ToString()
        {
            return $"({GetType().NiceName()} id={Id}{(IsDisposed ? " Disposed" : "")})";
        }


        //=== Private =========================================================

        public virtual void DisposeInternal()
        {
            //ReactiveLogs.Logger.IfDebug()?.Message($"DisposeInternal() @ {this}").Write();
            RemoveSelfFromChain();
            _onAdd = NullOnAdd;
            _onRemove = NullOnRemove;
            _onCompleted();
            _onCompleted = NullOnCompleted;
            GC.SuppressFinalize(this);
        }

        private void RemoveSelfFromChain()
        {
            lock (_chainLock)
            {
                PreviousInChain.NextInChain = NextInChain;
                NextInChain.PreviousInChain = PreviousInChain;
                PreviousInChain = null;
                //NextInChain = null; // Это чистить не планируем, см. конвенцию в описании переменной
            }
        }

        private void OnAddSuccess(T entity)
        {
            if (!_disposeWrapper.StartWorker())
                return;

            _onAdd(entity);

            _disposeWrapper.FinishWorker();
        }

        private void OnAddFail()
        {
        }

        private void OnRemoveSuccess(T entity)
        {
            if (!_disposeWrapper.StartWorker())
                return;

            _onRemove(entity);

            _disposeWrapper.FinishWorker();
        }

        private void OnRemoveFail()
        {
            if (!_disposeWrapper.StartWorker())
                return;

            _onRemove(default);

            _disposeWrapper.FinishWorker();
        }

        private static void NullOnAdd(T value)
        {
        }

        private static void NullOnRemove(T value)
        {
        }

        private static void NullOnCompleted()
        {
        }
    }
}