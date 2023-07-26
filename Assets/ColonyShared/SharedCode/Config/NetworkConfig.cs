using Newtonsoft.Json.Linq;

namespace SharedCode.Config
{
    public class NetworkConfig
    {
        public string Type { get; set; }

        public object Parameters { get; set; }
    }
}
