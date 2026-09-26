using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

/// <summary>
/// System-wide hotkeys through Carbon's RegisterEventHotKey, which (unlike event taps)
/// does not need any privacy permission. Must be used from the main thread.
/// </summary>
public class MacHotkeyManager : IHotkeyManager
{
    private const string Carbon = "/System/Library/Frameworks/Carbon.framework/Carbon";

    private const uint KEventClassKeyboard = 0x6B657962; // 'keyb'
    private const uint KEventHotKeyPressed = 5;
    private const uint KEventParamDirectObject = 0x2D2D2D2D; // '----'
    private const uint TypeEventHotKeyId = 0x686B6964; // 'hkid'
    private const uint HotkeySignature = 0x44545467; // 'DTTg'

    private const uint CmdKey = 0x0100;
    private const uint ShiftKey = 0x0200;
    private const uint OptionKey = 0x0800;
    private const uint ControlKey = 0x1000;

    [StructLayout(LayoutKind.Sequential)]
    private struct EventTypeSpec
    {
        public uint EventClass;
        public uint EventKind;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct EventHotKeyId
    {
        public uint Signature;
        public uint Id;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int EventHandlerProc(IntPtr nextHandler, IntPtr theEvent, IntPtr userData);

    [DllImport(Carbon)]
    private static extern IntPtr GetEventDispatcherTarget();

    [DllImport(Carbon)]
    private static extern int InstallEventHandler(IntPtr target, IntPtr handler, uint numTypes, EventTypeSpec[] list, IntPtr userData, out IntPtr handlerRef);

    [DllImport(Carbon)]
    private static extern int RemoveEventHandler(IntPtr handlerRef);

    [DllImport(Carbon)]
    private static extern int RegisterEventHotKey(uint keyCode, uint modifiers, EventHotKeyId hotKeyId, IntPtr target, uint options, out IntPtr hotKeyRef);

    [DllImport(Carbon)]
    private static extern int UnregisterEventHotKey(IntPtr hotKeyRef);

    [DllImport(Carbon)]
    private static extern int GetEventParameter(IntPtr theEvent, uint name, uint desiredType, IntPtr actualType, UIntPtr bufferSize, IntPtr actualSize, out EventHotKeyId data);

    // kVK_ANSI_* virtual key codes (they follow the physical US layout).
    private static readonly Dictionary<char, uint> KeyCodes = new()
    {
        ['A'] = 0x00, ['S'] = 0x01, ['D'] = 0x02, ['F'] = 0x03, ['H'] = 0x04, ['G'] = 0x05, ['Z'] = 0x06,
        ['X'] = 0x07, ['C'] = 0x08, ['V'] = 0x09, ['B'] = 0x0B, ['Q'] = 0x0C, ['W'] = 0x0D, ['E'] = 0x0E,
        ['R'] = 0x0F, ['Y'] = 0x10, ['T'] = 0x11, ['1'] = 0x12, ['2'] = 0x13, ['3'] = 0x14, ['4'] = 0x15,
        ['6'] = 0x16, ['5'] = 0x17, ['9'] = 0x19, ['7'] = 0x1A, ['8'] = 0x1C, ['0'] = 0x1D, ['O'] = 0x1F,
        ['U'] = 0x20, ['I'] = 0x22, ['P'] = 0x23, ['L'] = 0x25, ['J'] = 0x26, ['K'] = 0x28, ['N'] = 0x2D,
        ['M'] = 0x2E
    };

    private readonly Dictionary<int, IntPtr> _registeredHotkeys = new();
    private EventHandlerProc? _handler;
    private IntPtr _handlerRef;
    private bool _disposed;

    public event EventHandler<HotkeyEventArgs>? HotkeyPressed;

    public bool RegisterHotkey(int id, HotkeyModifiers modifiers, char key)
    {
        if (_disposed || _registeredHotkeys.ContainsKey(id) || !KeyCodes.TryGetValue(char.ToUpperInvariant(key), out var keyCode))
        {
            return false;
        }

        try
        {
            if (!EnsureHandlerInstalled())
            {
                return false;
            }

            uint nativeModifiers = 0;
            if (modifiers.HasFlag(HotkeyModifiers.Alt)) nativeModifiers |= OptionKey;
            if (modifiers.HasFlag(HotkeyModifiers.Ctrl)) nativeModifiers |= ControlKey;
            if (modifiers.HasFlag(HotkeyModifiers.Shift)) nativeModifiers |= ShiftKey;
            if (modifiers.HasFlag(HotkeyModifiers.Win)) nativeModifiers |= CmdKey;

            var hotKeyId = new EventHotKeyId { Signature = HotkeySignature, Id = (uint)id };
            var status = RegisterEventHotKey(keyCode, nativeModifiers, hotKeyId, GetEventDispatcherTarget(), 0, out var hotKeyRef);
            if (status != 0 || hotKeyRef == IntPtr.Zero)
            {
                return false;
            }

            _registeredHotkeys[id] = hotKeyRef;
            return true;
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            return false;
        }
    }

    public bool UnregisterHotkey(int id)
    {
        if (!_registeredHotkeys.Remove(id, out var hotKeyRef))
        {
            return false;
        }

        return UnregisterEventHotKey(hotKeyRef) == 0;
    }

    private bool EnsureHandlerInstalled()
    {
        if (_handlerRef != IntPtr.Zero)
        {
            return true;
        }

        // Keep the delegate alive for as long as the native side may call it.
        _handler = OnHotkeyEvent;
        var spec = new[] { new EventTypeSpec { EventClass = KEventClassKeyboard, EventKind = KEventHotKeyPressed } };
        var status = InstallEventHandler(
            GetEventDispatcherTarget(),
            Marshal.GetFunctionPointerForDelegate(_handler),
            (uint)spec.Length,
            spec,
            IntPtr.Zero,
            out _handlerRef);
        return status == 0 && _handlerRef != IntPtr.Zero;
    }

    private int OnHotkeyEvent(IntPtr nextHandler, IntPtr theEvent, IntPtr userData)
    {
        try
        {
            var status = GetEventParameter(
                theEvent,
                KEventParamDirectObject,
                TypeEventHotKeyId,
                IntPtr.Zero,
                (UIntPtr)Marshal.SizeOf<EventHotKeyId>(),
                IntPtr.Zero,
                out var hotKeyId);
            if (status == 0 && hotKeyId.Signature == HotkeySignature)
            {
                HotkeyPressed?.Invoke(this, new HotkeyEventArgs((int)hotKeyId.Id));
            }
        }
        catch
        {
            // Never let exceptions cross back into native code.
        }

        return 0;
    }

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var id in _registeredHotkeys.Keys.ToList())
        {
            UnregisterHotkey(id);
        }
        if (_handlerRef != IntPtr.Zero)
        {
            RemoveEventHandler(_handlerRef);
            _handlerRef = IntPtr.Zero;
        }
        _disposed = true;
    }
}
