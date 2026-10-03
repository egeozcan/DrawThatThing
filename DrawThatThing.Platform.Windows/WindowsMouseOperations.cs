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

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private const uint INPUT_MOUSE = 0;

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    private static void SendButton(uint flags)
    {
        var input = new INPUT { type = INPUT_MOUSE, mi = new MOUSEINPUT { dwFlags = flags } };
        SendInput(1, [input], Marshal.SizeOf<INPUT>());
    }

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
        SendButton(MOUSEEVENTF_LEFTDOWN);
    }

    public void LeftMouseUp()
    {
        SendButton(MOUSEEVENTF_LEFTUP);
    }

    public void RightMouseDown()
    {
        SendButton(MOUSEEVENTF_RIGHTDOWN);
    }

    public void RightMouseUp()
    {
        SendButton(MOUSEEVENTF_RIGHTUP);
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
