namespace Src.Locomotion
{
    /// <summary>
    /// Синхронизирован только Timestamp!
    /// </summary>
    public class LocomotionSyncedClock : ILocomotionClock, ILocomotionUpdateable
    {
        public float Time { get; private set; }

        public float DeltaTime { get; private set; }

        public LocomotionTimestamp Timestamp { get; private set; }

        void ILocomotionUpdateable.Update(float deltaTime)
        {
            DeltaTime = deltaTime;
            Time += deltaTime;
            if (Timestamp < ColonyShared.SharedCode.Utils.SyncTime.Now)
                Timestamp = ColonyShared.SharedCode.Utils.SyncTime.Now;
        }
    }
}
