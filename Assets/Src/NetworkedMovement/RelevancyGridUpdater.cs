using System;
using Assets.ColonyShared.GeneratedCode.Shared.Utils;
using Assets.Src.RubiconAI;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.NetworkedMovement
{
    // Is present only on `PlayerPawn`
    class RelevancyGridUpdater : EntityGameObjectComponent, IDebugInfoProvider
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        //#Dbg:
        float dbg_nextTimeLog;
        float dbg_deltaTimeLog = 5f;

        private void Update()
        {
            // It's no need on Cl, but Vit.said leave it. //#Update: now it's needed on Cl too
            ///#PZ-9704: #Dbg:
            ///AIWorld.RelevancyGrid.UpdatePosition(transform.position.ToShared(), new OuterRef<IEntity>(EntityId, TypeId));
            _dbg_goPos = transform.position;
            _dbg_cell = AIWorld.RelevancyGrid.UpdatePositionReturnsCellRef(transform.position.ToShared(), new OuterRef<IEntity>(EntityId, TypeId));

            if (LocomotionProfiler3.EnableProfile) LocomotionProfiler3.BeginSample("## RelevancyGridUpdater #Dbg");

            ///#Dbg:
            if (Logger.IsTraceEnabled && Time.realtimeSinceStartup > dbg_nextTimeLog)
            {
                dbg_nextTimeLog = Time.realtimeSinceStartup + dbg_deltaTimeLog;
                if (Logger.IsTraceEnabled) Logger.IfTrace()?.Message($"id: {EntityId}, pos: {transform.position}. Cell: {AIWorld.RelevancyGrid.Dbg_GetCellByObj(new OuterRef<IEntity>(EntityId, TypeId))}").Write();
            }

            LocomotionProfiler3.EndSample();
        }

        protected override void LostServer()
        {
            AIWorld.RelevancyGrid.Remove(new OuterRef<IEntity>(EntityId, TypeId));
        }

        //#Dbg:
        private Vector3 _dbg_goPos;
        private SpatialHash<OuterRef<IEntity>>.CellVector3 _dbg_cell;

        public string GetDebugInfo() => $"Rel-cyGridUpd-r.Cell: {_dbg_cell}\n"
                                      + $"Rel-cyGridUpd-r.G.o.Pos: {_dbg_goPos}";
    }
}
