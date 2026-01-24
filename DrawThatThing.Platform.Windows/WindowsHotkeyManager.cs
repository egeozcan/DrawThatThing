using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.Windows;

public class WindowsHotkeyManager : IHotkeyManager
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;

    private IntPtr _windowHandle;
    private readonly List<int> _registeredHotkeys = new();
    private bool _disposed;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public WindowsHotkeyManager()
    {
        _windowHandle = IntPtr.Zero;
    }

    public void SetWindowHandle(IntPtr handle)
    {
        _windowHandle = handle;
    }

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        if (_windowHandle == IntPtr.Zero)
            return false;

        uint nativeModifiers = 0;
        if (modifiers.HasFlag(HotkeyModifiers.Alt))
            nativeModifiers |= MOD_ALT;
        if (modifiers.HasFlag(HotkeyModifiers.Ctrl))
            nativeModifiers |= MOD_CONTROL;
        if (modifiers.HasFlag(HotkeyModifiers.Shift))
            nativeModifiers |= MOD_SHIFT;
        if (modifiers.HasFlag(HotkeyModifiers.Win))
            nativeModifiers |= MOD_WIN;

        bool result = RegisterHotKey(_windowHandle, id, nativeModifiers, (uint)char.ToUpper(key));
        if (result)
        {
            _registeredHotkeys.Add(id);
        }
        return result;
    }

    public bool UnregisterHotkey(int id)
    {
        if (_windowHandle == IntPtr.Zero)
            return false;

        bool result = UnregisterHotKey(_windowHandle, id);
        if (result)
        {
            _registeredHotkeys.Remove(id);
        }
        return result;
    }

    public void ProcessHotkeyMessage(int id)
    {
        HotkeyPressed?.Invoke(this, new HotkeyEventArgs(id));
    }

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var id in _registeredHotkeys.ToList())
        {
            UnregisterHotkey(id);
        }
        _registeredHotkeys.Clear();
        _disposed = true;
    }
}
