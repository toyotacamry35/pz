using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace OutlineEffect
{

[CustomEditor(typeof(OutlineEffect))]
public class OutlineEffectEditor : Editor
{
     OutlineEffect current;
     protected static Color WhiteColor { get { return Color.white; } }
      protected static Color EnableButtonColor { get { return new Color(.75f, .75f, .75f, 1f); } }
        protected enum GUIColorType { Background, Color }
        protected static GUIColorType ColorType;

        public override void OnInspectorGUI()
    {
       current = (OutlineEffect)target;

       DisplayFloatCurve(ref current.lineThickness, 0, 5f, "Line Thickness", "Толщина линий");
       DisplayFloatCurve(ref current.lineIntensity, 0, 5f, "Line Intensity", "Интенисвность линий");
       DisplayFloatCurve(ref current.fillAmount, 0, 5f, "Fill Amount", "Заливка");
        base.OnInspectorGUI();

    }

        public void DisplayFloatCurve(ref TOD.FloatCurve floatCurve, float min, float max, string name, string tooltip)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (floatCurve.use)
                {
                    floatCurve.curve = EditorGUILayout.CurveField(new GUIContent(name, tooltip), floatCurve.curve, Color.white, new Rect(0, min, 1, max));
                    ToggleButton(ref floatCurve.use, EditorStyles.miniButton);
                }
                else
                {
                    floatCurve.value = EditorGUILayout.Slider(new GUIContent(name, tooltip), floatCurve.value, min, max);
                    string tooltip02 = "Присвоить всей кривой значение " + floatCurve.value.ToString();
                    if (GUILayout.Button(new GUIContent("≡", tooltip02), EditorStyles.miniButtonLeft, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
                    {
                        if (EditorUtility.DisplayDialog("", "Присвоить всей кривой значение " + floatCurve.value.ToString() + "?", "Да", "Нет"))
                        {
                            floatCurve.curve = new AnimationCurve()
                            {
                                keys = new Keyframe[]
                                {
                                new Keyframe(0f, floatCurve.value),
                                new Keyframe(1f, floatCurve.value)
                                }
                            };

                        }
                        floatCurve.use = true;

                    }
                    tooltip02 = "Присвоить кривой в текущее время " + (current.CGTIME).ToString() + " значение " + floatCurve.value.ToString();
                    if (GUILayout.Button(new GUIContent("■", tooltip02), EditorStyles.miniButtonMid, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
                    {
                        floatCurve.curve.AddKey(new Keyframe(current.CGTIME, floatCurve.value));
                        floatCurve.use = true;
                    }
                    ToggleButton(ref floatCurve.use, EditorStyles.miniButtonRight);
                }


            }
            EditorGUILayout.EndHorizontal();
        }

        public static void ToggleButton(ref bool isToggle, GUIStyle style)
        {

            ToggleColor(isToggle, EnableButtonColor, WhiteColor, GUIColorType.Background);
            string name = (isToggle) ? "G" : "C";
            string tooltip = (isToggle) ? "значение" : "кривую";
            if (GUILayout.Button(new GUIContent(name, "Переключиться на " + tooltip), style, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
                isToggle = !isToggle;

            GUI.backgroundColor = Color.white;
        }

        protected static void ToggleColor(bool toggle, Color enableColor, Color disableColor, GUIColorType colorType)
        {
            switch (ColorType)
            {
                case GUIColorType.Background: GUI.backgroundColor = toggle ? enableColor : disableColor; break;
                case GUIColorType.Color: GUI.color = toggle ? enableColor : disableColor; break;
            }
        }
    }

}
