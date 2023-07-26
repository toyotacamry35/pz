using System;
using SharedCode.Network;

namespace SharedCode.Interfaces.Network
{
    public enum PacketType : byte
    {
        Send = 0,
        Response = 1,
        Request = 2,
        Exception = 3,
        SendRedirect = 4,
        RequestRedirect = 5,
    };
}
