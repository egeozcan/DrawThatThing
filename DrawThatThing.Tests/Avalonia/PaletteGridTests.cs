using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using DrawThatThing.Avalonia.Views;
using static DrawThatThing.Tests.Avalonia.ViewModelTestHelpers;

namespace DrawThatThing.Tests.Avalonia;

public class PaletteGridTests
{
    private static (MainWindow Window, DrawThatThing.Avalonia.ViewModels.MainWindowViewModel ViewModel) ShowWindowWithSelectedRow()
    {
        var (viewModel, _, _) = CreateViewModel();
        AddPaletteRow(viewModel, "1", "2", "#FF0000");
        var window = new MainWindow { DataContext = viewModel };
        window.Show();
        var grid = window.FindControl<DataGrid>("PaletteGrid")!;
        grid.SelectedItem = viewModel.ColorPalette[0];
        grid.Focus();
        return (window, viewModel);
    }

    private static int RealRows(DrawThatThing.Avalonia.ViewModels.MainWindowViewModel viewModel) =>
        viewModel.ColorPalette.Count(row => !row.IsNewRow);

    [AvaloniaFact]
    public void DeleteRemovesTheSelectedRow()
    {
        var (window, viewModel) = ShowWindowWithSelectedRow();

        window.KeyPress(Key.Delete, RawInputModifiers.None, PhysicalKey.Delete, null);

        Assert.Equal(0, RealRows(viewModel));
    }

    [AvaloniaFact]
    public void BackspaceDoesNotDeleteARowThatWasOnlyClicked()
    {
        // Backspace is how a Mac user starts correcting a cell; it must not throw the row (and its picked position) away.
        var (window, viewModel) = ShowWindowWithSelectedRow();

        window.KeyPress(Key.Back, RawInputModifiers.None, PhysicalKey.Backspace, null);

        Assert.Equal(1, RealRows(viewModel));
    }

    [AvaloniaFact]
    public void CommandBackspaceDeletesLikeInOtherMacApps()
    {
        var (window, viewModel) = ShowWindowWithSelectedRow();

        window.KeyPress(Key.Back, RawInputModifiers.Meta, PhysicalKey.Backspace, null);

        Assert.Equal(0, RealRows(viewModel));
    }
}
