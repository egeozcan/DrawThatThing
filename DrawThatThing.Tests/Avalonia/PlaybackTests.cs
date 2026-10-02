using System.Globalization;
using Avalonia.Headless.XUnit;
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
