using Avalonia.Headless.XUnit;
using static DrawThatThing.Tests.Avalonia.ViewModelTestHelpers;

namespace DrawThatThing.Tests.Avalonia;

public class PaletteFileTests
{
    [AvaloniaFact]
    public async Task ExportedPalettesKeepTheBackgroundColor()
    {
        var (viewModel, _, dialogs) = CreateViewModel();
        var csv = TestFiles.NewPath(".csv");
        File.WriteAllLines(csv, ["X;Y;RGB;BG", "1;2;#FF0000;false", "3;4;#FFFFFF;true"]);
        dialogs.ImportPath = csv;
        await viewModel.ImportColorsCommand.ExecuteAsync(null);

        var exported = TestFiles.NewPath(".csv");
        dialogs.ExportPath = exported;
        await viewModel.ExportColorsCommand.ExecuteAsync(null);
        dialogs.ImportPath = exported;
        await viewModel.ImportColorsCommand.ExecuteAsync(null);

        var rows = viewModel.ColorPalette.Where(row => !row.IsNewRow).Select(row => (row.X, row.Y, row.Rgb, row.IsBackground));
        Assert.Equal([("1", "2", "#FF0000", false), ("3", "4", "#FFFFFF", true)], rows);
    }

    [AvaloniaFact]
    public async Task PalettesExportedWithoutTheBackgroundColumnCanStillBeImported()
    {
        var (viewModel, _, dialogs) = CreateViewModel();
        var csv = TestFiles.NewPath(".csv");
        File.WriteAllLines(csv, ["X;Y;RGB", "1;2;#FF0000"]);
        dialogs.ImportPath = csv;

        await viewModel.ImportColorsCommand.ExecuteAsync(null);

        var row = Assert.Single(viewModel.ColorPalette, row => !row.IsNewRow);
        Assert.Equal(("1", "2", "#FF0000", false), (row.X, row.Y, row.Rgb, row.IsBackground));
    }
}
