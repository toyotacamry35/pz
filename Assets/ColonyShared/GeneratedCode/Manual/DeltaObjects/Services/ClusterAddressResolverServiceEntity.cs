using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class ClusterAddressResolverServiceEntity
    {
        private ConcurrentDictionary<int, ConcurrentDictionary<Guid, Guid>> EntityAddresses { get; } = new ConcurrentDictionary<int, ConcurrentDictionary<Guid, Guid>>();

        public ValueTask<Guid> GetEntityAddressRepositoryIdImpl(int typeId, Guid entityId)
        {
            if (!EntityAddresses.TryGetValue(typeId, out var dict))
                return new ValueTask<Guid>(Guid.Empty);

            Guid repositoryId;
            if (!dict.TryGetValue(entityId, out repositoryId))
                return new ValueTask<Guid>(Guid.Empty);

            return new ValueTask<Guid>(repositoryId);
        }

        public ValueTask<IReadOnlyList<(Guid entityId, Guid repoId)>> GetAllEntitiesByTypeImpl(int typeId)
        {
            if (!EntityAddresses.TryGetValue(typeId, out var dict))
                return new ValueTask<IReadOnlyList<(Guid entityId, Guid repoId)>>(Array.Empty<(Guid entityId, Guid repoId)>());

            return new ValueTask<IReadOnlyList<(Guid entityId, Guid repoId)>>(dict.Select(v => (v.Key, v.Value)).ToArray());
        }


        public Task SetEntityAddressRepositoryIdImpl(int typeId, Guid entityId, Guid repositoryId)
        {
            ConcurrentDictionary<Guid, Guid> dict;
            if (!EntityAddresses.TryGetValue(typeId, out dict))
            {
                EntityAddresses.TryAdd(typeId, new ConcurrentDictionary<Guid, Guid>());
                EntityAddresses.TryGetValue(typeId, out dict);
            }

            if (dict != null)
                dict[entityId] = repositoryId;

            return Task.CompletedTask;
        }

        public Task RemoveEntityAddressRepositoryIdImpl(int typeId, Guid entityId)
        {
            ConcurrentDictionary<Guid, Guid> dict;
            if (!EntityAddresses.TryGetValue(typeId, out dict))
                return Task.CompletedTask;

            Guid repositoryId;
            dict.TryRemove(entityId, out repositoryId);

            return Task.CompletedTask;
        }

        public Task SetEntitiesAddressRepositoryIdImpl(Dictionary<int, Guid> entities, Guid repositoryId)
        {
            if (entities != null)
                foreach (var pair in entities)
                {
                    var typeId = pair.Key;
                    var entityId = pair.Value;
                    ConcurrentDictionary<Guid, Guid> dict;
                    if (!EntityAddresses.TryGetValue(typeId, out dict))
                    {
                        EntityAddresses.TryAdd(typeId, new ConcurrentDictionary<Guid, Guid>());
                        EntityAddresses.TryGetValue(typeId, out dict);
                    }

                    if (dict != null)
                        dict[entityId] = repositoryId;
                }

            return Task.CompletedTask;
        }
    }
}
