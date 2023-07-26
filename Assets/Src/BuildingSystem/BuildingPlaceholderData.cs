using UnityEngine;
using SharedCode.DeltaObjects.Building;
using SharedCode.Aspects.Building;
using SharedCode.Entities.Engine;
using Assets.ColonyShared.SharedCode.Shared;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;

namespace Assets.Src.BuildingSystem
{
    public class BuildingPlaceholderData : PlaceholderData
    {
        private class CacheData
        {
            public BuildRecipeDef BuildRecipeDef { get; set; } = null;
            public bool CanPlace { get; set; } = true;
            public Vector3Int Block { get; set; } = Vector3Int.zero;
            public BuildingElementType Type { get; set; } = BuildingElementType.Unknown;
            public BuildingElementFace Face { get; set; } = BuildingElementFace.Unknown;
            public BuildingElementSide Side { get; set; } = BuildingElementSide.Unknown;
            public int ShiftY { get; set; } = 0;
            public int ShiftXZ { get; set; } = 0;
        }

        private CacheData cache = null;

        // данные от интерфейса локальные
        public BuildRecipeDef BuildRecipeDef { get; set; } = null;
        public BuildingElementType PlaceholderType { get; set; } = BuildingElementType.Center;
        public BuildingElementSide PlaceholderSide { get; set; } = BuildingElementSide.Forward;
        public int ShiftY { get; set; } = 0;
        public int ShiftXZ { get; set; } = 1;

        // вычисляемые данные
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public Vector3Int Block { get; set; } = Vector3Int.zero;
        public BuildingElementType Type { get; set; } = BuildingElementType.Unknown;
        public BuildingElementFace Face { get; set; } = BuildingElementFace.Unknown;
        public BuildingElementSide Side { get; set; } = BuildingElementSide.Unknown;

        public Vector3 PlacePosition { get; set; } = Vector3.zero;
        public Quaternion PlaceRotation { get; set; } = Quaternion.identity;
        public bool CacheInvalidOverride { get; set; } = false;

        public override bool IsCacheInvalid()
        {
            return CacheInvalidOverride || (cache == null) || (cache.BuildRecipeDef != BuildRecipeDef) || (cache.CanPlace != CanPlace.Result) || (cache.Block != Block) || (cache.Type != Type) || (cache.Face != Face) || (cache.Side != Side) || (cache.ShiftY != ShiftY) || (cache.ShiftXZ != ShiftXZ);
        }

        public override void Cache(bool validate)
        {
            if (validate)
            {
                if (cache == null)
                {
                    cache = new CacheData();
                }
                cache.BuildRecipeDef = BuildRecipeDef;
                cache.CanPlace = CanPlace.Result;
                cache.Block = Block;
                cache.Type = Type;
                cache.Face = Face;
                cache.Side = Side;
                cache.ShiftY = ShiftY;
                cache.ShiftXZ = ShiftXZ;
                if ((cache.Type != BuildingElementType.Wall) && (Side != BuildingElementSide.Unknown)) // walls have self rotation
                {
                    PlaceholderSide = Side;
                }
            }
            else
            {
                cache = null;
            }
        }

        public override ICreationData GetCreationData(PlaceData data)
        {
            if (data == null)
            {
                return null;
            }
            if (data.IsEmpty && !BuildSystem.Builder.IsSimpleMode)
            {
                return null;
            }
            var createBuildingElementData = data.IsEmpty ? new CreateBuildingPlaceAndBuildingElementData() : new CreateBuildingElementData();
            createBuildingElementData.Position = new SharedCode.Utils.Vector3(Position);
            createBuildingElementData.Rotation = new SharedCode.Utils.Quaternion(Rotation);
            createBuildingElementData.Block = new SharedCode.Utils.Vector3Int(Block);
            createBuildingElementData.Type = Type;
            createBuildingElementData.Face = Face;
            createBuildingElementData.Side = Side;
            createBuildingElementData.Health = BuildRecipeDef?.GetInitialHealth() ?? 0.0f;
            createBuildingElementData.Visual = BuildRecipeDef?.GetInitialVisual() ?? -1;
            createBuildingElementData.Interaction = BuildRecipeDef?.GetInitialInteraction() ?? -1;
            if (data.IsEmpty)
            {
                var createBuildingPlaceAndBuildingElementData = createBuildingElementData as CreateBuildingPlaceAndBuildingElementData;
                if (createBuildingPlaceAndBuildingElementData != null)
                {
                    createBuildingPlaceAndBuildingElementData.PlacePosition = new SharedCode.Utils.Vector3(PlacePosition);
                    createBuildingPlaceAndBuildingElementData.PlaceRotation = new SharedCode.Utils.Quaternion(PlaceRotation);
                }
            }
            return new CreationData(BuildType, data.PlaceId, BuildRecipeDef, createBuildingElementData);
        }

        public BuildingPlaceholderData()
        {
            BuildType = BuildType.BuildingElement;
        }

        public override void ChangeRotation(bool positive)
        {
            var counterClockwise = positive;
            PlaceholderSide = BuildingStructure.BitHelper.Rotate(PlaceholderSide, counterClockwise);
        }

        public override void ChangeShift(bool positive, PlaceholderShiftType shiftType)
        {
            var buildParamsDef = SharedCode.Utils.BuildUtils.BuildParamsDef;
            if (shiftType == PlaceholderShiftType.Vertical)
            {
                ShiftY += (positive ? 1 : -1);
                if (ShiftY > buildParamsDef.BuildingVecticalShiftMax)
                {
                    ShiftY = buildParamsDef.BuildingVecticalShiftRepeat ? buildParamsDef.BuildingVecticalShiftMin : buildParamsDef.BuildingVecticalShiftMax;
                }
                else if (ShiftY < buildParamsDef.BuildingVecticalShiftMin)
                {
                    ShiftY = buildParamsDef.BuildingVecticalShiftRepeat ? buildParamsDef.BuildingVecticalShiftMax : buildParamsDef.BuildingVecticalShiftMin;
                }
            }
            else
            {
                ShiftXZ += (positive ? 1 : -1);
                if (ShiftXZ > buildParamsDef.BuildingHorizontalShiftMax)
                {
                    ShiftXZ = buildParamsDef.BuildingVecticalShiftRepeat ? buildParamsDef.BuildingHorizontalShiftMin : buildParamsDef.BuildingHorizontalShiftMax;
                }
                else if (ShiftXZ < buildParamsDef.BuildingHorizontalShiftMin)
                {
                    ShiftXZ = buildParamsDef.BuildingVecticalShiftRepeat ? buildParamsDef.BuildingHorizontalShiftMax : buildParamsDef.BuildingHorizontalShiftMin;
                }
            }
        }

        public override bool CanBuildHere()
        {
            var sceneId = NodeAccessor.Repository.GetSceneForEntity(new OuterRef<IEntity>(
            GameState.Instance.CharacterRuntimeData.CharacterId, WorldCharacter.StaticTypeId));
            return BuildingEngineHelper.CanBuildHere((SharedCode.Utils.Vector3)Position, sceneId, SharedCode.Utils.BuildUtils.DefaultBuildingPlaceDef, true);
        }
    }
}