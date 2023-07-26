using JetBrains.Annotations;

public class StartupParams
{
    public static readonly StartupParams Instance = new StartupParams();

    public ConnectionParams ConnectionParams { get; set; }
    public PlayParams PlayParams { get; set; }
    public PlatformParams PlatformParams { get; set; }

    [UsedImplicitly]
    public bool AutoConnect { get; set; }
}

public struct PlayParams
{
    [UsedImplicitly]
    public bool AutoPlay { get; set; }
}

public struct PlatformParams
{
    public string PlatformUrl { get; set; }
}