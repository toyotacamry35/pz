using GeneratorAnnotations;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ColonyShared.SharedCode.Entities
{
    [EntityService]
    [GenerateDeltaObjectCode]
    public interface ISpawnDaemonsInfoCollector : IEntity
    {
        Task<bool> Update();
    }
}
