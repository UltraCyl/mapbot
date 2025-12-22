using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Default.EXtensions.Positions;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;
using Cursor = DreamPoeBot.Loki.Game.LokiPoe.InGameState.CursorItemOverlay;
using InventoryUi = DreamPoeBot.Loki.Game.LokiPoe.InGameState.InventoryUi;
using StashUi = DreamPoeBot.Loki.Game.LokiPoe.InGameState.StashUi;

namespace Default.EXtensions
{
    public static class Inventories
    {
        public static List<Item> InventoryItems => LokiPoe.InstanceInfo.GetPlayerInventoryItemsBySlot(InventorySlot.Main);
        public static List<Item> StashTabItems => StashUi.InventoryControl.Inventory.Items;
        public static int AvailableInventorySquares => LokiPoe.InstanceInfo.GetPlayerInventoryBySlot(InventorySlot.Main).AvailableInventorySquares;
        public static async Task<bool> OpenStash()
        {
            if (StashUi.IsOpened)
                return true;
            WalkablePosition stashPos;
            if (World.CurrentArea.IsTown)
            {
                stashPos = StaticPositions.GetStashPosByAct();
            }
            else
                var stashObj = LokiPoe.ObjectManager.Stash;
                if (stashObj == null)
                {
                    GlobalLog.Error("[OpenStash] Fail to find any Stash nearby.");
                    return false;
                }
                stashPos = stashObj.WalkablePosition();
            await PlayerAction.EnableAlwaysHighlight();
            await stashPos.ComeAtOnce();
            if (!await PlayerAction.Interact(LokiPoe.ObjectManager.Stash, () => StashUi.IsOpened && StashUi.StashTabInfo != null, "stash opening"))
                return false;
            await Wait.SleepSafe(200);
            return true;
        }
        public static async Task<bool> OpenStashTab(string tabName)
            if (!await OpenStash())
            if (StashUi.TabControl.CurrentTabName != tabName)
                GlobalLog.Debug($"[OpenStashTab] Now switching to tab \"{tabName}\".");
                var id = StashUi.StashTabInfo.InventoryId;
                var err = StashUi.TabControl.SwitchToTabMouse(tabName);
                if (err != SwitchToTabResult.None)
                    GlobalLog.Error($"[OpenStashTab] Fail to switch to tab \"{tabName}\". Error \"{err}\".");
                if (!await Wait.For(() => StashUi.StashTabInfo != null && StashUi.StashTabInfo.InventoryId != id, "stash tab switching"))
                await Wait.SleepSafe(200);
        public static async Task<bool> OpenInventory()
            if (InventoryUi.IsOpened && !LokiPoe.InGameState.PurchaseUi.IsOpened && !LokiPoe.InGameState.SellUi.IsOpened)
            await Coroutines.CloseBlockingWindows();
            LokiPoe.Input.SimulateKeyEvent(LokiPoe.Input.Binding.open_inventory_panel, true, false, false);
            if (!await Wait.For(() => InventoryUi.IsOpened, "inventory panel opening"))
            if (Settings.Instance.ArtificialDelays)
                await Wait.ArtificialDelay();
        public static async Task<bool> OpenLegacyUi()
            if (!LokiPoe.InGameState.ChallengesUi.IsOpened)
                if (!await OpenChallenges())
            if (LokiPoe.InGameState.ChallengesUi.IsLegacyTabSelected)
            var err = LokiPoe.InGameState.ChallengesUi.SwitchToLegacyTab();
            if (err != SwitchToTabResult.None)
                GlobalLog.Error($"[OpenLegacyUi] Switch to legacy tab error: \"{err}\".");
            if (!await Wait.For(() => LokiPoe.InGameState.ChallengesUi.IsLegacyTabSelected, "switching to legacy tab"))
        public static async Task<bool> OpenProphecyUi()
            if (LokiPoe.InGameState.ChallengesUi.IsPropheciesTabSelected)
            var err = LokiPoe.InGameState.ChallengesUi.SwitchToPropheciesTab();
                GlobalLog.Error($"[OpenProphecyUi] Switch to prophecy tab error: \"{err}\".");
            if (!await Wait.For(() => LokiPoe.InGameState.ChallengesUi.IsPropheciesTabSelected, "switching to prophecy tab"))
        public static IEnumerable<Item> GetExcessCurrency(string name)
            var currency = InventoryItems.FindAll(c => c.Name == name);
            if (currency.Count <= 1)
                return Enumerable.Empty<Item>();
            return currency.OrderByDescending(c => c.StackCount).Skip(1).ToList();
        public static bool StashTabCanFitItem(Vector2i itemPos)
            var item = InventoryUi.InventoryControl_Main.Inventory.FindItemByPos(itemPos);
            if (item == null)
                GlobalLog.Error($"[StashCanFitItem] Fail to find item at {itemPos} in player's inventory.");
            var tabType = StashUi.StashTabInfo.TabType;
            if (tabType == InventoryTabType.Currency)
                var name = item.Name;
                var stackCount = item.StackCount;
                var control = GetCurrencyControl(name);
                if (control != null && control.CanFit(name, stackCount))
                    return true;
                return StashUi.CurrencyTab.NonCurrency.Any(miscControl => miscControl.CanFit(name, stackCount));
            if (tabType == InventoryTabType.Essence)
                var control = StashUi.EssenceTab.GetInventoryControlForMetadata(item.Metadata);
                return StashUi.EssenceTab.NonEssences.Any(miscControl => miscControl.CanFit(name, stackCount));
            if (tabType == InventoryTabType.Divination)
                var control = StashUi.DivinationTab.GetInventoryControlForMetadata(item.Metadata);
                return control != null && control.CanFit(item.Name, item.StackCount);
            if (tabType == InventoryTabType.Fragment)
                var control = StashUi.FragmentTab.GetInventoryControlForMetadata(item.Metadata);
            if (tabType == InventoryTabType.Map)
                GlobalLog.Error("[StashTabCanFitItem] Map tab is unsupported. Returning false.");
            return StashUi.InventoryControl.Inventory.CanFitItem(item.Size);
        public static int GetCurrencyAmountInStashTab(string currencyName)
            int total = 0;
                var control = GetCurrencyControl(currencyName);
                if (control != null)
                    var item = control.CustomTabItem;
                    if (item != null)
                        total += item.StackCount;
                foreach (var miscControl in StashUi.CurrencyTab.NonCurrency)
                    var item = miscControl.CustomTabItem;
                    if (item != null && item.Name == currencyName)
                return total;
                foreach (var miscControl in StashUi.EssenceTab.NonEssences)
            if (tabType == InventoryTabType.Divination ||
                tabType == InventoryTabType.Map ||
                tabType == InventoryTabType.Fragment)
                return 0;
            foreach (var item in StashTabItems)
                if (item.Name == currencyName)
                    total += item.StackCount;
            return total;
        public static async Task<WithdrawResult> WithdrawCurrency(string name)
            foreach (var tab in Settings.Instance.GetTabsForCurrency(name))
                GlobalLog.Debug($"[WithdrawCurrency] Looking for \"{name}\" in \"{tab}\" tab.");
                if (!await OpenStashTab(tab))
                    return WithdrawResult.Error;
                var tabType = StashUi.StashTabInfo.TabType;
                if (tabType == InventoryTabType.Currency)
                    var control = GetControlWithCurrency(name);
                    if (control == null)
                    {
                        GlobalLog.Debug($"[WithdrawCurrency] There are no \"{name}\" in \"{tab}\" tab.");
                        continue;
                    }
                    if (!await FastMoveFromPremiumStashTab(control))
                        return WithdrawResult.Error;
                    GlobalLog.Debug($"[WithdrawCurrency] \"{name}\" have been successfully taken from \"{tab}\" tab.");
                    return WithdrawResult.Success;
                if (tabType == InventoryTabType.Essence)
                    var control = StashUi.EssenceTab.NonEssences.FirstOrDefault(c => c.CustomTabItem?.Name == name);
                if (tabType == InventoryTabType.Divination ||
                    tabType == InventoryTabType.Map ||
                    tabType == InventoryTabType.Fragment)
                    GlobalLog.Error($"[WithdrawCurrency] Unsupported behavior. Current stash tab is {tabType}.");
                    continue;
                var item = StashTabItems.Where(i => i.Name == name).OrderByDescending(i => i.StackCount).FirstOrDefault();
                if (item == null)
                    GlobalLog.Debug($"[WithdrawCurrency] There are no \"{name}\" in \"{tab}\" tab.");
                if (!await FastMoveFromStashTab(item.LocationTopLeft))
                GlobalLog.Debug($"[WithdrawCurrency] \"{name}\" have been successfully taken from \"{tab}\" tab.");
                return WithdrawResult.Success;
            return WithdrawResult.Unavailable;
        public static async Task<bool> FindTabWithCurrency(string name)
            var tabs = new List<string>(Settings.Instance.GetTabsForCurrency(name));
            // Moving currently opened stash tab to the front of the list, so bot does search in it first
            if (tabs.Count > 1 && StashUi.IsOpened)
                var currentTab = StashUi.StashTabInfo.DisplayName;
                var index = tabs.IndexOf(currentTab);
                if (index > 0)
                    var tab = tabs[index];
                    tabs.RemoveAt(index);
                    tabs.Insert(0, tab);
            foreach (var tab in tabs)
                GlobalLog.Debug($"[FindTabWithCurrency] Looking for \"{name}\" in \"{tab}\" tab.");
                    ErrorManager.ReportError();
                if (StashUi.StashTabInfo.IsPublicFlagged)
                    GlobalLog.Error($"[FindTabWithCurrency] Stash tab \"{tab}\" is public. Cannot use currency from it.");
                var amount = GetCurrencyAmountInStashTab(name);
                if (amount > 0)
                    GlobalLog.Debug($"[FindTabWithCurrency] Found {amount} \"{name}\" in \"{tab}\" tab.");
                GlobalLog.Debug($"[FindTabWithCurrency] There are no \"{name}\" in \"{tab}\" tab.");
            return false;
        public static InventoryControlWrapper GetControlWithCurrency(string currencyName)
            var control = GetCurrencyControl(currencyName);
            if (control?.CustomTabItem != null)
                return control;
            return StashUi.CurrencyTab.NonCurrency.FirstOrDefault(c => c.CustomTabItem?.Name == currencyName);
        public static List<InventoryControlWrapper> GetControlsWithCurrency(string currencyName)
            var controls = new List<InventoryControlWrapper>();
                controls.Add(control);
            foreach (var miscControl in StashUi.CurrencyTab.NonCurrency)
                if (miscControl.CustomTabItem?.Name == currencyName)
                    controls.Add(miscControl);
            return controls;
        #region Fast moving
        public static async Task<bool> FastMoveFromInventory(Vector2i itemPos)
                GlobalLog.Error($"[FastMoveFromInventory] Fail to find item at {itemPos} in player's inventory.");
            var itemName = item.FullName;
            GlobalLog.Debug($"[FastMoveFromInventory] Fast moving \"{itemName}\" at {itemPos} from player's inventory.");
            var err = InventoryUi.InventoryControl_Main.FastMove(item.LocalId);
            if (err != FastMoveResult.None)
                GlobalLog.Error($"[FastMoveFromInventory] Fast move error: \"{err}\".");
            if (await Wait.For(() => InventoryUi.InventoryControl_Main.Inventory.FindItemByPos(itemPos) == null, "fast move"))
                GlobalLog.Debug($"[FastMoveFromInventory] \"{itemName}\" at {itemPos} has been successfully fast moved from player's inventory.");
                if (Settings.Instance.ArtificialDelays)
                    await Wait.ArtificialDelay();
            GlobalLog.Error($"[FastMoveFromInventory] Fast move timeout for \"{itemName}\" at {itemPos} in player's inventory.");
        public static async Task<bool> FastMoveToVendor(Vector2i itemPos)
                GlobalLog.Error($"[FastMoveToVendor] Fail to find item at {itemPos} in player's inventory.");
            GlobalLog.Debug($"[FastMoveToVendor] Fast moving \"{itemName}\" at {itemPos} from player's inventory.");
            if (err != FastMoveResult.None && err != FastMoveResult.ItemTransparent)
                GlobalLog.Error($"[FastMoveToVendor] Fast move error: \"{err}\".");
            if (await Wait.For(() =>
                var movedItem = InventoryUi.InventoryControl_Main.Inventory.FindItemByPos(itemPos);
                if (movedItem == null)
                    GlobalLog.Error("[FastMoveToVendor] Unexpected error. Item became null instead of transparent.");
                return InventoryUi.InventoryControl_Main.IsItemTransparent(movedItem.LocalId);
            }, "fast move"))
                GlobalLog.Debug($"[FastMoveToVendor] \"{itemName}\" at {itemPos} has been successfully fast moved from player's inventory.");
            GlobalLog.Error($"[FastMoveToVendor] Fast move timeout for \"{itemName}\" at {itemPos} in player's inventory.");
        public static async Task<bool> FastMoveFromStashTab(Vector2i itemPos)
            var tabName = StashUi.TabControl.CurrentTabName;
            var item = StashUi.InventoryControl.Inventory.FindItemByPos(itemPos);
                GlobalLog.Error($"[FastMoveFromStashTab] Fail to find item at {itemPos} in \"{tabName}\" tab.");
            var stackCount = item.StackCount;
            GlobalLog.Debug($"[FastMoveFromStashTab] Fast moving \"{itemName}\" at {itemPos} from \"{tabName}\" tab.");
            var err = StashUi.InventoryControl.FastMove(item.LocalId);
                GlobalLog.Error($"[FastMoveFromStashTab] Fast move error: \"{err}\".");
                var i = StashUi.InventoryControl.Inventory.FindItemByPos(itemPos);
                return i == null || i.StackCount < stackCount;
                GlobalLog.Debug($"[FastMoveFromStashTab] \"{itemName}\" at {itemPos} has been successfully fast moved from \"{tabName}\" tab.");
            GlobalLog.Error($"[FastMoveFromStashTab] Fast move timeout for \"{itemName}\" at {itemPos} in \"{tabName}\" tab.");
        public static async Task<bool> FastMoveFromPremiumStashTab(InventoryControlWrapper control)
            if (control == null)
                GlobalLog.Error("[FastMoveFromPremiumStashTab] Inventory control is null.");
            var item = control.CustomTabItem;
                GlobalLog.Error("[FastMoveFromPremiumStashTab] Inventory control has no item.");
            var itemName = item.Name;
            GlobalLog.Debug($"[FastMoveFromPremiumStashTab] Fast moving \"{itemName}\" from \"{tabName}\" tab.");
            var moved = control.FastMove();
            if (moved != FastMoveResult.None)
                GlobalLog.Error($"[FastMoveFromPremiumStashTab] Fast move error: \"{moved}\".");
                var i = control.CustomTabItem;
                GlobalLog.Debug($"[FastMoveFromPremiumStashTab] \"{itemName}\" has been successfully fast moved from \"{tabName}\" tab.");
            GlobalLog.Error($"[FastMoveFromPremiumStashTab] Fast move timeout for \"{itemName}\" in \"{tabName}\" tab.");
        #endregion
        #region Extension methods
        public static int ItemAmount(this Inventory inventory, string itemName)
            int amount = 0;
            foreach (var item in inventory.Items)
                if (item.Name == itemName)
                    amount += item.StackCount;
            return amount;
        public static async Task<bool> PickItemToCursor(this InventoryControlWrapper inventory, Vector2i itemPos, bool rightClick = false)
            var item = inventory.Inventory.FindItemByPos(itemPos);
                GlobalLog.Error($"[PickItemToCursor] Cannot find item at {itemPos}");
            GlobalLog.Debug($"[PickItemToCursor] Now going to pick \"{item.Name}\" at {itemPos} to cursor.");
            int id = item.LocalId;
            if (rightClick)
                var err = inventory.UseItem(id);
                if (err != UseItemResult.None)
                    GlobalLog.Error($"[PickItemToCursor] Fail to pick item to cursor. Error: \"{err}\".");
                var err = inventory.Pickup(id);
                if (err != PickupResult.None)
            return await Wait.For(() => Cursor.Item != null, "item appear under cursor");
        public static async Task<bool> PickItemToCursor(this InventoryControlWrapper inventory, bool rightClick = false)
            var item = inventory.CustomTabItem;
                GlobalLog.Error("[PickItemToCursor] Custom inventory control is empty.");
            GlobalLog.Debug($"[PickItemToCursor] Now going to pick \"{item.Name}\" to cursor.");
                var err = inventory.UseItem();
                var err = inventory.Pickup();
        public static async Task<bool> PlaceItemFromCursor(this InventoryControlWrapper inventory, Vector2i pos)
            var cursorItem = Cursor.Item;
            if (cursorItem == null)
                GlobalLog.Error("[PlaceItemFromCursor] Cursor item is null.");
            GlobalLog.Debug($"[PlaceItemFromCursor] Now going to place \"{cursorItem.Name}\" from cursor to {pos}.");
            //apply item on another item, if we are in VirtualUse mode
            if (Cursor.Mode == LokiPoe.InGameState.CursorItemModes.VirtualUse)
                var destItem = inventory.Inventory.FindItemByPos(pos);
                if (destItem == null)
                    GlobalLog.Error("[PlaceItemFromCursor] Destination item is null.");
                int destItemId = destItem.LocalId;
                var applied = inventory.ApplyCursorTo(destItem.LocalId);
                if (applied != ApplyCursorResult.None)
                    GlobalLog.Error($"[PlaceItemFromCursor] Fail to place item from cursor. Error: \"{applied}\".");
                //wait for destination item change, it cannot become null, ID should change
                return await Wait.For(() =>
                    var item = inventory.Inventory.FindItemByPos(pos);
                    return item != null && item.LocalId != destItemId;
                }, "destination item change");
            //in other cases, place item to empty inventory slot or swap it with another item
            int cursorItemId = cursorItem.LocalId;
            var placed = inventory.PlaceCursorInto(pos.X, pos.Y, true);
            if (placed != PlaceCursorIntoResult.None)
                GlobalLog.Error($"[PlaceItemFromCursor] Fail to place item from cursor. Error: \"{placed}\".");
            //wait for cursor item change, if we placed - it should become null, if we swapped - ID should change
            if (!await Wait.For(() =>
                var item = Cursor.Item;
                return item == null || item.LocalId != cursorItemId;
            }, "cursor item change")) return false;
        #region Helpers/private
        private static bool CanFit(this InventoryControlWrapper control, string itemName, int amount)
            if (itemName == CurrencyNames.Prophecy)
            return item.Name == itemName && item.StackCount + amount <= 5000;
        private static InventoryControlWrapper GetCurrencyControl(string currencyName)
            return CurrencyControlDict.TryGetValue(currencyName, out var getControl) ? getControl() : null;
        private static async Task<bool> OpenChallenges()
            LokiPoe.Input.SimulateKeyEvent(LokiPoe.Input.Binding.open_challenges_panel, true, false, false);
            if (!await Wait.For(() => LokiPoe.InGameState.ChallengesUi.IsOpened, "challenges panel opening"))
        private static readonly Dictionary<string, Func<InventoryControlWrapper>> CurrencyControlDict = new Dictionary<string, Func<InventoryControlWrapper>>
            [CurrencyNames.ScrollFragment] = () => StashUi.CurrencyTab.ScrollFragment,
            [CurrencyNames.TransmutationShard] = () => StashUi.CurrencyTab.TransmutationShard,
            [CurrencyNames.AlterationShard] = () => StashUi.CurrencyTab.AlterationShard,
            [CurrencyNames.AlchemyShard] = () => StashUi.CurrencyTab.AlchemyShard,
            [CurrencyNames.AnnulmentShard] = () => StashUi.CurrencyTab.AnnulmentShard,
            [CurrencyNames.ChaosShard] = () => StashUi.CurrencyTab.ChaosShard,
            [CurrencyNames.RegalShard] = () => StashUi.CurrencyTab.RegalShard,
            [CurrencyNames.ExaltedShard] = () => StashUi.CurrencyTab.ExaltedShard,
            [CurrencyNames.MirrorShard] = () => StashUi.CurrencyTab.MirrorShard,
            [CurrencyNames.Wisdom] = () => StashUi.CurrencyTab.ScrollOfWisdom,
            [CurrencyNames.Portal] = () => StashUi.CurrencyTab.PortalScroll,
            [CurrencyNames.Transmutation] = () => StashUi.CurrencyTab.OrbOfTransmutation,
            [CurrencyNames.Augmentation] = () => StashUi.CurrencyTab.OrbOfAugmentation,
            [CurrencyNames.Alteration] = () => StashUi.CurrencyTab.OrbOfAlteration,
            [CurrencyNames.Scrap] = () => StashUi.CurrencyTab.ArmourersScrap,
            [CurrencyNames.Whetstone] = () => StashUi.CurrencyTab.BlacksmithsWhetstone,
            [CurrencyNames.Glassblower] = () => StashUi.CurrencyTab.GlassblowersBauble,
            [CurrencyNames.Chisel] = () => StashUi.CurrencyTab.CartographersChisel,
            [CurrencyNames.Chromatic] = () => StashUi.CurrencyTab.ChromaticOrb,
            [CurrencyNames.Chance] = () => StashUi.CurrencyTab.OrbOfChance,
            [CurrencyNames.Alchemy] = () => StashUi.CurrencyTab.OrbOfAlchemy,
            [CurrencyNames.Jeweller] = () => StashUi.CurrencyTab.JewellersOrb,
            [CurrencyNames.Fusing] = () => StashUi.CurrencyTab.OrbOfFusing,
            [CurrencyNames.Scouring] = () => StashUi.CurrencyTab.OrbOfScouring,
            [CurrencyNames.Blessed] = () => StashUi.CurrencyTab.BlessedOrb,
            [CurrencyNames.Regal] = () => StashUi.CurrencyTab.RegalOrb,
            [CurrencyNames.Chaos] = () => StashUi.CurrencyTab.ChaosOrb,
            [CurrencyNames.Vaal] = () => StashUi.CurrencyTab.VaalOrb,
            [CurrencyNames.Regret] = () => StashUi.CurrencyTab.OrbOfRegret,
            [CurrencyNames.Gemcutter] = () => StashUi.CurrencyTab.GemcuttersPrism,
            [CurrencyNames.Divine] = () => StashUi.CurrencyTab.DivineOrb,
            [CurrencyNames.Exalted] = () => StashUi.CurrencyTab.ExaltedOrb,
            [CurrencyNames.Mirror] = () => StashUi.CurrencyTab.MirrorOfKalandra,
            [CurrencyNames.SilverCoin] = () => StashUi.CurrencyTab.SilverCoin,
            [CurrencyNames.SextantApprentice] = () => StashUi.CurrencyTab.ApprenticeCartographersSextant,
            [CurrencyNames.SextantJourneyman] = () => StashUi.CurrencyTab.JourneymanCartographersSextant,
            [CurrencyNames.SextantMaster] = () => StashUi.CurrencyTab.MasterCartographersSextant,
            [CurrencyNames.Annulment] = () => StashUi.CurrencyTab.OrbOfAnnulment,
        };
    }
}
