using System;

namespace GeneratedCode.Network.Statistic
{
    public abstract class CoreBaseStatistic : ICoreStatistics
    {
        private DateTime _lastStatisticsLoggingTime = DateTime.UtcNow;

        protected virtual float PeriodSeconds { get; } = 1f;

        protected abstract void LogStatistics();

        public void Update()
        {
            if ((DateTime.UtcNow - _lastStatisticsLoggingTime).TotalSeconds >= PeriodSeconds)
            {
                _lastStatisticsLoggingTime = _lastStatisticsLoggingTime.AddSeconds(PeriodSeconds);
                LogStatistics();
            }
        }
    }
}
