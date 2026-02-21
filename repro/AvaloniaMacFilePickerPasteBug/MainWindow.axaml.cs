using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace AvaloniaMacFilePickerPasteBug;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenFilePicker_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select image",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Images")
                {
                    Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.webp"]
                }
            ]
        });

        var selected = files.FirstOrDefault();
        if (selected == null)
        {
            SelectedPathText.Text = "Selected path: (none)";
            return;
        }

        var localPath = selected.TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(localPath) && selected.Path.IsFile)
        {
            localPath = selected.Path.LocalPath;
        }

        SelectedPathText.Text = $"Selected path: {localPath ?? "(unavailable)"}";
    }
}
