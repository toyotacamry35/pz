using Assets.Src.Arithmetic;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vector3 = SharedCode.Utils.Vector3;
using Assets.ResourceSystem.Aspects.ManualDefsForSpells;
using Assets.ColonyShared.GeneratedCode.Impacts.ShapeUtils;
using SharedCode.Entities;
using System.Linq;
using Core.Environment.Logging.Extension;
using SharedCode.MovementSync;
using Shared.ManualDefsForSpells;

namespace Assets.ColonyShared.GeneratedCode.Impacts
{
    public class ImpactForAllInBox //: IImpactBinding<ImpactForAllInBoxDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactForAllInBoxDef indef)
        {
            ImpactInShapeDef adapter = new ImpactInShapeDef();
            adapter.AppliedSpells = indef.AppliedSpells;
            adapter.PredicateOnTarget = indef.PredicateOnTarget;

            var box = new BoxShapeDef();

            if (indef.AttackBoxes != null && indef.AttackBoxes.Length > 0)
            {
                box.Position = indef.AttackBoxes[0].center;
                box.Extents = indef.AttackBoxes[0].extents;
            }
            else
                Logger.IfError()?.Message("{0} have incorrect AttackBoxes def!", nameof(ImpactForAllInBox)).Write();

            adapter.Shape = box;

            await SpellImpacts.CastImpact(cast, adapter, repo);
        }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    }

    public class ImpactNearestInBox //: IImpactBinding<ImpactNearestInBoxDef>
    {
        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactNearestInBoxDef indef)
        {
            ImpactInShapeDef adapter = new ImpactInShapeDef();
            adapter.AppliedSpells = indef.AppliedSpells;
            adapter.PredicateOnTarget = indef.PredicateOnTarget;

            var box = new BoxShapeDef();

            if (indef.AttackBoxes != null && indef.AttackBoxes.Length > 0)
            {
                box.Position = indef.AttackBoxes[0].center;
                box.Extents = indef.AttackBoxes[0].extents;
            }
            else
                Logger.IfError()?.Message("{0} have incorrect AttackBoxes def!", nameof(ImpactNearestInBox)).Write();

            adapter.Shape = box;

            await SpellImpacts.CastImpact(cast, adapter, repo);
        }

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    }
}
