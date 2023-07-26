using System;
using System.Collections.Generic;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using ProcessSourceNamespace;
using ReactivePropsNs;

namespace Uins
{
    public class InteractionIndicators : HasDisposablesMonoBehaviour
    {
        [NotNull]
        public PlayerInteractionViewModel PlayerInteractionViewModel;

        [NotNull]
        public InteractionIndicatorsProvider IndicatorsProvider;

        private Queue<InteractionIndicator> _indicatorsQueue = new Queue<InteractionIndicator>();

        private bool _hasOurPawn;


        //=== Props ===============================================================

        private InteractionIndicator CurrentIndicator => _indicatorsQueue.Count > 0 ? _indicatorsQueue.Peek() : null;


        //=== Unity ===============================================================

        private void Awake()
        {
            IndicatorsProvider.AssertIfNull(nameof(IndicatorsProvider));
            if (!PlayerInteractionViewModel.AssertIfNull(nameof(PlayerInteractionViewModel)))
                PlayerInteractionViewModel.NewProcessSource += OnNewProcessSource;
        }

        private void Update()
        {
            try //TODOM убрать после отлова зависания индикаторов
            {
                if (_hasOurPawn)
                {
                    if (_indicatorsQueue.Count > 0)
                    {
                        if (CurrentIndicator.VisualStage == InteractionIndicator.IndicatorVisualStage.Disappeared)
                        {
                            //Освобождаем завершившийся
                            IndicatorsProvider.ReleaseIndicator(CurrentIndicator);
                            _indicatorsQueue.Dequeue();
                            if (_indicatorsQueue.Count == 0)
                                return;
                        }

                        CurrentIndicator.VisualUpdate();
                    }
                }
                else
                {
                    if (CurrentIndicator != null)
                        IndicatorsReset();
                }
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"<{nameof(InteractionIndicators)}> Exception: {e}").Write();
            }
        }


        //=== Public ==============================================================

        public void Init(IPawnSource pawnSource)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }


        //=== Private =============================================================

        private void IndicatorsReset()
        {
            while (_indicatorsQueue.Count > 0)
            {
                IndicatorsProvider.ReleaseIndicator(CurrentIndicator);
                _indicatorsQueue.Dequeue();
            }
        }

        private void OnNewProcessSource(IProcessSource processSource)
        {
            var interactionIndicator = IndicatorsProvider.GetIndicator(processSource);
            if (interactionIndicator.AssertIfNull(nameof(interactionIndicator)))
                return;

            _indicatorsQueue.Enqueue(interactionIndicator);
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            _hasOurPawn = newEgo != null;
        }
    }
}