using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GeneratedCode.Repositories;
using SharedCode.Entities;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectsInformationDataSetEngine
    {
        public Task RegisterWorldObjectsInNewInformationSetImpl(EntityId worldObjectId)
        {
            //фильтруем мировые объекты по предикатам
            if (WorldObjectInformationSetDef.PredicateFilter != null)
            {
                //TODO отфильтровать по предикатам
                //worldObjectsIds.RemoveAll();
            }

            //фильтруем мировые объекты по спелу
            if (WorldObjectInformationSetDef.SpellFilter != null)
            {
                //TODO отфильтровать по спелу
                //worldObjectsIds.RemoveAll();
            }

            return ((IHasWorldObjectsInformationDataSetEngine)parentEntity).RegisterWorldObjectsInNewInformationSet(worldObjectId);
        }

        public Task RegisterWorldObjectsInNewInformationSetBatchImpl(List<EntityId> worldObjectsIds)
        {
            //фильтруем мировые объекты по предикатам
            if (WorldObjectInformationSetDef.PredicateFilter != null)
            {
                //TODO отфильтровать по предикатам
                //worldObjectsIds.RemoveAll();
            }

            //фильтруем мировые объекты по спелу
            if (WorldObjectInformationSetDef.SpellFilter != null)
            {
                //TODO отфильтровать по спелу
                //worldObjectsIds.RemoveAll();
            }

            return ((IHasWorldObjectsInformationDataSetEngine)parentEntity).RegisterWorldObjectsInNewInformationSetBatch(worldObjectsIds);
        }

        public async Task UnregisterWorldObjectsInNewInformationSetImpl(EntityId worldObjectId)
        {
            var contains = await ((IHasWorldObjectsInformationDataSetEngine) parentEntity).ContainsWorldObjectInformation(worldObjectId);
            if (!contains)
                return;

            await ((IHasWorldObjectsInformationDataSetEngine)parentEntity).UnregisterWorldObjectsInNewInformationSet(worldObjectId);
        }

        public async Task UnregisterWorldObjectsInNewInformationSetBatchImpl(List<EntityId> worldObjectsIds)
        {
            var containsList = await ((IHasWorldObjectsInformationDataSetEngine)parentEntity).ContainsWorldObjectInformationList(worldObjectsIds);
            if (containsList == null || containsList.Count == 0)
                return;
            
            await ((IHasWorldObjectsInformationDataSetEngine)parentEntity).UnregisterWorldObjectsInNewInformationSetBatch(containsList);
        }
    }
}
