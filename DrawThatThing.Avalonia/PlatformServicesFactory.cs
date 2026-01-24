using System.Runtime.InteropServices;
using DrawThatThing.Platform;

namespace DrawThatThing.Avalonia;

public static class PlatformServicesFactory
{
    public static IPlatformServices? Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return CreateWindowsServices();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return CreateMacServices();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return CreateLinuxServices();
        }

        return null;
    }

    private static IPlatformServices? CreateWindowsServices()
    {
#if WINDOWS
        return new DrawThatThing.Platform.Windows.WindowsPlatformServices();
#else
        // Try to load dynamically
        try
        {
            var assembly = System.Reflection.Assembly.Load("DrawThatThing.Platform.Windows");
            var type = assembly.GetType("DrawThatThing.Platform.Windows.WindowsPlatformServices");
            if (type != null)
            {
                return (IPlatformServices?)Activator.CreateInstance(type);
            }
        }
        catch
        {
            // Platform assembly not available
        }
        return null;
#endif
    }

    private static IPlatformServices? CreateMacServices()
    {
        try
        {
            var assembly = System.Reflection.Assembly.Load("DrawThatThing.Platform.macOS");
            var type = assembly.GetType("DrawThatThing.Platform.macOS.MacPlatformServices");
            if (type != null)
            {
                return (IPlatformServices?)Activator.CreateInstance(type);
            }
        }
        catch
        {
            // Platform assembly not available
        }
        return null;
    }

    private static IPlatformServices? CreateLinuxServices()
    {
        try
        {
            var assembly = System.Reflection.Assembly.Load("DrawThatThing.Platform.Linux");
            var type = assembly.GetType("DrawThatThing.Platform.Linux.LinuxPlatformServices");
            if (type != null)
            {
                return (IPlatformServices?)Activator.CreateInstance(type);
            }
        }
        catch
        {
            // Platform assembly not available
        }
        return null;
    }
}
