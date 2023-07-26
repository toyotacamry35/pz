using Assets.ColonyShared.SharedCode.Shared;
using GeneratedCode.DeltaObjects;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using System;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class FencePlaceholderData : PlaceholderData
    {
        private class CacheData
        {
            public BuildRecipeDef BuildRecipeDef { get; set; } = null;
            public bool CanPlace { get; set; } = true;
            public Vector3 Position { get; set; } = Vector3.zero;
            public Quaternion Rotation { get; set; } = Quaternion.identity;
        }
        private CacheData cache = null;

        // данные от интерфейса локальные
        public BuildRecipeDef BuildRecipeDef { get; set; } = null;
        public float RotationY { get; private set; } = 0.0f;
        public float ShiftY { get; private set; } = 0.0f;
        public float DistanceXZ { get; private set; } = 5.0f;

        // вычисляемые данные
        public Vector3 Position { get; set; } = Vector3.zero;
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        public override bool IsCacheInvalid()
        {
            return ((cache == null) || (cache.CanPlace != CanPlace.Result) || (cache.BuildRecipeDef != BuildRecipeDef) || (cache.Position != Position) || (cache.Rotation != Rotation));
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
                cache.Position = Position;
                cache.Rotation = Rotation;
            }
            else
            {
                cache = null;
            }
        }

        public override ICreationData GetCreationData(PlaceData data)
        {
            var createFenceElementData = new CreateFenceElementData();
            createFenceElementData.Position = (SharedCode.Utils.Vector3)Position;
            createFenceElementData.Rotation = (SharedCode.Utils.Quaternion)Rotation;
            createFenceElementData.Health = BuildRecipeDef?.GetInitialHealth() ?? 0.0f;
            createFenceElementData.Visual = BuildRecipeDef?.GetInitialVisual() ?? -1;
            createFenceElementData.Interaction = BuildRecipeDef?.GetInitialInteraction() ?? -1;
            return new CreationData(BuildType, Guid.Empty, BuildRecipeDef, createFenceElementData);
        }

        public FencePlaceholderData()
        {
            BuildType = BuildType.FenceElement;
            var buildParamsDef = SharedCode.Utils.BuildUtils.BuildParamsDef;
            RotationY = buildParamsDef.FenceAngle;
            ShiftY = buildParamsDef.FenceVecticalShift;
            DistanceXZ = buildParamsDef.FenceHorizontalShift;
        }

        public override void ChangeRotation(bool positive)
        {
            var buildParamsDef = SharedCode.Utils.BuildUtils.BuildParamsDef;
            var newRotationY = RotationY + buildParamsDef.FenceAngleStep * (positive ? (+1.0f) : (-1.0f));
            RotationY = Repeat(newRotationY, buildParamsDef.FenceAngleMin, buildParamsDef.FenceAngleMax);
        }

        public override void ChangeShift(bool positive, PlaceholderShiftType shiftType)
        {
            var buildParamsDef = SharedCode.Utils.BuildUtils.BuildParamsDef;
            if (shiftType == PlaceholderShiftType.Vertical)
            {
                var newShiftY = ShiftY + buildParamsDef.FenceVecticalShiftStep * (positive ? (+1.0f) : (-1.0f));
                if (buildParamsDef.FenceVecticalShiftRepeat)
                {
                    ShiftY = Repeat(newShiftY, buildParamsDef.FenceVecticalShiftMin, buildParamsDef.FenceVecticalShiftMax);
                }
                else
                {
                    ShiftY = Clamp(newShiftY, buildParamsDef.FenceVecticalShiftMin, buildParamsDef.FenceVecticalShiftMax);
                }
            }
            else
            {
                var newDistanceXZ = DistanceXZ + buildParamsDef.FenceHorizontalShiftStep * (positive ? (+1.0f) : (-1.0f));
                if (buildParamsDef.FenceHorizontalShiftRepeat)
                {
                    DistanceXZ = Repeat(newDistanceXZ, buildParamsDef.FenceHorizontalShiftMin, buildParamsDef.FenceHorizontalShiftMax);
                }
                else
                {
                    DistanceXZ = Clamp(newDistanceXZ, buildParamsDef.FenceHorizontalShiftMin, buildParamsDef.FenceHorizontalShiftMax);
                }
            }
        }

        public override bool CanBuildHere()
        {
            var sceneId = NodeAccessor.Repository.GetSceneForEntity(new OuterRef<IEntity>(
            GameState.Instance.CharacterRuntimeData.CharacterId, WorldCharacter.StaticTypeId));
            return BuildingEngineHelper.CanBuildHere((SharedCode.Utils.Vector3)Position, sceneId, SharedCode.Utils.BuildUtils.DefaultFencePlaceDef, true);
        }
    }
}