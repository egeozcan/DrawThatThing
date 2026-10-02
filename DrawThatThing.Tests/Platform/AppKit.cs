using System.Runtime.InteropServices;
using DrawThatThing.Platform.macOS;

namespace DrawThatThing.Tests.Platform;

/// <summary>
/// Just enough Objective-C interop to ask a menu bar which key presses it claims. AppKit only allows the
/// real menu bar to be used from the main thread, which tests do not run on, so a standalone menu stands in.
/// </summary>
internal static class AppKit
{
    private static IntPtr _menuBar;

    static AppKit()
    {
        // The UI toolkit loads AppKit in the real app; a test process has to do it itself.
        NativeLibrary.Load("/System/Library/Frameworks/AppKit.framework/AppKit");
        MacAppIntegration.GetMainMenu = () => _menuBar;
    }

    private const string ObjC = "/usr/lib/libobjc.A.dylib";
    private const ulong NSEventTypeKeyDown = 10;
    private const ulong NSEventModifierFlagCommand = 1 << 20;

    [StructLayout(LayoutKind.Sequential)]
    private struct CGPoint
    {
        public double X;
        public double Y;
    }

    [DllImport(ObjC)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(ObjC)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern nint SendNInt(IntPtr receiver, IntPtr selector);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool SendBool(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendKeyEvent(
        IntPtr receiver, IntPtr selector, ulong type, CGPoint location, ulong modifierFlags, double timestamp,
        nint windowNumber, IntPtr context, IntPtr characters, IntPtr charactersIgnoringModifiers,
        [MarshalAs(UnmanagedType.I1)] bool isARepeat, ushort keyCode);

    /// <summary>A fresh menu bar holding only the application menu, as Avalonia sets it up.</summary>
    public static void ResetMainMenu()
    {
        _menuBar = Send(Send(objc_getClass("NSMenu"), Sel("alloc")), Sel("initWithTitle:"), NSString(string.Empty));
        Send(_menuBar, Sel("addItem:"), CreateMenuItem(AppMenuTitle));
    }

    public const string AppMenuTitle = "DrawThatThing";

    /// <summary>
    /// What Avalonia does when its window stops being the key window (e.g. a file dialog sheet opens):
    /// it takes its application menu item out of the menu bar and appends it again.
    /// </summary>
    public static void MoveAppMenuToTheEnd()
    {
        var appMenuItem = Send(_menuBar, Sel("itemWithTitle:"), NSString(AppMenuTitle));
        Send(_menuBar, Sel("removeItem:"), appMenuItem);
        Send(_menuBar, Sel("addItem:"), appMenuItem);
    }

    /// <summary>The titles of the menu bar's menus; macOS shows the first one as the application menu.</summary>
    public static List<string> MenuBarTitles()
    {
        var titles = new List<string>();
        var count = (int)SendNInt(_menuBar, Sel("numberOfItems"));
        for (int i = 0; i < count; i++)
        {
            var title = Send(Send(_menuBar, Sel("itemAtIndex:"), i), Sel("title"));
            titles.Add(Marshal.PtrToStringUTF8(Send(title, Sel("UTF8String"))) ?? string.Empty);
        }
        return titles;
    }

    private static IntPtr CreateMenuItem(string title)
    {
        return Send(
            Send(objc_getClass("NSMenuItem"), Sel("alloc")),
            Sel("initWithTitle:action:keyEquivalent:"),
            NSString(title), IntPtr.Zero, NSString(string.Empty));
    }

    /// <summary>Whether the menu bar swallows Command + the given key instead of letting the focused window handle it.</summary>
    public static bool MenuBarClaimsCommandKey(char key, ushort keyCode)
    {
        var characters = NSString(key.ToString());
        var keyEvent = SendKeyEvent(
            objc_getClass("NSEvent"),
            Sel("keyEventWithType:location:modifierFlags:timestamp:windowNumber:context:characters:charactersIgnoringModifiers:isARepeat:keyCode:"),
            NSEventTypeKeyDown, default, NSEventModifierFlagCommand, 0, 0, IntPtr.Zero, characters, characters, false, keyCode);
        return SendBool(_menuBar, Sel("performKeyEquivalent:"), keyEvent);
    }

    private static IntPtr Sel(string name) => sel_registerName(name);

    private static IntPtr NSString(string value)
    {
        var utf8 = Marshal.StringToCoTaskMemUTF8(value);
        try
        {
            return Send(objc_getClass("NSString"), Sel("stringWithUTF8String:"), utf8);
        }
        finally
        {
            Marshal.FreeCoTaskMem(utf8);
        }
    }
}
