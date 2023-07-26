using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using Prometheus;

namespace GeneratedCode.Network.Statistic
{
    public class ChainCallOverallRuntimeStatistics : CoreBaseStatistic
    {
        private static readonly NLog.Logger  _overallStatisticsLog = LogManager.GetLogger("Telemetry-ChainCallOverallRuntimeStatistics");
        private readonly Gauge _allCounter;
        private readonly Gauge _deferredCounter;
        private readonly Gauge _incomingCounter;
        private readonly Gauge _executedCounter;

        private int _incoming;
        private int _deferred;
        private int _executed;
        private int _all;

        public ChainCallOverallRuntimeStatistics()
        {
            _incomingCounter = Metrics.CreateGauge("chain_call_incoming","chain_call_incoming");
            _deferredCounter = Metrics.CreateGauge("chain_call_deferred","chain_call_deferred");
            _executedCounter = Metrics.CreateGauge("chain_call_executed","chain_call_executed");
            _allCounter = Metrics.CreateGauge("chain_call_all","chain_call_all");
        }
        
        public void Set(int incoming, int deffered, int executed, int all)
        {
            SetInternal(incoming, deffered, executed, all);
        }

        [Conditional("ENABLE_NETWORK_STATISTICS")]
        private void SetInternal(int incoming, int deffered, int executed, int all)
        {
            _incomingCounter.Set(incoming);
            _deferredCounter.Set(deffered);
            _executedCounter.Set(executed);
            _allCounter.Set(all);
            
            Interlocked.Exchange(ref _incoming, incoming);
            Interlocked.Exchange(ref _deferred, deffered);
            Interlocked.Exchange(ref _executed, executed);
            Interlocked.Exchange(ref _all, all);
        }

        protected override void LogStatistics()
        {
            if (_incoming != 0 || _deferred !=0 || _executed != 0 || _all != 0)
                _overallStatisticsLog.IfInfo()?.Message("Incoming queue size {incoming_queue_size}, " +
                                                              "deferred queue size {deferred_queue_size}, " +
                                                              "executing now {executing_now}, " +
                                                              "all runtime {all_runtime}",
                    _incoming,
                    _deferred,
                    _executed,
                    _all)
                    .Write();
        }
    }
}
