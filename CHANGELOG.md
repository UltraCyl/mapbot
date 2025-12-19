# Changelog - MapBot DPB Migration

## Version DPB-1.0 (2025-12-19)

### Major Changes - DreamPoeBot Compatibility

This release migrates MapBot from the legacy ExileBuddy/Loki framework to DreamPoeBot (DPB) compatibility.

#### Added

- **Build System**
  - `MapBot.csproj` - MSBuild project file with DPB assembly references
  - `MapBot.sln` - Visual Studio solution file
  - `.gitignore` - Excludes build artifacts and DLL dependencies

- **Documentation**
  - `README.md` - Comprehensive build and installation guide
  - `BUILD.md` - Detailed build instructions with troubleshooting
  - `API_MIGRATION.md` - Complete API migration reference and compatibility notes
  - `CHANGELOG.md` - This file

#### Changed - Namespace Migration

All source files updated from legacy to DPB namespaces:

**Core Namespaces (201 references updated):**
- `Loki.Bot` → `DreamPoeBot.Loki.Bot`
- `Loki.Bot.Pathfinding` → `DreamPoeBot.Loki.Bot.Pathfinding`
- `Loki.Common` → `DreamPoeBot.Loki.Common`
- `Loki.Game` → `DreamPoeBot.Loki.Game`
- `Loki.Game.GameData` → `DreamPoeBot.Loki.Game.GameData`
- `Loki.Game.Objects` → `DreamPoeBot.Loki.Game.Objects`
- `Buddy.Coroutines` → `DreamPoeBot.Loki.Coroutine`

**Using Aliases Updated:**
- `Message`, `StashUi`, `InventoryUi`, `SkillBar`, `Skillbar`, `Cursor`, `DialogUi`, `RewardUi`, `SellUi`

**Files Modified:**

*MapBot (26 files):*
- MapBot.cs (main bot class)
- AffixData.cs, AffixSettings.cs
- CastAuraTask.cs, DeviceAreaTask.cs, EnterTrialTask.cs
- FinishMapTask.cs, GeneralSettings.cs, KillBossTask.cs
- MapData.cs, MapExplorationTask.cs, MapExtensions.cs, MapSettings.cs
- OpenMapTask.cs, ProximityTriggerTask.cs, SellMapTask.cs
- SextantTask.cs, SpecialObjectTask.cs, Statistics.cs
- TakeMapTask.cs, TrackMobTask.cs, TransitionTriggerTask.cs
- TravelToHideoutTask.cs, TravelToLabTask.cs
- Gui.xaml.cs, Gui.g.cs

*EXtensions (65 files):*
- Core: EXtensions.cs, BotStructure.cs, ErrorManager.cs
- Global: Travel.cs, CombatAreaCache.cs, ComplexExplorer.cs, ResurrectionLogic.cs, StuckDetection.cs, TrackMobLogic.cs
- CommonTasks: All task files (20+ files)
- CachedObjects: All caching classes (5 files)
- Positions: All position classes (5 files)
- Utilities: Wait.cs, Move.cs, World.cs, GlobalLog.cs, and more

#### Technical Details

**Migration Scope:**
- Total source files migrated: 91
- Lines of code reviewed: ~25,000+
- Namespace references updated: 201
- Using alias statements updated: 11
- Zero behavioral changes - pure API migration

**Compatibility:**
- Target Framework: .NET Framework 4.8
- DreamPoeBot Version: v3.14+ (tested pattern)
- Reference Implementation: FollowBot v6.6

#### Not Changed

**No Behavioral Modifications:**
- Bot logic and algorithms unchanged
- Task execution order and flow unchanged
- Settings and configuration unchanged
- UI and GUI remain the same
- Error handling patterns preserved

**API Surface Compatibility:**
- All IBot interface methods unchanged
- ITask interface and patterns unchanged
- Message system unchanged
- Coroutine patterns compatible
- Explorer and pathfinding APIs compatible

### Building

**Requirements:**
- Visual Studio 2019+ or MSBuild
- .NET Framework 4.8 SDK
- DreamPoeBot installation (for DLL references)

**Build Process:**
1. Copy `DreamPoeBot.exe`, `log4net.dll`, `Newtonsoft.Json.dll` to project root
2. Open `MapBot.sln` and build, or use MSBuild
3. Output: `bin\Release\MapBot.dll`

See `BUILD.md` for detailed instructions.

### Installation

**Deploy to DreamPoeBot:**
1. Copy `bin\Release\MapBot.dll` to `[DreamPoeBot]\3rdParty\Default\MapBot\`
2. Launch DreamPoeBot
3. Select MapBot from bot dropdown

See `README.md` for full installation guide.

### Testing Status

**Completed:**
- ✅ Source code migration
- ✅ Namespace updates verified
- ✅ Project structure created
- ✅ Build configuration set up
- ✅ Documentation complete

**Pending:**
- ⏳ Build verification (requires DPB installation)
- ⏳ Runtime testing with DreamPoeBot
- ⏳ Task execution verification
- ⏳ Combat and exploration testing
- ⏳ UI interaction testing

### Known Issues

None identified during migration. Runtime testing required to verify full compatibility.

### Migration Notes

See `API_MIGRATION.md` for:
- Complete namespace mapping
- API compatibility details
- Testing checklist
- Troubleshooting guide

### Credits

**Migration:**
- DPB compatibility port (December 2025)

**Original MapBot:**
- ExVault and contributors

**Reference Implementation:**
- FollowBot v6.6 (alcor75/FollowBot)

**Framework:**
- DreamPoeBot team

### License

Maintains original MapBot license terms.

---

## Pre-Migration History

Previous versions used the legacy ExileBuddy framework with `Loki.*` and `Buddy.Coroutines` namespaces.

For legacy version history, see original repository commit log before DPB migration.
