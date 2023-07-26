using System;
using System.Collections.Generic;
using EnumerableExtensions;

namespace ReactivePropsNs.Touchables
{
    /// <summary>
    /// Специализированная обёртка для потокобезопасного диспоуза.
    /// Откладывает Dispose до завершения всех воркеров, которые кто-либо запускал
    /// При этом учитывается, что нормальная работа наших, например ITouchable не может приводить к зацикливанию и дедлоку.
    /// А вот приказ на диспоуз запросто может быть результатом обратного действия в результате обработки данных этого же самого стрима.
    /// </summary>
    public class ThreadSafeDisposeWrapper : IDisposable
    {
        private readonly bool IsDebugMode = false;

        private Action _internalDispose;
        private object _parent;
        private object _disposeLock = new object();
        private int _workers = 0;
        // TODO Превратить в enum
        private bool _willBeDisposed = false;
        private bool _isDisposed = false;

        /// <summary>
        /// Инструмент для отладки подвисших воркеров. Позволяет удобно посмотреть какой воркер не был штатно завершён
        /// </summary>
        private List<string> _stacks = new List<string>();

        public ThreadSafeDisposeWrapper(Action internalDispose, object parent)
        {
            _internalDispose = internalDispose;
            _parent = parent;
        }

        ~ThreadSafeDisposeWrapper()
        {
            if (_workers != 0)
                ReactiveLogs.Logger.Error(
                    $"{_parent}.DisposeMultithreadWrapper Workers count isn't zero on destruct. {nameof(_workers)}={_workers}\n" +
                    "Такая ошибка возникает, когда ГарбаджКоллектор сносит объект с незакрытыми воркерами, означает что какой-то тредсейфовый метод сдох не обработав корректно эксепшен, так что количество входов в функцию не равно количеству выходов из неё. Если диспоузВрапперу указать IsDebugMode = true; он будет запоминать стэктрейсы и удалять их когда выходит из функции (не забыть только убрать после себя). Так что посмотрев на стектрейс можно сразу увидеть в каком методе какого класса произошёл эксепшен, воткнуть туда try catch и вывести куда-нибудь информацию что случилось.\n" +
                    $"{_stacks.ItemsToStringByLines()}");
        }

        public bool IsDisposed
        {
            get
            {
                lock (_disposeLock)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Если объект уже ушёл на Dispose возвращает False, иначе добавляет счётчик активных воркеров.
        /// Пару вызовов StartWorker() FinishWorker() предполагается использовать внутри одного метода
        /// </summary>
        public bool StartWorker()
        {
            lock (_disposeLock)
            {
                if (_isDisposed || _willBeDisposed)
                    return false;

                _workers++;
                if (IsDebugMode)
                    _stacks.Add(new System.Diagnostics.StackTrace(1).ToString());
            }

            return true;
        }

        public void FinishWorker()
        {
            lock (_disposeLock)
            {
                if (_isDisposed)
                    return;

                if (IsDebugMode)
                    _stacks.Remove(new System.Diagnostics.StackTrace(1).ToString());
                if (--_workers != 0)
                    return;

                if (!_willBeDisposed)
                    return;

                _isDisposed = true;
            } // Это потому что DisposeInternal() вызывает что угодно, а делать это под локом как-то не вежливо.

            _internalDispose();
        }

        public virtual void RequestDispose()
        {
            lock (_disposeLock)
            {
                if (_isDisposed)
                    return;

                _willBeDisposed = true;

                if (_workers != 0)
                    return;

                _isDisposed = true;
            } // Это потому что DisposeInternal() вызывает что угодно, а делать это под локом как-то не вежливо.

            _internalDispose();
        }

        public override string ToString()
        {
            return $"{{{GetType().NiceName()} {(_isDisposed ? " Disposed" : (_willBeDisposed ? " WillBeDisposed" : ""))} _workers:{_workers}}}";
        }

        public void Dispose()
        {
            RequestDispose();
        }
    }
}