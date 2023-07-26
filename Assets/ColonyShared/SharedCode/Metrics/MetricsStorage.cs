using System;
using System.Collections.Concurrent;

namespace SharedCode.Monitoring
{
    public class MetricsStorage<TKey, TLabeledMetrics, TMetrics>
    {
        private readonly Func<TKey, TMetrics, TLabeledMetrics> _metricsFactory;
        private readonly TMetrics _metrics;

        private readonly ConcurrentDictionary<TKey, TLabeledMetrics> _createdMetrics =
            new ConcurrentDictionary<TKey, TLabeledMetrics>();
        
        public MetricsStorage(TMetrics metrics, Func<TKey, TMetrics, TLabeledMetrics> metricsFactory)
        {
            _metricsFactory = metricsFactory;
            _metrics = metrics;
        }

        public TLabeledMetrics GetOrCreate(TKey key)
        {
            if (_createdMetrics.TryGetValue(key, out var metrics))
            {
                return metrics;
            }
            else
            {
                var labeledMetrics = _metricsFactory(key, _metrics);
                _createdMetrics.TryAdd(key, labeledMetrics);
                return labeledMetrics;
            }
        }
    }
}