using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Building;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public class FenceLinkBehaviour : MonoBehaviour
    {
        private FenceLinkData linkData = null;
        private DelayAndDestroyVisualTimer destroyTimer = new DelayAndDestroyVisualTimer();

        public void SetData(FenceLinkData data)
        {
            linkData = data;
        }

        public bool IsDestroyed()
        {
            return destroyTimer.IsInProgress();
        }

        public FenceLinkData GetData()
        {
            return linkData;
        }

        public bool DestroyGameObject()
        {
            VisualBehaviour.UpdateState(gameObject, VisualBehaviour.VisualState.Destroyed, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, false, linkData.FracturedChunkScale);
            destroyTimer.Set(0.0f, linkData.Def.SelfDestructTime, null, gameObject);
            return destroyTimer.IsInProgress();
        }

        void Awake()
        {
            VisualBehaviour.UpdateState(gameObject, VisualBehaviour.VisualState.Destroyed, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, false, linkData.FracturedChunkScale);
            VisualBehaviour.UpdateState(gameObject, VisualBehaviour.VisualState.Constructed, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, false, linkData.FracturedChunkScale);
        }

        void OnDestroy()
        {
            linkData = null;
        }

        void Update()
        {
            destroyTimer.Update();
        }
    }
}