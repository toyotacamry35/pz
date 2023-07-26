using SharedCode.Utils;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    public class SelectEventTarget : TargetSelector
    {
        public Strategy HostStrategy { get; set; }
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<Vector3?> SelectPoint(Legionary legionary)
        {
            return HostStrategy.CurrentLegionary.GetPos(await SelectTarget(legionary));
        }

        public async ValueTask<Legionary> SelectTarget(Legionary legionary)
        {
            var leg= ((SpellCastEvent)HostStrategy.HostEvent).Target;
            if (leg != null && leg.IsValid)
                return leg;
            else
                return null;
        }

        public async ValueTask<bool> StillCouldSelect(Legionary targetLegionary)
        { 
            return targetLegionary.IsValid && targetLegionary == ((SpellCastEvent)HostStrategy.HostEvent).Target;
        }
    }
}
