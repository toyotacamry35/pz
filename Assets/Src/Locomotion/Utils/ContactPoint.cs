namespace Src.Locomotion
{
    public readonly struct ContactPoint
    {
        public readonly LocomotionVector Point;
        public readonly LocomotionVector LocalPoint;
        public readonly LocomotionVector Normal;
        public readonly ContactPointObjectType ObjectType;
        public readonly LocomotionVector ObjectPosition;
        public readonly ContactPointLocation Location;

        public ContactPoint(LocomotionVector point, LocomotionVector localPoint, LocomotionVector normal, ContactPointLocation location, ContactPointObjectType objectType, LocomotionVector objectPosition)
        {
            Point = point;
            LocalPoint = localPoint;
            Normal = normal;
            Location = location;
            ObjectType = objectType;
            ObjectPosition = objectPosition;
        }
    }
}