using DrawThatThing.Avalonia.Services;

namespace DrawThatThing.Tests.Fakes;

public class FakeDialogService : IDialogService
{
    public string? ImagePath { get; set; }
    public string? ImportPath { get; set; }
    public string? ExportPath { get; set; }
    public List<string> Messages { get; } = [];

    public Task<string?> PickImageAsync() => Task.FromResult(ImagePath);
    public Task<string?> PickPaletteToImportAsync() => Task.FromResult(ImportPath);
    public Task<string?> PickPaletteExportPathAsync() => Task.FromResult(ExportPath);

    public Task ShowMessageAsync(string message)
    {
        Messages.Add(message);
        return Task.CompletedTask;
    }
}
