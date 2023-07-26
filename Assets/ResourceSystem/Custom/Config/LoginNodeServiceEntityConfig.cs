using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Sessions;
using SharedCode.Config;
using System;

namespace GeneratedCode.Custom.Config
{
    public class LoginNodeServiceEntityConfig : CustomConfig
    {
        public Guid RealmsCollectionId { get; set; }
        public int MaxCCUOnRealm { get; set; }
        public ResourceRef<RealmRulesConfigDef> Rules { get; set; }
        public ResourceRef<RealmRulesQueriesConfigDef> RulesQueries { get; set; }
        public bool EnableAnonymousLogin { get; set; } = true;
    }
}
