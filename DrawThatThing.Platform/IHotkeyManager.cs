namespace DrawThatThing.Platform;

/// <summary>
/// Platform-agnostic interface for global hotkey management
/// </summary>
public interface IHotkeyManager : IDisposable
{
    /// <summary>
    /// Registers a global hotkey
    /// </summary>
    /// <param name="id">Unique identifier for the hotkey</param>
    /// <param name="modifiers">Modifier keys (Shift, Alt, Ctrl, etc.)</param>
    /// <param name="key">The key to register</param>
    /// <returns>True if registration succeeded</returns>
    bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key);

    /// <summary>
    /// Unregisters a global hotkey
    /// </summary>
    /// <param name="id">The hotkey identifier to unregister</param>
    /// <returns>True if unregistration succeeded</returns>
    bool UnregisterHotkey(int id);

    /// <summary>
    /// Event fired when a registered hotkey is pressed
    /// </summary>
    event EventHandler<HotkeyEventArgs>? HotkeyPressed;
}

[Flags]
public enum HotkeyModifiers
{
    None = 0,
    Alt = 1,
    Ctrl = 2,
    Shift = 4,
    Win = 8
}

public class HotkeyEventArgs : EventArgs
{
    public int Id { get; }

    public HotkeyEventArgs(int id)
    {
        Id = id;
    }
}
