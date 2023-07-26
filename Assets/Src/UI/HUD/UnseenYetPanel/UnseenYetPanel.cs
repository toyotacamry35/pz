using Assets.ColonyShared.SharedCode.Aspects.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Uins
{
    public class UnseenYetPanel : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private UnseenYetIndicator _indicatorPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _indicatorsRoot;


        //=== Unity ===========================================================

        private void Awake()
        {
            _indicatorPrefab.AssertIfNull(nameof(_indicatorPrefab));
            _indicatorsRoot.AssertIfNull(nameof(_indicatorsRoot));
        }


        //=== Public ==========================================================

        public UnseenYetIndicator GetNewIndicator(UnseenYetIndicatorDef def)
        {
            if (def.AssertIfNull(nameof(def)))
                return null;

            var indicator = Instantiate(_indicatorPrefab, _indicatorsRoot);
            if (indicator.AssertIfNull(nameof(indicator)))
                return null;

            indicator.Init(def);
            return indicator;
        }
    }
}