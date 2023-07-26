using System.Collections.Generic;
using GeneratedCode.Manual.Repositories;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace SharedCode.Refs.Operations
{
    public readonly struct EntityRefOperationResult
    {
        public EntityRefOperationResult(List<DeferredEntityModel> deferredEntities, bool replicationLevelChanged)
        {
            DeferredEntities = deferredEntities;
            ReplicationLevelChanged = replicationLevelChanged;
        }

        public List<DeferredEntityModel> DeferredEntities { get; }

        public bool ReplicationLevelChanged { get; }
    }

    public static class Extensions
    {
        public static EntityRefOperationResult? Combine(this EntityRefOperationResult? operationResult1, EntityRefOperationResult? operationResult2)
        {
            if (operationResult1 == null && operationResult2 == null)
            {
                return null;
            }
            
            List<DeferredEntityModel> deferredEntities = null;
            if (operationResult1?.DeferredEntities != null)
            {
                deferredEntities = operationResult1?.DeferredEntities;
            }

            if (operationResult2?.DeferredEntities != null)
            {
                if (deferredEntities != null)
                {
                    deferredEntities.AddRange(operationResult2.Value.DeferredEntities);
                }
                else
                {
                    deferredEntities = operationResult2.Value.DeferredEntities;
                }
            }

            var replicationLevelChanged = (operationResult1?.ReplicationLevelChanged ?? false) || (operationResult2?.ReplicationLevelChanged ?? false);

            return new EntityRefOperationResult(deferredEntities, replicationLevelChanged);
        }
    }
}