using SharedCode.Aspects.Building;
using System;

namespace Assets.Src.BuildingSystem
{
    public class FencePlaceStructure : PlaceStructure<FencePlaceDef, FenceElementData>
    {
        protected override void ElementAdded(Guid key, FenceElementData data) { }
        protected override void ElementRemoved(Guid key, FenceElementData data) { }
        public override void Bind(FencePlaceDef placeDef, SharedCode.Utils.Vector3 Position, SharedCode.Utils.Quaternion Rotation) { }
        public override void Unbind(FencePlaceDef placeDef) { }
        public override void SetVisualCheat(bool enable) { }
    }
}