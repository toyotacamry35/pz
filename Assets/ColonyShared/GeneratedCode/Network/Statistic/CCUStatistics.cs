using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using Prometheus;

namespace GeneratedCode.Network.Statistic
{
    public class CCUStatistics :  CoreBaseStatistic
    {
        private static readonly NLog.Logger _overallStatisticsLog = LogManager.GetLogger("Telemetry-CCUStatistics");
        private readonly Gauge _ccuCounter;
        private int _maxCcu;
        
        public CCUStatistics()
        {
            _ccuCounter = Metrics.CreateGauge("ccu", "ccu");
        }
        
        public int CCU { get; private set; }
        
        protected override float PeriodSeconds { get; } = 60f;

        public void SetCurrentCCU(int ccu)
        {
            _ccuCounter.Set(ccu);
            CCU = ccu;
        }

        public void SetMaxCCU(int maxccu)
        {
            _maxCcu = maxccu;
        }

        protected override void LogStatistics()
        {
            if (CCU == 0)
                return;

            _overallStatisticsLog.IfInfo()?.Message("CCU {ccu}, Max CCU {max_ccu}", CCU.ToString(), _maxCcu.ToString())
                .Write();
        }
    }
}
