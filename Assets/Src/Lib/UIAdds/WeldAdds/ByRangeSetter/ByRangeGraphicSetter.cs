using UnityEngine.UI;

namespace WeldAdds
{
    public abstract class ByRangeGraphicSetter : ByRangeSetter
    {
        public Graphic[] Tragets;

        private bool _isDisabledOptimization;


        //=== Protected ===========================================================

        protected override bool SetupOnAwake()
        {
            return !Tragets.IsNullOrEmptyOrHasNullElements(nameof(Tragets));
        }

        protected override void ApplySettingsToTarget(Settings settings)
        {
            if (!_isDisabledOptimization)
                DisableOptimization();

            for (int i = 0, len = Tragets.Length; i < len; i++)
            {
                if (Tragets[i] == null)
                    continue;

                ApplySettingsToGraphic(Tragets[i], settings);
            }
        }

        protected abstract void ApplySettingsToGraphic(Graphic graphic, Settings settings);

        private void DisableOptimization()
        {
            _isDisabledOptimization = true;

            foreach (var graphic in Tragets)
                if (graphic as Image != null)
                    (graphic as Image).DisableSpriteOptimizations();
        }
    }
}