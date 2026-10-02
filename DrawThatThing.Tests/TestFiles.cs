using System.Runtime.CompilerServices;

namespace DrawThatThing.Tests;

/// <summary>Temporary files for tests, in one folder per test run that is deleted when the run ends.</summary>
public static class TestFiles
{
    private static readonly string Root = Path.Combine(Path.GetTempPath(), $"DrawThatThing.Tests-{Guid.NewGuid():N}");

    [ModuleInitializer]
    internal static void DeleteWhenTheRunEnds()
    {
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            try
            {
                Directory.Delete(Root, recursive: true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // Not worth failing the run over; the system cleans its temp folder eventually.
            }
        };
    }

    /// <summary>A path for a new file with the given extension (e.g. ".png").</summary>
    public static string NewPath(string extension)
    {
        Directory.CreateDirectory(Root);
        return Path.Combine(Root, Guid.NewGuid().ToString("N") + extension);
    }

    public static string NewDirectory()
    {
        var path = Path.Combine(Root, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
