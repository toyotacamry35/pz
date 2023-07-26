using System;
using System.Linq;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.Shared;
using Assets.Src.SpawnSystem;
using Assets.Src.Tools;
using SharedCode.MovementSync;
using SharedCode.Utils;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    public interface TargetSelector : BehaviourExpression
    {
        ValueTask<bool> StillCouldSelect(Legionary targetLegionary);
        ValueTask<Legionary> SelectTarget(Legionary legionary);
        ValueTask<Vector3?> SelectPoint(Legionary legionary);
    }



    public class RandomPointTargetSelector : TargetSelector
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Strategy HostStrategy { get; set; }
        private RandomPointTargetSelectorDef _def;
        private TargetSelector _basisSelector;
        private Random _rand = new Random((int)(System.DateTime.UtcNow.Ticks % int.MaxValue));
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = (RandomPointTargetSelectorDef)def;
            _basisSelector = (_def.BasisSelectorDef.IsValid)
                ? (TargetSelector)await _def.BasisSelectorDef.Expr(hostStrategy)
                : null;
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<Legionary> SelectTarget(Legionary legionary)
        {
            return null;
        }

        public async ValueTask<Vector3?> SelectPoint(Legionary legionary)
        {
            var basisPoint = (_basisSelector != null)
                ? await _basisSelector.SelectPoint(legionary)
                : HostStrategy.CurrentLegionary.GetOtherDataFromMemory(legionary).Pos;
            //Logger.IfError()?.Message($"{_basisSelector != null} {basisPoint.HasValue} {(basisPoint.HasValue ? basisPoint.Value : default(Vector3))}").Write();
            if (!basisPoint.HasValue)
                return null;
            Vector3 randomPositionAround = (new Vector3((float)_rand.NextDouble() - 0.5f, 0, (float)_rand.NextDouble() - 0.5f) * await _def.InRange.Get(HostStrategy));
            randomPositionAround += basisPoint.Value;

            //DebugExtension.DebugWireSphere(basisPoint.Value.ToXYZ(), Color.red, randomPositionAround.magnitude, 3f, true);

            //DebugExtension.DebugWireSphere(randomPositionAround.ToXYZ(), Color.green, 1f, 3f, true);
            //NavMeshHit hit;
            //if (NavMesh.SamplePosition(randomDirection, out hit, _def.InRange.Get(HostStrategy), NavMesh.AllAreas))
            //    return hit.position;
            //return null;
            return randomPositionAround;
        }

        public ValueTask<bool> StillCouldSelect(Legionary targetLegionary)
        {
            throw new NotImplementedException();
        }
    }


}
