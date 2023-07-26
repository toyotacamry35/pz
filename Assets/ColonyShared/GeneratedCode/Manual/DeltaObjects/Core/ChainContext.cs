using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Entities.Core;

namespace GeneratedCode.DeltaObjects
{
    public partial class ChainContext
    {
        public ValueTask<TryGetContextValueResult> TryGetContextValueImpl(string key)
        {
            ChainContextValueWrapper wrapper;
            if (!Data.TryGetValue(key, out wrapper))
                return new ValueTask<TryGetContextValueResult>(new TryGetContextValueResult
                {
                    Result = false
                });

            return new ValueTask<TryGetContextValueResult>(new TryGetContextValueResult
            {
                Result = true,
                Value = wrapper.Value
            });
        }

        public ValueTask SetContextValueImpl(string key, object value)
        {
            Data[key] = new ChainContextValueWrapper
            {
                Value = value
            };
            return new ValueTask();
        }

        public ValueTask<IChainContext> CloneChainContextImpl()
        {
            var result = new ChainContext();
            foreach (var pair in Data)
                result.Data.Add(pair.Key, new ChainContextValueWrapper { Value = pair.Value });
            return new ValueTask<IChainContext>(result);
        }
    }
}
