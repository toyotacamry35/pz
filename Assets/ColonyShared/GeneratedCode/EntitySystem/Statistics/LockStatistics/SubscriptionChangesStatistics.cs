using System;
using Prometheus;

namespace GeneratedCode.EntitySystem.Statistics
{
    public class SubscriptionChangesStatistics
    {
        private readonly Summary.Child _waitTime;
        private readonly Summary.Child _processTime;
        private readonly Gauge.Child _queueSize;

        public SubscriptionChangesStatistics(Guid repositoryId)
        {
            var labelNames = new[] {"rid"};

            _waitTime = Metrics.CreateSummary("repository_subscription_change_wait_time", "",
                new SummaryConfiguration
                {
                    LabelNames = labelNames
                }).WithLabels(repositoryId.ToString());
            _processTime = Metrics.CreateSummary("repository_subscription_change_process_time", "",
                new SummaryConfiguration
                {
                    LabelNames = labelNames
                }).WithLabels(repositoryId.ToString());

            _queueSize = Metrics.CreateGauge("subscriptions_change_queue_size", "",
                new GaugeConfiguration
                {
                    LabelNames = labelNames
                }).WithLabels(repositoryId.ToString());
        }

        public void AddWaitTime(TimeSpan time)
        {
            _waitTime.Observe(time.TotalMilliseconds);
        }
        
        public void AddProcessTime(TimeSpan time)
        {
            _processTime.Observe(time.TotalMilliseconds);
        }
        
        public void IncChangeQueue()
        {
            _queueSize.Inc();
        }

        public void DecChangeQueue()
        {
            _queueSize.Dec();
        }
    }
}