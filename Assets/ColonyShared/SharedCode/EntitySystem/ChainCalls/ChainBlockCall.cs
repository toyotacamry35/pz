using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.Network.Statistic;
using NLog;
using ProtoBuf;
using SharedCode.Entities.Core;
using SharedCode.Entities.Service;
using SharedCode.Refs;
using SharedCode.Repositories;

namespace SharedCode.EntitySystem.ChainCalls
{
    [ProtoContract]
    public class ChainBlockCall : ChainBlockBase
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly NLog.Logger DebugLogger = LogManager.GetLogger("ChainCallDebug");

        [ProtoMember(1)]
        public PropertyAddress ObjectAddress { get;  set; }

        [ProtoMember(2)]
        public byte RemoteMethodId { get;  set; }

        [ProtoMember(3)]
        public byte[] RemoteMethodParametersDataBytes { get;  set; }

        [ProtoMember(4)]
        public string StoreResultKey { get;  set; }

        [ProtoMember(5, OverwriteList = true)]
        public Dictionary<int, string> ArgumetRefs { get;  set; }

        public ChainBlockCall()
        {
        }

        public ChainBlockCall(PropertyAddress address, byte remoteMethodId, byte[] remoteMethodParametersDataBytes, Dictionary<int, string> argumetRefs)
        {
            ObjectAddress = address;
            RemoteMethodId = remoteMethodId;
            RemoteMethodParametersDataBytes = remoteMethodParametersDataBytes;
            ArgumetRefs = argumetRefs;
        }

        public override async Task<bool> Execute(IEntityMethodsCallsChain chainCall, IChainCallServiceEntityInternal chainCallService)
        {
            var entityref = ((IEntitiesRepositoryExtension)chainCallService.EntitiesRepository).GetRef(ObjectAddress.EntityTypeId, ObjectAddress.EntityId);
            if (entityref == null)
            {
                Logger.IfError()?.Message("ChainBlockCall2 Execute entity typeId {0} id {1} not found in repository {2} ", ObjectAddress.EntityTypeId, ObjectAddress.EntityId, chainCallService.EntitiesRepository.Id).Write();
                return true;
            }
            entityref.AssertNotRemoteEntity();

            var entity = ((IEntityRefExt)entityref).GetEntity();
            var obj = ((IEntityExt)entity).ResolveDeltaObject(ObjectAddress.DeltaObjectLocalId);

            Statistics<ChainCallRuntimeStatistics>.Instance.BeforeGet(ReplicaTypeRegistry.GetTypeById(obj.TypeId), RemoteMethodId);
            using (var wrapper = await chainCallService.EntitiesRepository.Get(ObjectAddress.EntityTypeId, ObjectAddress.EntityId, RemoteMethodId))
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("ChainBlockCall Execute entity typeId {0} id {1} not found in repository {2} ", ObjectAddress.EntityTypeId, ObjectAddress.EntityId, chainCallService.EntitiesRepository.Id).Write();
                    return true;
                }

                if (entity == null)
                {
                    Logger.IfError()?.Message("ChainBlockCall3 Execute entity typeId {0} id {1} not found in repository {2} ", ObjectAddress.EntityTypeId, ObjectAddress.EntityId, chainCallService.EntitiesRepository.Id).Write();
                    return true;
                }

                if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                    DebugLogger.IfInfo()?.Message("EXECUTE: {0} METHOD {1} TYPEID {2} ENTITYID {3}", chainCall.Id, RemoteMethodId, ObjectAddress.EntityTypeId, ObjectAddress.EntityId).Write();
                Statistics<ChainCallRuntimeStatistics>.Instance.Execute(ReplicaTypeRegistry.GetTypeById(obj.TypeId), RemoteMethodId);
                (var execFn, var _) = ReplicaTypeRegistry.GetExecuteFunc(ReplicaTypeRegistry.GetTypeById(ObjectAddress.EntityTypeId), RemoteMethodId);
                await execFn(entity, RemoteMethodParametersDataBytes, chainCall.ChainContext, StoreResultKey, ArgumetRefs);
            }

            return true;
        }

        public override void AppendToStringBuilder(StringBuilder sb)
        {
            sb.Append("C,");
            sb.Append(ObjectAddress);
            sb.Append(",");
            sb.Append(RemoteMethodId);
        }

        public void SetStoreResultKey(string key)
        {
            StoreResultKey = key;
        }
    }
}
