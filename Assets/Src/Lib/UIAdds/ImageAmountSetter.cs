using UnityEngine;
using UnityEngine.UI;

namespace Uins
{
    /// <summary>
    /// Устанавливает Target.fillAmount в частных пределах ZeroAmountValue...OneAmountValue
    /// </summary>
    public class ImageAmountSetter : MonoBehaviour
    {
        public Image Target;

        public float ZeroAmountValue = 0;
        public float OneAmountValue = 1;

        private Vector4 _x1y1x2y2;
        private bool _isWoken;


        //=== Props ===========================================================

        private float _amount;

        public float Amount
        {
            get => _amount;
            set
            {
                if (!enabled)
                    return;

                if (!Mathf.Approximately(_amount, value))
                {
                    _amount = value;
                    if (_isWoken)
                        SyncIfWoken();
                }
            }
        }


        //=== Unity ===========================================================

        private void OnEnable()
        {
            Init();
        }

        private void Awake()
        {
            Init();
        }


        //=== Public ==========================================================

        public void SetAmount(float amount)
        {
            Amount = amount;
        }


        //=== Private =========================================================

        private void SyncIfWoken()
        {
            Target.fillAmount = LinearRelation.GetClampedY(_x1y1x2y2, _amount);
        }

        private Vector4 GetLrParams()
        {
            return new Vector4(0, ZeroAmountValue, 1, OneAmountValue);
        }

        private void Init()
        {
            if (Target.AssertIfNull(nameof(Target)))
            {
                enabled = false;
                return;
            }

            _isWoken = true;
            Target.DisableSpriteOptimizations();
            _x1y1x2y2 = GetLrParams();
            SyncIfWoken();
        }
    }
}