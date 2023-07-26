using ColonyShared.GeneratedCode.Shared.Aspects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Shared
{
    public static class ClusterCommands
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Task<Guid> GetWorldBoxIdToDrop(Vector3 position, Guid characterOwnerId, Guid worldSpaceId, IEntitiesRepository repo)
        {
            if (Constants.WorldConstants == null)
                return Task.FromResult<Guid>(Guid.Empty);
            Dictionary<OuterRef<IEntity>, Vector3> entities = new Dictionary<OuterRef<IEntity>, Vector3>();
            VisibilityGrid.Get(worldSpaceId, repo)?.SampleDataFor<SimpleObjectData, CharacterMovementState>(
                new OuterRef<IEntity>() { TypeId = ReplicaTypeRegistry.GetIdByType(typeof(IWorldCharacter)), Guid = characterOwnerId }, entities);

            var boxes = entities.Where(x => x.Key.TypeId == ReplicaTypeRegistry.GetIdByType(typeof(IWorldBox))).ToArray();
            var nearBox = boxes
                            .Where(x => Vector3.GetDistance(position, x.Value) <= Constants.WorldConstants.BoxInteractDistance)
                            .OrderBy(x => Vector3.GetSqrDistance(position, x.Value)).FirstOrDefault();

            if(Logger.IsDebugEnabled)
                Logger.IfDebug()?.Message($"GetWorldBoxIdToDrop. boxes = {string.Join(";", boxes.Select(v => v.Value))}. nearBox = {nearBox.Value}. nearBox.Key.TypeId = {nearBox.Key.TypeId}").Write();
            if (nearBox.Key.TypeId != 0)
            {
                return Task.FromResult<Guid>(nearBox.Key.Guid);
            }

            return Task.FromResult<Guid>(Guid.Empty);
        }

        public static async ValueTask<KeyValuePair<BaseItemResource, PropertyAddress>> GetActiveWeaponResource(Guid charId,
            IEntitiesRepository repository)
        {
            using (var worldCharacterWrapper = await repository.Get<IWorldCharacterClientBroadcast>(charId))
            {
                var worldCharacter = worldCharacterWrapper.Get<IWorldCharacterClientBroadcast>(charId);
                if (worldCharacter != null)
                    return ClusterHelpers.GetActiveWeaponResource(worldCharacter);
            }

            return default(KeyValuePair<BaseItemResource, PropertyAddress>);
        }
    }
}
