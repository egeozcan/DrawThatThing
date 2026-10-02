using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Platform.Storage;
using DrawThatThing.Avalonia.Services;

namespace DrawThatThing.Tests.Avalonia;

public class DialogServiceTests
{
    private sealed class Scope : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }

    /// <summary>Records, for every picker shown, whether the native dialog scope was open at that moment.</summary>
    private sealed class RecordingStorageProvider(Func<Scope?> currentScope) : IStorageProvider
    {
        public List<bool> ScopeOpenWhilePicking { get; } = [];

        public bool CanOpen => true;
        public bool CanSave => true;
        public bool CanPickFolder => true;

        public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            await Task.Yield(); // the native panel stays open across awaits
            ScopeOpenWhilePicking.Add(currentScope() is { Disposed: false });
            return [];
        }

        public async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
        {
            await Task.Yield();
            ScopeOpenWhilePicking.Add(currentScope() is { Disposed: false });
            return null;
        }

        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options) => throw new NotSupportedException();
        public Task<IStorageBookmarkFile?> OpenFileBookmarkAsync(string bookmark) => throw new NotSupportedException();
        public Task<IStorageBookmarkFolder?> OpenFolderBookmarkAsync(string bookmark) => throw new NotSupportedException();
        public Task<IStorageFile?> TryGetFileFromPathAsync(Uri filePath) => throw new NotSupportedException();
        public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath) => throw new NotSupportedException();
        public Task<IStorageFolder?> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder) => throw new NotSupportedException();
    }

    [AvaloniaFact]
    public async Task EveryFilePickerRunsInsideANativeDialogScope()
    {
        var scopes = new List<Scope>();
        var storage = new RecordingStorageProvider(() => scopes.LastOrDefault());
        var service = new DialogService(new Window(), () =>
        {
            var scope = new Scope();
            scopes.Add(scope);
            return scope;
        }, storage);

        await service.PickImageAsync();
        await service.PickPaletteToImportAsync();
        await service.PickPaletteExportPathAsync();

        Assert.Equal([true, true, true], storage.ScopeOpenWhilePicking);
        Assert.Equal(3, scopes.Count);
        Assert.All(scopes, scope => Assert.True(scope.Disposed));
    }
}
