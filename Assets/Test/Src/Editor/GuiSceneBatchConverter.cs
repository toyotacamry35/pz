using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Src.Test
{
    public static class GuiSceneBatchConverter
    {
        private static readonly Dictionary<string, Vector2> RightSizes = new Dictionary<string, Vector2>()
        {
            {"Ammo0", new Vector2(15, 15)},
            {"/", new Vector2(5, 15)},
            {"Of", new Vector2(11, 20)},
        };

        //[MenuItem("Debug/-------------------- Doll slots text fields size check")]
        public static void TextFieldsWidthFix()
        {
            var texts = Object.FindObjectsOfType<TextMeshProUGUI>()
                .Where(tmpu => tmpu.name == "Ammo0")
                .SelectMany(tmpu => tmpu.transform.GetComponentsInChildren<TextMeshProUGUI>());

            foreach (var tmpu in texts)
            {
                Debug.Log($"Correction of wrong size: '{tmpu.transform.FullName()}': {tmpu.rectTransform.sizeDelta}");
                var sizeDelta = tmpu.rectTransform.sizeDelta;
                if (sizeDelta.x > 20 || sizeDelta.y > 20)
                {
                    SetSizeDelta(tmpu);
                }
            }
        }

        private static string FullName(this Transform tr)
        {
            if (tr == null)
            {
                return "(null)";
            }

            string trName = tr.name;
            Transform trParent = tr.parent;
            while (trParent != null)
            {
                trName = trParent.name + "/" + trName;
                trParent = trParent.parent;
            }

            return trName;
        }


        private static void SetSizeDelta(TextMeshProUGUI tmpu)
        {
            foreach (var kvp in RightSizes)
            {
                if (kvp.Key == tmpu.name)
                {
                    tmpu.rectTransform.sizeDelta = kvp.Value;
                    break;
                }
            }
        }
    }
}