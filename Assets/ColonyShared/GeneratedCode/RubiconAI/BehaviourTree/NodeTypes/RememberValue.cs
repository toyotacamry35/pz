using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using Assets.Src.RubiconAI.BehaviourTree.Expressions;
using ColonyShared.SharedCode.Utils;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    class RememberValue : BehaviourNode
    {
        StatModifierDef _modDef;
        MemorizedStatDef _memStatDef;
        TargetSelector _target;
        TargetSelector _memoryOf;
        float _time;
        float _flat;
        private RememberValueDef _def;
        protected override async ValueTask OnCreate(BehaviourNodeDef def)
        {
            _def = (RememberValueDef)def;
            _modDef = _def.ModDef;
            _memStatDef = _def.StatDef;
            _target = (TargetSelector)await _def.Target.ExprOptional(HostStrategy);
            _memoryOf = (TargetSelector)await _def.Memory.ExprOptional(HostStrategy);
        }

        public override async ValueTask<ScriptResultType> OnStart()
        {
            _time = await _def.Time.GetOptional(HostStrategy);
            _flat = await _def.Flat.GetOptional(HostStrategy);
            
            var target = _target == null ? null : await _target.SelectTarget(HostStrategy.CurrentLegionary);
            if (target == null && _target == null)
                target = HostStrategy.CurrentLegionary;
            if (target == null)
            {
                StatusDescription = "No target found";
                return ScriptResultType.Failed;

            }
            var memory = HostStrategy.CurrentLegionary.Knowledge.Memory;
            if (_memoryOf != null)
            {
                AIProfiler.BeginSample("Select Target");
                var memoryTarget = await _memoryOf.SelectTarget(HostStrategy.CurrentLegionary);
                AIProfiler.EndSample();
                if (memoryTarget == null || !(memoryTarget is MobLegionary ml))
                {
                    StatusDescription = "No memory target found";
                    return ScriptResultType.Failed;
                }
                memory = ml.Knowledge.Memory;
            }
            switch (_def.Change)
            {
                case RememberValueDef.ChangeType.Add:
                    {

                        AIProfiler.BeginSample("GetStatMod");
                        var rememberedValue = memory.GetStatMod(target, _memStatDef, new StatModKey() { Assigner = HostStrategy.CurrentLegionary, Def = _modDef });
                        AIProfiler.EndSample();

                        AIProfiler.BeginSample("SetStatMod");
                        if (rememberedValue != null)
                            memory.SetStatMod(
                                target,
                                rememberedValue.EndsAt - SyncTime.NowUnsynced + SyncTime.FromSeconds(_time),
                                _memStatDef,
                                rememberedValue.Flat + _flat,
                                new StatModKey() { Assigner = HostStrategy.CurrentLegionary, Def = _modDef });
                        else
                            memory.SetStatMod(
                                target,
                                SyncTime.FromSeconds(_time),
                                _memStatDef,
                                _flat,
                                new StatModKey() { Assigner = HostStrategy.CurrentLegionary, Def = _modDef });
                        AIProfiler.EndSample();
                    }
                    break;
                case RememberValueDef.ChangeType.Set:
                    AIProfiler.BeginSample("SetStatMod");
                    memory.SetStatMod(
                        target,
                        SyncTime.FromSeconds(_time),
                        _memStatDef,
                        _flat,
                        new StatModKey() { Assigner = HostStrategy.CurrentLegionary, Def = _modDef });
                    AIProfiler.EndSample();
                    break;
                case RememberValueDef.ChangeType.Remove:
                    AIProfiler.BeginSample("EndStatMod");
                    memory.RemoveStatMod(target, _memStatDef, new StatModKey() { Assigner = HostStrategy.CurrentLegionary, Def = _modDef });
                    AIProfiler.EndSample();
                    break;
            }
            return ScriptResultType.Succeeded;
        }
    }
}
