using Uins;
using UnityEngine;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace WeldAdds
{
    [Binding]
    public class PartialFillAmountAdapter : BindingViewModel
    {
        public Image Image;

        /// <summary>
        /// Желаемое значение Image.fillAmount при Amount=0
        /// </summary>
        [Range(0, 1)]
        public float MinAmount = 0;

        /// <summary>
        /// Желаемое значение Image.fillAmount при Amount=1
        /// </summary>
        [Range(0, 1)]
        public float MaxAmount = 1;

        private float _amount;
        private float _imageFillAmount;
        private LinearRelation _linearRelation;
        private bool _isAfterAwake;


        //=== Props ===============================================================

        [Binding]
        public float Amount
        {
            get => _amount;
            set
            {
                if (!Mathf.Approximately(_amount, value))
                {
                    _amount = value;
                    if (_isAfterAwake)
                        _imageFillAmount = _linearRelation.GetClampedY(_amount);
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===============================================================

        private void Awake()
        {
            if (!Image.AssertIfNull(nameof(Image)))
                Image.DisableSpriteOptimizations();

            _linearRelation = new LinearRelation(0, MinAmount, 1, MaxAmount);
            _imageFillAmount = _linearRelation.GetClampedY(_amount);
            _isAfterAwake = true;
        }

        private void Update() //TODOM избавиться от этого
        {
            if (!Mathf.Approximately(Image.fillAmount, _imageFillAmount))
                Image.fillAmount = _imageFillAmount;
        }
    }
}