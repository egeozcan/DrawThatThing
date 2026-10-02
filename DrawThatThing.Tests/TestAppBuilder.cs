using Avalonia;
using Avalonia.Headless;
using DrawThatThing.Tests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace DrawThatThing.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder
        .Configure<Application>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
