using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DrawThatThing.Avalonia.Services;
using DrawThatThing.Core.Extensions;
using DrawThatThing.Core.Imaging;
using DrawThatThing.Core.Models;
using DrawThatThing.Core.Readers;
using DrawThatThing.Platform;
using CoreColor = DrawThatThing.Core.Models.Color;

namespace DrawThatThing.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public const int StopMouseHotkey = 0;
    public const int SetStartPositionHotkey = 1;
    public const int PickColorHotkey = 2;
    public const int ToggleDebugHotkey = 3;
    public const int AddDebugPointHotkey = 4;

    private static readonly (int Id, char Key)[] Hotkeys =
    [
        (StopMouseHotkey, 'C'),
        (SetStartPositionHotkey, 'S'),
        (PickColorHotkey, 'A'),
        (ToggleDebugHotkey, 'D'),
        (AddDebugPointHotkey, 'Q')
    ];

    private readonly IPlatformServices? _platformServices;
    private readonly IDialogService _dialogs;
    private readonly BitmapReaderCatalog _readers;
    private readonly Dictionary<int, (HotkeyModifiers Modifiers, char Key)> _windowOnlyHotkeys = new();
    private List<MouseDragAction>? _actions;
    private string? _lastParsedImage;
    private bool _loadComplete = true;
    private bool _hotkeyEventsSubscribed;
    private CancellationTokenSource? _playCancellation;
    private Task _playTask = Task.CompletedTask;

    public MainWindowViewModel(IPlatformServices? platformServices, IDialogService dialogs)
    {
        _platformServices = platformServices;
        _dialogs = dialogs;
        _readers = BitmapReaderCatalog.CreateDefault(Path.Combine(AppContext.BaseDirectory, "Plugins"));

        ColorPalette.CollectionChanged += (_, e) =>
        {
            foreach (ColorPaletteItem item in e.NewItems ?? Array.Empty<ColorPaletteItem>())
            {
                item.PropertyChanged += OnPaletteItemChanged;
            }
            foreach (ColorPaletteItem item in e.OldItems ?? Array.Empty<ColorPaletteItem>())
            {
                item.PropertyChanged -= OnPaletteItemChanged;
            }
        };
        ColorPalette.Add(ColorPaletteItem.CreateNewRow());

        foreach (var name in _readers.Names)
        {
            Parsers.Add(name);
        }
        SelectedParser = Parsers.FirstOrDefault();

        foreach (var (id, key) in Hotkeys)
        {
            UpdateShortcutLabel(id, FormatShortcut(DefaultHotkeyModifiers, key));
        }
    }

    /// <summary>Raised after a debug point was added, so the view can move the caret to the end.</summary>
    public event EventHandler? DebugPointAdded;

    public ObservableCollection<string> Parsers { get; } = [];

    /// <summary>Brush selector plugins. None exist yet, just like in the original application.</summary>
    public ObservableCollection<string> BrushSelectors { get; } = [];

    public ObservableCollection<ParserSettingItem> ParserSettings { get; } = [];

    /// <summary>The palette rows. The last row is always an empty "new row" that turns into a real row once edited.</summary>
    public ObservableCollection<ColorPaletteItem> ColorPalette { get; } = [];

    [ObservableProperty]
    private string? _selectedParser;

    [ObservableProperty]
    private string? _selectedBrushSelector;

    [ObservableProperty]
    private decimal? _previewWidth = 0;

    [ObservableProperty]
    private decimal? _previewHeight = 0;

    [ObservableProperty]
    private Bitmap? _previewImage;

    [ObservableProperty]
    private string _mousePositionX = "0";

    [ObservableProperty]
    private string _mousePositionY = "0";

    [ObservableProperty]
    private bool _canLoadImage = true;

    [ObservableProperty]
    private string _loadImageButtonText = "Parse Image";

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

    /// <summary>Why the Stop shortcut could not be registered system-wide; null when it was.</summary>
    [ObservableProperty]
    private string? _stopMouseShortcutWarning;

    [ObservableProperty]
    private string? _setStartPositionShortcutWarning;

    [ObservableProperty]
    private string? _pickColorShortcutWarning;

    private HotkeyModifiers DefaultHotkeyModifiers => HotkeyModifiers.Shift | HotkeyModifiers.Alt;

    partial void OnSelectedParserChanged(string? value)
    {
        UpdateOptions();
    }

    #region Hotkeys

    /// <summary>
    /// Registers the system-wide hotkeys (Shift + Alt + C/S/A/D/Q, like the original application).
    /// macOS 15 and later refuse global hotkeys that only use Option (+ Shift), so there
    /// Control + Option is used instead. A hotkey that cannot be registered globally (usually because
    /// another application already uses it) gets a warning on its label and is also handled as a
    /// shortcut of this window; the others are registered regardless, so Stop keeps working while drawing.
    /// </summary>
    public void RegisterHotkeys()
    {
        var hotkeyManager = _platformServices?.Hotkeys;
        var candidates = new List<HotkeyModifiers>();
        if (_platformServices?.Platform == PlatformType.macOS && OperatingSystem.IsMacOSVersionAtLeast(15))
        {
            candidates.Add(HotkeyModifiers.Ctrl | HotkeyModifiers.Alt);
        }
        else
        {
            candidates.Add(DefaultHotkeyModifiers);
            if (_platformServices?.Platform == PlatformType.macOS)
            {
                candidates.Add(HotkeyModifiers.Ctrl | HotkeyModifiers.Alt);
            }
        }

        if (hotkeyManager != null && !_hotkeyEventsSubscribed)
        {
            hotkeyManager.HotkeyPressed += (_, e) =>
                global::Avalonia.Threading.Dispatcher.UIThread.Post(() => HandleHotkey(e.Id));
            _hotkeyEventsSubscribed = true;
        }

        _windowOnlyHotkeys.Clear();
        foreach (var (id, key) in Hotkeys)
        {
            var modifiers = candidates[0];
            var registered = false;
            if (hotkeyManager != null)
            {
                hotkeyManager.UnregisterHotkey(id);
                foreach (var candidate in candidates)
                {
                    if (hotkeyManager.RegisterHotkey(id, candidate, key))
                    {
                        modifiers = candidate;
                        registered = true;
                        break;
                    }
                }
            }

            // Fall back to a shortcut that works while this window has the keyboard focus, for when the system
            // still delivers the key press to it (it usually goes to the application that owns the combination).
            if (!registered)
            {
                _windowOnlyHotkeys[id] = (modifiers, key);
            }
            UpdateShortcutLabel(
                id,
                FormatShortcut(modifiers, key),
                registered ? null : "This shortcut could not be registered, probably because another application already uses it.");
        }
    }

    public void UnregisterHotkeys()
    {
        _platformServices?.Hotkeys.Dispose();
    }

    /// <summary>
    /// Stops a running playback before the application exits, so the mouse button is not left pressed.
    /// </summary>
    public void Shutdown()
    {
        StopPlayback();
        try
        {
            _playTask.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException)
        {
            // Already reported, or not worth reporting while quitting.
        }
        UnregisterHotkeys();
    }

    /// <summary>
    /// Handles a key press inside the window for hotkeys that could not be registered globally.
    /// </summary>
    public bool TryHandleWindowHotkey(HotkeyModifiers modifiers, char key)
    {
        foreach (var (id, hotkey) in _windowOnlyHotkeys)
        {
            if (hotkey.Modifiers == modifiers && char.ToUpperInvariant(hotkey.Key) == char.ToUpperInvariant(key))
            {
                HandleHotkey(id);
                return true;
            }
        }
        return false;
    }

    private void UpdateShortcutLabel(int id, string text, string? warning = null)
    {
        switch (id)
        {
            case StopMouseHotkey:
                StopMouseShortcutText = text;
                StopMouseShortcutWarning = warning;
                break;
            case SetStartPositionHotkey:
                SetStartPositionShortcutText = text;
                SetStartPositionShortcutWarning = warning;
                break;
            case PickColorHotkey:
                PickColorShortcutText = text;
                PickColorShortcutWarning = warning;
                break;
        }
    }

    private string FormatShortcut(HotkeyModifiers modifiers, char key)
    {
        var isMac = _platformServices?.Platform == PlatformType.macOS;
        var parts = new List<string>();
        if (modifiers.HasFlag(HotkeyModifiers.Ctrl)) parts.Add(isMac ? "Control" : "Ctrl");
        if (modifiers.HasFlag(HotkeyModifiers.Shift)) parts.Add("Shift");
        if (modifiers.HasFlag(HotkeyModifiers.Alt)) parts.Add(isMac ? "Option" : "Alt");
        if (modifiers.HasFlag(HotkeyModifiers.Win)) parts.Add(isMac ? "Command" : "Win");
        parts.Add(key.ToString());
        return string.Join(" + ", parts);
    }

    public void HandleHotkey(int id)
    {
        switch (id)
        {
            case StopMouseHotkey:
                StopPlayback();
                break;
            case SetStartPositionHotkey:
                SetStartPositionToCursor();
                break;
            case PickColorHotkey:
                AddCurrentMousePositionToPalette();
                break;
            case ToggleDebugHotkey:
                ShowDebugPanel = !ShowDebugPanel;
                break;
            case AddDebugPointHotkey:
                AddDebugPoint();
                break;
        }
    }

    #endregion

    #region Color palette

    private void SetStartPositionToCursor()
    {
        if (_platformServices == null)
        {
            return;
        }

        var (x, y) = _platformServices.Mouse.GetCursorPosition();
        MousePositionX = x.ToString(CultureInfo.InvariantCulture);
        MousePositionY = y.ToString(CultureInfo.InvariantCulture);
    }

    private void AddCurrentMousePositionToPalette()
    {
        if (_platformServices == null)
        {
            return;
        }

        var (x, y) = _platformServices.Mouse.GetCursorPosition();
        if (_platformServices.ScreenCapture.GetPixelColor(x, y) is not { } color)
        {
            var hint = _platformServices.Platform == PlatformType.macOS
                ? " Allow DrawThatThing under System Settings → Privacy & Security → Screen Recording"
                  + " (Screen & System Audio Recording on macOS 15 and later), then restart it."
                : string.Empty;
            _ = _dialogs.ShowMessageAsync("Could not read the color under the cursor." + hint);
            return;
        }
        var (r, g, b) = color;
        AddPaletteRow(
            x.ToString(CultureInfo.InvariantCulture),
            y.ToString(CultureInfo.InvariantCulture),
            new CoreColor(r, g, b).ToHex(),
            false);
    }

    private void AddPaletteRow(string x, string y, string rgb, bool isBackground, bool isOpener = false)
    {
        var item = new ColorPaletteItem { X = x, Y = y, Rgb = rgb, IsBackground = isBackground, IsOpener = isOpener };
        ColorPalette.Insert(ColorPalette.Count - 1, item);
        ShowReparseButton();
    }

    public void RemovePaletteRows(IEnumerable<ColorPaletteItem> items)
    {
        foreach (var item in items.Where(i => !i.IsNewRow).ToList())
        {
            ColorPalette.Remove(item);
        }
        ShowReparseButton();
    }

    private void OnPaletteItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ColorPaletteItem item || e.PropertyName == nameof(ColorPaletteItem.RgbBrush))
        {
            return;
        }

        // Typing into the empty last row turns it into a real row and adds a new empty one, like a DataGridView.
        if (item.IsNewRow && e.PropertyName != nameof(ColorPaletteItem.IsNewRow) && !item.IsBlank)
        {
            item.IsNewRow = false;
            ColorPalette.Add(ColorPaletteItem.CreateNewRow());
        }

        ShowReparseButton();
    }

    /// <summary>
    /// The colors to draw with. An opener row is not a color but a button that opens the palette;
    /// it is clicked before choosing any of the colors listed after it, up to the next opener row.
    /// </summary>
    private List<ColorSpot> GetColorPalette()
    {
        var palette = new List<ColorSpot>();
        var opener = Point.Empty;
        foreach (var row in ColorPalette.Where(row => !row.IsNewRow))
        {
            if (row.IsOpener)
            {
                opener = ParsePalettePosition(row);
                continue;
            }
            var color = row.Rgb.ToColor();
            if (!color.IsEmpty)
            {
                palette.Add(new ColorSpot
                {
                    Color = color,
                    Point = ParsePalettePosition(row),
                    IsBackgroundColor = row.IsBackground,
                    Opener = opener
                });
            }
        }
        return palette;
    }

    /// <summary>
    /// A row without a valid position (e.g. only the RGB value was typed in) has no palette swatch to click;
    /// it must not turn into a click at the top-left corner of the screen.
    /// </summary>
    private static Point ParsePalettePosition(ColorPaletteItem row)
    {
        return int.TryParse(row.X, NumberStyles.Integer, CultureInfo.InvariantCulture, out var x)
               && int.TryParse(row.Y, NumberStyles.Integer, CultureInfo.InvariantCulture, out var y)
            ? new Point(x, y)
            : Point.Empty;
    }

    [RelayCommand]
    private async Task ExportColorsAsync()
    {
        var fileName = await _dialogs.PickPaletteExportPathAsync();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("X;Y;RGB;BG;Opener");
        foreach (var row in ColorPalette.Where(row => !row.IsNewRow))
        {
            sb.AppendLine($"{row.X};{row.Y};{row.Rgb};{row.IsBackground};{row.IsOpener}");
        }

        try
        {
            await File.WriteAllTextAsync(fileName, sb.ToString());
        }
        catch (Exception ex)
        {
            await _dialogs.ShowMessageAsync(ex.Message);
        }
    }

    [RelayCommand]
    private async Task ImportColorsAsync()
    {
        var fileName = await _dialogs.PickPaletteToImportAsync();
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        List<string[]> values;
        try
        {
            values = (await File.ReadAllLinesAsync(fileName))
                .Skip(1)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(';'))
                .ToList();
        }
        catch (Exception)
        {
            return;
        }
        if (values.Count == 0)
        {
            return;
        }

        foreach (var row in ColorPalette.Where(row => !row.IsNewRow).ToList())
        {
            ColorPalette.Remove(row);
        }
        foreach (var value in values)
        {
            AddPaletteRow(
                value.ElementAtOrDefault(0)?.Trim() ?? string.Empty,
                value.ElementAtOrDefault(1)?.Trim() ?? string.Empty,
                value.ElementAtOrDefault(2)?.Trim() ?? string.Empty,
                (value.ElementAtOrDefault(3) ?? string.Empty).Trim().ToBool(),
                (value.ElementAtOrDefault(4) ?? string.Empty).Trim().ToBool());
        }
    }

    #endregion

    #region Parsing

    private void UpdateOptions()
    {
        foreach (var setting in ParserSettings)
        {
            setting.PropertyChanged -= OnParserSettingChanged;
        }
        ParserSettings.Clear();
        if (SelectedParser != null)
        {
            foreach (var setting in _readers.GetDefaultSettings(SelectedParser))
            {
                var item = new ParserSettingItem(setting.Name, setting.DefaultValue);
                item.PropertyChanged += OnParserSettingChanged;
                ParserSettings.Add(item);
            }
        }
        ShowReparseButton();
    }

    private void OnParserSettingChanged(object? sender, PropertyChangedEventArgs e)
    {
        ShowReparseButton();
    }

    [RelayCommand]
    private async Task LoadImageAsync()
    {
        var imagePath = await _dialogs.PickImageAsync();
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return;
        }

        await ParseImageAsync(imagePath);
    }

    /// <summary>Parses an image dropped onto the window or pasted from the clipboard.</summary>
    public async Task<bool> TryLoadImageFromPathAsync(string? imagePath)
    {
        if (!CanLoadImage || string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
        {
            return false;
        }

        await ParseImageAsync(imagePath);
        return true;
    }

    [RelayCommand]
    private async Task ReparseAsync()
    {
        if (!string.IsNullOrEmpty(_lastParsedImage))
        {
            await ParseImageAsync(_lastParsedImage);
        }
    }

    private async Task ParseImageAsync(string imagePath)
    {
        // Without a position the colors behind the opener would be clicked while hidden, onto whatever is there.
        if (ColorPalette.Any(row => !row.IsNewRow && row.IsOpener && ParsePalettePosition(row).IsEmpty))
        {
            await _dialogs.ShowMessageAsync("Every opener row needs a position (X and Y).");
            return;
        }

        CanLoadImage = false;
        LoadImageButtonText = "Loading...";
        ShowReparse = false;
        _lastParsedImage = imagePath;
        _loadComplete = false;

        var parserOptions = new Dictionary<string, string>();
        foreach (var setting in ParserSettings)
        {
            parserOptions[setting.Name ?? string.Empty] = setting.Value ?? string.Empty;
        }
        var palette = GetColorPalette();
        var parser = SelectedParser;

        CalculationResult result;
        try
        {
            result = await Task.Run(() =>
            {
                if (parser == null)
                {
                    throw new InvalidOperationException("Please select a parser.");
                }
                var reader = _readers.Create(parser, imagePath);
                var actions = reader.GetDrawInstructions(palette, parserOptions).WithPaletteOpeners(palette).ToList();
                var bitmap = PixelBitmap.Load(imagePath);
                var preview = PreviewRenderer.RenderPng(actions, bitmap.Width, bitmap.Height);
                return new CalculationResult(actions, bitmap.Width, bitmap.Height, preview, null);
            });
        }
        catch (Exception ex)
        {
            result = new CalculationResult(null, 0, 0, null, ex);
        }

        CanLoadImage = true;
        LoadImageButtonText = "Parse Image";
        _loadComplete = true;

        if (result.Error != null)
        {
            await _dialogs.ShowMessageAsync(result.Error.Message);
            return;
        }

        _actions = result.Actions;
        PreviewHeight = result.ImageHeight;
        PreviewWidth = result.ImageWidth;
        using (var stream = new MemoryStream(result.PreviewPng!))
        {
            var previous = PreviewImage;
            PreviewImage = new Bitmap(stream);
            previous?.Dispose();
        }
        ShowClearUnusedColorsButton();
    }

    private void ShowReparseButton()
    {
        if (string.IsNullOrWhiteSpace(_lastParsedImage) || !_loadComplete)
        {
            return;
        }
        ShowReparse = true;
        ShowClearUnusedColorsButton();
    }

    private void ShowClearUnusedColorsButton()
    {
        ShowClearUnusedColors = GetUnusedColors().Any();
    }

    private bool ActionsLoaded() => _actions is { Count: > 0 };

    private IEnumerable<CoreColor> GetUnusedColors()
    {
        if (!ActionsLoaded())
        {
            return [];
        }
        return GetColorPalette()
            .Where(colorSpot => _actions!.All(x => x.Color.DifferenceTo(colorSpot.Color) != 0))
            .Where(colorSpot => colorSpot.Color.DifferenceTo(CoreColor.White) != 0)
            .Select(x => x.Color)
            .ToList();
    }

    [RelayCommand]
    private void ClearUnusedColors()
    {
        if (!ActionsLoaded())
        {
            return;
        }
        foreach (var color in GetUnusedColors())
        {
            if (color.R == 255 && color.G == 255 && color.B == 255)
            {
                continue;
            }
            RemovePaletteRows(ColorPalette.Where(row => !row.IsNewRow && !row.IsOpener && color.DifferenceTo(row.Rgb.ToColor()) == 0));
        }
        ShowClearUnusedColors = false;
    }

    private readonly record struct CalculationResult(
        List<MouseDragAction>? Actions,
        int ImageWidth,
        int ImageHeight,
        byte[]? PreviewPng,
        Exception? Error);

    #endregion

    #region Playback

    [RelayCommand]
    private Task PlayAsync()
    {
        if (_actions == null)
        {
            return Task.CompletedTask;
        }

        var actions = _actions.ToList();
        var offset = new Point(MousePositionX.ToInt(), MousePositionY.ToInt());
        return RunPlaybackAsync((mouse, token) =>
        {
            foreach (var action in actions.TakeWhile(_ => !token.IsCancellationRequested))
            {
                foreach (var _ in action.Play(offset, mouse, token))
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        });
    }

    /// <summary>
    /// Drives the mouse on a background thread until the playback finishes or Stop is pressed.
    /// Only one playback (PLAY or TEST) runs at a time; errors are shown to the user.
    /// </summary>
    private async Task RunPlaybackAsync(Action<IMouseOperations, CancellationToken> playback)
    {
        if (_platformServices == null || !_playTask.IsCompleted)
        {
            return;
        }

        var mouse = _platformServices.Mouse;
        using var cancellation = new CancellationTokenSource();
        _playCancellation = cancellation;
        _playTask = Task.Run(() => playback(mouse, cancellation.Token));
        Exception? error = null;
        try
        {
            await _playTask;
        }
        catch (Exception ex)
        {
            error = ex;
        }
        finally
        {
            _playCancellation = null;
        }

        // Only once this playback no longer owns the Stop hotkey, because another one may start meanwhile.
        if (error != null)
        {
            await _dialogs.ShowMessageAsync(error.Message);
        }
    }

    private void StopPlayback()
    {
        _playCancellation?.Cancel();
    }

    #endregion

    #region Debug panel

    private void AddDebugPoint()
    {
        if (_platformServices == null)
        {
            return;
        }

        var (x, y) = _platformServices.Mouse.GetCursorPosition();
        var currentDebugPoints = DebugRoutes;
        if (!currentDebugPoints.EndsWith(',') && !currentDebugPoints.EndsWith('\n') && currentDebugPoints.Trim().Length > 0)
        {
            currentDebugPoints += ", ";
        }
        currentDebugPoints += string.Create(CultureInfo.InvariantCulture, $"{x}|{y}");
        DebugRoutes = currentDebugPoints;
        DebugPointAdded?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void DebugAddPoint() => AddDebugPoint();

    [RelayCommand]
    private Task PlayDebugPointsAsync()
    {
        var lines = DebugRoutes.Split(["\r\n", "\n", "\r"], StringSplitOptions.None);
        return RunPlaybackAsync((mouse, token) =>
        {
            foreach (var line in lines.Where(l => l.Trim().Length > 0))
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var started = false;
                try
                {
                    foreach (var cor in line.Split(',').Select(point => point.Trim().Split('|')))
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }
                        var xy = cor.Where(x => x.Trim().Length > 0).ToArray();
                        var cx = int.Parse(xy[0], CultureInfo.InvariantCulture);
                        var cy = int.Parse(xy[1], CultureInfo.InvariantCulture);
                        mouse.SetCursorPosition(cx, cy);
                        if (!started)
                        {
                            started = true;
                            mouse.LeftMouseDown();
                        }
                        token.WaitHandle.WaitOne(10);
                    }
                }
                finally
                {
                    mouse.LeftMouseUp();
                }
            }
        });
    }

    #endregion
}

