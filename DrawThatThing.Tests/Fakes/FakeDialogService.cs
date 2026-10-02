using DrawThatThing.Avalonia.Services;

namespace DrawThatThing.Tests.Fakes;

public class FakeDialogService : IDialogService
{
    public string? ImagePath { get; set; }
    public string? ImportPath { get; set; }
    public string? ExportPath { get; set; }
    public List<string> Messages { get; } = [];

    /// <summary>When set, message boxes stay open until <see cref="CloseMessages"/> is called.</summary>
    public bool KeepMessagesOpen { get; set; }

    private TaskCompletionSource _openMessages = new();

    public Task<string?> PickImageAsync() => Task.FromResult(ImagePath);
    public Task<string?> PickPaletteToImportAsync() => Task.FromResult(ImportPath);
    public Task<string?> PickPaletteExportPathAsync() => Task.FromResult(ExportPath);

    public Task ShowMessageAsync(string message)
    {
        Messages.Add(message);
        return KeepMessagesOpen ? _openMessages.Task : Task.CompletedTask;
    }

    public void CloseMessages()
    {
        _openMessages.TrySetResult();
        _openMessages = new TaskCompletionSource();
    }
}
