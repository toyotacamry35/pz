using ColonyShared.SharedCode;

namespace Src.Locomotion
{
    public static class ValueExtensionsLocomotion
    {
        public static LocomotionVector LocomotionVector(this in Value value) => LocomotionHelpers.WorldToLocomotionVector(value.Vector3);
        
        public static Value ToValue(this in LocomotionVector v) => new Value(LocomotionHelpers.LocomotionToWorldVector(v));
    }
}