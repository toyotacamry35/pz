using Assets.Src.RubiconAI;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.MapSystem;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.MapSystem;
using SharedCode.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldSpaced : IWorldSpacedImplementRemoteMethods, IHookOnInit, IHookOnDestroy, IHookOnUnload, IHookOnDatabaseLoad
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public async Task<bool> AssignToWorldSpaceImpl(OuterRef<IWorldSpaceServiceEntity> ownWorldSpace)
        {
            OwnWorldSpace = ownWorldSpace;
            var wsGuid = EntitiesRepository.Id;
            if (OwnWorldSpace.Guid != Guid.Empty)
                wsGuid = OwnWorldSpace.Guid;
            if (!ReplicatedObjectsWhitelist.ShouldReplicateToAnyoneEnMasse(parentEntity))
                return true;
            
            return true;
        }

        public Task OnInit()
        {
            if (OwnWorldSpace == default)
                return Task.CompletedTask;
            SubscribeSceneRemovingFallback();
            return TryAddAsSavable();
        }

        public Task OnDatabaseLoad()
        {
            if (OwnWorldSpace == default)
                return Task.CompletedTask;
            SubscribeSceneRemovingFallback();
            return Task.CompletedTask;
        }

        public Task OnDestroy()
        {
            if (OwnWorldSpace == default)
                return Task.CompletedTask;
            UnsubscribeSceneRemoving();
            return TryRemoveAsSavable();
        }

        public Task OnUnload()
        {
            if (OwnWorldSpace == default)
                return Task.CompletedTask;
            UnsubscribeSceneRemoving();
            return Task.CompletedTask;
        }

        private void SubscribeSceneRemovingFallback()
        {
            var se = ((IScenicEntity) parentEntity);
            var mapOwner = se.MapOwner;
            AIWorld.GetWorld(OwnWorldSpace.Guid).Attach(new OuterRef<IEntity>(parentEntity));
            EntitiesRepository.SubscribeOnDestroyOrUnload(mapOwner.OwnerScene.TypeId, mapOwner.OwnerScene.Guid, OnSceneVanished);
        }

        private async Task TryAddAsSavable()
        {
            if (!(parentEntity is IDatabasedMapedEntity))
                return;
            
            var se = ((IScenicEntity) parentEntity);
            var mapOwner = se.MapOwner;
            var staticIdFromExport = se.StaticIdFromExport;
            using (var ent = await EntitiesRepository.Get<ISceneEntity>(mapOwner.OwnerSceneId))
            {
                var sceneEnt = ent.Get<ISceneEntityServer>(mapOwner.OwnerSceneId);
                await sceneEnt.SetLoadableObj(new OuterRef<IEntity>(parentEntity), staticIdFromExport, default);
            }
        }

        private void UnsubscribeSceneRemoving()
        {
            ((IHasWorldSpaced) parentEntity).WorldSpaced.Destroyed = true;
            var se = ((IScenicEntity) parentEntity);
            var mapOwner = se.MapOwner;
            AIWorld.GetWorld(OwnWorldSpace.Guid).Detach(new OuterRef<IEntity>(parentEntity));
            EntitiesRepository.UnsubscribeOnDestroyOrUnload(mapOwner.OwnerScene.TypeId, mapOwner.OwnerScene.Guid, OnSceneVanished);
        }
        
        private async Task TryRemoveAsSavable()
        {
            if (!(parentEntity is IDatabasedMapedEntity))
                return;
            
            var se = ((IScenicEntity) parentEntity);
            var mapOwner = se.MapOwner;
            using (var ent = await EntitiesRepository.Get<ISceneEntity>(mapOwner.OwnerSceneId))
            {
                var sceneEnt = ent.Get<ISceneEntityServer>(mapOwner.OwnerSceneId);
                await sceneEnt.RemoveObject(new OuterRef<IEntity>(parentEntity));
            }
        }

        private async Task OnSceneVanished(int typeId, Guid guid, IEntity entity)
        {
            await EntitiesRepository.Destroy(parentEntity.TypeId, parentEntity.Id, true);
        }
    }
}
