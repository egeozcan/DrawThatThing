using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using DrawThatThing.Avalonia.Services;
using DrawThatThing.Avalonia.ViewModels;
using DrawThatThing.Avalonia.Views;
using DrawThatThing.Platform.macOS;

namespace DrawThatThing.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            // On macOS the file dialogs need an Edit menu for Cmd+V etc. in their text fields.
            var dialogs = new DialogService(mainWindow, OperatingSystem.IsMacOS() ? MacAppIntegration.BeginNativeDialog : null);
            var viewModel = new MainWindowViewModel(PlatformServicesFactory.Create(), dialogs);
            mainWindow.DataContext = viewModel;

            mainWindow.Opened += (_, _) =>
            {
                viewModel.RegisterHotkeys();
                if (OperatingSystem.IsMacOS())
                {
                    // Drawing needs permission to control the mouse; let macOS ask for it right away.
                    MacAppIntegration.EnsureAccessibilityAccess();
                }
            };
            if (OperatingSystem.IsMacOS())
            {
                // The toolkit rearranges the menu bar when the window gains or loses the focus (e.g. to a file
                // dialog), so an open dialog's Edit menu has to be put back in its place afterwards.
                mainWindow.Activated += (_, _) =>
                    Dispatcher.UIThread.Post(MacAppIntegration.OnWindowActivationChanged, DispatcherPriority.Background);
                mainWindow.Deactivated += (_, _) =>
                    Dispatcher.UIThread.Post(MacAppIntegration.OnWindowActivationChanged, DispatcherPriority.Background);
            }
            desktop.ShutdownRequested += (_, _) => viewModel.UnregisterHotkeys();

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
