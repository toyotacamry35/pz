using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Network
{
    public interface INetworkMessageFuncInfo
    {
        Task Run();

        Task GetTaskToComplete();
    }
}
