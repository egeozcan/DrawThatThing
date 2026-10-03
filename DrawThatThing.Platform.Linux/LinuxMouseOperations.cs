using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.Linux;

public class LinuxMouseOperations : IMouseOperations
{
    private const string X11 = "libX11.so.6";
    private const string XTest = "libXtst.so.6";

    [DllImport(X11)]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport(X11)]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport(X11)]
    private static extern int XDefaultScreen(IntPtr display);

    [DllImport(X11)]
    private static extern IntPtr XRootWindow(IntPtr display, int screen);

    [DllImport(X11)]
    private static extern bool XQueryPointer(IntPtr display, IntPtr window, out IntPtr root, out IntPtr child,
        out int root_x, out int root_y, out int win_x, out int win_y, out uint mask);

    [DllImport(X11)]
    private static extern int XWarpPointer(IntPtr display, IntPtr src_w, IntPtr dest_w, int src_x, int src_y,
        uint src_width, uint src_height, int dest_x, int dest_y);

    [DllImport(X11)]
    private static extern int XFlush(IntPtr display);

    [DllImport(XTest)]
    private static extern int XTestFakeMotionEvent(IntPtr display, int screen, int x, int y, ulong delay);

    [DllImport(XTest)]
    private static extern int XTestFakeButtonEvent(IntPtr display, uint button, bool is_press, ulong delay);

    private const uint Button1 = 1; // Left
    private const uint Button2 = 2; // Middle
    private const uint Button3 = 3; // Right

    private IntPtr? _display;

    private IntPtr GetDisplay()
    {
        _display ??= XOpenDisplay(IntPtr.Zero);
        return _display.Value;
    }

    public void SetCursorPosition(int x, int y)
    {
        var display = GetDisplay();
        if (display == IntPtr.Zero) return;

        // XTest motion is seen by applications as real pointer motion (drags), unlike a plain warp.
        XTestFakeMotionEvent(display, XDefaultScreen(display), x, y, 0);
        XFlush(display);
    }

    public (int X, int Y) GetCursorPosition()
    {
        var display = GetDisplay();
        if (display == IntPtr.Zero) return (0, 0);

        int screen = XDefaultScreen(display);
        IntPtr root = XRootWindow(display, screen);

        if (XQueryPointer(display, root, out _, out _, out int x, out int y, out _, out _, out _))
        {
            return (x, y);
        }
        return (0, 0);
    }

    public void LeftMouseDown()
    {
        var display = GetDisplay();
        if (display == IntPtr.Zero) return;

        XTestFakeButtonEvent(display, Button1, true, 0);
        XFlush(display);
    }

    public void LeftMouseUp()
    {
        var display = GetDisplay();
        if (display == IntPtr.Zero) return;

        XTestFakeButtonEvent(display, Button1, false, 0);
        XFlush(display);
    }

    public void RightMouseDown()
    {
        var display = GetDisplay();
        if (display == IntPtr.Zero) return;

        XTestFakeButtonEvent(display, Button3, true, 0);
        XFlush(display);
    }

    public void RightMouseUp()
    {
        var display = GetDisplay();
        if (display == IntPtr.Zero) return;

        XTestFakeButtonEvent(display, Button3, false, 0);
        XFlush(display);
    }

    public void Click()
    {
        LeftMouseDown();
        Thread.Sleep(10);
        LeftMouseUp();
    }

    public void Click(int x, int y)
    {
        SetCursorPosition(x, y);
        Click();
    }

    ~LinuxMouseOperations()
    {
        if (_display.HasValue && _display.Value != IntPtr.Zero)
        {
            XCloseDisplay(_display.Value);
        }
    }
}
