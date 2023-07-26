using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;

namespace ReactivePropsNs
{
    public class DisposableComposite : ICollection<IDisposable>, IDisposable
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("DisposableComposite");

        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private object _lock = new object();

        //=== Props ===========================================================

        public int Count
        {
            get
            {
                lock (_lock)
                    return _disposables.Count;
            }
        }
        public bool IsReadOnly => false;


        //=== Public ==========================================================

        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        public void Add(IDisposable item)
        {
            if (item == this)
            {
                Logger.IfError()?.Message($"DispComp.Add  item == this : {this}. Ignored!").Write();
                return;
            }

            lock(_lock)
                _disposables.Add(item);
        }

        public void Clear()
        {
            do
            {
                IDisposable item = null;
                lock (_lock)
                {
                    if (_disposables.Count == 0)
                        break;
                    item = _disposables[_disposables.Count - 1];
                    _disposables.RemoveAt(_disposables.Count - 1);
                }
                item.Dispose();
            } while (true);
        }

        public bool Contains(IDisposable item)
        {
            throw new Exception($"{GetType()}.{nameof(Contains)} is incorrect use case.");
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            throw new Exception($"{GetType()}.{nameof(Contains)} is incorrect use case.");
        }

        public bool Remove(IDisposable item)
        {
            return _disposables.Remove(item);
        }

        public void Dispose()
        {
            //ReactiveLogs.Logger.IfDebug()?.Message($"SomeDispose: {new StackTrace()}").Write();
            Clear();
        }
    }
}