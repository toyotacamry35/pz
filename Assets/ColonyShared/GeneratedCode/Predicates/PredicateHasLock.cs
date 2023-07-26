using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects.AccessRights;

namespace Assets.Src.Predicates
{
    [UsedImplicitly]
    public class PredicateHasLock : IPredicateBinding<PredicateHasLockDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repo, PredicateHasLockDef def)
        {
            if (def.Target.Target == null)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("No target").Write();;   
                return false;
            }

            OuterRef<IEntity> targetRef = await def.Target.Target.GetOuterRef(cast, repo);
            if (!targetRef.IsValid)
            {
                if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("targetRef is invalid").Write();;   
                return false;
            }

            var nfo = await PredicateCheckIfHasAccessRights.GetOwnerInformation(targetRef, repo);
            var result = nfo.LockPredicate != null && nfo.LockPredicate != AccessPredicateDef.Empty;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"LockPredicate:{(result ? nfo.LockPredicate.____GetDebugAddress() : "Empty")} Container:{targetRef} Owner:{nfo.Owner} Repo:{repo} Wizard:{cast.WhereAmI}").Write();
            return result;
        }
    }
}