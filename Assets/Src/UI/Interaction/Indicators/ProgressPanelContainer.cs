using System.Collections.Generic;
using Assets.Src.Aspects.Impl;
using Core.Environment.Logging.Extension;
using DG.Tweening;
using JetBrains.Annotations;
using SharedCode.Entities.Mineable;
using UnityEngine;

namespace Uins
{
    public class ProgressPanelContainer : ProgressPanelBase
    {
        private const int MaxIndicatorsCount = 3;

        public ProgressPanelOneItem[] ProgressPanels;

        private ProgressPanelOneItem CurrentProgressPanel;


        //=== Unity ===========================================================

        private void Awake()
        {
            if (!ProgressPanels.IsNullOrEmptyOrHasNullElements(nameof(ProgressPanels)))
            {
                if (ProgressPanels.Length != MaxIndicatorsCount)
                    UI.Logger.IfError()?.Message($"Unexpected {nameof(ProgressPanels)} length: {ProgressPanels.Length}", gameObject).Write();
            }
            CanvasGroup = null;
        }


        //=== Public ==========================================================

        public override void SetupResources(InteractionIndicator interactionIndicator)
        {
            var suitable = interactionIndicator != null ? GetSuitablePanel(interactionIndicator.ProcessSource.ExpectedItems) : null;
            if (suitable != CurrentProgressPanel)
            {
                //нужна другая ProgressPanel
                var prevAlpha = CanvasGroup?.alpha ?? -2;
                CurrentProgressPanel?.SetupResources(null);
                CurrentProgressPanel = suitable;
                CurrentProgressPanel?.SetupResources(interactionIndicator);
                CanvasGroup = CurrentProgressPanel?.CanvasGroup;
                if (prevAlpha > -1 && CanvasGroup != null)
                    CanvasGroup.alpha = prevAlpha;
            }
            else
            {
                if (CurrentProgressPanel == null)
                    return;

                //Реинитим текущую
                CurrentProgressPanel.SetupResources(interactionIndicator);
            }
        }

        public override void HideAndReset()
        {
            for (int i = 0, len = ProgressPanels.Length; i < len; i++)
                ProgressPanels[i].HideAndReset();

            if (CurrentProgressPanel != null)
                SetupResources(null);
        }

        public override void HideAllResources()
        {
            CurrentProgressPanel?.HideAllResources();
        }

        public override void HideResourcesButOne()
        {
            CurrentProgressPanel?.HideResourcesButOne();
        }

        public override void BringOneResourceToCenterAndThenHideIt(TweenCallback onEnd)
        {
            CurrentProgressPanel?.BringOneResourceToCenterAndThenHideIt(onEnd);
        }

        public override string ToString()
        {
            return name;
        }

        //=== Private =========================================================

        private ProgressPanelOneItem GetSuitablePanel([NotNull] List<ProbabilisticItemPack> expected)
        {
            int resourcesCount = Mathf.Min(MaxIndicatorsCount, expected?.Count ?? 0);
            resourcesCount = Mathf.Max(1, resourcesCount);
            return ProgressPanels[resourcesCount - 1];
        }
    }
}