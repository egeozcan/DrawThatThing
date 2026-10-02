using DrawThatThing.Platform.macOS;

namespace DrawThatThing.Tests.Platform;

// AppKit's menu bar is process-wide state, so these tests must not run in parallel with each other.
[Collection(nameof(MacAppIntegrationTests))]
public class MacAppIntegrationTests
{
    private const ushort KeyCodeV = 0x09;

    [MacOSFact]
    public void WithoutANativeDialogCommandVReachesTheAppsOwnWindow()
    {
        AppKit.ResetMainMenu();

        MacAppIntegration.OnWindowActivated();

        // A menu item claiming Cmd+V (even a disabled one) would stop the app's text boxes from ever seeing the paste.
        Assert.False(AppKit.MenuBarClaimsCommandKey('v', KeyCodeV));
    }

    [MacOSFact]
    public void WhileANativeDialogIsOpenTheEditMenuProvidesItsShortcuts()
    {
        AppKit.ResetMainMenu();

        using (MacAppIntegration.BeginNativeDialog())
        {
            Assert.True(AppKit.MenuBarClaimsCommandKey('v', KeyCodeV));
        }

        Assert.False(AppKit.MenuBarClaimsCommandKey('v', KeyCodeV));
    }

    [MacOSFact]
    public void TheEditMenuComesBackWhenTheMenuBarIsRebuiltDuringADialog()
    {
        AppKit.ResetMainMenu();

        using (MacAppIntegration.BeginNativeDialog())
        {
            AppKit.ResetMainMenu(); // the toolkit rebuilds the menu bar when the window is activated
            MacAppIntegration.OnWindowActivated();

            Assert.True(AppKit.MenuBarClaimsCommandKey('v', KeyCodeV));
        }
    }

    [MacOSFact]
    public void NestedDialogsKeepTheEditMenuUntilTheLastOneCloses()
    {
        AppKit.ResetMainMenu();

        using (MacAppIntegration.BeginNativeDialog())
        {
            using (MacAppIntegration.BeginNativeDialog())
            {
            }

            Assert.True(AppKit.MenuBarClaimsCommandKey('v', KeyCodeV));
        }

        Assert.False(AppKit.MenuBarClaimsCommandKey('v', KeyCodeV));
    }
}
