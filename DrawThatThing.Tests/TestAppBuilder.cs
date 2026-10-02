using Avalonia;
using Avalonia.Headless;
using DrawThatThing.Tests;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace DrawThatThing.Tests;

public class TestAppBuilder
{
    // The real App provides the themes; without a desktop lifetime it does not create a main window itself.
    public static AppBuilder BuildAvaloniaApp() => AppBuilder
        .Configure<DrawThatThing.Avalonia.App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
