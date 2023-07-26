using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using Assets.ColonyShared.SharedCode.Entities;
using NLog;
using Assets.ColonyShared.SharedCode.Entities.Engine;
using Core.Environment.Logging.Extension;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using ResourceSystem.Utils;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Refs;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldPersonalMachineEngine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<OuterRef> GetOrAddMachineImpl(WorldPersonalMachineDef def, OuterRef key)
        {
            OuterRef result = default;

            if (!key.IsValid)
                return result;

            if (!WorldPersonalMachines.TryGetValue(key, out result))
            {
                if (def == null)
                    return result;

                var ce = await EntitiesRepository.Create<ICraftEngine>(Guid.NewGuid(), craftEngine =>
                {
                    craftEngine.OwnerInformation.Owner = new OuterRef<IEntity>(parentEntity.Id, parentEntity.TypeId);
                    craftEngine.UseOwnOutputContainer = true;
                    craftEngine.OutputContainer.Size = def.OutContainerSize;
                    craftEngine.MaxQueueSize = def.MaxQueueSize;
                    craftEngine.ResultContainerAddress = EntityPropertyResolver.GetPropertyAddress(craftEngine.OutputContainer);
                    craftEngine.CurrentWorkbenchType = def.WorkbenchType;
                    return Task.CompletedTask;
                });
                result = new OuterRef(ce.Id, ce.TypeId);
                WorldPersonalMachines.Add(key, result);
                await SubscribeOnDestroy(key);
            }
            else
            {
                IEntityRef loadedCraftEngine = await EntitiesRepository.Load(result.TypeId, result.Guid);
            }

            return result;
        }

        public Task RemoveMachineImpl(OuterRef key)
        {
            if (WorldPersonalMachines.ContainsKey(key))
            {
                WorldPersonalMachines.Remove(key);
            }

            return Task.CompletedTask;
        }

        private Task SubscribeOnDestroy(OuterRef outerRef)
        {
            EntitiesRepository.SubscribeOnDestroyOrUnload(outerRef.TypeId, outerRef.Guid, RemoveKeyOnDestroy);
            return Task.CompletedTask;
        }

        private async Task RemoveKeyOnDestroy(int typeId, Guid guid, IEntity entity)
        {
            var target = new OuterRef(guid, typeId);
            var oref = new OuterRef(parentEntity.Id, parentEntity.TypeId);
            using (var ent = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.ParentEntityId))
            {
                var pm = ent.Get<IHasWorldPersonalMachineEngineServer>(oref, ReplicationLevel.Server)?.worldPersonalMachineEngine;

                if (pm != null)
                    await pm.RemoveMachine(target);
                else
                    Logger.IfWarn()?.Message($"RemoveKeyOnDestroy: Unexpected - Target({oref}) is not {nameof(IHasWorldPersonalMachineEngine)}.").Write();
            }
        }
    }
}