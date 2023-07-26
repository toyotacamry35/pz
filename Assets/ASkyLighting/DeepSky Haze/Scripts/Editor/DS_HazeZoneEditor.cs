using UnityEngine;
using UnityEditor;
using TOD;
using System.IO;

namespace DeepSky.Haze
{
    public class RectHazeComplex
    {
        public Rect labelRect;
        public Rect mainRect;
        public Rect buttonsRect;

        public RectHazeComplex(float labelSize, float buttonSize, float height)
        {
            float width = EditorGUIUtility.currentViewWidth;
            GUILayout.BeginHorizontal();
            labelRect = GUILayoutUtility.GetRect(labelSize, height);
            mainRect = GUILayoutUtility.GetRect(width - (labelSize + buttonSize + 35f), height, GUILayout.ExpandWidth(true));
            buttonsRect = GUILayoutUtility.GetRect(buttonSize, height);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }
    }

    [CustomEditor(typeof(DS_HazeZone), true)]
    public class DS_HazeZoneEditor : Editor
    {
        protected DS_HazeZone hazeZone;

        protected SerializedProperty m_ContextProp;
        protected DS_HazePresetNamePopup m_PresetNamePopup;
        protected Rect m_SavePresetRect;
        
        protected bool m_HelpTxtExpanded = false;
        protected GUIStyle headerLabelStyle;
        protected GUIStyle headerBoldLabelStyle;
        protected GUIStyle tableLabelStyle;
        protected GUIStyle buttonLeft;
        protected GUIStyle buttonRight;
        protected Texture2D helpIconImage;
        protected GUIStyle helpIconStyle;
        public virtual void OnEnable()
        {
            m_ContextProp = serializedObject.FindProperty("m_Context");
            m_PresetNamePopup = new DS_HazePresetNamePopup();
            m_PresetNamePopup.OnCreate += CreateNewHazeContextPreset;
        }

