using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    class StateHandlerBehaviour : StatHandlerBehaviour
    {
        public GameObject VisualGameObject = null;

        // ----------------------------------------------------------------------------------------
        private void ConnectToState()
        {
            if (VisualGameObject != null)
            {
                VisualBehaviour.UpdateState(VisualGameObject, VisualBehaviour.VisualState.Constructed, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, true, 1.0f);
            }
        }

        private void SetState(float value)
        {
            if (VisualGameObject != null)
            {
                VisualBehaviour.UpdateState(VisualGameObject, (value > 0.5f) ? VisualBehaviour.VisualState.Destroyed : VisualBehaviour.VisualState.Constructed, OutlineEffect.OutlineColor.Blue, string.Empty, string.Empty, true, 1.0f);
            }
        }

        // ----------------------------------------------------------------------------------------
        protected override void OnAwakeHandler()
        {
            ConnectToState();
        }
        protected override void OnDestroyHandler() { }
        protected override void OnGotClient() { }
        protected override void OnLostClient() { }

        protected override void OnStatChanged(float newValue)
        {
            SetState(newValue);
        }
        protected override void OnSetInitialValue(float initialValue)
        {
            SetState(initialValue);
        }
        protected override void OnSetFinalValue(float finalValue)
        {
            SetState(finalValue);
        }
    }
}
