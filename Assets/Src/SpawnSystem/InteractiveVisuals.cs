using Assets.Src.Shared;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    class InteractiveVisuals : ColonyBehaviour
    {
        public GameObject VisualPrefab;
        public GameObject VisualInstance;

        protected override void ClientStart()
        {
            VisualInstance = Instantiate(VisualPrefab, this.transform);
        }

        private void OnDestroy()
        {
            if(VisualInstance != null)
                Destroy(VisualInstance);
        }
    }
}