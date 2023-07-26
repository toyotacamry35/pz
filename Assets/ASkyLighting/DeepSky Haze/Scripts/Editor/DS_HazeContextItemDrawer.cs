using UnityEngine;
using UnityEditor;

namespace DeepSky.Haze
{
    [CustomPropertyDrawer(typeof(DS_HazeContextItem))]
    public class DS_HazeContextItemDrawer : PropertyDrawer
    {

        private const float kControlLineCount = 3f;

        static public float expandedHeight {
            get { return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * kControlLineCount; }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isAdv = true;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.indentLevel++;

            float curY = position.y + EditorGUIUtility.singleLineHeight / 2;
            float ctrlCount = 0;
            float ctrlSizeY = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            Rect fLight = new Rect(position.x, curY + ctrlSizeY * ctrlCount++, position.width - ((isAdv) ? 25.0f : 0), EditorGUIUtility.singleLineHeight);
            Rect fLightButton = new Rect(position.x + position.width - 20.0f, curY + ctrlSizeY * (ctrlCount - 1), 20.0f, EditorGUIUtility.singleLineHeight);
            Rect fAmb = new Rect(position.x, curY + ctrlSizeY * ctrlCount++, position.width - ((isAdv) ? 25.0f : 0), EditorGUIUtility.singleLineHeight);
            Rect fAmbButton = new Rect(position.x + position.width - 20.0f, curY + ctrlSizeY * (ctrlCount - 1), 20.0f, EditorGUIUtility.singleLineHeight);
            
            SerializedProperty fogAmbientColour = property.FindPropertyRelative("fogAmbient.color");
            SerializedProperty fogAmbientColourGradient = property.FindPropertyRelative("fogAmbient.gradient");
            SerializedProperty useFogAmbientColourGradient = property.FindPropertyRelative("fogAmbient.use");

            SerializedProperty fogLightColour = property.FindPropertyRelative("fogLight.color");
            SerializedProperty fogLightColourGradient = property.FindPropertyRelative("fogLight.gradient");
            SerializedProperty useFogLightColourGradient = property.FindPropertyRelative("fogLight.use");

            GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
            popupStyle.alignment = TextAnchor.MiddleLeft;
            popupStyle.fixedWidth = 40;

           

                if (useFogLightColourGradient.boolValue && isAdv)
                    EditorGUI.PropertyField(fLight, fogLightColourGradient, new GUIContent("Fog/Mist Light Color"));
                else
                    EditorGUI.PropertyField(fLight, fogLightColour, new GUIContent("Fog/Mist Light Color"));

            if (isAdv) AC.CustomEditor.AC_CustomEditor.ToggleButtonRect(fLightButton, useFogLightColourGradient, "C");

            if (useFogAmbientColourGradient.boolValue && isAdv)
                EditorGUI.PropertyField(fAmb, fogAmbientColourGradient, new GUIContent("Fog/Mist Ambient Color"));
            else
                EditorGUI.PropertyField(fAmb, fogAmbientColour, new GUIContent("Fog/Mist Ambient Color"));

            if (isAdv) AC.CustomEditor.AC_CustomEditor.ToggleButtonRect(fAmbButton, useFogAmbientColourGradient, "C");

            

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
            
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + expandedHeight;
        }

        public void HazePropertyDraw(SerializedProperty property, ref SerializedProperty use, ref SerializedProperty curve, ref SerializedProperty prop, Rect rect, Rect rectButton, string name, float max)
        {
            if (use.boolValue)
            {
                EditorGUI.PropertyField(rect, curve, new GUIContent(name));
                curve.animationCurveValue = null;
            }
            else
            {
                EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width - 62, rect.height), prop, 0, max, name);
            }
            AC.CustomEditor.AC_CustomEditor.ToggleButtonRect(rectButton, use, "C");

            if (EditorGUI.EndChangeCheck())
                if (prop.floatValue > max)
                    prop.floatValue = max;
        }

        public void HazePropertyDraw(SerializedProperty property, ref SerializedProperty use, ref SerializedProperty curve, ref SerializedProperty prop, Rect rect, Rect rectButton, string name)
        {
            if (use.boolValue)
                EditorGUI.PropertyField(rect, curve, new GUIContent(name));
            else
            {
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 62, rect.height), prop, new GUIContent(name));
            }
            AC.CustomEditor.AC_CustomEditor.ToggleButtonRect(rectButton, use, "C");
        }
    }
}
