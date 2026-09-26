using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.Linux;

/// <summary>
/// System-wide hotkeys on X11 through XGrabKey on the root window. Uses its own display
/// connection, serviced by a background thread.
/// </summary>
public class LinuxHotkeyManager : IHotkeyManager
{
    private const string X11 = "libX11.so.6";

    [DllImport(X11)]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport(X11)]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport(X11)]
    private static extern IntPtr XDefaultRootWindow(IntPtr display);

    [DllImport(X11)]
    private static extern byte XKeysymToKeycode(IntPtr display, IntPtr keysym);

    [DllImport(X11)]
    private static extern int XGrabKey(IntPtr display, int keycode, uint modifiers, IntPtr grabWindow, bool ownerEvents, int pointerMode, int keyboardMode);

    [DllImport(X11)]
    private static extern int XUngrabKey(IntPtr display, int keycode, uint modifiers, IntPtr grabWindow);

    [DllImport(X11)]
    private static extern int XSelectInput(IntPtr display, IntPtr window, IntPtr eventMask);

    [DllImport(X11)]
    private static extern int XPending(IntPtr display);

    [DllImport(X11)]
    private static extern int XNextEvent(IntPtr display, IntPtr eventReturn);

    [DllImport(X11)]
    private static extern int XSync(IntPtr display, bool discard);

    private const int KeyPress = 2;
    private const int GrabModeAsync = 1;
    private const int XEventSize = 24 * 8;
    private const int XKeyEventStateOffset = 80;
    private const int XKeyEventKeycodeOffset = 84;

    private const uint ShiftMask = 1 << 0;
    private const uint LockMask = 1 << 1;
    private const uint ControlMask = 1 << 2;
    private const uint Mod1Mask = 1 << 3; // Alt
    private const uint Mod2Mask = 1 << 4; // Num Lock
    private const uint Mod4Mask = 1 << 6; // Super

    // Caps Lock and Num Lock must not stop a hotkey from working, so grab every combination of them.
    private static readonly uint[] IgnoredModifierCombinations = [0, LockMask, Mod2Mask, LockMask | Mod2Mask];
    private const uint RelevantModifiers = ShiftMask | ControlMask | Mod1Mask | Mod4Mask;

    private readonly ConcurrentQueue<Action> _pendingWork = new();
    private readonly Dictionary<int, (int Keycode, uint Modifiers)> _registeredHotkeys = new();
    private readonly object _startLock = new();
    private Thread? _thread;
    private IntPtr _display;
    private IntPtr _root;
    private volatile bool _running;
    private bool _disposed;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        uint nativeModifiers = 0;
        if (modifiers.HasFlag(HotkeyModifiers.Alt)) nativeModifiers |= Mod1Mask;
        if (modifiers.HasFlag(HotkeyModifiers.Ctrl)) nativeModifiers |= ControlMask;
        if (modifiers.HasFlag(HotkeyModifiers.Shift)) nativeModifiers |= ShiftMask;
        if (modifiers.HasFlag(HotkeyModifiers.Win)) nativeModifiers |= Mod4Mask;

        // Latin letters and digits have keysyms equal to their (lowercase) ASCII code.
        var keysym = (IntPtr)char.ToLowerInvariant(key);

        return Invoke(() =>
        {
            if (_registeredHotkeys.ContainsKey(id))
            {
                return false;
            }
            int keycode = XKeysymToKeycode(_display, keysym);
            if (keycode == 0)
            {
                return false;
            }
            foreach (var ignored in IgnoredModifierCombinations)
            {
                XGrabKey(_display, keycode, nativeModifiers | ignored, _root, false, GrabModeAsync, GrabModeAsync);
            }
            XSync(_display, false);
            _registeredHotkeys[id] = (keycode, nativeModifiers);
            return true;
        });
    }

    public bool UnregisterHotkey(int id)
    {
        return Invoke(() =>
        {
            if (!_registeredHotkeys.Remove(id, out var hotkey))
            {
                return false;
            }
            foreach (var ignored in IgnoredModifierCombinations)
            {
                XUngrabKey(_display, hotkey.Keycode, hotkey.Modifiers | ignored, _root);
            }
            XSync(_display, false);
            return true;
        });
    }

    private bool Invoke(Func<bool> work)
    {
        if (_disposed || !EnsureThread())
        {
            return false;
        }

        var result = false;
        using var done = new ManualResetEventSlim();
        _pendingWork.Enqueue(() =>
        {
            try
            {
                result = work();
            }
            catch
            {
                result = false;
            }
            finally
            {
                done.Set();
            }
        });
        return done.Wait(TimeSpan.FromSeconds(5)) && result;
    }

    private bool EnsureThread()
    {
        lock (_startLock)
        {
            if (_thread != null)
            {
                return _display != IntPtr.Zero;
            }

            try
            {
                _display = XOpenDisplay(IntPtr.Zero);
            }
            catch (DllNotFoundException)
            {
                _display = IntPtr.Zero;
            }

            if (_display == IntPtr.Zero)
            {
                _thread = new Thread(() => { });
                return false;
            }

            _root = XDefaultRootWindow(_display);
            _running = true;
            _thread = new Thread(EventLoop)
            {
                IsBackground = true,
                Name = "DrawThatThing hotkeys"
            };
            _thread.Start();
            return true;
        }
    }

    private void EventLoop()
    {
        var xEvent = Marshal.AllocHGlobal(XEventSize);
        try
        {
            while (_running)
            {
                while (_pendingWork.TryDequeue(out var work))
                {
                    work();
                }

                while (XPending(_display) > 0)
                {
                    XNextEvent(_display, xEvent);
                    if (Marshal.ReadInt32(xEvent) != KeyPress)
                    {
                        continue;
                    }

                    var state = (uint)Marshal.ReadInt32(xEvent, XKeyEventStateOffset) & RelevantModifiers;
                    var keycode = Marshal.ReadInt32(xEvent, XKeyEventKeycodeOffset);
                    foreach (var (id, hotkey) in _registeredHotkeys)
                    {
                        if (hotkey.Keycode == keycode && hotkey.Modifiers == state)
                        {
                            HotkeyPressed?.Invoke(this, new HotkeyEventArgs(id));
                        }
                    }
                }

                Thread.Sleep(15);
            }

            foreach (var hotkey in _registeredHotkeys.Values)
            {
                foreach (var ignored in IgnoredModifierCombinations)
                {
                    XUngrabKey(_display, hotkey.Keycode, hotkey.Modifiers | ignored, _root);
                }
            }
            _registeredHotkeys.Clear();
            XCloseDisplay(_display);
            _display = IntPtr.Zero;
        }
        finally
        {
            Marshal.FreeHGlobal(xEvent);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _running = false;
        _thread?.Join(TimeSpan.FromSeconds(2));
    }
}
