using DrawThatThing.Platform;
using DrawThatThing.Platform.Linux;

namespace DrawThatThing.Tests.Platform;

public class LinuxHotkeyManagerTests
{
    /// <summary>Runs only where no X server can be reached, so the test never grabs real keys.</summary>
    public sealed class WithoutX11DisplayFactAttribute : FactAttribute
    {
        public WithoutX11DisplayFactAttribute()
        {
            if (OperatingSystem.IsLinux() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
            {
                Skip = "An X server is available.";
            }
        }
    }

    [WithoutX11DisplayFact]
    public void WithoutAnXServerRegistrationFailsAndDisposingIsSafe()
    {
        var manager = new LinuxHotkeyManager();

        Assert.False(manager.RegisterHotkey(0, HotkeyModifiers.Shift | HotkeyModifiers.Alt, 'C'));

        var exception = Record.Exception(manager.Dispose);
        Assert.Null(exception);
    }
}
