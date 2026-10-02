using DrawThatThing.Platform.macOS;
using DrawThatThing.Tests.Fakes;

namespace DrawThatThing.Tests.Platform;

public class MacMouseOperationsTests
{
    /// <summary>
    /// Mimics the window server: posted events are applied asynchronously, so the cursor location
    /// read back right after posting a move can still be the previous one.
    /// </summary>
    private sealed class LaggingQuartz : IQuartzEvents
    {
        public (double X, double Y) Location { get; set; }
        public List<(QuartzMouseEventType Type, double X, double Y)> Posted { get; } = [];

        public void PostMouseEvent(QuartzMouseEventType type, double x, double y, QuartzMouseButton button, bool isClick)
        {
            Posted.Add((type, x, y));
        }

        public (double X, double Y) GetCursorLocation() => Location;
    }

    [Fact]
    public void LeftMouseDownRightAfterAMovePressesAtTheNewPosition()
    {
        var quartz = new LaggingQuartz { Location = (10, 10) };
        var mouse = new MacMouseOperations(quartz, new ManualTimeProvider());

        mouse.SetCursorPosition(500, 300);
        mouse.LeftMouseDown();

        Assert.Equal((QuartzMouseEventType.LeftMouseDown, 500d, 300d), quartz.Posted[^1]);
    }

    [Fact]
    public void LeftMouseUpAfterADragReleasesAtTheLastDraggedPosition()
    {
        var quartz = new LaggingQuartz { Location = (10, 10) };
        var mouse = new MacMouseOperations(quartz, new ManualTimeProvider());

        mouse.SetCursorPosition(100, 100);
        mouse.LeftMouseDown();
        mouse.SetCursorPosition(120, 130);
        mouse.LeftMouseUp();

        Assert.Equal(
            [
                (QuartzMouseEventType.MouseMoved, 100d, 100d),
                (QuartzMouseEventType.LeftMouseDown, 100d, 100d),
                (QuartzMouseEventType.LeftMouseDragged, 120d, 130d),
                (QuartzMouseEventType.LeftMouseUp, 120d, 130d)
            ],
            quartz.Posted);
    }

    [Fact]
    public void ButtonEventsLongAfterTheLastMoveUseTheRealCursorLocation()
    {
        var quartz = new LaggingQuartz { Location = (10, 10) };
        var time = new ManualTimeProvider();
        var mouse = new MacMouseOperations(quartz, time);

        mouse.SetCursorPosition(500, 300);
        time.Advance(TimeSpan.FromSeconds(5));
        quartz.Location = (42, 24); // the user moved the mouse in the meantime
        mouse.LeftMouseDown();

        Assert.Equal((QuartzMouseEventType.LeftMouseDown, 42d, 24d), quartz.Posted[^1]);
    }

    [Fact]
    public void GetCursorPositionReturnsThePixelUnderTheHotspot()
    {
        var quartz = new LaggingQuartz { Location = (918.61, 86.49) };
        var mouse = new MacMouseOperations(quartz, new ManualTimeProvider());

        Assert.Equal((918, 86), mouse.GetCursorPosition());
    }
}
