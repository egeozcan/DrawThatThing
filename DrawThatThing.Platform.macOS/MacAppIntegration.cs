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
    /// Adds a standard Edit menu (Undo, Redo, Cut, Copy, Paste, Select All) to the menu bar.
    /// Native dialogs such as the open panel rely on these menu items for their keyboard shortcuts,
    /// so without it Cmd+V does nothing in the "Go to folder" (Cmd+Shift+G) field.
    /// The items target the first responder, so for the app's own windows they stay disabled and
    /// the key presses reach the app's text boxes as usual.
    /// Safe to call repeatedly; it re-adds the menu if the toolkit rebuilt the menu bar.
    /// </summary>
    public static void EnsureEditMenu()
    {
        try
        {
            var app = Send(objc_getClass("NSApplication"), Sel("sharedApplication"));
            var mainMenu = Send(app, Sel("mainMenu"));
            if (mainMenu == IntPtr.Zero)
            {
                return;
            }

            if (_editMenuItem != IntPtr.Zero && SendNInt(mainMenu, Sel("indexOfItem:"), _editMenuItem) >= 0)
            {
                return;
            }

            if (_editMenuItem == IntPtr.Zero)
            {
                _editMenuItem = CreateEditMenuItem();
            }

            // A menu item can only be in one menu at a time.
            var currentParent = Send(_editMenuItem, Sel("menu"));
            if (currentParent != IntPtr.Zero)
            {
                Send(currentParent, Sel("removeItem:"), _editMenuItem);
            }

            var count = SendNInt(mainMenu, Sel("numberOfItems"));
            Send(mainMenu, Sel("insertItem:atIndex:"), _editMenuItem, Math.Min(1, count));
        }
        catch
        {
            // Purely a convenience; never break the app over it.
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
