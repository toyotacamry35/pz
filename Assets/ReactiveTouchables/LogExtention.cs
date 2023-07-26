using System;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using SharedCode.Serializers;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;

namespace ReactivePropsNs.Touchables {
    public static class LogTouchableExtention {

        public static ITouchable<T> LogThreadPool<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor =  new Processor<T>(prefix, toString, logger, Processor<T>.LogLevel.Error, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
        public static ITouchable<T> LogInfoThreadPool<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new Processor<T>(prefix, toString, logger, Processor<T>.LogLevel.Info, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
        public static ITouchable<T> LogDebugThreadPool<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new Processor<T>(prefix, toString, logger, Processor<T>.LogLevel.Debug, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }
        public static ITouchable<T> LogTraceThreadPool<T>(this ITouchable<T> source, ICollection<IDisposable> disposables, string prefix, Func<T, string> toString = null, NLog.Logger logger = null) where T : IDeltaObject {
            var processor = new Processor<T>(prefix, toString, logger, Processor<T>.LogLevel.Trace, (taskFactory, repository) => AsyncUtils.RunAsyncTask(taskFactory, repository));
            disposables.Add(processor);
            var subscription = source.Subscribe(processor);
            disposables.Add(subscription);
            return processor;
        }

        private static int _nextFreeId = 0;
        public class Processor<T> : ITouchable<T>, IToucher<T> where T : IDeltaObject {
            private int _id;
            public enum LogLevel {Trace, Info, Debug, Error }
            private string _prefix;
            private Func<T, string> _toString = null;
            private NLog.Logger _logger = null;
            private LogLevel _logLevel;
            /// <param name="prefix">И так вроде понятно....</param>
            /// <param name="toString">Если ничего не указано используется встроенная </param>
            /// <param name="logger">В какой логгер кидать. Если ничего не написано используется ReactiveLogger</param>
            public Processor(string prefix, Func<T, string> toString, NLog.Logger logger, LogLevel logLevel, Action<Func<Task>, IEntitiesRepository> asyncTaskRunner) {
                _id = System.Threading.Interlocked.Increment(ref _nextFreeId);
                _disposeWrapper = new ThreadSafeDisposeWrapper(DisposeInternal, this);
                _proxy = new DeltaObjectChildProxy<T>(asyncTaskRunner, DeepLog);
                _logger = logger;
                _prefix = prefix;
                _toString = toString;
                _logLevel = logLevel;
            }
            #region IToucher<T>
            private void Log(string msg) {
                var logger = _logger ?? ReactiveLogs.Logger;
                msg = $"{_prefix}: {msg}";
                switch (_logLevel) {
                    case LogLevel.Trace: logger.IfTrace()?.Message(msg).Write(); break;
                    case LogLevel.Info: logger.IfInfo()?.Message(msg).Write(); break;
                    case LogLevel.Debug: logger.IfDebug()?.Message(msg).Write(); break;
                    case LogLevel.Error: logger.IfError()?.Message(msg).Write(); break;
                }
            }

            public void OnAdd(T deltaObject) {
                if (!_disposeWrapper.StartWorker())
                    return;

                Log($"OnAdd({(deltaObject == null ? "<null>" : _toString != null ? _toString(deltaObject) : deltaObject.ToString())})");
                _proxy.OnAdd(deltaObject);

                _disposeWrapper.FinishWorker();
            }
            public void OnRemove(T deltaObject) {
                if (!_disposeWrapper.StartWorker())
                    return;

                Log($"OnRemove({(deltaObject == null ? "<null>" : _toString != null ? _toString(deltaObject) : deltaObject.ToString())})");
                _proxy.OnAdd(deltaObject);
            }

            public void OnCompleted() {
                if (!_disposeWrapper.IsDisposed)
                    return;

                Log($"OnCompleted()");
                _disposeWrapper.RequestDispose();
            }

            private Func<string, string> _parentCreateLogHandler;
            public void SetRequestLogHandler(Func<string, string> handler) {
                if (!_disposeWrapper.IsDisposed)
                    return;
                _parentCreateLogHandler = handler;
            }
            #endregion
            #region ITouchable
            private DeltaObjectChildProxy<T> _proxy;
            public IDisposable Subscribe(IToucher<T> toucher) {
                return _proxy.Subscribe(toucher);
            }
            public string DeepLog(string prefix) {
                if (!_disposeWrapper.IsDisposed)
                    return $"{prefix}ITouchable<{typeof(T).NiceName()}>.Log{(_logLevel != LogLevel.Error ? _logLevel.ToString() : "")}[{_id}]({_prefix}){(_parentCreateLogHandler != null ? "\n" + _parentCreateLogHandler(prefix + '\t') : "")}";
                return $"{prefix}ITouchable<{typeof(T).NiceName()}>.Log{(_logLevel != LogLevel.Error ? _logLevel.ToString() : "")}[{_id}]({_prefix}) DISPOSED";
            }
            #endregion
            #region IDisposible
            private ThreadSafeDisposeWrapper _disposeWrapper;
            public bool IsDisposed => _disposeWrapper.IsDisposed;
            public void Dispose() {
                _disposeWrapper.RequestDispose();
            }
            private void DisposeInternal() {
                _proxy.Dispose();
                _parentCreateLogHandler = null;
            }
            #endregion
        }
    }
}
