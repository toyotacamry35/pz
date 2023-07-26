using UnityEngine;

namespace WeldAdds
{
    public class ByRangeEnableSetter : ByRangeSetter
    {
        public MonoBehaviour[] Tragets;


        //=== Protected ===========================================================

        protected override bool SetupOnAwake()
        {
            return !Tragets.IsNullOrEmptyOrHasNullElements(nameof(Tragets));
        }

        protected override void ApplySettingsToTarget(Settings settings)
        {
            for (int i = 0, len = Tragets.Length; i < len; i++)
            {
                if (Tragets[i] == null)
                    continue;

                Tragets[i].enabled = !Mathf.Approximately(settings.Float, 0);
            }
        }
    }
}