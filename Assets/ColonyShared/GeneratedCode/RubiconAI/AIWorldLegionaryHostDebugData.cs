using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Lib.Extensions;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using static Assets.Src.RubiconAI.AIWorld;
using CellVec3 = Assets.Src.NetworkedMovement.SpatialHash<SharedCode.EntitySystem.OuterRef<SharedCode.EntitySystem.IEntity>>.CellVector3;

namespace Assets.Src.RubiconAI
{
    public class AIWorldLegionaryHostDebugData
    {
        public ActionType ActionType;
        public bool ReallyClose;
        public bool LegionaryNotNull;
        public long UpdatesCount;
        public DateTime LastUpdatedDateTime;
        public float LastBetweanUpdatesSec;
        public float LastBetweanUpdatesRecentMax;
        public CellVec3 CellVector;
        public Dictionary<OuterRef<IEntity>, CellVec3> AllObjects;

        internal void UpdateData(ActionType actionType, bool reallyClose, bool legionaryNotNull, long updatesCount,
            DateTime lastUpdatedDateTime, float lastBetweanUpdatesSec, float lastBetweanUpdatesRecentMax, CellVec3 cellVector, Dictionary<OuterRef<IEntity>, CellVec3> allObjects)
        {
            ActionType = actionType;
            ReallyClose = reallyClose;
            LegionaryNotNull = legionaryNotNull;
            UpdatesCount = updatesCount;
            LastUpdatedDateTime = lastUpdatedDateTime;
            LastBetweanUpdatesSec = lastBetweanUpdatesSec;
            LastBetweanUpdatesRecentMax = lastBetweanUpdatesRecentMax;
            CellVector = cellVector;
            AllObjects = allObjects;
        }

        //#Note: 'cos of `IsTimeToGatherDebugData` shows not actual data when LegionaryHost updates rarely (it could 
        public override string ToString()
        {
            return $"LegionaryHost::0 ActionType:  {ActionType}\n"
                 + $"LegionaryHost::2 _reallyClose:  {ReallyClose}\n"
                 + $"LegionaryHost::4 Legionary!=null ?:  {LegionaryNotNull}\n"
                 + $"LegionaryHost::5 _updatesCount:  {UpdatesCount}\n"
                 + $"LegionaryHost::6 _lastUpdatedDateTime:  {SharedHelpers.TimeStamp(LastUpdatedDateTime)}\n"
                 + $"LegionaryHost::7 _lastBetweanUpdatesSec / RecentMax:  {LastBetweanUpdatesSec:##.00} / {LastBetweanUpdatesRecentMax:##.00}\n"
                 + $"LegionaryHost::8.1 Cell my:  {CellVector}\n"
                 + $"LegionaryHost::8.2 Cells char:  {AllObjects?.ToStringCustom(":\n:") ?? "null"}";
        }

    }




}
