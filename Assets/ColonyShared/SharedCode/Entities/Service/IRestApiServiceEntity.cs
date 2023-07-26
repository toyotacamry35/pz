using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratorAnnotations;
using SharedCode.Cloud;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;

namespace SharedCode.Entities.Service
{
    [GenerateDeltaObjectCode]
    [EntityService(replicateToNodeType: CloudNodeType.None, addedByDefaultToNodeType: CloudNodeType.None)]
    public interface IRestApiServiceEntity : IEntity
    {
 
    }
}
