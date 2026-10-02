using DrawThatThing.Platform;

namespace DrawThatThing.Tests.Fakes;

/// <summary>Records mouse operations instead of moving the real cursor.</summary>
public class FakeMouse : IMouseOperations
{
    private readonly object _lock = new();
    private readonly List<string> _log = [];

    public (int X, int Y) Position { get; set; }

    /// <summary>Invoked after every recorded operation, e.g. to cancel playback at a certain point.</summary>
    public Action<string>? OnOperation { get; set; }

    public Exception? ThrowOnMove { get; set; }

    public List<string> Log
    {
        get
        {
            lock (_lock)
            {
                return [.. _log];
            }
        }
    }

    public void SetCursorPosition(int x, int y)
    {
        if (ThrowOnMove != null)
        {
            throw ThrowOnMove;
        }
        Position = (x, y);
        Record($"move {x},{y}");
    }

    public (int X, int Y) GetCursorPosition() => Position;

    public void LeftMouseDown() => Record("down");

    public void LeftMouseUp() => Record("up");

    public void RightMouseDown() => Record("rdown");

    public void RightMouseUp() => Record("rup");

    public void Click()
    {
        LeftMouseDown();
        LeftMouseUp();
    }

    public void Click(int x, int y)
    {
        SetCursorPosition(x, y);
        Click();
    }

    private void Record(string operation)
    {
        lock (_lock)
        {
            _log.Add(operation);
        }
        OnOperation?.Invoke(operation);
    }
}
