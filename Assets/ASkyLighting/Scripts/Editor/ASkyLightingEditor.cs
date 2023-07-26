using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using TOD;
using AC.CustomEditor;
using System.IO;

public enum EditorDayPartsState
{
    None,
    Select,
    Drag
}

[CustomEditor(typeof(ASkyLighting))]
public class ASkyLightingEditor : AC_CustomEditor
{
    SerializedObject serObject;
    ASkyLighting timeOfDayManager;
    private Material _dayPart;
    private float currentWidth;
    private Texture _clockTexture;
    private Texture[] menuIcons;
    private Material _clockMat;
    private bool isCreatedSplatData = false;

    public Vector2 startOffset;
    public Vector2 endOffset;
    private Rect m_SavePresetRect;
    private ASkyLightingPresetNamePopup m_PresetNamePopup;
    private ASkyLightingContextAsset m_WaitingToLoad = null;

    private Vector2 scrollPosCloudPreset = Vector2.zero;

    //private float editPartsPercent = 0;
    public bool isDefaultPercents = false;

    public int[] planetsInterface;

    private int cloudSelectPreset = 0;

    public List<AWeatherPreset> weatherPresets = new List<AWeatherPreset>();
    //-----------------------------------------------------

    private int dayPartsSelect = -1;

    private EditorDayPartsState dayPartsState = EditorDayPartsState.None;

    private bool isOpenDayParts = false;

    protected GUISkin skin;

    #region SerializeProperties

    SerializedProperty directionalLight;
    SerializedProperty directionalLight02;

    SerializedProperty starsCubemap;

    SerializedProperty playTime;
    SerializedProperty playServerTime;
    SerializedProperty timeline;

    SerializedProperty fulltimeline;
    //-----------------------------------------------------

    SerializedProperty ambientColorGradient;
    SerializedProperty ambientSkyGradient;
    SerializedProperty ambientEquatorGradient;
    SerializedProperty ambientGroundGradient;
    SerializedProperty sunColorGradient;

    SerializedProperty fxAmbientGradient;
    SerializedProperty topSkyColorGradient;
    SerializedProperty horizonColorGradient;
    SerializedProperty horizonSkyColorGradient;
    SerializedProperty groundColorGradient;

    SerializedProperty starsRotationMode;
    SerializedProperty starsOffsets;

    SerializedProperty starsColorGradient;


    SerializedProperty skyColorGradient;
    SerializedProperty sunHighlightColorGradient;
    SerializedProperty moonHighlightColorGradient;

    #endregion


    GUIStyle textTitleStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;

