using System.Globalization;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using DrawThatThing.Avalonia.ViewModels;
using static DrawThatThing.Tests.Avalonia.ViewModelTestHelpers;

namespace DrawThatThing.Tests.Avalonia;

public class PlaybackTests
{
    [AvaloniaFact]
    public async Task PlayDrawsTheParsedImageAtTheStartPosition()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        await LoadDrawingAsync(viewModel, dialogs, (1, 2));
        viewModel.MousePositionX = "100";
        viewModel.MousePositionY = "200";

        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => platform.FakeMouse.Log.Count(op => op == "up") == 2);

        Assert.Equal(
            ["move 900,10", "down", "move 900,10", "up", "move 101,202", "down", "move 101,202", "up"],
            platform.FakeMouse.Log);
    }

    [AvaloniaFact]
    public async Task APaletteColorWithoutAPositionIsNotClicked()
    {
        // E.g. the user only typed the RGB value; clicking (0, 0) instead would open the Apple menu on macOS.
        var (viewModel, platform, dialogs) = CreateViewModel();
        viewModel.SelectedParser = "PointReader";
        AddPaletteRow(viewModel, "", "", "#000000");
        AddPaletteRow(viewModel, "abc", "30", "#FFFFFF");
        dialogs.ImagePath = TestImages.CreatePng(4, 4, (1, 2));
        await viewModel.LoadImageCommand.ExecuteAsync(null);

        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => platform.FakeMouse.Log.Contains("up"));
        Thread.Sleep(200);

        Assert.Equal(["move 1,2", "down", "move 1,2", "up"], platform.FakeMouse.Log);
    }

    [AvaloniaFact]
    public async Task TheOpenerIsClickedBeforeChoosingAColorListedBelowIt()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        viewModel.SelectedParser = "PointReader";
        // The opener's own color is not a palette color, so the black pixel is not drawn by clicking the opener.
        AddPaletteRow(viewModel, "800", "5", "#000000");
        viewModel.ColorPalette[^2].IsOpener = true;
        AddPaletteRow(viewModel, "850", "40", "#000000");
        AddPaletteRow(viewModel, "900", "30", "#FFFFFF");
        dialogs.ImagePath = TestImages.CreatePng(4, 4, (1, 2));
        await viewModel.LoadImageCommand.ExecuteAsync(null);
        Assert.Empty(dialogs.Messages);

        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => platform.FakeMouse.Log.Count(op => op == "up") == 3);
        Thread.Sleep(200);

        Assert.Equal(
            [
                "move 800,5", "down", "move 800,5", "up",
                "move 850,40", "down", "move 850,40", "up",
                "move 1,2", "down", "move 1,2", "up"
            ],
            platform.FakeMouse.Log);
    }

    [AvaloniaFact]
    public async Task EachColorIsOpenedByTheNearestOpenerAboveIt()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        viewModel.SelectedParser = "PointReader";
        AddPaletteRow(viewModel, "900", "30", "#FFFFFF");
        AddPaletteRow(viewModel, "700", "5", "#808080");
        viewModel.ColorPalette[^2].IsOpener = true;
        AddPaletteRow(viewModel, "750", "40", "#FF0000");
        AddPaletteRow(viewModel, "800", "5", "#808080");
        viewModel.ColorPalette[^2].IsOpener = true;
        AddPaletteRow(viewModel, "850", "40", "#000000");
        dialogs.ImagePath = TestImages.CreatePng(4, 4, (1, 2));
        await viewModel.LoadImageCommand.ExecuteAsync(null);
        Assert.Empty(dialogs.Messages);

        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => platform.FakeMouse.Log.Count(op => op == "up") == 3);
        Thread.Sleep(200);

        Assert.Equal(
            [
                "move 800,5", "down", "move 800,5", "up",
                "move 850,40", "down", "move 850,40", "up",
                "move 1,2", "down", "move 1,2", "up"
            ],
            platform.FakeMouse.Log);
    }

    [AvaloniaFact]
    public async Task AnOpenerWithoutAPositionIsReportedInsteadOfClickingHiddenColors()
    {
        var (viewModel, _, dialogs) = CreateViewModel();
        viewModel.SelectedParser = "PointReader";
        AddPaletteRow(viewModel, "", "", "#808080");
        viewModel.ColorPalette[^2].IsOpener = true;
        AddPaletteRow(viewModel, "850", "40", "#000000");
        dialogs.ImagePath = TestImages.CreatePng(4, 4, (1, 2));

        await viewModel.LoadImageCommand.ExecuteAsync(null);

        Assert.Equal(["Every opener row needs a position (X and Y)."], dialogs.Messages);
        Assert.Null(viewModel.PreviewImage);
    }

    [AvaloniaFact]
    public async Task PlaybackErrorsAreShownToTheUser()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        await LoadDrawingAsync(viewModel, dialogs, (1, 2));
        platform.FakeMouse.ThrowOnMove = new InvalidOperationException("The mouse could not be moved.");

        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => dialogs.Messages.Count > 0);

        Assert.Equal(["The mouse could not be moved."], dialogs.Messages);
    }

    [AvaloniaFact]
    public async Task StopAlsoStopsTheDebugTestPlayback()
    {
        var (viewModel, platform, _) = CreateViewModel();
        viewModel.DebugRoutes = string.Join(", ", Enumerable.Range(0, 1000).Select(i => $"{i}|0"));
        platform.FakeMouse.OnOperation = operation =>
        {
            if (operation == "move 5,0")
            {
                // Hotkeys are handled on the UI thread.
                global::Avalonia.Threading.Dispatcher.UIThread.Post(() => viewModel.HandleHotkey(MainWindowViewModel.StopMouseHotkey));
            }
        };

        var testPlayback = viewModel.PlayDebugPointsCommand.ExecuteAsync(null);
        WaitUntil(() => testPlayback.IsCompleted);
        await testPlayback;

        var moves = platform.FakeMouse.Log.Count(op => op.StartsWith("move"));
        Assert.True(moves < 100, $"{moves} points were played after Stop.");
        Assert.Equal("up", platform.FakeMouse.Log[^1]);
    }

    [AvaloniaFact]
    public async Task StopStillWorksForAPlaybackStartedWhileAnErrorMessageIsOpen()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        await LoadDrawingAsync(viewModel, dialogs, (1, 2));
        dialogs.KeepMessagesOpen = true;
        platform.FakeMouse.ThrowOnMove = new InvalidOperationException("The mouse could not be moved.");
        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => dialogs.Messages.Count > 0);

        platform.FakeMouse.ThrowOnMove = null;
        viewModel.DebugRoutes = string.Join(", ", Enumerable.Range(0, 1000).Select(i => $"{i}|0"));
        var testPlayback = viewModel.PlayDebugPointsCommand.ExecuteAsync(null);
        WaitUntil(() => platform.FakeMouse.Log.Count > 0);
        dialogs.CloseMessages();
        Dispatcher.UIThread.RunJobs();
        viewModel.HandleHotkey(MainWindowViewModel.StopMouseHotkey);

        WaitUntil(() => testPlayback.IsCompleted, timeoutMilliseconds: 2000);
        Assert.True(platform.FakeMouse.Log.Count < 500);
    }

    [AvaloniaFact]
    public void QuittingStopsThePlaybackAndReleasesTheButton()
    {
        var (viewModel, platform, _) = CreateViewModel();
        viewModel.DebugRoutes = string.Join(", ", Enumerable.Range(0, 1000).Select(i => $"{i}|0"));
        var testPlayback = viewModel.PlayDebugPointsCommand.ExecuteAsync(null);
        WaitUntil(() => platform.FakeMouse.Log.Count > 0);

        viewModel.Shutdown();

        Assert.True(platform.FakeMouse.Log.Count < 500);
        Assert.Equal("up", platform.FakeMouse.Log[^1]);
        Assert.True(platform.FakeHotkeys.Disposed);
    }

    [AvaloniaFact]
    public async Task PlayAndTestNeverDriveTheMouseAtTheSameTime()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        await LoadDrawingAsync(viewModel, dialogs, (1, 2));
        viewModel.DebugRoutes = string.Join(", ", Enumerable.Range(0, 200).Select(i => $"{i}|0")); // about 2 s

        var testPlayback = viewModel.PlayDebugPointsCommand.ExecuteAsync(null);
        viewModel.PlayCommand.Execute(null);
        WaitUntil(() => testPlayback.IsCompleted);
        await testPlayback;
        Thread.Sleep(200);

        Assert.DoesNotContain("move 900,10", platform.FakeMouse.Log);
    }

    [AvaloniaFact]
    public void DebugPointsAreWrittenSoTheTestPlaybackCanReadThemInAnyLanguage()
    {
        var (viewModel, platform, _) = CreateViewModel();
        platform.FakeMouse.Position = (-100, 5); // a monitor left of the main one
        var culture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("sv-SE"); // writes minus signs as U+2212
        try
        {
            viewModel.HandleHotkey(MainWindowViewModel.AddDebugPointHotkey);
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }

        Assert.Equal("-100|5", viewModel.DebugRoutes);
    }
}
