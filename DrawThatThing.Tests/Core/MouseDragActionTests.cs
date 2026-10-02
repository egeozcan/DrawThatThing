using System.Diagnostics;
using DrawThatThing.Core.Models;
using DrawThatThing.Tests.Fakes;

namespace DrawThatThing.Tests.Core;

public class MouseDragActionTests
{
    private static void PlayToEnd(MouseDragAction action, FakeMouse mouse, Point? offset = null, CancellationToken token = default)
    {
        foreach (var _ in action.Play(offset ?? new Point(0, 0), mouse, token))
        {
        }
    }

    [Fact]
    public void DrawsTheStrokeRelativeToTheOffset()
    {
        var mouse = new FakeMouse();
        var action = new MouseDragAction([new Point(1, 2), new Point(2, 2)]);

        PlayToEnd(action, mouse, new Point(100, 200));

        Assert.Equal(["move 101,202", "down", "move 101,202", "move 102,202", "up"], mouse.Log);
    }

    [Fact]
    public void APaletteClickWithoutAPositionDoesNotClickTheScreen()
    {
        // DetailedReader produces this when the palette is empty; clicking (0, 0) would open the Apple menu.
        var mouse = new FakeMouse();
        var action = new MouseDragAction([Point.Empty], discardOffset: true, Color.Black);

        PlayToEnd(action, mouse);

        Assert.Empty(mouse.Log);
    }

    [Fact]
    public async Task StoppingBeforeAPaletteClickSkipsTheClick()
    {
        var mouse = new FakeMouse();
        var action = new MouseDragAction([new Point(500, 20)], discardOffset: true, Color.Black);
        using var cancellation = new CancellationTokenSource();
        var stopwatch = Stopwatch.StartNew();

        var playback = Task.Run(() => PlayToEnd(action, mouse, token: cancellation.Token));
        await Task.Delay(100);
        cancellation.Cancel();

        await playback.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Empty(mouse.Log);
        Assert.True(stopwatch.ElapsedMilliseconds < 700, $"Stopping took {stopwatch.ElapsedMilliseconds} ms.");
    }

    [Fact]
    public void AnAlreadyStoppedPlaybackDoesNotTouchTheMouse()
    {
        var mouse = new FakeMouse();
        var action = new MouseDragAction([new Point(11, 12), new Point(12, 12)]);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        PlayToEnd(action, mouse, token: cancellation.Token);

        Assert.Empty(mouse.Log);
    }

    [Fact]
    public void StoppingMidStrokeReleasesTheButtonWithoutMovingFurther()
    {
        var mouse = new FakeMouse();
        var points = Enumerable.Range(0, 100).Select(i => new Point(i, 0)).ToList();
        var action = new MouseDragAction(points);
        using var cancellation = new CancellationTokenSource();
        mouse.OnOperation = operation =>
        {
            if (operation == "move 10,0")
            {
                cancellation.Cancel();
            }
        };

        PlayToEnd(action, mouse, token: cancellation.Token);

        Assert.Equal("up", mouse.Log[^1]);
        Assert.Equal("move 10,0", mouse.Log[^2]);
    }
}
