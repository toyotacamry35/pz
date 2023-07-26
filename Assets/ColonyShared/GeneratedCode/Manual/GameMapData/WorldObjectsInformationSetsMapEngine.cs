using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Entities.GameMapData;
using GeneratedCode.Repositories;
using NLog;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectsInformationSetsMapEngine
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<Dictionary<WorldObjectInformationSetDef, OuterRef>> SubscribeImpl(List<WorldObjectInformationSetDef> worldObjectSetsDef, Guid repositoryId)
        {
            var result = new Dictionary<WorldObjectInformationSetDef, OuterRef>();
            foreach (var worldObjectSetDef in worldObjectSetsDef)
            {
                var outerRef = await getOrAddWorldObjectSet(worldObjectSetDef);
                if (outerRef == default(OuterRef))
                {
                    Logger.IfError()?.Message("Error on get worldObjectSet {0}", worldObjectSetDef).Write();
                    continue;
                }

                result.Add(worldObjectSetDef, outerRef);
                await EntitiesRepository.SubscribeReplication(outerRef.TypeId, outerRef.Guid, repositoryId, ReplicationLevel.ClientFull);
            }
            return result;
        }

        private async Task<OuterRef> getOrAddWorldObjectSet(WorldObjectInformationSetDef def)
        {
            OuterRef setOuterRef;
            if (!WorldObjectInformationSets.TryGetValue(def, out setOuterRef))
            {
                var typeId = ReplicaTypeRegistry.GetIdByType(ReplicaTypeRegistry.GetTypeByName(def.EntityTypeName));
                var newWorldObjectSetRef = await EntitiesRepository.Create(typeId, Guid.NewGuid(), (entity) =>
                    {
                        ((IHasWorldObjectsInformationDataSetEngine) entity).WorldObjectsInformationDataSetEngine.WorldObjectInformationSetDef = def;
                        return Task.CompletedTask;
                    });
                setOuterRef = new OuterRef(newWorldObjectSetRef.Id, newWorldObjectSetRef.TypeId);
                WorldObjectInformationSets.Add(def, setOuterRef);
                var parentEntityTypeId = this.parentEntity.TypeId;
                var parentEntityId = this.parentEntity.Id;
                var repository = EntitiesRepository;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await repository.Get(parentEntityTypeId, parentEntityId))
                    {
                        var worldSpaceParentEntity = wrapper.Get<IHasWorldObjectsInformationSetsMapEngine>(parentEntityTypeId, parentEntityId);
                        var result = await worldSpaceParentEntity.RegisterWorldObjectsInNewInformationSet(setOuterRef);
                        if (!result)
                            Logger.IfError()?.Message("WorldObjectSet {0} ref {1} registration error", def, setOuterRef).Write();
                    }
                });
            }
            return setOuterRef;
        }

        public async Task<bool> UnsubscribeImpl(List<WorldObjectInformationSetDef> worldObjectSetsDef, Guid repositoryId)
        {
            foreach (var worldObjectSetDef in worldObjectSetsDef)
            {
                OuterRef outerRef;
                if (!WorldObjectInformationSets.TryGetValue(worldObjectSetDef, out outerRef))
                {
                    Logger.IfError()?.Message("Cant unsusbscribe worldObjectSet {0} - not exist", worldObjectSetDef).Write();
                    continue;
                }

                await EntitiesRepository.UnsubscribeReplication(outerRef.TypeId, outerRef.Guid, repositoryId, ReplicationLevel.ClientFull);
            }

            return true;
        }

        public async Task<bool> AddWorldObjectImpl(OuterRef worldObjectRef)
        {
            foreach (var worldObjectSetRef in this.WorldObjectInformationSets.Values)
            {
                using (var wrapper =  await EntitiesRepository.Get(worldObjectSetRef.TypeId, worldObjectSetRef.Guid))
                {
                    var entity = wrapper.Get<IHasWorldObjectsInformationDataSetEngine>(worldObjectSetRef.TypeId, worldObjectSetRef.Guid);
                    if (entity == null)
                    {
                        Logger.IfError()?.Message("WorldObjectSetEntity {0} not found", worldObjectSetRef).Write();
                        return false;
                    }

                    var def = entity.WorldObjectsInformationDataSetEngine.WorldObjectInformationSetDef;
                    if (def.ObjectsTypeFilter == null || def.ObjectsTypeFilter.Count == 0)
                    {
                        Logger.IfError()?.Message("RegisterWorldObjectsInNewInformationSetImpl {0} ObjectsTypeFilter is null or empty!!!", def).Write();
                        return false; // Требуем от дизайнеров чтобы типы указывали явно, иначе они забьют и будет проверятся на предикаты вся туева хуча кактусов, камней и т.д.
                    }

                    if (!def.ObjectsTypeFilter.Contains(ReplicaTypeRegistry.GetTypeById(worldObjectRef.TypeId).Name))
                        return false;

                    await entity.WorldObjectsInformationDataSetEngine.RegisterWorldObjectsInNewInformationSet(new EntityId(worldObjectSetRef.TypeId, worldObjectSetRef.Guid));
                }
            }

            return true;
        }

        public async Task<bool> RemoveWorldObjectImpl(OuterRef worldObjectRef)
        {
            foreach (var worldObjectSetRef in this.WorldObjectInformationSets.Values)
            {
                using (var wrapper = await EntitiesRepository.Get(worldObjectSetRef.TypeId, worldObjectSetRef.Guid))
                {
                    var entity = wrapper.Get<IHasWorldObjectsInformationDataSetEngine>(worldObjectSetRef.TypeId, worldObjectSetRef.Guid);
                    if (entity == null)
                    {
                        Logger.IfError()?.Message("WorldObjectSetEntity {0} not found", worldObjectSetRef).Write();
                        return false;
                    }

                    var def = entity.WorldObjectsInformationDataSetEngine.WorldObjectInformationSetDef;
                    if (def.ObjectsTypeFilter == null || def.ObjectsTypeFilter.Count == 0)
                    {
                        Logger.IfError()?.Message("RegisterWorldObjectsInNewInformationSetImpl {0} ObjectsTypeFilter is null or empty!!!", def).Write();
                        return false; // Требуем от дизайнеров чтобы типы указывали явно, иначе они забьют и будет проверятся на предикаты вся туева хуча кактусов, камней и т.д.
                    }

                    if (!def.ObjectsTypeFilter.Contains(ReplicaTypeRegistry.GetTypeById(worldObjectRef.TypeId).Name))
                        return false;

                    await entity.WorldObjectsInformationDataSetEngine.UnregisterWorldObjectsInNewInformationSet(new EntityId(worldObjectSetRef.TypeId, worldObjectSetRef.Guid));
                }
            }

            return true;
        }
    }
}
