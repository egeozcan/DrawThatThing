using DrawThatThing.Core.Interfaces;
using DrawThatThing.Core.Models;
using DrawThatThing.Core.Readers;

namespace DrawThatThing.Tests.Core;

public class BitmapReaderCatalogTests
{
    private sealed class PickyReader : IBitmapReader
    {
        public PickyReader(string path)
        {
            throw new ArgumentException($"PickyReader cannot read {Path.GetFileName(path)}.");
        }

        public IEnumerable<MouseDragAction> GetDrawInstructions(List<ColorSpot> paletteColorSpots, IDictionary<string, string>? settings = null, IBrushChanger? brushChanger = null) => [];
    }

    [Fact]
    public void ACopyOfTheCoreLibraryInThePluginsFolderDoesNotAddParsers()
    {
        // Copying a plugin's build output also copies DrawThatThing.Core.dll next to it.
        var plugins = TestFiles.NewDirectory();
        File.Copy(typeof(AbstractReader).Assembly.Location, Path.Combine(plugins, "DrawThatThing.Core.dll"));

        var catalog = BitmapReaderCatalog.CreateDefault(plugins);

        Assert.Equal(["AbstractReader", "DetailedReader", "LinearReader", "PointReader"], catalog.Names.Order());
    }

    [Fact]
    public void AParserThatRejectsTheImageReportsItsOwnMessage()
    {
        var catalog = new BitmapReaderCatalog();
        catalog.Add(typeof(PickyReader));

        var exception = Assert.Throws<ArgumentException>(() => catalog.Create(nameof(PickyReader), "/tmp/cat.png"));

        Assert.Equal("PickyReader cannot read cat.png.", exception.Message);
    }
}
