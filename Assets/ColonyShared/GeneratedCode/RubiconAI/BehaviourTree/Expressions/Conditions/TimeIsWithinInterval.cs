using Assets.ColonyShared.GeneratedCode.Regions;
using Assets.ColonyShared.SharedCode.Aspects.Templates;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class TimeIsWithinInterval : Condition
    {
        public Strategy HostStrategy { get; set; }

        private InGameTimeIntervalDef _intervalDef;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _intervalDef = ((TimeIsWithinIntervalDef)def).Interval.Target;
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            return RegionTime.IsNowWithinInterval(_intervalDef);
        }

        public async ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return await Evaluate(other);
        }

        public void OnIMGUI()
        {
        }
    }

}
