using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace WeldAdds
{
    /// <summary>
    /// Костыль для обновления Image и особенно его потомков. При любом изменении Flag или Value происходит обновление _targets
    /// отложенное хотя бы на 1 фрейм относительно изменения
    /// </summary>
    public class ImageRefresher : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private Image[] _targets;

        private bool _needForRefresh;
        private int _frameCount;


        //=== Props ==============================================================

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
                    SetRefreshFlag();
                }
            }
        }

        private float _value;

        public float Value
        {
            get => _value;
            set
            {
                if (!enabled)
                    return;

                if (!Mathf.Approximately(_value, value))
                {
                    _value = value;
                    SetRefreshFlag();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _targets.IsNullOrEmptyOrHasNullElements(nameof(_targets));
        }

        private void Update()
        {
            if (!_needForRefresh || _frameCount == Time.frameCount)
                return;

            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i]?.SetAllDirty();
            }
        }


        //=== Private =========================================================

        private void SetRefreshFlag()
        {
            _needForRefresh = true;
            _frameCount = Time.frameCount;
        }
    }
}