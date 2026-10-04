using Avalonia.Headless.XUnit;
using static DrawThatThing.Tests.Avalonia.ViewModelTestHelpers;

namespace DrawThatThing.Tests.Avalonia;

public class PaletteFileTests
{
    [AvaloniaFact]
    public async Task ExportedPalettesKeepTheBackgroundColorAndOpeners()
    {
        var (viewModel, _, dialogs) = CreateViewModel();
        var csv = TestFiles.NewPath(".csv");
        File.WriteAllLines(csv, ["X;Y;RGB;BG;Opener", "1;2;#FF0000;false;false", "3;4;#FFFFFF;true;false", "5;6;#808080;false;true"]);
        dialogs.ImportPath = csv;
        await viewModel.ImportColorsCommand.ExecuteAsync(null);

        var exported = TestFiles.NewPath(".csv");
        dialogs.ExportPath = exported;
        await viewModel.ExportColorsCommand.ExecuteAsync(null);
        dialogs.ImportPath = exported;
        await viewModel.ImportColorsCommand.ExecuteAsync(null);

        var rows = viewModel.ColorPalette.Where(row => !row.IsNewRow).Select(row => (row.X, row.Y, row.Rgb, row.IsBackground, row.IsOpener));
        Assert.Equal([("1", "2", "#FF0000", false, false), ("3", "4", "#FFFFFF", true, false), ("5", "6", "#808080", false, true)], rows);
    }

    [AvaloniaFact]
    public async Task OpenerRowsWithoutAColorSurviveImport()
    {
        var (viewModel, _, dialogs) = CreateViewModel();
        var csv = TestFiles.NewPath(".csv");
        File.WriteAllLines(csv, ["X;Y;RGB;BG;Opener", "800;5;;False;True", "1;2;#FF0000;false;false"]);
        dialogs.ImportPath = csv;
        await viewModel.ImportColorsCommand.ExecuteAsync(null);

        var rows = viewModel.ColorPalette.Where(row => !row.IsNewRow).Select(row => (row.X, row.Y, row.IsOpener));
        Assert.Equal([("800", "5", true), ("1", "2", false)], rows);
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
