using System.Runtime.InteropServices;

namespace DrawThatThing.Platform.Linux;

public class LinuxScreenCapture : IScreenCapture
{
    private const string X11 = "libX11.so.6";

    [DllImport(X11)]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport(X11)]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport(X11)]
    private static extern int XDefaultScreen(IntPtr display);

    [DllImport(X11)]
    private static extern IntPtr XRootWindow(IntPtr display, int screen);

    [DllImport(X11)]
    private static extern IntPtr XGetImage(IntPtr display, IntPtr drawable, int x, int y, uint width, uint height, ulong plane_mask, int format);

    [DllImport(X11)]
    private static extern int XDestroyImage(IntPtr ximage);

    [DllImport(X11)]
    private static extern uint XGetPixel(IntPtr ximage, int x, int y);

    private const int ZPixmap = 2;
    private const ulong AllPlanes = 0xFFFFFFFF;

    [StructLayout(LayoutKind.Sequential)]
    private struct XImage
    {
        public int width;
        public int height;
        public int xoffset;
        public int format;
        public IntPtr data;
        public int byte_order;
        public int bitmap_unit;
        public int bitmap_bit_order;
        public int bitmap_pad;
        public int depth;
        public int bytes_per_line;
        public int bits_per_pixel;
        public ulong red_mask;
        public ulong green_mask;
        public ulong blue_mask;
        public IntPtr obdata;
    }

    public (byte R, byte G, byte B) GetPixelColor(int x, int y)
    {
        IntPtr display = XOpenDisplay(IntPtr.Zero);
        if (display == IntPtr.Zero)
        {
            return (0, 0, 0);
        }

        try
        {
            int screen = XDefaultScreen(display);
            IntPtr root = XRootWindow(display, screen);
            IntPtr image = XGetImage(display, root, x, y, 1, 1, AllPlanes, ZPixmap);

            if (image == IntPtr.Zero)
            {
                return (0, 0, 0);
            }

            try
            {
                uint pixel = XGetPixel(image, 0, 0);
                byte r = (byte)((pixel >> 16) & 0xFF);
                byte g = (byte)((pixel >> 8) & 0xFF);
                byte b = (byte)(pixel & 0xFF);
                return (r, g, b);
            }
            finally
            {
                XDestroyImage(image);
            }
        }
        finally
        {
            XCloseDisplay(display);
        }
    }

    public byte[] CaptureRegion(int x, int y, int width, int height)
    {
        byte[] pixels = new byte[width * height * 4];

        IntPtr display = XOpenDisplay(IntPtr.Zero);
        if (display == IntPtr.Zero)
        {
            return pixels;
        }

        try
        {
            int screen = XDefaultScreen(display);
            IntPtr root = XRootWindow(display, screen);
            IntPtr image = XGetImage(display, root, x, y, (uint)width, (uint)height, AllPlanes, ZPixmap);

            if (image == IntPtr.Zero)
            {
                return pixels;
            }

            try
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        uint pixel = XGetPixel(image, col, row);
                        int index = (row * width + col) * 4;

                        pixels[index] = (byte)((pixel >> 16) & 0xFF);     // R
                        pixels[index + 1] = (byte)((pixel >> 8) & 0xFF);  // G
                        pixels[index + 2] = (byte)(pixel & 0xFF);         // B
                        pixels[index + 3] = 255;                          // A
                    }
                }
            }
            finally
            {
                XDestroyImage(image);
            }
        }
        finally
        {
            XCloseDisplay(display);
        }

        return pixels;
    }
}
