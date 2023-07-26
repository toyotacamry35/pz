namespace Src.Locomotion
{
    public interface ILocomotionCollisionsReceiver
    {
        void OnCollision(CollisionInfo nfo);
    }
    
    public readonly struct CollisionInfo
    {
        public readonly LocomotionVector Point;
        public readonly LocomotionVector Normal;
        public readonly LocomotionVector ObjectPosition;
        public readonly int ObjectLayer;

        public CollisionInfo(LocomotionVector point, LocomotionVector normal, LocomotionVector objectPosition, int objectLayer)
        {
            Point = point;
            Normal = normal;
            ObjectPosition = objectPosition;
            ObjectLayer = objectLayer;
        }
    }
}