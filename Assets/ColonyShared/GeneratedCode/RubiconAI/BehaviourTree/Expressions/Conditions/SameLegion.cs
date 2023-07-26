using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class SameLegion : Condition
    {
        public Strategy HostStrategy { get; set; }
        private TargetSelector _selector;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _selector = (TargetSelector)await ((SameLegionDef) def).Target.Expr(hostStrategy);
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return (await _selector.SelectTarget(legionary))?.AssignedLegion == legionary.AssignedLegion;
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return (await _selector.SelectTarget(self))?.AssignedLegion == other?.AssignedLegion;
        }
    }



}
