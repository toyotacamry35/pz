using ProtoBuf;
using SharedCode.DeltaObjects.Building;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedCode.Entities.Building
{
    [ProtoContract]
    public class IPositionedBuildWrapper
    {
        [ProtoMember(1, AsReference = true, DynamicType = true)]
        public IPositionedBuild PositionedBuild { get; set; }
    }

    public interface IHasBuildPlace
    {
        [ReplicationLevel(ReplicationLevel.Server)]
        IBuildPlace BuildPlace { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IBuildPlace : IDeltaObject
    {
        // - Delayed parts removal ----------------------------------------------------------------
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> RemoveDelay(List<KeyValuePair<BuildType, Guid>> elements);

        // - Element manipulation -----------------------------------------------------------------
        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> Check(BuildType type, IPositionedBuildWrapper buildWrapper);

        [ReplicationLevel(ReplicationLevel.Master)]
        Task<bool> Start(BuildType type, IPositionedBuildWrapper buildWrapper);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<bool> OnProgress(BuildType type, Guid elementId);

        [ReplicationLevel(ReplicationLevel.Server)]
        Task<OperationResultEx> Operate(BuildType type, Guid callerId, Guid elementId, OperationData data);
    }
}
