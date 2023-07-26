using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Network
{
    public class RemoteMethodAttribute: Attribute
    {
        public MessageSendOptions SendOptions { get; private set; }

        public int TimeoutSeconds { get; private set; }

        public RemoteMethodAttribute() : this(MessageSendOptions.ReliableOrdered, 0)
        {
        }

        public RemoteMethodAttribute(MessageSendOptions sendOptions):this(sendOptions, 0)
        {
        }

        public RemoteMethodAttribute(int timeoutSeconds) :this(MessageSendOptions.ReliableOrdered, timeoutSeconds)
        {
        }

        public RemoteMethodAttribute(MessageSendOptions sendOptions, int timeoutSeconds)
        {
            SendOptions = sendOptions;
            TimeoutSeconds = timeoutSeconds;
        }
    }
}
