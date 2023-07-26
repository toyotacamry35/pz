using System;
using SharedCode.Utils;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Entities.GameObjectEntities;
using GeneratorAnnotations;
using SharedCode.Entities.Service;
using SharedCode.MapSystem;
using Assets.ColonyShared.GeneratedCode.Regions;
using System.Linq;
using SharedCode.Aspects.Regions;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.Building;
using GeneratedCode.DeltaObjects;
// костры, верстаки
namespace SharedCode.Entities.Engine
{
    [GenerateDeltaObjectCode]
    public interface IBuildingEngine : IEntity, IHasOwner
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        Task<BuildOperationResult> Build(PropertyAddress address, int slodIds, Vector3 position, Quaternion rotation);
    }

    [Flags]
    public enum BuildOperationResult
    {
        None = 0x00,
        SuccessBuild = 0x01,
        ErrorCanNotPlaceHere = 0x02,
        ErrorNoItemFound = 0x04,
        ErrorIsNotBuildingType = 0x10,
        ErrorUnknown = 0x20,

        Success = SuccessBuild,
        Error = ErrorCanNotPlaceHere | ErrorNoItemFound | ErrorIsNotBuildingType | ErrorUnknown
    }

    public static class BuildOperationResultExtensions
    {
        public static bool Is(this BuildOperationResult check, BuildOperationResult check2)
        {
            return (check & check2) != 0;
        }
    }

    public static class BuildingEngineHelper
    {
        public static bool CanBuildHere(Vector3 buildPoint, Guid sceneId, IResource canBuildType, bool onClient)
        {
            if (canBuildType == null)
            {
                return true;
            }
            var entityType = DefToType.GetEntityType(canBuildType.GetType());
            if (entityType == null)
            {
                return true;
            }

            List<IRegion> regions = new List<IRegion>();
            var rootRegion = RegionsHolder.GetRegionByGuid(sceneId);
            rootRegion?.GetAllContainingRegionsNonAlloc(regions, buildPoint);
            foreach (var region in regions)
            {
                var datas = region?.RegionDef?.Data;
                if (datas != null)
                {
                    foreach (var data in datas)
                    {
                        var buildBlockerDef = data.Target as BuildBlockerDef;
                        if (buildBlockerDef != null)
                        {
                            if ((entityType == typeof(IBuildingPlace)) && buildBlockerDef.BlockBuildings)
                            {
                                return false;
                            }
                            else if ((entityType == typeof(IFencePlace)) && buildBlockerDef.BlockFences)
                            {
                                return false;
                            }
                            else if (((entityType == typeof(IWorldBench)) || (entityType == typeof(IWorldMachine)) || (entityType == typeof(IWorldPersonalMachine)) && buildBlockerDef.BlockWorkbenches))
                            {
                                return false;
                            }
                            else if ((entityType == typeof(ICharacterChest)) && buildBlockerDef.BlockChests)
                            {
                                return false;
                            }
                            else if ((entityType == typeof(IObelisk)) && buildBlockerDef.BlockObelisks)
                            {
                                return false;
                            }
                            else if ((entityType == typeof(IWorldBaken)) && buildBlockerDef.BlockBakens)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}