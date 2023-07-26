using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.Network.Statistic
{
    public class RpcInnerStat
    {
        public string MessageTypeField;
        public long CountField;//outdoing

        public string MessageType => MessageTypeField;
        public long Count => CountField;

        public override string ToString()
        {
            return $"{MessageTypeField}: {CountField}";
        }
    }
}
