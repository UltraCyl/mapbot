using System.Linq;
using System.Threading.Tasks;
using Default.EXtensions.CachedObjects;
using Default.EXtensions.Global;
using Default.EXtensions.Positions;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Bot;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;
using DreamPoeBot.Loki.Game.GameData;
using DreamPoeBot.Loki.Game.Objects;

namespace Default.EXtensions.CommonTasks
{
    public class LootItemTask : ITask
    {
        private const int MaxItemPickupAttempts = 10;
        private static readonly Interval LogInterval = new Interval(1000);
        private CachedWorldItem _item;
        private bool _isInPreTownrunMode;
        public async Task<bool> Run()
        {
            if (!World.CurrentArea.IsCombatArea)
                return false;
            var items = CombatAreaCache.Current.Items;
            var validItems = items.FindAll(i => !i.Ignored && !i.Unwalkable);
            if (validItems.Count == 0)
            if (_item == null)
            {
                if (_isInPreTownrunMode)
                {
                    var squares = Inventories.AvailableInventorySquares;
                    _item = validItems
                        .Where(i => i.Position.Distance <= 40 && CanFit(i.Size, squares))
                        .OrderBy(i => i.Rarity != Rarity.Unique)
                        .ThenByDescending(i => i.Size.X * i.Size.Y)
                        .ThenBy(i => i.Position.DistanceSqr)
                        .FirstOrDefault();
                    if (_item == null)
                    {
                        if (!await PlayerAction.TpToTown())
                        {
                            ErrorManager.ReportError();
                            return true;
                        }
                        ReturnAfterTownrunTask.Enabled = true;
                        return true;
                    }
                }
                else
                    _item = validItems.OrderBy(i => i.Rarity != Rarity.Unique).ThenBy(i => i.Position.DistanceSqr).First();
            }
            var pos = _item.Position;
            if (pos.IsFar || pos.IsFarByPath)
                if (LogInterval.Elapsed)
                    GlobalLog.Debug($"[LootItemTask] Items to pick up: {validItems.Count}");
                    GlobalLog.Debug($"[LootItemTask] Moving to {pos}");
                if (!PlayerMoverManager.MoveTowards(pos))
                    GlobalLog.Error($"[LootItemTask] Fail to move to {pos}. Marking this item as unwalkable.");
                    _item.Unwalkable = true;
                    _item = null;
                return true;
            var itemObj = _item.Object;
            if (itemObj == null)
                items.Remove(_item);
                _item = null;
            if (!CanFit(_item.Size, Inventories.AvailableInventorySquares))
                _isInPreTownrunMode = true;
            var attempts = ++_item.InteractionAttempts;
            if (attempts > MaxItemPickupAttempts)
                if (_item.Position.Name == CurrencyNames.Mirror)
                    GlobalLog.Error("[LootItemTask] Fail to pick up the Mirror of Kalandra. Now stopping the bot.");
                    BotManager.Stop();
                    GlobalLog.Error("[LootItemTask] All attempts to pick up an item have been spent. Now ignoring it.");
                    _item.Ignored = true;
            if (attempts % 3 == 0)
                await PlayerAction.DisableAlwaysHighlight();
            if (attempts == MaxItemPickupAttempts / 2)
                if (PortalNearby)
                    GlobalLog.Debug("[LootItemTask] There is a portal nearby, which probably blocks current item label.");
                    GlobalLog.Debug("[LootItemTask] Now going to create a new portal at some distance.");
                    if (await MoveAway(40, 70))
                        await PlayerAction.CreateTownPortal();
                    GlobalLog.Debug("[LootItemTask] Now trying to move away from item, sometimes it helps.");
                    await MoveAway(30, 50);
            await PlayerAction.EnableAlwaysHighlight();
            GlobalLog.Debug($"[LootItemTask] Now picking up {pos}");
            var cached = new CachedItem(itemObj.Item);
            if (await PlayerAction.Interact(itemObj))
                await Wait.LatencySleep();
                if (await Wait.For(() => _item.Object == null, "item pick up", 100, 400))
                    items.Remove(_item);
                    GlobalLog.Info($"[Events] Item looted ({cached.Name})");
                    Utility.BroadcastMessage(this, Events.Messages.ItemLootedEvent, cached);
            await Wait.StuckDetectionSleep(300);
            return true;
        }
        private static async Task<bool> MoveAway(int min, int max)
            var pos = WorldPosition.FindPathablePositionAtDistance(min, max, 5);
            if (pos == null)
                GlobalLog.Debug("[LootItemTask] Fail to find any pathable position at distance.");
            await Move.AtOnce(pos, "distant position", 10);
        //items tend to stuck inside opened strongbox, this problem usually fixes itself upon area change
        private static void UnignoreStrongboxItems()
                return;
            var strBox = LokiPoe.ObjectManager.Objects.FirstOrDefault<Chest>(c => c.IsStrongBox && c.IsOpened && !c.IsLocked);
            if (strBox == null)
            var boxPos = strBox.Position;
            foreach (var ignoredItem in CombatAreaCache.Current.Items.Where(i => i.Ignored))
                var itemPos = ignoredItem.Position;
                if (itemPos.AsVector.Distance(boxPos) < 30)
                    GlobalLog.Debug($"[LootItemTask] Removing ignored flag from {itemPos} because it is near opened strongbox.");
                    ignoredItem.Ignored = false;
        private static bool CanFit(Vector2i size, int availableSquares)
            var itemSquares = size.X * size.Y;
            if ((availableSquares - itemSquares) < Settings.Instance.MinInventorySquares)
            return LokiPoe.InstanceInfo.GetPlayerInventoryBySlot(InventorySlot.Main).CanFitItem(size);
        public MessageResult Message(Message message)
            var id = message.Id;
            if (id == Events.Messages.AreaChanged)
                _isInPreTownrunMode = false;
                UnignoreStrongboxItems();
                return MessageResult.Processed;
            if (id == "GetCurrentItem")
                message.AddOutput(this, _item);
            if (id == "SetCurrentItem")
                _item = message.GetInput<CachedWorldItem>();
            if (id == "ResetCurrentItem")
            return MessageResult.Unprocessed;
        private static bool PortalNearby => LokiPoe.ObjectManager.Objects.Any<Portal>(p => p.IsPlayerPortal() && p.Distance <= 20);
        #region Unused interface methods
        public async Task<LogicResult> Logic(Logic logic)
            return LogicResult.Unprovided;
        public void Tick()
        public void Start()
        public void Stop()
        public string Name => "LootItemTask";
        public string Description => "Task that handles item looting.";
        public string Author => "ExVault";
        public string Version => "1.0";
        #endregion
    }
}
