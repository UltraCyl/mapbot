# How to Copy DPB DLLs to Project Root

This guide shows you exactly how to copy the required DreamPoeBot DLLs to the MapBot project root directory.

## What You Need to Copy

You need to copy **3 files** from your DreamPoeBot installation to the MapBot project root:

1. `DreamPoeBot.exe`
2. `log4net.dll`
3. `Newtonsoft.Json.dll`

## Where to Find These Files

These files are in your DreamPoeBot installation folder. Common locations:
- `C:\DreamPoeBot\`
- `C:\Program Files\DreamPoeBot\`
- Or wherever you installed DreamPoeBot

## Where to Copy Them To

Copy them to the **MapBot project root** directory - the same folder where you see:
- `MapBot.sln`
- `MapBot.csproj`
- `README.md`

## Method 1: Using Windows File Explorer (Easiest)

### Step-by-Step:

1. **Open your DreamPoeBot installation folder**
   - Navigate to where DreamPoeBot is installed (e.g., `C:\DreamPoeBot\`)
   - You should see `DreamPoeBot.exe` and other files

2. **Select the 3 required files**
   - Click on `DreamPoeBot.exe`
   - Hold `Ctrl` and click on `log4net.dll`
   - Still holding `Ctrl`, click on `Newtonsoft.Json.dll`
   - All 3 files should now be highlighted

3. **Copy the files**
   - Press `Ctrl+C` or right-click and select "Copy"

4. **Navigate to MapBot project root**
   - Go to where you cloned the MapBot repository
   - Look for the folder containing `MapBot.sln`

5. **Paste the files**
   - Press `Ctrl+V` or right-click and select "Paste"
   - The 3 DLL files should now appear in your MapBot project folder

### Visual Check:

After copying, your MapBot project root should look like this:

```
mapbot/
├── DreamPoeBot.exe          ← You just copied this
├── log4net.dll              ← You just copied this
├── Newtonsoft.Json.dll      ← You just copied this
├── MapBot.sln               ← Already there
├── MapBot.csproj            ← Already there
├── README.md                ← Already there
├── .gitignore               ← Already there
└── 3rdParty/                ← Already there
```

## Method 2: Using PowerShell (Command Line)

### Step-by-Step:

1. **Open PowerShell**
   - Press `Win+R`, type `powershell`, press Enter

2. **Navigate to MapBot project root**
   ```powershell
   cd "C:\path\to\mapbot"
   # Replace with your actual MapBot folder path
   ```

3. **Copy the files using commands**
   ```powershell
   # Set your DPB installation path
   $DPB_PATH = "C:\DreamPoeBot"  # Change this to YOUR DPB path!
   
   # Copy the 3 required files
   Copy-Item "$DPB_PATH\DreamPoeBot.exe" -Destination .
   Copy-Item "$DPB_PATH\log4net.dll" -Destination .
   Copy-Item "$DPB_PATH\Newtonsoft.Json.dll" -Destination .
   ```

4. **Verify files were copied**
   ```powershell
   # Check if files exist
   Test-Path "DreamPoeBot.exe"       # Should return: True
   Test-Path "log4net.dll"           # Should return: True
   Test-Path "Newtonsoft.Json.dll"   # Should return: True
   ```

## Method 3: Using Command Prompt (CMD)

### Step-by-Step:

1. **Open Command Prompt**
   - Press `Win+R`, type `cmd`, press Enter

2. **Navigate to MapBot project root**
   ```cmd
   cd C:\path\to\mapbot
   ```

3. **Copy the files**
   ```cmd
   REM Change C:\DreamPoeBot to your actual DPB installation path
   copy "C:\DreamPoeBot\DreamPoeBot.exe" .
   copy "C:\DreamPoeBot\log4net.dll" .
   copy "C:\DreamPoeBot\Newtonsoft.Json.dll" .
   ```

4. **Verify files were copied**
   ```cmd
   dir DreamPoeBot.exe
   dir log4net.dll
   dir Newtonsoft.Json.dll
   ```

## Common Issues

### "Cannot find DreamPoeBot.exe"

**Problem:** You're looking in the wrong folder.

**Solution:** 
- Search for `DreamPoeBot.exe` in Windows Explorer
- Use the search box: type "DreamPoeBot.exe"
- It will show you the correct location

### "Access Denied" Error

**Problem:** You need administrator permissions.

**Solution:**
- Right-click PowerShell/CMD
- Select "Run as Administrator"
- Try copying again

### "File already exists"

**Problem:** You already copied the files.

**Solution:**
- You're done! The files are already there
- You can skip this step and proceed to building

## Verify You Did It Correctly

After copying, check that these files are in your MapBot project root:

```powershell
# In PowerShell, from the MapBot project root:
ls *.exe, *.dll | Select-Object Name, Length

# You should see:
# Name                   Length
# ----                   ------
# DreamPoeBot.exe        [some size]
# log4net.dll            [some size]
# Newtonsoft.Json.dll    [some size]
```

Or in Windows Explorer, you should see these 3 files next to `MapBot.sln`.

## What Happens Next?

After copying these files:

1. **Open MapBot.sln** in Visual Studio
2. **Build the project** (Ctrl+Shift+B)
3. The build should succeed without "missing reference" errors

## Note About .gitignore

Don't worry about accidentally committing these DLL files to git - they're already excluded in the `.gitignore` file. Git will ignore them automatically.

## Need Help?

If you're still stuck:

1. Check that DreamPoeBot is actually installed
2. Verify you have the correct MapBot folder (contains MapBot.sln)
3. Make sure you have permission to copy files to that location
4. Try Method 1 (File Explorer) - it's the most straightforward

---

**Quick Summary:**
1. Find DreamPoeBot installation folder
2. Copy `DreamPoeBot.exe`, `log4net.dll`, `Newtonsoft.Json.dll`
3. Paste into MapBot project root (where `MapBot.sln` is)
4. Verify files are there
5. Build the project!
