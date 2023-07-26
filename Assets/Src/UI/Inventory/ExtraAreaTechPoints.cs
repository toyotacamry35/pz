using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SharedCode.Aspects.Science;
using UnityEngine;

namespace Uins
{
    public class ExtraAreaTechPoints : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private TechPointViewModel _techPointViewModelPrefab;

        [SerializeField, UsedImplicitly]
        private Transform _techPointsTransform;


        private List<TechPointViewModel> _techPointViewModels = new List<TechPointViewModel>();


        //=== Unity ===========================================================

        private void Awake()
        {
            _techPointViewModelPrefab.AssertIfNull(nameof(_techPointViewModelPrefab));
            _techPointsTransform.AssertIfNull(nameof(_techPointsTransform));
        }


        //=== Public ==========================================================

        public void Init(ICharacterPoints characterPoints)
        {
            var availableTechPoints = characterPoints.GetAvailableTechPoints();
            if (availableTechPoints.AssertIfNull(nameof(availableTechPoints)))
                return;

            foreach (var techPoint in availableTechPoints)
            {
                var techPointViewModel = Instantiate(_techPointViewModelPrefab, _techPointsTransform);
                if (techPointViewModel.AssertIfNull(nameof(techPointViewModel)))
                    break;

                techPointViewModel.Set(techPoint, characterPoints.GetTechPointsCount(techPoint));
                _techPointViewModels.Add(techPointViewModel);
            }

            characterPoints.TechPointsCountChanged += OnLocalTechPointsChangedCount;
        }


        //=== Private =========================================================

        private void OnLocalTechPointsChangedCount(CurrencyResource techPoint, int newCount, bool isInitial)
        {
            var techPointViewModel = _techPointViewModels.FirstOrDefault(vm => vm.TechPointDef == techPoint);
            if (!techPointViewModel.AssertIfNull(nameof(techPointViewModel)))
                techPointViewModel.Count = newCount;
        }
    }
}