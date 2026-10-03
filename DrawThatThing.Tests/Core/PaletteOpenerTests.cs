using DrawThatThing.Core.Extensions;
using DrawThatThing.Core.Models;

namespace DrawThatThing.Tests.Core;

public class PaletteOpenerTests
{
    private static readonly Color Red = new(255, 0, 0);

    [Fact]
    public void OnlyClicksThatChooseAColorWithAnOpenerGetOne()
    {
        List<ColorSpot> palette =
        [
            new(Color.Black, new Point(900, 10)),
            new(Red, new Point(850, 40)) { Opener = new Point(800, 5) }
        ];
        List<MouseDragAction> actions =
        [
            new([new Point(900, 10)], true, Color.Black),
            new([new Point(1, 2)]),
            new([new Point(850, 40)], true, Red),
            new([new Point(850, 40)])
        ];

        var result = actions.WithPaletteOpeners(palette).ToList();

        Assert.Equal(5, result.Count);
        Assert.Same(actions[1], result[1]);
        Assert.Equal([new Point(800, 5)], result[2].Points);
        Assert.True(result[2].DiscardOffset);
        Assert.True(result[2].Color.IsEmpty);
        Assert.Same(actions[2], result[3]);
        Assert.Same(actions[3], result[4]);
    }

    [Fact]
    public void TheFirstPaletteColorAtAPositionDecidesWhetherItsOpenerIsClicked()
    {
        // The parsers choose the first of two equal palette colors, which here is clicked directly.
        List<ColorSpot> palette =
        [
            new(Red, new Point(850, 40)),
            new(Red, new Point(850, 40)) { Opener = new Point(800, 5) }
        ];
        List<MouseDragAction> actions = [new([new Point(850, 40)], true, Red)];

        Assert.Equal(actions, actions.WithPaletteOpeners(palette));
    }

    [Fact]
    public void AColorWithoutAPositionDoesNotClickItsOpener()
    {
        // The color itself is not clicked, so opening the palette would leave it open over the drawing.
        List<ColorSpot> palette = [new(Red, Point.Empty) { Opener = new Point(800, 5) }];
        List<MouseDragAction> actions = [new([Point.Empty], true, Red)];

        Assert.Equal(actions, actions.WithPaletteOpeners(palette));
    }
}
