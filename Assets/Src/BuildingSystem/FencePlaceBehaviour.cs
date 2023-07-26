using SharedCode.DeltaObjects.Building;
using SharedCode.Entities.Building;

namespace Assets.Src.BuildingSystem
{
    class FencePlaceBehaviour : PlaceBehaviour<FencePlaceData, IFencePlace>
    {
        protected override PlaceType GetPlaceType() { return PlaceType.FencePlace; }

        protected override void AwakePlace() { }

        protected override void DestroyPlace(FencePlaceData data) { }

        protected override bool DestroyGameObject(FencePlaceData data) { return false; }

        protected override void CreateServer(FencePlaceData data) { }

        protected override void DestroyServer(FencePlaceData data) { }

        protected override void CreateVisual(FencePlaceData data) { }

        protected override void DestroyVisual(FencePlaceData data) { }

        protected override void UpdateVisual(FencePlaceData data) { }

        protected override void BindPropertyChanged(FencePlaceData data, PropertyData.PropertyArgs propertyArgs) { }

        protected override void BindFinished(FencePlaceData data) { }

        protected override void UnbindFinished(FencePlaceData data) { }
    }
}