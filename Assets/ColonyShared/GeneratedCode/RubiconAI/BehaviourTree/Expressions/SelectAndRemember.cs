using System.Collections.Generic;
using JetBrains.Annotations;
using Assets.Src.RubiconAI.BehaviourTree.NodeTypes;
using System;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Utils;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI.BehaviourTree.Expressions
{
    public class SelectAndRemember : TargetSelector, ICollectionSelector, IInvalidatableSelector
    {
        public Strategy HostStrategy { get; set; }
        Legionary _rememberedLegionary;
        Vector3? _rememberedPoint;
        TargetSelector _targetSelector;
        public async ValueTask InitWith(Strategy hostStrategy, IBehaviourExpressionDef def)
        {
            var selectorDef = ((SelectAndRememberDef)def).Selector.Target;
            _targetSelector = (TargetSelector)await hostStrategy.MetaExpression.Get(hostStrategy, selectorDef);
            _rememberedPoint = null;
            _rememberedLegionary = null;
        }

        public void OnIMGUI()
        {
        }

        public async ValueTask<Vector3?> SelectPoint(Legionary legionary)
        {
            if (_rememberedLegionary != null)
                if (_rememberedLegionary.Valid())
                    return HostStrategy.CurrentLegionary.GetPos(_rememberedLegionary);
            if (_rememberedPoint.HasValue)
                return _rememberedPoint;
            if (AIProfiler.EnableProfile)
                AIProfiler.BeginSample("SelectPoint ", _targetSelector.GetType().Name);
            _rememberedPoint = await _targetSelector.SelectPoint(legionary);
            if (AIProfiler.EnableProfile)
                AIProfiler.EndSample();
            return _rememberedPoint;
        }

        public async ValueTask<Legionary> SelectTarget(Legionary legionary)
        {
            if (await Invalid())
                return null;
            if (_rememberedLegionary != null)
                return _rememberedLegionary;
            if (AIProfiler.EnableProfile)
                AIProfiler.BeginSample("SelectTarget ", _targetSelector.GetType().Name);
            _rememberedLegionary = await _targetSelector.SelectTarget(legionary);
            if (AIProfiler.EnableProfile)
                AIProfiler.EndSample();
            return _rememberedLegionary;
        }

        public async ValueTask GetLegionaries([NotNull] Legionary legionary, List<Legionary> collection)
        {
            await ((ICollectionSelector)_targetSelector).GetLegionaries(legionary, collection);
        }

        public async ValueTask<bool> Invalid()
        {
            if (_rememberedPoint.HasValue)
                return false;
            if (_rememberedLegionary == null)
                return false;
            if (!_rememberedLegionary.IsValid)
                return true;
            return !await _targetSelector.StillCouldSelect(_rememberedLegionary);
        }

        public async ValueTask<bool> StillCouldSelect(Legionary targetLegionary)
        {
            if (_rememberedLegionary != targetLegionary)
                return false;
            return !await Invalid();
        }
    }
}
