using Avalonia.Headless.XUnit;
using DrawThatThing.Avalonia.ViewModels;
using DrawThatThing.Platform;
using static DrawThatThing.Tests.Avalonia.ViewModelTestHelpers;

namespace DrawThatThing.Tests.Avalonia;

public class PickColorTests
{
    [AvaloniaFact]
    public void PickingAColorAddsItWithTheCursorPositionToThePalette()
    {
        var (viewModel, platform, _) = CreateViewModel();
        platform.FakeMouse.Position = (40, 50);
        platform.FakeScreen.Color = (0x12, 0xAB, 0xEF);

        viewModel.HandleHotkey(MainWindowViewModel.PickColorHotkey);

        var row = Assert.Single(viewModel.ColorPalette, row => !row.IsNewRow);
        Assert.Equal(("40", "50", "#12ABEF"), (row.X, row.Y, row.Rgb));
    }

    [AvaloniaFact]
    public void WhenTheScreenCannotBeReadNoMadeUpColorIsAdded()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        platform.Platform = PlatformType.macOS;
        platform.FakeScreen.Color = null;

        viewModel.HandleHotkey(MainWindowViewModel.PickColorHotkey);
        WaitUntil(() => dialogs.Messages.Count > 0);

        Assert.DoesNotContain(viewModel.ColorPalette, row => !row.IsNewRow);
        Assert.Contains("Screen & System Audio Recording", Assert.Single(dialogs.Messages));
    }
}
