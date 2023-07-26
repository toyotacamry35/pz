using Assets.Src.Aspects;
using ColonyShared.SharedCode.Utils;
using GeneratedCode.DeltaObjects;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Assets.ColonyShared.GeneratedCode.Shared;
using Assets.ColonyShared.GeneratedCode.Regions;
using Core.Environment.Logging.Extension;
using SharedCode.Entities;
using SharedCode.Entities.Engine;
using SharedCode.Logging;
using SharedCode.Serializers;

namespace Assets.Src.Effects
{
    public class EffectStaticAOE : IEffectBinding<EffectStaticAOEDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectStaticAOEDef def)
        {
            if (cast.IsSlave)
                return;
            var selfDef = def;
            var oRef = await selfDef.Target.Target.GetOuterRef(cast, repo);
            Transform t = default;
            using (var ew = await repo.Get(oRef))
            {
                var e = PositionedObjectHelper.GetPositioned(ew, oRef.TypeId, oRef.Guid);
                if (e == null)
                    return;
                t = e.Transform;
            }

            var ownerScene = repo.GetSceneForEntity(oRef);
            if (ownerScene == default)
                return;
            var region = RegionsHolder.GetRegionByGuid(ownerScene);
            if (region != null)
                ((RootRegion)region).AddDynamicRegion((new ModifierCauser(def, cast.SpellId.Counter), cast.Caster), t, selfDef.RegionDef.Target);
            else
                Logger.IfWarn()?.Message("Region {0} is not ready yet", ownerScene).Write();
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectStaticAOEDef def)
        {
            if (cast.IsSlave)
                return;
            var oRef = await def.Target.Target.GetOuterRef(cast, repo);
            var ownerScene = repo.GetSceneForEntity(oRef);
            if (ownerScene == default)
                return;
            var region = RegionsHolder.GetRegionByGuid(ownerScene);
            if (region != null)
                ((RootRegion)region).RemoveDynamicRegion((new ModifierCauser(def, cast.SpellId.Counter), cast.Caster));
        }

      
    }
}
