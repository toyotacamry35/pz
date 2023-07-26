using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Network;

namespace SharedCode.EntitySystem
{
    public interface IRemoteEntity
    {
        void SetNetworkProxy(INetworkProxy networkProxy);

        INetworkProxy GetNetworkProxy();
    }
}
