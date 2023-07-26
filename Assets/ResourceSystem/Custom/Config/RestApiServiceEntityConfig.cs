using Assets.Src.ResourcesSystem.Base;
using SharedCode.Config;
using System.Collections.Generic;

namespace GeneratedCode.Custom.Config
{
    public class RestApiServiceEntityConfig : CustomConfig
    {
        public int Port { get; set; }

        public List<string> AllowedIPs { get; set; }
    }
}
