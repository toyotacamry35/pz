using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using NLog;

namespace GeneratedCode.Network.Statistic
{
    public static class Statistics<T> where T : ICoreStatistics, new()
    {
        public static T Instance { get; } = Create();

        private static T Create()
        {
            var t = new T();
            CoreStatisticsManager.Register(t);
            return t;
        }
    };

    public struct CtsCancellationWrap : IDisposable
    {
        private readonly CancellationTokenSource _cts;

        public CtsCancellationWrap([CanBeNull] CancellationTokenSource cts)
        {
            _cts = cts;
            disposedValue = false;
        }

        #region IDisposable Support
        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    public static class CoreStatisticsManager
    {
        private static CancellationTokenSource _cts;

        private static readonly NLog.Logger _log = LogManager.GetCurrentClassLogger();

        private static ICoreStatistics[] _statistics = Array.Empty<ICoreStatistics>();

        private static readonly object _locker = new object();

        public static CtsCancellationWrap Init()
        {
            InitImpl();
            return new CtsCancellationWrap(_cts);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private static void InitImpl()
        {

            var cts = new CancellationTokenSource();
            if (Interlocked.CompareExchange(ref _cts, cts, null) != null)
                return;

            Thread t = new Thread(Update);
            t.IsBackground = true;
            t.Start(cts.Token);
        }

        internal static void Register(ICoreStatistics newStatistics)
        {
            lock (_locker)
            {
                _statistics = _statistics.Append(newStatistics).ToArray();
            }
        }

        private static void Update(object parameter)
        {
            var token = (CancellationToken)parameter;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    _cts = null;
                    return;
                }

                foreach (var coreStatistics in _statistics)
                {
                    try
                    {
                        coreStatistics.Update();
                    }
                    catch (Exception e)
                    {
                        _log.Error(e, "{0} exception", coreStatistics.GetType().Name);
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}
