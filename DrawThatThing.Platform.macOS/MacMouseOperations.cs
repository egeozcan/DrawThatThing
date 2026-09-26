using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

/// <summary>
/// Synthesizes mouse input with Quartz events. Posting events requires the Accessibility
/// permission (System Settings → Privacy &amp; Security → Accessibility); without it macOS
/// silently drops them.
/// </summary>
public class MacMouseOperations : IMouseOperations
{
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";
    private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGEventCreateMouseEvent(IntPtr source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton);

    [DllImport(CoreGraphics)]
    private static extern void CGEventPost(CGEventTapLocation tap, IntPtr eventRef);

    [DllImport(CoreGraphics)]
    private static extern void CGEventSetIntegerValueField(IntPtr eventRef, int field, long value);

    [DllImport(CoreGraphics)]
    private static extern CGPoint CGEventGetLocation(IntPtr eventRef);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGEventCreate(IntPtr source);

    [DllImport(CoreFoundation)]
    private static extern void CFRelease(IntPtr cf);

    private const int KCGMouseEventClickState = 1;

    private enum CGEventType : uint
    {
        LeftMouseDown = 1,
        LeftMouseUp = 2,
        RightMouseDown = 3,
        RightMouseUp = 4,
        MouseMoved = 5,
        LeftMouseDragged = 6,
        RightMouseDragged = 7
    }

    private enum CGMouseButton : uint
    {
        Left = 0,
        Right = 1,
        Center = 2
    }

    private enum CGEventTapLocation : uint
    {
        HID = 0,
        Session = 1,
        AnnotatedSession = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CGPoint
    {
        public double X;
        public double Y;

        public CGPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    private volatile bool _leftButtonDown;
    private volatile bool _rightButtonDown;

    public void SetCursorPosition(int x, int y)
    {
        // Moving while a button is held has to be reported as a drag, otherwise
        // applications see a plain hover and nothing gets drawn.
        var (type, button) = _leftButtonDown
            ? (CGEventType.LeftMouseDragged, CGMouseButton.Left)
            : _rightButtonDown
                ? (CGEventType.RightMouseDragged, CGMouseButton.Right)
                : (CGEventType.MouseMoved, CGMouseButton.Left);
        Post(type, new CGPoint(x, y), button, clickState: null);
    }

    public (int X, int Y) GetCursorPosition()
    {
        var point = GetCursorLocation();
        return ((int)Math.Round(point.X), (int)Math.Round(point.Y));
    }

    public void LeftMouseDown()
    {
        _leftButtonDown = true;
        Post(CGEventType.LeftMouseDown, GetCursorLocation(), CGMouseButton.Left, clickState: 1);
    }

    public void LeftMouseUp()
    {
        _leftButtonDown = false;
        Post(CGEventType.LeftMouseUp, GetCursorLocation(), CGMouseButton.Left, clickState: 1);
    }

    public void RightMouseDown()
    {
        _rightButtonDown = true;
        Post(CGEventType.RightMouseDown, GetCursorLocation(), CGMouseButton.Right, clickState: 1);
    }

    public void RightMouseUp()
    {
        _rightButtonDown = false;
        Post(CGEventType.RightMouseUp, GetCursorLocation(), CGMouseButton.Right, clickState: 1);
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

    private static CGPoint GetCursorLocation()
    {
        var eventRef = CGEventCreate(IntPtr.Zero);
        if (eventRef == IntPtr.Zero)
        {
            return new CGPoint(0, 0);
        }

        try
        {
            return CGEventGetLocation(eventRef);
        }
        finally
        {
            CFRelease(eventRef);
        }
    }

    private static void Post(CGEventType type, CGPoint point, CGMouseButton button, long? clickState)
    {
        var eventRef = CGEventCreateMouseEvent(IntPtr.Zero, type, point, button);
        if (eventRef == IntPtr.Zero)
        {
            return;
        }

        try
        {
            if (clickState.HasValue)
            {
                CGEventSetIntegerValueField(eventRef, KCGMouseEventClickState, clickState.Value);
            }
            CGEventPost(CGEventTapLocation.HID, eventRef);
        }
        finally
        {
            CFRelease(eventRef);
        }
    }
}
