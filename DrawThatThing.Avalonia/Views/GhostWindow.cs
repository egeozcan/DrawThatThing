using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Runtime.InteropServices;

namespace DrawThatThing.Avalonia.Views;

/// <summary>
/// A translucent, click-through window showing the image that will be drawn, so the start position can be judged
/// against whatever is on the screen.
/// </summary>
public sealed class GhostWindow : Window
{
    private readonly Image _image = new()
    {
        Stretch = Stretch.Fill,
        Opacity = 0.5,
        IsHitTestVisible = false
    };

    public GhostWindow()
    {
        SystemDecorations = SystemDecorations.None;
        ShowInTaskbar = false;
        ShowActivated = false;
        Topmost = true;
        CanResize = false;
        Focusable = false;
        IsHitTestVisible = false;
        TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
        Background = Brushes.Transparent;
        Content = _image;
        RenderOptions.SetBitmapInterpolationMode(_image, BitmapInterpolationMode.None);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        MakeClickThrough();
    }

    /// <summary>
    /// Avalonia's IsHitTestVisible only affects its own hit testing; the operating system still delivers clicks to the
    /// window (and activates it), so the window has to be told to ignore the mouse natively.
    /// </summary>
    private void MakeClickThrough()
    {
        var handle = TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        try
        {
            if (OperatingSystem.IsMacOS())
            {
                objc_msgSend(handle, sel_registerName("setIgnoresMouseEvents:"), true);
            }
            else if (OperatingSystem.IsWindows())
            {
                const int GwlExStyle = -20;
                const long Transparent = 0x20, Layered = 0x80000, ToolWindow = 0x80, NoActivate = 0x08000000;
                var style = GetWindowLongPtr(handle, GwlExStyle).ToInt64();
                SetWindowLongPtr(handle, GwlExStyle, new IntPtr(style | Transparent | Layered | ToolWindow | NoActivate));
            }
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            // Without click-through the ghost is only a nuisance, not a reason to fail.
        }
    }

    [DllImport("/usr/lib/libobjc.dylib")]
    private static extern IntPtr sel_registerName(string name);

    [DllImport("/usr/lib/libobjc.dylib")]
    private static extern void objc_msgSend(IntPtr receiver, IntPtr selector, [MarshalAs(UnmanagedType.I1)] bool value);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int index);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int index, IntPtr value);

    /// <summary>
    /// Shows the image with its top-left corner at the given cursor position.
    /// </summary>
    /// <param name="cursorUnitsArePoints">
    /// True where cursor coordinates are device-independent points (macOS) rather than physical pixels.
    /// </param>
    public void Follow(Bitmap image, int cursorX, int cursorY, bool cursorUnitsArePoints)
    {
        var scaling = RenderScaling;
        _image.Source = image;
        // One image pixel is one cursor unit, which is what playback offsets the drawing by.
        Width = cursorUnitsArePoints ? image.PixelSize.Width : image.PixelSize.Width / scaling;
        Height = cursorUnitsArePoints ? image.PixelSize.Height : image.PixelSize.Height / scaling;
        // Avalonia positions windows in points on macOS too, so the cursor location needs no conversion.
        Position = new PixelPoint(cursorX, cursorY);
        if (!IsVisible)
        {
            Show();
        }
    }
}
