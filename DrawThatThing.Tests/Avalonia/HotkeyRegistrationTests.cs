using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using DrawThatThing.Avalonia.ViewModels;
using DrawThatThing.Platform;
using DrawThatThing.Tests.Fakes;

namespace DrawThatThing.Tests.Avalonia;

public class HotkeyRegistrationTests
{
    private const HotkeyModifiers ShiftAlt = HotkeyModifiers.Shift | HotkeyModifiers.Alt;

    private static (MainWindowViewModel ViewModel, FakePlatformServices Platform) CreateViewModel(params char[] takenKeys)
    {
        var platform = new FakePlatformServices { Platform = PlatformType.Windows };
        foreach (var key in takenKeys)
        {
            platform.FakeHotkeys.Taken.Add((ShiftAlt, key));
        }
        return (new MainWindowViewModel(platform, new FakeDialogService()), platform);
    }

    [AvaloniaFact]
    public void RegistersAllHotkeysSystemWide()
    {
        var (viewModel, platform) = CreateViewModel();

        viewModel.RegisterHotkeys();

        Assert.Equal(5, platform.FakeHotkeys.Registered.Count);
        Assert.All(platform.FakeHotkeys.Registered.Values, hotkey => Assert.Equal(ShiftAlt, hotkey.Modifiers));
        Assert.Equal("Shift + Alt + C", viewModel.StopMouseShortcutText);
    }

    [AvaloniaFact]
    public void AHotkeyTakenByAnotherAppDoesNotTurnOffTheOthers()
    {
        var (viewModel, platform) = CreateViewModel('S');

        viewModel.RegisterHotkeys();

        Assert.Equal(
            [
                MainWindowViewModel.StopMouseHotkey,
                MainWindowViewModel.PickColorHotkey,
                MainWindowViewModel.ToggleDebugHotkey,
                MainWindowViewModel.AddDebugPointHotkey
            ],
            platform.FakeHotkeys.Registered.Keys.Order());
        Assert.Equal("Shift + Alt + C", viewModel.StopMouseShortcutText);
    }

    [AvaloniaFact]
    public void AHotkeyThatCannotBeRegisteredStillWorksInTheWindowAndSaysSo()
    {
        var (viewModel, platform) = CreateViewModel('S');
        platform.FakeMouse.Position = (12, 34);
        viewModel.RegisterHotkeys();

        Assert.True(viewModel.TryHandleWindowHotkey(ShiftAlt, 's'));
        Assert.Equal(("12", "34"), (viewModel.MousePositionX, viewModel.MousePositionY));
        Assert.Equal("Shift + Alt + S", viewModel.SetStartPositionShortcutText);
        Assert.Contains("only works while this window is focused", viewModel.SetStartPositionShortcutWarning);
        Assert.Null(viewModel.StopMouseShortcutWarning);

        // Registered hotkeys are left to the system, which delivers them through HotkeyPressed.
        Assert.False(viewModel.TryHandleWindowHotkey(ShiftAlt, 'C'));
    }

    [AvaloniaFact]
    public void PressingASystemWideHotkeyRunsItsActionOnce()
    {
        var (viewModel, platform) = CreateViewModel();
        platform.FakeMouse.Position = (5, 6);
        viewModel.RegisterHotkeys();
        viewModel.RegisterHotkeys(); // e.g. the window was opened again

        platform.FakeHotkeys.Press(MainWindowViewModel.PickColorHotkey);
        Dispatcher.UIThread.RunJobs();

        Assert.Single(viewModel.ColorPalette, row => !row.IsNewRow);
    }
}
