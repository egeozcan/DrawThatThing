namespace DrawThatThing.Platform.Linux;

public class LinuxPlatformServices : IPlatformServices
{
    private readonly LinuxMouseOperations _mouse;
    private readonly LinuxHotkeyManager _hotkeys;
    private readonly LinuxScreenCapture _screenCapture;

    public LinuxPlatformServices()
    {
        _mouse = new LinuxMouseOperations();
        _hotkeys = new LinuxHotkeyManager();
        _screenCapture = new LinuxScreenCapture();
    }

    public IMouseOperations Mouse => _mouse;
    public IHotkeyManager Hotkeys => _hotkeys;
    public IScreenCapture ScreenCapture => _screenCapture;
    public PlatformType Platform => PlatformType.Linux;
}
