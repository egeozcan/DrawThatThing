#!/bin/bash

# Builds DrawThatThing.app, a self-contained macOS application bundle.
#
# Running from a bundle (instead of `dotnet run` in a terminal) means macOS asks for the
# Accessibility and Screen Recording permissions for "DrawThatThing" itself, rather than for
# the terminal application.
#
# Usage: ./build-macos-app.sh [osx-arm64|osx-x64]   (defaults to the current machine's architecture)

set -e

cd "$(dirname "$0")"

RID="$1"
if [ -z "$RID" ]; then
    if [ "$(uname -m)" = "arm64" ]; then RID="osx-arm64"; else RID="osx-x64"; fi
fi

PUBLISH_DIR="publish/$RID"
APP_DIR="publish/DrawThatThing.app"

echo "Publishing for $RID..."
dotnet publish DrawThatThing.Avalonia/DrawThatThing.Avalonia.csproj -c Release -r "$RID" --self-contained -o "$PUBLISH_DIR"

echo "Creating $APP_DIR..."
rm -rf "$APP_DIR"
mkdir -p "$APP_DIR/Contents/MacOS" "$APP_DIR/Contents/Resources"
cp -R "$PUBLISH_DIR/." "$APP_DIR/Contents/MacOS/"

# LSMinimumSystemVersion matches the oldest macOS the bundled .NET 9 runtime supports.
cat > "$APP_DIR/Contents/Info.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>DrawThatThing</string>
    <key>CFBundleDisplayName</key>
    <string>DrawThatThing</string>
    <key>CFBundleIdentifier</key>
    <string>com.github.egeozcan.drawthatthing</string>
    <key>CFBundleExecutable</key>
    <string>DrawThatThing.Avalonia</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleVersion</key>
    <string>1.0</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>LSMinimumSystemVersion</key>
    <string>12.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
PLIST

if command -v codesign >/dev/null 2>&1; then
    # macOS ties granted permissions to the code signature. An ad-hoc signature (the default) changes with every
    # build, so the permissions have to be granted again after each rebuild. Signing with a real identity keeps
    # them: SIGN_IDENTITY="Developer ID Application: Your Name (TEAMID)" ./build-macos-app.sh
    codesign --force --deep --sign "${SIGN_IDENTITY:--}" "$APP_DIR"
fi

echo ""
echo "Done: $APP_DIR"
echo "On first use, allow DrawThatThing in System Settings → Privacy & Security under"
echo "  • Accessibility (to move and click the mouse)"
echo "  • Screen Recording, called Screen & System Audio Recording on macOS 15+ (to pick colors from the screen)"
