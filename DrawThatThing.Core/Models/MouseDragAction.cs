using DrawThatThing.Platform;

namespace DrawThatThing.Core.Models;

public class MouseDragAction
{
    public List<Point> Points { get; }

    /// <summary>
    /// True for actions that click on a screen position as-is (e.g. choosing a color on the palette),
    /// instead of relative to the mouse start position.
    /// </summary>
    public bool DiscardOffset { get; }

    public Color Color { get; }

    public MouseDragAction(List<Point> points, bool discardOffset = false, Color? color = null)
    {
        Points = points;
        DiscardOffset = discardOffset;
        Color = color ?? Color.Empty;
    }

    public void PushPoint(Point point)
    {
        Points.Add(point);
    }

    public void AddPoint(Point point)
    {
        Points.Insert(0, point);
    }

    /// <summary>
    /// Presses the left button on the first point, drags through all points and releases.
    /// Blocks the calling thread; yields after every point so the caller can stop early.
    /// Cancelling also cuts the waits short, and the button is always released, even when cancelled half way.
    /// Empty points are skipped, so an action without any position does nothing.
    /// </summary>
    public IEnumerable<bool> Play(Point offset, IMouseOperations mouse, CancellationToken cancellationToken = default)
    {
        var points = Points.Where(point => !point.IsEmpty).ToList();
        if (points.Count == 0)
        {
            yield return false;
            yield break;
        }
        if (DiscardOffset)
        {
            offset = new Point(0, 0);
        }
        int waitTime = DiscardOffset ? 100 : 10;
        int loopWaitTime = DiscardOffset ? 100 : 1;

        // Give the drawing application a moment before choosing a color on its palette.
        if (DiscardOffset && Wait(1100, cancellationToken))
        {
            yield break;
        }
        if (cancellationToken.IsCancellationRequested)
        {
            yield break;
        }

        mouse.SetCursorPosition(points[0].X + offset.X, points[0].Y + offset.Y);
        mouse.LeftMouseDown();
        try
        {
            if (Wait(waitTime, cancellationToken))
            {
                yield break;
            }
            foreach (var point in points)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                mouse.SetCursorPosition(point.X + offset.X, point.Y + offset.Y);
                yield return true;
                if (Wait(loopWaitTime, cancellationToken))
                {
                    yield break;
                }
            }
            Wait(waitTime, cancellationToken);
        }
        finally
        {
            mouse.LeftMouseUp();
        }
    }

    /// <summary>Sleeps for the given time; returns true if cancelled.</summary>
    private static bool Wait(int milliseconds, CancellationToken cancellationToken)
    {
        return cancellationToken.WaitHandle.WaitOne(milliseconds);
    }
}
