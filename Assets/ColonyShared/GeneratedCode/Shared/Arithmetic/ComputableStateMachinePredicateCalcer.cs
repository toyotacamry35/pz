using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Assets.ColonyShared.GeneratedCode.Time;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates.Cluster;
using Assets.Src.Arithmetic;
using Core.Environment.Logging.Extension;
using Core.Reflection;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.GeneratedCode.Shared.Arithmetic
{
    // Is similar to `CalcerCalcer`, but about lootTable & on cluster
    public static class ComputableStateMachinePredicateCalcer
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ClusterPredicateCalcer");

        public static readonly IReadOnlyDictionary<Type, Func<ComputableStateMachinePredicateDef, int, Guid, IEntitiesRepository, Task<bool>>> Implementations;

        // --- C-tor ----------------------

        static ComputableStateMachinePredicateCalcer()
        {
            Implementations = MethodBase.GetCurrentMethod().DeclaringType.GetMethodsSafe(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(
                v => v.Name == "CalcAsyncDo"
                && v.GetParameters().Length == 4
                && v.GetParameters()[1].ParameterType == typeof(int)
                && v.GetParameters()[2].ParameterType == typeof(Guid)
                && v.GetParameters()[3].ParameterType == typeof(IEntitiesRepository)
                && v.ReturnType == typeof(Task<bool>)
                && typeof(ComputableStateMachinePredicateDef).IsAssignableFrom(v.GetParameters()[0].ParameterType)
                )
                .Select(
                method => new { func = DelegateCreator.PredicateCalcerMagicMethod<ComputableStateMachinePredicateDef, Task<bool>>(method), type = method.GetParameters()[0].ParameterType }
                )
                .ToDictionary(v => v.type, v => v.func);
            
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(ComputableStateMachinePredicateCalcer)} static ctor done. Implementations.N == {Implementations.Count}").Write();
        }

        public static void TriggerStaticCtorToBeCalledHack()
        {
        }

        // --- API ----------------------------------------------------------

        [System.Diagnostics.Contracts.Pure]
        public static async Task<bool> CalcAsync([NotNull] this ComputableStateMachinePredicateDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            if (pred == null)
                return false;

            //if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"{nameof(ComputableStateMachinePredicateCalcer)}.{nameof(CalcAsync)} Implementations.N == {Implementations.Count}. by type `{pred.GetType()}` has table?: {Implementations.ContainsKey(pred.GetType())}.").Write();
            return await Implementations[pred.GetType()](pred, entityTypeId, entityId, repo);
        }

        // --- Privates predicates: -------------------------------------------------------------------------------------
        
        // --- 1. basic logical (True, Or, Inverse, ...): ------------------------

        [UsedImplicitly]
        private static Task<bool> CalcAsyncDo(ComputableStateMachinePredicateTrueDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("CalcAsync( ClusterPredicateTrue )").Write();;
            return Task.FromResult(true);
        }

        [UsedImplicitly]
        private static Task<bool> CalcAsyncDo(ComputableStateMachinePredicateFalseDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("CalcAsync( ClusterPredicateFalse )").Write();;
            return Task.FromResult(false);
        }

        [UsedImplicitly]
        private static async Task<bool> CalcAsyncDo(ComputableStateMachinePredicateInverseDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            return !await pred.Predicate.Target.CalcAsync(entityTypeId, entityId, repo);
        }

        [UsedImplicitly]
        private static async Task<bool> CalcAsyncDo(ComputableStateMachinePredicateAndDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            //return pred.Predicates.All(x=>x.Target.Calc(obj));
            foreach (var predicate in pred.Predicates)
            {
                if (!await predicate.Target.CalcAsync(entityTypeId, entityId, repo))
                    return false;
            }

            return true;
        }

        [UsedImplicitly]
        private static async Task<bool> CalcAsyncDo(ComputableStateMachinePredicateOrDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            //return pred.Predicates.Any(x=>x.Target.Calc(obj));
            foreach (var predicate in pred.Predicates)
            {
                if (await predicate.Target.CalcAsync(entityTypeId, entityId, repo))
                    return true;
            }

            return false;
        }

        // --- 2. Specific predicates: -------------------------------------------------

        [UsedImplicitly]
        private static async Task<bool> CalcAsyncDo(ComputableStateMachinePredicateInGameTimeDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            if (Logger.IsDebugEnabled)  Logger.IfDebug()?.Message("CalcAsync( ClusterPredicateInGameTime )").Write();;
            return await InGameTime.IsNowWithinInterval(pred.TimeInterval, repo);
        }

        [UsedImplicitly]
        private static async Task<bool> CalcAsyncDo(ComputableStateMachinePredicateExpiredLifespanPercentDef pred, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"CalcAsyncDo({nameof(ComputableStateMachinePredicateExpiredLifespanPercentDef)})").Write();
            using (var wrapper = await repo.Get(entityTypeId, entityId))
            {
                var entity = wrapper?.Get<IHasLifespanClientBroadcast>(entityTypeId, entityId, ReplicationLevel.ClientBroadcast);
                if (entity == null)
                {
                    Logger.IfError()?.Message("Can't get {nameof(IHasLifespanClientBroadcast)} by {entityTypeId}, {entityId}.").Write();
                    return false;
                }
        
                return await entity.Lifespan.IsExpiredLifespanPercentInRange(pred.FromIncluding, pred.TillExcluding); 
            }
        }

    }
}
