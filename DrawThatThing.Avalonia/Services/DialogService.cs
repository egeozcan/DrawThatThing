using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace DrawThatThing.Avalonia.Services;

public sealed class DialogService : IDialogService
{
    private static readonly FilePickerFileType CsvFiles = new("CSV Files") { Patterns = ["*.csv"] };

    private readonly Window _owner;
    private readonly Func<IDisposable>? _beginNativeDialog;
    private readonly IStorageProvider? _storageProvider;

    /// <param name="owner">The window the dialogs belong to.</param>
    /// <param name="beginNativeDialog">
    /// Called before a native file dialog opens; the result is disposed once it closes.
    /// </param>
    /// <param name="storageProvider">Shows the file dialogs; the owner window's by default.</param>
    public DialogService(Window owner, Func<IDisposable>? beginNativeDialog = null, IStorageProvider? storageProvider = null)
    {
        _owner = owner;
        _beginNativeDialog = beginNativeDialog;
        _storageProvider = storageProvider;
    }

    private IStorageProvider StorageProvider => _storageProvider ?? _owner.StorageProvider;

    public async Task<string?> PickImageAsync()
    {
        using var nativeDialog = _beginNativeDialog?.Invoke();
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Parse Image",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Images") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.webp"] },
                new FilePickerFileType("JPG File") { Patterns = ["*.jpg", "*.jpeg"] },
                new FilePickerFileType("PNG Files") { Patterns = ["*.png"] }
            ]
        });
        return ToLocalPath(files.FirstOrDefault());
    }

    public async Task<string?> PickPaletteToImportAsync()
    {
        using var nativeDialog = _beginNativeDialog?.Invoke();
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Import Color Palette",
            AllowMultiple = false,
            FileTypeFilter = [CsvFiles]
        });
        return ToLocalPath(files.FirstOrDefault());
    }

    public async Task<string?> PickPaletteExportPathAsync()
    {
        using var nativeDialog = _beginNativeDialog?.Invoke();
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Color Palette",
            DefaultExtension = "csv",
            SuggestedFileName = "palette.csv",
            FileTypeChoices = [CsvFiles],
            ShowOverwritePrompt = true
        });
        return ToLocalPath(file);
    }

    public async Task ShowMessageAsync(string message)
    {
        var okButton = new Button
        {
            Content = "OK",
            MinWidth = 75,
            HorizontalAlignment = HorizontalAlignment.Right,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            IsDefault = true,
            IsCancel = true
        };
        var dialog = new Window
        {
            Title = _owner.Title,
            SizeToContent = SizeToContent.WidthAndHeight,
            CanResize = false,
            ShowInTaskbar = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Background = Brushes.White,
            MinWidth = 220,
            MaxWidth = 480,
            Content = new StackPanel
            {
                Margin = new global::Avalonia.Thickness(16),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    okButton
                }
            }
        };
        okButton.Click += (_, _) => dialog.Close();
        await dialog.ShowDialog(_owner);
    }

    private static string? ToLocalPath(IStorageItem? item)
    {
        if (item == null)
        {
            return null;
        }

        var localPath = item.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(localPath))
        {
            return localPath;
        }

        return item.Path.IsFile ? item.Path.LocalPath : null;
    }
}
