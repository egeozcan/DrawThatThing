namespace DrawThatThing.Platform.macOS;

public class MacPlatformServices : IPlatformServices
{
    private readonly MacMouseOperations _mouse;
    private readonly MacHotkeyManager _hotkeys;
    private readonly MacScreenCapture _screenCapture;

    public MacPlatformServices()
    {
        _mouse = new MacMouseOperations();
        _hotkeys = new MacHotkeyManager();
        _screenCapture = new MacScreenCapture();
    }

    public IMouseOperations Mouse => _mouse;
    public IHotkeyManager Hotkeys => _hotkeys;
    public IScreenCapture ScreenCapture => _screenCapture;
    public PlatformType Platform => PlatformType.macOS;
}
