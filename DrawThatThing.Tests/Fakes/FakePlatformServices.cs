using DrawThatThing.Platform;

namespace DrawThatThing.Tests.Fakes;

public class FakePlatformServices : IPlatformServices
{
    public FakeMouse FakeMouse { get; } = new();
    public FakeHotkeyManager FakeHotkeys { get; } = new();
    public FakeScreenCapture FakeScreen { get; } = new();

    public PlatformType Platform { get; set; } = PlatformType.Windows;

    public IMouseOperations Mouse => FakeMouse;
    public IHotkeyManager Hotkeys => FakeHotkeys;
    public IScreenCapture ScreenCapture => FakeScreen;
}

public class FakeScreenCapture : IScreenCapture
{
    /// <summary>The color on the whole screen; null when the screen cannot be read.</summary>
    public (byte R, byte G, byte B)? Color { get; set; } = (0, 0, 0);

    public (byte R, byte G, byte B)? GetPixelColor(int x, int y) => Color;

    /// <summary>Draws the screen contents for a captured region; blank when not set.</summary>
    public Func<int, int, int, int, byte[]>? Region { get; set; }

    public byte[] CaptureRegion(int x, int y, int width, int height) =>
        Region?.Invoke(x, y, width, height) ?? new byte[width * height * 4];
}
