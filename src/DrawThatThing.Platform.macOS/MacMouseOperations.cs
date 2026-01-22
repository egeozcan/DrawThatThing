using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.macOS;

public class MacMouseOperations : IMouseOperations
{
    // CoreGraphics framework
    private const string CoreGraphics = "/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics";

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGEventCreateMouseEvent(IntPtr source, CGEventType mouseType, CGPoint mouseCursorPosition, CGMouseButton mouseButton);

    [DllImport(CoreGraphics)]
    private static extern void CGEventPost(CGEventTapLocation tap, IntPtr eventRef);

    [DllImport(CoreGraphics)]
    private static extern void CFRelease(IntPtr cf);

    [DllImport(CoreGraphics)]
    private static extern CGPoint CGEventGetLocation(IntPtr eventRef);

    [DllImport(CoreGraphics)]
    private static extern IntPtr CGEventCreate(IntPtr source);

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

    public void SetCursorPosition(int x, int y)
    {
        var point = new CGPoint(x, y);
        var moveEvent = CGEventCreateMouseEvent(IntPtr.Zero, CGEventType.MouseMoved, point, CGMouseButton.Left);
        if (moveEvent != IntPtr.Zero)
        {
            CGEventPost(CGEventTapLocation.HID, moveEvent);
            CFRelease(moveEvent);
        }
    }

    public (int X, int Y) GetCursorPosition()
    {
        var eventRef = CGEventCreate(IntPtr.Zero);
        if (eventRef != IntPtr.Zero)
        {
            try
            {
                var point = CGEventGetLocation(eventRef);
                return ((int)point.X, (int)point.Y);
            }
            finally
            {
                CFRelease(eventRef);
            }
        }
        return (0, 0);
    }

    public void LeftMouseDown()
    {
        var pos = GetCursorPosition();
        var point = new CGPoint(pos.X, pos.Y);
        var downEvent = CGEventCreateMouseEvent(IntPtr.Zero, CGEventType.LeftMouseDown, point, CGMouseButton.Left);
        if (downEvent != IntPtr.Zero)
        {
            CGEventPost(CGEventTapLocation.HID, downEvent);
            CFRelease(downEvent);
        }
    }

    public void LeftMouseUp()
    {
        var pos = GetCursorPosition();
        var point = new CGPoint(pos.X, pos.Y);
        var upEvent = CGEventCreateMouseEvent(IntPtr.Zero, CGEventType.LeftMouseUp, point, CGMouseButton.Left);
        if (upEvent != IntPtr.Zero)
        {
            CGEventPost(CGEventTapLocation.HID, upEvent);
            CFRelease(upEvent);
        }
    }

    public void RightMouseDown()
    {
        var pos = GetCursorPosition();
        var point = new CGPoint(pos.X, pos.Y);
        var downEvent = CGEventCreateMouseEvent(IntPtr.Zero, CGEventType.RightMouseDown, point, CGMouseButton.Right);
        if (downEvent != IntPtr.Zero)
        {
            CGEventPost(CGEventTapLocation.HID, downEvent);
            CFRelease(downEvent);
        }
    }

    public void RightMouseUp()
    {
        var pos = GetCursorPosition();
        var point = new CGPoint(pos.X, pos.Y);
        var upEvent = CGEventCreateMouseEvent(IntPtr.Zero, CGEventType.RightMouseUp, point, CGMouseButton.Right);
        if (upEvent != IntPtr.Zero)
        {
            CGEventPost(CGEventTapLocation.HID, upEvent);
            CFRelease(upEvent);
        }
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
}
