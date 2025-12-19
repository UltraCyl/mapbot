# Quick Reference - MapBot DPB

Quick reference for developers working with the DPB-compatible MapBot.

## Project Structure

```
mapbot/
├── MapBot.sln              # Visual Studio solution
├── MapBot.csproj           # Build project file
├── README.md               # Main documentation
├── BUILD.md                # Build instructions
├── API_MIGRATION.md        # API reference
├── CHANGELOG.md            # Version history
├── .gitignore              # Git exclusions
└── 3rdParty/
    └── Default/
        ├── MapBot/         # Bot implementation (26 files)
        ├── EXtensions/     # Shared framework (65 files)
        └── Properties/     # Assembly info
```

## Build Quick Start

```powershell
# 1. Copy DLL references to project root
Copy-Item "C:\DreamPoeBot\DreamPoeBot.exe" .
Copy-Item "C:\DreamPoeBot\log4net.dll" .
Copy-Item "C:\DreamPoeBot\Newtonsoft.Json.dll" .

# 2. Build
msbuild MapBot.sln /p:Configuration=Release

# 3. Deploy
Copy-Item "bin\Release\MapBot.dll" "C:\DreamPoeBot\3rdParty\Default\MapBot\"
```

## Namespace Quick Reference

| Legacy | DPB |
|--------|-----|
| `Loki.Bot` | `DreamPoeBot.Loki.Bot` |
| `Loki.Game` | `DreamPoeBot.Loki.Game` |
| `Buddy.Coroutines` | `DreamPoeBot.Loki.Coroutine` |

## Common Using Statements

```csharp
using System.Threading.Tasks;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Bot.Pathfinding;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Coroutine;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;
```

## Common Patterns

### Bot Class Structure
```csharp
public class MapBot : IBot, ITaskManagerHolder, IUrlProvider
{
    public void Initialize() { }
    public void Start() { }
    public void Tick() { }
    public void Stop() { }
    public void Deinitialize() { }
    public MessageResult Message(Message message) { }
    public async Task<LogicResult> Logic(Logic logic) { }
}
```

### Task Structure
```csharp
public class ExampleTask : ITask
{
    public async Task<bool> Run()
    {
        // Implementation
        return true;
    }
    
    public MessageResult Message(Message message) 
        => MessageResult.Unprocessed;
        
    public async Task<LogicResult> Logic(Logic logic) 
        => LogicResult.Unprovided;
        
    public void Start() { }
    public void Tick() { }
    public void Stop() { }
    
    public string Name => "ExampleTask";
    public string Description => "Task description";
    public string Author => "Author";
    public string Version => "1.0";
}
```

### Coroutine Usage
```csharp
await Coroutine.Sleep(1000);  // Wait 1 second
await Coroutine.Yield();      // Yield control
```

### Common APIs
```csharp
// World and areas
World.CurrentArea
World.Act1.LioneyeWatch

// Bot management
BotManager.Current
BotManager.Stop()

// Task management
_taskManager.Add(new ExampleTask());
_taskManager.Start();

// Player
LokiPoe.Me
LokiPoe.Me.IsDead

// Exploration
ExilePather.Reload()
ComplexExplorer.AddSettingsProvider(...)

// Logging
GlobalLog.Info("Message")
GlobalLog.Error("Error")
```

## File Organization

### MapBot Files (3rdParty/Default/MapBot/)
- **MapBot.cs** - Main bot class
- ***Task.cs** - Task implementations
- **Settings.cs** - Configuration classes
- **Data.cs** - Data structures

### EXtensions Files (3rdParty/Default/EXtensions/)
- **CommonTasks/** - Reusable tasks
- **Global/** - Global logic (Travel, Combat, etc.)
- **CachedObjects/** - Object caching
- **Positions/** - Position utilities
- Core utilities (Wait, Move, World, etc.)

## Development Workflow

### Making Changes

1. **Edit source files** in 3rdParty/Default/
2. **Build project:** `msbuild MapBot.sln /p:Configuration=Debug`
3. **Copy DLL:** To DPB installation
4. **Test:** Run in DreamPoeBot
5. **Iterate:** Repeat as needed

### Adding New Task

1. Create new `*Task.cs` in MapBot folder
2. Implement `ITask` interface
3. Add to MapBot.csproj `<Compile Include="..." />`
4. Register in MapBot.cs `AddTasks()` method
5. Rebuild and test

### Debugging

- Use Debug build for symbols
- Check DreamPoeBot logs
- Use GlobalLog for tracing
- Verify task execution order

## Common Issues

| Issue | Solution |
|-------|----------|
| Build fails | Check DLL references in project root |
| DLL not found | Verify output path, check .gitignore |
| Bot won't load | Check DPB logs, verify DLL location |
| Task not running | Check task registration in AddTasks() |
| Namespace errors | Ensure using DreamPoeBot.Loki.* |

## Resources

- **README.md** - Installation and setup
- **BUILD.md** - Detailed build guide
- **API_MIGRATION.md** - Complete API reference
- **CHANGELOG.md** - Version history

## Testing Checklist

Before committing changes:

- [ ] Code builds without errors
- [ ] No namespace errors
- [ ] DLL loads in DreamPoeBot
- [ ] Bot starts successfully
- [ ] Tasks execute as expected
- [ ] No runtime exceptions
- [ ] Logging works correctly

## Git Workflow

```bash
# Check status
git status

# Stage changes
git add .

# Commit
git commit -m "Description of changes"

# Push
git push origin branch-name
```

## Support

- Check documentation files (README, BUILD, API_MIGRATION)
- Review DreamPoeBot forums
- Check FollowBot v6.6 for patterns
- Review commit history for context

---

*Quick Reference v1.0 - DPB Migration*
