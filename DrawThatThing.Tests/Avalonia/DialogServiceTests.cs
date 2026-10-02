using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using DrawThatThing.Avalonia.Services;

namespace DrawThatThing.Tests.Avalonia;

public class DialogServiceTests
{
    private sealed class Scope : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose() => Disposed = true;
    }

    [AvaloniaFact]
    public async Task EveryFilePickerRunsInsideANativeDialogScope()
    {
        var scopes = new List<Scope>();
        var service = new DialogService(new Window(), () =>
        {
            var scope = new Scope();
            scopes.Add(scope);
            return scope;
        });

        await service.PickImageAsync();
        await service.PickPaletteToImportAsync();
        await service.PickPaletteExportPathAsync();

        Assert.Equal(3, scopes.Count);
        Assert.All(scopes, scope => Assert.True(scope.Disposed));
    }
}
