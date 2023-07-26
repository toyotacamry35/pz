using Assets.Src.Lib.DOTweenAdds;
using JetBrains.Annotations;
using UnityEngine;

namespace WeldAdds
{
    public class TweenComponentSetter : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private TweenComponentBase _tweenComponent;

        private bool _isAfterAwake;


        //=== Props ===========================================================

        private bool _flag;

        public bool Flag
        {
            get => _flag;
            set
            {
                if (!enabled)
                    return;

                if (_flag != value)
                {
                    _flag = value;
                    if (_isAfterAwake)
                        SyncIfWoken();
                }
            }
        }

        //=== Unity ===========================================================

        private void Awake()
        {
            _tweenComponent.AssertIfNull(nameof(_tweenComponent), gameObject);
            _isAfterAwake = true;
            SyncIfWoken();
        }


        //=== Private =========================================================

        private void SyncIfWoken()
        {
            _tweenComponent.Play(Flag);
        }
    }
}