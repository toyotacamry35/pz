using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class IsLegionOfType : Condition
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Strategy HostStrategy { get; set; }
        private LegionDef _def;
        private TargetSelector _targetSelector;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = ((IsLegionOfTypeDef)def).LegionType;
            _targetSelector = (TargetSelector)await ((IsLegionOfTypeDef)def).Target.ExprOptional(hostStrategy);
        }


        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            if (_targetSelector != null)
            {
                var t = _targetSelector.SelectTarget(legionary);
                
                var def = t.IsCompleted? t.Result?.AssignedLegion?.Def : (await t)?.AssignedLegion?.Def;
                //Logger.IfError()?.Message($"{_def.____GetDebugShortName()} {(def == null ? "NULL" : def.____GetDebugShortName())}").Write();
                return _def.IsOfMyKin(def);
            }

            //Logger.IfError()?.Message($"{_def.____GetDebugShortName()} {(legionary?.AssignedLegion?.Def==null?"NULL":legionary?.AssignedLegion?.Def.____GetDebugShortName())}").Write();
            return _def.IsOfMyKin(legionary?.AssignedLegion?.Def);
        }

        public ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return Evaluate(other);
        }
    }



}
