using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Impacts.ShapeUtils
{
    public class LinksOfTarget : IShapeBinding<LinksOfTargetDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<bool> CheckCollision(LinksOfTargetDef def, Transform shapeOwnerTransform, Vector3 point, IEntitiesRepository repo)
        {
            return true;
        }
         
        public async Task<List<VisibilityDataSample>> GetPosibleVisibilityData(LinksOfTargetDef def, SpellPredCastData castData, Guid worldspaceId, Transform shapeOwnerTransform, IEntitiesRepository repo)
        {
            List<VisibilityDataSample> result = new List<VisibilityDataSample>();
            var targetOref = await def.Target.Target.GetOuterRef(castData, repo);
            using (var ent = await repo.Get(targetOref.TypeId, targetOref.Guid))
            {
                var le = ent.Get<IHasLinksEngineServer>(targetOref, ReplicationLevel.Server)?.LinksEngine;

                if (le != null)
                {
                    if(le.Links.TryGetValue(def.LinkType, out var outerRefs))
                    {
                        foreach(var oref in outerRefs.Links)
                        if(oref.Key.IsValid)
                        {
                            VisibilityDataSample vds = new VisibilityDataSample();
                            vds.Ref = new OuterRef<IEntity>(oref.Key);
                            result.Add(vds);
                        }
                    }
                    else
                        Logger.IfWarn()?.Message($"[LinksOfTarget][GetPosibleVisibilityData] LinksEngine don't contains LinkType: {def.LinkType}\n").Write();
                }
            }
            
            return result;
        }
    }
}
