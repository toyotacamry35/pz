using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Logging;
using Assets.ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using SharedCode.Entities.Service;
using SharedCode.Refs;
using SharedCode.Serializers;

namespace SharedCode.EntitySystem.ChainCalls
{
    [ProtoContract]
    public class ChainCancellationToken
    {
        [ProtoMember(1)]
        [BsonElement("TypeId")]
        public int _typeId;//TODO не имеем права использовать, так как может меняться в процессе выполнения, не меняется только _chainId

        [ProtoMember(2)]
        [BsonElement("EntityId")]
        public Guid _entityId;//TODO не имеем права использовать, так как может меняться в процессе выполнения, не меняется только _chainId

        [ProtoMember(3)]
        [BsonElement("ChainId")]
        public Guid _chainId;

        [ProtoMember(4)]
        [BsonElement("IsCanceled")]
        public bool _isCanceled;


        public override string ToString() =>
            $"{nameof(ChainCancellationToken)}:  {nameof(_typeId)}: {_typeId}, {nameof(_entityId)}: {_entityId}, {nameof(_chainId)}: {_chainId} {nameof(_isCanceled)}: {_isCanceled}.";

        public ChainCancellationToken()
        {
        }

        public bool IsValid => _chainId != Guid.Empty;

        public bool IsCanceled => _isCanceled;

        public bool CanBeCancelled => !IsCanceled && IsValid;

        public ChainCancellationToken(int typeId, Guid entityId, Guid chainId)
        {
            _typeId = typeId;
            _entityId = entityId;
            _chainId = chainId;
        }


        public void Cancel(IEntitiesRepository entitiesRepository)
        {
            if (!IsValid)
            {
                Log.Logger.IfError()?.Message("Cant cancel empty ChainCancellationToken  {0}", this.ToString()).Write();
                return;
            }

            if (_isCanceled)
            {
                Log.Logger.IfError()?.Message("Chain call token already canceled {0}", this.ToString()).Write();
                return;
            }

            _isCanceled = true;
            //Logger.IfDebug()?.Message($" > > > >  Cancel(1):  _repositoryId: {_repositoryId}, _typeId: {_typeId}, _entityId: {_entityId}, _chainId: {_chainId}.  repo: {entitiesRepository.CloudNodeType}").Write();
            AsyncUtils.RunAsyncTask(async () =>
            {
                //Logger.IfDebug()?.Message($" > > > >  Cancel(2):  _repositoryId: {_repositoryId}, _typeId: {_typeId}, _entityId: {_entityId}, _chainId: {_chainId}.  repo: {entitiesRepository.CloudNodeType}").Write();
                using (var wrapper = await entitiesRepository.Get<IChainCallServiceEntityExternal>(entitiesRepository.Id))
                {
                    var entity = wrapper?.Get<IChainCallServiceEntityExternal>(entitiesRepository.Id);
                    if (entity == null)
                    {
                        Log.Logger.IfError()?.Message("Cancel token IChainCallServiceEntityExternal with id {0} not found. Token {1}", entitiesRepository.Id, this.ToString()).Write();
                        return;
                    }

                    //Logger.IfDebug()?.Message($" > > > >  Cancel(3):  _repositoryId: {_repositoryId}, _typeId: {_typeId}, _entityId: {_entityId}, _chainId: {_chainId}.  repo: {entitiesRepository.CloudNodeType}").Write();
                    await entity.CancelChain(_typeId, _entityId, _chainId);
                }
            }, entitiesRepository);
        }
    }
}
