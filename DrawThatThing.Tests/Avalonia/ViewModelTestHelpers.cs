using System.Diagnostics;
using Avalonia.Threading;
using DrawThatThing.Avalonia.ViewModels;
using DrawThatThing.Platform;
using DrawThatThing.Tests.Fakes;

namespace DrawThatThing.Tests.Avalonia;

internal static class ViewModelTestHelpers
{
    public static (MainWindowViewModel ViewModel, FakePlatformServices Platform, FakeDialogService Dialogs) CreateViewModel()
    {
        var platform = new FakePlatformServices { Platform = PlatformType.Windows };
        var dialogs = new FakeDialogService();
        return (new MainWindowViewModel(platform, dialogs), platform, dialogs);
    }

    /// <summary>Runs the UI thread's queued work until the condition holds.</summary>
    public static void WaitUntil(Func<bool> condition, int timeoutMilliseconds = 5000)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            if (stopwatch.ElapsedMilliseconds > timeoutMilliseconds)
            {
                throw new TimeoutException("The condition was not met in time.");
            }
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(5);
        }
    }

    /// <summary>
    /// Parses a small image with PointReader and a palette with black at (900, 10) and white (which is not drawn),
    /// so PLAY has something to draw.
    /// </summary>
    public static async Task LoadDrawingAsync(MainWindowViewModel viewModel, FakeDialogService dialogs, params (int X, int Y)[] blackPixels)
    {
        viewModel.SelectedParser = "PointReader";
        AddPaletteRow(viewModel, "900", "10", "#000000");
        AddPaletteRow(viewModel, "900", "30", "#FFFFFF");
        dialogs.ImagePath = TestImages.CreatePng(4, 4, blackPixels);
        await viewModel.LoadImageCommand.ExecuteAsync(null);
        Assert.Empty(dialogs.Messages);
    }

    /// <summary>Types a row into the palette grid's empty last row.</summary>
    public static void AddPaletteRow(MainWindowViewModel viewModel, string x, string y, string rgb)
    {
        var newRow = viewModel.ColorPalette[^1];
        newRow.X = x;
        newRow.Y = y;
        newRow.Rgb = rgb;
    }
}
