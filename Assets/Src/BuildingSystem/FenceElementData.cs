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
    public class FenceElementData : ElementDataEx<IPositionedFenceElementAlways, PositionedFenceElementAlwaysReplica>
    {
        private static Dictionary<string, PropertyDataHelper.CopyInfo> copyActionsCache = null;
        protected override Dictionary<string, PropertyDataHelper.CopyInfo> GetСopyActionsCache() { return copyActionsCache; }
        protected override void SetСopyActionsCache(Dictionary<string, PropertyDataHelper.CopyInfo> cache) { copyActionsCache = cache; }

        [Bind]
        public SharedCode.Utils.Vector3 Position { get; protected set; } = SharedCode.Utils.Vector3.zero;
        [Bind]
        public SharedCode.Utils.Quaternion Rotation { get; protected set; } = SharedCode.Utils.Quaternion.identity;

        public override BuildRecipeDef ExtractBuildRecipeDef(PositionedFenceElementAlwaysReplica element) { return element.RecipeDef; }
        public override Guid ExtractOwnerId(PositionedFenceElementAlwaysReplica element) { return element.OwnerId; }
        public override Guid ExtractElementId(PositionedFenceElementAlwaysReplica element) { return element.ElementId; }
        public override UnityRef<GameObject> ExtractPrefab() { return BuildRecipeDef.Prefab; }
        public override ResourceRef<UnityGameObjectDef> ExtractPrefabDef() { return BuildRecipeDef.PrefabDef; }

        public override bool IsDestroyed() { return (State == BuildState.Destroyed); }

        public void SetPlace(bool canPlace, Vector3 position, Quaternion rotation)
        {
            Valid = canPlace;
            Position = new SharedCode.Utils.Vector3(position);
            Rotation = new SharedCode.Utils.Quaternion(rotation);
        }

        public void BindToPlaceholder(BuildRecipeDef buildRecipeDef, bool canPlace, Vector3 position, Quaternion rotation)
        {
            Valid = canPlace;
            Position = new SharedCode.Utils.Vector3(position);
            Rotation = new SharedCode.Utils.Quaternion(rotation);
            State = BuildState.Completed;
            BuildRecipeDef = buildRecipeDef;
            Placeholder = true;
            Selected = true;
            InvokeBindFinished();
        }
    }
}
