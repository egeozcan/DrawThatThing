# DrawThatThing

A cross-platform desktop application that converts images into automated mouse drag actions, allowing you to "replay" an image as a drawing.

![Screenshot of DrawThatThing](http://i.imgur.com/KCvriGW.png "Screenshot")

## Features

- **Image Parsing**: Load images and analyze pixel data using multiple parsing algorithms
- **Mouse Automation**: Convert parsed image data to mouse drag sequences and replay them
- **Color Management**: Define custom color palettes, pick colors from screen, mark background colors
- **Plugin Architecture**: Extensible bitmap readers and brush changers
- **Global Hotkeys**: System-wide keyboard shortcuts for quick actions
- **Live Preview**: See how the image will be drawn before executing

## Project Structure

```
DrawThatThing/
├── DrawThatThing.Avalonia/         # Main Avalonia UI application
├── DrawThatThing.Core/             # Platform-agnostic core library
├── DrawThatThing.Platform/         # Platform abstraction interfaces
├── DrawThatThing.Platform.Windows/ # Windows-specific implementations
├── DrawThatThing.Platform.macOS/   # macOS-specific implementations
├── DrawThatThing.Platform.Linux/   # Linux-specific implementations
├── DrawThatThing.sln               # Solution file
├── build.sh                        # Build script for Linux/macOS
└── build.cmd                       # Build script for Windows
```

---

## Requirements

| Platform | .NET SDK | Additional Requirements |
|----------|----------|------------------------|
| All | .NET 8.0 SDK or later | - |
| Windows | - | None (Win32 APIs included) |
| macOS | - | Accessibility permissions (System Preferences → Security & Privacy → Privacy → Accessibility) |
| Linux | - | X11, libXtst (XTest extension) |

## Installing .NET 8 SDK

### Windows
```powershell
# Using winget
winget install Microsoft.DotNet.SDK.8

# Or download from https://dotnet.microsoft.com/download/dotnet/8.0
```

### macOS
```bash
# Using Homebrew
brew install --cask dotnet-sdk

# Or download from https://dotnet.microsoft.com/download/dotnet/8.0
```

### Linux (Ubuntu/Debian)
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Install X11 dependencies for mouse automation
sudo apt-get install -y libx11-dev libxtst-dev
```

### Linux (Fedora)
```bash
sudo dnf install dotnet-sdk-8.0
sudo dnf install libX11-devel libXtst-devel
```

### Linux (Arch)
```bash
sudo pacman -S dotnet-sdk
sudo pacman -S libx11 libxtst
```

---

## Building from Source

```bash
# Restore dependencies
dotnet restore DrawThatThing.sln

# Build in Debug mode
dotnet build DrawThatThing.sln

# Build in Release mode
dotnet build DrawThatThing.sln -c Release
```

Or use the provided build scripts:

```bash
# Linux/macOS
./build.sh

# Windows
build.cmd
```

## Running the Application

```bash
dotnet run --project DrawThatThing.Avalonia/DrawThatThing.Avalonia.csproj
```

## Publishing Self-Contained Executables

Create standalone executables that don't require .NET to be installed:

### Windows (x64)
```bash
dotnet publish DrawThatThing.Avalonia -c Release -r win-x64 --self-contained -o publish/win-x64
```

### Windows (ARM64)
```bash
dotnet publish DrawThatThing.Avalonia -c Release -r win-arm64 --self-contained -o publish/win-arm64
```

### macOS (Intel)
```bash
dotnet publish DrawThatThing.Avalonia -c Release -r osx-x64 --self-contained -o publish/osx-x64
```

### macOS (Apple Silicon)
```bash
dotnet publish DrawThatThing.Avalonia -c Release -r osx-arm64 --self-contained -o publish/osx-arm64
```

### Linux (x64)
```bash
dotnet publish DrawThatThing.Avalonia -c Release -r linux-x64 --self-contained -o publish/linux-x64
```

### Linux (ARM64)
```bash
dotnet publish DrawThatThing.Avalonia -c Release -r linux-arm64 --self-contained -o publish/linux-arm64
```

---

## Platform-Specific Notes

### Windows

No additional configuration required. The application uses Win32 APIs for:
- Mouse control (`user32.dll` - SetCursorPos, mouse_event)
- Global hotkeys (`user32.dll` - RegisterHotKey)
- Screen capture (`gdi32.dll` - BitBlt)

### macOS

**Important**: You must grant Accessibility permissions for mouse automation to work.

1. Open **System Preferences** (or **System Settings** on macOS Ventura+)
2. Go to **Security & Privacy** → **Privacy** → **Accessibility**
3. Click the lock icon to make changes
4. Add **DrawThatThing** to the list and enable it

The application uses:
- CoreGraphics framework for mouse control (`CGEventCreateMouseEvent`, `CGEventPost`)
- Carbon framework for global hotkeys (`RegisterEventHotKey`)
- CoreGraphics for screen capture (`CGWindowListCreateImage`)

### Linux

Requires X11 and the XTest extension. Wayland is not currently supported for mouse automation.

**Check if XTest is available:**
```bash
xdpyinfo | grep -i xtest
```

**Install dependencies:**
```bash
# Ubuntu/Debian
sudo apt-get install libx11-6 libxtst6

# Fedora
sudo dnf install libX11 libXtst

# Arch
sudo pacman -S libx11 libxtst
```

**For Wayland users**: You may need to run the application under XWayland or switch to an X11 session for full functionality.

The application uses:
- X11 for mouse control (`XTestFakeMotionEvent`, `XTestFakeButtonEvent`)
- X11 for global hotkeys (`XGrabKey`)
- X11 for screen capture (`XGetImage`)

---

## Global Hotkeys

| Hotkey | Action |
|--------|--------|
| `Shift+Alt+C` | Stop playback |
| `Shift+Alt+S` | Capture current mouse position |
| `Shift+Alt+A` | Add color at current cursor position |
| `Shift+Alt+D` | Toggle debug panel |
| `Shift+Alt+Q` | Add debug point |

---

## Troubleshooting

### Mouse automation not working

**Windows**: Run the application as Administrator if mouse events are being blocked.

**macOS**: Ensure Accessibility permissions are granted (see macOS notes above).

**Linux**:
- Verify XTest extension is installed: `xdpyinfo | grep -i xtest`
- Ensure you're running under X11, not pure Wayland

### Application won't start

**All platforms**: Verify .NET 8 SDK is installed: `dotnet --version`

**Linux**: Install missing dependencies:
```bash
sudo apt-get install libicu-dev libssl-dev
```

### Hotkeys not responding

**Windows**: Another application may have registered the same hotkeys.

**macOS**: Grant Accessibility permissions and restart the application.

**Linux**: Ensure no other application is grabbing those key combinations.

---

## Links

- **Project Page**: http://egeozcan.github.com/DrawThatThing
- **Issues**: https://github.com/egeozcan/DrawThatThing/issues

---

## License

This project is licensed under the GPL-3.0 License - see the [LICENSE.txt](LICENSE.txt) file for details.
