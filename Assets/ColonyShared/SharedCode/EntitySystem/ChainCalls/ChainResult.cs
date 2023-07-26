using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem.ChainCalls
{
    public class ChainResult<T>: ChainArgument<T>, IChainResult
    {
        public string Key { get; private set; }

        public ChainResult(string key):base(default(T))
        {
            Key = key;
        }
    }
}
