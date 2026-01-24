namespace DrawThatThing.Platform.Linux;

/// <summary>
/// Linux hotkey manager using X11
/// Note: For full implementation, you would need to use XGrabKey
/// This is a simplified implementation that can be extended
/// </summary>
public class LinuxHotkeyManager : IHotkeyManager
{
    private readonly Dictionary<int, (HotkeyModifiers Modifiers, char Key)> _registeredHotkeys = new();
    private bool _disposed;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        // Note: Full implementation would use X11's XGrabKey
        // This requires an X11 event loop to be running
        if (_registeredHotkeys.ContainsKey(id))
            return false;

        _registeredHotkeys[id] = (modifiers, key);
        return true;
    }

    public bool UnregisterHotkey(int id)
    {
        return _registeredHotkeys.Remove(id);
    }

    protected virtual void OnHotkeyPressed(int id)
    {
        HotkeyPressed?.Invoke(this, new HotkeyEventArgs(id));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _registeredHotkeys.Clear();
        _disposed = true;
    }
}
