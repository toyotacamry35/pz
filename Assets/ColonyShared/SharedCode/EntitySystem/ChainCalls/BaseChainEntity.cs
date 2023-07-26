using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Core.Environment.Logging.Extension;
using SharedCode.Logging;
using SharedCode.Entities.Service;
using SharedCode.Serializers;

namespace SharedCode.EntitySystem.ChainCalls
{
    public class BaseChainEntity: IChainedEntity
    {
        protected EntityMethodsCallsChainBatch chainBatch;

        protected IEntity __entity__;

        private int _firstTypeId;

        private Guid _firstEntityId;

        private Guid _firstRepositoryId;

        public BaseChainEntity(IEntity entity)
        {
            this.__entity__ = entity;
            _firstTypeId = this.__entity__.TypeId;
            _firstEntityId = this.__entity__.Id;
            _firstRepositoryId = ((IEntityExt) this.__entity__).OwnerNodeId;
            chainBatch = new EntityMethodsCallsChainBatch();
        }

        public BaseChainEntity(IEntity entity, IChainedEntity fromChain)
        {
            this.__entity__ = entity;
            var baseChainEntity = fromChain.GetBaseChainEntity();
            chainBatch = baseChainEntity.chainBatch;
            _firstEntityId = baseChainEntity._firstEntityId;
            _firstTypeId = baseChainEntity._firstTypeId;
            _firstRepositoryId = baseChainEntity._firstRepositoryId;
        }

        public ChainCancellationToken Run([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (chainBatch == null)
            {
                Log.Logger.IfError()?.Message("Try run null chain call for entity {0}", __entity__.Id).Write();
                return null;
            }

            if (chainBatch.Chain.Count == 0)
                Log.Logger.IfWarn()?.Message("Try run empty chain call {0} for entity {1}", chainBatch.Id, __entity__.Id).Write();

            if (chainBatch.Chain.Count > 5)
                Log.Logger.IfError()?.Message("Too many blocks in chain call {0} for entity {1} count", chainBatch.Id, __entity__.Id, chainBatch.Chain.Count).Write();

            if (chainBatch.Chain.Count(x => x is ChainBlockPeriod && ((ChainBlockPeriod)x).Repeat != 0) > 1)
            {
                Log.Logger.IfError()?.Message("Try run chain call for entity {0} with more than one periodic call. Skipped", __entity__.Id).Write();
                return null;
            }

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await __entity__.EntitiesRepository.Get<IChainCallServiceEntityExternal>(_firstRepositoryId))
                {
                    var chainEntity = wrapper?.Get<IChainCallServiceEntityExternal>(_firstRepositoryId);
                    if (chainEntity == null)
                    {
                        Log.Logger.IfError()?.Message("IChainCallServiceEntityExternal with id {0} not found. Call from {1}:{2}", _firstRepositoryId, filePath, lineNumber).Write();
                        return;
                    }

                    await chainEntity.ChainCall(chainBatch);
                }
            }, __entity__.EntitiesRepository);

            return CreateCancellationToken();
        }

        public ChainCancellationToken CreateCancellationToken()
        {
            return new ChainCancellationToken(_firstTypeId, _firstEntityId, chainBatch.Id);
        }

        BaseChainEntity IChainedEntity.GetBaseChainEntity()
        {
            return this;
        }

        protected bool validateCallBlock(ChainBlockCall callBlock)
        {
            if (callBlock.RemoteMethodParametersDataBytes != null && callBlock.RemoteMethodParametersDataBytes.Length > 1024)
                Log.Logger.IfError()?.Message("Chain CallBlock too big attributes array size {0}", callBlock.RemoteMethodParametersDataBytes.Length).Write();
            return true;
        }
    }
}
