using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;
using SharedCode.Utils.Extensions;
using SharedCode.Wizardry;

namespace Shared.ManualDefsForSpells
{
    public class ImpactDestroyEntity : IImpactBinding<ImpactDestroyEntityDef>
    {
        [NotNull]
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ImpactDestroyEntity");

        public async ValueTask Apply(SpellWordCastData cast, IEntitiesRepository repo, ImpactDestroyEntityDef def)
        {
            var t = ((IWithTarget)cast.CastData).Target;
            using (var wrapper = await repo.Get(t.RepTypeId(ReplicationLevel.Server), t.Guid))
            {
                var destroyable = wrapper.Get<IDestroyableServer>(t.RepTypeId(ReplicationLevel.Server), t.Guid);
                if (destroyable == null)
                {
                    Logger.IfError()?.Message($"`{nameof(IDestroyableServer)}` target entity is null").Write();
                    return;
                }

                await destroyable.Destroy();
            }
        }

    }
}
