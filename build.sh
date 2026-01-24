#!/bin/bash

# Build script for DrawThatThing cross-platform application

set -e

echo "Building DrawThatThing..."

# Restore packages
dotnet restore DrawThatThing.sln

# Build in Release mode
dotnet build DrawThatThing.sln -c Release

echo "Build completed successfully!"
echo ""
echo "To run the application:"
echo "  dotnet run --project DrawThatThing.Avalonia/DrawThatThing.Avalonia.csproj"
echo ""
echo "To publish for different platforms:"
echo "  Windows: dotnet publish DrawThatThing.Avalonia -c Release -r win-x64 --self-contained"
echo "  macOS:   dotnet publish DrawThatThing.Avalonia -c Release -r osx-x64 --self-contained"
echo "  Linux:   dotnet publish DrawThatThing.Avalonia -c Release -r linux-x64 --self-contained"
