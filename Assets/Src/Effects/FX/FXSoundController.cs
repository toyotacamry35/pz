using System;
using UnityEngine;

namespace Assets.Src.Effects.FX
{
    public class FXSoundController : MonoBehaviour, IFXElementController
    {
        public event Action<IFXElementController> Completed;

        [SerializeField]
        private AkEvent _akEvent;

        [SerializeField]
        private AkSwitch _akSwitch;
        public bool Init(bool force)
        {
            if (_akEvent != null && !force)
                return true;
            
            _akEvent = GetComponent<AkEvent>();
            _akSwitch = GetComponent<AkSwitch>();
            return true;
        }

        public bool Init()
        {
            return Init(false);
        }

        private void OnValidate()
        {
            Init(true);
        }

        public void Tick()
        {
        }

        public void Hide()
        {
        }

        public void HideImmediately()
        {
        }

        public void Show(FXElementParams fxElementParams)
        {
            if (fxElementParams!= null && fxElementParams.AkGameObject != null)
            {
                AkSoundEngine.SetSwitch(_akSwitch.data.GroupId, _akSwitch.data.Id, fxElementParams.AkGameObject.gameObject);
                AkSoundEngine.PostEvent(_akEvent.data.Id, fxElementParams.AkGameObject.gameObject);
            }

            OnCompleted();
        }
        
        protected virtual void OnCompleted()
        {
            Completed?.Invoke(this);
        }
    }
}