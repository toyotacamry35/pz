using NLog;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using System;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class BankCell
    {
        public async Task<OuterRef> GetOpenOuterRefImpl(OuterRef oref)
        {
            return new OuterRef(ParentEntityId, ParentTypeId);
        }

    }
}
