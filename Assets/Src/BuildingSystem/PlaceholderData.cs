using SharedCode.DeltaObjects.Building;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public enum PlaceholderShiftType
    {
        Vertical,
        Horizontal,
    }

    public abstract class PlaceholderData
    {
        public static float Repeat(float value, float minValue, float maxValue)
        {
            return Mathf.Repeat(value - minValue, maxValue - minValue) + minValue;
        }
        public static float Clamp(float value, float minValue, float maxValue)
        {
            return Mathf.Clamp(value, minValue, maxValue);
        }

        //тип
        public BuildType BuildType { get; set; } = BuildType.None;
        //созданный элемент
        public ElementData Placeholder { get; set; } = null;

        //данные от интерфейса общие
        public bool Visible { get { return (Placeholder != null); } }
        public CanPlaceData CanPlace { get; } = new CanPlaceData(); // можно или нельзя поставить
        public Vector3 InterfacePosition { get; set; } = Vector3.zero; // куда ставить
        public Quaternion InterfaceRotation { get; set; } = Quaternion.identity; //куда ставить

        public abstract bool IsCacheInvalid();
        public abstract void Cache(bool validate);

        public abstract ICreationData GetCreationData(PlaceData data);

        public abstract void ChangeRotation(bool positive);
        public abstract void ChangeShift(bool positive, PlaceholderShiftType shiftType);

        public abstract bool CanBuildHere();
    }
}