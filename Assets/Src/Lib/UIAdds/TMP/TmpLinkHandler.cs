using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Uins.Tmp
{
    /// <summary>
    /// TextMeshPro links handler
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TmpLinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private TextMeshProUGUI _textMeshProUgui;

        public static event Action<TMP_LinkInfo> LinkClick;


        //=== Unity ===========================================================

        private void Awake()
        {
            _textMeshProUgui = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textMeshProUgui, Input.mousePosition, Camera.current);

            if (linkIndex != -1)
            {
                LinkClick?.Invoke(_textMeshProUgui.textInfo.linkInfo[linkIndex]);
                //Debug.Log($"{name}: linkID='{_textMeshProUgui.textInfo.linkInfo[linkIndex].GetLinkID()}'"); //DEBUG
            }
        }
    }
}