using SharedCode.MovementSync;
using SharedCode.Utils;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{


    class Self : TargetSelector
    {
        SelfDef _selfDef;
        public Strategy HostStrategy { get; set; }
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _selfDef = (SelfDef)def;
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<Legionary> SelectTarget(Legionary legionary)
        {
            if (_selfDef.FromSelf.Target != null)
                return await ((TargetSelector)await _selfDef.FromSelf.Expr(HostStrategy)).SelectTarget(HostStrategy.CurrentLegionary);
            return HostStrategy.CurrentLegionary;
        }

        public async ValueTask<Vector3?> SelectPoint(Legionary legionary)
        {
            if (_selfDef.FromSelf.Target != null)
                return await ((TargetSelector)await _selfDef.FromSelf.Expr(HostStrategy)).SelectPoint(HostStrategy.CurrentLegionary);
            return HostStrategy.CurrentLegionary.GetPos(HostStrategy.CurrentLegionary);
        }

        public async ValueTask<bool> StillCouldSelect(Legionary targetLegionary)
        {
            return true;
        }
    }
    
    class TargetLegion : TargetSelector
    {
        public Strategy HostStrategy { get; set; }
        TargetSelector _selector;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _selector = (TargetSelector)await ((TargetLegionDef)def).TargetSelector.ExprOptional(hostStrategy);
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<Legionary> SelectTarget(Legionary legionary)
        {
            if (_selector == null)
                return legionary.AssignedLegion.Legionary;
            else
                return (await _selector.SelectTarget(legionary)).AssignedLegion.Legionary;
        }

        public async ValueTask<Vector3?> SelectPoint(Legionary legionary)
        {
            return HostStrategy.CurrentLegionary.GetOtherDataFromMemory(await SelectTarget(legionary)).Pos;
        }

        public async ValueTask<bool> StillCouldSelect(Legionary targetLegionary)
        {
            return await _selector.StillCouldSelect(targetLegionary);
        }
    }
}
