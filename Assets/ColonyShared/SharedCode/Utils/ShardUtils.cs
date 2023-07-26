using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Utils
{
    public static class ShardUtils
    {
        public static int GetShard(Guid guid, int shardsCount)
        {
            return guid.GetHashCode() % shardsCount;
        }
    }
}
