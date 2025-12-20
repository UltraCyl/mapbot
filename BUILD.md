# Build Instructions for MapBot DPB

This document provides step-by-step instructions for building MapBot for DreamPoeBot.

## Prerequisites Checklist

Before building, ensure you have:

- [ ] Visual Studio 2019 or later installed
- [ ] .NET Framework 4.8 SDK installed
- [ ] DreamPoeBot installed on your system
- [ ] Access to DreamPoeBot installation folder

## Build Steps

### 1. Prepare DLL References

Copy the following files from your DreamPoeBot installation folder to the MapBot project root:

```powershell
# Example paths - adjust to your DreamPoeBot installation location
$DPB_PATH = "C:\DreamPoeBot"  # Change this to your DPB installation path
$PROJECT_ROOT = "." # Current directory

Copy-Item "$DPB_PATH\DreamPoeBot.exe" -Destination $PROJECT_ROOT
Copy-Item "$DPB_PATH\log4net.dll" -Destination $PROJECT_ROOT
Copy-Item "$DPB_PATH\MahApps.Metro.dll" -Destination $PROJECT_ROOT
Copy-Item "$DPB_PATH\Newtonsoft.Json.dll" -Destination $PROJECT_ROOT
```

Or manually:
- Copy `DreamPoeBot.exe` from DPB installation to project root
- Copy `log4net.dll` from DPB installation to project root
- Copy `MahApps.Metro.dll` from DPB installation to project root
- Copy `Newtonsoft.Json.dll` from DPB installation to project root

### 2. Build with Visual Studio

1. Open `MapBot.sln` in Visual Studio
2. Select `Build > Build Solution` (or press `Ctrl+Shift+B`)
3. Wait for build to complete
4. Find the output DLL at `bin\Debug\MapBot.dll` or `bin\Release\MapBot.dll`

### 3. Build with MSBuild (Command Line)

```powershell
# For Debug build
msbuild MapBot.sln /p:Configuration=Debug /v:minimal

# For Release build
msbuild MapBot.sln /p:Configuration=Release /v:minimal
```

### 4. Install MapBot in DreamPoeBot

After successful build:

1. Locate your DreamPoeBot installation folder
2. Copy `bin\Release\MapBot.dll` to `[DreamPoeBot]\3rdParty\Default\MapBot\`
   - Create the directory structure if it doesn't exist
3. Launch DreamPoeBot
4. Select MapBot from the bot dropdown menu

## Troubleshooting Build Issues

### Error: "Could not load file or assembly 'DreamPoeBot'"

**Cause:** DreamPoeBot.exe not found in project root

**Solution:** 
1. Verify DreamPoeBot.exe is copied to the project root directory
2. Check that the file isn't corrupted
3. Ensure you have the correct version of DreamPoeBot

### Error: "The type or namespace name 'Loki' does not exist"

**Cause:** Old namespace references remain (shouldn't happen after migration)

**Solution:** 
1. Clean the solution: `Build > Clean Solution`
2. Rebuild: `Build > Rebuild Solution`
3. If persists, check for any files that weren't migrated

### Error: Missing log4net or Newtonsoft.Json

**Cause:** Required DLL dependencies not in project root

**Solution:**
1. Copy both log4net.dll and Newtonsoft.Json.dll from DPB installation
2. Ensure they're in the same directory as DreamPoeBot.exe

### Build succeeds but MapBot doesn't load in DPB

**Causes & Solutions:**

1. **Wrong DLL location:** Ensure MapBot.dll is in the correct folder
2. **Version mismatch:** Verify DPB version is v3.14 or later
3. **Missing dependencies:** Check that all referenced DLLs are present
4. **Check DPB logs:** Look for loading errors in DreamPoeBot logs

## Build Configuration

### Debug vs Release

**Debug Build:**
- Includes debugging symbols
- No optimizations
- Larger DLL size
- Use for development and troubleshooting

**Release Build:**
- Optimized code
- Smaller DLL size
- No debugging symbols
- Use for production/normal use

To change configuration in Visual Studio:
1. Use the dropdown at the top (usually shows "Debug" or "Release")
2. Select your desired configuration
3. Rebuild the solution

## Verification

After building, verify your MapBot.dll:

```powershell
# Check file exists
Test-Path "bin\Release\MapBot.dll"

# Check file size (should be a few hundred KB)
(Get-Item "bin\Release\MapBot.dll").Length

# Check assembly version
[Reflection.Assembly]::LoadFile("$PWD\bin\Release\MapBot.dll").GetName().Version
```

## Next Steps

Once built and installed:

1. Launch DreamPoeBot
2. Select MapBot from the bot list
3. Configure MapBot settings through the GUI
4. Start the bot and verify it works correctly

## Support

For build issues:
- Check this document thoroughly
- Verify all prerequisites are met
- Check DreamPoeBot community forums
- Review the main README.md for additional context

For MapBot functionality issues:
- Check MapBot settings and configuration
- Review DreamPoeBot logs
- Consult MapBot documentation
