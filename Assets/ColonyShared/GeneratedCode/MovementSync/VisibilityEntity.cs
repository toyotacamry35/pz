using Assets.Src.Tools;
using GeneratedCode.Repositories;
using SharedCode.AI;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.OurSimpleIoC;
using SharedCode.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog.Fluent;
using SharedCode.Repositories;
using Vector3 = SharedCode.Utils.Vector3;

namespace GeneratedCode.DeltaObjects
{
    public partial class VisibilityEntity : IHookOnInit, IHookOnDestroy, IHookOnUnload
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private const float DelayUpdateVisibility = 1f;
        private static TimeSpan ForceOwnerhshipChangeTimeout = TimeSpan.FromSeconds(5);
        private static TimeSpan UpdateTimeBudget = TimeSpan.FromSeconds(1);

        private readonly Stopwatch _updateTime = new Stopwatch();

        private CancellationTokenSource _cts;
        public Task OnInit()
        {
            _cts = new CancellationTokenSource();
            DelayUpdate(Id, DelayUpdateVisibility, _cts.Token, EntitiesRepository);
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            return Task.CompletedTask;
        }

        public Task OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            return Task.CompletedTask;
        }

        Dictionary<Guid, VisibilitySubjectStatus> _visibilityStatuses = new Dictionary<Guid, VisibilitySubjectStatus>();
        HashSet<OuterRef<IEntity>> _diffSet = new HashSet<OuterRef<IEntity>>();
        HashSet<OuterRef<IEntity>> _prevObjects = new HashSet<OuterRef<IEntity>>();
        
        private Dictionary<OuterRef<IEntity>, DateTime> _ownershipChangeDates = new Dictionary<OuterRef<IEntity>, DateTime>();

        private Dictionary<OuterRef<IEntity>, ChangeOwnershipRequest> _changeOwnershipRequests = new Dictionary<OuterRef<IEntity>, ChangeOwnershipRequest>();

        private bool CheckClientIsReady(Guid userId)
        {
            var client= EntitiesRepository.TryGetLockfree<IClientCommunicationEntityServer>(userId, ReplicationLevel.Server);
            if (client != null)
            {
                return client.LevelLoaded;
            }

            return false;
        }
        
        List<Guid> _keysToRemove = new List<Guid>();
        public async Task<bool> UpdateImpl()
        {
            try
            {
                _updateTime.Restart();
                var tickStartDate = DateTime.UtcNow;
                
                var grid = VisibilityGrid.Get(WorldSpace, EntitiesRepository);
                ConcurrentDictionary<Guid, ConcurrentDictionary<ValueTuple<int, Guid>, bool>> chars;
                using (var wrapper = await EntitiesRepository.Get<IWorldSpaceServiceEntity>(WorldSpace))
                {
                    var worldSpaceServiceEntity = wrapper.Get<IWorldSpaceServiceEntity>(WorldSpace);

                    chars = worldSpaceServiceEntity?.AllUsersAndTheirCharacters;
                }
                if (chars == null)
                    return true;

                await _visibilityStatuses.RemoveAllNonAlloc(
                    (userId, status) => !chars.ContainsKey(userId) || !CheckClientIsReady(userId),
                    _keysToRemove,
                    async (userId, status) =>
                    {
                        foreach (var entityRefPair in status.Entities)
                        {
                            var hasMobMovementEntityLockFree =
                                EntitiesRepository.TryGetLockfree<IHasMobMovementSync>(entityRefPair.Key, ReplicationLevel.Master);
                            var pathFindingOwnerRepositoryId = hasMobMovementEntityLockFree?.MovementSync.PathFindingOwnerRepositoryIdRuntime;
                            await TryRemoveOwnership(pathFindingOwnerRepositoryId, userId, entityRefPair.Key);
                        }

                        AsyncUtils.RunAsyncTask(async () =>
                        {
                            foreach (var entityRefPair in status.Entities)
                            {
                                await EntitiesRepository.UnsubscribeReplication(entityRefPair.Key.TypeId, entityRefPair.Key.Guid, userId,
                                    ReplicationLevel.ClientBroadcast);
                            }
                        });
                    });
                
                Dictionary<Guid, ValueTuple<List<OuterRef<IEntity>>, VisibilitySubjectStatus>> toSubscribeDict = null;
                Dictionary<Guid, ValueTuple<List<OuterRef<IEntity>>, VisibilitySubjectStatus>> toUnsubscribeDict = null;

                foreach (var c in chars)
                {
                    if (!CheckClientIsReady(c.Key))
                    {
                        continue;
                    }
                    
                    var userId = c.Key;
                    var status = _visibilityStatuses.GetOrCreate(userId);
                    _diffSet.Clear();
                    _prevObjects.Clear();
                    foreach (var oRef in status.Entities)
                    {
                        _diffSet.Add(oRef.Key);
                        _prevObjects.Add(oRef.Key);
                    }

                    status.Entities.Clear();

                    Vector3 selfPosition = default;
                    foreach (var character in c.Value)
                    {
                        var characterRef = new OuterRef<IEntity>(character.Key.Item2, character.Key.Item1);
                        status.Entities.TryAdd(characterRef, Vector3.Default);
                        grid.SampleDataFor<GenericVisibilityType, CharacterMovementState>(characterRef, status.Entities, out selfPosition);
                    }
                    
                    // for each visible entity
                    foreach (var entity in status.Entities)
                    {
                        if (!_diffSet.Remove(entity.Key))
                            _diffSet.Add(entity.Key);

                        if (EntitiesRepository.TryGetLockfree<IHasMobMovementSync>(entity.Key, ReplicationLevel.Master, out var hasMobMovementEntityLockFree))
                        {
                            var pathFindingOwnerRepositoryId = hasMobMovementEntityLockFree?.MovementSync.PathFindingOwnerRepositoryIdRuntime;
                            if (pathFindingOwnerRepositoryId == Guid.Empty)
                            {
                                await TrySetPathfindingOwner(entity.Key, userId);
                            }
                            else
                            {
                                if (_ownershipChangeDates.TryGetValue(entity.Key, out var lastChangeDate))
                                {
                                    if (tickStartDate - lastChangeDate > ForceOwnerhshipChangeTimeout)
                                    {
                                        var sqrDistance = Vector3.GetSqrDistance(selfPosition, entity.Value);
                                        if (_changeOwnershipRequests.TryGetValue(entity.Key, out var request))
                                        {
                                            if (request.SqrMagnitude > sqrDistance)
                                            {
                                                _changeOwnershipRequests[entity.Key] = new ChangeOwnershipRequest(entity.Key, userId, sqrDistance);
                                            }
                                        }
                                        else
                                        {
                                            _changeOwnershipRequests[entity.Key] = new ChangeOwnershipRequest(entity.Key, userId, sqrDistance);
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.IfError()?.Message("Mob {mob} has owner but does not exist in _ownershipChangeDates", entity.Key).Write();
                                }
                            }
                        }
                    }

                    List<OuterRef<IEntity>> toSubscribe = null;
                    List<OuterRef<IEntity>> toUnsubscribe = null;

                    foreach (var oRef in _diffSet)
                    {
                        //either this is new visible entity
                        if (!_prevObjects.Contains(oRef))
                        {
                            if (toSubscribe == null)
                                toSubscribe = new List<OuterRef<IEntity>>();
                            toSubscribe.Add(oRef);
                        }
                        //or it is not visible now
                        else
                        {
                            var hasMobMovementEntityLockFree = EntitiesRepository.TryGetLockfree<IHasMobMovementSync>(oRef, ReplicationLevel.Master);
                            var pathFindingOwnerRepositoryId = hasMobMovementEntityLockFree?.MovementSync.PathFindingOwnerRepositoryIdRuntime;
                            if (toUnsubscribe == null)
                                toUnsubscribe = new List<OuterRef<IEntity>>();
                            toUnsubscribe.Add(oRef);
                            
                            await TryRemoveOwnership(pathFindingOwnerRepositoryId, userId, oRef);
                        }
                    }

                    if (toSubscribe != null)
                    {
                        if (toSubscribeDict == null)
                            toSubscribeDict = new Dictionary<Guid, ValueTuple<List<OuterRef<IEntity>>, VisibilitySubjectStatus>>();
                        toSubscribeDict.Add(userId, (toSubscribe, status));
                    }

                    if (toUnsubscribe != null)
                    {
                        if (toUnsubscribeDict == null)
                            toUnsubscribeDict = new Dictionary<Guid, ValueTuple<List<OuterRef<IEntity>>, VisibilitySubjectStatus>>();
                        toUnsubscribeDict.Add(userId, (toUnsubscribe, status));
                    }
                }

                if (_changeOwnershipRequests.Count > 0)
                {
                    foreach (var changeOwnershipRequest in _changeOwnershipRequests)
                    {
                        await TrySetPathfindingOwner(changeOwnershipRequest.Key, changeOwnershipRequest.Value.OwnerRepositoryId);
                    }
                    
                    _changeOwnershipRequests.Clear();
                }

                if (toSubscribeDict != null)
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        // TODO Сейчас возможна подписка на ентитю в Initialize стейте
                        // await Task.Delay(1000);
                        foreach (var toSubscribe in toSubscribeDict)
                            foreach (var oRef in toSubscribe.Value.Item1)
                            {
                                try
                                {
                                    await EntitiesRepository.SubscribeReplication(oRef.TypeId, oRef.Guid, toSubscribe.Key, ReplicationLevel.ClientBroadcast);
                                }
                                catch (Exception e)
                                {
                                    Logger.IfError()?.Message(e, "Visibility UpdateImpl Subscribe {0}", EntitiesRepository.Id).Write();
                                    toSubscribe.Value.Item2.Entities.TryRemove(oRef, out _);
                                }
                            }
                    }, EntitiesRepository);

                if (toUnsubscribeDict != null)
                    AsyncUtils.RunAsyncTask(async () =>
                    {
                        foreach (var toUnsubscribe in toUnsubscribeDict)
                            foreach (var oRef in toUnsubscribe.Value.Item1)
                            {
                                try
                                {
                                    //TODO переделать на UnsubscribeReplicationBatch
                                    await EntitiesRepository.UnsubscribeReplication(oRef.TypeId, oRef.Guid, toUnsubscribe.Key, ReplicationLevel.ClientBroadcast);
                                }
                                catch (Exception e)
                                {
                                    Logger.IfError()?.Message(e, "Visibility UpdateImpl Unsubscribe {0}", EntitiesRepository.Id).Write();
                                    toUnsubscribe.Value.Item2.Entities.TryAdd(oRef, Vector3.Default);
                                }
                            }
                    }, EntitiesRepository);
                _updateTime.Stop();
                if (_updateTime.Elapsed > UpdateTimeBudget)
                {
                    Logger.IfWarn()?.Message("Visibility enity update time {time}", _updateTime.Elapsed).Write();
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Visibility UpdateImpl exception").Write();;
                throw;
            }

            return true;
        }

        private async Task TryRemoveOwnership(Guid? pathFindingOwnerRepositoryId, Guid userId, OuterRef<IEntity> entityRef)
        {
            if (pathFindingOwnerRepositoryId == userId)
            {
                using (var wrapper = await EntitiesRepository.Get(entityRef))
                {
                    if (wrapper.TryGet<IHasMobMovementSync>(entityRef.TypeId, entityRef.Guid, out var hasMobMovementEntity))
                    {
                        await hasMobMovementEntity.MovementSync.SetPathFindingOwnerRepositoryId(default);
                    }
                }
            }
        }

        private  async Task TrySetPathfindingOwner(OuterRef<IEntity> entityRef, Guid ownerRepositoryId)
        {
            using (var wrapper = await EntitiesRepository.Get(entityRef))
            {
                if (wrapper.TryGet<IHasMobMovementSync>(entityRef.TypeId, entityRef.Guid, out var hasMobMovementEntity))
                {
                    if (hasMobMovementEntity.MovementSync.PathFindingOwnerRepositoryIdRuntime != ownerRepositoryId)
                    {
                        _ownershipChangeDates[entityRef] = DateTime.UtcNow;
                        await hasMobMovementEntity.MovementSync.SetPathFindingOwnerRepositoryId(ownerRepositoryId);
                    }
                }
            }
        }

        private void DelayUpdate(Guid entityId, float delay, CancellationToken token, IEntitiesRepository repository)
        {
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(delay), token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                if (token.IsCancellationRequested)
                    return;

                try
                {
                    using (var wrapper = await ((IEntitiesRepositoryExtension)repository).GetExclusive<IVisibilityEntity>(entityId))
                    {
                        var entity = wrapper?.Get<IVisibilityEntity>(entityId);
                        if (entity != null)
                            await entity.Update();
                    }
                }
                finally
                {
                    DelayUpdate(entityId, delay, token, repository);
                }
            }, repository);
        }

        public async Task<bool> ForceUnsubscribeAllImpl(Guid user)
        {
            if (_visibilityStatuses.TryGetValue(user, out var status))
            {
                foreach(var oRef in status.Entities)
                    await EntitiesRepository.UnsubscribeReplication(oRef.Key.TypeId, oRef.Key.Guid, user, ReplicationLevel.ClientBroadcast);
            }
            return true;
        }
        
        private class ChangeOwnershipRequest
        {
            public OuterRef<IEntity> EntityRef { get; }
            
            public Guid OwnerRepositoryId { get; }
            
            public float SqrMagnitude { get; }

            public ChangeOwnershipRequest(OuterRef<IEntity> entityRef, Guid ownerRepositoryId, float sqrMagnitude)
            {
                EntityRef = entityRef;
                OwnerRepositoryId = ownerRepositoryId;
                SqrMagnitude = sqrMagnitude;
            }
        }
    }

    public class VisibilitySubjectStatus
    {
        public ConcurrentDictionary<OuterRef<IEntity>, Vector3> Entities = new ConcurrentDictionary<OuterRef<IEntity>, Vector3>();
    }
}
