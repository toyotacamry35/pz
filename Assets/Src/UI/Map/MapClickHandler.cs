using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Uins
{
    public class MapClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, UsedImplicitly]
        private UserMarkers _userMarkers;

        private RectTransform _rectTransform;


        //=== Unity ==============================================================

        private void Awake()
        {
            _userMarkers.AssertIfNull(nameof(_userMarkers));
            _rectTransform = transform.GetRectTransform();
        }


        //=== Public ==============================================================

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, eventData.position, eventData.pressEventCamera, out localCursor))
            {
                UI.Logger.IfError()?.Message($"Not found localCursor mousePos={eventData.position}").Write();
                return;
            }

            bool? isLMB = null;

            switch (eventData.pointerId)
            {
                case -1: //ЛКМ
                    isLMB = true;
                    break;

                case -2: //ПКМ
                    isLMB = false;
                    break;
            }

            if (!isLMB.HasValue)
                return;

            //Предполагается, что данный компонент находится на одном объекте с Image карты
            float anchoredPosX = LinearRelation.GetY(
                new Vector4(-_rectTransform.sizeDelta.x / 2, 0, _rectTransform.sizeDelta.x / 2, 1),
                localCursor.x);
            float anchoredPosY = LinearRelation.GetY(
                new Vector4(-_rectTransform.sizeDelta.y / 2, 0, _rectTransform.sizeDelta.y / 2, 1),
                localCursor.y);

//            UI.CallerLog($"{eventData.pointerId} ({isLMB}), position={eventData.position}, localCursor={localCursor}, " +
//                         $"anchoredPos={anchoredPosX}, {anchoredPosY}");
            _userMarkers.OnMapClick(new Vector2(anchoredPosX, anchoredPosY), isLMB.Value);
        }
    }
}