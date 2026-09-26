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
    /// The button is always released, even when cancelled half way.
    /// </summary>
    public IEnumerable<bool> Play(Point offset, IMouseOperations mouse, CancellationToken cancellationToken = default)
    {
        if (DiscardOffset)
        {
            offset = new Point(0, 0);
            Thread.Sleep(100);
        }
        if (Points.Count == 0)
        {
            yield return false;
            yield break;
        }
        if (DiscardOffset)
        {
            Thread.Sleep(1000);
        }
        int waitTime = DiscardOffset ? 100 : 10;
        int loopWaitTime = DiscardOffset ? 100 : 1;
        mouse.SetCursorPosition(Points[0].X + offset.X, Points[0].Y + offset.Y);
        mouse.LeftMouseDown();
        try
        {
            Thread.Sleep(waitTime);
            foreach (var point in Points.Where(point => !point.IsEmpty))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                mouse.SetCursorPosition(point.X + offset.X, point.Y + offset.Y);
                yield return true;
                Thread.Sleep(loopWaitTime);
            }
            Thread.Sleep(waitTime);
        }
        finally
        {
            mouse.LeftMouseUp();
        }
    }
}
