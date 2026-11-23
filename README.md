# SunamoWinStd

## Short description

Windows specifický kód pro net9.0, například nástroje pro práci se soubory a procesy. Součást sbírky pinp s testy a Runnerem.


Windows-specific code on net9.0 moniker

## Overview

SunamoWinStd is part of the Sunamo package ecosystem, providing modular, platform-independent utilities for .NET development.

## Main Components

### Key Classes

- **FileUtil**
- **ClipboardHelperWinStd**
- **KnownFolders**

### Key Methods

- `WhoIsLocking()`
- `Instance()`
- `ContainsText()`
- `GetLines()`
- `SetLines()`
- `SetList()`
- `PathsAndTargetsOfAll()`
- `Create()`
- `IsReparsePoint()`
- `Delete()`

## Installation

```bash
dotnet add package SunamoWinStd
```

## Dependencies

- **Microsoft.Extensions.Logging.Abstractions** (v9.0.3)
- **System.Management** (v9.0.3)
- **TextCopy** (v6.2.1)

## Package Information

- **Package Name**: SunamoWinStd
- **Version**: 25.8.27.2
- **Target Framework**: net9.0
- **Category**: Platform-Independent NuGet Package
- **Source Files**: 37

## Related Packages

This package is part of the Sunamo package ecosystem. For more information about related packages, visit the main repository.

## License

See the repository root for license information.

<!-- sunamo:test-coverage:start -->
## Test coverage

| Metric | Value |
|---|---|
| Method | static |
| Test projects | 1 |
| Public types referenced by tests | 7/21 (33.3 %) |
| Public methods referenced by tests | 20/76 (26.3 %) |
| Line coverage | n/a |
| Branch coverage | n/a |
| Generated (UTC) | 2026-11-22 21:24 |

<!-- sunamo:test-coverage:data {"package":"SunamoWinStd","container":"pinp","testProjects":1,"publicTypes":21,"typesReferencedByTests":7,"publicMethods":76,"methodsReferencedByTests":20,"lineRate":null,"branchRate":null,"method":"static","generatedUtc":"2026-11-22 21:24"} -->
<!-- sunamo:test-coverage:end -->
