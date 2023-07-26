using SharedCode.EntitySystem;
using System;
using SharedCode.DeltaObjects.Building;

namespace Assets.Src.BuildingSystem
{
    public class BuildingPlaceEventArgs : EventArgs
    {
    }

    public interface IBuildInterface
    {
        event EventHandler<BuildingPlaceEventArgs> BuildingPlaceRegistered;
        event EventHandler<BuildingPlaceEventArgs> BuildingPlaceUnregistered;
        event EventHandler<BuildingPlaceEventArgs> BuildingPlaceChanged;

        bool IsEnabled { get; }
        bool IsSimpleMode { get; }

        bool DamageEnableCheat { get; }
        float DamageValueCheat { get; }

        bool ClaimResourcesEnableCheat { get; }
        bool ClaimResourcesValueCheat { get; }

        PlaceData RegisterPlace(IEntitiesRepository EntitiesRepository, PlaceType type, EventHandler BindFinished, EventHandler UnbindFinished, EventHandler<PropertyData.PropertyArgs> BindPropertyChanged, Guid placeId);
        bool UnregisterPlace(IEntitiesRepository EntitiesRepository, PlaceType type, Guid placeId);
        void InvokeBuildingPlaceChanged();

        void ActivateBuildingPlace(IEntitiesRepository entitiesRepository, UnityEngine.Vector3 position);
        void DeactiveBuildingPlace(IEntitiesRepository entitiesRepository);

        void CreateElement(IEntitiesRepository entitiesRepository, PlaceholderData data);
        void RemoveElement(IEntitiesRepository entitiesRepository, BuildType type, Guid placeId, Guid elementId);
        void DamageElement(IEntitiesRepository entitiesRepository, BuildType type, Guid placeId, Guid elementId, float damage);
        void InteractElement(IEntitiesRepository entitiesRepository, BuildType type, Guid placeId, Guid elementId, int interaction);

        bool CalculatePlaceholder(PlaceholderData data);
        bool ShowPlaceholder(PlaceholderData data);
        bool HidePlaceholder(PlaceholderData data);

        void UpdateCheats(IEntitiesRepository entitiesRepository);
        void SetDamageCheat(IEntitiesRepository entitiesRepository, bool enable, float damage);
        void SetClaimResourceCheat(IEntitiesRepository entitiesRepository, bool enable, bool claim);
        void SetVisualCheat(IEntitiesRepository entitiesRepository, bool enable);
        void SetDebugCheat(IEntitiesRepository entitiesRepository, bool enable, bool verbose);
    }
}