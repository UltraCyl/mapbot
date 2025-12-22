using System.Collections.Generic;
using System.Linq;
using DreamPoeBot.Common;
using DreamPoeBot.Loki.Common;
using DreamPoeBot.Loki.Game;

namespace Default.EXtensions.Positions
{
    public class TgtPosition : WalkablePosition
    {
        private static readonly Vector2i Uninitialized = new Vector2i(int.MaxValue, int.MaxValue);
        private List<WorldPosition> _tgtPositions = new List<WorldPosition>();
        private readonly string _tgtName;
        private readonly bool _closest;
        private uint _areaHash;
        public override bool Initialized => _areaHash == LokiPoe.LocalData.AreaHash;
        public TgtPosition(string name, string tgtName, bool closest = false, int step = 10, int radius = 30)
            : base(name, Uninitialized, step, radius)
        {
            _tgtName = tgtName;
            _closest = closest;
        }
        public void ResetCurrentPosition()
            if (!Initialized)
            {
                HardInitialize();
            }
            else if (!SetCurrentPosition())
                GlobalLog.Error("[TgtPosition] No walkable position can be found.");
                ErrorManager.ReportCriticalError();
        public void ProceedToNext()
            if (_tgtPositions.Count <= 1)
                GlobalLog.Error("[TgtPosition] Cannot proceed to next, current one is the last.");
                return;
            var pos = _tgtPositions.OrderBy(p => p.DistanceSqr).First();
            _tgtPositions.Remove(pos);
            GlobalLog.Debug($"[TgtPosition] {pos} has been removed.");
            if (!SetCurrentPosition())
        public override bool Initialize()
            if (!FindTgtPositions())
                GlobalLog.Debug($"[TgtPosition] Fail to find any \"{_tgtName}\" tgt.");
                return false;
                GlobalLog.Debug("[TgtPosition] No walkable position can be found.");
            _areaHash = LokiPoe.LocalData.AreaHash;
            return true;
        protected override void HardInitialize()
                GlobalLog.Error($"[TgtPosition] Fail to find any \"{_tgtName}\" tgt.");
        private bool FindTgtPositions()
            _tgtPositions.Clear();
            if (_tgtName.Contains('|'))
                var tgtNames = _tgtName.Split('|').Select(tgt => tgt.Trim()).ToList();
                foreach (var tgtName in tgtNames)
                {
                    _tgtPositions.AddRange(Tgt.FindAll(tgtName));
                }
            else
                _tgtPositions.AddRange(Tgt.FindAll(_tgtName));
            return _tgtPositions.Count > 0;
        private bool SetCurrentPosition()
            bool walkableFound = _closest ? FindClosestPosition() : FindDistantPosition();
            if (walkableFound)
                GlobalLog.Warn($"[TgtPosition] Registering {this}");
                return true;
            return false;
        private bool FindDistantPosition()
            _tgtPositions = _tgtPositions.OrderByDescending(p => p.DistanceSqr).ToList();
            foreach (var pos in _tgtPositions)
                Vector = pos;
                if (FindWalkable()) return true;
        private bool FindClosestPosition()
            _tgtPositions = _tgtPositions.OrderBy(p => p.DistanceSqr).ToList();
    }
}
