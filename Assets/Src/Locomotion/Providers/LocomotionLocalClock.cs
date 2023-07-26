namespace Src.Locomotion
{
    public class LocomotionLocalClock : ILocomotionClock, ILocomotionUpdateable
    {
        public float Time { get; private set; }

        public float DeltaTime { get; private set; }

        public LocomotionTimestamp Timestamp { get; private set; }

        void ILocomotionUpdateable.Update(float deltaTime)
        {
            DeltaTime = deltaTime;
            Time += deltaTime;
            Timestamp = ColonyShared.SharedCode.Utils.SyncTime.NowUnsynced;
        }
    }
}