using System.Linq;
using System.Threading.Tasks;
using Default.EXtensions;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;

namespace Default.MapBot
{
    public class OpenMapTask : ITask
    {
        internal static bool Enabled;
        public async Task<bool> Run()
        {
            if (!Enabled)
                return false;
            var area = World.CurrentArea;
            // This is more complicated after 3.0 because GGG added stairs to laboratory
            //if (!area.IsHideoutArea && !area.IsMapRoom)
            //    return false;
            if (area.IsHideoutArea)
                goto inProperArea;
            if (area.IsMapRoom)
            {
                if (await DeviceAreaTask.HandleStairs(true))
                    return true;
            }
            return false;
            inProperArea:
            var map = Inventories.InventoryItems.Find(i => i.IsMap());
            if (map == null)
                GlobalLog.Error("[OpenMapTask] There is no map in inventory.");
                Enabled = false;
                return true;
            var mapPos = map.LocationTopLeft;
            if (!await PlayerAction.TryTo(OpenDevice, "Open Map Device", 3, 2000))
                ErrorManager.ReportError();
            if (!await ClearDevice())
            if (!await PlayerAction.TryTo(() => PlaceIntoDevice(mapPos), "Place map into device", 3))
            var fragment = Inventories.InventoryItems.Find(i => i.IsSacrificeFragment());
            if (fragment != null)
                await PlayerAction.TryTo(() => PlaceIntoDevice(fragment.LocationTopLeft), "Place vaal fragment into device", 3);
            var portal = LokiPoe.ObjectManager.Objects.Closest<Portal>();
            if (portal == null)
                GlobalLog.Error("[OpenMapTask] Unknown error. Fail to find any portal near map device.");
            var isTargetable = portal.IsTargetable;
            if (!await PlayerAction.TryTo(ActivateDevice, "Activate Map Device", 3))
            if (isTargetable)
                if (!await Wait.For(() => !portal.Fresh().IsTargetable, "old map portals despawning", 200, 10000))
                {
                    ErrorManager.ReportError();
                }
            if (!await Wait.For(() =>
                    var p = portal.Fresh();
                    return p.IsTargetable && p.LeadsTo(a => a.IsMap);
                },
                "new map portals spawning", 500, 15000))
            await Wait.SleepSafe(500);
            if (!await TakeMapPortal(portal))
            return true;
        }
        private static async Task<bool> OpenDevice()
            if (MapDevice.IsOpen) return true;
            var device = LokiPoe.ObjectManager.MapDevice;
            if (device == null)
                if (World.CurrentArea.IsHideoutArea)
                    GlobalLog.Error("[OpenMapTask] Fail to find Map Device in hideout.");
                else
                    GlobalLog.Error("[OpenMapTask] Unknown error. Fail to find Map Device in Templar Laboratory.");
                GlobalLog.Error("[OpenMapTask] Now stopping the bot because it cannot continue.");
                BotManager.Stop();
            GlobalLog.Debug("[OpenMapTask] Now going to open Map Device.");
            await device.WalkablePosition().ComeAtOnce();
            if (await PlayerAction.Interact(device, () => MapDevice.IsOpen, "Map Device opening"))
                GlobalLog.Debug("[OpenMapTask] Map Device has been successfully opened.");
            GlobalLog.Debug("[OpenMapTask] Fail to open Map Device.");
        private static async Task<bool> ClearDevice()
            var itemPositions = MapDevice.InventoryControl.Inventory.Items.Select(i => i.LocationTopLeft).ToList();
            if (itemPositions.Count == 0)
            GlobalLog.Error("[OpenMapTask] Map Device is not empty. Now going to clean it.");
            foreach (var itemPos in itemPositions)
                if (!await PlayerAction.TryTo(() => FastMoveFromDevice(itemPos), null, 2))
                    return false;
            GlobalLog.Debug("[OpenMapTask] Map Device has been successfully cleaned.");
        private static async Task<bool> PlaceIntoDevice(Vector2i itemPos)
            var oldCount = MapDevice.InventoryControl.Inventory.Items.Count;
            if (!await Inventories.FastMoveFromInventory(itemPos))
            if (!await Wait.For(() => MapDevice.InventoryControl.Inventory.Items.Count == oldCount + 1, "item amount change in Map Device"))
        private static async Task<bool> ActivateDevice()
            GlobalLog.Debug("[OpenMapTask] Now going to activate the Map Device.");
            await Wait.SleepSafe(500); // Additional delay to ensure Activate button is targetable
            var map = MapDevice.InventoryControl.Inventory.Items.Find(i=> i.Class == ItemClasses.Map);
                GlobalLog.Error("[OpenMapTask] Unexpected error. There is no map in the Map Device.");
            LokiPoe.InGameState.ActivateResult activated;
            if (World.CurrentArea.IsHideoutArea)
                if (MapSettings.Instance.MapDict.TryGetValue(map.CleanName(), out var data) && data.ZanaMod > 0)
                    var modIndex = data.ZanaMod;
                    var deviceOptions = LokiPoe.InGameState.MasterDeviceUi.Options;
                    var maxIndex = deviceOptions.Count - 1;
                    if (modIndex > maxIndex)
                    {
                        GlobalLog.Error($"[OpenMapTask] Invalid Zana mod index {modIndex}. Map Device has only {maxIndex} indexes. Please correct your map settings.");
                        activated = LokiPoe.InGameState.MasterDeviceUi.Activate();
                    }
                    else if (deviceOptions[modIndex].Item3.Contains("<red>"))
                        GlobalLog.Warn("[OpenMapTask] Not enough currency to pay for Zana mod activation.");
                    else
                        GlobalLog.Warn($"[OpenMapTask] Opening {map.Name} with {deviceOptions[modIndex].Item1} mod.");
                        activated = LokiPoe.InGameState.MasterDeviceUi.ActivateWithOption(modIndex);
                    activated = LokiPoe.InGameState.MasterDeviceUi.Activate();
            else
                activated = LokiPoe.InGameState.MapDeviceUi.Activate();
            if (activated != LokiPoe.InGameState.ActivateResult.None)
                GlobalLog.Error($"[OpenMapTask] Fail to activate the Map Device. Error: \"{activated}\".");
            if (await Wait.For(() => !MapDevice.IsOpen, "Map Device closing"))
                GlobalLog.Debug("[OpenMapTask] Map Device has been successfully activated.");
            GlobalLog.Error("[OpenMapTask] Fail to activate the Map Device.");
        private static async Task<bool> FastMoveFromDevice(Vector2i itemPos)
            var item = MapDevice.InventoryControl.Inventory.FindItemByPos(itemPos);
            if (item == null)
                GlobalLog.Error($"[FastMoveFromDevice] Fail to find item at {itemPos} in Map Device.");
            var itemName = item.FullName;
            GlobalLog.Debug($"[FastMoveFromDevice] Fast moving \"{itemName}\" at {itemPos} from Map Device.");
            var moved = MapDevice.InventoryControl.FastMove(item.LocalId);
            if (moved != FastMoveResult.None)
                GlobalLog.Error($"[FastMoveFromDevice] Fast move error: \"{moved}\".");
            if (await Wait.For(() => MapDevice.InventoryControl.Inventory.FindItemByPos(itemPos) == null, "fast move"))
                GlobalLog.Debug($"[FastMoveFromDevice] \"{itemName}\" at {itemPos} has been successfully fast moved from Map Device.");
            GlobalLog.Error($"[FastMoveFromDevice] Fast move timeout for \"{itemName}\" at {itemPos} in Map Device.");
        private static async Task<bool> TakeMapPortal(Portal portal, int attempts = 3)
            for (int i = 1; i <= attempts; ++i)
                if (!LokiPoe.IsInGame || World.CurrentArea.IsMap)
                GlobalLog.Debug($"[OpenMapTask] Take portal to map attempt: {i}/{attempts}");
                if (await PlayerAction.TakePortal(portal))
                await Wait.SleepSafe(1000);
        public MessageResult Message(Message message)
            if (message.Id == MapBot.Messages.NewMapEntered)
                return MessageResult.Processed;
            return MessageResult.Unprocessed;
        private static class MapDevice
            public static bool IsOpen => World.CurrentArea.IsHideoutArea
                ? LokiPoe.InGameState.MasterDeviceUi.IsOpened
                : LokiPoe.InGameState.MapDeviceUi.IsOpened;
            public static InventoryControlWrapper InventoryControl => World.CurrentArea.IsHideoutArea
                ? LokiPoe.InGameState.MasterDeviceUi.InventoryControl
                : LokiPoe.InGameState.MapDeviceUi.InventoryControl;
        #region Unused interface methods
        public async Task<LogicResult> Logic(Logic logic)
            return LogicResult.Unprovided;
        public void Tick()
        public void Start()
        public void Stop()
        public string Name => "OpenMapTask";
        public string Description => "Task for opening maps via Map Device.";
        public string Author => "ExVault";
        public string Version => "1.0";
        #endregion
    }
}
