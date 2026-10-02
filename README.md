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
├── DrawThatThing.Core/             # Platform-agnostic core library (image parsers, preview)
├── DrawThatThing.Platform/         # Platform abstraction interfaces
├── DrawThatThing.Platform.Windows/ # Windows-specific implementations
├── DrawThatThing.Platform.macOS/   # macOS-specific implementations
├── DrawThatThing.Platform.Linux/   # Linux-specific implementations
├── DrawThatThing.sln               # Solution file
├── build.sh                        # Build script for Linux/macOS
├── build-macos-app.sh              # Builds DrawThatThing.app for macOS
└── build.cmd                       # Build script for Windows
```

---

## Requirements

| Platform | .NET SDK | Additional Requirements |
|----------|----------|------------------------|
| All | .NET 9.0 SDK or later | - |
| Windows | - | None (Win32 APIs included) |
| macOS | - | Accessibility permissions (System Preferences → Security & Privacy → Privacy → Accessibility) |
| Linux | - | X11, libXtst (XTest extension) |

## Installing .NET 9 SDK

### Windows
```powershell
# Using winget
winget install Microsoft.DotNet.SDK.9

# Or download from https://dotnet.microsoft.com/download/dotnet/9.0
```

### macOS
```bash
# Using Homebrew
brew install --cask dotnet-sdk

# Or download from https://dotnet.microsoft.com/download/dotnet/9.0
```

### Linux (Ubuntu/Debian)
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

# Install X11 dependencies for mouse automation
sudo apt-get install -y libx11-dev libxtst-dev
```

### Linux (Fedora)
```bash
sudo dnf install dotnet-sdk-9.0
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

The easiest way to run DrawThatThing on a Mac is as an application bundle:

```bash
./build-macos-app.sh            # creates publish/DrawThatThing.app
open publish/DrawThatThing.app
```

Running it as a bundle (rather than through `dotnet run` in a terminal) makes macOS ask for
permissions for **DrawThatThing** itself instead of for your terminal app.

DrawThatThing needs two permissions, both under **System Settings → Privacy & Security**:

- **Accessibility**: to move and click the mouse. Without it macOS silently ignores the drawing.
  The app asks for it on start-up.
- **Screen Recording** (called **Screen & System Audio Recording** on macOS 15 and later): to pick
  colors from the screen with the "Pick color" hotkey. Without it the colors cannot be read and the
  app tells you so instead of adding a color. The app asks
  for it the first time you pick a color. You may have to restart the app after granting it.

The hotkeys are system-wide (you can use them while another app, e.g. your browser, is in front)
and don't need any permission. macOS 15 (Sequoia) and later no longer allow system-wide hotkeys
that only use **Shift + Option**, so on those versions the hotkeys are **Control + Option + key**
instead. The window always shows the combination that is active.

The application uses:
- CoreGraphics for mouse control (`CGEventCreateMouseEvent`, `CGEventPost`)
- Carbon for global hotkeys (`RegisterEventHotKey`)
- CoreGraphics for screen capture (`CGWindowListCreateImage`, falling back to the `screencapture` tool)

### Linux

Requires X11 and the XTest extension. Wayland is not currently supported for mouse automation or
global hotkeys (under XWayland the hotkeys only fire while an X11 window has the focus).

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

| Hotkey | macOS 15+ | Action |
|--------|-----------|--------|
| `Shift+Alt+C` | `Control+Option+C` | Stop playback |
| `Shift+Alt+S` | `Control+Option+S` | Set the mouse start position to the cursor |
| `Shift+Alt+A` | `Control+Option+A` | Add the color under the cursor (and its position) to the palette |
| `Shift+Alt+D` | `Control+Option+D` | Toggle the debug panel |
| `Shift+Alt+Q` | `Control+Option+Q` | Add the cursor position as a debug point |

On macOS before 15 the hotkeys are `Shift+Option+…`.

---

## Usage

1. Open the drawing application you want to draw in.
2. Build the **Color Palette**: hover over each color of the drawing application's palette and press
   the *pick color* hotkey. This stores where the color is on the screen, so DrawThatThing can click it
   when it needs that color. You can also type rows in by hand, mark a color as the background color
   (**BG Color**), delete rows with the Delete key (Cmd+Backspace on a Mac) or the right-click menu, and **Export**/**Import**
   palettes as CSV files.
3. Choose a **Parser** and adjust its **Parser Settings**:
   - **AbstractReader**: fills areas of the same color with strokes; needs a background color.
     `MinimumStrokeSize` skips tiny areas.
   - **DetailedReader**: chains neighboring pixels of the same color into strokes; skips white.
   - **LinearReader**: draws every dark pixel (`MaxLight` is the maximum R+G+B) using the black palette color.
   - **PointReader**: clicks every pixel on its own; `MixPoints` randomizes the order.
4. Click **Parse Image** (or drop an image onto the window) and check the preview. After changing the
   palette or the settings, **Refresh** parses the same image again, and **Clear Unused Colors** removes
   palette colors that the image doesn't use.
5. Hover over the spot where the top-left corner of the drawing should go and press the
   *set start position* hotkey, then click **PLAY >>**. Press the *stop* hotkey to stop.

Additional parsers can be added by putting a DLL with an `IBitmapReader` implementation (with a
constructor taking the image path) into a `Plugins` folder next to the application's executable. For
the macOS bundle that is `DrawThatThing.app/Contents/MacOS/Plugins`; adding files there invalidates the
bundle's signature, so sign it again afterwards with `codesign --force --deep --sign - DrawThatThing.app`.

---

## Troubleshooting

### Mouse automation not working

**Windows**: Run the application as Administrator if mouse events are being blocked.

**macOS**: Ensure Accessibility permissions are granted (see macOS notes above). If you rebuilt the
app, remove it from the Accessibility list and add it again; macOS ties the permission to the exact build.

**Linux**:
- Verify XTest extension is installed: `xdpyinfo | grep -i xtest`
- Ensure you're running under X11, not pure Wayland

### Application won't start

**All platforms**: Verify .NET 9 SDK is installed: `dotnet --version`

**Linux**: Install missing dependencies:
```bash
sudo apt-get install libicu-dev libssl-dev
```

### Hotkeys not responding

Check the combinations shown at the bottom of the window (Control + Option on macOS 15+). A hotkey
shown in red is already used by another application, so it only works while the DrawThatThing window
is focused; quit the other application and restart DrawThatThing to get it back.

**Linux**: Ensure no other application is grabbing those key combinations.

### Picking a color fails (macOS)

Grant the Screen Recording (Screen & System Audio Recording) permission and restart the application;
macOS only applies it after a restart.

---

## Links

- **Project Page**: http://egeozcan.github.com/DrawThatThing
- **Issues**: https://github.com/egeozcan/DrawThatThing/issues

---

## License

This project is licensed under the GPL-3.0 License - see the [LICENSE.txt](LICENSE.txt) file for details.
