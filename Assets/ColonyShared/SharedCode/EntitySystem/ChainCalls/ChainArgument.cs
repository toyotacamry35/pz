using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.EntitySystem.ChainCalls
{
    public class ChainArgument<T>
    {
        public  T Value { get; }

        public ChainArgument(T value)
        {
            Value = value;
        }

        public static implicit operator T(ChainArgument<T> from)
        {
            return from.Value;
        }

        public static implicit operator ChainArgument<T>(T value)
        {
            return new ChainArgument<T>(value);
        }
    }
}
