using System.Linq;
using JetBrains.Annotations;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using UnityEngine;

namespace Uins
{
    public class ExtraAreaSciences : MonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private ScienceViewModel[] _scienceViewModels;

        private IGuiWindow _inventoryWindow;

        private void Awake()
        {
            _scienceViewModels.IsNullOrEmptyOrHasNullElements(nameof(_scienceViewModels));
        }

        public void Init(ICharacterPoints characterPoints, IGuiWindow inventoryWindow)
        {
            if (characterPoints.AssertIfNull(nameof(characterPoints)))
                return;

            _inventoryWindow = inventoryWindow;
            characterPoints.SciencePointsCountChanged += OnScienceCountChanged;
            foreach (var scienceDef in characterPoints.GetAvailableSciences())
                OnScienceCountChanged(scienceDef, characterPoints.GetSciencesCount(scienceDef), true);
        }

        private void OnScienceCountChanged(ScienceDef scienceDef, int count, bool isFirstTime)
        {
            //UI.CallerLog($"{scienceDef.____GetDebugRootName()}, count={count}, ift{isFirstTime.AsSign()}"); //DEBUG
            var scienceViewModel = _scienceViewModels.FirstOrDefault(svm => svm.ScienceDef == scienceDef);
            if (scienceViewModel.AssertIfNull(nameof(scienceViewModel)))
                return;

            var delta = count - scienceViewModel.Count;
            scienceViewModel.Count = count;

            if (!isFirstTime && delta > 0 && _inventoryWindow.State.Value == GuiWindowState.Closed)
            {
                CenterNotificationQueue.Instance.SendNotification(
                    new AchievedPointsNotificationInfo(new[] {new ScienceCount() {Science = scienceDef, Count = delta}}));
            }
        }
    }
}