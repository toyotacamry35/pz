using SharedCode.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Utils
{
    public static class NetworkUtils
    {
        public static string GetMyIp(bool local)
        {
            if (local)
                return "127.0.0.1";

            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                // HACK: раньше брали первый попавшийся, но так как на машине может быть установлен докер,
                // который создает виртуальные адаптеры, то фильтруем хотя бы их
                if (netInterface.Name.IndexOf("docker", StringComparison.InvariantCultureIgnoreCase) == -1
                     && (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                         netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            var ipAddress = addrInfo.Address;
                            return ipAddress.ToString();
                        }
                    }
                }
            }

            
            throw new Exception("Not found local ip");
        }

        public static bool IsLocalIp(IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip))
            {
                return true;
            }

            if (IPAddress.Any.Equals(ip))
            {
                return true;
            }
            
            foreach (var localIp in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (localIp.Equals(ip))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
