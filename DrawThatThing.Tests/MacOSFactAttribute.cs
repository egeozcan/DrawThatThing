namespace DrawThatThing.Tests;

/// <summary>A test that talks to macOS frameworks and is skipped on other systems.</summary>
public sealed class MacOSFactAttribute : FactAttribute
{
    public MacOSFactAttribute()
    {
        if (!OperatingSystem.IsMacOS())
        {
            Skip = "Needs macOS.";
        }
    }
}
