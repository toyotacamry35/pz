using System.Collections.Generic;
using SharedCode.Aspects.Science;
using UnityEngine;

namespace Uins
{
    public class TechPointsChangedMessage : MonoBehaviour
    {
        private Dictionary<CurrencyResource, int> _techPoints = new Dictionary<CurrencyResource, int>();
        private IGuiWindow _inventoryWindow;


        //=== Public ==========================================================

        public void Init(ICharacterPoints characterPoints, IGuiWindow inventoryWindow)
        {
            if (characterPoints.AssertIfNull(nameof(characterPoints)) ||
                inventoryWindow.AssertIfNull(nameof(inventoryWindow)))
                return;

            characterPoints.TechPointsCountChanged += OnTechPointCountChanged;
            _inventoryWindow = inventoryWindow;
        }


        //=== Private =========================================================

        private void OnTechPointCountChanged(CurrencyResource techPointDef, int count, bool isFirstTime)
        {
            if (techPointDef.AssertIfNull(nameof(techPointDef)))
                return;

            var prevCount = _techPoints.ContainsKey(techPointDef) ? _techPoints[techPointDef] : 0;
            _techPoints[techPointDef] = count;

            if (prevCount < count && !isFirstTime && _inventoryWindow.State.Value == GuiWindowState.Closed)
            {
                var delta = count - prevCount;
                CenterNotificationQueue.Instance.SendNotification(
                    new AchievedPointsNotificationInfo(new[] {new TechPointCount() {TechPoint = techPointDef, Count = delta}}));
            }
        }
    }
}