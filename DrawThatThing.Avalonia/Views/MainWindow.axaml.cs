using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using DrawThatThing.Avalonia.ViewModels;
using DrawThatThing.Platform;

namespace DrawThatThing.Avalonia.Views;

public partial class MainWindow : Window
{
    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp"
    };

    private bool _isClipboardPasteInFlight;

    public MainWindow()
    {
        InitializeComponent();
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        PaletteGrid.KeyDown += OnPaletteGridKeyDown;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MainWindowViewModel vm)
        {
            vm.DebugPointAdded -= OnDebugPointAdded;
            vm.DebugPointAdded += OnDebugPointAdded;
        }
    }

    private void OnDebugPointAdded(object? sender, EventArgs e)
    {
        DebugRoutesBox.Focus();
        DebugRoutesBox.CaretIndex = DebugRoutesBox.Text?.Length ?? 0;
    }

    private void OnPaletteGridKeyDown(object? sender, KeyEventArgs e)
    {
        // Delete, or Cmd+Backspace as elsewhere on macOS. A plain Backspace on a clicked cell must not
        // throw the whole row away.
        var isDeleteGesture = e.Key == Key.Delete || (e.Key == Key.Back && e.KeyModifiers.HasFlag(KeyModifiers.Meta));
        if (!isDeleteGesture || e.Source is TextBox)
        {
            return;
        }

        DeleteSelectedPaletteRows();
        e.Handled = true;
    }

    private void OnDeletePaletteRowsClick(object? sender, RoutedEventArgs e)
    {
        DeleteSelectedPaletteRows();
    }

    private void DeleteSelectedPaletteRows()
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.RemovePaletteRows(PaletteGrid.SelectedItems.OfType<ColorPaletteItem>());
        }
    }

    /// <summary>
    /// Hotkeys that could not be registered system-wide still work while this window is focused.
    /// </summary>
    private bool TryHandleWindowHotkey(KeyEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm || e.Key < Key.A || e.Key > Key.Z)
        {
            return false;
        }

        var modifiers = HotkeyModifiers.None;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Alt)) modifiers |= HotkeyModifiers.Alt;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control)) modifiers |= HotkeyModifiers.Ctrl;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) modifiers |= HotkeyModifiers.Shift;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Meta)) modifiers |= HotkeyModifiers.Win;
        if (modifiers == HotkeyModifiers.None)
        {
            return false;
        }

        return vm.TryHandleWindowHotkey(modifiers, (char)('A' + (e.Key - Key.A)));
    }

    private async void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (TryHandleWindowHotkey(e))
        {
            e.Handled = true;
            return;
        }

        if (_isClipboardPasteInFlight)
        {
            return;
        }

        if (e.Key != Key.V)
        {
            return;
        }

        var isPasteGesture = e.KeyModifiers.HasFlag(KeyModifiers.Meta) || e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (!isPasteGesture || IsTextInputFocused())
        {
            return;
        }

        var imagePath = await TryGetClipboardImagePathAsync();
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        _isClipboardPasteInFlight = true;
        try
        {
            if (await TryOpenImageAsync(imagePath))
            {
                e.Handled = true;
            }
        }
        finally
        {
            _isClipboardPasteInFlight = false;
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = TryExtractImagePath(e.Data) != null
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        var imagePath = TryExtractImagePath(e.Data);
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        if (await TryOpenImageAsync(imagePath))
        {
            e.Handled = true;
        }
    }

    private async Task<bool> TryOpenImageAsync(string imagePath)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return false;
        }

        return await vm.TryLoadImageFromPathAsync(imagePath);
    }

    private async Task<string?> TryGetClipboardImagePathAsync()
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard == null)
        {
            return null;
        }

        var filesData = await clipboard.GetDataAsync(DataFormats.Files);
        var path = TryExtractImagePath(filesData);
        if (!string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        var fileNamesData = await clipboard.GetDataAsync(DataFormats.FileNames);
        path = TryExtractImagePath(fileNamesData);
        if (!string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        var text = await clipboard.GetTextAsync();
        return NormalizeImagePath(text);
    }

    private static string? TryExtractImagePath(IDataObject? data)
    {
        if (data == null)
        {
            return null;
        }

        if (data.Contains(DataFormats.Files))
        {
            var path = TryExtractImagePath(data.GetFiles());
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
        }

        if (data.Contains(DataFormats.FileNames))
        {
            var path = TryExtractImagePath(data.Get(DataFormats.FileNames));
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
        }

        if (data.Contains(DataFormats.Text))
        {
            var path = NormalizeImagePath(data.GetText());
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
        }

        return null;
    }

    private static string? TryExtractImagePath(object? data)
    {
        return data switch
        {
            IEnumerable<IStorageItem> items => TryExtractImagePath(items.Select(i => i.TryGetLocalPath())),
            IStorageItem item => NormalizeImagePath(item.TryGetLocalPath()),
            IEnumerable<string> names => TryExtractImagePath(names),
            string text => NormalizeImagePath(text),
            _ => null
        };
    }

    private static string? TryExtractImagePath(IEnumerable<string?> candidates)
    {
        foreach (var candidate in candidates)
        {
            var normalized = NormalizeImagePath(candidate);
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                return normalized;
            }
        }

        return null;
    }

    private static string? NormalizeImagePath(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var candidate = raw
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)[0]
            .Trim();

        if (candidate.Length > 1 &&
            ((candidate.StartsWith('"') && candidate.EndsWith('"')) ||
             (candidate.StartsWith('\'') && candidate.EndsWith('\''))))
        {
            candidate = candidate[1..^1];
        }

        if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri) && uri.IsFile)
        {
            candidate = uri.LocalPath;
        }
        else if (candidate.StartsWith("~/", StringComparison.Ordinal))
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            candidate = Path.Combine(home, candidate[2..]);
        }

        candidate = Environment.ExpandEnvironmentVariables(candidate);
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return null;
        }

        if (!Path.IsPathRooted(candidate))
        {
            return null;
        }

        var extension = Path.GetExtension(candidate);
        if (!SupportedImageExtensions.Contains(extension))
        {
            return null;
        }

        return candidate;
    }

    private bool IsTextInputFocused()
    {
        var focused = FocusManager?.GetFocusedElement();
        if (focused is not InputElement inputElement)
        {
            return false;
        }

        if (focused is TextBox || focused is MaskedTextBox)
        {
            return true;
        }

        return inputElement
            .GetSelfAndVisualAncestors()
            .OfType<InputElement>()
            .Any(control => control is TextBox or MaskedTextBox);
    }
}
