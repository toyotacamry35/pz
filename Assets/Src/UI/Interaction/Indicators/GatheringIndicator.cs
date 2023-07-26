using ProcessSourceNamespace;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class GatheringIndicator : InteractionIndicator
    {
        //=== Protected ===========================================================

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