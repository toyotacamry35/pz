using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.MovementSync;
using GeneratorAnnotations;

namespace SharedCode.AI
{
    [GenerateDeltaObjectCode]
    public interface ISpatialTriggerEntity : IEntity, IEntityObject, IWorldObject, IHasWizardEntity, IHasSimpleMovementSync
    {
    }
}
