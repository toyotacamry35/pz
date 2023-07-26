using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Entities.Cloud;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;

namespace GeneratedCode.DeltaObjects
{
    public partial class ChainCallServiceEntityExternal
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task ChainCallImpl(EntityMethodsCallsChainBatch batch)
        {
            if (batch.Chain.Count == 0)
            {
                Logger.IfError()?.Message("EntityMethodsCallsChainBatch {0} has 0 chain calls", batch.Id).Write();
                return;
            }
            var currentChainCall = EntityMethodsCallsChain.CreateFromBatch(batch);
            using (var chainCallInternal = await EntitiesRepository.GetMasterService<IChainCallServiceEntityInternal>())
            {
                await chainCallInternal.GetMasterService<IChainCallServiceEntityInternal>().ChainCall(currentChainCall);
            }
        }

        public async Task CancelChainImpl(int typeId, Guid entityId, Guid chainId)
        {
            using (var chainCallInternal = await EntitiesRepository.GetMasterService<IChainCallServiceEntityInternal>())
            {
                await chainCallInternal.GetMasterService<IChainCallServiceEntityInternal>().CancelChain(typeId, entityId, chainId);
            }
        }

        public async Task<bool> CancelAllChainImpl(int typeId, Guid entityId)
        {
            using (var chainCallInternal = await EntitiesRepository.GetMasterService<IChainCallServiceEntityInternal>())
            {
                var result = await chainCallInternal.GetMasterService<IChainCallServiceEntityInternal>().CancelAllChain(typeId, entityId);
                return result;
            }
        }
    }
}
