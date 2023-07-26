using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Arithmetic;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class CheckStat : Condition
    {
        public Strategy HostStrategy { get; set; }
        private CheckStatDef _def;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = (CheckStatDef)def;
        }


        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            float value = 0;
            using (var wrapper = await legionary.Repository.Get(legionary.Ref.TypeId, legionary.Ref.Guid))
            {
                value = (await wrapper.Get<IHasStatsEngineServer>(legionary.Ref.TypeId, legionary.Ref.Guid, SharedCode.EntitySystem.ReplicationLevel.Server).Stats.TryGetValue(_def.Stat.Target)).Item2;
            }
            if (_def.Operation == CheckStatDef.CompareType.Less)
            {
                return value < _def.Value;
            }
            else if (_def.Operation == CheckStatDef.CompareType.More)
            {
                return value > _def.Value;
            }
            return false;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return await Evaluate(other);
        }
    }
}
