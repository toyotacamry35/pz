using System;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using SharedCode.EntitySystem;

namespace Assets.ColonyShared.GeneratedCode.Shared.Arithmetic
{
    public static class ComputableStateMachineCalcer
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("ClusterComputableStateMachineCalcer");

        // #TODO: Add dynamic delegate choosing when heirs of `ComputableStateMachineDef` will appear
        [System.Diagnostics.Contracts.Pure]
        public static async Task<ComputableStatesDef> CalcCurrStates([NotNull] this ComputableStateMachineDef stateMachine, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            return await CalcImpl(stateMachine, entityTypeId, entityId, repo);
        }

        // --- Privates: -------------------------------------------------------------------------------------

        private static async Task<ComputableStatesDef> CalcImpl([NotNull] ComputableStateMachineDef stateMachine, int entityTypeId, Guid entityId, IEntitiesRepository repo)
        {
            if (stateMachine.StatesTable == null || stateMachine.StatesTable.Count == 0)
                return null;

            foreach (var entry in stateMachine.StatesTable)
            {
                var predicateResult = (entry.Predicate.Target != null) && await entry.Predicate.Target?.CalcAsync(entityTypeId, entityId, repo);
                if (!predicateResult)
                    continue;

                return entry.States;
            }

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"CalcImpl(`{nameof(ComputableStateMachineDef)}`). NULL 'll be returned.").Write();
            return null;
        }

    }

}