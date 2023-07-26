using SharedCode.EntitySystem;
using System;

namespace SharedCode.DeltaObjects.Building
{
    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IPositionedAttachment : IDeltaObject, IPositionedBuild
    {
        [ReplicationLevel(ReplicationLevel.Always)]
        Guid ParentElementId { get; set; }
    }
}
