using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.Windows;

/// <summary>
/// System-wide hotkeys through RegisterHotKey. The hotkeys belong to a private thread with its
/// own message loop, so no window handle or window procedure hook is needed.
/// </summary>
public class WindowsHotkeyManager : IHotkeyManager
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool PeekMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostThreadMessage(uint idThread, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [StructLayout(LayoutKind.Sequential)]
    private struct Msg
    {
        public IntPtr Hwnd;
        public uint Message;
        public IntPtr WParam;
        public IntPtr LParam;
        public uint Time;
        public int PtX;
        public int PtY;
    }

    private const uint MOD_ALT = 0x0001;
    private const uint MOD_CONTROL = 0x0002;
    private const uint MOD_SHIFT = 0x0004;
    private const uint MOD_WIN = 0x0008;
    private const uint MOD_NOREPEAT = 0x4000;

    private const uint WM_HOTKEY = 0x0312;
    private const uint WM_QUIT = 0x0012;
    private const uint WM_APP_INVOKE = 0x8000 + 1;
    private const uint PM_NOREMOVE = 0x0000;

    private readonly ConcurrentQueue<Action> _pendingWork = new();
    private readonly HashSet<int> _registeredHotkeys = new();
    private readonly ManualResetEventSlim _threadReady = new();
    private Thread? _thread;
    private uint _threadId;
    private bool _disposed;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        uint nativeModifiers = MOD_NOREPEAT;
        if (modifiers.HasFlag(HotkeyModifiers.Alt))
            nativeModifiers |= MOD_ALT;
        if (modifiers.HasFlag(HotkeyModifiers.Ctrl))
            nativeModifiers |= MOD_CONTROL;
        if (modifiers.HasFlag(HotkeyModifiers.Shift))
            nativeModifiers |= MOD_SHIFT;
        if (modifiers.HasFlag(HotkeyModifiers.Win))
            nativeModifiers |= MOD_WIN;

        return Invoke(() =>
        {
            if (_registeredHotkeys.Contains(id) || !RegisterHotKey(IntPtr.Zero, id, nativeModifiers, char.ToUpperInvariant(key)))
            {
                return false;
            }
            _registeredHotkeys.Add(id);
            return true;
        });
    }

    public bool UnregisterHotkey(int id)
    {
        return Invoke(() => _registeredHotkeys.Remove(id) && UnregisterHotKey(IntPtr.Zero, id));
    }

    /// <summary>
    /// Runs <paramref name="work"/> on the hotkey thread (hotkeys are owned by the thread that registered them).
    /// </summary>
    private bool Invoke(Func<bool> work)
    {
        if (_disposed)
        {
            return false;
        }

        EnsureThread();
        var result = false;
        using var done = new ManualResetEventSlim();
        _pendingWork.Enqueue(() =>
        {
            try
            {
                result = work();
            }
            finally
            {
                done.Set();
            }
        });

        if (!PostThreadMessage(_threadId, WM_APP_INVOKE, IntPtr.Zero, IntPtr.Zero))
        {
            return false;
        }

        return done.Wait(TimeSpan.FromSeconds(5)) && result;
    }

    private void EnsureThread()
    {
        if (_thread != null)
        {
            return;
        }

        _thread = new Thread(MessageLoop)
        {
            IsBackground = true,
            Name = "DrawThatThing hotkeys"
        };
        _thread.Start();
        _threadReady.Wait();
    }

    private void MessageLoop()
    {
        _threadId = GetCurrentThreadId();
        // Make sure the thread has a message queue before anyone posts to it.
        PeekMessage(out _, IntPtr.Zero, 0, 0, PM_NOREMOVE);
        _threadReady.Set();

        while (GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
        {
            if (msg.Message == WM_HOTKEY)
            {
                HotkeyPressed?.Invoke(this, new HotkeyEventArgs(msg.WParam.ToInt32()));
            }
            else if (msg.Message == WM_APP_INVOKE)
            {
                while (_pendingWork.TryDequeue(out var work))
                {
                    work();
                }
            }
        }

        foreach (var id in _registeredHotkeys)
        {
            UnregisterHotKey(IntPtr.Zero, id);
        }
        _registeredHotkeys.Clear();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_thread != null)
        {
            PostThreadMessage(_threadId, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
            _thread.Join(TimeSpan.FromSeconds(2));
        }
    }
}
