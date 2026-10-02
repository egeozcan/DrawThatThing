using DrawThatThing.Platform.macOS;

namespace DrawThatThing.Tests.Platform;

// AppKit's menu bar is process-wide state, so these tests must not run in parallel with each other.
[Collection(nameof(MacAppIntegrationTests))]
public class MacAppIntegrationTests
{
    [MacOSFact]
    public void WithoutANativeDialogCommandVReachesTheAppsOwnWindow()
    {
        AppKit.ResetMainMenu();

        MacAppIntegration.OnWindowActivationChanged();

        // A menu item claiming Cmd+V (even a disabled one) would stop the app's text boxes from ever seeing the paste.
        Assert.False(AppKit.MenuBarClaimsCommandKey('v'));
    }

    [MacOSFact]
    public void WhileANativeDialogIsOpenTheEditMenuProvidesItsShortcuts()
    {
        AppKit.ResetMainMenu();

        using (MacAppIntegration.BeginNativeDialog())
        {
            Assert.True(AppKit.MenuBarClaimsCommandKey('v'));
        }

        Assert.False(AppKit.MenuBarClaimsCommandKey('v'));
    }

    [MacOSFact]
    public void TheEditMenuGoesNextToTheApplicationMenu()
    {
        AppKit.ResetMainMenu();

        using (MacAppIntegration.BeginNativeDialog())
        {
            Assert.Equal([AppKit.AppMenuTitle, "Edit"], AppKit.MenuBarTitles());
        }

        Assert.Equal([AppKit.AppMenuTitle], AppKit.MenuBarTitles());
    }

    [MacOSFact]
    public void TheEditMenuDoesNotTakeOverTheApplicationMenuWhileTheDialogIsOpen()
    {
        AppKit.ResetMainMenu();

        using (MacAppIntegration.BeginNativeDialog())
        {
            // The dialog sheet takes the focus, so Avalonia moves its application menu to the end of the menu bar,
            // where the Edit menu would end up first, i.e. shown as the application menu.
            AppKit.MoveAppMenuToTheEnd();
            MacAppIntegration.OnWindowActivationChanged();

            Assert.Equal([AppKit.AppMenuTitle, "Edit"], AppKit.MenuBarTitles());
            Assert.True(AppKit.MenuBarClaimsCommandKey('v'));
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

            Assert.True(AppKit.MenuBarClaimsCommandKey('v'));
        }

        Assert.False(AppKit.MenuBarClaimsCommandKey('v'));
    }
}
