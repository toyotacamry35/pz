using System;
using System.Diagnostics;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using ReactiveProps;

namespace ReactivePropsNs
{
    public class SubscriptionAgent<T> : ISubscription<T>, IPooledObject
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Pool<SubscriptionAgent<T>> _Pool = Pool.Create(() => new SubscriptionAgent<T>(), PoolSizes.GetPoolMaxCapacityForType<SubscriptionAgent<T>>(5000));

        private ISubscriptionAgentOwner _owner;
        private IListener<T> _listener;
        private Func<string, string> _deepLog;
        private bool _isDisposed;
        private int _id;

        public SubscriptionAgent<T> Previous;

        /// <summary>
        /// Конвенция: Ссылка на Next не должна чиститься при уничтожении этого элемента,
        /// чтобы избежать обрыва вызова onNext по цепочке если элемент при обработке этого вызова застрелился
        /// </summary>
        public SubscriptionAgent<T> Next;

        public bool IsDisposed => _isDisposed;

        public int Id => _id; //DEBUG

        public static SubscriptionAgent<T> Create(ISubscriptionAgentOwner owner, IListener<T> listener)
        {
            var agent = _Pool.Acquire();
            agent._listener = listener;
            agent._owner = owner;
            agent._isDisposed = default;
            agent._id = default;
            agent.Previous = default;
            agent.Next = default;
            return agent;
        }

        public static void Release(SubscriptionAgent<T> agent)
        {
            _Pool.Release(agent);
        }

        private int _cycleInterupter = 0;

        public void OnNext(T value)
        {
            if (!IsDisposed)
            {
                ++_cycleInterupter;
                if (_cycleInterupter >=3) { // Сделал усложнение чтобы незацикливающуюся рекурсию не отсекать. Вообще это конечно жопа - какие вещи в коде оставлять.
                    ReactiveLogs.Logger.IfError()?.Message($"!!!!! !!!!! Dead Loop Interupter EXCPTION !!!!! !!!!!\n\n{new StackTrace(true)}\n\n").Write();
                    return;
                }
                try
                {
                    _listener.OnNext(value);
                }
                catch (Exception e)
                {
                    ReactiveLogs.Logger.IfError()?.Message(e,
                        $"EXCEPTION!!! IStream<{typeof(T).NiceName()}>.OnNext({value}) EXCEPTION MESSAGE: {e.Message}\n" +
                        ///!!!#DANGER: switch this log line with stack on causes random crashes: - DON'T PUSH IT!!!:  $"{nameof(e.StackTrace)} {e.StackTrace}\n" +
                        $"{Log("EXCEPTION STREAM SOURCE:\t")}").Write();
                    //HandleOnNextException(value, e);
                }
                finally
                {
                    _cycleInterupter--;
                }
            }
        }

        public void OnCompleted()
        {
            Dispose();
        }

        public void SetRequestLogHandler(Func<string, string> handler)
        {
            _deepLog = handler;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            _isDisposed = true;
            _owner.Unsubscribe(this);
            _owner = null;
            _listener.SetRequestLogHandler(null);
            try
            {
                _listener.OnCompleted();
            }
            catch (Exception e)
            {
                ReactiveLogs.Logger.IfError()?.Message(e, $"<{typeof(T).NiceName()}> OnCompleted() exception: {e.Message}\n{e.StackTrace}").Write();
            }

            _listener = null;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        private async void HandleOnNextException(T value, Exception e)
        {
            ReactiveLogs.Logger.Error(e,
                $"REPEATED EXCEPTION LOG AFTER 1sec !!! IStream<{typeof(T).NiceName()}>.OnNext({value}) EXCEPTION MESSAGE: {e.Message}\n{Log("EXCEPTION STREAM SOURCE:\t")}");
            try
            {
                ReactiveLogs.Logger.IfError()?.Message($"EXCEPTION STACKTRACE:\n{e.StackTrace}").Write();
            }
            catch (Exception e2)
            {
                ReactiveLogs.Logger.IfError()?.Message(e2, $"CRASH ON STACKTRACE CREATION:\n{e2.Message}").Write();
            }
        }

        private string Log(string prefix) => _deepLog != null ? _deepLog.Invoke(prefix) : _owner.DeepLog(prefix);

        private SubscriptionAgent() { }

        bool IPooledObject.Released { get; set; }

        public interface ISubscriptionAgentOwner
        {
            void Unsubscribe(SubscriptionAgent<T> agent);
            string DeepLog(string prefix);
        }
    }
}