            return style;
        }
    }

    GUIStyle miniTextStyle
    {
        get
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 8;

            return style;
        }
    }


    void Clock(Rect rect)
    {
        //Rect rect = GUILayoutUtility.GetRect (100f, 100f);
        //rect.width = 100;
        GUI.DrawTexture(rect, _clockTexture);

        float correctedTimeline = timeline.floatValue;
        correctedTimeline -= 6.0f;
        float x;
        float z;

        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            _clockMat.SetPass(0);

            GL.Begin(GL.TRIANGLES);
            GL.Color(Color.black);
            float segments = (correctedTimeline / 24f) * 120;

            for (float i = -30; i < segments; i += 1.0f)
            {
                GL.Color(new Color(0f, 0f, 0.4f, 0.25f));
                GL.Vertex3(50, 50, 0);
                float theta = 2.0f * 3.1415926f * i / 120;


                x = 49f * Mathf.Cos(theta);
                z = 49f * Mathf.Sin(theta);
                GL.Color(new Color(0f, 0f, 0.4f, 0.25f));
                GL.Vertex3(x + 50, z + 50, 0);

                float theta2 = 2.0f * 3.1415926f * (i + 1) / 120;

                x = 49f * Mathf.Cos(theta2);
                z = 49f * Mathf.Sin(theta2);
                GL.Color(new Color(0f, 0f, 0.4f, 0.25f));
                GL.Vertex3(x + 50, z + 50, 0);
            }

            Repaint();

            //GL.Vertex3 (49, 0, 0);
            GL.PopMatrix();
            GL.End();
            GUI.EndClip();
        }

        Vector2 mousePos = Event.current.mousePosition;
        Vector2 localPos;
        localPos.x = mousePos.x - rect.x;
        localPos.y = mousePos.y - rect.y;

        Vector2 normal = new Vector2(50f, 50f) - localPos;
        if (normal.magnitude < 50.0f)
        {
            normal.Normalize();
            float angle = GetAngle(0, 1, normal.x, normal.y);
            if (angle < 0)
                angle = (180.0f - Mathf.Abs(angle)) + 180.0f;

            //Debug.Log (angle / 15);

            if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
            {
                timeline.floatValue = angle / 15;
                int days = Mathf.FloorToInt(fulltimeline.floatValue / 24);
                fulltimeline.floatValue = (days + timeline.floatValue / 24) * 24;
            }
        }
    }

    public static float GetAngle(float Ax, float Ay, float Bx, float By)
    {
        float result;
        result = Mathf.Atan2(Ax * By - Bx * Ay, Ax * Bx + Ay * By);
        result *= Mathf.Rad2Deg;
        return result;
    }
    //	Color WhiteColor { get { return Color.white; } }

    void OnEnable()
    {
        timeOfDayManager = (ASkyLighting) target;

        Shader shader = Shader.Find("Hidden/Internal-Colored");
        _dayPart = new Material(shader);
        _dayPart.hideFlags = HideFlags.HideAndDontSave;

        _dayPart.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        _dayPart.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

        _dayPart.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
        _dayPart.SetInt("_ZWrite", 0);

        if (_clockTexture == null)
            _clockTexture = AssetDatabase.LoadAssetAtPath("Assets/ASkyLighting/Editor/clock.png", typeof(Texture)) as Texture;
        if (menuIcons == null) menuIcons = new Texture[9];
        for (int i = 0; i < 9; i++)
            if (menuIcons[i] == null)
                menuIcons[i] =
                    AssetDatabase.LoadAssetAtPath("Assets/ASkyLighting/Editor/" + i.ToString() + ".png", typeof(Texture)) as Texture;
        timeline = serializedObject.FindProperty("Timeline");

        Shader shaderClock = Shader.Find("Hidden/Internal-Colored");
        _clockMat = new Material(shaderClock);
        _clockMat.hideFlags = HideFlags.HideAndDontSave;
        _clockMat.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        _clockMat.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _clockMat.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
        _clockMat.SetInt("_ZWrite", 0);

        planetsInterface = new int[timeOfDayManager.context.sattellites.Count];


        serObject = new SerializedObject(target);

        directionalLight = serObject.FindProperty("m_DirectionalLight");
        directionalLight02 = serObject.FindProperty("m_DirectionalLight02");
        starsCubemap = serObject.FindProperty("starsCubemap");

        playTime = serObject.FindProperty("playTime");
        playServerTime = serObject.FindProperty("useServerTime");

        timeline = serObject.FindProperty("_timeline");
        fulltimeline = serObject.FindProperty("fullTimeline");
        sunColorGradient = serObject.FindProperty("context.sunColor.gradient");

        topSkyColorGradient = serObject.FindProperty("context.topSky.gradient");
        horizonSkyColorGradient = serObject.FindProperty("context.horizonSky.gradient");
        ambientColorGradient = serObject.FindProperty("context.ambientColor.gradient");
        ambientSkyGradient = serObject.FindProperty("context.ambientSky.gradient");
        ambientEquatorGradient = serObject.FindProperty("context.ambientEquator.gradient");
        ambientGroundGradient = serObject.FindProperty("context.ambientGround.gradient");
        groundColorGradient = serObject.FindProperty("context.groundColor.gradient");

        starsOffsets = serObject.FindProperty("context.starsOffsets");

        starsColorGradient = serObject.FindProperty("context.starsColor.gradient");


        skyColorGradient = serObject.FindProperty("context.weather.cloudSystem.cloudsSettings.skyColor.gradient");
        sunHighlightColorGradient = serObject.FindProperty("context.weather.cloudSystem.cloudsSettings.sunHighlight.gradient");
        moonHighlightColorGradient = serObject.FindProperty("context.weather.cloudSystem.cloudsSettings.moonHighlight.gradient");

        m_PresetNamePopup = new ASkyLightingPresetNamePopup();
        m_PresetNamePopup.OnCreate += CreateSkyLightingPreset;
    }

    public override void OnInspectorGUI()
    {
        timeOfDayManager = (ASkyLighting) target;
        serObject.Update();

        if (skin == null)
            skin = AssetDatabase.LoadAssetAtPath("Assets/ASkyLighting/Editor/timeOfDaySkin.guiskin", typeof(GUISkin)) as GUISkin;


        currentWidth = EditorGUIUtility.currentViewWidth;
        Rect main = GUILayoutUtility.GetRect(currentWidth, 115f);
        Rect rect = new Rect(main.x, main.y, 100f, 100f);
        rect.width = 100;
        rect.y += 5.0f;

        Clock(rect);

        Rect rect02 = rect;
        rect02.x += 110f;
        rect02.width = currentWidth - 130.0f;


        EditorGUILayout.Separator();
        //---------------------------------------------
        Header(rect02);

        timeOfDayManager.defaultContext = (ASkyLightingContextAsset) EditorGUILayout.ObjectField(
            "Default Preset ",
            timeOfDayManager.defaultContext,
            typeof(ASkyLightingContextAsset),
            false,
            GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        {
            if (GUILayout.Button("Create Preset", GUI.skin.FindStyle("ButtonLeft")))
            {
                PopupWindow.Show(m_SavePresetRect, m_PresetNamePopup);
            }

            if (Event.current.type == EventType.Repaint) m_SavePresetRect = GUILayoutUtility.GetLastRect();

            if (GUILayout.Button("Load Preset", GUI.skin.FindStyle("ButtonRight")))
            {
                int ctrlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<ASkyLightingContextAsset>(null, false, "", ctrlID);
            }
        }
        EditorGUILayout.EndHorizontal();
        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            m_WaitingToLoad = EditorGUIUtility.GetObjectPickerObject() as ASkyLightingContextAsset;
        }



        if (timeOfDayManager.isDayPartSettings)
        {
            DayElements();
        }

        if (timeOfDayManager.isAtmosphereSettings)
            Atmosphere();
        if (timeOfDayManager.isSunSettings)
            Sun();
        if (timeOfDayManager.isSatellites)
            Satellites();
        if (timeOfDayManager.isStarsSettings)
            Stars();
        if (timeOfDayManager.isOtherSettings)
            Clouds();
        if (timeOfDayManager.isResourcesSettings)
            ResourcesAndComponents();
        if (timeOfDayManager.isPresetSettings)
            Presets();

        //WorldAndTime();
        Rect rect03 = rect;
        rect03.x += 110f;
        rect03.y += 70f;
        rect03.width = currentWidth - 155.0f;
        rect03.height = 37f;
        Cockpit(rect03);

        serObject.ApplyModifiedProperties();

        if (Event.current.type == EventType.Repaint && m_WaitingToLoad != null)
        {
            timeOfDayManager.LoadFromContextPreset(m_WaitingToLoad);
            m_WaitingToLoad = null;
        }
    }



    public void CreateSkyLightingPreset(string name)
    {
        ASkyLightingContextAsset asset = timeOfDayManager.context.GetContextAsset();

        string[] paths = Directory.GetDirectories(Application.dataPath, "LightingPresets", SearchOption.AllDirectories);
        if (paths.Length == 0 || paths.Length > 1)
        {
            Debug.LogError("Unable to find the folder! Has it been renamed?");
            return;
        }

        int assetind = paths[0].IndexOf("Assets", 0);
        string rootpath = paths[0].Substring(assetind);
        string contextpath = rootpath + Path.DirectorySeparatorChar + "Sky";

        if (!AssetDatabase.IsValidFolder(contextpath))
        {
            AssetDatabase.CreateFolder(rootpath, "Sky");
        }

        AssetDatabase.CreateAsset(asset, contextpath + Path.DirectorySeparatorChar + name + ".asset");
        AssetDatabase.SaveAssets();

        EditorGUIUtility.PingObject(asset);
    }


    void Cockpit(Rect rect)
    {
        GUI.Box(rect, "");

        GUI.Label(new Rect(rect.x, rect.y + 2f, 80, 15.0f), "Server Time");
        playServerTime.boolValue = EditorGUI.Toggle(new Rect(rect.x + 95.0f, rect.y, 45, 15.0f), playServerTime.boolValue);

        GUI.Label(new Rect(rect.x + 120, rect.y + 2f, 60, 15.0f), "Play Time");
        playTime.boolValue = EditorGUI.Toggle(new Rect(rect.x + 195.0f, rect.y, 45, 15.0f), playTime.boolValue);

        GUI.Label(new Rect(rect.x + 220, rect.y + 2f, 120, 15.0f), "Day in Seconds");
        timeOfDayManager.dayInSeconds = EditorGUI.FloatField(
            new Rect(rect.x + 340, rect.y + 2f, rect.width - 345, 15.0f),
            timeOfDayManager.dayInSeconds);


        //GUI.Label(new Rect(rect.x, rect.y + 17f, 120, 15.0f), "Timeline [" + (timeOfDayManager.timeline / 24).ToString("F2") + "]");
        // timeOfDayManager.timeline = EditorGUI.FloatField(new Rect(rect.x + 120f, rect.y + 17f, rect.width - 120f, 15.0f), timeOfDayManager.timeline);

        EditorGUI.BeginChangeCheck();

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y + 18f, rect.width - 5, 15.0f),
            timeline,
            new GUIContent(
                " Timeline [" +
                (timeOfDayManager.timeline / 24).ToString("F3") +
                "]" +
                " | Full [" +
                (timeOfDayManager.fullTimeline / 24).ToString("F3") +
                "]"));

        if (EditorGUI.EndChangeCheck())
        {
            int days = Mathf.FloorToInt(fulltimeline.floatValue / 24);
            fulltimeline.floatValue = (days + timeline.floatValue / 24) * 24;
        }
    }

    void Header(Rect rect)
    {
        Color guiColor = GUI.color;

        float widthButton = rect.width / 4 - 10.0f;
        float heigtButton = rect.height / 3 - 5.0f;

        GUI.color = (timeOfDayManager.isDayPartSettings) ? Color.gray : Color.white;
        if (GUI.Button(new Rect(rect.x, rect.y, widthButton, heigtButton), new GUIContent("Day Parts", menuIcons[0], "Day Parts")))
            timeOfDayManager.isDayPartSettings = !timeOfDayManager.isDayPartSettings;
        GUI.color = (timeOfDayManager.isAtmosphereSettings) ? Color.gray : Color.white;
        if (GUI.Button(
            new Rect(rect.x + widthButton + 5.0f, rect.y, widthButton, heigtButton),
            new GUIContent("Atmosphere", menuIcons[1], "Atmosphere")))
            timeOfDayManager.isAtmosphereSettings = !timeOfDayManager.isAtmosphereSettings;
        GUI.color = (timeOfDayManager.isSunSettings) ? Color.gray : Color.white;
        if (GUI.Button(
            new Rect(rect.x + (widthButton + 5.0f) * 2, rect.y, widthButton, heigtButton),
            new GUIContent("Sun", menuIcons[2], "Sun")))
            timeOfDayManager.isSunSettings = !timeOfDayManager.isSunSettings;
        GUI.color = (timeOfDayManager.isOtherSettings) ? Color.gray : Color.white;
        if (GUI.Button(
            new Rect(rect.x, rect.y + heigtButton + 5.0f, widthButton, heigtButton),
            new GUIContent("Clouds", menuIcons[3], "Clouds")))
            timeOfDayManager.isOtherSettings = !timeOfDayManager.isOtherSettings;
        GUI.color = (timeOfDayManager.isStarsSettings) ? Color.gray : Color.white;
        if (GUI.Button(
            new Rect(rect.x + widthButton + 5.0f, rect.y + heigtButton + 5.0f, widthButton, heigtButton),
            new GUIContent("Stars", menuIcons[6], "Stars")))
            timeOfDayManager.isStarsSettings = !timeOfDayManager.isStarsSettings;
        GUI.color = (timeOfDayManager.isSatellites) ? Color.gray : Color.white;
        if (GUI.Button(
            new Rect(rect.x + (widthButton + 5.0f) * 3, rect.y, widthButton, heigtButton),
            new GUIContent("Satellites", menuIcons[4], "Satellites")))
            timeOfDayManager.isSatellites = !timeOfDayManager.isSatellites;
        GUI.color = (timeOfDayManager.isResourcesSettings) ? Color.gray : Color.white;
        if (GUI.Button(
            new Rect(rect.x + (widthButton + 5.0f) * 2, rect.y + heigtButton + 5.0f, widthButton, heigtButton),
            new GUIContent("Resources", menuIcons[7], "Resources")))
            timeOfDayManager.isResourcesSettings = !timeOfDayManager.isResourcesSettings;

        GUI.color = guiColor;
    }

    void DayElementSelect(float pos, float widthRect, ref int editSelect, ref DayPart[] dayElement, bool isDayParts)
    {
        editSelect = -1;
        float min = 9999f;
        for (int i = 0; i < dayElement.Length; i++)
        {
            float currentBorder = Mathf.Abs(dayElement[i].percent.x * widthRect - pos);
            if (min > currentBorder)
            {
                min = currentBorder;
                editSelect = i;
            }
        }
    }



    void DayElements()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("DayParts", EditorStyles.boldLabel);
        timeOfDayManager.isDebug = GUILayout.Toggle(timeOfDayManager.isDebug, new GUIContent("Debug"));
        GUILayout.EndHorizontal();
        GUILayout.Space(1);
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);

        DayParts(55f, 50f, ref dayPartsSelect, ref dayPartsState, timeOfDayManager.dayParts, true, ref isOpenDayParts);
        //DayParts(30f, ref skyBhvrSelect, ref skyBhvrState, timeOfDayManager.skyObjectBehaviours, false, ref isOpenSkyBhvrs);

        GUILayout.EndVertical();
    }


    void DayParts(
        float height,
        float fullHeight,
        ref int editSelect,
        ref EditorDayPartsState editorState,
        DayPart[] dayElement,
        bool isDayParts,
        ref bool isOpen)
    {
        float widthRect = currentWidth - 65.0f;
        float timePos = widthRect * timeline.floatValue / 24f;

        Rect bigRect = GUILayoutUtility.GetRect(widthRect, height + fullHeight);
        Rect rect = new Rect(bigRect.x, bigRect.y + fullHeight, bigRect.width, height);
        float currentHeight = height - 5f;
        Vector2 mousePos = Event.current.mousePosition;
        Vector2 pos = new Vector2(mousePos.x - rect.x, mousePos.y - rect.y);

        if (pos.x > 0 && pos.x < rect.width && pos.y > 0 && pos.y < currentHeight)
        {
            switch (editorState)
            {
                case EditorDayPartsState.None:
                    if (Event.current.type == EventType.MouseDown)
                    {
                        editorState = EditorDayPartsState.Select;
                        isCreatedSplatData = true;
                        timeOfDayManager.hazeCore.mainPreset.CreateSplit(timeOfDayManager.dayParts);
                        timeOfDayManager.context.CreateSplitData(timeOfDayManager.dayParts);
                        DayElementSelect(pos.x, widthRect, ref editSelect, ref dayElement, isDayParts);
                    }

                    break;
                case EditorDayPartsState.Select:
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        if (pos.x > (dayElement[editSelect].percent.x * widthRect - 8.0f) &&
                            pos.x < (dayElement[editSelect].percent.x * widthRect + 8.0f))
                        {
                            isCreatedSplatData = true;
                            timeOfDayManager.hazeCore.mainPreset.CreateSplit(timeOfDayManager.dayParts);
                            timeOfDayManager.context.CreateSplitData(timeOfDayManager.dayParts);
                            editorState = EditorDayPartsState.Drag;
                        }
                        else
                        {
                            if (pos.x < rect.width - 24f)
                            {
                                isCreatedSplatData = true;
                                timeOfDayManager.hazeCore.mainPreset.CreateSplit(timeOfDayManager.dayParts);
                                timeOfDayManager.context.CreateSplitData(timeOfDayManager.dayParts);
                            }

                            DayElementSelect(pos.x, widthRect, ref editSelect, ref dayElement, isDayParts);
                        }
                    }

                    if (Event.current.button != 0)
                        editorState = EditorDayPartsState.None;
                }
                    break;
                case EditorDayPartsState.Drag:
                {
                    int prev = (editSelect > 0) ? editSelect - 1 : dayElement.Length - 1;
                    int next = (editSelect < (dayElement.Length - 1)) ? editSelect + 1 : 0;

                    float prevfloat = (dayElement[prev].percent.x >= dayElement[prev].percent.y)
                        ? 0
                        : dayElement[prev].percent.x * widthRect;
                    float nextfloat = (dayElement[editSelect].percent.x >= dayElement[editSelect].percent.y)
                        ? widthRect
                        : dayElement[next].percent.x * widthRect;


                    if (pos.x > prevfloat + 4.0f && pos.x < nextfloat - 4.0f)
                    {
                        dayElement[editSelect].percent.x = pos.x / widthRect;
                        dayElement[prev].percent.y = pos.x / widthRect;
                    }
                }
                    break;
            }
        }

        if (pos.x > 0 && pos.x < rect.width && pos.y > currentHeight + 5 && pos.y < currentHeight + 20)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                isCreatedSplatData = true;
                timeOfDayManager.hazeCore.mainPreset.CreateSplit(timeOfDayManager.dayParts);
                timeOfDayManager.context.CreateSplitData(timeOfDayManager.dayParts);
            }
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && isCreatedSplatData)
        {
            isCreatedSplatData = false;
            timeOfDayManager.hazeCore.mainPreset.Merge(timeOfDayManager.dayParts);
            timeOfDayManager.context.MergeSplitData(timeOfDayManager.dayParts);
        }

        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            _dayPart.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.Color(Color.black);

            for (int i = 0; i < dayElement.Length; i++)
                DrawDayElement(dayElement[i], widthRect, currentHeight);

            float center = widthRect * 0.5f;

            float morningSunTime = center * timeOfDayManager.context.nightTimePercentage;
            float morningStartTime = morningSunTime; // - maxSunSize * widthRect / 5f;

            float eveningSunTime = widthRect - morningSunTime;
            float eveningEndTime = eveningSunTime; // + maxSunSize * widthRect / 5f;

            Vector2 upDownBorder = new Vector2(8f, -5f);



            DrawRectDayTime(timePos, 1.0f, Vector2.zero, currentHeight, 0, Color.black);

            if (editorState != EditorDayPartsState.None)
                DrawRectDayTime(
                    dayElement[editSelect].percent.x * widthRect - 2.0f,
                    4.0f,
                    Vector2.zero,
                    currentHeight,
                    0,
                    (editorState == EditorDayPartsState.Select) ? Color.gray : Color.black);

            GL.End();



            GL.PopMatrix();
            GUI.EndClip();

            GUI.BeginClip(bigRect);
            GL.PushMatrix();

            DrawDotDayTime(morningStartTime, 5f, eveningEndTime - morningStartTime, fullHeight - 5f, Color.white);

            GL.Begin(GL.QUADS);
            DrawRectDayTime(morningStartTime, 1.0f, upDownBorder, fullHeight + currentHeight, 0, Color.gray);
            DrawRectDayTime(eveningEndTime, 1.0f, upDownBorder, fullHeight + currentHeight, 0, Color.gray);
            DrawRectDayTime(center, 1.0f, upDownBorder, fullHeight + currentHeight, 0, Color.gray);
            GL.End();

            startOffset = new Vector2(0.25f, 0.75f);
            endOffset = new Vector2(0.5f, 0);

            upDownBorder = new Vector2(0f, 5f);

            DrawBezierDayTime(
                morningStartTime,
                center,
                startOffset,
                endOffset,
                upDownBorder,
                widthRect,
                fullHeight,
                Mathf.Abs(timeOfDayManager.context.worldLatitude / 100f),
                timePos,
                Color.gray);

            DrawBezierDayTime(
                eveningEndTime,
                center,
                startOffset,
                endOffset,
                upDownBorder,
                widthRect,
                fullHeight,
                Mathf.Abs(timeOfDayManager.context.worldLatitude / 100f),
                timeline.floatValue / 24f,
                Color.gray);
            //// GL.Color(Color.gray);
            // DrawBezierDayTime(eveningEndTime, center, startOffset, -0.15f * widthRect, upDownBorder, currentHeight * (90f - timeOfDayManager.context.worldLatitude));
            // GL.End();

            Repaint();

            GL.PopMatrix();

            GUI.EndClip();
        }



        for (int i = 0; i < dayElement.Length; i++)
        {
            GUIContent content = new GUIContent(dayElement[i].state.ToString());
            GUIStyle style = GUI.skin.label;
            style.alignment = TextAnchor.MiddleCenter;

            Rect currentRect;
            if (dayElement[i].percent.x > dayElement[i].percent.y)
            {
                if (dayElement[i].percent.x > 1.0f - dayElement[i].percent.y)
                    currentRect = new Rect(rect.x, rect.y, dayElement[i].percent.y * widthRect, currentHeight);
                else
                    currentRect = new Rect(
                        rect.x + dayElement[i].percent.x * widthRect,
                        rect.y,
                        (1.0f - dayElement[i].percent.x) * widthRect,
                        currentHeight);
            }
            else
                currentRect = new Rect(
                    rect.x + dayElement[i].percent.x * widthRect,
                    rect.y,
                    (dayElement[i].percent.y - dayElement[i].percent.x) * widthRect,
                    currentHeight);

            GUI.Label(currentRect, content, style);
        }

        if (GUI.Button(new Rect(rect.x + widthRect + 1f, rect.y, 20f, currentHeight), (isOpen) ? "►" : "▼"))
            isOpen = !isOpen;

        if (isOpen)
        {
            for (int i = 0; i < dayElement.Length; i++)
            {
                GUILayout.BeginHorizontal();
                dayElement[i].state = (DayName) EditorGUILayout.EnumPopup(dayElement[i].state, GUILayout.ExpandWidth(true));
                if (!isDefaultPercents)
                    dayElement[i].percent = EditorGUILayout.Vector2Field("", dayElement[i].percent);
                else
                    dayElement[i].defaultPercent = EditorGUILayout.Vector2Field("", dayElement[i].defaultPercent);
                dayElement[i].color = EditorGUILayout.ColorField(dayElement[i].color);
                dayElement[i].reflections = (Cubemap) EditorGUILayout.ObjectField(dayElement[i].reflections, typeof(Cubemap), false);
                GUILayout.EndHorizontal();
            }

            timeOfDayManager.context.nightTimePercentage = EditorGUILayout.Slider(
                new GUIContent(
                    "Night Time: [" + timeOfDayManager.context.nightTimePercentage * 100 + "%] of Full Day",
                    "Процент ночного времени"),
                timeOfDayManager.context.nightTimePercentage,
                0.1f,
                0.9f);
            GUILayout.BeginHorizontal();

            GUILayout.Label("Manual Data Stretch");

            if (GUILayout.Button("Split Data"))
            {
                isCreatedSplatData = true;
                timeOfDayManager.hazeCore.mainPreset.CreateSplit(timeOfDayManager.dayParts);
                timeOfDayManager.context.CreateSplitData(timeOfDayManager.dayParts);
            }

            if (timeOfDayManager.context.topSky.parts == null)
                GUI.enabled = false;
            else
                GUI.enabled = true;
            if (GUILayout.Button("Merge Data"))
            {
                isCreatedSplatData = false;
                timeOfDayManager.hazeCore.mainPreset.Merge(timeOfDayManager.dayParts);
                timeOfDayManager.context.MergeSplitData(timeOfDayManager.dayParts);
            }

            if (GUILayout.Button((isDefaultPercents) ? "Default Params" : "Modified Params"))
            {
                isDefaultPercents = !isDefaultPercents;
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            timeOfDayManager.context.nightTimePercentage = EditorGUILayout.Slider(
                new GUIContent(
                    "Night Time: [" + timeOfDayManager.context.nightTimePercentage * 100 + "%] of Full Day",
                    "Процент ночного времени"),
                timeOfDayManager.context.nightTimePercentage,
                0f,
                1f);
            if (EditorGUI.EndChangeCheck())
                ASkyLighting.StretchDayPart(ref timeOfDayManager.dayParts, timeOfDayManager.context.nightTimePercentage);
        }
    }



    void DrawBezierDayTime(
        float start,
        float end,
        Vector2 startOffset,
        Vector2 leftOffset,
        Vector2 offset,
        float currentWidth,
        float currentHeight,
        float latutude,
        float timePosition,
        Color color)
    {
        float x = currentHeight + offset.x;
        float y = currentHeight + offset.x - (currentHeight + offset.x) * latutude + offset.y * latutude;

        GL.Begin(GL.LINE_STRIP);
        GL.Color(color);

        for (float i = 0; i <= 1f; i += 0.05f)
        {
            Vector2 last = CubicBezier(
                new Vector2(start, x),
                new Vector2(Mathf.Lerp(start, end, startOffset.x), Mathf.Lerp(x, y, startOffset.y)),
                new Vector2(Mathf.Lerp(end, start, leftOffset.x), Mathf.Lerp(y, x, leftOffset.y)),
                new Vector2(end, y),
                i);
            GL.Vertex3(last.x, last.y, 0);
        }

        Vector2 final = CubicBezier(
            new Vector2(start, x),
            new Vector2(Mathf.Lerp(start, end, startOffset.x), Mathf.Lerp(x, y, startOffset.y)),
            new Vector2(Mathf.Lerp(end, start, leftOffset.x), Mathf.Lerp(y, x, leftOffset.y)),
            new Vector2(end, y),
            1f);
        GL.Vertex3(final.x, final.y, 0);

        GL.End();
    }

    void DrawRectDayTime(float time, float width, Vector2 offset, float currentHeight, float heightThis, Color color)
    {
        GL.Color(color);
        GL.Vertex3(time, offset.y, 0);
        GL.Vertex3(time + width, offset.y + heightThis, 0);
        GL.Vertex3(time + width, currentHeight + offset.x + heightThis, 0);
        GL.Vertex3(time, currentHeight + offset.x + heightThis, 0);
    }

    void DrawDotDayTime(float time, float top, float sizex, float sizey, Color color)
    {
        GL.Begin(GL.QUADS);
        GL.Color(color);
        GL.Vertex3(time, top, 0);
        GL.Vertex3(time + sizex, top, 0);
        GL.Vertex3(time + sizex, top + sizey, 0);
        GL.Vertex3(time, top + sizey, 0);
        GL.End();
    }

    public static Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return p0 * (omt2 * omt) +
               p1 * (3f * omt2 * t) +
               p2 * (3f * omt * t2) +
               p3 * (t2 * t);
    }

    void DrawDayElement(DayPart _dayPart, float _width, float _height)
    {
        GL.Color(_dayPart.color);
        if (_dayPart.percent.x > _dayPart.percent.y)
        {
            GL.Vertex3(_width * _dayPart.percent.x, 0, 0);
            GL.Vertex3(_width * _dayPart.percent.x, _height, 0);
            GL.Vertex3(_width, _height, 0);
            GL.Vertex3(_width, 0, 0);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, _height, 0);
            GL.Vertex3(_width * _dayPart.percent.y, _height, 0);
            GL.Vertex3(_width * _dayPart.percent.y, 0, 0);
        }
        else
        {
            GL.Vertex3(_width * _dayPart.percent.x, 0, 0);
            GL.Vertex3(_width * _dayPart.percent.x, _height, 0);
            GL.Vertex3(_width * _dayPart.percent.y, _height, 0);
            GL.Vertex3(_width * _dayPart.percent.y, 0, 0);
        }
    }

    void ResourcesAndComponents()
    {
        GUILayout.Label("Resources", EditorStyles.boldLabel);
        GUILayout.Space(1);
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(directionalLight, new GUIContent("Directional Light"));
        EditorGUILayout.PropertyField(directionalLight02, new GUIContent("Support Light"));

        timeOfDayManager.skyboxSource = (Material) EditorGUILayout.ObjectField(
            "Skybox Source Material",
            timeOfDayManager.skyboxSource,
            typeof(Material),
            false);
        timeOfDayManager.skyboxRendering = (Material) EditorGUILayout.ObjectField(
            "Finalize Source Material",
            timeOfDayManager.skyboxRendering,
            typeof(Material),
            false);
        timeOfDayManager.skyboxClouds = (Material) EditorGUILayout.ObjectField(
            "Clouds Source Material",
            timeOfDayManager.skyboxClouds,
            typeof(Material),
            false);
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical(GUILayout.Width(100));
        //EditorGUILayout.LabelField("Stars Cubemap");
        starsCubemap.objectReferenceValue = (Cubemap) EditorGUILayout.ObjectField(
            "",
            starsCubemap.objectReferenceValue,
            typeof(Cubemap),
            true,
            GUILayout.Width(100));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }


    void Atmosphere()
    {
        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Atmosphere", EditorStyles.boldLabel);

        bool oldUseHaze = timeOfDayManager.useHaze;
        timeOfDayManager.useHaze = GUILayout.Toggle(timeOfDayManager.useHaze, new GUIContent("Use Haze"));
        if (oldUseHaze != timeOfDayManager.useHaze)
            SwitchHaze();
        GUILayout.EndHorizontal();
        GUILayout.Space(1);
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);

        GUILayout.Label("Atmosphere:", EditorStyles.boldLabel);
        DisplayColorGradient(ref timeOfDayManager.context.topSky, topSkyColorGradient, "Top Sky Color", "Это цвет неба в зените (♪)");
        DisplayColorGradient(
            ref timeOfDayManager.context.horizonSky,
            horizonSkyColorGradient,
            "Horizon Sky Color",
            "Это цвет неба у горизонта");

        DisplayFloatCurve(ref timeOfDayManager.context.skyElevation, 0, 5f, "Horizon Sky Elevation", "Высота горизонта  (♪)");
        DisplayFloatCurve(ref timeOfDayManager.context.atmosphereThickness, 0, 1f, "Atmosphere Thickness", "Толщина атмосферы");
        /*
        EditorGUILayout.BeginHorizontal();
        {
            timeOfDayManager.context.useHorizonFade = EditorGUILayout.Toggle(new GUIContent("Horizon Fade", "Прозрачность луны и звезд на горизонте"), timeOfDayManager.context.useHorizonFade, EditorStyles.radioButton);
            GUI.enabled = timeOfDayManager.context.useHorizonFade;
            ASkyLighting.horizonFade = EditorGUILayout.Slider(ASkyLighting.horizonFade, .03f, 7f);
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();
        */
        EditorGUILayout.Separator();
        GUILayout.Label("Ambient:", EditorStyles.boldLabel);

        //DisplayColorGradient(ref timeOfDayManager.context.nightColor, nightColorGradient, "Night Color", "Ночной цвет (♪)");



        timeOfDayManager.context.ambientMode =
            (UnityEngine.Rendering.AmbientMode) EditorGUILayout.EnumPopup("Ambient Mode", timeOfDayManager.context.ambientMode);

        switch (timeOfDayManager.context.ambientMode)
        {
            case UnityEngine.Rendering.AmbientMode.Skybox:
                DisplayFloatCurve(ref timeOfDayManager.context.ambientIntensity, 0, 8f, "Ambient Intensity", "Интенсивность амбиента");
                break;
            case UnityEngine.Rendering.AmbientMode.Flat:
                DisplayColorGradient(ref timeOfDayManager.context.ambientColor, ambientColorGradient, "Ambient Color", "Цвет амбиента");
                break;
            case UnityEngine.Rendering.AmbientMode.Trilight:
                DisplayColorGradient(ref timeOfDayManager.context.ambientSky, ambientSkyGradient, "Ambient Sky", "Цвет амбиента неба");
                DisplayColorGradient(
                    ref timeOfDayManager.context.ambientEquator,
                    ambientEquatorGradient,
                    "Ambient Equator",
                    "Цвет амбиента экватора");
                DisplayColorGradient(
                    ref timeOfDayManager.context.ambientGround,
                    ambientGroundGradient,
                    "Ambient Ground",
                    "Цвет амбиента земли");
                break;
        }

        EditorGUILayout.Separator();
        GUILayout.Label("Other:", EditorStyles.boldLabel);
        timeOfDayManager.context.cloudRenderingLayer =
            EditorGUILayout.LayerField("Cloud Layer", timeOfDayManager.context.cloudRenderingLayer);
        DisplayColorGradient(ref timeOfDayManager.context.groundColor, groundColorGradient, "Ground Color", "Цвет земли для скайбокса");
        DisplayFloatCurve(ref timeOfDayManager.context.exposure, 0, 5f, "Exposure", "Воздействие HDR");

        GUILayout.Label("Emission Curves:", EditorStyles.boldLabel);
        DisplayFloatCurve(ref timeOfDayManager.context.fxAmbient, 0, 1f, "Twilight Emission Curve", "Мощность подсветки окружения");
        DisplayFloatCurve(ref timeOfDayManager.context.fxAmbient02, 0, 1f, "Night  Emission Curve", "Мощность подсветки окружения два");
        DisplayFloatCurve(ref timeOfDayManager.context.fxAmbient03, 0, 1f, "Dark Night Emission Curve", "Мощность подсветки окружения три");

        GUILayout.EndVertical();
        ApplyChanges();
    }

    void Sun()
    {
        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Sun", EditorStyles.boldLabel);

        GUILayout.Space(1);
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);


        GUILayout.Label("Moving", EditorStyles.boldLabel);

        timeOfDayManager.context.worldLongitude = EditorGUILayout.Slider(
            new GUIContent("Longitude", "Долгота"),
            timeOfDayManager.context.worldLongitude,
            0,
            1f);
        timeOfDayManager.context.worldLatitude = EditorGUILayout.Slider(
            new GUIContent("Latitude", "Широта"),
            timeOfDayManager.context.worldLatitude,
            -90f,
            90f);

        GUILayout.Space(5);
        timeOfDayManager.context.sunSize = EditorGUILayout.Slider("Sun Size", timeOfDayManager.context.sunSize, 0.01f, 0.1f);
        timeOfDayManager.context.sunSpotSize = EditorGUILayout.Slider("Sun Spot Size", timeOfDayManager.context.sunSpotSize, 0.5f, 2f);
        EditorGUILayout.Separator();

        GUILayout.Label("Color", EditorStyles.boldLabel);
        DisplayColorGradient(ref timeOfDayManager.context.sunColor, sunColorGradient, "Sun Color", "Цвет солнца");

        DisplayFloatCurve(
            ref timeOfDayManager.context.sunLightIntensity,
            0,
            3f,
            "Sun Light Intensity",
            "Это интенсивность солнечного направленного света");
        DisplayFloatCurve(ref timeOfDayManager.context.sunLightShadowStrenght, 0, 1f, "Sun Shadow Strenght", "Мощность тени от солнца");
        DisplayFloatCurve(ref timeOfDayManager.context.sunLightShadowBias, 0, 1f, "Sun Shadow Bias", "Мощность тени от солнца");

        timeOfDayManager.context.sunFlare = (Flare) EditorGUILayout.ObjectField(
            "Sun Flare",
            timeOfDayManager.context.sunFlare,
            typeof(Flare),
            false);
        GUILayout.EndVertical();

        ApplyChanges();
    }

    void DisplayColorGradient(ref ColorGradient colorGradient, SerializedProperty gradient, string name, string tooltip)
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (colorGradient.use)
            {
                EditorGUILayout.PropertyField(gradient, new GUIContent(name, tooltip));
                ToggleButton(ref colorGradient.use, EditorStyles.miniButton);
            }
            else
            {
                colorGradient.color = EditorGUILayout.ColorField(new GUIContent(name, tooltip), colorGradient.color, true, false, true);
                ApplyGradientFullButton(ref colorGradient);
                ApplyGradientCurrentButton(ref colorGradient);
                ToggleButton(ref colorGradient.use, EditorStyles.miniButtonRight);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DisplayFloatCurve(ref FloatCurve floatCurve, float min, float max, string name, string tooltip)
    {
        EditorGUILayout.BeginHorizontal();
        {
            if (floatCurve.use)
            {
                floatCurve.curve = EditorGUILayout.CurveField(
                    new GUIContent(name, tooltip),
                    floatCurve.curve,
                    Color.white,
                    new Rect(0, min, 1, max));
                ToggleButton(ref floatCurve.use, EditorStyles.miniButton);
            }
            else
            {
                floatCurve.value = EditorGUILayout.Slider(new GUIContent(name, tooltip), floatCurve.value, min, max);
                ApplyFullButton(ref floatCurve);
                ApplyCurrentButton(ref floatCurve);
                ToggleButton(ref floatCurve.use, EditorStyles.miniButtonRight);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void ApplyFullButton(ref FloatCurve floatCurve)
    {
        string tooltip = "Присвоить всей кривой значение " + floatCurve.value.ToString();
        if (GUILayout.Button(new GUIContent("≡", tooltip), EditorStyles.miniButtonLeft, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
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
    }

    void ApplyCurrentButton(ref FloatCurve floatCurve)
    {
        string tooltip = "Присвоить кривой в текущее время " + ASkyLighting.CGTime.ToString() + " значение " + floatCurve.value.ToString();
        if (GUILayout.Button(new GUIContent("■", tooltip), EditorStyles.miniButtonMid, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
        {
            floatCurve.curve.AddKey(new Keyframe(ASkyLighting.CGTime, floatCurve.value));
            floatCurve.use = true;
        }
    }

    void ApplyGradientFullButton(ref ColorGradient colorGradient)
    {
        string tooltip = "Присвоить всему градиенту цвет";
        if (GUILayout.Button(new GUIContent("≡", tooltip), EditorStyles.miniButtonLeft, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
        {
            if (EditorUtility.DisplayDialog("", "Присвоить всему градиенту цвет?", "Да", "Нет"))
            {
                colorGradient.ApplyToGradient();
            }

            colorGradient.use = true;
        }
    }

    void ApplyGradientCurrentButton(ref ColorGradient colorGradient)
    {
        string tooltip = "Присвоить цвет в текущее время " + ASkyLighting.CGTime.ToString();
        if (GUILayout.Button(new GUIContent("■", tooltip), EditorStyles.miniButtonMid, GUILayout.MaxWidth(20), GUILayout.MaxHeight(16)))
        {
            colorGradient.gradient.AddKey(colorGradient.color, ASkyLighting.CGTime);
            colorGradient.use = true;
        }
    }

    void Satellites()
    {
        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Satellites", EditorStyles.boldLabel);
        timeOfDayManager.context.useCelestials = GUILayout.Toggle(timeOfDayManager.context.useCelestials, new GUIContent("Use Satellites"));
        GUILayout.EndHorizontal();
        GUILayout.Space(1);


        if (EditorGUI.EndChangeCheck())
        {
            serObject.ApplyModifiedProperties();
            timeOfDayManager.CreateCelestials();
            timeOfDayManager.ForceUpdate();
        }

        if (!timeOfDayManager.context.useCelestials)
            return;

        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);
        EditorGUI.BeginChangeCheck();

        var origFontStyle = EditorStyles.label.fontStyle;
        EditorStyles.label.fontStyle = FontStyle.Bold;
        timeOfDayManager.context.isEclipse = EditorGUILayout.Toggle("Eclipse", timeOfDayManager.context.isEclipse, EditorStyles.toggle);
        EditorStyles.label.fontStyle = origFontStyle;

        if (timeOfDayManager.context.isEclipse)
        {
            EditorGUILayout.BeginHorizontal("button");
            GUILayout.Space(20);
            timeOfDayManager.context.nightItem.isOpen = EditorGUILayout.Foldout(
                timeOfDayManager.context.nightItem.isOpen,
                new GUIContent("Dark Night Params", "Dark Night Params"));
            EditorGUILayout.EndHorizontal();

            GUI.color = new Color(0.9f, 0.9f, 0.9f);

            if (timeOfDayManager.context.nightItem.isOpen)
            {
                GUILayout.BeginVertical("box");
                GUI.color = Color.white;
                GUILayout.Label("Atmosphere", EditorStyles.boldLabel);
                timeOfDayManager.context.nightItem.topSky = EditorGUILayout.ColorField(
                    new GUIContent("Top Sky", "Top Sky"),
                    timeOfDayManager.context.nightItem.topSky,
                    true,
                    false,
                    true);
                timeOfDayManager.context.nightItem.horizonSky = EditorGUILayout.ColorField(
                    new GUIContent("Horizon Sky", "Horizon Sky"),
                    timeOfDayManager.context.nightItem.horizonSky);

                timeOfDayManager.context.nightItem.atmosphereThickness = EditorGUILayout.Slider(
                    "Atmosphere Thickness",
                    timeOfDayManager.context.nightItem.atmosphereThickness,
                    0,
                    1f);
                timeOfDayManager.context.nightItem.skyElevation = EditorGUILayout.Slider(
                    "Sky Elevation",
                    timeOfDayManager.context.nightItem.skyElevation,
                    0,
                    5f);
                timeOfDayManager.context.nightItem.groundColor = EditorGUILayout.ColorField(
                    "Ground Color",
                    timeOfDayManager.context.nightItem.groundColor);
                timeOfDayManager.context.nightItem.exposure = EditorGUILayout.Slider(
                    "Exposure",
                    timeOfDayManager.context.nightItem.exposure,
                    0,
                    5f);
                EditorGUILayout.Separator();
                GUILayout.Label("Ambient", EditorStyles.boldLabel);
                if (timeOfDayManager.context.ambientMode == UnityEngine.Rendering.AmbientMode.Skybox)
                    timeOfDayManager.context.nightItem.ambientIntensity = EditorGUILayout.Slider(
                        "Ambient Intensity",
                        timeOfDayManager.context.nightItem.ambientIntensity,
                        0,
                        8f);
                else if (timeOfDayManager.context.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight)
                {
                    timeOfDayManager.context.nightItem.ambientGround = EditorGUILayout.ColorField(
                        new GUIContent("Ambient Ground", "Ambient Ground"),
                        timeOfDayManager.context.nightItem.ambientGround,
                        true,
                        false,
                        true);
                    timeOfDayManager.context.nightItem.ambientEquator = EditorGUILayout.ColorField(
                        new GUIContent("Ambient Equator", "Ambient Equator"),
                        timeOfDayManager.context.nightItem.ambientEquator,
                        true,
                        false,
                        true);
                    timeOfDayManager.context.nightItem.ambientSky = EditorGUILayout.ColorField(
                        new GUIContent("Ambient Sky", "Ambient Sky"),
                        timeOfDayManager.context.nightItem.ambientSky,
                        true,
                        false,
                        true);
                }

                EditorGUILayout.Separator();
                GUILayout.Label("Stars", EditorStyles.boldLabel);
                timeOfDayManager.context.nightItem.starsColor = EditorGUILayout.ColorField(
                    new GUIContent("Stars Color", "Stars Color"),
                    timeOfDayManager.context.nightItem.starsColor,
                    true,
                    false,
                    true);
                timeOfDayManager.context.nightItem.startIntensity = EditorGUILayout.Slider(
                    "Stars Intensity",
                    timeOfDayManager.context.nightItem.startIntensity,
                    0,
                    2.5f);
                EditorGUILayout.Separator();
                GUILayout.Label("Clouds", EditorStyles.boldLabel);
                timeOfDayManager.context.nightItem.cloudSkyColor = EditorGUILayout.ColorField(
                    new GUIContent("Cloud Sky Color", "Cloud Sky Color"),
                    timeOfDayManager.context.nightItem.cloudSkyColor,
                    true,
                    false,
                    true);
                timeOfDayManager.context.nightItem.cloudSunHighlight = EditorGUILayout.ColorField(
                    new GUIContent("Cloud Sun Highlight", "Cloud Sun Highlight"),
                    timeOfDayManager.context.nightItem.cloudSunHighlight,
                    true,
                    false,
                    true);
                timeOfDayManager.context.nightItem.cloudMoonHighlight = EditorGUILayout.ColorField(
                    new GUIContent("Cloud Moon Hightlight", "Cloud Moon Hightlight"),
                    timeOfDayManager.context.nightItem.cloudMoonHighlight,
                    true,
                    false,
                    true);
                timeOfDayManager.context.nightItem.cloudLightIntensity = EditorGUILayout.Slider(
                    "Cloud Light Intensity",
                    timeOfDayManager.context.nightItem.cloudLightIntensity,
                    0,
                    5f);

                GUILayout.Label("Other", EditorStyles.boldLabel);

                timeOfDayManager.nightParts.reflections = (Cubemap) EditorGUILayout.ObjectField(
                    "Night Reflection",
                    timeOfDayManager.nightParts.reflections,
                    typeof(Cubemap),
                    false);

                timeOfDayManager.context.nightItem.fxAmbient = EditorGUILayout.Slider(
                    "Twilight Emission Curve",
                    timeOfDayManager.context.nightItem.fxAmbient,
                    0,
                    1f);
                timeOfDayManager.context.nightItem.fxAmbient02 = EditorGUILayout.Slider(
                    "Night Emission Curve",
                    timeOfDayManager.context.nightItem.fxAmbient02,
                    0,
                    1f);
                timeOfDayManager.context.nightItem.fxAmbient03 = EditorGUILayout.Slider(
                    "Dark Night Emission Curve",
                    timeOfDayManager.context.nightItem.fxAmbient03,
                    0,
                    1f);

                GUILayout.EndVertical();
            }

            GUI.color = Color.white;
        }

        GUILayout.Space(3);

        timeOfDayManager.context.nightItem.eclipseTopSkyColor = EditorGUILayout.ColorField(
            new GUIContent("Eclipse Top Sky"),
            timeOfDayManager.context.nightItem.eclipseTopSkyColor,
            true,
            false,
            true);
        timeOfDayManager.context.nightItem.eclipseSunColor = EditorGUILayout.ColorField(
            new GUIContent("Eclipse Sun Color"),
            timeOfDayManager.context.nightItem.eclipseSunColor,
            true,
            false,
            true);
        timeOfDayManager.context.nightItem.eclipseSunPower = EditorGUILayout.Slider(
            "Eclipse Sun Power",
            timeOfDayManager.context.nightItem.eclipseSunPower,
            0f,
            3f);

        ASkyLighting.sunDotLimit = EditorGUILayout.Slider("Sun Dot Limit", ASkyLighting.sunDotLimit, 0.01f, 0.25f);
        ASkyLighting.shadowSmooth = EditorGUILayout.Slider(
            "Shadows Smooth" + (Mathf.Lerp(0.0001f, 0.01f, ASkyLighting.shadowSmooth) / 0.0025f).ToString(),
            ASkyLighting.shadowSmooth,
            0,
            1f);
        ASkyLighting.highlightOffset = EditorGUILayout.Slider(
            "Highlight Offset: " + (Mathf.Lerp(0.005f, 0.1f, ASkyLighting.highlightOffset) / 0.025f).ToString(),
            ASkyLighting.highlightOffset,
            0f,
            1f);
        ApplyChanges();



        if (timeOfDayManager.context.sattellites == null)
            timeOfDayManager.context.sattellites = new List<Celestial>();

        GUILayout.EndVertical();
        GUILayout.Space(3);



        for (int i = 0; i < timeOfDayManager.context.sattellites.Count; i++)
            PlanetInspector(i, 0);



        if (timeOfDayManager.context.sattellites.Count < 10)
        {
            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                timeOfDayManager.context.sattellites.Add(new Celestial());
            }
        }



        //ApplyChanges();
    }


    void PlanetInspector(int id, int level)
    {
        GUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth - 105f));
        if (GUILayout.Button((timeOfDayManager.context.sattellites[id].isActive) ? "▼" : "▲", GUILayout.Width(64), GUILayout.Height(16)))
            timeOfDayManager.context.sattellites[id].isActive = !timeOfDayManager.context.sattellites[id].isActive;

        GUILayout.Label(timeOfDayManager.context.sattellites[id].name, EditorStyles.boldLabel, GUILayout.Width(300));
        EditorGUI.BeginChangeCheck();
        timeOfDayManager.context.sattellites[id].isRendered =
            EditorGUILayout.Toggle("", timeOfDayManager.context.sattellites[id].isRendered);
        if (EditorGUI.EndChangeCheck())
            timeOfDayManager.CreateCelestials();
        GUILayout.EndHorizontal();


        if (timeOfDayManager.context.sattellites[id].isActive)
        {
            GUI.color = new Color(1f - 0.1f * level, Mathf.Clamp(1f - 0.25f * level, 0, 1), Mathf.Clamp(1f - 0.5f * level, 0, 1));
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
            {
                GUI.color = Color.white;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        if (timeOfDayManager.context.sattellites[id].prefab != null)
                        {
                            Texture ico = (Texture) AssetPreview.GetAssetPreview(
                                timeOfDayManager.context.sattellites[id].prefab.gameObject);
                            GUILayout.Box(ico, GUILayout.Width(64), GUILayout.Height(64));
                        }

                        if (GUILayout.Button("X", GUILayout.Width(64), GUILayout.Height(20)))
                        {
                            timeOfDayManager.context.sattellites.Remove(timeOfDayManager.context.sattellites[id]);
                            timeOfDayManager.CreateCelestials();
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {
                        EditorGUI.BeginChangeCheck();

                        timeOfDayManager.context.sattellites[id].name = EditorGUILayout.TextField(
                            "Name",
                            timeOfDayManager.context.sattellites[id].name);
                        timeOfDayManager.context.sattellites[id].prefab = (ASkyPlanet) EditorGUILayout.ObjectField(
                            "Prefab",
                            (ASkyPlanet) timeOfDayManager.context.sattellites[id].prefab,
                            typeof(ASkyPlanet),
                            false);

                        List<string> tempStr = new List<string>();
                        tempStr.Add("None");
                        for (int j = 0; j < timeOfDayManager.context.sattellites.Count; j++)
                        {
                            if (j != id)
                                tempStr.Add(timeOfDayManager.context.sattellites[j].name);
                        }

                        timeOfDayManager.context.sattellites[id].parent = EditorGUILayout.Popup(
                            "Parent",
                            timeOfDayManager.context.sattellites[id].parent,
                            tempStr.ToArray());
                        timeOfDayManager.context.sattellites[id].skyBoxIndex = (CelestialSkyboxIndex) EditorGUILayout.EnumPopup(
                            "SkyboxIndex",
                            timeOfDayManager.context.sattellites[id].skyBoxIndex);
                        timeOfDayManager.context.sattellites[id].isAtmopshere = EditorGUILayout.Toggle(
                            "Atmosphere",
                            timeOfDayManager.context.sattellites[id].isAtmopshere);
                        timeOfDayManager.context.sattellites[id].isShadowCasting = EditorGUILayout.Toggle(
                            "Shadow Casting",
                            timeOfDayManager.context.sattellites[id].isShadowCasting);
                        timeOfDayManager.context.sattellites[id].scale = EditorGUILayout.FloatField(
                            "Scale",
                            timeOfDayManager.context.sattellites[id].scale);
                        GUILayout.Space(2);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();


                if (PlanetInspectorChecker(
                    ref timeOfDayManager.context.sattellites[id].isOrbit,
                    "Orbit Parameters",
                    new Color(0.75f, 0.75f, 0.75f)))
                {
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
                    GUI.color = Color.white;
                    GUILayout.Label("Orbit Form", EditorStyles.boldLabel);
                    timeOfDayManager.context.sattellites[id].planetMovement = (CelestialMovement) EditorGUILayout.EnumPopup(
                        "Orbit Center",
                        timeOfDayManager.context.sattellites[id].planetMovement);

                    if (timeOfDayManager.context.sattellites[id].planetMovement == CelestialMovement.World)
                    {
                        timeOfDayManager.context.sattellites[id].mainDistance = EditorGUILayout.Slider(
                            "Orbit Center Distance",
                            timeOfDayManager.context.sattellites[id].mainDistance,
                            0,
                            100f);
                        timeOfDayManager.context.sattellites[id].mainDirection = EditorGUILayout.Vector3Field(
                            "Orbit Center Direction",
                            timeOfDayManager.context.sattellites[id].mainDirection);
                        timeOfDayManager.context.sattellites[id].mainRotation = EditorGUILayout.Vector3Field(
                            "Planet System Rotation",
                            timeOfDayManager.context.sattellites[id].mainRotation);
                        timeOfDayManager.context.sattellites[id].up = EditorGUILayout.Vector3Field(
                            "Up",
                            timeOfDayManager.context.sattellites[id].up);
                    }

                    timeOfDayManager.context.sattellites[id].periapsis = EditorGUILayout.FloatField(
                        (timeOfDayManager.context.sattellites[id].eccentricity == 0) ? "Radius" : "Periapsis",
                        timeOfDayManager.context.sattellites[id].periapsis);
                    timeOfDayManager.context.sattellites[id].eccentricity = EditorGUILayout.Slider(
                        "Eccentricy",
                        timeOfDayManager.context.sattellites[id].eccentricity,
                        0,
                        0.8f);
                    GUILayout.Space(3);
                    GUILayout.Label("Orbit Transforms", EditorStyles.boldLabel);
                    timeOfDayManager.context.sattellites[id].longitude = EditorGUILayout.Slider(
                        "Longitude",
                        timeOfDayManager.context.sattellites[id].longitude,
                        -180f,
                        180f);
                    timeOfDayManager.context.sattellites[id].inclination = EditorGUILayout.Slider(
                        "Inclination",
                        timeOfDayManager.context.sattellites[id].inclination,
                        -180f,
                        180f);
                    timeOfDayManager.context.sattellites[id].argument = EditorGUILayout.Slider(
                        "Argument",
                        timeOfDayManager.context.sattellites[id].argument,
                        -180f,
                        180f);
                    GUILayout.Space(2);
                    GUILayout.EndVertical();
                }

                if (PlanetInspectorChecker(
                    ref timeOfDayManager.context.sattellites[id].isMovement,
                    "Movement Parameters",
                    new Color(0.75f, 0.75f, 0.75f)))
                {
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
                    GUI.color = Color.white;
                    GUILayout.Label("Orbit Movement", EditorStyles.boldLabel);
                    timeOfDayManager.context.sattellites[id].isTimelineAcceleration = EditorGUILayout.Toggle(
                        "Sync with time",
                        timeOfDayManager.context.sattellites[id].isTimelineAcceleration);
                    timeOfDayManager.context.sattellites[id].meanAnomaly = EditorGUILayout.Slider(
                        "Start Position",
                        timeOfDayManager.context.sattellites[id].meanAnomaly,
                        0,
                        1f);
                    timeOfDayManager.context.sattellites[id].directionOrbit = EditorGUILayout.Toggle(
                        "Forward Direction",
                        timeOfDayManager.context.sattellites[id].directionOrbit);
                    GUILayout.Space(1);
                    timeOfDayManager.context.sattellites[id].periodDays = EditorGUILayout.IntSlider(
                        "Period Duration (Days)",
                        timeOfDayManager.context.sattellites[id].periodDays,
                        1,
                        20);
                    timeOfDayManager.context.sattellites[id].turnsPeriod = EditorGUILayout.IntSlider(
                        "Turns Count",
                        timeOfDayManager.context.sattellites[id].turnsPeriod,
                        1,
                        20);
                    if (timeOfDayManager.context.sattellites[id].periodDays != timeOfDayManager.context.sattellites[id].turnsPeriod)
                        EditorGUILayout.LabelField(
                            "Orbital Period ",
                            ((float) timeOfDayManager.context.sattellites[id].turnsPeriod /
                             (float) timeOfDayManager.context.sattellites[id].periodDays).ToString() +
                            " Turns/Day");

                    GUILayout.Space(5);
                    GUILayout.Label("Planet Rotation", EditorStyles.boldLabel);

                    timeOfDayManager.context.sattellites[id].meanAnomalyRot = EditorGUILayout.Slider(
                        "Start Rotation",
                        timeOfDayManager.context.sattellites[id].meanAnomalyRot,
                        0,
                        1f);
                    timeOfDayManager.context.sattellites[id].directionRotation = EditorGUILayout.Toggle(
                        "Forward Direction",
                        timeOfDayManager.context.sattellites[id].directionRotation);
                    GUILayout.Space(1);
                    timeOfDayManager.context.sattellites[id].periodDaysRot = EditorGUILayout.IntSlider(
                        "Period Duration (Days)",
                        timeOfDayManager.context.sattellites[id].periodDaysRot,
                        1,
                        20);
                    timeOfDayManager.context.sattellites[id].turnsPeriodRot = EditorGUILayout.IntSlider(
                        "Turns Count",
                        timeOfDayManager.context.sattellites[id].turnsPeriodRot,
                        1,
                        20);
                    if (timeOfDayManager.context.sattellites[id].periodDaysRot != timeOfDayManager.context.sattellites[id].turnsPeriodRot)
                        EditorGUILayout.LabelField(
                            "Rotation Period ",
                            ((float) timeOfDayManager.context.sattellites[id].turnsPeriodRot /
                             (float) timeOfDayManager.context.sattellites[id].periodDaysRot).ToString() +
                            " Rotations/Day");

                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }

                if (PlanetInspectorChecker(
                    ref timeOfDayManager.context.sattellites[id].isVisual,
                    "Visual Parameters",
                    new Color(0.75f, 0.75f, 0.75f)))
                {
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
                    GUI.color = Color.white;
                    GUILayout.Label("Color", EditorStyles.boldLabel);

                    SerializedProperty elementProperty =
                        serObject.FindProperty(
                            string.Format(
                                "context.sattellites.Array.data[{0}].color.gradient",
                                id)); // listIterator.GetArrayElementAtIndex(i);
                    DisplayColorGradient(ref timeOfDayManager.context.sattellites[id].color, elementProperty, "Color", "Цвет спутника");
                    if (timeOfDayManager.context.sattellites[id].color.use == true &&
                        timeOfDayManager.context.sattellites[id].isTimelineAcceleration)
                        timeOfDayManager.context.sattellites[id].isColorMovement = EditorGUILayout.Toggle(
                            "Color Movement Factor",
                            timeOfDayManager.context.sattellites[id].isColorMovement);

                    if (timeOfDayManager.context.sattellites[id].isAtmopshere)
                    {
                        GUILayout.Label("Atmosphere Parameters", EditorStyles.boldLabel);
                        DisplayFloatCurve(ref timeOfDayManager.context.sattellites[id].curve, 0f, 50f, "Intensity", "Интенсивность");
                        timeOfDayManager.context.sattellites[id].eclipseAtmoIntensity = EditorGUILayout.Slider(
                            "Eclipse Intensity",
                            timeOfDayManager.context.sattellites[id].eclipseAtmoIntensity,
                            0f,
                            50f);
                        DisplayFloatCurve(ref timeOfDayManager.context.sattellites[id].gamma, 0.5f, 2f, "Atmosphere Thickness", "");
                        DisplayFloatCurve(
                            ref timeOfDayManager.context.sattellites[id].atmosphereDensity,
                            0.5f,
                            4f,
                            "Atmosphere Density",
                            "");
                    }

                    if (timeOfDayManager.context.sattellites[id].parent == 0)
                    {
                        if (timeOfDayManager.context.sattellites[id].moonDirLight != CelestialDirectionLight.Disabled)
                        {
                            GUILayout.Label("Direction Light", EditorStyles.boldLabel);
                            timeOfDayManager.context.sattellites[id].moonDirLight = (CelestialDirectionLight) EditorGUILayout.EnumPopup(
                                "Direction Light",
                                timeOfDayManager.context.sattellites[id].moonDirLight);
                            SerializedProperty elementProperty2 = serObject.FindProperty(
                                string.Format("context.sattellites.Array.data[{0}].moonLightColor.gradient", id));
                            DisplayColorGradient(
                                ref timeOfDayManager.context.sattellites[id].moonLightColor,
                                elementProperty2,
                                "Light Color",
                                "Цвет света");
                            DisplayFloatCurve(
                                ref timeOfDayManager.context.sattellites[id].moonLightIntensity,
                                0f,
                                15f,
                                "Light Intensity",
                                "Интенсивность света");
                            DisplayFloatCurve(
                                ref timeOfDayManager.context.sattellites[id].moonLightShadowStrenght,
                                0f,
                                1f,
                                "Shadow Strenght",
                                "Мощность тени");
                            DisplayFloatCurve(
                                ref timeOfDayManager.context.sattellites[id].moonLightShadowBias,
                                0f,
                                1f,
                                "Shadow Bias",
                                "Смещение тени");
                            GUILayout.Space(5);
                        }
                        else
                        {
                            timeOfDayManager.context.sattellites[id].moonDirLight = (CelestialDirectionLight) EditorGUILayout.EnumPopup(
                                "Direction Light",
                                timeOfDayManager.context.sattellites[id].moonDirLight);
                        }
                    }

                    if (timeOfDayManager.context.sattellites[id].moonDirLight != CelestialDirectionLight.Disabled)
                    {
                        timeOfDayManager.context.sattellites[id].isHalo = GUILayout.Toggle(
                            timeOfDayManager.context.sattellites[id].isHalo,
                            "Halo");
                        if (timeOfDayManager.context.sattellites[id].isHalo)
                        {
                            timeOfDayManager.context.sattellites[id].haloColor = EditorGUILayout.ColorField(
                                new GUIContent("Halo Color", "Halo Color"),
                                timeOfDayManager.context.sattellites[id].haloColor,
                                true,
                                false,
                                true);
                            timeOfDayManager.context.sattellites[id].haloSize = EditorGUILayout.Slider(
                                "Halo Size",
                                timeOfDayManager.context.sattellites[id].haloSize,
                                0,
                                1f);
                        }
                    }


                    GUILayout.Space(3);
                    GUILayout.EndVertical();
                }

                if (EditorGUI.EndChangeCheck())
                {
                    timeOfDayManager.CreateCelestials();
                    serObject.ApplyModifiedProperties();
                    timeOfDayManager.ForceUpdate();
                }
            }
            GUI.color = Color.white;
            EditorGUILayout.Separator();
            GUILayout.EndVertical();
        }
    }

    bool PlanetInspectorChecker(ref bool param, string label, Color color)
    {
        if (param)
            GUI.color = color;
        else
            GUI.color = Color.white;
        if (GUILayout.Button(((param) ? "▼" : "▲") + " " + label, GUILayout.Height(16)))
            param = !param;

        GUI.color = Color.white;

        if (param)
            return true;
        else
            return false;
    }

    void Stars()
    {
        EditorGUI.BeginChangeCheck();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Stars", EditorStyles.boldLabel);
        timeOfDayManager.context.useStars = GUILayout.Toggle(timeOfDayManager.context.useStars, new GUIContent("Use Stars"));
        GUILayout.EndHorizontal();
        GUILayout.Space(1);

        if (!timeOfDayManager.context.useStars)
            return;

        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);

        DisplayColorGradient(ref timeOfDayManager.context.starsColor, starsColorGradient, "Stars Color", "Цвет звезд");

        DisplayFloatCurve(ref timeOfDayManager.context.starsIntensity, 0, 2.5f, "Stars Intensity", "Интенсивность звезд");
        EditorGUILayout.Separator();

        GUILayout.Label("Direction Light", EditorStyles.boldLabel);
        timeOfDayManager.context.starsLightColor = EditorGUILayout.ColorField(
            new GUIContent("Light Color", "Цвет света"),
            timeOfDayManager.context.starsLightColor,
            true,
            false,
            true);
        DisplayFloatCurve(ref timeOfDayManager.context.starsLightIntensity, 0, 3f, "Stars Light Intensity", "Интенсивность света звезд");
        timeOfDayManager.context.starsLightShadowStrenght = EditorGUILayout.Slider(
            new GUIContent("Shadow Strenght", "Мощность тени"),
            timeOfDayManager.context.starsLightShadowStrenght,
            0,
            1f);
        timeOfDayManager.context.starsLightShadowBias = EditorGUILayout.Slider(
            new GUIContent("Shadow Bias", "Смещение тени"),
            timeOfDayManager.context.starsLightShadowBias,
            0,
            1f);

        GUI.enabled = true;
        GUILayout.EndVertical();

        ApplyChanges();
    }

    void OpenPresets()
    {
        DirectoryInfo dir = new DirectoryInfo("Assets/ASkyLighting/LightingPresets/Weather");
        FileInfo[] info = dir.GetFiles("*.asset");

        if (weatherPresets == null)
            weatherPresets = new List<AWeatherPreset>();
        else
            weatherPresets.Clear();

        foreach (FileInfo f in info)
        {
            bool isExist = false;
            foreach (AWeatherPreset preset in timeOfDayManager.context.weather.currentWeatherPresets)
            {
                if (preset.Name == f.Name.Substring(0, f.Name.Length - 6))
                    isExist = true;
            }

            if (!isExist)
            {
                string path = f.FullName.Substring(f.FullName.LastIndexOf("Assets\\"));
                weatherPresets.Add(AssetDatabase.LoadAssetAtPath<AWeatherPreset>(path));
            }
        }

        //timeOfDayManager.Weather.weatherPresets = 
    }

    void Clouds()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Clouds", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        timeOfDayManager.context.useClouds = GUILayout.Toggle(timeOfDayManager.context.useClouds, new GUIContent("Use Clouds"));
        if (EditorGUI.EndChangeCheck())
            timeOfDayManager.context.weather.cloudSystem.SwitchClouds(timeOfDayManager.context.useClouds);
        GUILayout.EndHorizontal();
        GUILayout.Space(1);
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
        GUILayout.Space(3);

        //---------------------------------------------------------------------------------------------------
        GUILayout.BeginHorizontal();
        if (timeOfDayManager.context.weather.cloudSystem.isCloudPresets) GUI.color = Color.gray;
        else GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent("Cloud Presets", "Пресеты облаков"), GUI.skin.FindStyle("ButtonLeft")))
        {
            timeOfDayManager.context.weather.cloudSystem.isCloudPresets = !timeOfDayManager.context.weather.cloudSystem.isCloudPresets;
        }

        GUI.color = Color.white;

        if (timeOfDayManager.context.weather.cloudSystem.isCloudLayers) GUI.color = Color.gray;
        else GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent("Layers", "Статичные слои облаков"), GUI.skin.FindStyle("ButtonMid")))
        {
            timeOfDayManager.context.weather.cloudSystem.isCloudLayers = !timeOfDayManager.context.weather.cloudSystem.isCloudLayers;
        }

        GUI.color = Color.white;

        if (timeOfDayManager.context.weather.cloudSystem.isCloudGlobal) GUI.color = Color.gray;
        else GUI.color = Color.white;
        if (GUILayout.Button(new GUIContent("Global", "Глобальные параметры облаков"), GUI.skin.FindStyle("ButtonRight")))
        {
            timeOfDayManager.context.weather.cloudSystem.isCloudGlobal = !timeOfDayManager.context.weather.cloudSystem.isCloudGlobal;
        }

        GUI.color = Color.white;
        GUILayout.EndHorizontal();


        if (Application.isPlaying)
        {
            if (timeOfDayManager.context.weather.currentWeatherPresets.Count > 0)
            {
                GUIContent[] zonePrefabs = new GUIContent[timeOfDayManager.context.weather.currentWeatherPresets.Count];
                for (int idx = 0; idx < zonePrefabs.Length; idx++)
                {
                    zonePrefabs[idx] = new GUIContent(timeOfDayManager.context.weather.currentWeatherPresets[idx].Name);
                }

                int weatherID = EditorGUILayout.Popup(new GUIContent("Current Weather"), GetActiveWeatherID(), zonePrefabs);
                timeOfDayManager.context.weather.ChangeWeather(weatherID);
            }
        }

        GUILayout.EndVertical();



        if (timeOfDayManager.context.weather.cloudSystem.isCloudPresets)
        {
            GUILayout.Label("Cloud Presets", EditorStyles.boldLabel, GUILayout.Width(100));

            GUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth - 35f));

            GUILayout.BeginVertical(
                GUI.skin.GetStyle("Box"),
                GUILayout.MinHeight(200),
                GUILayout.Height(timeOfDayManager.context.weather.currentWeatherPresets.Count * 29f + 29f));
            for (int i = 0; i < timeOfDayManager.context.weather.currentWeatherPresets.Count; i++)
            {
                if (timeOfDayManager.context.weather.currentWeatherPresets[i] ==
                    timeOfDayManager.context.weather.currentActiveWeatherPreset)
                    GUI.color = Color.gray;
                else
                    GUI.color = new Color(0.85f, 0.85f, 0.85f, 1.0f);

                GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));
                if (GUILayout.Button(
                    timeOfDayManager.context.weather.currentWeatherPresets[i].Name,
                    GUI.skin.FindStyle("ButtonLeft"),
                    GUILayout.Width(85)))
                {
                    timeOfDayManager.isOpenPresets = false;
                    if (!Application.isPlaying)
                        timeOfDayManager.context.weather.currentActiveWeatherPreset =
                            timeOfDayManager.context.weather.currentWeatherPresets[i];
                    else
                    {
                        if (timeOfDayManager.context.weather.currentWeatherPresets[i] !=
                            timeOfDayManager.context.weather.currentActiveWeatherPreset)
                            timeOfDayManager.context.weather.currentActiveWeatherPreset02 =
                                timeOfDayManager.context.weather.currentWeatherPresets[i];
                    }

                    cloudSelectPreset = 0;
                }

                Color old = GUI.color;
                GUI.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
                if (GUILayout.Button("X", GUI.skin.FindStyle("ButtonRight"), GUILayout.Width(25)))
                {
                    timeOfDayManager.context.weather.currentActiveWeatherPreset = timeOfDayManager.context.weather.currentWeatherPresets[i];
                    timeOfDayManager.context.weather.currentWeatherPresets.Remove(
                        timeOfDayManager.context.weather.currentActiveWeatherPreset);
                    OpenPresets();
                    timeOfDayManager.isOpenPresets = true;
                    cloudSelectPreset = 0;
                }

                GUI.color = old;

                for (int j = 0; j < timeOfDayManager.context.weather.currentWeatherPresets[i].cloudConfig.Count; j++)
                {
                    GUI.color = timeOfDayManager.context.weather.currentWeatherPresets[i].cloudConfig[j].BaseColor;
                    if (cloudSelectPreset == j &&
                        timeOfDayManager.context.weather.currentActiveWeatherPreset ==
                        timeOfDayManager.context.weather.currentWeatherPresets[i])
                        GUI.color = Color.green;

                    GUIStyle current = GUI.skin.FindStyle("Button");
                    if (timeOfDayManager.context.weather.currentWeatherPresets[i].cloudConfig.Count > 1)
                    {
                        if (timeOfDayManager.context.weather.currentWeatherPresets[i].cloudConfig.Count > j + 1 && j == 0)
                            current = GUI.skin.FindStyle("ButtonLeft");
                        else if (timeOfDayManager.context.weather.currentWeatherPresets[i].cloudConfig.Count == j + 1)
                            current = GUI.skin.FindStyle("ButtonRight");
                        else
                            current = GUI.skin.FindStyle("ButtonMid");
                    }

                    if (GUILayout.Button(j.ToString(), current, GUILayout.Width(25)))
                    {
                        timeOfDayManager.isOpenPresets = false;
                        timeOfDayManager.context.weather.currentActiveWeatherPreset =
                            timeOfDayManager.context.weather.currentWeatherPresets[i];
                        cloudSelectPreset = j;
                    }

                    GUI.color = Color.gray;
                }

                GUI.color = Color.white;
                GUI.color = new Color(0.95f, 0.95f, 0.95f, 1.0f);
                if (GUILayout.Button("+", GUILayout.Width(17)))
                {
                    if (timeOfDayManager.context.weather.currentWeatherPresets[i].cloudConfig.Count <
                        timeOfDayManager.context.weather.cloudSystem.cloudsSettings.cloudsLayersVariables.Count)
                    {
                        timeOfDayManager.context.weather.currentActiveWeatherPreset =
                            timeOfDayManager.context.weather.currentWeatherPresets[i];
                        timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Add(new AWeatherCloudsConfig());
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("", "Необходим еще один Cloud Layer", "Ок");
                    }
                }


                GUILayout.EndHorizontal();
                GUI.color = Color.white;
            }

            GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"), GUILayout.ExpandWidth(true));

            GUI.color = new Color(0.85f, 0.85f, 0.85f, 1.0f);
            if (GUILayout.Button(new GUIContent("Find Presets", "Найти подходящие пресеты")))
            {
                OpenPresets();
                timeOfDayManager.isOpenPresets = true;
                cloudSelectPreset = 0;
            }

            if (cloudSelectPreset >= 0 &&
                timeOfDayManager.context.weather.currentActiveWeatherPreset != null &&
                timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Count > 0)
                if (GUILayout.Button(new GUIContent("Remove", "Удалить выделенный динамический слой облаков")))
                {
                    timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Remove(
                        timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[cloudSelectPreset]);
                    if (timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Count > 0)
                        cloudSelectPreset = timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Count - 1;
                }

            if (GUILayout.Button(new GUIContent("Save", "Сохранить выделенный динамический слой облаков")))
            {
                EditorUtility.SetDirty(timeOfDayManager.context.weather.currentActiveWeatherPreset);
                AssetDatabase.SaveAssets();
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if ((EditorGUIUtility.currentViewWidth - 350) > 0f)
            {
                if (timeOfDayManager.isOpenPresets)
                {
                    GUI.color = new Color(0.35f, 0.35f, 0.35f, 1);

                    Rect rect = GUILayoutUtility.GetRect(
                        160.0f,
                        1000,
                        100,
                        Mathf.Max(150, timeOfDayManager.context.weather.currentWeatherPresets.Count * 29f + 29f),
                        GUI.skin.GetStyle("Box"));
                    GUI.Box(rect, "");
                    GUI.color = Color.white;



                    Rect innerRect = new Rect(rect.x, rect.y, rect.width - 15, rect.height);

                    int count = weatherPresets.Count;
                    int maxHor = Mathf.Max(1, Mathf.FloorToInt((innerRect.width) / 165));
                    int maxVer = Mathf.FloorToInt((int) count / (int) maxHor);
                    if (Mathf.FloorToInt((int) count % (int) maxHor) > 0)
                        maxVer++;

                    scrollPosCloudPreset = GUI.BeginScrollView(
                        rect,
                        scrollPosCloudPreset,
                        new Rect(innerRect.x, innerRect.y, innerRect.width, maxVer * 29f + 29f),
                        false,
                        true);

                    GUI.Label(new Rect(innerRect.x, innerRect.y, innerRect.width, 25f), "Доступные ассеты", skin.GetStyle("Label"));
                    GUI.BeginGroup(new Rect(innerRect.x, innerRect.y, innerRect.width, maxVer * 29f + 29f));



                    for (int j = 0; j < maxVer; j++)
                    {
                        for (int i = 0; i < maxHor; i++)
                        {
                            int select = i + j * maxHor;
                            if (select < count)
                            {
                                float aWidth = (maxHor < 2) ? Mathf.Min(innerRect.width - 15, 160) : 160;
                                float length = innerRect.width - (aWidth + 5) * maxHor;

                                Rect button = new Rect(length / 2 + 165 * i, 26 * j + 25, aWidth, 23);

                                if (GUI.Button(
                                    button,
                                    new GUIContent(weatherPresets[select].Name, "Добавить пресет облаков в таблицу активных пресетов"),
                                    skin.GetStyle("Button")))
                                {
                                    Debug.Log(i + j * maxHor);
                                    timeOfDayManager.context.weather.currentWeatherPresets.Add(weatherPresets[select]);
                                    weatherPresets.Remove(weatherPresets[select]);
                                    OpenPresets();
                                    timeOfDayManager.isOpenPresets = true;
                                    return;
                                }

                                //GUI.color = colorOld;
                            }
                        }
                    }

                    GUI.EndGroup();
                    GUI.EndScrollView();
                    //
                }
                else
                {
                    if (timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Count > 0)
                    {
                        GUILayout.BeginVertical(
                            GUI.skin.GetStyle("Box"),
                            GUILayout.Height(timeOfDayManager.context.weather.currentWeatherPresets.Count * 29f + 29f));

                        for (int i = 0; i < timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig.Count; i++)
                        {
                            if (i == cloudSelectPreset)
                            {
                                GUILayout.BeginVertical(
                                    "  " + timeOfDayManager.context.weather.currentActiveWeatherPreset.Name + ": " + " Layer " + i,
                                    skin.GetStyle("LabelBlack"),
                                    GUILayout.ExpandWidth(true));
                                GUILayout.Space(25);
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(new GUIContent("Base Color", "Базовый цвет"), GUILayout.Width(100));
                                timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].BaseColor =
                                    EditorGUILayout.ColorField(
                                        timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].BaseColor);
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(new GUIContent("Light Influence", "Влияние света"), GUILayout.Width(100));
                                timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].DirectLightInfluence =
                                    EditorGUILayout.Slider(
                                        timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].DirectLightInfluence,
                                        0f,
                                        100f);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(new GUIContent("Density", "Плотность"), GUILayout.Width(100));
                                timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].Density = EditorGUILayout.Slider(
                                    timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].Density,
                                    -0.25f,
                                    0.25f);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(new GUIContent("Coverage", "Охват"), GUILayout.Width(100));
                                timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].Coverage =
                                    EditorGUILayout.Slider(
                                        timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].Coverage,
                                        -1f,
                                        2f);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(new GUIContent("Alpha", "Прозрачность"), GUILayout.Width(100));
                                timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].Alpha = EditorGUILayout.Slider(
                                    timeOfDayManager.context.weather.currentActiveWeatherPreset.cloudConfig[i].Alpha,
                                    0f,
                                    10f);
                                GUILayout.EndHorizontal();
                                GUILayout.FlexibleSpace();

                                GUILayout.EndVertical();
                            }
                        }

                        GUILayout.EndVertical();
                    }
                }
            }

            GUILayout.EndHorizontal();
        }



        if (timeOfDayManager.context.weather.cloudSystem.isCloudLayers)
        {
            AClouds cloudSystem = timeOfDayManager.context.weather.cloudSystem;

            GUILayout.Label("Static Cloud Layers", EditorStyles.boldLabel, GUILayout.Width(150));
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
            GUILayout.Space(3);

            for (int i = 0; i < cloudSystem.cloudsSettings.cloudsLayersVariables.Count; i++)
            {
                if (cloudSystem.showCloudLayer == null || cloudSystem.showCloudLayer.Count == 0)
                    cloudSystem.CreateShowCloudLayers();

                GUILayout.BeginVertical("", GUI.skin.GetStyle("Box"));
                cloudSystem.showCloudLayer[i] = EditorGUILayout.BeginToggleGroup(
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].Name,
                    cloudSystem.showCloudLayer[i]);
                if (cloudSystem.showCloudLayer[i])
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Layer Name", "Имя слоя"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].Name = EditorGUILayout.TextField(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].Name);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Quality", "Качество"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].Quality = EditorGUILayout.IntSlider(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].Quality,
                        5,
                        100);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Segments", "Количество сегментов"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].segmentCount = EditorGUILayout.IntSlider(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].segmentCount,
                        4,
                        16);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Thickness", "Толщина"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].thickness = EditorGUILayout.Slider(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].thickness,
                        0.001f,
                        0.1f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Curved", "Искривление"), GUILayout.Width(60));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].curved = EditorGUILayout.Toggle(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].curved,
                        GUILayout.Width(40));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].curvedIntensity = EditorGUILayout.Slider(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].curvedIntensity,
                        0.001f,
                        0.5f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Scaling", "Размер"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].Scaling = EditorGUILayout.Slider(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].Scaling,
                        0.5f,
                        2f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Layer Altitude", "Высота слоя"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].layerAltitude = EditorGUILayout.FloatField(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].layerAltitude);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Layer Offset", "Смещение слоя"), GUILayout.Width(100));
                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].LayerOffset = EditorGUILayout.FloatField(
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].LayerOffset);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();

                    Rect rect = GUILayoutUtility.GetRect(220, 150);
                    GUI.Box(new Rect(rect.x + 5, rect.y, rect.width - 5, rect.height), "", GUI.skin.GetStyle("Box"));

                    cloudSystem.cloudsSettings.cloudsLayersVariables[i].myCloudsTexture = (Texture) EditorGUI.ObjectField(
                        new Rect(rect.x + 5, rect.y + 5, 130, 130),
                        "",
                        cloudSystem.cloudsSettings.cloudsLayersVariables[i].myCloudsTexture,
                        typeof(Texture),
                        true);
                    //EditorGUILayout.LabelField("Cloud Map");
                    //

                    GUI.color = Color.gray;
                    if (GUI.Button(
                        new Rect(rect.x + rect.width - 110, rect.y - 20, 110, 18),
                        new GUIContent("Remove", "Удалить статический слой облаков")))
                    {
                        cloudSystem.cloudsSettings.cloudsLayersVariables.Remove(cloudSystem.cloudsSettings.cloudsLayersVariables[i]);
                        if (cloudSystem.cloudsSettings.cloudsLayersVariables.Count > i)
                            cloudSystem.cloudsSettings.cloudsLayersVariables.RemoveAt(i);

                        cloudSystem.CreateShowCloudLayers();
                    }

                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndToggleGroup();
                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Application.isPlaying)
                if (GUILayout.Button(new GUIContent("Recalculate", "Пересчитать статические данные облаков"), GUILayout.Width(100)))
                {
                    timeOfDayManager.InitBakedModules();
                }

            if (GUILayout.Button(new GUIContent("Add Layer", "Добавить статический слой"), GUILayout.Width(100)))
            {
                timeOfDayManager.context.weather.cloudSystem.cloudsSettings.cloudsLayersVariables.Add(new TOD.ACloudsLayerVariables());
                timeOfDayManager.context.weather.cloudSystem.showCloudLayer.Add(true);
            }

            GUILayout.Space(35);

            EditorGUILayout.EndHorizontal();
        }



        if (timeOfDayManager.context.weather.cloudSystem.isCloudGlobal)
        {
            AClouds cloudSystem = timeOfDayManager.context.weather.cloudSystem;

            GUILayout.Label("Cloud Global", EditorStyles.boldLabel, GUILayout.Width(100));
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUI.skin.GetStyle("Box"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 35f));
            GUILayout.Space(3);

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();

            cloudSystem.cloudsHDR = EditorGUILayout.Toggle(
                new GUIContent("Clouds HDR", "Использовать HDR для облаков"),
                cloudSystem.cloudsHDR);
            cloudSystem.useCloudSpeed = EditorGUILayout.Toggle(
                new GUIContent("Clouds Speed", "Разрешить облакам двигаться"),
                cloudSystem.useCloudSpeed);

            bool oldUseBakeClouds = cloudSystem.useBakeClouds;
            cloudSystem.useBakeClouds = EditorGUILayout.Toggle(
                new GUIContent("Use Baked Skybox", "Запекать Скайбокс в текстуру"),
                cloudSystem.useBakeClouds);
            if (oldUseBakeClouds != cloudSystem.useBakeClouds)
                SwitchBakedSkybox();

            bool oldUseCloudShadows = cloudSystem.useCloudShadows;
            cloudSystem.useCloudShadows = EditorGUILayout.Toggle(
                new GUIContent("Use Cloud Shadow", "Использовать тени облаков"),
                cloudSystem.useCloudShadows);
            if (oldUseCloudShadows != cloudSystem.useCloudShadows)
                SwitchCloudShadows();

            if (cloudSystem.useCloudShadows)
            {
                cloudSystem.useMovingShadows = EditorGUILayout.Toggle(
                    new GUIContent("Moving Shadows", "Разрешить теням двигаться"),
                    cloudSystem.useMovingShadows);
                cloudSystem.cloudsSettings.cloudShadowsPower = EditorGUILayout.Slider(
                    new GUIContent("Cloud Shadow Power", "Разрешить теням двигаться"),
                    cloudSystem.cloudsSettings.cloudShadowsPower,
                    0,
                    1f);
                cloudSystem.cloudsSettings.cloudShadowsSize = EditorGUILayout.Slider(
                    new GUIContent("Cloud Shadow Size", "Разрешить теням двигаться"),
                    cloudSystem.cloudsSettings.cloudShadowsSize,
                    -1,
                    1f);
            }

            cloudSystem.cloudsSettings.useWindZoneDirection = EditorGUILayout.Toggle(
                new GUIContent("Use Wind Zone Direction", "Задать направление облаков по ветру"),
                cloudSystem.cloudsSettings.useWindZoneDirection);
            cloudSystem.cloudsSettings.cloudsWindStrengthModificator = EditorGUILayout.Slider(
                new GUIContent("Wind Strength Modificator", "Модификатор силы ветра"),
                cloudSystem.cloudsSettings.cloudsWindStrengthModificator,
                0,
                0.1f);

            if (cloudSystem.cloudsSettings.useWindZoneDirection == false)
            {
                cloudSystem.cloudsSettings.cloudsWindDirectionX = EditorGUILayout.Slider(
                    new GUIContent("Clouds Wind Direction X", "Направление движения по X"),
                    cloudSystem.cloudsSettings.cloudsWindDirectionX,
                    -1f,
                    1f);
                cloudSystem.cloudsSettings.cloudsWindDirectionY = EditorGUILayout.Slider(
                    new GUIContent("Clouds Wind Direction Y", "Направление движения по Y"),
                    cloudSystem.cloudsSettings.cloudsWindDirectionY,
                    -1f,
                    1f);
            }

            GUILayout.EndVertical();

            cloudSystem.worldScale = EditorGUILayout.Slider(new GUIContent("World Scale", "Размер"), cloudSystem.worldScale, 1f, 3f);
            cloudSystem.fixedAltitude = EditorGUILayout.Toggle(
                new GUIContent("Fixed Altitude", "Использовать фиксированную высоту"),
                cloudSystem.fixedAltitude);
            if (cloudSystem.fixedAltitude)
                cloudSystem.cloudsAltitude = EditorGUILayout.Slider(
                    new GUIContent("Clouds Altitude", "Высота"),
                    cloudSystem.cloudsAltitude,
                    200f,
                    1500f);

            cloudSystem.cloudsSettings.cloudQuality = EditorGUILayout.IntSlider("Quality", cloudSystem.cloudsSettings.cloudQuality, 1, 10);

            DisplayColorGradient(ref cloudSystem.cloudsSettings.skyColor, skyColorGradient, "Sky Color", "Базовый цвет облаков");
            DisplayColorGradient(
                ref cloudSystem.cloudsSettings.sunHighlight,
                sunHighlightColorGradient,
                "Sun Highlight Color",
                "Цвет подсветки облаков солнцем");
            DisplayColorGradient(
                ref cloudSystem.cloudsSettings.moonHighlight,
                moonHighlightColorGradient,
                "Moon Highlight Color",
                "Цвет подсветки облаков луной");

            DisplayFloatCurve(ref cloudSystem.cloudsSettings.lightIntensity, 0f, 5f, "Light Intensity", "Яркость облаков");
            cloudSystem.cloudTransitionSpeed = EditorGUILayout.Slider(
                new GUIContent("Transition Speed", "Скорость перехода между типами облаков"),
                cloudSystem.cloudTransitionSpeed,
                0f,
                1f);
            ApplyChanges();

            GUILayout.EndVertical();
        }
    }

    private void ApplyChanges()
    {
        if (EditorGUI.EndChangeCheck())
        {
            serObject.ApplyModifiedProperties();
            timeOfDayManager.ForceUpdate();
        }
    }

    void Presets()
    {
        GUILayout.Label("Other", EditorStyles.boldLabel);
        GUILayout.Space(1);
        GUILayout.BeginVertical(GUI.skin.GetStyle("Box"));
        GUILayout.Space(3);



        GUILayout.EndVertical();
    }


    public void SwitchBakedSkybox()
    {
        if (timeOfDayManager.PlayerCamera == null)
            timeOfDayManager.GetCamera();

        if (timeOfDayManager.context.weather.cloudSystem.useBakeClouds)
        {
            timeOfDayManager.PlayerCamera.clearFlags = CameraClearFlags.SolidColor;
            for (int i = 0; i < timeOfDayManager.context.weather.cloudSystem.Clouds.transform.childCount; i++)
            {
                GameObject cloudsLayer = timeOfDayManager.context.weather.cloudSystem.Clouds.transform.GetChild(i).gameObject;
                cloudsLayer.layer = timeOfDayManager.context.cloudRenderingLayer;
            }
        }
        else
        {
            timeOfDayManager.PlayerCamera.clearFlags = CameraClearFlags.Skybox;
            for (int i = 0; i < timeOfDayManager.context.weather.cloudSystem.Clouds.transform.childCount; i++)
            {
                GameObject cloudsLayer = timeOfDayManager.context.weather.cloudSystem.Clouds.transform.GetChild(i).gameObject;
                cloudsLayer.layer = LayerMask.NameToLayer("Default");
            }
        }
    }



    public void SwitchReflection()
    {
        QualitySettings.realtimeReflectionProbes = timeOfDayManager.isReflectionProbe;

        if (timeOfDayManager.probe == null)
        {
            if (Camera.main != null)
                timeOfDayManager.probe = Camera.main.GetComponentInChildren<ReflectionProbe>();
            if (timeOfDayManager.probe != null)
                timeOfDayManager.probe.gameObject.SetActive(timeOfDayManager.isReflectionProbe);
        }
        else
            timeOfDayManager.probe.gameObject.SetActive(timeOfDayManager.isReflectionProbe);
    }

    public void SwitchHaze()
    {
        if (timeOfDayManager.PlayerCamera == null)
            timeOfDayManager.GetCamera();

        DeepSky.Haze.DS_HazeView component = timeOfDayManager.PlayerCamera.GetComponent<DeepSky.Haze.DS_HazeView>();
        if (component != null)
            component.enabled = timeOfDayManager.useHaze;
    }

    public void SwitchCloudShadows()
    {
        if (timeOfDayManager.PlayerCamera == null)
            timeOfDayManager.GetCamera();

        timeOfDayManager.context.weather.cloudSystem.cloudShadow.enabled = timeOfDayManager.context.weather.cloudSystem.useCloudShadows;
    }



    public int GetActiveWeatherID()
    {
        for (int i = 0; i < timeOfDayManager.context.weather.currentWeatherPresets.Count; i++)
        {
            if (timeOfDayManager.context.weather.currentWeatherPresets[i] == timeOfDayManager.context.weather.currentActiveWeatherPreset)
                return i;
        }

        return -1;
    }
}