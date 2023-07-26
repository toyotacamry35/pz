using Assets.Src.ResourcesSystem.Base;
using System;

namespace GeneratedCode.Custom.Config
{
    public class ServerServicesConfigDef : BaseResource
    {
        public string APIToken { get; set; }
        public string APIEndpoint { get; set; }
        public string APIHostname { get; set; }

        public string APIEndpoint_ProfileGet { get; set; }
        public string APIEndpoint_EmailPut { get; set; }
        public string APIEndpoint_CodePost { get; set; }

        public ResourceRef<PrometheusConfig> Prometheus { get; set; }
        public ResourceRef<ElasticConfig> Elastic { get; set; }

        public string InternalBindAddress { get; set; }
        public string ExternalBindAddress { get; set; }
        public string InternalAnnounceAddress { get; set; }
        public string ExternalAnnounceAddress { get; set; }

        public string MongoShardConnectionString { get; set; }
        public string MongoShardDataBaseName { get; set; }
        public string MongoMetaConnectionString { get; set; }
        public string MongoMetaDataBaseName { get; set; }
        public string RealmId { get; set; } = "";

        public class PrometheusConfig : BaseResource
        {
            public bool ServerMetricsEnabled { get; set; } = true;

            public bool ProcessMetricsEnabled { get; set; } = true;

            public string PushGatewayAddress { get; set; } = string.Empty;
        }

        public class ElasticConfig : BaseResource
        {
            public bool Enabled { get; set; } = true;

            public string[] Address { get; set; } = System.Array.Empty<string>();

            public string Login { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
