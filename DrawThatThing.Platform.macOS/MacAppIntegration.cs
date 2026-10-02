using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

/// <summary>
/// Small bits of native macOS glue the UI toolkit does not provide.
/// </summary>
public static class MacAppIntegration
{
    private const string ObjC = "/usr/lib/libobjc.A.dylib";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const string ApplicationServices = "/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices";

    [DllImport(ObjC)]
    private static extern IntPtr objc_getClass(string name);

    [DllImport(ObjC)]
    private static extern IntPtr sel_registerName(string name);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector, IntPtr arg1, nint arg2);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr Send(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2, IntPtr arg3);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern nint SendNInt(IntPtr receiver, IntPtr selector);

    [DllImport(ObjC, EntryPoint = "objc_msgSend")]
    private static extern nint SendNInt(IntPtr receiver, IntPtr selector, IntPtr arg1);

    [DllImport(ApplicationServices)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool AXIsProcessTrusted();

    [DllImport(ApplicationServices)]
    [return: MarshalAs(UnmanagedType.I1)]
    private static extern bool AXIsProcessTrustedWithOptions(IntPtr options);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFDictionaryCreate(IntPtr allocator, IntPtr[] keys, IntPtr[] values, nint numValues, IntPtr keyCallBacks, IntPtr valueCallBacks);

    [DllImport(CoreFoundation)]
    private static extern IntPtr CFStringCreateWithCString(IntPtr allocator, string cStr, uint encoding);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    private const uint KCFStringEncodingUtf8 = 0x08000100;

    private static IntPtr _editMenuItem;
    private static int _openNativeDialogs;

    /// <summary>
    /// Returns whether the app may post mouse events. If not, macOS is asked to show its
    /// "allow this app to control your computer" prompt.
    /// </summary>
    public static bool EnsureAccessibilityAccess()
    {
        try
        {
            if (AXIsProcessTrusted())
            {
                return true;
            }

            var coreFoundation = NativeLibrary.Load(CoreFoundation);
            var trueValue = Marshal.ReadIntPtr(NativeLibrary.GetExport(coreFoundation, "kCFBooleanTrue"));
            var keyCallBacks = NativeLibrary.GetExport(coreFoundation, "kCFTypeDictionaryKeyCallBacks");
            var valueCallBacks = NativeLibrary.GetExport(coreFoundation, "kCFTypeDictionaryValueCallBacks");
            var promptKey = CFStringCreateWithCString(IntPtr.Zero, "AXTrustedCheckOptionPrompt", KCFStringEncodingUtf8);
            var options = CFDictionaryCreate(IntPtr.Zero, [promptKey], [trueValue], 1, keyCallBacks, valueCallBacks);
            try
            {
                return AXIsProcessTrustedWithOptions(options);
            }
            finally
            {
                CFRelease(options);
                CFRelease(promptKey);
            }
        }
        catch
        {
            return true;
        }
    }

    /// <summary>
    /// Returns the menu bar the Edit menu goes into. Tests replace it with a standalone menu, because
    /// AppKit only allows the real menu bar to be changed from the main thread.
    /// </summary>
    internal static Func<IntPtr> GetMainMenu { get; set; } =
        () => Send(Send(objc_getClass("NSApplication"), Sel("sharedApplication")), Sel("mainMenu"));

    /// <summary>
    /// Shows a standard Edit menu (Undo, Redo, Cut, Copy, Paste, Select All) until the returned object is
    /// disposed. Native dialogs such as the open panel rely on these menu items for their keyboard
    /// shortcuts, so without it Cmd+V does nothing in the "Go to folder" (Cmd+Shift+G) field.
    /// The menu must not stay around afterwards: AppKit lets the menu bar handle Cmd+V and friends before
    /// the focused window, even when the items are disabled, so the app's own text boxes would never get them.
    /// Main thread only.
    /// </summary>
    public static IDisposable BeginNativeDialog()
    {
        _openNativeDialogs++;
        EnsureEditMenu();
        return new NativeDialogScope();
    }

    /// <summary>
    /// When its window gains or loses the focus (e.g. to a dialog sheet), the toolkit takes its application
    /// menu out of the menu bar and appends it again, which would leave the Edit menu first, where macOS
    /// shows it as the application menu. Call this afterwards to put the Edit menu back behind it.
    /// </summary>
    public static void OnWindowActivationChanged()
    {
        if (_openNativeDialogs > 0)
        {
            EnsureEditMenu();
        }
    }

    private static void EnsureEditMenu()
    {
        try
        {
            var mainMenu = GetMainMenu();
            if (mainMenu == IntPtr.Zero)
            {
                return;
            }

            // Its place is right after the application menu, which is always the first one.
            if (_editMenuItem != IntPtr.Zero && SendNInt(mainMenu, Sel("indexOfItem:"), _editMenuItem) >= 1)
            {
                return;
            }

            if (_editMenuItem == IntPtr.Zero)
            {
                _editMenuItem = CreateEditMenuItem();
            }

            // A menu item can only be in one menu at a time.
            RemoveEditMenu();

            var count = SendNInt(mainMenu, Sel("numberOfItems"));
            Send(mainMenu, Sel("insertItem:atIndex:"), _editMenuItem, Math.Min(1, count));
        }
        catch
        {
            // Purely a convenience; never break the app over it.
        }
    }

    private static void RemoveEditMenu()
    {
        if (_editMenuItem == IntPtr.Zero)
        {
            return;
        }

        var currentParent = Send(_editMenuItem, Sel("menu"));
        if (currentParent != IntPtr.Zero)
        {
            Send(currentParent, Sel("removeItem:"), _editMenuItem);
        }
    }

    private sealed class NativeDialogScope : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (--_openNativeDialogs == 0)
            {
                try
                {
                    RemoveEditMenu();
                }
                catch
                {
                    // Purely a convenience; never break the app over it.
                }
            }
        }
    }

    private static IntPtr CreateEditMenuItem()
    {
        var menu = Send(Send(objc_getClass("NSMenu"), Sel("alloc")), Sel("initWithTitle:"), NSString("Edit"));
        AddItem(menu, "Undo", "undo:", "z");
        AddItem(menu, "Redo", "redo:", "Z");
        Send(menu, Sel("addItem:"), Send(objc_getClass("NSMenuItem"), Sel("separatorItem")));
        AddItem(menu, "Cut", "cut:", "x");
        AddItem(menu, "Copy", "copy:", "c");
        AddItem(menu, "Paste", "paste:", "v");
        AddItem(menu, "Select All", "selectAll:", "a");

        var item = Send(
            Send(objc_getClass("NSMenuItem"), Sel("alloc")),
            Sel("initWithTitle:action:keyEquivalent:"),
            NSString("Edit"),
            IntPtr.Zero,
            NSString(string.Empty));
        Send(item, Sel("setSubmenu:"), menu);
        return item;
    }

    private static void AddItem(IntPtr menu, string title, string action, string keyEquivalent)
    {
        Send(menu, Sel("addItemWithTitle:action:keyEquivalent:"), NSString(title), Sel(action), NSString(keyEquivalent));
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
