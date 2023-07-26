using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using Entities.GameMapData;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using ResourceSystem.Utils;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Serializers;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectInformationSetsEngine
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetImpl(WorldObjectInformationClientSubSetDef subSetDef)
        {
            int counter;
            if (WorldObjectInformationRefsCounter.TryGetValue(subSetDef, out counter))
                WorldObjectInformationRefsCounter[subSetDef] = counter + 1;
            else
            {
                WorldObjectInformationRefsCounter.Add(subSetDef, 1);

                var hasWorldSpaced = (IHasWorldSpaced) parentEntity;
                var worldSpaceEntityRef = hasWorldSpaced.WorldSpaced.OwnWorldSpace;

                var userId = Guid.Empty;
                using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
                {
                    var loginInternalServiceEntity = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                    userId = await loginInternalServiceEntity.GetUserIdByCharacterId(parentEntity.Id);

                }

                using (var wrapper = await EntitiesRepository.Get(worldSpaceEntityRef.TypeId, worldSpaceEntityRef.Guid))
                {
                    var wsEntity = wrapper.Get<IHasWorldObjectsInformationSetsMapEngineServer>(worldSpaceEntityRef.TypeId, worldSpaceEntityRef.Guid, ReplicationLevel.Server);
                    var result = await wsEntity.WorldObjectsInformationSetsMapEngine.Subscribe(new List<WorldObjectInformationSetDef>() { subSetDef.DataSet.Target }, userId);
                    foreach(var pair in result)
                        if (pair.Key == subSetDef.DataSet.Target)
                            CurrentWorldObjectInformationRefs.Add(subSetDef, pair.Value);
                }
            }

            return AddWorldObjectInformationSubSetResult.Success;
        }

        public async Task<RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetImpl(WorldObjectInformationClientSubSetDef subSetDef)
        {
            int counter;
            if (!WorldObjectInformationRefsCounter.TryGetValue(subSetDef, out counter))
            {
                Logger.IfError()?.Message("RemoveWorldObjectInformationSubSetImpl {0} id:{1} subset def {2} not found", subSetDef, parentEntity.TypeName, parentEntity.Id).Write();
                return RemoveWorldObjectInformationSubSetResult.ErrorNotFound;
            }

            counter--;
            if (counter > 0)
                WorldObjectInformationRefsCounter[subSetDef] = counter;
            else
            {
                WorldObjectInformationRefsCounter.Remove(subSetDef);
                CurrentWorldObjectInformationRefs.Remove(subSetDef);

                var hasWorldSpaced = (IHasWorldSpaced)parentEntity;
                var worldSpaceEntityRef = hasWorldSpaced.WorldSpaced.OwnWorldSpace;

                var userId = Guid.Empty;
                using (var wrapper = await EntitiesRepository.GetFirstService<ILoginInternalServiceEntityServer>())
                {
                    var loginInternalServiceEntity = wrapper.GetFirstService<ILoginInternalServiceEntityServer>();
                    userId = await loginInternalServiceEntity.GetUserIdByCharacterId(parentEntity.Id);
                }

                using (var wrapper = await EntitiesRepository.Get(worldSpaceEntityRef.TypeId, worldSpaceEntityRef.Guid))
                {
                    var wsEntity = wrapper.Get<IHasWorldObjectsInformationSetsMapEngineServer>(worldSpaceEntityRef.TypeId, worldSpaceEntityRef.Guid, ReplicationLevel.Server);
                    var result = await wsEntity.WorldObjectsInformationSetsMapEngine.Unsubscribe(new List<WorldObjectInformationSetDef>() { subSetDef.DataSet.Target }, userId);
                }
            }

            return RemoveWorldObjectInformationSubSetResult.Success;
        }

        public Task<AddWorldObjectInformationSubSetResult> AddWorldObjectInformationSubSetCheatImpl(WorldObjectInformationClientSubSetDef subSetDef)
        {
            return AddWorldObjectInformationSubSet(subSetDef);
        }

        public Task<RemoveWorldObjectInformationSubSetResult> RemoveWorldObjectInformationSubSetCheatImpl(WorldObjectInformationClientSubSetDef subSetDef)
        {
            return RemoveWorldObjectInformationSubSet(subSetDef);
        }
    }
}
