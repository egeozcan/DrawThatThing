using DrawThatThing.Core.Imaging;

namespace DrawThatThing.Tests.Core;

public class PaletteGridMathTests
{
    [Fact]
    public void CellCentersAreSpacedEvenlyRowByRow()
    {
        var centers = PaletteGrid.CellCenters(10, 20, 40, 30, 4, 2);

        Assert.Equal([(10, 20), (20, 20), (30, 20), (40, 20), (10, 30), (20, 30), (30, 30), (40, 30)], centers);
    }

    [Fact]
    public void CornersCanBeGivenInAnyOrder()
    {
        Assert.Equal(PaletteGrid.CellCenters(10, 20, 40, 30, 4, 2), PaletteGrid.CellCenters(40, 30, 10, 20, 4, 2));
    }

    [Fact]
    public void AColumnOrRowOfOneUsesTheFirstCorner()
    {
        Assert.Equal([(10, 20), (30, 20)], PaletteGrid.CellCenters(10, 20, 30, 99, 2, 1));
        Assert.Equal([(10, 20)], PaletteGrid.CellCenters(10, 20, 30, 99, 1, 1));
    }

    [Fact]
    public void ADegenerateGridHasNoCells()
    {
        Assert.Empty(PaletteGrid.CellCenters(0, 0, 10, 10, 0, 2));
    }

    [Fact]
    public void TheDominantColorIgnoresAStrayPixel()
    {
        var pixels = new byte[3 * 3 * 4];
        for (var i = 0; i < 9; i++)
        {
            (pixels[i * 4], pixels[i * 4 + 1], pixels[i * 4 + 2]) = (10, 20, 30);
        }
        pixels[0] = 200; // the top-left pixel

        Assert.Equal((10, 20, 30), PaletteGrid.DominantColor(pixels, 3, 3, 1, 1, 1));
    }
}
