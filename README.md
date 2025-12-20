# MapBot for DreamPoeBot (DPB)

This is a DPB-compatible version of MapBot, ported from the legacy ExileBuddy framework to work with DreamPoeBot.

## What Changed

This version has been migrated from the legacy APIs to DreamPoeBot (DPB) compatible APIs:
- `Loki.*` → `DreamPoeBot.Loki.*`
- `Buddy.Coroutines` → `DreamPoeBot.Loki.Coroutine`

## Prerequisites

To build MapBot, you need:
1. **Visual Studio 2019 or later** (or Visual Studio 2022 recommended)
2. **.NET Framework 4.8 SDK** installed
3. **DreamPoeBot** installed on your system

## Required DLL References

MapBot requires the following DLL files from your DreamPoeBot installation:
- `DreamPoeBot.exe` - Main DPB executable
- `log4net.dll` - Logging framework
- `MahApps.Metro.dll` - UI framework (for NumericUpDown controls)
- `Newtonsoft.Json.dll` - JSON serialization

These DLLs should be located in the root directory of your DreamPoeBot installation.

## Building MapBot

### Option 1: Using Visual Studio

1. **Copy DreamPoeBot DLLs to the project root:**
   ```
   Copy the following files from your DreamPoeBot installation folder to the MapBot project root:
   - DreamPoeBot.exe
   - log4net.dll
   - MahApps.Metro.dll
   - Newtonsoft.Json.dll
   ```

2. **Open the solution:**
   ```
   Open MapBot.sln in Visual Studio
   ```

3. **Build the project:**
   - Select `Build > Build Solution` (or press Ctrl+Shift+B)
   - The output DLL will be in `bin\Debug\MapBot.dll` or `bin\Release\MapBot.dll`

### Option 2: Using MSBuild (Command Line)

1. **Copy DreamPoeBot DLLs** as described in Option 1

2. **Build using MSBuild:**
   ```powershell
   # For Debug build
   msbuild MapBot.sln /p:Configuration=Debug
   
   # For Release build
   msbuild MapBot.sln /p:Configuration=Release
   ```

## Installing MapBot in DreamPoeBot

1. **Locate your DreamPoeBot installation folder** (e.g., `C:\DreamPoeBot\`)

2. **Copy the built DLL:**
   ```
   Copy bin\Release\MapBot.dll to:
   [DreamPoeBot Installation]\3rdParty\Default\MapBot\MapBot.dll
   ```
   
   Or if the 3rdParty folder structure doesn't exist:
   ```
   [DreamPoeBot Installation]\Plugins\MapBot.dll
   ```

3. **Launch DreamPoeBot** and select MapBot from the bot list

## Project Structure

```
MapBot/
├── MapBot.sln                    # Visual Studio solution file
├── MapBot.csproj                 # Project file with build configuration
├── README.md                     # This file
└── 3rdParty/
    └── Default/
        ├── MapBot/               # MapBot source files
        │   ├── MapBot.cs         # Main bot class
        │   ├── *Task.cs          # Various task implementations
        │   └── ...
        ├── EXtensions/           # Shared framework code
        │   ├── CommonTasks/      # Common task implementations
        │   ├── Global/           # Global logic and utilities
        │   ├── Positions/        # Position handling
        │   └── ...
        └── Properties/
            └── AssemblyInfo.cs   # Assembly metadata
```

## Configuration

MapBot configuration is handled through DreamPoeBot's UI. After installing:
1. Start DreamPoeBot
2. Select MapBot from the bot dropdown
3. Configure settings in the MapBot GUI tab

## Troubleshooting

### Build Errors: "Could not resolve DreamPoeBot"
- Ensure you've copied the required DLLs (DreamPoeBot.exe, etc.) to the project root directory
- Check that the file paths in MapBot.csproj point to the correct locations

### MapBot doesn't appear in DreamPoeBot
- Verify MapBot.dll was copied to the correct folder
- Check DreamPoeBot logs for any loading errors
- Ensure you're using a compatible version of DreamPoeBot

### Runtime Errors
- Ensure all dependencies are present in the DreamPoeBot installation folder
- Check DreamPoeBot version compatibility (this version is based on DPB v3.14+)

## Known Differences from Legacy MapBot

This DPB-compatible version maintains the same core functionality as the legacy ExileBuddy version, but uses updated APIs:
- Coroutine handling now uses `DreamPoeBot.Loki.Coroutine` instead of `Buddy.Coroutines`
- All Loki API calls are prefixed with `DreamPoeBot.Loki.*`
- Bot lifecycle and message handling follow DPB patterns

## Contributing

When making changes:
1. Maintain compatibility with DreamPoeBot APIs
2. Follow existing code style and patterns
3. Test thoroughly before submitting changes
4. Update this README if adding new features or requirements

## License

This project follows the same license as the original MapBot source code.

## Credits

- Original MapBot authors (ExVault and contributors)
- DreamPoeBot team for the bot framework
- Community contributors

## Support

For issues related to:
- **Building/compiling**: Check this README and ensure all prerequisites are met
- **MapBot functionality**: Refer to MapBot documentation and community forums
- **DreamPoeBot compatibility**: Check DreamPoeBot documentation and support channels
