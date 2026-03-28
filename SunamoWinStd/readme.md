# SunamoWinStd

Windows-specific utilities for .NET: NTFS junction/reparse points, process management, file locking detection, clipboard operations, browser/editor launching, and OS helpers.

## Features

- **JunctionPoint** - Create, delete, detect, and resolve NTFS junction points
- **ReparsePoint** - Resolve NTFS reparse point targets (junctions, symlinks, mount points)
- **PH** - Process helper: start processes, detect running processes, handle management
- **PHWin** - Open files in VS Code/Codium, launch URLs in browsers, detect installed browsers
- **FileUtil** - Detect which process is locking a file (via `handle.exe`)
- **WindowsOSHelper** - AppData folder resolution, executable lookup in PATH, admin detection
- **ClipboardHelperWinStd** - Clipboard read/write via TextCopy
- **TidyExeHelper** - HTML Tidy integration via memory-mapped file IPC
- **KnownFolders** - Windows known folder GUIDs

## Target Frameworks

`net10.0`, `net9.0`, `net8.0`

## Installation

```
dotnet add package SunamoWinStd
```

## Links

- [NuGet](https://www.nuget.org/profiles/sunamo)
- [GitHub](https://github.com/sunamo/PlatformIndependentNuGetPackages)
- [Developer site](https://sunamo.cz)

Request for new features / bug report / etc: [Mail](mailto:radek.jancik@sunamo.cz) or on GitHub
