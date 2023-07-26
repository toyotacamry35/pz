using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.Entities.Core;
using SharedCode.Serializers;
using SharedCode.OurSimpleIoC;

namespace SharedCode.EntitySystem.ChainCalls
{
    internal static class ChainCallHelpers
    {
        public static async ValueTask<(T result, int newOffset)> GetArg<T>(byte[] data, int offset, int num, IChainContext chainContext, IReadOnlyDictionary<int, string> argumentRefs)
        {
            var serializer = ServicesPool.Services.Get<ISerializer>();

            string key;
            T value;

            if (argumentRefs != null && argumentRefs.TryGetValue(num, out key))
            {
                var valueFromContext = await chainContext.TryGetContextValue(key);
                if (!valueFromContext.Result)
                    value = default;
                else
                    value = (T)valueFromContext.Value;
            }
            else
                value = serializer.Deserialize<T>(data, ref offset);
            return (value, offset);
        }
    }
}
