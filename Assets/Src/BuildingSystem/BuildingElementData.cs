using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class BuildingElementData : ElementDataEx<IPositionedBuildingElementAlways, PositionedBuildingElementAlwaysReplica>
    {
        private static Dictionary<string, PropertyDataHelper.CopyInfo> copyActionsCache = null;
        protected override Dictionary<string, PropertyDataHelper.CopyInfo> GetСopyActionsCache() { return copyActionsCache; }
        protected override void SetСopyActionsCache(Dictionary<string, PropertyDataHelper.CopyInfo> cache) { copyActionsCache = cache; }

        [Bind]
        public SharedCode.Utils.Vector3 Position { get; protected set; } = SharedCode.Utils.Vector3.zero;
        [Bind]
        public SharedCode.Utils.Quaternion Rotation { get; protected set; } = SharedCode.Utils.Quaternion.identity;
        [Bind]
        public SharedCode.Utils.Vector3Int Block { get; protected set; } = SharedCode.Utils.Vector3Int.zero;
        [Bind]
        public BuildingElementType Type { get; protected set; } = BuildingElementType.Unknown;
        [Bind]
        public BuildingElementFace Face { get; protected set; } = BuildingElementFace.Unknown;
        [Bind]
        public BuildingElementSide Side { get; protected set; } = BuildingElementSide.Unknown;

        public override BuildRecipeDef ExtractBuildRecipeDef(PositionedBuildingElementAlwaysReplica element) { return element.RecipeDef; }
        public override Guid ExtractOwnerId(PositionedBuildingElementAlwaysReplica element) { return element.OwnerId; }
        public override Guid ExtractElementId(PositionedBuildingElementAlwaysReplica element) { return element.ElementId; }
        public override UnityRef<GameObject> ExtractPrefab() { return BuildRecipeDef.Prefab; }
        public override ResourceRef<UnityGameObjectDef> ExtractPrefabDef() { return BuildRecipeDef.PrefabDef; }

        public override bool IsDestroyed() { return (State == BuildState.Destroyed); }

        public void SetPlace(bool canPlace, Vector3 position, Quaternion rotation, Vector3Int block, BuildingElementType type, BuildingElementFace face, BuildingElementSide side)
        {
            Valid = canPlace;
            Position = new SharedCode.Utils.Vector3(position);
            Rotation = new SharedCode.Utils.Quaternion(rotation);
            Block = new SharedCode.Utils.Vector3Int(block);
            Type = type;
            Face = face;
            Side = side;
        }

        public void BindToPlaceholder(BuildRecipeDef buildRecipeDef, bool canPlace, Vector3 position, Quaternion rotation, Vector3Int block, BuildingElementType type, BuildingElementFace face, BuildingElementSide side)
        {
            Valid = canPlace;
            Position = new SharedCode.Utils.Vector3(position);
            Rotation = new SharedCode.Utils.Quaternion(rotation);
            Block = new SharedCode.Utils.Vector3Int(block);
            Type = type;
            Face = face;
            Side = side;
            State = BuildState.Completed;
            BuildRecipeDef = buildRecipeDef;
            Placeholder = true;
            Selected = true;
            InvokeBindFinished();
        }
    }
}
