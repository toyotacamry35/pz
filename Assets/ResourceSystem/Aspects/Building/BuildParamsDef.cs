using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Wizardry;

namespace SharedCode.Aspects.Building
{
    public class BuildParamsDef : BaseResource
    {
        public float BuildingPlacePositionThreshold2 { get; set; }
        public bool  ElasticEnable { get; set; }
        public float ElasticDeltaTime { get; set; }
        public float ElasticLerpFactor { get; set; }
        public float ElasticMinLerpFactor { get; set; }
        public float ElasticMaxLerpFactor { get; set; }
        public float ElasticMinSqrDistance { get; set; }
        public float ElasticMinAngle { get; set; }
        public Utils.Vector3 CharacterCenterPoint { get; set; }
        public float FenceAngle { get; set; }
        public bool FenceAngleRepeat { get; set; }
        public float FenceAngleStep { get; set; }
        public float FenceAngleMin { get; set; }
        public float FenceAngleMax { get; set; }
        public float FenceVecticalShift { get; set; }
        public bool FenceVecticalShiftRepeat { get; set; }
        public float FenceVecticalShiftStep { get; set; }
        public float FenceVecticalShiftMin { get; set; }
        public float FenceVecticalShiftMax { get; set; }
        public float FenceHorizontalShift { get; set; }
        public bool FenceHorizontalShiftRepeat { get; set; }
        public float FenceHorizontalShiftStep { get; set; }
        public float FenceHorizontalShiftMin { get; set; }
        public float FenceHorizontalShiftMax { get; set; }
        public float BuildingShiftXZ0VerticalAngle { get; set; }
        public float BuildingShiftXZ1VerticalAngle { get; set; }
        public float BuildingShiftXZ2VerticalAngle { get; set; }
        public bool BuildingAngleRepeat { get; set; }
        public bool BuildingVecticalShiftRepeat { get; set; }
        public int BuildingVecticalShiftMin { get; set; }
        public int BuildingVecticalShiftMax { get; set; }
        public bool BuildingHorizontalShiftRepeat { get; set; }
        public int BuildingHorizontalShiftMin { get; set; }
        public int BuildingHorizontalShiftMax { get; set; }
        public float RaycastNearDistance { get; set; }
        public float RaycastFarDistance { get; set; }
        public float DepthSeconds { get; set; }
        public float FracturedChunkScale { get; set; }
        public bool ClaimResources { get; set; }
        public bool ClaimResourcesfromInventory { get; set; }
        public bool ClaimResourcesfromDoll { get; set; }
        public float ReclaimGap { get; set; }
        public ResourceRef<WorldBoxDef> DropBoxDef { get; set; }
        public bool RestrictHits { get; set; }
        public float MinHit { get; set; }
        public float MaxHit { get; set; }
        public ResourceRef<SpellDef> InteractionSpellDef { get; set; }
        public ResourceRef<SpellDef> AttackSpellDef { get; set; }
        public UnityRef<UnityEngine.GameObject> EdgeHelperPrefab { get; set; }
        public ResourceRef<UnityGameObjectDef> EdgeHelperPrefabDef { get; set; }
        public UnityRef<UnityEngine.GameObject> RaycastHelperPrefab { get; set; }
        public ResourceRef<UnityGameObjectDef> RaycastHelperPrefabDef { get; set; }
        public ResourceRef<BuildingPlaceDef> DefaultBuildingPlaceDef { get; set; }
        public ResourceRef<FencePlaceDef> DefaultFencePlaceDef { get; set; }
    }
}
