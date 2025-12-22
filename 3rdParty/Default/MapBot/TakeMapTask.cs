using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Default.EXtensions;
using Default.EXtensions.CachedObjects;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;
using InventoryUi = DreamPoeBot.Loki.Game.LokiPoe.InGameState.InventoryUi;
using StashUi = DreamPoeBot.Loki.Game.LokiPoe.InGameState.StashUi;
using ExSettings = Default.EXtensions.Settings;

namespace Default.MapBot
{
    public class TakeMapTask : ITask
    {
        private static readonly GeneralSettings Settings = GeneralSettings.Instance;
        private static readonly Dictionary<string, bool> AvailableCurrency = new Dictionary<string, bool>
        {
            [CurrencyNames.Transmutation] = true,
            [CurrencyNames.Augmentation] = true,
            [CurrencyNames.Alteration] = true,
            [CurrencyNames.Alchemy] = true,
            [CurrencyNames.Chaos] = true,
            [CurrencyNames.Scouring] = true,
            [CurrencyNames.Chisel] = true,
            [CurrencyNames.Vaal] = true
        };
        private static readonly Dictionary<string, int> AmountForAvailable = new Dictionary<string, int>
            [CurrencyNames.Transmutation] = 1,
            [CurrencyNames.Augmentation] = 5,
            [CurrencyNames.Alteration] = 5,
            [CurrencyNames.Alchemy] = 5,
            [CurrencyNames.Chaos] = 5,
            [CurrencyNames.Scouring] = 5,
            [CurrencyNames.Chisel] = 4,
            [CurrencyNames.Vaal] = 1
        private static bool _hasFragments = true;
        public async Task<bool> Run()
            if (MapBot.IsOnRun)
                return false;
            var area = World.CurrentArea;
            if (!area.IsTown && !area.IsHideoutArea)
            if (Settings.StopRequested)
            {
                GlobalLog.Warn("Stopping the bot by a user's request (stop after current map)");
                Settings.StopRequested = false;
                BotManager.Stop();
                return true;
            }
            var mapTabs = ExSettings.Instance.GetTabsForCategory(ExSettings.StashingCategory.Map);
            Item map;
            foreach (var tab in mapTabs)
                if (!await Inventories.OpenStashTab(tab))
                {
                    ErrorManager.ReportError();
                    return true;
                }
                if (StashUi.StashTabInfo.IsPremiumMap)
                    GlobalLog.Error("Map stash tab is unsupported and there are no plans to support it in the future. Please remove it from stashing settings.");
                    BotManager.Stop();
                if ((map = FindProperMap()) != null)
                    goto hasProperMap;
                GlobalLog.Debug($"[TakeMapTask] Fail to find a proper map in \"{tab}\" tab.");
            GlobalLog.Error("[TakeMapTask] Fail to find a proper map in all map tabs. Now stopping the bot because it cannot continue.");
            BotManager.Stop();
            return true;
            hasProperMap:
            GlobalLog.Info($"[TakeMapTask] Map of choice is \"{map.Name}\" (Tier: {map.MapTier})");
            if (!await Inventories.FastMoveFromStashTab(map.LocationTopLeft))
                ErrorManager.ReportError();
            if (!await Wait.For(() => (map = Inventories.InventoryItems.Find(i => i.IsMap())) != null, "map appear in inventory"))
                GlobalLog.Error("[TakeMapTask] Unexpected error. Map did not appear in player's inventory after fast move from stash.");
            var mapPos = map.LocationTopLeft;
            var mapRarity = map.RarityLite();
            if (mapRarity == Rarity.Unique || !map.IsIdentified || map.IsMirrored || map.IsCorrupted)
                ChooseMap(mapPos);
            switch (mapRarity)
                case Rarity.Normal:
                    if (!await HandleNormalMap(mapPos)) return true;
                    break;
                case Rarity.Magic:
                    if (!await HandleMagicMap(mapPos)) return true;
                case Rarity.Rare:
                    if (!await HandleRareMap(mapPos)) return true;
                default:
                    GlobalLog.Error($"[TakeMapTask] Unknown map rarity: \"{mapRarity}\".");
                    ErrorManager.ReportCriticalError();
            UpdateMapReference(mapPos, ref map);
            if (map.ShouldUpgrade(Settings.VaalUpgrade) && HasCurrency(CurrencyNames.Vaal))
                if (!await CorruptMap(mapPos))
                UpdateMapReference(mapPos, ref map);
            if (map.ShouldUpgrade(Settings.FragmentUpgrade) && _hasFragments)
                await GetFragment();
            ChooseMap(mapPos);
            return false;
        }
        private static Item FindProperMap()
            var maps = new List<Item>();
            foreach (var map in Inventories.StashTabItems.Where(i => i.IsMap()))
                if (map.Ignored())
                    continue;
                var rarity = map.RarityLite();
                if (rarity == Rarity.Unique)
                    maps.Add(map);
                if (!map.BelowTierLimit())
                if (rarity == Rarity.Rare && Settings.ExistingRares == ExistingRares.NoRun && NoRareUpgrade(map))
                if (!Settings.RunUnId && !map.IsIdentified)
                if (map.HasBannedAffix())
                    if (map.IsCorrupted || map.IsMirrored)
                        continue;
                    if (rarity == Rarity.Magic && !HasMagicOrbs)
                    if (rarity == Rarity.Rare)
                    {
                        if (NoRareUpgrade(map))
                        {
                            if (Settings.ExistingRares == ExistingRares.NoReroll)
                                continue;
                            if (Settings.ExistingRares == ExistingRares.Downgrade)
                            {
                                if (HasScourTransmute) maps.Add(map);
                            }
                        }
                        if (!HasRareOrbs)
                            continue;
                    }
                maps.Add(map);
            if (maps.Count == 0)
                return null;
            var sortedMaps = maps
                .OrderByDescending(m => m.Priority())
                .ThenByDescending(m => m.MapTier)
                .ThenByDescending(m => m.RarityLite())
                .ThenByDescending(m => m.Quality)
                .ToList();
            var unique = sortedMaps.Find(m => m.RarityLite() == Rarity.Unique);
            if (unique != null)
                return unique;
            if (Settings.RunUnId)
                var unId = sortedMaps.Find(m => !m.IsIdentified);
                if (unId != null)
                    return unId;
            return sortedMaps[0];
        private static async Task<bool> HandleNormalMap(Vector2i mapPos)
            var map = InventoryUi.InventoryControl_Main.Inventory.FindItemByPos(mapPos);
            if (map == null)
                GlobalLog.Error($"[HandleNormalMap] Fail to find a map at {mapPos}.");
            if (map.ShouldUpgrade(Settings.ChiselUpgrade) && HasCurrency(CurrencyNames.Chisel))
                if (!await ApplyChisels(mapPos))
                    return false;
            if (map.ShouldUpgrade(Settings.RareUpgrade) && HasRareOrbs)
                if (!await ApplyOrb(mapPos, CurrencyNames.Alchemy))
                return await RerollRare(mapPos);
            if (map.ShouldUpgrade(Settings.MagicUpgrade) && HasMagicOrbs)
                if (!await ApplyOrb(mapPos, CurrencyNames.Transmutation))
                return await RerollMagic(mapPos);
        private static async Task<bool> HandleMagicMap(Vector2i mapPos)
                GlobalLog.Error($"[HandleMagicMap] Fail to find map at {mapPos}.");
            if (map.ShouldUpgrade(Settings.MagicRareUpgrade) && HasMagicToRareOrbs)
                if (!await ApplyOrb(mapPos, CurrencyNames.Scouring))
            return await RerollMagic(mapPos);
        private static async Task<bool> HandleRareMap(Vector2i mapPos)
                GlobalLog.Error($"[HandleRareMap] Fail to find map at {mapPos}.");
            if (Settings.ExistingRares == ExistingRares.Downgrade && map.HasBannedAffix() && NoRareUpgrade(map) && HasScourTransmute)
            return await RerollRare(mapPos);
        public static async Task<bool> RerollMagic(Vector2i mapPos)
            while (true)
                var map = InventoryUi.InventoryControl_Main.Inventory.FindItemByPos(mapPos);
                if (map == null)
                    GlobalLog.Error($"[RerollMagic] Fail to find a map at {mapPos}.");
                if (rarity != Rarity.Magic)
                    GlobalLog.Error($"[TakeMapTask] RerollMagic is called on {rarity} map.");
                var affix = map.GetBannedAffix();
                if (affix != null)
                    GlobalLog.Info($"[RerollMagic] Rerolling banned \"{affix}\" affix.");
                    if (!await ApplyOrb(mapPos, CurrencyNames.Alteration))
                        return false;
                if (map.CanAugment() && HasCurrency(CurrencyNames.Augmentation))
                    if (!await ApplyOrb(mapPos, CurrencyNames.Augmentation))
        public static async Task<bool> RerollRare(Vector2i mapPos)
                    GlobalLog.Error($"[RerollRare] Fail to find a map at {mapPos}.");
                if (rarity != Rarity.Rare)
                    GlobalLog.Error($"[TakeMapTask] RerollRare is called on {rarity} map.");
                if (affix == null)
                GlobalLog.Info($"[RerollRare] Rerolling banned \"{affix}\" affix.");
                if (Settings.RerollMethod == RareReroll.Chaos)
                    if (!await ApplyOrb(mapPos, CurrencyNames.Chaos))
                else
                    if (!await ApplyOrb(mapPos, CurrencyNames.Scouring))
                    if (!await ApplyOrb(mapPos, CurrencyNames.Alchemy))
        public static async Task<bool> ApplyChisels(Vector2i mapPos)
                    GlobalLog.Error($"[ApplyChisels] Fail to find a map at {mapPos}.");
                if (map.Quality >= 18)
                if (!await ApplyOrb(mapPos, CurrencyNames.Chisel))
        private static async Task<bool> CorruptMap(Vector2i mapPos)
            if (!await ApplyOrb(mapPos, CurrencyNames.Vaal))
            if (!map.IsIdentified)
                GlobalLog.Warn("[CorruptMap] Unidentified corrupted map retains it's original affixes. We are good to go.");
            if (map.Ignored())
                GlobalLog.Warn("[CorruptMap] Map has been changed to another. New one is ignored in settings.");
            if (!map.BelowTierLimit())
                GlobalLog.Warn("[CorruptMap] Map tier has been increased beyond tier limit in settings.");
            var affix = map.GetBannedAffix();
            if (affix != null)
                GlobalLog.Warn($"[CorruptMap] Banned \"{affix}\" has been spawned.");
            GlobalLog.Warn("[CorruptMap] Resulting corrupted map fits all requirements. We are good to go.");
        private static async Task<bool> ApplyOrb(Vector2i targetPos, string orbName)
            if (!await Inventories.FindTabWithCurrency(orbName))
                GlobalLog.Warn($"[TakeMapTask] There are no \"{orbName}\" in all tabs assigned to them. Now marking this currency as unavailable.");
                AvailableCurrency[orbName] = false;
            if (StashUi.StashTabInfo.IsPremiumCurrency)
                var control = Inventories.GetControlWithCurrency(orbName);
                if (!await control.PickItemToCursor(true))
            else
                var orb = Inventories.StashTabItems.Find(i => i.Name == orbName);
                if (!await StashUi.InventoryControl.PickItemToCursor(orb.LocationTopLeft, true))
            if (!await InventoryUi.InventoryControl_Main.PlaceItemFromCursor(targetPos))
        private static async Task GetFragment()
            var tabs = new List<string>(ExSettings.Instance.GetTabsForCategory(ExSettings.StashingCategory.Fragment));
            if (tabs.Count > 1 && StashUi.IsOpened)
                var currentTab = StashUi.StashTabInfo.DisplayName;
                var index = tabs.IndexOf(currentTab);
                if (index > 0)
                    var tab = tabs[index];
                    tabs.RemoveAt(index);
                    tabs.Insert(0, tab);
            foreach (var tab in tabs)
                GlobalLog.Debug($"[TakeMapTask] Looking for Sacrifice Fragment in \"{tab}\" tab.");
                    return;
                if (StashUi.StashTabInfo.IsPremiumSpecial)
                    var tabType = StashUi.StashTabInfo.TabType;
                    if (tabType == InventoryTabType.Fragment)
                        foreach (var control in SacrificeControls)
                            var fragment = control.CustomTabItem;
                            if (fragment != null)
                                GlobalLog.Debug($"[TakeMapTask] Found \"{fragment.Name}\" in \"{tab}\" tab.");
                                await Inventories.FastMoveFromPremiumStashTab(control);
                                return;
                            GlobalLog.Debug($"[TakeMapTask] There are no Sacrifice Fragments in \"{tab}\" tab.");
                    else
                        GlobalLog.Error($"[TakeMapTask] Incorrect tab type ({tabType}) for sacrifice fragments.");
                    var fragment = Inventories.StashTabItems
                        .Where(i => i.IsSacrificeFragment())
                        .OrderBy(i => i.Name == "Sacrifice at Midnight") // move midnights to the end of the list
                        .FirstOrDefault();
                    if (fragment != null)
                        GlobalLog.Debug($"[TakeMapTask] Found \"{fragment.Name}\" in \"{tab}\" tab.");
                        await Inventories.FastMoveFromStashTab(fragment.LocationTopLeft);
                        return;
                    GlobalLog.Debug($"[TakeMapTask] There are no Sacrifice Fragments in \"{tab}\" tab.");
            GlobalLog.Info("[TakeMapTask] There are no Sacrifice Fragments in all tabs assigned to them. Now marking them as unavailable.");
            _hasFragments = false;
        private static void ChooseMap(Vector2i mapPos)
            OpenMapTask.Enabled = true;
            GlobalLog.Warn($"[TakeMapTask] Now going to \"{map.FullName}\".");
        // ReSharper disable once RedundantAssignment
        private static void UpdateMapReference(Vector2i mapPos, ref Item map)
            map = InventoryUi.InventoryControl_Main.Inventory.FindItemByPos(mapPos);
        private static bool NoRareUpgrade(Item map)
            return !map.ShouldUpgrade(Settings.RareUpgrade) && !map.ShouldUpgrade(Settings.MagicRareUpgrade);
        private static bool HasCurrency(string name)
            if (AvailableCurrency[name])
            GlobalLog.Debug($"[TakeMapTask] HasCurrency is false for {name}.");
        private static bool HasMagicOrbs
            get
                return HasCurrency(CurrencyNames.Alteration) &&
                       HasCurrency(CurrencyNames.Augmentation) &&
                       HasCurrency(CurrencyNames.Transmutation);
        private static bool HasRareOrbs
                if (Settings.RerollMethod == RareReroll.ScourAlch)
                    return HasScourAlchemy;
                return HasCurrency(CurrencyNames.Alchemy) &&
                       HasCurrency(CurrencyNames.Chaos);
        private static bool HasMagicToRareOrbs
                return HasScourAlchemy && HasCurrency(CurrencyNames.Chaos);
        private static bool HasScourAlchemy
                return HasCurrency(CurrencyNames.Scouring) &&
                       HasCurrency(CurrencyNames.Alchemy);
        private static bool HasScourTransmute
        private static IEnumerable<InventoryControlWrapper> SacrificeControls => new[]
            StashUi.FragmentTab.SacrificeAtDusk,
            StashUi.FragmentTab.SacrificeAtDawn,
            StashUi.FragmentTab.SacrificeAtNoon,
            StashUi.FragmentTab.SacrificeAtMidnight
        private static void UpdateAvailableCurrency(string currencyName)
            if (!AvailableCurrency.TryGetValue(currencyName, out bool available))
                return;
            if (available)
            var amount = Inventories.GetCurrencyAmountInStashTab(currencyName);
            if (amount >= AmountForAvailable[currencyName])
                GlobalLog.Info($"[TakeMapTask] There are {amount} \"{currencyName}\" in current stash tab. Now marking this currency as available.");
                AvailableCurrency[currencyName] = true;
        private static void UpdateAvailableFragments()
            if (StashUi.StashTabInfo.IsPremiumFragment)
                if (SacrificeControls.Any(c => c.CustomTabItem != null))
                    GlobalLog.Info("[TakeMapTask] Sacrifice Fragment has been stashed. Now marking it as available.");
                    _hasFragments = true;
                if (Inventories.StashTabItems.Any(i => i.IsSacrificeFragment()))
        public MessageResult Message(Message message)
            if (message.Id == Events.Messages.ItemStashedEvent)
                var item = message.GetInput<CachedItem>();
                var itemType = item.Type.ItemType;
                if (itemType == ItemTypes.Currency)
                    UpdateAvailableCurrency(item.Name);
                else if (itemType == ItemTypes.MapFragment && !_hasFragments)
                    UpdateAvailableFragments();
                return MessageResult.Processed;
            return MessageResult.Unprocessed;
        // Reset every Start in case user added something manually
        public void Start()
            foreach (var key in AvailableCurrency.Keys.ToList())
                AvailableCurrency[key] = true;
            _hasFragments = true;
        #region Unused interface methods
        public async Task<LogicResult> Logic(Logic logic)
            return LogicResult.Unprovided;
        public void Tick()
        public void Stop()
        public string Name => "TakeMapTask";
        public string Author => "ExVault";
        public string Description => "Task for taking maps from the stash.";
        public string Version => "1.0";
        #endregion
    }
}
