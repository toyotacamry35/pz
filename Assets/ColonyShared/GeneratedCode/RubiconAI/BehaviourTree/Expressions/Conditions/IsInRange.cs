using Assets.Src.Tools;
using JetBrains.Annotations;
using NLog;
using SharedCode.Utils;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SVector2 = SharedCode.Utils.Vector2;
using SVector3 = SharedCode.Utils.Vector3;
using static ColonyHelpers.SharedHelpers;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions.Conditions
{
    class IsInRange : Condition
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public Strategy HostStrategy { get; set; }
        private TargetSelector _targetSelector;
        private TargetSelector _basisSelector;
        private IsInRangeDef _def;
        private float _range;
        private float _heightDeltaUpMax;
        private float _heightDeltaDownMax;
        // sector: (From Left border, To Right border) clockwise
        private Vector2 _sectorLR;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            _def = (IsInRangeDef) def;
            _range              = await _def.Range.Get(hostStrategy);
            _heightDeltaUpMax   = await _def.HeightDeltaUpMax.GetOptional(hostStrategy, _range);
            _heightDeltaDownMax = await _def.HeightDeltaDownMax.GetOptional(hostStrategy, -_range);
            _sectorLR = new Vector2(await _def.SectorBorderL.GetOptional(hostStrategy),
                                  await _def.SectorBorderR.GetOptional(hostStrategy));
            _targetSelector = (TargetSelector)await _def.TargetSelectorDef.Expr(hostStrategy);
            _basisSelector = (_def.BasisSelectorDef.IsValid)
                ? (TargetSelector) await _def.BasisSelectorDef.Expr(hostStrategy)
                : null;
        }

        public async ValueTask<bool> Evaluate(Legionary legionary)
        {
            var targetPoint = await _targetSelector.SelectPoint(legionary);
            var basisPoint = (_basisSelector != null) 
                ? await _basisSelector.SelectPoint(legionary)
                : HostStrategy.CurrentLegionary.GetOtherDataFromMemory(legionary).Pos;
            
            if (!targetPoint.HasValue || !basisPoint.HasValue)
                return _def.Not; //if doesn't has value and we're NOT , meaning NOT IN RANGE. Null value is not in range

            var deltaHeight = targetPoint.Value.y - basisPoint.Value.y;
            if (!deltaHeight.InRange(_heightDeltaDownMax, _heightDeltaUpMax))
                return _def.Not;

            var _distance = (targetPoint.Value - basisPoint.Value).SqrMagnitude;
            if (_distance > _range * _range)
                return _def.Not;

            if (_sectorLR != Vector2.zero)
            {
                var basisTransformLeg = (_basisSelector != null)
                    ? await _basisSelector.SelectTarget(legionary)
                    : legionary;
                var basisPos = (HostStrategy.CurrentLegionary)?.GetPos(basisTransformLeg);
                if (!basisPos.HasValue)
                {
                    Logger.IfError()?.Message("basisTransform == null").Write();
                    return _def.Not;
                }


                bool isInsideSector = IsPointInsideSector(_sectorLR, basisPos.Value, (basisTransformLeg as MobLegionary)?.Forward ?? Vector3.one, targetPoint.Value, _def.DebugDraw);
                isInsideSector = _def.InverseSector ? !isInsideSector : isInsideSector;
                isInsideSector = _def.Not ? !isInsideSector : isInsideSector;
                return isInsideSector;
            }

            return !_def.Not;
        }

        public ValueTask<bool> EvaluateOther(Legionary self, Legionary other)
        {
            return Evaluate(other);
        }

    }
}