public partial class ColorPaletteItem : ObservableObject
{
    private static readonly IBrush EmptyBrush = Brushes.Transparent;

    [ObservableProperty]
    private string _x = string.Empty;

    [ObservableProperty]
    private string _y = string.Empty;

    [ObservableProperty]
    private string _rgb = string.Empty;

    [ObservableProperty]
    private bool _isBackground;

    /// <summary>A button that opens the palette, clicked before choosing any color in the rows below it.</summary>
    [ObservableProperty]
    private bool _isOpener;

    [ObservableProperty]
    private IBrush _rgbBrush = EmptyBrush;

    [ObservableProperty]
    private IBrush _rgbForeground = Brushes.Black;

    [ObservableProperty]
    private bool _isNewRow;

    public bool IsBlank => string.IsNullOrWhiteSpace(X) && string.IsNullOrWhiteSpace(Y) && string.IsNullOrWhiteSpace(Rgb) && !IsBackground && !IsOpener;

    public static ColorPaletteItem CreateNewRow() => new() { IsNewRow = true };

    // An opener is not a color, so it cannot be the background color either.
    partial void OnIsOpenerChanged(bool value)
    {
        if (value)
        {
            IsBackground = false;
        }
    }

    partial void OnIsBackgroundChanged(bool value)
    {
        if (value)
        {
            IsOpener = false;
        }
    }

    partial void OnRgbChanged(string value)
    {
        var color = value.ToColor();
        RgbBrush = color.IsEmpty ? EmptyBrush : new SolidColorBrush(global::Avalonia.Media.Color.FromRgb(color.R, color.G, color.B));
        // Keep the hex code readable on dark colors.
        var isDark = !color.IsEmpty && (color.R * 299 + color.G * 587 + color.B * 114) / 1000 < 128;
        RgbForeground = isDark ? Brushes.White : Brushes.Black;
    }
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
