namespace DrawThatThing.Platform;

/// <summary>
/// Aggregates all platform-specific services
/// </summary>
public interface IPlatformServices
{
    IMouseOperations Mouse { get; }
    IHotkeyManager Hotkeys { get; }
    IScreenCapture ScreenCapture { get; }

    /// <summary>
    /// Gets the current platform identifier
    /// </summary>
    PlatformType Platform { get; }
}

public enum PlatformType
{
    Windows,
    macOS,
    Linux,
    Unknown
}
