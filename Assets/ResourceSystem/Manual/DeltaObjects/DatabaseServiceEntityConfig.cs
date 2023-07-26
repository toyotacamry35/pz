using System;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.Custom.Config;
using SharedCode.Config;

namespace GeneratedCode.DeltaObjects
{
    public class DatabaseServiceEntityConfig : CustomConfig
    {
        public string DatabaseType { get; set; }
        public ResourceRef<ServerServicesConfigDef> ConnectionAddresses { get; set; }
        public bool GenerateVersionsSnapshot { get; set; }
        public bool CheckMigrations { get; set; }
        public int MaxItemsToWrite { get; set; }
        public int SaveDelay { get; set; }
        public int MongoTimeout { get; set; }
        public bool VerboseLogs { get; set; }
        public int CacheLifeTime { get; set; }
    }
}
