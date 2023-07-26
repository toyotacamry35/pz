using Assets.Src.Arithmetic;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using Shared.ManualDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.Predicates
{
    public class PredicateCompareStat : IPredicateBinding<PredicateCompareStatDef>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async ValueTask<bool> True(SpellPredCastData cast, IEntitiesRepository repository, PredicateCompareStatDef def)
        {
            var selfDef = (PredicateCompareStatDef)def;
            var statRes = selfDef.Stat;

            float? res = null;
            var guid = cast.Caster.Guid;
            var typeId = cast.Caster.TypeId;
            if (selfDef.Target.Target != null)
            {
                var ent = await selfDef.Target.Target.GetOuterRef(cast, repository);
                guid = ent.Guid;
                typeId = ent.TypeId;
            }
            if (typeId == 0)
                return false;
            using (var wrapper = await repository.Get(typeId, guid))
            {
                var entity = wrapper?.Get<IHasStatsEngineClientBroadcast>(typeId, guid, ReplicationLevel.ClientBroadcast);
                if (entity != null)
                {
                    var val = await entity.Stats.TryGetValue(selfDef.Stat);
                    if (val.Item1)
                        res = val.Item2;
                }
            }

            if (res.HasValue)
            {
                var currVal = res.Value;
                var value = await selfDef.Value.Target.CalcAsyncFromSpell(new OuterRef<IEntity>(guid, typeId), repository, cast);
                bool result = false;
                switch (selfDef.Type)
                {
                    case ComprasionType.More:
                        result = res > value;
                        break;
                    case ComprasionType.Less:
                        result = currVal < value;
                        break;
                    case ComprasionType.Equal:
                        result = Math.Abs(currVal - value) < Math.E;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return result;
            }

            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"PredicateCompareStat: {selfDef.Stat.Target.DebugName} ({(res.HasValue ? res.Value : float.NaN)}) is {selfDef.Type} than {selfDef.Value} == False").Write();
            return false;
        }
    }
}