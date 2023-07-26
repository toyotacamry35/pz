using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Threading.Tasks;

namespace GeneratedCode.Telemetry
{
    [GenerateDeltaObjectCode]
    [EntityService(CloudNodeType.Server)]
    public interface ISessionHolderEntity : IEntity
    {
        IDeltaDictionary<Guid, Guid> SessionsByGuid { get; set; }

        ValueTask Register(Guid guid, Guid session);
        ValueTask Unregister(Guid guid);
    }
}
