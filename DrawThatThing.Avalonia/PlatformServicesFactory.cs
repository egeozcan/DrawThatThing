using DrawThatThing.Platform;
using DrawThatThing.Platform.Linux;
using DrawThatThing.Platform.macOS;
using DrawThatThing.Platform.Windows;

namespace DrawThatThing.Avalonia;

public static class PlatformServicesFactory
{
    public static IPlatformServices? Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsPlatformServices();
        }
        if (OperatingSystem.IsMacOS())
        {
            return new MacPlatformServices();
        }
        if (OperatingSystem.IsLinux())
        {
            return new LinuxPlatformServices();
        }
        return null;
    }
}
