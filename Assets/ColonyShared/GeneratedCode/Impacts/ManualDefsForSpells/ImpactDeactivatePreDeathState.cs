using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils.Extensions;

namespace Assets.Src.ManualDefsForSpells
{
    public class ImpactDeactivatePreDeathState : IImpactBinding<ImpactDeactivatePreDeathStateDef>
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactDeactivatePreDeathStateDef def)
        {
            var t = await def.Target.Target.GetOuterRef(cast, repo);
            if (!t.IsValid)
                return;
            using (var wrapper = await repo.Get(t.RepTypeId(ReplicationLevel.Server), t.Guid))
            {
                var mortal = wrapper.Get<IHasMortalServer>(t.RepTypeId(ReplicationLevel.Server), t.Guid);
                if (mortal == null)
                {
                    Logger.IfDebug()?.Message($"`{nameof(IHasMortalServer)}` target entity is null").Write(); // common on impacts `OnSuccess`
                    return;
                }

                await mortal.Mortal.DeactivatePreDeathState();
            }
        }

    }
}
