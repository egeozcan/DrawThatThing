namespace DrawThatThing.Platform.macOS;

/// <summary>
/// Synthesizes mouse input with Quartz events. Posting events requires the Accessibility
/// permission (System Settings → Privacy &amp; Security → Accessibility); without it macOS
/// silently drops them.
/// </summary>
public class MacMouseOperations : IMouseOperations
{
    /// <summary>How long a position this class moved the cursor to is trusted over the reported cursor location.</summary>
    private static readonly TimeSpan PostedPositionLifetime = TimeSpan.FromSeconds(1);

    private readonly IQuartzEvents _quartz;
    private readonly TimeProvider _time;
    private readonly object _positionLock = new();
    private volatile bool _leftButtonDown;
    private volatile bool _rightButtonDown;
    private (double X, double Y) _postedPosition;
    private long? _postedTimestamp;

    public MacMouseOperations()
        : this(new QuartzEvents(), TimeProvider.System)
    {
    }

    internal MacMouseOperations(IQuartzEvents quartz, TimeProvider time)
    {
        _quartz = quartz;
        _time = time;
    }

    public void SetCursorPosition(int x, int y)
    {
        // Moving while a button is held has to be reported as a drag, otherwise
        // applications see a plain hover and nothing gets drawn.
        var (type, button) = _leftButtonDown
            ? (QuartzMouseEventType.LeftMouseDragged, QuartzMouseButton.Left)
            : _rightButtonDown
                ? (QuartzMouseEventType.RightMouseDragged, QuartzMouseButton.Right)
                : (QuartzMouseEventType.MouseMoved, QuartzMouseButton.Left);
        _quartz.PostMouseEvent(type, x, y, button, isClick: false);
        lock (_positionLock)
        {
            _postedPosition = (x, y);
            _postedTimestamp = _time.GetTimestamp();
        }
    }

    public (int X, int Y) GetCursorPosition()
    {
        // The location is fractional on Retina screens; the hotspot is inside the pixel it was truncated to.
        var (x, y) = _quartz.GetCursorLocation();
        return ((int)Math.Floor(x), (int)Math.Floor(y));
    }

    public void LeftMouseDown()
    {
        _leftButtonDown = true;
        PostButtonEvent(QuartzMouseEventType.LeftMouseDown, QuartzMouseButton.Left);
    }

    public void LeftMouseUp()
    {
        _leftButtonDown = false;
        PostButtonEvent(QuartzMouseEventType.LeftMouseUp, QuartzMouseButton.Left);
    }

    public void RightMouseDown()
    {
        _rightButtonDown = true;
        PostButtonEvent(QuartzMouseEventType.RightMouseDown, QuartzMouseButton.Right);
    }

    public void RightMouseUp()
    {
        _rightButtonDown = false;
        PostButtonEvent(QuartzMouseEventType.RightMouseUp, QuartzMouseButton.Right);
    }

    public void Click()
    {
        LeftMouseDown();
        Thread.Sleep(10);
        LeftMouseUp();
    }

    public void Click(int x, int y)
    {
        SetCursorPosition(x, y);
        Click();
    }

    private void PostButtonEvent(QuartzMouseEventType type, QuartzMouseButton button)
    {
        var (x, y) = GetButtonEventLocation();
        _quartz.PostMouseEvent(type, x, y, button, isClick: true);
    }

    /// <summary>
    /// Posted events are applied asynchronously, so right after a move the reported cursor location can
    /// still be the old one. Pressing there would draw a line from the old position, so a button event
    /// shortly after a move goes to where the cursor was moved to.
    /// </summary>
    private (double X, double Y) GetButtonEventLocation()
    {
        lock (_positionLock)
        {
            if (_postedTimestamp is { } postedAt && _time.GetElapsedTime(postedAt) < PostedPositionLifetime)
            {
                return _postedPosition;
            }
        }
        return _quartz.GetCursorLocation();
    }
}
