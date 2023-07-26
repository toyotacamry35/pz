using System;
using System.Threading.Tasks;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasCorpseSpawner
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        ICorpseSpawner CorpseSpawner { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface ICorpseSpawner : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.Master)]
        Task SpawnCorpse(Guid entityId, int entityTypeId, PositionRotation corpsePlace);
    }
}
