using System;
using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ProcessSourceNamespace;
using UnityEngine;

namespace Uins
{
    public class InteractionIndicatorsProvider : MonoBehaviour
    {
        public Transform InstantiationParent;
        public GatheringIndicator GatheringIndicatorPrefab;
        public MiningIndicator MiningIndicatorPrefab;
        public MachineCraftingIndicator MachineCraftingIndicatorPrefab;

        private List<InteractionIndicator> _releasedIndicators = new List<InteractionIndicator>();
        private int _indicatorsCount;


        //=== Unity ===============================================================

        private void Awake()
        {
            InstantiationParent.AssertIfNull(nameof(InstantiationParent));
            GatheringIndicatorPrefab.AssertIfNull(nameof(GatheringIndicatorPrefab));
            MiningIndicatorPrefab.AssertIfNull(nameof(MiningIndicatorPrefab));
            MachineCraftingIndicatorPrefab.AssertIfNull(nameof(MachineCraftingIndicatorPrefab));
        }


        //=== Public ==============================================================

        public InteractionIndicator GetIndicator(IProcessSource processSource)
        {
            var indicatorType = GetIndicatorType(processSource);

            for (int i = 0, len = _releasedIndicators.Count; i < len; i++)
            {
                var indicator = _releasedIndicators[i];
                if (indicator.GetType() == indicatorType)
                {
                    var suitableIndicator = indicator;
                    _releasedIndicators.RemoveAt(i);
                    suitableIndicator.Init(processSource);
                    return suitableIndicator;
                }
            }

            var indicatorPrefab = GetIndicatorPrefab(indicatorType);

            if (indicatorPrefab == null)
                return null;

            var newIndicator = Instantiate(indicatorPrefab, InstantiationParent);
            if (newIndicator.AssertIfNull(nameof(newIndicator)))
                return null;

            newIndicator.transform.localPosition = Vector3.zero;
            newIndicator.name = $"Indicator[{++_indicatorsCount}]{processSource.Id.Type}";
            newIndicator.Init(processSource);
            return newIndicator;
        }

        public void ReleaseIndicator([NotNull] InteractionIndicator interactionIndicator)
        {
            interactionIndicator.InstantHideAndReset();
            _releasedIndicators.Add(interactionIndicator);
        }


        //=== Private =============================================================

        private Type GetIndicatorType([NotNull] IProcessSource processSource)
        {
            if (processSource.Id.Type == ProcessSourceId.ProcessType.CommonInteraction)
                return typeof(GatheringIndicator);

            if (processSource.Id.Type == ProcessSourceId.ProcessType.Mining)
                return typeof(MiningIndicator);

            if (processSource.Id.Type == ProcessSourceId.ProcessType.BuildingUpgrade)
                return typeof(MachineCraftingIndicator);

            UI.Logger.IfError()?.Message($"{nameof(GetIndicatorType)}({processSource}) Unhandled type: {processSource.GetType()}").Write();
            return null;
        }

        private InteractionIndicator GetIndicatorPrefab(Type indicatorType)
        {
            if (indicatorType == typeof(GatheringIndicator))
                return GatheringIndicatorPrefab;

            if (indicatorType == typeof(MiningIndicator))
                return MiningIndicatorPrefab;

            if (indicatorType == typeof(MachineCraftingIndicator))
                return MachineCraftingIndicatorPrefab;

            UI.Logger.IfError()?.Message($"{nameof(GetIndicatorPrefab)}({indicatorType}) Not found prefab of such type", gameObject).Write();
            return null;
        }
    }
}