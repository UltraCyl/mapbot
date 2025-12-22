# MapBot for DreamPoeBot

MapBot is a bot for running maps in Path of Exile, ported to work with DreamPoeBot (DPB).

## Requirements

- DreamPoeBot installation
- .NET Framework 4.8 Developer Pack
- Visual Studio 2019 or later (or MSBuild)

## Building

**Important:** DPB_DIR should point to the folder containing `DreamPoeBot.exe` (the types are embedded in the EXE, not separate DLLs).

### Option 1: Using DPB_DIR environment variable

Set the `DPB_DIR` environment variable to point to your DreamPoeBot installation directory:

```cmd
set DPB_DIR=C:\Path\To\DreamPoeBot
dotnet build MapBot.csproj
```

or in PowerShell:

```powershell
$env:DPB_DIR = "C:\Path\To\DreamPoeBot"
dotnet build MapBot.csproj
```

or in Git Bash:

```bash
export DPB_DIR=/c/Path/To/DreamPoeBot
dotnet build MapBot.csproj
```

### Option 2: Using MSBuild property

For Git Bash:
```bash
dotnet build MapBot.csproj -p:DPB_DIR=C:/Users/pc/Desktop/DreamPoeBot-ForTesting/DreamPoeBot
```

For CMD/PowerShell:
```cmd
dotnet build MapBot.csproj /p:DPB_DIR=C:\Users\pc\Desktop\DreamPoeBot-ForTesting\DreamPoeBot
```

**Note:** Adjust the path to match your actual DreamPoeBot installation location.

### Option 3: Edit the project file

Edit `MapBot.csproj` and change the `DPB_DIR` default value to point to your DreamPoeBot installation:

```xml
<DPB_DIR Condition="'$(DPB_DIR)' == ''">C:\Path\To\Your\DreamPoeBot</DPB_DIR>
```

Then build:

```cmd
dotnet build MapBot.csproj
```

## Installation

After building, copy the compiled `MapBot.dll` from the output directory to your DreamPoeBot plugins directory:

```
Copy: bin\MapBot.dll
To: [DreamPoeBot]\Plugins\Compiled\MapBot\MapBot.dll
```

Or the appropriate plugins directory structure used by your DPB installation.

## Configuration

Configure MapBot through the DreamPoeBot user interface after loading the plugin.

## Namespace Changes from Legacy

This version has been ported from the legacy ExileBuddy API to DreamPoeBot:

- `Loki.*` → `DreamPoeBot.Loki.*`
- `Buddy.Coroutines` → `DreamPoeBot.Loki.Coroutine`

## Included Components

- **MapBot**: Main bot implementation for running maps
- **EXtensions Framework**: Shared framework including:
  - CommonTasks: Reusable task implementations
  - Global: Global logic and utilities
  - CachedObjects: Object caching system
  - Positions: Position handling utilities

## License

Original code by ExVault. Ported to DreamPoeBot.
