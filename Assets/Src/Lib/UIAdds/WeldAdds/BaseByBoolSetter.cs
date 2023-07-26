using UnityEngine;

namespace WeldAdds
{
    public abstract class BaseByBoolSetter : MonoBehaviour
    {
        private bool _isAfterAwake;
        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (_flag != value)
                {
                    _flag = value;
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