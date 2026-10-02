using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

internal enum QuartzMouseEventType : uint
{
    LeftMouseDown = 1,
    LeftMouseUp = 2,
    RightMouseDown = 3,
    RightMouseUp = 4,
    MouseMoved = 5,
    LeftMouseDragged = 6,
    RightMouseDragged = 7
}

internal enum QuartzMouseButton : uint
{
    Left = 0,
    Right = 1,
    Center = 2
}

/// <summary>The Quartz event calls <see cref="MacMouseOperations"/> needs, separated so its logic can be tested.</summary>
internal interface IQuartzEvents
{
    void PostMouseEvent(QuartzMouseEventType type, double x, double y, QuartzMouseButton button, bool isClick);

    (double X, double Y) GetCursorLocation();
}

internal sealed class QuartzEvents : IQuartzEvents
{
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    private const int KCGMouseEventClickState = 1;
    private const uint KCGHIDEventTap = 0;

    [StructLayout(LayoutKind.Sequential)]
    private struct CGPoint
    {
        public double X;
        public double Y;
    }

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGEventCreateMouseEvent(IntPtr source, QuartzMouseEventType mouseType, CGPoint mouseCursorPosition, QuartzMouseButton mouseButton);

    [DllImport(CoreGraphics)]
    private static extern void CGEventPost(uint tap, IntPtr eventRef);

    [DllImport(CoreGraphics)]
    private static extern void CGEventSetIntegerValueField(IntPtr eventRef, int field, long value);

    [DllImport(CoreGraphics)]
    private static extern CGPoint CGEventGetLocation(IntPtr eventRef);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGEventCreate(IntPtr source);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    public void PostMouseEvent(QuartzMouseEventType type, double x, double y, QuartzMouseButton button, bool isClick)
    {
        var eventRef = CGEventCreateMouseEvent(IntPtr.Zero, type, new CGPoint { X = x, Y = y }, button);
        if (eventRef == IntPtr.Zero)
        {
            return;
        }

        try
        {
            if (isClick)
            {
                CGEventSetIntegerValueField(eventRef, KCGMouseEventClickState, 1);
            }
            CGEventPost(KCGHIDEventTap, eventRef);
        }
        finally
        {
            CFRelease(eventRef);
        }
    }

    public (double X, double Y) GetCursorLocation()
    {
        var eventRef = CGEventCreate(IntPtr.Zero);
        if (eventRef == IntPtr.Zero)
        {
            return (0, 0);
        }

        try
        {
            var point = CGEventGetLocation(eventRef);
            return (point.X, point.Y);
        }
        finally
        {
            CFRelease(eventRef);
        }
    }
}
