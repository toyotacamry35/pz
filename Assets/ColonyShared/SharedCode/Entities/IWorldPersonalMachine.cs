using Assets.ColonyShared.SharedCode.Entities;
using GeneratorAnnotations;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.Wizardry;

namespace SharedCode.Entities
{
    [GenerateDeltaObjectCode]
    [DatabaseSaveType(DatabaseSaveType.Explicit)]
    public interface IWorldPersonalMachine : IEntity, IMountable, IDatabasedMapedEntity, IHasStatsEngine, IHasHealth, IHasWizardEntity, IHasOpenMechanics, IOpenable, ICanBeActive
    {
    }
}