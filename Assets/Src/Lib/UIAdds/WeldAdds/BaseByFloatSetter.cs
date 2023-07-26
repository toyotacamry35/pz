using System;
using UnityEngine;

namespace WeldAdds
{
    public abstract class BaseByFloatSetter : MonoBehaviour
    {
        private bool _isAfterAwake;
        private float _amount;

        public float Amount
        {
            get => _amount;
            set
            {
                if (Math.Abs(_amount - value) > float.Epsilon)
                {
                    _amount = value;
                    if (_isAfterAwake && enabled)
                        SyncIfWoken();
                }
            }
        }

        private void Awake()
        {
            Init();
            
            _isAfterAwake = true;
            SyncIfWoken();
        }
        
        private void OnEnable()
        {
            SyncIfWoken();
        }
        protected abstract void Init();

        protected abstract void SyncIfWoken();
    }
}