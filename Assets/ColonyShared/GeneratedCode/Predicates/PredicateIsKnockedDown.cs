using Assets.Src.Arithmetic;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using GeneratedDefsForSpells;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Predicates
{
    public class PredicateIsKnockedDown : IPredicateBinding<PredicateIsKnockedDownDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateIsKnockedDownDef def)
        {
            var selfDef = (PredicateIsKnockedDownDef) def;
            if (selfDef.Target.Target != null)
            {
                var ent = await selfDef.Target.Target.GetOuterRef(cast, repository);
                var hmc = repository.TryGetLockfree<IHasMortalClientBroadcast>(ent, ReplicationLevel.ClientBroadcast);
                if (hmc != null)
                {
                    return hmc.Mortal.IsKnockedDown;
                }
            }

            return false;
        }
    }
}