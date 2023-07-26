namespace Assets.Src.Telemetry
{
    public interface ITelemetryInterface
    {
        float AverageFrameTime { get; }
        float AverageFPS { get; }
        float FrameTime { get; }
        float FPS { get; }
        long AllocatedMemoryTotal { get; }
        long AllocatedMemoryOnFrame { get; }
        bool IsUpdateRequired { get; }
        void Update();
    }
}
