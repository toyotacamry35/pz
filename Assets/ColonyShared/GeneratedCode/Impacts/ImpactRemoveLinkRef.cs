using Assets.ColonyShared.SharedCode.Entities;
using Assets.ColonyShared.SharedCode.Entities.Engine;
using ColonyShared.ManualDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace Assets.ColonyShared.GeneratedCode.Impacts
{
    public class ImpactRemoveLinkRef : IImpactBinding<ImpactRemoveLinkRefDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();


        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactRemoveLinkRefDef def)
        {
            var targetOref = await def.Target.Target.GetOuterRef(cast, repo);

            using (var ent = await repo.Get(targetOref.TypeId, targetOref.Guid))
            {
                var le = ent.Get<IHasLinksEngineServer>(targetOref, ReplicationLevel.Server)?.LinksEngine;

                if (le != null)
                {
                    var oref = await def.LinkedObject.Target.GetOuterRef(cast, repo);

                    if (!oref.IsValid)
                        return;

                    await le.RemoveLinkRef(oref);
                }
                else
                {
                    Logger.IfWarn()?.Message($"ImpactRemoveLink: Unexpected - Target({targetOref}) is not {nameof(IHasLinksEngineServer)}.").Write();
                }
            }

        }
    }
}
