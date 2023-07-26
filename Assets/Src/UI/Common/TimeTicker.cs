using System;
using System.Collections.Generic;
using System.Threading;
using Core.Environment.Logging.Extension;
using ReactivePropsNs;
using SharedCode.Utils;
using UnityAsyncAwaitUtil;
using UnityEngine;

namespace Uins
{
    public class TimeTicker : HasDisposablesMonoBehaviour
    {
        public static TimeTicker Instance { get; private set; }

        private Dictionary<int, ReactivePropertyContainer> _utcTimers = new Dictionary<int, ReactivePropertyContainer>();
        private Dictionary<int, ReactivePropertyContainer> _localTimers = new Dictionary<int, ReactivePropertyContainer>();
        private bool _sendingInProgress = false;
        private Queue<SuspendedTicker> _suspendedTickers = new Queue<SuspendedTicker>();


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        private void Update()
        {
            var utcTimestamp = DateTime.UtcNow.ToUnix();
            var localTimestamp = DateTime.Now.ToUnix();

            _sendingInProgress = true;
            foreach (var kvp in _utcTimers)
                kvp.Value.SetTime(utcTimestamp);

            foreach (var kvp in _localTimers)
                kvp.Value.SetTime(localTimestamp);
            _sendingInProgress = false;
            
            while (_suspendedTickers.Count > 0) {
                var suspended = _suspendedTickers.Dequeue();
                var stream = GetTimer(suspended.RoundIntervalInSeconds, suspended.LocalTimers);
                suspended.Proxy.SetCreateLog(prefix => $"{prefix}{nameof(TimeTicker)}.StreamProxy suspended\n{stream.DeepLog(prefix + "\t")}");
                stream.Subscribe(D, dt => suspended.Proxy.OnNext(dt), suspended.Proxy.Dispose);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public IStream<DateTime> GetUtcTimer(float intervalInSeconds)
        {
            return GetTimer(intervalInSeconds, localTimers: false);
        }

        public IStream<DateTime> GetLocalTimer(float intervalInSeconds)
        {
            return GetTimer(intervalInSeconds, localTimers: true);
        }


        //=== Private =========================================================

        private IStream<DateTime> GetTimer(float intervalInSeconds, bool localTimers)
        {
            if (!SyncContextUtil.IsInUnity)
                throw new Exception("RunAsyncTaskFromUnity not from unity");

            if (intervalInSeconds <= float.Epsilon)
            {
                UI.Logger.IfError()?.Message($"Wrong interval: {intervalInSeconds}").Write();
                return null;
            }

            var roundInterval = Math.Max(1, Mathf.RoundToInt(intervalInSeconds * 1000));
            ReactivePropertyContainer container;

            if (_sendingInProgress) {
                var proxy = new StreamProxy<DateTime>(createLog: prefix => $"{prefix}{typeof(TimeTicker).NiceName()}.GetTimer({roundInterval})");
                _suspendedTickers.Enqueue(new SuspendedTicker() { Proxy = proxy, RoundIntervalInSeconds = intervalInSeconds, LocalTimers = localTimers });
                return proxy;
            }
            var timers = localTimers ? _localTimers : _utcTimers;
            if (!timers.TryGetValue(roundInterval, out container))
            {
                container = new ReactivePropertyContainer(D, roundInterval, localTimers ? DateTime.Now.ToUnix() : DateTime.UtcNow.ToUnix());
                timers.Add(roundInterval, container);
            }
            return container.GetOutStream(); //восстановление обратно до DateTime
        }

        //            var f1 = 0.1f; //DEBUG Тесты
        //            TimeTicker.Instance.Get(f1).Action(D, (dt) => DebugInfo(f1, dt));
        //
        //            var f2 = 2;
        //            TimeTicker.Instance.Get(f2).Action(D, (dt) => DebugInfo(f2, dt));
        //
        //            var f3 = 3.051345f; //==> 3.052
        //            TimeTicker.Instance.Get(f3).Action(D, (dt) => DebugInfo(f3, dt));
        //
        //        private void DebugInfo(float f, DateTime dateTime)
        //        {
        //            UI.Logger.IfDebug()?.Message($"[{f}] dt={dateTime:mm:ss:ffff} utcNow={DateTime.UtcNow:mm:ss:ffff}").Write();
        //        }


        //=== Subclass ================================================================================================

        private struct SuspendedTicker
        {
            public bool LocalTimers;
            public float RoundIntervalInSeconds;
            public StreamProxy<DateTime> Proxy;
        }

        private class ReactivePropertyContainer
        {
            public int roundInterval;

            const int CONNECTIONS_PER_STREAM = 25;
            private int lastStreamConnections = 0;

            const int REMOVE_UNUSED_CONTAINERS_PERIOD = 500;

            private List<OutStreamContainer> OutStreams = new List<OutStreamContainer>();
            private DisposableComposite innerDisposables = new DisposableComposite();

            public ReactivePropertyContainer(ICollection<IDisposable> disposables, int roundInterval, long time)
            {
                this.roundInterval = roundInterval;
                disposables.Add(innerDisposables);
                OutStreams.Add(new OutStreamContainer(innerDisposables, roundInterval, time));

            }

            public void SetTime(long time) {
                for (int i = 0; i < OutStreams.Count; i++)
                    OutStreams[i].SetTime(time);
                if (Time.frameCount % REMOVE_UNUSED_CONTAINERS_PERIOD == 0)
                    RemoveEmptyContainers();
            }
            public IStream<DateTime> GetOutStream()
            {
                if (++lastStreamConnections <= CONNECTIONS_PER_STREAM)
                    return OutStreams[OutStreams.Count - 1].OutStream;
                var innerContainer = OutStreams[0].CreateAdditionalOne(innerDisposables);
                OutStreams.Add(innerContainer);
                lastStreamConnections = 0;
                //ReactiveLogs.Logger.IfError()?.Message($"!!!!!!!!!! !!!!!!!!!! !!!!!!!!!! NEW STREAM CONTAINER (interval:{roundInterval}) !!!!!!!!!! !!!!!!!!!! !!!!!!!!!!").Write();
                RemoveEmptyContainers();
                return innerContainer.OutStream;
            }
            private void RemoveEmptyContainers()
            {
                for (int i = OutStreams.Count - 2; i >= 0; --i)
                    if (!OutStreams[i].OutStream.HasListeners())
                        OutStreams.RemoveAt(i);
            }

            private class OutStreamContainer
            {
                private int roundInterval;
                public int roundShift;
                public long lastProcessedTime = 0;
                public StreamProxy<DateTime> OutStream;

                public OutStreamContainer(ICollection<IDisposable> disposables, int roundInterval)
                {
                    this.roundInterval = roundInterval;
                    roundShift = UnityEngine.Random.Range(0, roundInterval);
                    disposables.Add(OutStream = PooledStreamProxy<DateTime>.Create(createLog: DeepLog));
                }
                public OutStreamContainer(ICollection<IDisposable> disposables, int roundInterval, long currentTime) : this (disposables, roundInterval)
                {
                    lastProcessedTime = (currentTime + roundShift) / roundInterval;
                }
                private string DeepLog(string prefix)
                {
                    return $"{prefix}{typeof(TimeTicker).NiceName()}.GetTimer({roundInterval}) // lastProcessedTime:{UnixTimeHelper.DateTimeFromUnix(lastProcessedTime * roundInterval)}";
                }
                public void SetTime(long time)
                {
                    var processedTime = (time + roundShift) / roundInterval;
                    if (lastProcessedTime == processedTime)
                        return;
                    lastProcessedTime = processedTime;
                    OutStream.OnNext(UnixTimeHelper.DateTimeFromUnix(processedTime * roundInterval));
                }
                public OutStreamContainer CreateAdditionalOne(ICollection<IDisposable> disposables)
                {
                    var container = new OutStreamContainer(disposables, roundInterval);
                    container.lastProcessedTime = (((lastProcessedTime * roundInterval) - roundShift) + container.roundShift) / roundInterval;
                    return container;
                }
            }
        }
    }
}