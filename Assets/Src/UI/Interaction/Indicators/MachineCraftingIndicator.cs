using ProcessSourceNamespace;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class MachineCraftingIndicator : InteractionIndicator
    {
        //=== Props ===============================================================

        private float _timeLeft;

        [Binding]
        public float TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                if (!Mathf.Approximately(_timeLeft, value))
                {
                    _timeLeft = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Protected ===========================================================

        protected override void Visual_AnyUpdate()
        {
            base.Visual_AnyUpdate();
            if ((int) VisualStage >= (int) IndicatorVisualStage.Appearing)
                TimeLeft = Mathf.Max(0, AlwaysCorrectProgressDuration - Time.time + LastChangesTime);
        }

        protected override void OnStateChanged(IProcessSource processSource, bool isEndProgressChanged, bool isStartProgressChanged,
            bool isProgressDurationChanged, bool isExpectedItemsChanged)
        {
            base.OnStateChanged(processSource, isEndProgressChanged, isStartProgressChanged, isProgressDurationChanged,
                isExpectedItemsChanged);

            if (IsProgressDurationChanged || IsStartProgressChanged || IsEndProgressChanged)
                ProgressSetup();
        }

        public override void Init(IProcessSource processSource)
        {
            base.Init(processSource);
            ProgressSetup();
        }
    }
}