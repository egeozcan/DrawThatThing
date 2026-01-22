using DrawThatThing.Platform;

namespace DrawThatThing.Core.Models;

public class MouseDragAction
{
    public List<Point> Points { get; }
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

    public async IAsyncEnumerable<bool> PlayAsync(Point offset, IMouseOperations mouse, CancellationToken cancellationToken = default)
    {
        if (DiscardOffset)
        {
            offset = new Point(0, 0);
            await Task.Delay(100, cancellationToken);
        }

        if (Points.Count == 0)
        {
            yield return false;
            yield break;
        }

        if (DiscardOffset)
        {
            await Task.Delay(1000, cancellationToken);
        }

        int waitTime = DiscardOffset ? 100 : 10;
        int loopWaitTime = DiscardOffset ? 100 : 1;

        mouse.SetCursorPosition(Points[0].X + offset.X, Points[0].Y + offset.Y);
        mouse.LeftMouseDown();
        await Task.Delay(waitTime, cancellationToken);

        foreach (var point in Points.Where(p => !p.IsEmpty))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                mouse.LeftMouseUp();
                yield break;
            }

            mouse.SetCursorPosition(point.X + offset.X, point.Y + offset.Y);
            yield return true;
            await Task.Delay(loopWaitTime, cancellationToken);
        }

        await Task.Delay(waitTime, cancellationToken);
        mouse.LeftMouseUp();
    }
}
