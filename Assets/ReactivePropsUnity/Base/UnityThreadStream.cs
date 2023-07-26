using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Environment.Logging.Extension;
using ReactivePropsNs.Touchables;

namespace ReactivePropsNs
{
    /// <summary>
    /// Посредник ловит события изо всяких разных трэдов, и не нарушая порядок перекладывает в UnityThread и заодно запоминает последнее переданое значение
    /// </summary>
    public class UnityThreadStream<T> : IListener<T>, IStream<T>
    {
        private static readonly bool IsDebug = false;
        private readonly string _name;
        private ThreadSafeDisposeWrapper _disposeWrapper;
        private ReactiveProperty<T> _output;
        private string _stackTrace;
        private Func<string, string> _createLog;

        public UnityThreadStream(Func<string, string> createLog)
        {
            _name = $"{GetType().NiceName()} [{UniqueId.Id++}] {(IsDebug ? new StackTrace().ToString() : "")}";
            _disposeWrapper = new ThreadSafeDisposeWrapper(InternalDispose, this);
            if (createLog == null)
            {
                _stackTrace = Tools.StackTraceLastString();
                ReactiveLogs.Logger.IfWarn()?.Message($"Не создан формирователь лога для {GetType().NiceName()} по адресу: \n {_stackTrace}").Write();
                _createLog = prefix => $"{prefix} {GetType().NiceName()} ({_stackTrace})";
            }
            else
                _createLog = createLog;
            _output = PooledReactiveProperty<T>.Create(DeepLog);
        }

        private Queue<T> _recivedValues = new Queue<T>();
        private object _recivedValuesQueueLock = new object();
        private bool _completed = false;
        private bool _processing = false;


        //=== Props ===========================================================

        public bool IsDisposed => _disposeWrapper.IsDisposed;



        //=== Public ==========================================================

        public override string ToString()
        {
            return IsDisposed ? $"{_name} DISPOSED" :_name;
        }

        public IDisposable Subscribe(IListener<T> listener)
        {
            return _output.Subscribe(listener);
        }

        public void Dispose()
        {
            _disposeWrapper.RequestDispose();
        }

        public void OnNext(T value)
        {
            if (!_disposeWrapper.StartWorker())
                return;
            bool requestProcessing = false;
            lock (_recivedValuesQueueLock)
            {
                _recivedValues.Enqueue(value);
                requestProcessing = !_processing;
                _processing = true;
            }

            // TODO тут наверное также нужен try finaly
            if (requestProcessing)
                UnityQueueHelper.RunInUnityThreadNoWait(ProcessingIntoUnityThread);
            _disposeWrapper.FinishWorker();
        }

        public void OnCompleted()
        {
            if (!_disposeWrapper.StartWorker())
                return;
            bool requestProcessing = false;
            lock (_recivedValuesQueueLock)
            {
                _completed = true;
                requestProcessing = !_processing;
                _processing = true;
            }

            if (requestProcessing)
                UnityQueueHelper.RunInUnityThreadNoWait(ProcessingIntoUnityThread);
            _disposeWrapper.FinishWorker();
        }


        //=== Private =========================================================

        private void ProcessingIntoUnityThread()
        {
            if (!_disposeWrapper.StartWorker())
                return;
            do
            {
                T nextValue;
                lock (_recivedValuesQueueLock)
                {
                    if (_recivedValues.Count == 0)
                    {
                        if (_completed)
                            _disposeWrapper.RequestDispose();
                        _processing = false;
                        break;
                    }

                    // TODO Сделать аварийный выход и перевызов Processing() для перевызова необходимо извращение чтобы прервать трэд
                    nextValue = _recivedValues.Dequeue();
                }

                //TODO try finaly
                _output.Value = nextValue;
            } while (true);

            _disposeWrapper.FinishWorker();
        }


        private void InternalDispose()
        {
            _output.Dispose();
        }

        public string DeepLog(string prefix)
        {
            return _createLog != null ? _createLog.Invoke(prefix) : $"{prefix}{GetType().NiceName()} {(IsDisposed ? "DISPOSED" : "UNDEFINED")}";
        }
        public void SetRequestLogHandler(Func<string, string> handler) => _createLog = handler;
    }
}