        public void HeaderLoad()
        {
            hazeZone = target as DS_HazeZone;
            hazeZone.defaultPreset = (DS_HazeContextAsset)EditorGUILayout.ObjectField("Default Preset", hazeZone.defaultPreset, typeof(DS_HazeContextAsset), false);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Create Preset", buttonLeft))
                {
                    PopupWindow.Show(m_SavePresetRect, m_PresetNamePopup);
                }
                if (Event.current.type == EventType.Repaint) m_SavePresetRect = GUILayoutUtility.GetLastRect();

                if (GUILayout.Button("Load Preset", buttonRight))
                {
                    //hazeZone.LoadFromContextPreset(hazeZone.defaultPreset);
                    
                    int ctrlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<DS_HazeContextAsset>(null, false, "", ctrlID);
                    
                }
                m_HelpTxtExpanded = EditorGUILayout.Toggle(m_HelpTxtExpanded, helpIconStyle, GUILayout.Width(helpIconImage.width));
            }
            EditorGUILayout.EndHorizontal();


            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                hazeZone.m_WaitingToLoad = EditorGUIUtility.GetObjectPickerObject() as DS_HazeContextAsset;
            }
            EditorGUILayout.Space();
        }

        public void HeaderLoadSmall()
        {
            hazeZone = target as DS_HazeZone;
            hazeZone.defaultPreset = (DS_HazeContextAsset)EditorGUILayout.ObjectField("Default Preset", hazeZone.defaultPreset, typeof(DS_HazeContextAsset), false);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Load Preset", buttonRight))
                {
                    int ctrlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                    EditorGUIUtility.ShowObjectPicker<DS_HazeContextAsset>(null, false, "", ctrlID);
                }
                m_HelpTxtExpanded = EditorGUILayout.Toggle(m_HelpTxtExpanded, helpIconStyle, GUILayout.Width(helpIconImage.width));
            }
            EditorGUILayout.EndHorizontal();


            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                hazeZone.m_WaitingToLoad = EditorGUIUtility.GetObjectPickerObject() as DS_HazeContextAsset;
            }
            EditorGUILayout.Space();
        }
        public override void OnInspectorGUI()
        {
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            headerLabelStyle = new GUIStyle(EditorStyles.label);
            headerLabelStyle.alignment = TextAnchor.MiddleLeft;

            headerBoldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
            headerBoldLabelStyle.alignment = TextAnchor.MiddleLeft;

            tableLabelStyle = new GUIStyle(EditorStyles.label);
            tableLabelStyle.alignment = TextAnchor.MiddleCenter;
            // Get the styles for the mini buttons and 'solo' toggle.
            GUIStyle variantStyle = new GUIStyle(EditorStyles.helpBox);
            variantStyle.padding = new RectOffset(0, 2, 3, 3);

            buttonLeft = GUI.skin.FindStyle("ButtonLeft");
            buttonRight = GUI.skin.FindStyle("ButtonRight");
            GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
            helpBoxStyle.richText = true;
            helpIconImage = EditorGUIUtility.FindTexture("console.infoicon.sml");
            helpIconStyle = new GUIStyle();
            helpIconStyle.normal.background = helpIconImage;
            helpIconStyle.onNormal.background = helpIconImage;
            helpIconStyle.active.background = helpIconImage;
            helpIconStyle.onActive.background = helpIconImage;
            helpIconStyle.focused.background = helpIconImage;
            helpIconStyle.onFocused.background = helpIconImage;

            serializedObject.Update();
            hazeZone = target as DS_HazeZone;

            EditorGUILayout.BeginVertical();
            {
                HeaderLoad();

                float maxLabelWidth = EditorGUIUtility.labelWidth;
                if (EditorGUIUtility.currentViewWidth < 350)
                {
                    maxLabelWidth = Mathf.Lerp(50, EditorGUIUtility.labelWidth, (1.0f / (350.0f - 275.0f)) * (EditorGUIUtility.currentViewWidth - 275.0f));
                }

                SerializedProperty nVariant = m_ContextProp.FindPropertyRelative("m_NightItem");

                Rect nRect = EditorGUILayout.BeginHorizontal(variantStyle);
                {
                    nRect.y += 5;
                    nRect.width = 50;
                                      
                    nVariant.isExpanded = EditorGUI.Foldout(nRect, nVariant.isExpanded, GUIContent.none, true, foldoutStyle);

                    EditorGUILayout.LabelField("Dark Night", EditorStyles.boldLabel, GUILayout.Height(20));
                }
                EditorGUILayout.EndHorizontal();

                

                if (nVariant.isExpanded)
                {
                    EditorGUILayout.BeginVertical(variantStyle);
                    EditorGUI.BeginChangeCheck();


                    EditorGUILayout.LabelField(new GUIContent("  Air:", "Рассеяние, вызванное молекулами воздуха"), EditorStyles.boldLabel);

                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.airScattering, new GUIContent("    Scattering", "Множитель рассеивания"), 0, 8f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.airHeightFalloff, new GUIContent("    Height Falloff", "Насколько быстро плотность воздуха уменьшается с высотой"), 0.0001f, 0.1f);


                    HazePropertyDraw(hazeZone.Context.m_NightItem, nVariant, ref hazeZone.Context.m_NightItem.fogHorizon, new GUIContent("    Horizon Color", "Оттенок рассеивания"));

                    EditorGUILayout.LabelField(new GUIContent("  Haze:", "Рассеивание, вызванное загрязнением в воздухе"), EditorStyles.boldLabel);


                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.hazeScattering, new GUIContent("    Scattering", "Множитель рассеивания"), 0, 8f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.hazeHeightFalloff, new GUIContent("    Height Falloff", "Насколько быстро плотность воздуха уменьшается с высотой"), 0.0001f, 0.1f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.hazeScatteringDir, new GUIContent("    Scatter Direction", "Значение насколько свет рассеивается вперед (положительные значения) или назад (отрицательные значения) вдоль исходного направления света. Пыль, как правило, сильно рассеяна вперед, создавая яркое свечение вокруг солнца"), -0.99f, 0.99f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.hazeScatteringRatio, new GUIContent("    Direct Ratio", "Контроль смеси между прямым и косвенным освещением. Прямой свет использует теневую карту, поэтому, чтобы она не стала слишком нереалистично темной в затененных областях, добавлено некоторое вторичное рассеяние."), 0, 1.0f);


                    EditorGUILayout.LabelField(new GUIContent("  Mist:", "Туман, основанный на высоте"), EditorStyles.boldLabel);

                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogOpacity02, new GUIContent("    Opacity", "Максимальное количество тумана затеняющего сцену, независимо от фактического исчезновения (Extinction)"), 0, 1f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogScattering, new GUIContent("    Scattering", "Множитель освещения, применяемого к туману"), 0, 8.0f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogExtinction, new GUIContent("    Extinction", "Параметр покрытия туманом"), 0, 8f);

                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogHeightFalloff, new GUIContent("    Height Falloff", "Как быстро плотность тумана уменьшается с высотой"), 0.0001f, 0.1f);


                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogDistance, new GUIContent("    Start Distance", "Расстояние начала тумана от камеры."), 0f, 1f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogHeight, new GUIContent("    Start Height"), -200f, 200f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogScatteringDir, new GUIContent("    Scatter Direction", "Насколько свет рассеивается вперед(положительные значения) или назад (отрицательные значения) вдоль исходного направления света. Более высокие значения создадут яркое свечение вокруг солнца"), -0.99f, 0.99f);

                    HazePropertyDraw(hazeZone.Context.m_NightItem, nVariant, ref hazeZone.Context.m_NightItem.fogAmbient, new GUIContent("    Ambient Color", "Базовый цвет тумана"));
                    HazePropertyDraw(hazeZone.Context.m_NightItem, nVariant, ref hazeZone.Context.m_NightItem.fogLight, new GUIContent("    Light Color", "Цвет, освещающий туман"));

                    EditorGUILayout.LabelField(new GUIContent("  Fog:", "Классический туман"), EditorStyles.boldLabel);

                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogDensity, new GUIContent("    Fog Density", "Плотность тумана."), 0.01f, 0.0001f);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, nVariant, ref hazeZone.Context.m_NightItem.fogColor, new GUIContent("    Fog Color", "Цвет тумана"));
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.fogAlpha, new GUIContent("    Fog Alpha"), 0, 1f);

                    hazeZone.Context.m_NightItem.isCave = EditorGUILayout.Toggle("    Cave", hazeZone.Context.m_NightItem.isCave);
                    HazePropertyDraw(hazeZone.Context.m_NightItem, ref hazeZone.Context.m_NightItem.caveAmbient, new GUIContent("    Cave Ambient"), 0, 1f);
                    if (EditorGUI.EndChangeCheck())
                    {

                        //hazeZone.Context.m_NightItem.UpdateCurvesFull();
                        if (Camera.main != null)
                            Camera.main.GetComponent<DS_HazeView>().SetMaterialFromContext(hazeZone.Context.m_ComplexItem);
                    }
                    EditorGUILayout.EndVertical();
                }

                

                float width = EditorGUIUtility.currentViewWidth;

                    RectHazeComplex daysHeader = new RectHazeComplex(100f, 20f, 25f);
                    DrawDayParts(daysHeader);

                    SerializedProperty cvElem = m_ContextProp.FindPropertyRelative("m_ComplexItem");
                    EditorGUI.BeginChangeCheck();


                    EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField(new GUIContent("AIR", "Рассеяние, вызванное молекулами воздуха"), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();

                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.airScattering, new GUIContent("Scattering", "Множитель рассеивания"), 0, 8f, "airScattering");
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.airHeightFalloff, new GUIContent("Height Falloff", "Насколько быстро плотность воздуха уменьшается с высотой"), 0.0001f, 0.1f, "airHeightFalloff");
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, cvElem, ref hazeZone.Context.m_ComplexItem.fogHorizon, new GUIContent("Horizon Color", "Оттенок рассеивания"), "fogHorizon.gradient");
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField(new GUIContent("HAZE", "Рассеивание, вызванное загрязнением в воздухе"), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.hazeScattering, new GUIContent("Scattering", "Множитель рассеивания"), 0, 8f, "hazeScattering");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.hazeHeightFalloff, new GUIContent("Height Falloff", "Насколько быстро плотность воздуха уменьшается с высотой"), 0.0001f, 0.1f, "hazeHeightFalloff");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.hazeScatteringDir, new GUIContent("Scatter Direction", "Значение насколько свет рассеивается вперед (положительные значения) или назад (отрицательные значения) вдоль исходного направления света. Пыль, как правило, сильно рассеяна вперед, создавая яркое свечение вокруг солнца"), -0.99f, 0.99f, "hazeScatteringDir");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.hazeScatteringRatio, new GUIContent("Direct Ratio", "Контроль смеси между прямым и косвенным освещением. Прямой свет использует теневую карту, поэтому, чтобы она не стала слишком нереалистично темной в затененных областях, добавлено некоторое вторичное рассеяние."), 0, 1.0f, "hazeScatteringRatio");
                EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField(new GUIContent("MIST", "Туман, основанный на высоте"), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogOpacity02, new GUIContent("Opacity", "Максимальное количество тумана затеняющего сцену, независимо от фактического исчезновения (Extinction)"), 0, 1f, "fogOpacity02");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogScattering, new GUIContent("Scattering", "Множитель освещения, применяемого к туману"), 0, 8.0f, "fogScattering");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogExtinction, new GUIContent("Extinction", "Параметр покрытия туманом"), 0, 8f, "fogExtinction");

                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogHeightFalloff, new GUIContent("Height Falloff", "Как быстро плотность тумана уменьшается с высотой"), 0.0001f, 0.1f, "fogHeightFalloff");

                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogDistance, new GUIContent("Start Distance", "Расстояние начала тумана от камеры."), 0f, 1f, "fogDistance");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogHeight, new GUIContent("Start Height"), -200f, 200f, "fogHeight");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogScatteringDir, new GUIContent("Scatter Direction", "Насколько свет рассеивается вперед(положительные значения) или назад (отрицательные значения) вдоль исходного направления света. Более высокие значения создадут яркое свечение вокруг солнца"), -0.99f, 0.99f, "fogScatteringDir");

                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, cvElem, ref hazeZone.Context.m_ComplexItem.fogAmbient, new GUIContent("Ambient Color", "Базовый цвет тумана"), "fogAmbient.gradient");
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, cvElem, ref hazeZone.Context.m_ComplexItem.fogLight, new GUIContent("Light Color", "Цвет, освещающий туман"), "fogLight.gradient");
                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.LabelField(new GUIContent("FOG", "Классический туман"), EditorStyles.boldLabel);
                        EditorGUILayout.EndVertical();
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogDensity, new GUIContent("Fog Density"), 0.01f, 0.0001f, "fogDensity");
                HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, cvElem, ref hazeZone.Context.m_ComplexItem.fogColor, new GUIContent("Fog Color", "Цвет тумана"), "fogColor.gradient");
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.fogAlpha, new GUIContent("Fog Alpha"), 0, 1f, "fogAlpha");

                hazeZone.Context.m_ComplexItem.isCave = EditorGUILayout.Toggle("Cave", hazeZone.Context.m_ComplexItem.isCave);
                        HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, ref hazeZone.Context.m_ComplexItem.caveAmbient, new GUIContent("Cave Ambient"), 0, 1f, "caveAmbient");
                /*
                for (int i = 0; i < hazeZone.Context.m_ComplexItem.fogLight.parts.Count; i++)
                {
                    SerializedProperty ctxVariants = m_ContextProp.FindPropertyRelative("m_ComplexItem.fogLight.parts");
                    SerializedProperty cvElem2 = ctxVariants.GetArrayElementAtIndex(i);
                    //SerializedProperty cvElem = ctxVariants.GetArrayElementAtIndex(cv);
                    HazePropertyDraw(new RectHazeComplex(100f, 20f, 20f), hazeZone.Context.m_ComplexItem, cvElem2, ref hazeZone.Context.m_ComplexItem.fogLight, new GUIContent(hazeZone.Context.m_ComplexItem.fogLight.parts[i].state.ToString(), "Цвет, освещающий туман"), "gradient");
                }
                */


                if (EditorGUI.EndChangeCheck())
                    {

                        hazeZone.Context.m_ComplexItem.UpdateCurvesFull();
                        if (Camera.main != null)
                            Camera.main.GetComponent<DS_HazeView>().SetMaterialFromContext(hazeZone.Context.m_ComplexItem);
                    }

                }
            
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();

            if (Event.current.type == EventType.Repaint && hazeZone.m_WaitingToLoad != null)
            {
                DS_HazeZone editingObject = (DS_HazeZone)target;
                editingObject.LoadFromContextPreset(hazeZone.m_WaitingToLoad);
                hazeZone.m_WaitingToLoad = null;
            }
        }


        public void DrawDayParts(RectHazeComplex rectComplex)
        {

            if (TOD.ASkyLighting._instance == null)
                return;

            TOD.DayPart[] dayParts = TOD.ASkyLighting._instance.dayParts;
            for (int i = 0; i < dayParts.Length; i++)
            {

                if (dayParts[i].percent.x < dayParts[i].percent.y)
                {
                    Rect current = new Rect(rectComplex.mainRect.x + rectComplex.mainRect.width * dayParts[i].percent.x, rectComplex.mainRect.y, (dayParts[i].percent.y - dayParts[i].percent.x) * rectComplex.mainRect.width, rectComplex.mainRect.height);
                    EditorGUI.DrawRect(current, dayParts[i].color);
                    EditorGUI.LabelField(current, dayParts[i].state.ToString(), tableLabelStyle);
                }

                else
                {
                    Rect current = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y, dayParts[i].percent.y * rectComplex.mainRect.width, rectComplex.mainRect.height);
                    EditorGUI.DrawRect(current, dayParts[i].color);
                    

                    Rect current2 = new Rect(rectComplex.mainRect.x + dayParts[i].percent.x * rectComplex.mainRect.width, rectComplex.mainRect.y, rectComplex.mainRect.width * (1 - dayParts[i].percent.x), rectComplex.mainRect.height);
                    EditorGUI.DrawRect(current2, dayParts[i].color);

                    if (current.width > current2.width)
                        EditorGUI.LabelField(current, dayParts[i].state.ToString(), tableLabelStyle);
                    else
                        EditorGUI.LabelField(current2, dayParts[i].state.ToString(), tableLabelStyle);

                }

            }

            EditorGUI.DrawRect(new Rect(rectComplex.mainRect.x + rectComplex.mainRect.width * TOD.ASkyLighting._instance.timeline / 24f, rectComplex.mainRect.y, 1, rectComplex.mainRect.height), Color.black);
        }

        public void CreateNewHazeContextPreset(string name)
        {
            DS_HazeZone editingObject = (DS_HazeZone)target;

            editingObject.Context.m_ComplexItem.nightTimePercentBaked = ASkyLighting._instance.context.nightTimePercentage;
            DS_HazeContextAsset asset = editingObject.Context.GetContextAsset();

            string[] paths = Directory.GetDirectories(Application.dataPath, "LightingPresets", SearchOption.AllDirectories);
            if (paths.Length == 0 || paths.Length > 1)
            {
                Debug.LogError("Unable to find the DeepSky Haze folder! Has it been renamed?");
                return;
            }
            int assetind = paths[0].IndexOf("Assets", 0);
            string rootpath = paths[0].Substring(assetind);
            string contextpath = rootpath + Path.DirectorySeparatorChar + "Haze";

            if (!AssetDatabase.IsValidFolder(contextpath))
            {
                AssetDatabase.CreateFolder(rootpath, "Haze");
            }

            AssetDatabase.CreateAsset(asset, contextpath + Path.DirectorySeparatorChar + name + ".asset");
            AssetDatabase.SaveAssets();

            EditorGUIUtility.PingObject(asset);
        }


        public static void SetZoneFromContextPreset(DS_HazeZone zone, DS_HazeContextAsset ctx)
        {
            if (!zone)
            {
                Debug.LogError("DeepSky::DS_HazeZoneEditor:SetZoneFromContextPreset - null zone passed!");
                return;
            }
            if (!ctx)
            {
                Debug.LogError("DeepSky::DS_HazeZoneEditor:SetZoneFromContextPreset - null ctx passed!");
                return;
            }

            zone.Context.CopyFrom(ctx.Context);
        }



        public void HazePropertyDraw(RectHazeComplex rectComplex, DS_HazeContextItem current, ref FloatCurve param, GUIContent name, float left, float right, string fieldName)
        {
            EditorGUI.LabelField(rectComplex.labelRect, name, headerLabelStyle);
            if (param.use)
            {
                Rect curveRectCorrect = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y + 1, rectComplex.mainRect.width, rectComplex.mainRect.height-2);
                param.curve = EditorGUI.CurveField(curveRectCorrect, "", param.curve);
                EditorGUI.DrawRect(new Rect(curveRectCorrect.x + curveRectCorrect.width * TOD.ASkyLighting._instance.timeline / 24f, curveRectCorrect.y + 1, 1, curveRectCorrect.height-1), Color.black);
            }
            else
            {
                Rect sliderRect = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y, rectComplex.mainRect.width - 60f, rectComplex.mainRect.height);
                param.value = EditorGUI.Slider(sliderRect, "", param.value, left, right);
                ControlCurveButtons(rectComplex.mainRect, current, ref param, name.text, fieldName);
            }

            LastButton(rectComplex.buttonsRect, current, ref param);
            
        }
        /*
        public void HazePropertyDraw(RectHazeComplex rectComplex, HazeItemEclipse current, ref float param, GUIContent name, float left, float right, string fieldName)
        {
            EditorGUI.LabelField(rectComplex.labelRect, name, headerLabelStyle);
            Rect sliderRect = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y, rectComplex.mainRect.width - 60f, rectComplex.mainRect.height);
            param = EditorGUI.Slider(sliderRect, "", param, left, right);               
        }
        */



        public void HazePropertyDraw(RectHazeComplex rectComplex, DS_HazeContextItem current, SerializedProperty ctx, ref ColorGradient param, GUIContent name, string fieldName)
        {
            EditorGUI.LabelField(rectComplex.labelRect, name, headerLabelStyle);
            if (param.use)
            {
                Rect curveRectCorrect = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y + 1, rectComplex.mainRect.width, rectComplex.mainRect.height - 2);
                SerializedProperty fogAmbient = ctx.FindPropertyRelative(fieldName);
                EditorGUI.PropertyField(curveRectCorrect, fogAmbient, new GUIContent("", ""));
                EditorGUI.DrawRect(new Rect(curveRectCorrect.x + curveRectCorrect.width * TOD.ASkyLighting._instance.timeline / 24f, curveRectCorrect.y, 1, curveRectCorrect.height), Color.black);
            }
            else
            {
                Rect sliderRect = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y+1, rectComplex.mainRect.width - 60f, rectComplex.mainRect.height-2);
                param.color = EditorGUI.ColorField(sliderRect, new GUIContent("", ""), param.color, true, false, true);
                ControlCurveButtons(rectComplex.mainRect, current, ref param, name.text, fieldName);
            }

            LastButton(rectComplex.buttonsRect, current, ref param);

            
        }
        /*
        public void HazePropertyDraw(RectHazeComplex rectComplex, HazeItemEclipse current, SerializedProperty ctx, ref Color param, GUIContent name, string fieldName)
        {
            EditorGUI.LabelField(rectComplex.labelRect, name, headerLabelStyle);

            Rect sliderRect = new Rect(rectComplex.mainRect.x, rectComplex.mainRect.y + 1, rectComplex.mainRect.width - 60f, rectComplex.mainRect.height - 2);
            param = EditorGUI.ColorField(sliderRect, new GUIContent("", ""), param, true, false, true);
        }
        */
        public void HazePropertyDraw(HazeItemEclipse current, SerializedProperty ctx, ref Color param, GUIContent name)
        {
            param = EditorGUILayout.ColorField(name, param, true, false, true);
        }
        

        public void ControlCurveButtons(Rect rect, DS_HazeContextItem current, ref ColorGradient colorGradient, string name, string fieldName)
        {
            Rect leftButton = new Rect(rect.x + rect.width - 50f, rect.y, 25f, rect.height);
            Rect rightButton = new Rect(rect.x + rect.width - 25f, rect.y, 25f, rect.height);

            if (GUI.Button(leftButton, new GUIContent("≡", "Присвоить цвет всему градиенту"), EditorStyles.miniButtonLeft))
			{
                colorGradient.ApplyToGradient();
				colorGradient.use = true;
			}

            if (GUI.Button(rightButton, new GUIContent("■", "Записать цвет в градиент"), EditorStyles.miniButtonMid))
			{
                colorGradient.gradient.AddKey(colorGradient.color, ASkyLighting.CGTime);
				colorGradient.use = true;
			}

        }
        public void ControlCurveButtons(Rect rect, DS_HazeContextItem current, ref FloatCurve floatCurve, string name, string fieldName)
        {
            Rect leftButton = new Rect(rect.x + rect.width - 50f, rect.y, 25f, rect.height);
            Rect rightButton = new Rect(rect.x + rect.width - 25f, rect.y, 25f, rect.height);

            if (GUI.Button(leftButton, new GUIContent("≡", "Присвоить значение всей кривой"), EditorStyles.miniButtonLeft))
			{
                floatCurve.ApplyToCurve();
				floatCurve.use = true;
			}

            if (GUI.Button(rightButton, new GUIContent("■", "Записать значение в кривую"), EditorStyles.miniButtonMid))
			{
                floatCurve.curve.AddKey(new Keyframe(ASkyLighting.CGTime, floatCurve.value));
				floatCurve.use = true;
			}
        }



        public void HazePropertyDraw(HazeItemEclipse current, ref float param, GUIContent name, float left, float right)
        {
            param = EditorGUILayout.Slider(name, param, left, right);
        }

        public void LastButton(Rect rect, DS_HazeContextItem current, ref FloatCurve floatCurve)
        {
            if (floatCurve.use)
                GUI.color = Color.gray;
            else
                GUI.color = Color.white;

            if (GUI.Button(rect, new GUIContent(((floatCurve.use) ? "G" : "C"), "Переключиться на " + ((floatCurve.use) ? "кривую" : "значение")), ((floatCurve.use) ? EditorStyles.miniButton : EditorStyles.miniButtonRight)))
            {
                if (floatCurve.use == true)
                {
                    floatCurve.value = floatCurve.curve.Evaluate(ASkyLighting.CGTime);
                }
                floatCurve.use = !floatCurve.use;

            }

            GUI.color = Color.white;
        }

        public void LastButton(Rect rect, DS_HazeContextItem current, ref ColorGradient colorGradient)
        {
            if (colorGradient.use)
                GUI.color = Color.gray;
            else
                GUI.color = Color.white;

            if (GUI.Button(rect, new GUIContent(((colorGradient.use) ? "G" : "C"), "Переключиться на " + ((colorGradient.use) ? "кривую" : "значение")), ((colorGradient.use) ? EditorStyles.miniButton : EditorStyles.miniButtonRight)))
            {
                if (colorGradient.use == true)
                {
                    colorGradient.color = colorGradient.gradient.Evaluate(ASkyLighting.CGTime);
                }

                colorGradient.use = !colorGradient.use;
            }

            GUI.color = Color.white;
        }
    }
}