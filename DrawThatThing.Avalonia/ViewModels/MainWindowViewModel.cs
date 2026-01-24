using System.Collections.ObjectModel;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawThatThing.Core.Models;
using DrawThatThing.Platform;

namespace DrawThatThing.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IPlatformServices? _platformServices;
    private CancellationTokenSource? _playbackCancellation;
    private List<MouseDragAction>? _actions;
    private string? _lastParsedImage;

    public MainWindowViewModel()
    {
        _platformServices = PlatformServicesFactory.Create();
        ColorPalette = new ObservableCollection<ColorPaletteItem>();
        ParserSettings = new ObservableCollection<ParserSettingItem>();
        Parsers = new ObservableCollection<string>();
        BrushSelectors = new ObservableCollection<string>();

        LoadPlugins();
    }

    [ObservableProperty]
    private ObservableCollection<string> _parsers;

    [ObservableProperty]
    private string? _selectedParser;

    [ObservableProperty]
    private ObservableCollection<string> _brushSelectors;

    [ObservableProperty]
    private string? _selectedBrushSelector;

    [ObservableProperty]
    private ObservableCollection<ParserSettingItem> _parserSettings;

    [ObservableProperty]
    private ObservableCollection<ColorPaletteItem> _colorPalette;

    [ObservableProperty]
    private decimal _previewWidth;

    [ObservableProperty]
    private decimal _previewHeight;

    [ObservableProperty]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private string _mousePositionX = "0";

    [ObservableProperty]
    private string _mousePositionY = "0";

    [ObservableProperty]
    private bool _canLoadImage = true;

    [ObservableProperty]
    private bool _showReparse;

    [ObservableProperty]
    private bool _showClearUnusedColors;

    [ObservableProperty]
    private bool _showDebugPanel;

    [ObservableProperty]
    private string _debugRoutes = string.Empty;

    partial void OnSelectedParserChanged(string? value)
    {
        if (value != null)
        {
            UpdateParserSettings();
        }
    }

    private void LoadPlugins()
    {
        // Load plugins from the Plugins directory
        var pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        if (Directory.Exists(pluginsDir))
        {
            var pluginFiles = Directory.GetFiles(pluginsDir, "*.dll");
            foreach (var file in pluginFiles)
            {
                var name = Path.GetFileName(file);
                Parsers.Add(name);
            }
        }

        // Add default parsers if no plugins found
        if (Parsers.Count == 0)
        {
            Parsers.Add("LinearReader.dll");
            Parsers.Add("PointReader.dll");
            Parsers.Add("AbstractReader.dll");
        }

        if (Parsers.Count > 0)
        {
            SelectedParser = Parsers[0];
        }
    }

    private void UpdateParserSettings()
    {
        ParserSettings.Clear();
        // Add default settings based on parser type
        // These would normally come from plugin attributes
        if (SelectedParser?.Contains("Linear") == true)
        {
            ParserSettings.Add(new ParserSettingItem("Tolerance", "10"));
            ParserSettings.Add(new ParserSettingItem("MinR", "0"));
            ParserSettings.Add(new ParserSettingItem("MaxR", "255"));
            ParserSettings.Add(new ParserSettingItem("MinG", "0"));
            ParserSettings.Add(new ParserSettingItem("MaxG", "255"));
            ParserSettings.Add(new ParserSettingItem("MinB", "0"));
            ParserSettings.Add(new ParserSettingItem("MaxB", "255"));
        }
        else if (SelectedParser?.Contains("Point") == true)
        {
            ParserSettings.Add(new ParserSettingItem("Tolerance", "10"));
        }
        else if (SelectedParser?.Contains("Abstract") == true)
        {
            ParserSettings.Add(new ParserSettingItem("Tolerance", "10"));
            ParserSettings.Add(new ParserSettingItem("FloodFill", "true"));
        }

        ShowReparse = !string.IsNullOrEmpty(_lastParsedImage);
    }

    [RelayCommand]
    private async Task LoadImageAsync()
    {
        // In a real implementation, you'd use a file picker dialog
        // For now, this is a placeholder
        CanLoadImage = false;

        try
        {
            // Simulate loading - in real app, use Avalonia's storage provider
            await Task.Delay(100);

            // After loading, update preview
            ShowReparse = true;
            UpdateClearUnusedColorsVisibility();
        }
        finally
        {
            CanLoadImage = true;
        }
    }

    [RelayCommand]
    private void Reparse()
    {
        if (!string.IsNullOrEmpty(_lastParsedImage))
        {
            // Re-parse the image with current settings
        }
    }

    [RelayCommand]
    private void ClearUnusedColors()
    {
        // Remove colors that aren't used in the parsed image
        if (_actions == null) return;

        var usedColors = _actions
            .Select(a => a.Color)
            .Where(c => !c.IsEmpty)
            .Distinct()
            .ToHashSet();

        var itemsToRemove = ColorPalette
            .Where(cp => !usedColors.Any(c => c.DifferenceTo(cp.GetColor()) == 0))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            ColorPalette.Remove(item);
        }

        ShowClearUnusedColors = false;
    }

    private void UpdateClearUnusedColorsVisibility()
    {
        // Check if there are unused colors
        ShowClearUnusedColors = _actions != null && ColorPalette.Count > 0;
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        if (_actions == null || _platformServices == null) return;

        _playbackCancellation = new CancellationTokenSource();
        var offset = new Point(
            int.TryParse(MousePositionX, out var x) ? x : 0,
            int.TryParse(MousePositionY, out var y) ? y : 0
        );

        try
        {
            foreach (var action in _actions)
            {
                if (_playbackCancellation.Token.IsCancellationRequested)
                    break;

                await foreach (var _ in action.PlayAsync(offset, _platformServices.Mouse, _playbackCancellation.Token))
                {
                    // Progress updates could go here
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Playback was cancelled
        }
    }

    [RelayCommand]
    private void StopPlayback()
    {
        _playbackCancellation?.Cancel();
    }

    [RelayCommand]
    private async Task ExportColorsAsync()
    {
        // Export color palette to CSV
        // Would use Avalonia's storage provider for file dialog
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ImportColorsAsync()
    {
        // Import color palette from CSV
        // Would use Avalonia's storage provider for file dialog
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void AddDebugPoint()
    {
        if (_platformServices == null) return;

        var (x, y) = _platformServices.Mouse.GetCursorPosition();
        var newPoint = $"{x}|{y}";

        if (!string.IsNullOrEmpty(DebugRoutes) &&
            !DebugRoutes.EndsWith(",") &&
            !DebugRoutes.EndsWith("\n"))
        {
            DebugRoutes += ", ";
        }

        DebugRoutes += newPoint;
    }

    [RelayCommand]
    private async Task PlayDebugAsync()
    {
        if (_platformServices == null || string.IsNullOrEmpty(DebugRoutes)) return;

        try
        {
            foreach (var line in DebugRoutes.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var started = false;
                var points = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var point in points)
                {
                    var coords = point.Trim().Split('|');
                    if (coords.Length != 2) continue;

                    if (int.TryParse(coords[0], out var px) && int.TryParse(coords[1], out var py))
                    {
                        _platformServices.Mouse.SetCursorPosition(px, py);

                        if (!started)
                        {
                            started = true;
                            _platformServices.Mouse.LeftMouseDown();
                        }

                        await Task.Delay(10);
                    }
                }

                _platformServices.Mouse.LeftMouseUp();
            }
        }
        catch
        {
            // Handle parsing errors
        }
    }

    public void HandleHotkey(int id)
    {
        switch (id)
        {
            case 0: // Shift + Alt + C - Stop
                StopPlayback();
                break;
            case 1: // Shift + Alt + S - Set position
                if (_platformServices != null)
                {
                    var (x, y) = _platformServices.Mouse.GetCursorPosition();
                    MousePositionX = x.ToString();
                    MousePositionY = y.ToString();
                }
                break;
            case 2: // Shift + Alt + A - Add color
                AddColorFromCursor();
                break;
            case 3: // Shift + Alt + D - Toggle debug
                ShowDebugPanel = !ShowDebugPanel;
                break;
            case 4: // Shift + Alt + Q - Add debug point
                AddDebugPoint();
                break;
        }
    }

    private void AddColorFromCursor()
    {
        if (_platformServices == null) return;

        var (x, y) = _platformServices.Mouse.GetCursorPosition();
        var (r, g, b) = _platformServices.ScreenCapture.GetPixelColor(x, y);

        ColorPalette.Add(new ColorPaletteItem
        {
            X = x,
            Y = y,
            Hex = $"#{r:X2}{g:X2}{b:X2}",
            IsBackground = false
        });

        ShowReparse = !string.IsNullOrEmpty(_lastParsedImage);
    }
}

public partial class ColorPaletteItem : ObservableObject
{
    [ObservableProperty]
    private int _x;

    [ObservableProperty]
    private int _y;

    [ObservableProperty]
    private string _hex = "#000000";

    [ObservableProperty]
    private bool _isBackground;

    public Core.Models.Color GetColor() => Core.Models.Color.FromHex(Hex);
}

public partial class ParserSettingItem : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _value = string.Empty;

    public ParserSettingItem()
    {
    }

    public ParserSettingItem(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
