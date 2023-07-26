using System;
using System.Collections.Generic;
using System.IO;
using Core.Environment.Logging.Extension;
using GeneratedCode.Custom.Config;
using NLog;
using NLog.Targets;
using Prometheus;

namespace GeneratedCode.Network.Statistic
{
    public static class PrometheusStatistic
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static IDisposable Start(string job, string instance, string realmId, ServerServicesConfigDef.PrometheusConfig cfg)
        {
            if (!(cfg?.ServerMetricsEnabled ?? false))
            {
                Logger.IfInfo()?.Message("PrometheusStatistic disabled").Write();
                return null;
            }
            Logger.IfInfo()?.Message("PrometheusStatistic enabled {Endpoint} {job} {Instance}", cfg.PushGatewayAddress, job, instance)
                .Write();
            
            Metrics.SuppressDefaultMetrics();

            var pusher = new MetricPusher(
                new MetricPusherOptions
                {
                    Endpoint = cfg.PushGatewayAddress,
                    Job = job,
                    Instance = instance,
                    AdditionalLabels = new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("RealmId", realmId)
                    }
                });

            
            pusher.Start();
            return pusher;
        }
    }
}