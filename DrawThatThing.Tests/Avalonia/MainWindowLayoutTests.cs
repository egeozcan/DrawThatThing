using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DrawThatThing.Avalonia.ViewModels;
using DrawThatThing.Avalonia.Views;
using DrawThatThing.Platform;
using DrawThatThing.Tests.Fakes;

namespace DrawThatThing.Tests.Avalonia;

public class MainWindowLayoutTests
{
    [AvaloniaTheory]
    [InlineData(PlatformType.Windows)]
    [InlineData(PlatformType.macOS)]
    public void PlayStaysInsideTheWindowWhenAHotkeyOnlyWorksInTheWindow(PlatformType platformType)
    {
        var platform = new FakePlatformServices { Platform = platformType };
        foreach (var key in new[] { 'C', 'S', 'A' })
        {
            platform.FakeHotkeys.Taken.Add((HotkeyModifiers.Shift | HotkeyModifiers.Alt, key));
            platform.FakeHotkeys.Taken.Add((HotkeyModifiers.Ctrl | HotkeyModifiers.Alt, key));
        }
        var viewModel = new MainWindowViewModel(platform, new FakeDialogService());
        viewModel.RegisterHotkeys();
        var window = new MainWindow { DataContext = viewModel };
        window.Show();
        window.Width = window.MinWidth;
        window.Height = 660;
        Dispatcher.UIThread.RunJobs();
        window.UpdateLayout();

        var play = window.FindControl<Button>("PlayButton")!;
        var playRight = play.TranslatePoint(new Point(play.Bounds.Width, 0), window)!.Value.X;

        Assert.True(playRight <= window.ClientSize.Width, $"PLAY ends at {playRight}, the window is {window.ClientSize.Width} wide.");

        // Instead of a longer label, the shortcut is marked and explains itself in a tooltip.
        var stopLabel = window.GetVisualDescendants().OfType<TextBlock>().Single(t => t.Text == viewModel.StopMouseShortcutText);
        Assert.Contains("windowOnly", stopLabel.Classes);
        Assert.Equal(viewModel.StopMouseShortcutWarning, ToolTip.GetTip(stopLabel));
    }
}
