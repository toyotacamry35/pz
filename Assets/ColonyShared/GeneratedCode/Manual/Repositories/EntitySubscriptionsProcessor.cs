using System;
using System.Collections.Generic;
using GeneratedCode.Manual.Repositories;
using NLog;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using SharedCode.Refs;

namespace GeneratedCode.Repositories
{
    public interface IEntitySubscriptionsProcessor
    {
        void Process(
            EntitiesRepository repository,
            Queue<EntityRefSubscriptionsChange> entitiesToProcess,
            Dictionary<Guid, List<DeferredEntityModel>> newLinkedEntities,
            List<(long level, IEntityRef entityRef)> linkedEntitiesList,
            List<EntitiesRepository.RepositoryReplicationInfo> rootEntityReplicationRepositories,
            long maxSubscriptionsMask,
            bool subscribeToDatabase,
            EntityRefSubscriptionsChange processingEntity,
            BaseEntity entity,
            ref EntityCollections collections);
    }

    public class EntitySubscriptionsProcessor : IEntitySubscriptionsProcessor
    {
        public void Process(
            EntitiesRepository repository,
            Queue<EntityRefSubscriptionsChange> entitiesToProcess,
            Dictionary<Guid, List<DeferredEntityModel>> newLinkedEntities,
            List<(long level, IEntityRef entityRef)> linkedEntitiesList,
            List<EntitiesRepository.RepositoryReplicationInfo> rootEntityReplicationRepositories,
            long maxSubscriptionsMask,
            bool subscribeToDatabase,
            EntityRefSubscriptionsChange processingEntity,
            BaseEntity entity,
            ref EntityCollections collections)
        {
            entity.GetAllLinkedEntities(maxSubscriptionsMask, linkedEntitiesList, 0, subscribeToDatabase);
            foreach (var linkedEntity in linkedEntitiesList)
            {
                var linkedEntityOldReachabilityLevel = processingEntity.OldReachabilityReplicationLevel == null
                    ? (ReplicationLevel?) null
                    : ((ReplicationLevel) linkedEntity.level | processingEntity.OldReachabilityReplicationLevel.Value);

                var linkedEntityNewReachabilityLevel = processingEntity.NewReachabilityReplicationLevel == null
                    ? (ReplicationLevel?) null
                    : ((ReplicationLevel) linkedEntity.level | processingEntity.NewReachabilityReplicationLevel.Value);

                var subscriptionsChanges = repository.GetSubscriptionsChanges(
                    rootEntityReplicationRepositories,
                    linkedEntityOldReachabilityLevel,
                    linkedEntityNewReachabilityLevel);
                if (subscriptionsChanges != null && subscriptionsChanges.Count != 0)
                {
                    foreach (var subscriptionsChange in subscriptionsChanges)
                    {
                        if (subscriptionsChange.NewSubscriber)
                        {
                            if (!newLinkedEntities.TryGetValue(subscriptionsChange.RepositoryId, out var repositoryNewLinkedEntities))
                            {
                                repositoryNewLinkedEntities = new List<DeferredEntityModel>();
                                newLinkedEntities[subscriptionsChange.RepositoryId] = repositoryNewLinkedEntities;
                            }

                            // we are assuming that entity will change level to (subscriptionsChange.ReplicationMask)
                            // but entity could have itself subscribers and already had this level or even bigger
                            // this case is handling at receiver side with checking existing entities and they levels
                            repositoryNewLinkedEntities.Add(new DeferredEntityModel(
                                new OuterRef(linkedEntity.entityRef.Id, linkedEntity.entityRef.TypeId),
                                subscriptionsChange.ReplicationMask));
                        }
                    }

                    entitiesToProcess.Enqueue(new EntityRefSubscriptionsChange(
                        linkedEntityOldReachabilityLevel,
                        linkedEntityNewReachabilityLevel,
                        linkedEntity.entityRef,
                        subscriptionsChanges));
                }
            }

            // applying changes that parent entity calculated for us
            foreach (var subscriptionsChange in processingEntity.RepositoriesSubscriptions)
            {
                var entityExt = (IEntityExt) entity;

                // entityExt.ReplicateRepositoryIds could be deleted after node disconnected
                // it is possible that we are creating new ReplicateRefsContainer() for subscriptionsChange.RepositoryId that doesn't exists anymore
                var replicateRefsContainer =
                    entityExt.ReplicateRepositoryIds.GetOrAdd(subscriptionsChange.RepositoryId, id => new ReplicateRefsContainer());
                var newReplicationLevel = (ReplicationLevel)replicateRefsContainer.GetChangedReplicationMask(subscriptionsChange.SubscriptionsChanges);
                var oldReplicationLevel = (ReplicationLevel) replicateRefsContainer.GetReplicationMask();

                if (newReplicationLevel > oldReplicationLevel)
                {
                    var subscribeMask = ((long)oldReplicationLevel ^ (long)newReplicationLevel) & (long)newReplicationLevel;
                    entityExt.CreateReplicationSetIfNotExists(newReplicationLevel);
                    ((IEntityRefExt) processingEntity.Value).SerializeAndSaveNewSubscriber(newReplicationLevel, oldReplicationLevel, subscribeMask);
                }
                else
                {
                    // we don't need to serialize anything because we just send downgrade in this case
                }

                ((IEntitiesRepositoryExtension)repository).UpdateSubscriptions(
                    processingEntity.Value.TypeId,
                    processingEntity.Value.Id,
                    subscriptionsChange.RepositoryId,
                    subscriptionsChange.SubscriptionsChanges,
                    newLinkedEntities,
                    ref collections);
            }
        }
    }
}