using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;

namespace SharedCode.Entities
{
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    [GenerateDeltaObjectCode]
    public interface IWorldBench : IEntity, IMountable, IHasOwner, IHasOpenMechanics, IDatabasedMapedEntity, IHasStatsEngine, IHasHealth, IOpenable
    {
        
    }
}