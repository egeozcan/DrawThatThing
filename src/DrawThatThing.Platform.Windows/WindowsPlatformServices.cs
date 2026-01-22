namespace DrawThatThing.Platform.Windows;

public class WindowsPlatformServices : IPlatformServices
{
    private readonly WindowsMouseOperations _mouse;
    private readonly WindowsHotkeyManager _hotkeys;
    private readonly WindowsScreenCapture _screenCapture;

    public WindowsPlatformServices()
    {
        _mouse = new WindowsMouseOperations();
        _hotkeys = new WindowsHotkeyManager();
        _screenCapture = new WindowsScreenCapture();
    }

    public IMouseOperations Mouse => _mouse;
    public IHotkeyManager Hotkeys => _hotkeys;
    public IScreenCapture ScreenCapture => _screenCapture;
    public PlatformType Platform => PlatformType.Windows;

    public WindowsHotkeyManager WindowsHotkeys => _hotkeys;
}
