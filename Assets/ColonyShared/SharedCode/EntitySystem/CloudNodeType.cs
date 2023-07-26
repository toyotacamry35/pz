using System;

namespace SharedCode.Cloud
{
    [Flags]
    public enum CloudNodeType: int
    {
        None = 0,
        Server = 1,
        Client = 2
    }
}
