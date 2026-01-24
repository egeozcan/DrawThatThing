using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.Windows;

public class WindowsMouseOperations : IMouseOperations
{
    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, IntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    public void SetCursorPosition(int x, int y)
    {
        SetCursorPos(x, y);
    }

    public (int X, int Y) GetCursorPosition()
    {
        if (GetCursorPos(out POINT point))
        {
            return (point.X, point.Y);
        }
        return (0, 0);
    }

    public void LeftMouseDown()
    {
        var pos = GetCursorPosition();
        mouse_event(MOUSEEVENTF_LEFTDOWN, pos.X, pos.Y, 0, IntPtr.Zero);
    }

    public void LeftMouseUp()
    {
        var pos = GetCursorPosition();
        mouse_event(MOUSEEVENTF_LEFTUP, pos.X, pos.Y, 0, IntPtr.Zero);
    }

    public void RightMouseDown()
    {
        var pos = GetCursorPosition();
        mouse_event(MOUSEEVENTF_RIGHTDOWN, pos.X, pos.Y, 0, IntPtr.Zero);
    }

    public void RightMouseUp()
    {
        var pos = GetCursorPosition();
        mouse_event(MOUSEEVENTF_RIGHTUP, pos.X, pos.Y, 0, IntPtr.Zero);
    }

    public void Click()
    {
        LeftMouseDown();
        LeftMouseUp();
    }

    public void Click(int x, int y)
    {
        SetCursorPosition(x, y);
        Click();
    }
}
