namespace Src.Locomotion
{
    public interface ILocomotionClock
    {
        float Time { get; }

        float DeltaTime { get; }

        LocomotionTimestamp Timestamp { get; }
    }
}
