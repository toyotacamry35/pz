using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Manual.Repositories;
using GeneratedCode.Network.Statistic;
using GeneratedCode.Repositories;
using NLog;
using SharedCode.Cloud;
using SharedCode.Entities.Core;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using SharedCode.Refs;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class ChainCallServiceEntityInternal : IHookOnInit
    {
        private static readonly NLog.Logger DebugLogger = LogManager.GetLogger("ChainCallDebug");
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        ConcurrentQueue<ChainCallInfo> _incommingCalls = new ConcurrentQueue<ChainCallInfo>();

        SortedDictionary<long, ChainCallInfo> _defferedChainsToExecute = new SortedDictionary<long, ChainCallInfo>();

        private ConcurrentDictionary<Guid, ChainCallInfo> _allDefferedCalls = new ConcurrentDictionary<Guid, ChainCallInfo>();

        private const int MaxIncomingChainCallPerFrame = 1000;

        private const int ChainCallFrameDelay = 10;

        private int _executed = 0;

        private int _deffered = 0;

        public Task OnInit()
        {
            AsyncUtils.RunAsyncTask(updateDefferedChainCalls);
            return Task.CompletedTask;
        }

        private async Task addChainCall(IEntityMethodsCallsChain chainCall, int typeId, Guid entityId)
        {
            var chainCallInfo = new ChainCallInfo
            {
                TypeId = typeId,
                ChainCallId = chainCall.Id,
                EntityId = entityId
            };
            if (ServerCoreRuntimeParameters.CollectChainCallHistory)
            {
                var description = await chainCall.GetDescription();
                DebugLogger.IfInfo()?.Message("ADD: {0} CALL TIME {1} DESC {2}", chainCall.Id, UnixTimeHelper.DateTimeFromUnix(chainCall.NextTimeToCall).ToString(), description).Write();
            }

            resheduleToIncomingQueue(chainCallInfo, chainCall.NextTimeToCall);
        }

        private void setOverallStatistics()
        {
            Statistics<ChainCallOverallRuntimeStatistics>.Instance.Set(_incommingCalls.Count, _deffered, _executed, _allDefferedCalls.Count);
        }

        void cancelChainCall(Guid chainCallId)
        {
            ChainCallInfo chainCallInfo;
            if (_allDefferedCalls.TryRemove(chainCallId, out chainCallInfo))
                chainCallInfo.IsCanceled = true;
        }

        private async Task updateDefferedChainCalls()
        {
            try
            {
                while (!EntitiesRepository.StopToken.IsCancellationRequested)
                {
                    try
                    {
                        _deffered = _defferedChainsToExecute.Count;
                        setOverallStatistics();

                        int count = 0;
                        int delay = ChainCallFrameDelay;
                        ChainCallInfo incomingChainCall;
                        while (_incommingCalls.TryDequeue(out incomingChainCall))
                        {
                            if (incomingChainCall.IsCanceled)
                                continue;

                            ChainCallInfo callInfo;
                            if (!_defferedChainsToExecute.TryGetValue(incomingChainCall.NextTimeToCall, out callInfo))
                                _defferedChainsToExecute.Add(incomingChainCall.NextTimeToCall, incomingChainCall);
                            else
                            {
                                while (callInfo.Next != null)
                                    callInfo = callInfo.Next;
                                callInfo.Next = incomingChainCall;
                            }

                            count++;
                            if (count > MaxIncomingChainCallPerFrame)
                            {
                                delay = 1;
                                break;
                            }
                        }

                        count = 0;
                        if (_defferedChainsToExecute.Count > 0)
                        {
                            var dateTimeNow = DateTime.UtcNow.ToUnix();
                            long lastKey = int.MinValue;
                            foreach (var pair in _defferedChainsToExecute)
                            {
                                if (count > MaxIncomingChainCallPerFrame)
                                {
                                    delay = 1;
                                    break;
                                }

                                if (pair.Key > dateTimeNow)
                                    break;

                                lastKey = pair.Key;

                                var callInfo = pair.Value;
                                while (callInfo != null)
                                {
                                    var next = callInfo.Next;
                                    callInfo.Next = null;

                                    if (!callInfo.IsCanceled)
                                    {
                                        _allDefferedCalls.TryRemove(callInfo.ChainCallId, out _);
                                        executeChainCall(callInfo);
                                    }

                                    callInfo = next;
                                }
                            }

                            var key = GetFirst();
                            while (key <= lastKey)
                            {
                                _defferedChainsToExecute.Remove(key);
                                if (!_defferedChainsToExecute.Any())
                                    break;
                                key = GetFirst();
                            }
                        }
                        await Task.Delay(delay);
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "updateDefferedChainCalls exception").Write();;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "updateDefferedChainCalls fatal exception").Write();;
            }
        }

        private long GetFirst()
        {
            long key = default;
            foreach (var chainCallInfo in _defferedChainsToExecute)
            {
                key = chainCallInfo.Key;
                break;
            }

            return key;
        }

        void executeChainCall(ChainCallInfo ci)
        {
            var callInfo = ci;
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    Interlocked.Increment(ref _executed);
                    bool finished = false;
                    IEntityMethodsCallsChain chainCall = null;
                    try
                    {
                        using (var wrapper = await EntitiesRepository.Get(callInfo.TypeId, callInfo.EntityId))
                        { 
                            var entity = wrapper?.Get<IEntity>(callInfo.TypeId, callInfo.EntityId);
                            if (entity == null)
                            {
                                Logger.IfError()?.Message("Chain call {0}. entity not found {1} typeId {2}", callInfo.ChainCallId, callInfo.EntityId, callInfo.TypeId).Write();
                                if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                                    DebugLogger.IfInfo()?.Message("NOT FOUND ENTITY: {0} TYPEID {1} ENTITYID {2}", callInfo.ChainCallId, ci.TypeId, ci.EntityId).Write();
                                return;
                            }

                            if (callInfo.IsCanceled)
                            {
                                if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                                    DebugLogger.IfInfo()?.Message("IS CANCELED: {0}", callInfo.ChainCallId).Write();
                                return;
                            }

                            chainCall = ((IEntityExt)entity).GetChainCall(callInfo.ChainCallId);
                            if (chainCall == null)
                            {
                                Logger.IfError()?.Message("Chain call {0} not found in entity {1} typeId {2} is cancelled {3}", callInfo.ChainCallId, callInfo.EntityId, callInfo.TypeId, callInfo.IsCanceled).Write();
                                if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                                    DebugLogger.IfInfo()?.Message("NOT FOUND CHAINCALL: {0} TYPEID {1} ENTITYID {2}", callInfo.ChainCallId, ci.TypeId, ci.EntityId).Write();
                                return;
                            }

                            if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                                DebugLogger.IfInfo()?.Message("START: {0} DESC {1}", chainCall.Id, await chainCall.GetDescription()).Write();
                        }

                        using (var wrapper = await EntitiesRepository.Get(callInfo.TypeId, callInfo.EntityId))
                        {
                            while (true)
                            {
                                if (callInfo.IsCanceled)
                                    break;

                                var chainBlock = await chainCall.GetCurrentChainBlock();
                                if (chainBlock == null)
                                {
                                    finished = true;
                                    break;
                                }


                                var nextEntityCallResult = await chainCall.TryGetNextEntityToCall();
                                if (!nextEntityCallResult.Result)
                                {
                                    finished = true;
                                    break;
                                }

                                if (nextEntityCallResult.TypeID != callInfo.TypeId || nextEntityCallResult.EntityId != callInfo.EntityId)
                                {
                                    await redirectChainCall(chainCall, nextEntityCallResult.TypeID, nextEntityCallResult.EntityId);
                                    break;
                                }

                                var dateTimeNow = DateTime.UtcNow.ToUnix();
                                if (dateTimeNow < chainCall.NextTimeToCall)
                                {
                                    resheduleToIncomingQueue(callInfo, chainCall.NextTimeToCall);
                                    break;
                                }

                                var increment = true;
                                try
                                {
                                    increment = await chainBlock.Execute(chainCall, this);
                                }
                                catch (RepositoryTimeoutException e)
                                {
                                    resheduleToIncomingQueue(callInfo, DateTime.UtcNow.ToUnix());

                                    if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                                        DebugLogger.IfInfo()?.Message("TIMEOUT EXECUTE: {0} CALL TIME {1}", callInfo.ChainCallId, UnixTimeHelper.DateTimeFromUnix(callInfo.NextTimeToCall).ToString()).Write();
                                    Logger.IfError()?.Message(e, "executeChainCall RepositoryTimeoutException").Write();;
                                    break;
                                }
                                catch (EntityRestoredException e)
                                {
                                    resheduleToIncomingQueue(callInfo, DateTime.UtcNow.ToUnix());

                                    if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                                        DebugLogger.IfInfo()?.Message("RESTORED EXECUTE: {0} CALL TIME {1}", callInfo.ChainCallId, UnixTimeHelper.DateTimeFromUnix(callInfo.NextTimeToCall).ToString()).Write();
                                    Logger.IfError()?.Message(e, "executeChainCall EntityRestoredException").Write();;
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Logger.IfError()?.Message(e, "execute chainBlock {0} error. Break chain. Exception", chainCall.Id).Write();
                                    finished = true;
                                    break;
                                }

                                if (increment)
                                    await chainCall.IncrementCurrentChainIndex();
                            }
                        }
                    }
                    catch (RepositoryTimeoutException e)
                    {
                        resheduleToIncomingQueue(callInfo, DateTime.UtcNow.ToUnix());

                        if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                            DebugLogger.IfInfo()?.Message("TIMEOUT GET: {0} CALL TIME {1}", callInfo.ChainCallId, UnixTimeHelper.DateTimeFromUnix(callInfo.NextTimeToCall).ToString()).Write();
                        Logger.IfError()?.Message(e, "executeChainCall RepositoryTimeoutException").Write();;
                    }
                    catch (EntityRestoredException e)
                    {
                        resheduleToIncomingQueue(callInfo, DateTime.UtcNow.ToUnix());

                        if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                            DebugLogger.IfInfo()?.Message("RESTORED GET: {0} CALL TIME {1}", callInfo.ChainCallId, UnixTimeHelper.DateTimeFromUnix(callInfo.NextTimeToCall).ToString()).Write();
                        Logger.IfError()?.Message(e, "executeChainCall EntityRestoredException").Write();;
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Message(e, "executeChainCall exception").Write();;
                    }

                    if (finished)
                    {
                        if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                            DebugLogger.IfInfo()?.Message("FINISHED: {0}", callInfo.ChainCallId).Write();
                        await removeOnFinished(callInfo, chainCall);
                    }

                }
                finally
                {
                    Interlocked.Decrement(ref _executed);
                }
            });
        }

        private void resheduleToIncomingQueue(ChainCallInfo callInfo, long nextTimeToCall)
        {
            _allDefferedCalls[callInfo.ChainCallId] = callInfo;
            callInfo.NextTimeToCall = nextTimeToCall;
            callInfo.Next = null;
            _incommingCalls.Enqueue(callInfo);
        }

        private async Task removeOnFinished(ChainCallInfo callInfo, IEntityMethodsCallsChain chainCall)
        {
            using (var wrapper = await EntitiesRepository.Get(callInfo.TypeId, callInfo.EntityId))
            {
                var entityExt = (IEntityExt)wrapper?.Get<IEntity>(callInfo.TypeId, callInfo.EntityId);
                await removeEntityChain(entityExt, chainCall.Id);
            }
        }

        async Task redirectChainCall(IEntityMethodsCallsChain chainCall, int typeId, Guid entityId)
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<IClusterAddressResolverServiceEntityServer>())
            {
                var clusterAddressResolverServiceEntity = wrapper?.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                if (clusterAddressResolverServiceEntity == null)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} not found IClusterAddressResolverServiceEntityServer. EntityMethodsCallsChain id {2} ", typeId, entityId, chainCall.Id).Write();
                    return;
                }

                var remoteRepositoryId = await clusterAddressResolverServiceEntity.GetEntityAddressRepositoryId(typeId, entityId);
                if (remoteRepositoryId == Guid.Empty)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} {3} not found in cluster. EntityMethodsCallsChain id {2}.  [ It's Ok if U destroy entity by the last step of chaincall. Boris 'll fix this mis-error in this case. ]", typeId, entityId, chainCall.Id, ReplicaTypeRegistry.GetTypeById(typeId)).Write();
                    return;
                }

                if (remoteRepositoryId == EntitiesRepository.Id)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} not found in this repository. EntityMethodsCallsChain id {2} ", typeId, entityId, chainCall.Id).Write();
                    return;
                }

                using (var remoteChainCallServiceWrapper = await EntitiesRepository.Get<IChainCallServiceEntityInternal>(remoteRepositoryId))
                {
                    if (remoteChainCallServiceWrapper == null)
                    {
                        Logger.IfError()?.Message("IClusterAddressResolverServiceEntity for repository {0} not found in repository {1} ", remoteRepositoryId, EntitiesRepository.Id).Write();
                        return;
                    }
                    Logger.IfDebug()?.Message("redirecting chainCall {0} from {1} to {2}", chainCall.Id, EntitiesRepository.Id, remoteRepositoryId).Write();
                    var remoteChainCallService = remoteChainCallServiceWrapper.Get<IChainCallServiceEntityInternal>(remoteRepositoryId);
                    if (remoteChainCallService == null)
                    {
                        Logger.IfError()?.Message("remote ChainCallServiceInternal for repository {0} not found in repository {1} ", remoteRepositoryId, EntitiesRepository.Id).Write();
                        return;
                    }
                    await remoteChainCallService.ChainCall(chainCall);
                }
            }
        }

        async Task redirectCancelChain(Guid chainCallId, int typeId, Guid entityId)
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<IClusterAddressResolverServiceEntityServer>())
            {
                var clusterAddressResolverServiceEntity = wrapper?.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                if (clusterAddressResolverServiceEntity == null)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} not found IClusterAddressResolverServiceEntityServer. EntityMethodsCallsChain id {2} ", typeId, entityId, chainCallId).Write();
                    return;
                }
                var remoteRepositoryId = await clusterAddressResolverServiceEntity.GetEntityAddressRepositoryId(typeId, entityId);
                if (remoteRepositoryId == Guid.Empty)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} not found in cluster. EntityMethodsCallsChain id {2} ", typeId, entityId, chainCallId).Write();
                    return;
                }
                using (var remoteChainCallServiceWrapper = await EntitiesRepository.Get<IChainCallServiceEntityInternal>(remoteRepositoryId))
                {
                    if (remoteChainCallServiceWrapper == null)
                    {
                        Logger.IfError()?.Message("IClusterAddressResolverServiceEntity for repository {0} not found in repository {1} ", remoteRepositoryId, EntitiesRepository.Id).Write();
                        return;
                    }
                    Logger.IfDebug()?.Message("redirecting CancelChain {0} from {1} to {2}", chainCallId, EntitiesRepository.Id, remoteRepositoryId).Write();
                    var remoteChainCallService = remoteChainCallServiceWrapper.Get<IChainCallServiceEntityInternal>(remoteRepositoryId);
                    if (remoteChainCallService == null)
                    {
                        Logger.IfError()?.Message("remote ChainCallServiceInternal for repository {0} not found in repository {1} ", remoteRepositoryId, EntitiesRepository.Id).Write();
                        return;
                    }
                    await remoteChainCallService.CancelChain(typeId, entityId, chainCallId);
                }
            }
        }

        async Task<bool> redirectCancelAllChain(int typeId, Guid entityId)
        {
            using (var wrapper = await EntitiesRepository.GetFirstService<IClusterAddressResolverServiceEntityServer>())
            {
                var clusterAddressResolverServiceEntity = wrapper?.GetFirstService<IClusterAddressResolverServiceEntityServer>();
                if (clusterAddressResolverServiceEntity == null)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} not found IClusterAddressResolverServiceEntityServer", typeId, entityId).Write();
                    return false;
                }
                var remoteRepositoryId = await clusterAddressResolverServiceEntity.GetEntityAddressRepositoryId(typeId, entityId);
                if (remoteRepositoryId == Guid.Empty)
                {
                    Logger.IfError()?.Message("Entity {0} id {1} not found in cluster", typeId, entityId).Write();
                    return false;
                }
                using (var remoteChainCallServiceWrapper = await EntitiesRepository.Get<IChainCallServiceEntityInternalServer>(remoteRepositoryId))
                {
                    if (remoteChainCallServiceWrapper == null)
                    {
                        Logger.IfError()?.Message("IClusterAddressResolverServiceEntity for repository {0} not found in repository {1} ", remoteRepositoryId, EntitiesRepository.Id).Write();
                        return false;
                    }

                    Logger.IfDebug()?.Message("redirecting CancelAllChain from {0} to {1}", EntitiesRepository.Id, remoteRepositoryId).Write();
                    var remoteChainCallService = remoteChainCallServiceWrapper.Get<IChainCallServiceEntityInternalServer>(remoteRepositoryId);
                    if (remoteChainCallService == null)
                    {
                        Logger.IfError()?.Message("remote ChainCallServiceInternal for repository {0} not found in repository {1} ", remoteRepositoryId, EntitiesRepository.Id).Write();
                        return false;
                    }
                    var result = await remoteChainCallService.CancelAllChain(typeId, entityId);
                    return result;
                }
            }
        }

        public async Task ChainCallImpl(IEntityMethodsCallsChain chainCall)
        {
            await ChainCallInternal(chainCall);
        }

        public async Task AddExistingChainCallsImpl(List<IEntityMethodsCallsChain> chainCalls, int typeId, Guid entityId)
        {
            foreach (var chainCall in chainCalls)
                await addChainCall(chainCall, typeId, entityId);
        }

        public async Task ChainCallBatchImpl(List<IEntityMethodsCallsChain> chainCalls)
        {
            foreach (var chainCall in chainCalls)
                await ChainCallInternal(chainCall);
        }

        private async Task ChainCallInternal(IEntityMethodsCallsChain chainCall)
        {
            Logger.IfDebug()?.Message("incoming chainCall {0} repository {1}", chainCall.Id, EntitiesRepository.Id).Write();
            var nextEntityToCallResult = await chainCall.TryGetNextEntityToCall();
            if (!nextEntityToCallResult.Result)
            {
                Logger.IfError()?.Message("EntityMethodsCallsChain {0} has no actual chaincall blocks", chainCall.Id).Write();
                return;
            }

            var entityRef = ((IEntitiesRepositoryExtension) EntitiesRepository).GetRef(nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId);
            if (entityRef == null)
            {
                if (EntitiesRepository.CloudNodeType == CloudNodeType.Client)
                    return;

                await redirectChainCall(chainCall, nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId);
                return;
            }

            var description = await chainCall.GetDescription();
            using (var wrapper = await EntitiesRepository.Get(nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId, (object)description))
            {
                if (wrapper == null)
                {
                    Logger.IfError()?.Message("ChainCallImpl Entity type {0} id {1} not found", nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId).Write();
                    return;
                }

                var entityExt = wrapper.Get<IEntityExt>(nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId);
                if (entityExt == null || entityExt.OwnerNodeId != EntitiesRepository.Id)
                {
                    await redirectChainCall(chainCall, nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId);
                    return;
                }

                await entityExt.AddChainCall(chainCall);
                await addChainCall(chainCall, nextEntityToCallResult.TypeID, nextEntityToCallResult.EntityId);
            }
        }

        //TODO !!!! Обработать ситуацию когда ChainCall переехал на другую Entity (А в токене остались ссылки на старую Entity), сейчас нет функционала поиска ChainCall по его Id
        public async Task CancelChainImpl(int typeId, Guid entityId, Guid chainId)
        {
            using (var wrapper = await EntitiesRepository.Get(typeId, entityId))
            {
                if (wrapper == null)
                {
                    if (EntitiesRepository.CloudNodeType == CloudNodeType.Client)
                        return;
                    await redirectCancelChain(chainId, typeId, entityId);
                    return;
                }

                var entityExt = wrapper.Get<IEntityExt>(typeId, entityId);
                if (entityExt == null || entityExt.OwnerNodeId != EntitiesRepository.Id)
                {
                    await redirectCancelChain(chainId, typeId, entityId);
                    return;
                }

                await removeEntityChain(entityExt, chainId);
            }
        }

        //TODO !!!! Обработать ситуацию когда ChainCall переехал на другую Entity (А в токене остались ссылки на старую Entity), сейчас нет функционала поиска ChainCall по его Id
        public async Task<bool> CancelAllChainImpl(int typeId, Guid entityId)
        {
            using (var wrapper = await EntitiesRepository.Get(typeId, entityId))
            {
                if (wrapper == null)
                {
                    if (EntitiesRepository.CloudNodeType == CloudNodeType.Client)
                        return false;
                    var result = await redirectCancelAllChain(typeId, entityId);
                    return result;
                }

                var entityExt = wrapper.Get<IEntityExt>(typeId, entityId);
                if (entityExt == null || entityExt.OwnerNodeId != EntitiesRepository.Id)
                {
                    var result = await redirectCancelAllChain(typeId, entityId);
                    return result;
                }

                if (entityExt.GetChainCalls().Count > 0)
                    using (var wrapper2 = await ((IEntitiesRepositoryExtension)EntitiesRepository).GetExclusive(typeId, entityId))
                    {
                        foreach (var chainId in entityExt.GetChainCalls().Keys.ToList())
                            await removeEntityChain(entityExt, chainId);
                    }
            }
            return true;
        }

        private async Task removeEntityChain(IEntityExt entityExt, Guid chainId)
        {
            if (ServerCoreRuntimeParameters.CollectChainCallHistory)
                DebugLogger.IfInfo()?.Message("REMOVED: {0}", chainId).Write();
            cancelChainCall(chainId);
            if (entityExt != null)
            {
                using (var wrapper = await ((IEntitiesRepositoryExtension) EntitiesRepository).GetExclusive(((IEntity)entityExt).TypeId, ((IEntity)entityExt).Id))
                {
                    var result = await entityExt.TryRemoveChainCall(chainId);
                    if (result.Result)
                    {
                        List<Guid> forksCopy = null;
                        lock (result.ChainCall.ForksIds)
                        {
                            forksCopy = result.ChainCall.ForksIds.ToList();
                            result.ChainCall.ForksIds.Clear();
                        }

                        foreach (var forkId in forksCopy)
                            await removeEntityChain(entityExt, forkId);
                    }
                }
            }
        }
    }

    public class ChainCallInfo
    {
        public long NextTimeToCall { get; set; }

        public int TypeId { get; set; }

        public Guid EntityId { get; set; }

        public Guid ChainCallId { get; set; }

        public ChainCallInfo Next { get; set; }

        public bool IsCanceled { get; set; }
    }
}
