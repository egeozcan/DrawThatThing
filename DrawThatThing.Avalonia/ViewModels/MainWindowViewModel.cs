using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawThatThing.Core.Models;
using DrawThatThing.Platform;
using SkiaSharp;

namespace DrawThatThing.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IPlatformServices? _platformServices;
    private readonly Func<Task<string?>> _pickImagePathAsync;
    private CancellationTokenSource? _playbackCancellation;
    private List<MouseDragAction> _actions = [];
    private string? _lastParsedImage;

    public MainWindowViewModel(Func<Task<string?>>? pickImagePathAsync = null)
    {
        _platformServices = PlatformServicesFactory.Create();
        _pickImagePathAsync = pickImagePathAsync ?? (() => Task.FromResult<string?>(null));
        ColorPalette = new ObservableCollection<ColorPaletteItem>();
        ParserSettings = new ObservableCollection<ParserSettingItem>();
        Parsers = new ObservableCollection<string>();
        BrushSelectors = new ObservableCollection<string>();

        LoadPlugins();
        UpdateParserSettings();
        InitializeShortcutLabels();
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

    [ObservableProperty]
    private string _stopMouseShortcutText = string.Empty;

    [ObservableProperty]
    private string _setStartPositionShortcutText = string.Empty;

    [ObservableProperty]
    private string _pickColorShortcutText = string.Empty;

    [ObservableProperty]
    private string _pickColorButtonText = "Pick Color";

    [ObservableProperty]
    private bool _canPickColor = true;

    [ObservableProperty]
    private bool _suspendHotkeys;

    private void InitializeShortcutLabels()
    {
        var altLabel = _platformServices?.Platform == PlatformType.macOS ? "Option" : "Alt";
        StopMouseShortcutText = $"Shift + {altLabel} + C";
        SetStartPositionShortcutText = $"Shift + {altLabel} + S";
        PickColorShortcutText = $"Shift + {altLabel} + A";
    }

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

        if (BrushSelectors.Count == 0)
        {
            BrushSelectors.Add("Default");
            SelectedBrushSelector = BrushSelectors[0];
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
        else
        {
            ParserSettings.Add(new ParserSettingItem("Tolerance", "10"));
            ParserSettings.Add(new ParserSettingItem("MinR", "0"));
            ParserSettings.Add(new ParserSettingItem("MaxR", "255"));
            ParserSettings.Add(new ParserSettingItem("MinG", "0"));
            ParserSettings.Add(new ParserSettingItem("MaxG", "255"));
            ParserSettings.Add(new ParserSettingItem("MinB", "0"));
            ParserSettings.Add(new ParserSettingItem("MaxB", "255"));
        }

        ShowReparse = !string.IsNullOrEmpty(_lastParsedImage);
    }

    [RelayCommand]
    private async Task LoadImageAsync()
    {
        SuspendHotkeys = true;
        string? imagePath;
        try
        {
            imagePath = await _pickImagePathAsync();
        }
        finally
        {
            SuspendHotkeys = false;
        }

        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return;
        }

        await ParseImageWithCurrentParserAsync(imagePath);
    }

    public async Task<bool> TryLoadImageFromPathAsync(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return false;
        }

        await ParseImageWithCurrentParserAsync(imagePath);
        return true;
    }

    [RelayCommand]
    private async Task ReparseAsync()
    {
        if (!string.IsNullOrEmpty(_lastParsedImage))
        {
            await ParseImageWithCurrentParserAsync(_lastParsedImage);
        }
    }

    [RelayCommand]
    private void ClearUnusedColors()
    {
        // Remove colors that aren't used in the parsed image
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
        ShowClearUnusedColors = _actions.Count > 0 && ColorPalette.Count > 0;
    }

    private async Task ParseImageWithCurrentParserAsync(string imagePath)
    {
        CanLoadImage = false;
        try
        {
            var settings = ParserSettings.ToDictionary(s => s.Name, s => s.Value, StringComparer.OrdinalIgnoreCase);
            var palette = BuildPalette();
            var parserName = SelectedParser ?? string.Empty;

            var parseResult = await Task.Run(() => ParseImage(imagePath, parserName, settings, palette));
            if (!parseResult.HasValue)
            {
                return;
            }
            var result = parseResult.Value;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _actions = result.Actions;
                using var stream = new MemoryStream(result.PreviewPng);
                var parsedPreview = new Bitmap(stream);
                PreviewImage?.Dispose();
                PreviewImage = parsedPreview;
                PreviewWidth = parsedPreview.PixelSize.Width;
                PreviewHeight = parsedPreview.PixelSize.Height;
                _lastParsedImage = imagePath;
                ShowReparse = true;
                UpdateClearUnusedColorsVisibility();
            });
        }
        catch
        {
            // Keep app responsive for malformed images/settings.
        }
        finally
        {
            CanLoadImage = true;
        }
    }

    private List<PaletteEntry> BuildPalette()
    {
        return ColorPalette
            .Select((item, index) => PaletteEntry.TryCreate(item, index))
            .Where(entry => entry != null)
            .Select(entry => entry!.Value)
            .ToList();
    }

    private static ParseResult? ParseImage(
        string imagePath,
        string parserName,
        IReadOnlyDictionary<string, string> settings,
        IReadOnlyList<PaletteEntry> palette)
    {
        using var source = SKBitmap.Decode(imagePath);
        if (source == null)
        {
            return null;
        }

        using var preview = new SKBitmap(source.Width, source.Height);
        int width = source.Width;
        int height = source.Height;

        var indices = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pixel = source.GetPixel(x, y);
                if (palette.Count == 0)
                {
                    preview.SetPixel(x, y, pixel);
                    indices[x, y] = -1;
                    continue;
                }

                int nearestIndex = FindNearestPaletteIndex(pixel, palette);
                indices[x, y] = nearestIndex;
                preview.SetPixel(x, y, palette[nearestIndex].SkColor);
            }
        }

        var actions = BuildActions(parserName, indices, width, height, palette, settings);
        using var image = SKImage.FromBitmap(preview);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return new ParseResult(actions, encoded.ToArray());
    }

    private static List<MouseDragAction> BuildActions(
        string parserName,
        int[,] indices,
        int width,
        int height,
        IReadOnlyList<PaletteEntry> palette,
        IReadOnlyDictionary<string, string> settings)
    {
        if (palette.Count == 0)
        {
            return [];
        }

        if (parserName.Contains("Point", StringComparison.OrdinalIgnoreCase))
        {
            return BuildPointActions(indices, width, height, palette, settings);
        }

        if (parserName.Contains("Abstract", StringComparison.OrdinalIgnoreCase))
        {
            return BuildAbstractActions(indices, width, height, palette, settings);
        }

        return BuildLinearActions(indices, width, height, palette);
    }

    private static List<MouseDragAction> BuildLinearActions(
        int[,] indices,
        int width,
        int height,
        IReadOnlyList<PaletteEntry> palette)
    {
        var actions = new List<MouseDragAction>();
        int? lastColorIndex = null;
        var backgroundIndices = palette.Where(p => p.IsBackground).Select(p => p.Index).ToHashSet();

        for (int y = 0; y < height; y++)
        {
            int x = 0;
            while (x < width)
            {
                int index = indices[x, y];
                if (index < 0 || backgroundIndices.Contains(index))
                {
                    x++;
                    continue;
                }

                EnsureColorAction(actions, ref lastColorIndex, palette[index]);
                var run = new List<Point>();
                while (x < width && indices[x, y] == index)
                {
                    run.Add(new Point(x, y));
                    x++;
                }

                if (run.Count > 0)
                {
                    actions.Add(new MouseDragAction(run));
                }
            }
        }

        return actions;
    }

    private static List<MouseDragAction> BuildPointActions(
        int[,] indices,
        int width,
        int height,
        IReadOnlyList<PaletteEntry> palette,
        IReadOnlyDictionary<string, string> settings)
    {
        var actions = new List<MouseDragAction>();
        var pointsByColor = new Dictionary<int, List<Point>>();
        var backgroundIndices = palette.Where(p => p.IsBackground).Select(p => p.Index).ToHashSet();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = indices[x, y];
                if (index < 0 || backgroundIndices.Contains(index))
                {
                    continue;
                }

                if (!pointsByColor.TryGetValue(index, out var points))
                {
                    points = [];
                    pointsByColor[index] = points;
                }
                points.Add(new Point(x, y));
            }
        }

        bool mixPoints = GetBoolSetting(settings, "MixPoints", false);
        var random = new Random();
        int? lastColorIndex = null;

        foreach (var group in pointsByColor.OrderBy(g => g.Key))
        {
            EnsureColorAction(actions, ref lastColorIndex, palette[group.Key]);
            var points = group.Value;
            if (mixPoints)
            {
                points = points.OrderBy(_ => random.Next()).ToList();
            }

            foreach (var point in points)
            {
                actions.Add(new MouseDragAction([point]));
            }
        }

        return actions;
    }

    private static List<MouseDragAction> BuildAbstractActions(
        int[,] indices,
        int width,
        int height,
        IReadOnlyList<PaletteEntry> palette,
        IReadOnlyDictionary<string, string> settings)
    {
        int minimumStroke = GetIntSetting(settings, "MinimumStrokeSize", 15);
        var actions = new List<MouseDragAction>();
        var backgroundIndices = palette.Where(p => p.IsBackground).Select(p => p.Index).ToHashSet();
        var visited = new bool[width, height];
        var groups = new List<(int colorIndex, List<Point> points)>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (visited[x, y])
                {
                    continue;
                }

                int index = indices[x, y];
                if (index < 0 || backgroundIndices.Contains(index))
                {
                    visited[x, y] = true;
                    continue;
                }

                var queue = new Queue<Point>();
                var group = new List<Point>();
                queue.Enqueue(new Point(x, y));
                visited[x, y] = true;

                while (queue.Count > 0)
                {
                    var p = queue.Dequeue();
                    group.Add(p);

                    foreach (var (nx, ny) in EnumerateNeighbors(p.X, p.Y, width, height))
                    {
                        if (visited[nx, ny] || indices[nx, ny] != index)
                        {
                            continue;
                        }

                        visited[nx, ny] = true;
                        queue.Enqueue(new Point(nx, ny));
                    }
                }

                if (group.Count >= minimumStroke)
                {
                    groups.Add((index, group));
                }
            }
        }

        int? lastColorIndex = null;
        foreach (var group in groups.OrderBy(g => g.colorIndex))
        {
            EnsureColorAction(actions, ref lastColorIndex, palette[group.colorIndex]);
            actions.Add(new MouseDragAction(group.points));
        }

        return actions;
    }

    private static IEnumerable<(int x, int y)> EnumerateNeighbors(int x, int y, int width, int height)
    {
        if (x > 0) yield return (x - 1, y);
        if (x < width - 1) yield return (x + 1, y);
        if (y > 0) yield return (x, y - 1);
        if (y < height - 1) yield return (x, y + 1);
    }

    private static void EnsureColorAction(List<MouseDragAction> actions, ref int? lastColorIndex, PaletteEntry paletteEntry)
    {
        if (lastColorIndex == paletteEntry.Index)
        {
            return;
        }

        lastColorIndex = paletteEntry.Index;
        actions.Add(new MouseDragAction(
            [new Point(paletteEntry.PaletteX, paletteEntry.PaletteY)],
            true,
            paletteEntry.Color));
    }

    private static int FindNearestPaletteIndex(SKColor pixel, IReadOnlyList<PaletteEntry> palette)
    {
        int bestIndex = 0;
        int bestDifference = int.MaxValue;
        for (int i = 0; i < palette.Count; i++)
        {
            var c = palette[i].Color;
            int difference = Math.Abs(pixel.Red - c.R) + Math.Abs(pixel.Green - c.G) + Math.Abs(pixel.Blue - c.B);
            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private static int GetIntSetting(IReadOnlyDictionary<string, string> settings, string key, int fallback)
    {
        return settings.TryGetValue(key, out var value) && int.TryParse(value, out var parsed) ? parsed : fallback;
    }

    private static bool GetBoolSetting(IReadOnlyDictionary<string, string> settings, string key, bool fallback)
    {
        return settings.TryGetValue(key, out var value) && bool.TryParse(value, out var parsed) ? parsed : fallback;
    }

    private readonly record struct ParseResult(List<MouseDragAction> Actions, byte[] PreviewPng);

    private readonly record struct PaletteEntry(
        int Index,
        int PaletteX,
        int PaletteY,
        global::DrawThatThing.Core.Models.Color Color,
        bool IsBackground,
        SKColor SkColor)
    {
        public static PaletteEntry? TryCreate(ColorPaletteItem item, int index)
        {
            try
            {
                var color = global::DrawThatThing.Core.Models.Color.FromHex(item.Hex);
                if (color.IsEmpty)
                {
                    return null;
                }

                return new PaletteEntry(
                    index,
                    item.X,
                    item.Y,
                    color,
                    item.IsBackground,
                    new SKColor(color.R, color.G, color.B, color.A));
            }
            catch
            {
                return null;
            }
        }
    }

    [RelayCommand]
    private async Task PlayAsync()
    {
        if (_actions.Count == 0 || _platformServices == null) return;

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
    private void CaptureStartPosition()
    {
        if (_platformServices == null)
        {
            return;
        }

        var (x, y) = _platformServices.Mouse.GetCursorPosition();
        MousePositionX = x.ToString();
        MousePositionY = y.ToString();
    }

    [RelayCommand]
    private void ToggleDebugPanel()
    {
        ShowDebugPanel = !ShowDebugPanel;
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
    private async Task PickColorAsync()
    {
        if (!CanPickColor)
        {
            return;
        }

        CanPickColor = false;
        PickColorButtonText = "Move cursor...";

        try
        {
            // Delay to let user move off the button and place cursor on target color.
            await Task.Delay(1200);
            await Dispatcher.UIThread.InvokeAsync(AddColorFromCursor);
        }
        finally
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                PickColorButtonText = "Pick Color";
                CanPickColor = true;
            });
        }
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
                CaptureStartPosition();
                break;
            case 2: // Shift + Alt + A - Add color
                AddColorFromCursor();
                break;
            case 3: // Shift + Alt + D - Toggle debug
                ToggleDebugPanel();
                break;
            case 4: // Shift + Alt + Q - Add debug point
                AddDebugPoint();
                break;
        }
    }

    private void AddColorFromCursor()
    {
        int x = 0;
        int y = 0;
        string hex = "#000000";

        try
        {
            if (_platformServices != null)
            {
                (x, y) = _platformServices.Mouse.GetCursorPosition();
                try
                {
                    var (r, g, b) = _platformServices.ScreenCapture.GetPixelColor(x, y);
                    hex = $"#{r:X2}{g:X2}{b:X2}";
                }
                catch
                {
                    // Screen sampling can fail without permissions; keep fallback hex.
                }
            }
        }
        catch
        {
            // Fall through to safe defaults.
        }

        if (x == 0 && y == 0 && PreviewImage != null && PreviewImage.PixelSize.Width > 0 && PreviewImage.PixelSize.Height > 0)
        {
            x = PreviewImage.PixelSize.Width / 2;
            y = PreviewImage.PixelSize.Height / 2;
        }

        ColorPalette.Add(new ColorPaletteItem
        {
            X = x,
            Y = y,
            Hex = hex,
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

    [ObservableProperty]
    private IBrush _previewBrush = new SolidColorBrush(Colors.Black);

    partial void OnHexChanged(string value)
    {
        PreviewBrush = CreatePreviewBrush(value);
    }

    private static IBrush CreatePreviewBrush(string hex)
    {
        try
        {
            var color = global::Avalonia.Media.Color.Parse(string.IsNullOrWhiteSpace(hex) ? "#000000" : hex);
            return new SolidColorBrush(color);
        }
        catch
        {
            return new SolidColorBrush(Colors.Black);
        }
    }

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
