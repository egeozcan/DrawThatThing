using DrawThatThing.Platform;

namespace DrawThatThing.Tests.Fakes;

/// <summary>A hotkey manager where chosen key combinations are "already taken" by another application.</summary>
public class FakeHotkeyManager : IHotkeyManager
{
    public HashSet<(HotkeyModifiers Modifiers, char Key)> Taken { get; } = [];

    public Dictionary<int, (HotkeyModifiers Modifiers, char Key)> Registered { get; } = new();

    public bool Disposed { get; private set; }

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        if (Registered.ContainsKey(id) || Taken.Contains((modifiers, key)))
        {
            return false;
        }
        Registered[id] = (modifiers, key);
        return true;
    }

    public bool UnregisterHotkey(int id) => Registered.Remove(id);

    public void Press(int id) => HotkeyPressed?.Invoke(this, new HotkeyEventArgs(id));

    public void Dispose()
    {
        Registered.Clear();
        Disposed = true;
    }
}
