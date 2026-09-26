namespace DrawThatThing.Avalonia.Services;

public interface IDialogService
{
    Task<string?> PickImageAsync();
    Task<string?> PickPaletteToImportAsync();
    Task<string?> PickPaletteExportPathAsync();
    Task ShowMessageAsync(string message);
}
