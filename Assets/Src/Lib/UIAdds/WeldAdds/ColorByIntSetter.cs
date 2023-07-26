using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    public class ColorByIntSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Graphic Target;

        [SerializeField, UsedImplicitly]
        private Color[] Colors;

        private bool _isAfterAwake;
        
        private int _index;

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    if (_isAfterAwake && enabled)
                        SyncIfWoken();
                }
            }
        }

        private void Awake()
        {
            Target.AssertIfNull(nameof(Target), gameObject);
            _isAfterAwake = true;
            SyncIfWoken();
        }

        private void OnEnable()
        {
            SyncIfWoken();
        }

        private void SyncIfWoken()
        {
            if (Index >= 0 && Index < Colors.Length) 
                Target.color = Colors[Index];
        }
    }
}