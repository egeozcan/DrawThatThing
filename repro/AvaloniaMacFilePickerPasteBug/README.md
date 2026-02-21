# Avalonia macOS File Picker Paste Repro

This is a minimal repro app for a macOS file picker shortcut issue.

## Environment
- Avalonia: 11.1.0
- SDK: .NET 9
- macOS: fill your version when filing

## Run
```bash
dotnet run --project /Users/egecan/Code/DrawThatThing/repro/AvaloniaMacFilePickerPasteBug/AvaloniaMacFilePickerPasteBug.csproj
```

## Repro Steps
1. In the app, paste into `Clipboard test` textbox with `Cmd+V` (works).
2. Click `Open File Picker`.
3. In the file picker, press `Cmd+Shift+G` to open the path modal.
4. Try `Cmd+V` (paste) and `Cmd+A` (select all) inside that path text field.

## Expected
- `Cmd+V` pastes and `Cmd+A` selects all in the path modal.

## Actual
- `Cmd+V` does not paste (OS beep), while in-app textbox shortcuts work.

## Notes
- This repro intentionally has no custom app-level keyboard handling.
