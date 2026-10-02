namespace DrawThatThing.Platform;

/// <summary>
/// Platform-agnostic interface for screen capture operations
/// </summary>
public interface IScreenCapture
{
    /// <summary>
    /// Gets the color of the pixel at the specified screen coordinates
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <returns>
    /// The color at the specified position, or null if the screen cannot be read
    /// (for example because the user has not allowed it)
    /// </returns>
    (byte R, byte G, byte B)? GetPixelColor(int x, int y);

    /// <summary>
    /// Captures a region of the screen
    /// </summary>
    /// <param name="x">X coordinate of the top-left corner</param>
    /// <param name="y">Y coordinate of the top-left corner</param>
    /// <param name="width">Width of the region</param>
    /// <param name="height">Height of the region</param>
    /// <returns>Byte array containing the captured image data in RGBA format</returns>
    byte[] CaptureRegion(int x, int y, int width, int height);
}
