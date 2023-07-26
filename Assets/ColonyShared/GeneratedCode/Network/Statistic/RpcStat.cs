using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.Network.Statistic
{
    public class RpcStat
    {
        public string MessageTypeField;
        public long SentCountField;//outdoing
        public long ReceiveCountField;//incoming
        public long SentBytesField;//outdoing sent
        public long ReceiveResponseBytesField;//outdoing receive
        public long ReceiveBytesField;//incoming receive
        public long SentResponseBytesField;//incoming sent

        public string MessageType => MessageTypeField;
        public long SentCount => SentCountField;
        public long ReceiveCount => ReceiveCountField;
        public long SentBytes => SentBytesField;
        public long ReceiveResponseBytes => ReceiveResponseBytesField;
        public long ReceiveBytes => ReceiveBytesField;
        public long SentResponseBytes => SentResponseBytesField;
    }
}
