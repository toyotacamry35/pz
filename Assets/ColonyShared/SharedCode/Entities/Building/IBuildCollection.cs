using SharedCode.DeltaObjects.Building;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.ChainCalls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedCode.Entities.Building
{
    public interface IBuildCollection
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> ContainsKey(BuildType type, Guid elementId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<IPositionedBuild> TryGetValue(BuildType type, Guid elementId);

        ///////////////////////////////////////////////////////////////////////////////////////////
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> CheckStructure(BuildType type, IPositionedBuild build);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> AddStructure(BuildType type, IPositionedBuild build);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<List<KeyValuePair<BuildType, Guid>>> RemoveStructure(BuildType type, Guid elementId);

        ///////////////////////////////////////////////////////////////////////////////////////////
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> AddElement(BuildType type, IPositionedBuild build);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> RemoveElements(List<KeyValuePair<BuildType, Guid>> elements);

        ///////////////////////////////////////////////////////////////////////////////////////////
        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<ChainCancellationToken> StartChain(BuildType type, float tick, int count, Guid elementId);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> CancelChain(BuildType type, ChainCancellationToken token);

        [EntityMethodCallType(EntityMethodCallType.Immutable)]
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> RemoveChain(List<KeyValuePair<BuildType, Guid>> elements);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> OnProgress(BuildType type, Guid elementId);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> RemoveDelay(List<KeyValuePair<BuildType, Guid>> elements);
    }
}
