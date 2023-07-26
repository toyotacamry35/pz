using UnityEngine;

namespace Assets.Src.Instancenator
{
    public class InstanceCompositionUpdateSignaller : MonoBehaviour
    {
        private void Awake()
        {
            InstanceCompositionDistanceManager.SetVegetationLodBias(InstanceCompositionDistanceManager.CurrentDistanceBias, immediately: true);
        }

        private void Update()
        {
            InstanceCompositionDistanceManager.UpdateDistances(Time.deltaTime);
        }
    }
}
