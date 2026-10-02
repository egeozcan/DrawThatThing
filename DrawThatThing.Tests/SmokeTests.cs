using DrawThatThing.Core.Readers;

namespace DrawThatThing.Tests;

public class SmokeTests
{
    [Fact]
    public void BuiltInReadersAreDiscovered()
    {
        Assert.Equal(["AbstractReader", "DetailedReader", "LinearReader", "PointReader"], BitmapReaderCatalog.CreateDefault().Names.OrderBy(n => n));
    }
}
