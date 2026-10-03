using Avalonia.Headless.XUnit;
using DrawThatThing.Avalonia.ViewModels;
using static DrawThatThing.Tests.Avalonia.ViewModelTestHelpers;

namespace DrawThatThing.Tests.Avalonia;

public class PickPaletteTests
{
    /// <summary>A screen with 12-point swatches, 16 points apart, whose first one's top-left is at (100, 300).</summary>
    private static byte[] ScreenRegion(int x, int y, int width, int height)
    {
        var pixels = new byte[width * height * 4];
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                var (screenX, screenY) = (x + column, y + row);
                var color = (R: (byte)192, G: (byte)192, B: (byte)192);
                var (cellX, offsetX) = Math.DivRem(screenX - 100, 16);
                var (cellY, offsetY) = Math.DivRem(screenY - 300, 16);
                if (screenX >= 100 && screenY >= 300 && cellX < 4 && cellY < 2 && offsetX < 12 && offsetY < 12)
                {
                    var index = (byte)(cellY * 4 + cellX + 1);
                    color = (index, index, index);
                }
                var o = (row * width + column) * 4;
                (pixels[o], pixels[o + 1], pixels[o + 2], pixels[o + 3]) = (color.R, color.G, color.B, 255);
            }
        }
        return pixels;
    }

    private static List<(string X, string Y, string Rgb)> Rows(MainWindowViewModel viewModel) =>
        viewModel.ColorPalette.Where(row => !row.IsNewRow).Select(row => (row.X, row.Y, row.Rgb)).ToList();

    [AvaloniaFact]
    public void PickingSimilarSwatchesAddsTheWholePalette()
    {
        var (viewModel, platform, _) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        platform.FakeMouse.Position = (106, 306);

        viewModel.HandleHotkey(MainWindowViewModel.FindSwatchesHotkey);

        var rows = Rows(viewModel);
        Assert.Equal(8, rows.Count);
        Assert.Equal(("106", "306", "#010101"), rows[0]);
        Assert.Equal(("154", "306", "#040404"), rows[3]);
        Assert.Equal(("106", "322", "#050505"), rows[4]);
        Assert.Contains("Added 8", viewModel.PaletteStatus);
    }

    [AvaloniaFact]
    public void ColorsThePaletteAlreadyHasAreNotAddedAgain()
    {
        var (viewModel, platform, _) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        platform.FakeMouse.Position = (106, 306);
        viewModel.HandleHotkey(MainWindowViewModel.FindSwatchesHotkey);

        viewModel.HandleHotkey(MainWindowViewModel.FindSwatchesHotkey);

        Assert.Equal(8, Rows(viewModel).Count);
        Assert.Contains("8 skipped", viewModel.PaletteStatus);
    }

    [AvaloniaFact]
    public void HoveringSomethingThatIsNotASwatchSaysSo()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        platform.FakeMouse.Position = (700, 700);

        viewModel.HandleHotkey(MainWindowViewModel.FindSwatchesHotkey);
        WaitUntil(() => dialogs.Messages.Count > 0);

        Assert.Empty(Rows(viewModel));
        Assert.Contains("No color swatch", Assert.Single(dialogs.Messages));
    }

    [AvaloniaFact]
    public void AnUnreadableScreenAddsNothing()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        platform.FakeScreen.Color = null;

        viewModel.HandleHotkey(MainWindowViewModel.FindSwatchesHotkey);
        WaitUntil(() => dialogs.Messages.Count > 0);

        Assert.Empty(Rows(viewModel));
    }

    [AvaloniaFact]
    public void TwoCornerPressesPickAGrid()
    {
        var (viewModel, platform, _) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        viewModel.GridColumns = "4";
        viewModel.GridRows = "2";

        platform.FakeMouse.Position = (106, 306);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        Assert.Empty(Rows(viewModel));
        Assert.Contains("106,306", viewModel.PaletteStatus);

        platform.FakeMouse.Position = (154, 322);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);

        var rows = Rows(viewModel);
        Assert.Equal(8, rows.Count);
        Assert.Equal(("106", "306", "#010101"), rows[0]);
        Assert.Equal(("154", "322", "#080808"), rows[7]);
    }

    [AvaloniaFact]
    public void AnInvalidGridSizeAddsNothingAndStartsOver()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        viewModel.GridColumns = "abc";

        platform.FakeMouse.Position = (106, 306);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        platform.FakeMouse.Position = (154, 322);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        WaitUntil(() => dialogs.Messages.Count > 0);

        Assert.Empty(Rows(viewModel));

        viewModel.GridColumns = "4";
        platform.FakeMouse.Position = (106, 306);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        Assert.Contains("Grid starts", viewModel.PaletteStatus);
    }

    [AvaloniaFact]
    public void CornersOnTheSameColumnCannotSpanSeveralColumns()
    {
        var (viewModel, platform, dialogs) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        viewModel.GridColumns = "4";
        platform.FakeMouse.Position = (106, 306);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);

        platform.FakeMouse.Position = (106, 322);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        WaitUntil(() => dialogs.Messages.Count > 0);

        Assert.Empty(Rows(viewModel));
        Assert.Contains("different swatches", Assert.Single(dialogs.Messages));
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        Assert.Contains("Grid starts", viewModel.PaletteStatus);
    }

    [AvaloniaFact]
    public void AnotherPaletteHotkeyCancelsAPendingGridCorner()
    {
        var (viewModel, platform, _) = CreateViewModel();
        platform.FakeScreen.Region = ScreenRegion;
        viewModel.GridColumns = "4";
        platform.FakeMouse.Position = (106, 306);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);
        viewModel.HandleHotkey(MainWindowViewModel.PickColorHotkey);

        platform.FakeMouse.Position = (154, 322);
        viewModel.HandleHotkey(MainWindowViewModel.PickGridCornerHotkey);

        Assert.Single(Rows(viewModel)); // only the color picked directly
        Assert.Contains("Grid starts", viewModel.PaletteStatus);
    }

    [AvaloniaFact]
    public void ASwatchNearTheTopLeftOfTheScreenIsStillFound()
    {
        var (viewModel, platform, _) = CreateViewModel();
        // Like a real capture of a rectangle partly off the screen: the off-screen part is blank.
        platform.FakeScreen.Region = (x, y, w, h) =>
        {
            var pixels = new byte[w * h * 4];
            for (var row = 0; row < h; row++)
            {
                for (var column = 0; column < w; column++)
                {
                    var (sx, sy) = (x + column, y + row);
                    if (sx < 0 || sy < 0)
                    {
                        continue;
                    }
                    var inside = sx is >= 5 and < 17 && sy is >= 5 and < 17;
                    var o = (row * w + column) * 4;
                    (pixels[o], pixels[o + 1], pixels[o + 2]) = inside ? ((byte)9, (byte)9, (byte)9) : ((byte)192, (byte)192, (byte)192);
                }
            }
            return pixels;
        };
        platform.FakeMouse.Position = (11, 11);

        viewModel.HandleHotkey(MainWindowViewModel.FindSwatchesHotkey);

        Assert.Equal(("11", "11", "#090909"), Assert.Single(Rows(viewModel)));
    }
}
