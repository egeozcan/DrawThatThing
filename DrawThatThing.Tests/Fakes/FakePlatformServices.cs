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
    public (byte R, byte G, byte B) Color { get; set; }

    public (byte R, byte G, byte B) GetPixelColor(int x, int y) => Color;

    public byte[] CaptureRegion(int x, int y, int width, int height) => new byte[width * height * 4];
}
