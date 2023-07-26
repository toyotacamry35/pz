using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;

namespace Assets.Src.BuildingSystem
{
    public enum BuildMode
    {
        None = 0,
        PlaceNew = 1,
        PickExisted = 2,
    }

    public interface ICharacterBuildInterface
    {
        BuildMode ActiveMode { get; }
        BuildType ActiveType { get; }
        bool IsBuildingPlaceActive { get; }
        bool IsBuildingPlaceInRange { get; }
        bool HasSelectedElement { get; }
        CanPlaceData CanPlace { get; }

        bool Activate(bool activate);

        bool ActivatePlaceholder(BuildRecipeDef buildRecipeDef);
        bool SetResourcesIsEnough(bool resourcesIsEnough);

        bool CyclePlaceholderRotation(bool positive);
        bool CyclePlaceholderShift(bool positive, PlaceholderShiftType shiftType);

        bool CreateElement();
        bool RemoveElement();
    }
}
