namespace DrawThatThing.Platform.macOS;

/// <summary>
/// macOS hotkey manager using Carbon API
/// Note: For full implementation, you would need to use the Carbon API's RegisterEventHotKey
/// This is a simplified implementation that can be extended
/// </summary>
public class MacHotkeyManager : IHotkeyManager
{
    private readonly Dictionary<int, (HotkeyModifiers Modifiers, char Key)> _registeredHotkeys = new();
    private bool _disposed;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        // Note: Full implementation would use Carbon's RegisterEventHotKey
        // This requires more complex setup with an event handler callback
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
