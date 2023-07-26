using UnityEngine;

namespace WeldAdds
{
    public class ByRangeGameObjectActiveSetter : ByRangeSetter
    {
        public GameObject[] Targets;


        //=== Protected ===========================================================

        protected override bool SetupOnAwake()
        {
            return !Targets.IsNullOrEmptyOrHasNullElements(nameof(Targets), gameObject);
        }

        protected override void ApplySettingsToTarget(Settings settings)
        {
            bool isActive = !Mathf.Approximately(settings.Float, 0);
            for (int i = 0, len = Targets.Length; i < len; i++)
            {
                if (Targets[i].activeSelf != isActive)
                    Targets[i].SetActive(isActive);
            }
        }
    }
}