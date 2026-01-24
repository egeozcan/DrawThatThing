namespace DrawThatThing.Platform;

/// <summary>
/// Platform-agnostic interface for mouse operations
/// </summary>
public interface IMouseOperations
{
    /// <summary>
    /// Sets the cursor position on screen
    /// </summary>
    void SetCursorPosition(int x, int y);

    /// <summary>
    /// Gets the current cursor position
    /// </summary>
    (int X, int Y) GetCursorPosition();

    /// <summary>
    /// Simulates a left mouse button down event
    /// </summary>
    void LeftMouseDown();

    /// <summary>
    /// Simulates a left mouse button up event
    /// </summary>
    void LeftMouseUp();

    /// <summary>
    /// Simulates a right mouse button down event
    /// </summary>
    void RightMouseDown();

    /// <summary>
    /// Simulates a right mouse button up event
    /// </summary>
    void RightMouseUp();

    /// <summary>
    /// Performs a click at the current position
    /// </summary>
    void Click();

    /// <summary>
    /// Performs a click at the specified position
    /// </summary>
    void Click(int x, int y);
}
