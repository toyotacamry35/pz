using System;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Uins
{
    public class MapIndicatorClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, UsedImplicitly]
        private MapIndicator _mapIndicator;


        //=== Unity ==============================================================

        private void Awake()
        {
            _mapIndicator.AssertIfNull(nameof(_mapIndicator));
        }


        //=== Public ==============================================================

        public void OnPointerClick(PointerEventData eventData)
        {
            bool? isLmb = null;

            switch (eventData.pointerId)
            {
                case -1: //ЛКМ
                    isLmb = true;
                    break;

                case -2: //ПКМ
                    isLmb = false;
                    break;
            }

            if (!isLmb.HasValue)
                return;

            try
            {
                _mapIndicator.OnClick(isLmb.Value);
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message(e.ToString()).Write();
            }
        }
    }
}