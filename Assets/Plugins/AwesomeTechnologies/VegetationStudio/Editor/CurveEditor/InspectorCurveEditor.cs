using System.Collections.Generic;
using AwesomeTechnologies.VegetationStudio;
using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies.External.CurveEditor
{
    public static class FxStyles
    {
        public static GUIStyle TickStyleRight;
        public static GUIStyle TickStyleLeft;
        public static GUIStyle TickStyleCenter;

        public static GUIStyle PreSlider;
        public static GUIStyle PreSliderThumb;
        public static GUIStyle PreButton;
        public static GUIStyle PreDropdown;

        public static GUIStyle PreLabel;
        public static GUIStyle HueCenterCursor;
        public static GUIStyle HueRangeCursor;

        public static GUIStyle CenteredBoldLabel;
        public static GUIStyle WheelThumb;
        public static UnityEngine.Vector2 WheelThumbSize;

        public static GUIStyle Header;
        public static GUIStyle HeaderCheckbox;
        public static GUIStyle HeaderFoldout;

        public static Texture2D PlayIcon;
        public static Texture2D CheckerIcon;
        public static Texture2D PaneOptionsIcon;

        public static GUIStyle CenteredMiniLabel;

        static FxStyles()
        {
            TickStyleRight = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleRight,
                fontSize = 9
            };

            TickStyleLeft = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 9
            };

            TickStyleCenter = new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9
            };

            PreSlider = new GUIStyle("PreSlider");
            PreSliderThumb = new GUIStyle("PreSliderThumb");
            PreButton = new GUIStyle("PreButton");
            PreDropdown = new GUIStyle("preDropdown");

            PreLabel = new GUIStyle("ShurikenLabel")
            {
                normal = { textColor = Color.white }
            };

            HueCenterCursor = new GUIStyle("ColorPicker2DThumb")
            {
                normal = { background = (Texture2D)EditorGUIUtility.LoadRequired("Builtin Skins/DarkSkin/Images/ShurikenPlus.png") },
                fixedWidth = 6,
                fixedHeight = 6
            };

            HueRangeCursor = new GUIStyle(HueCenterCursor)
            {
                normal = { background = (Texture2D)EditorGUIUtility.LoadRequired("Builtin Skins/DarkSkin/Images/CircularToggle_ON.png") }
            };

            WheelThumb = new GUIStyle("ColorPicker2DThumb");

            CenteredBoldLabel = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.UpperCenter,
                fontStyle = FontStyle.Bold
            };

            CenteredMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.UpperCenter
            };

            WheelThumbSize = new UnityEngine.Vector2(
                !Mathf.Approximately(WheelThumb.fixedWidth, 0f) ? WheelThumb.fixedWidth : WheelThumb.padding.horizontal,
                !Mathf.Approximately(WheelThumb.fixedHeight, 0f) ? WheelThumb.fixedHeight : WheelThumb.padding.vertical
            );

            Header = new GUIStyle("ShurikenModuleTitle")
            {
                font = (new GUIStyle("Label")).font,
                border = new RectOffset(15, 7, 4, 4),
                fixedHeight = 22,
                contentOffset = new UnityEngine.Vector2(20f, -2f)
            };

            HeaderCheckbox = new GUIStyle("ShurikenCheckMark");
            HeaderFoldout = new GUIStyle("Foldout");

            PlayIcon = (Texture2D)EditorGUIUtility.LoadRequired("Builtin Skins/DarkSkin/Images/IN foldout act.png");
            CheckerIcon = (Texture2D)EditorGUIUtility.LoadRequired("Icons/CheckerFloor.png");

            if (EditorGUIUtility.isProSkin)
                PaneOptionsIcon = (Texture2D)EditorGUIUtility.LoadRequired("Builtin Skins/DarkSkin/Images/pane options.png");
            else
                PaneOptionsIcon = (Texture2D)EditorGUIUtility.LoadRequired("Builtin Skins/LightSkin/Images/pane options.png");
        }
    }

    public class InspectorCurveEditor
    {
        #region Enums

        public enum InspectorCurveType
        {
            Height = 0,
            Steepness=1,
            Distance=2
        }

        enum EditMode
        {
            None,
            Moving,
            TangentEdit
        }

        enum Tangent
        {
            In,
            Out
        }
        #endregion

        #region Structs
        public struct Settings
        {
            public Rect Bounds;
            public RectOffset Padding;
            public Color SelectionColor;
            public float CurvePickingDistance;
            public float KeyTimeClampingDistance;

            public static Settings DefaultSettings
            {
                get
                {
                    return new Settings
                    {
                        Bounds = new Rect(0f, 0f, 1f, 1f),
                        Padding = new RectOffset(10, 10, 10, 10),
                        SelectionColor = Color.yellow,
                        CurvePickingDistance = 6f,
                        KeyTimeClampingDistance = 1e-4f
                    };
                }
            }
        }

        public struct CurveState
        {
            public bool Visible;
            public bool Editable;
            public uint MinPointCount;
            public float ZeroKeyConstantValue;
            public Color Color;
            public float Width;
            public float HandleWidth;
            public bool ShowNonEditableHandles;
            public bool OnlyShowHandlesOnSelection;
            public bool LoopInBounds;

          

            public static CurveState DefaultState
            {
                get
                {
                    return new CurveState
                    {
                        Visible = true,
                        Editable = true,
                        MinPointCount = 2,
                        ZeroKeyConstantValue = 0f,
                        Color = Color.white,
                        Width = 2f,
                        HandleWidth = 2f,
                        ShowNonEditableHandles = true,
                        OnlyShowHandlesOnSelection = false,
                        LoopInBounds = false
                    };
                }
            }
        }

        public struct Selection
        {
            public AnimationCurve Curve;
            public int KeyframeIndex;
            public Keyframe? Keyframe;

            public Selection(AnimationCurve curve, int keyframeIndex, Keyframe? keyframe)
            {
                Curve = curve;
                KeyframeIndex = keyframeIndex;
                Keyframe = keyframe;
            }
        }

        internal struct MenuAction
        {
            internal AnimationCurve Curve;
            internal int Index;
            internal Vector3 Position;

            internal MenuAction(AnimationCurve curve)
            {
                Curve = curve;
                Index = -1;
                Position = Vector3.zero;
            }

            internal MenuAction(AnimationCurve curve, int index)
            {
                Curve = curve;
                Index = index;
                Position = Vector3.zero;
            }

            internal MenuAction(AnimationCurve curve, Vector3 position)
            {
                Curve = curve;
                Index = -1;
                Position = position;
            }
        }
        #endregion

        #region Fields & properties
        public Settings settings { get; private set; }

        Dictionary<AnimationCurve, CurveState> m_Curves;
        Rect m_CurveArea;

        public InspectorCurveType CurveType;

        AnimationCurve m_SelectedCurve;
        int m_SelectedKeyframeIndex = -1;

        EditMode m_EditMode = EditMode.None;
        Tangent m_TangentEditMode;

        bool m_Dirty;
        #endregion

        #region Constructors & destructors
        public InspectorCurveEditor()
            : this(Settings.DefaultSettings)
        { }

        public InspectorCurveEditor(Settings settings)
        {
            this.settings = settings;
            m_Curves = new Dictionary<AnimationCurve, CurveState>();
        }

        #endregion

        #region Public API
        public void Add(params AnimationCurve[] curves)
        {
            foreach (var curve in curves)
                Add(curve, CurveState.DefaultState);
        }

        public void Add(AnimationCurve curve)
        {
            Add(curve, CurveState.DefaultState);
        }

        public void Add(AnimationCurve curve, CurveState state)
        {
            m_Curves.Add(curve, state);
        }

        public void Remove(AnimationCurve curve)
        {
            m_Curves.Remove(curve);
        }

        public void RemoveAll()
        {
            m_Curves.Clear();
        }

        public CurveState GetCurveState(AnimationCurve curve)
        {
            CurveState state;
            if (!m_Curves.TryGetValue(curve, out state))
                throw new KeyNotFoundException("curve");

            return state;
        }

        public void SetCurveState(AnimationCurve curve, CurveState state)
        {
            if (!m_Curves.ContainsKey(curve))
                throw new KeyNotFoundException("curve");

            m_Curves[curve] = state;
        }

        public Selection GetSelection()
        {
            Keyframe? key = null;
            if (m_SelectedKeyframeIndex > -1)
            {
                var curve = m_SelectedCurve;//.animationCurveValue;

                if (m_SelectedKeyframeIndex >= curve.length)
                    m_SelectedKeyframeIndex = -1;
                else
                    key = curve[m_SelectedKeyframeIndex];
            }

            return new Selection(m_SelectedCurve, m_SelectedKeyframeIndex, key);
        }

        //public void SetKeyframe(AnimationCurve curve, int keyframeIndex, Keyframe keyframe)
        //{
        //    var animCurve = curve;//.animationCurveValue;
        //    SetKeyframe(animCurve, keyframeIndex, keyframe);
        //    SaveCurve(curve, animCurve);
        //}

        public bool OnGUI(Rect rect)
        {
            if (Event.current.type == EventType.Repaint)
                m_Dirty = false;

            GUI.BeginClip(rect);
            {
                var area = new Rect(UnityEngine.Vector2.zero, rect.size);
                m_CurveArea = settings.Padding.Remove(area);

                foreach (var curve in m_Curves)
                    OnCurveGUI(area, curve.Key, curve.Value);

                OnGeneralUI();
            }
            GUI.EndClip();

            return m_Dirty;
        }

        #endregion

        #region UI & events

        void OnCurveGUI(Rect rect, AnimationCurve curve, CurveState state)
        {
            // Discard invisible curves
            if (!state.Visible)
                return;

            var animCurve = curve;//.animationCurveValue;
            var keys = animCurve.keys;
            var length = keys.Length;

            // Curve drawing
            // Slightly dim non-editable curves
            var color = state.Color;
            if (!state.Editable)
                color.a *= 0.5f;

            Handles.color = color;
            var bounds = settings.Bounds;

            if (length == 0)
            {
                var p1 = CurveToCanvas(new Vector3(bounds.xMin, state.ZeroKeyConstantValue));
                var p2 = CurveToCanvas(new Vector3(bounds.xMax, state.ZeroKeyConstantValue));
                Handles.DrawAAPolyLine(state.Width, p1, p2);
            }
            else if (length == 1)
            {
                var p1 = CurveToCanvas(new Vector3(bounds.xMin, keys[0].value));
                var p2 = CurveToCanvas(new Vector3(bounds.xMax, keys[0].value));
                Handles.DrawAAPolyLine(state.Width, p1, p2);
            }
            else
            {
                var prevKey = keys[0];
                for (int k = 1; k < length; k++)
                {
                    var key = keys[k];
                    var pts = BezierSegment(prevKey, key);

                    if (float.IsInfinity(prevKey.outTangent) || float.IsInfinity(key.inTangent))
                    {
                        var s = HardSegment(prevKey, key);
                        Handles.DrawAAPolyLine(state.Width, s[0], s[1], s[2]);
                    }
                    else Handles.DrawBezier(pts[0], pts[3], pts[1], pts[2], color, null, state.Width);

                    prevKey = key;
                }

                // Curve extents & loops
                if (keys[0].time > bounds.xMin)
                {
                    if (state.LoopInBounds)
                    {
                        var p1 = keys[length - 1];
                        p1.time -= settings.Bounds.width;
                        var p2 = keys[0];
                        var pts = BezierSegment(p1, p2);

                        if (float.IsInfinity(p1.outTangent) || float.IsInfinity(p2.inTangent))
                        {
                            var s = HardSegment(p1, p2);
                            Handles.DrawAAPolyLine(state.Width, s[0], s[1], s[2]);
                        }
                        else Handles.DrawBezier(pts[0], pts[3], pts[1], pts[2], color, null, state.Width);
                    }
                    else
                    {
                        var p1 = CurveToCanvas(new Vector3(bounds.xMin, keys[0].value));
                        var p2 = CurveToCanvas(keys[0]);
                        Handles.DrawAAPolyLine(state.Width, p1, p2);
                    }
                }

                if (keys[length - 1].time < bounds.xMax)
                {
                    if (state.LoopInBounds)
                    {
                        var p1 = keys[length - 1];
                        var p2 = keys[0];
                        p2.time += settings.Bounds.width;
                        var pts = BezierSegment(p1, p2);

                        if (float.IsInfinity(p1.outTangent) || float.IsInfinity(p2.inTangent))
                        {
                            var s = HardSegment(p1, p2);
                            Handles.DrawAAPolyLine(state.Width, s[0], s[1], s[2]);
                        }
                        else Handles.DrawBezier(pts[0], pts[3], pts[1], pts[2], color, null, state.Width);
                    }
                    else
                    {
                        var p1 = CurveToCanvas(keys[length - 1]);
                        var p2 = CurveToCanvas(new Vector3(bounds.xMax, keys[length - 1].value));
                        Handles.DrawAAPolyLine(state.Width, p1, p2);
                    }
                }
            }

            // Make sure selection is correct (undo can break it)
            bool isCurrentlySelectedCurve = curve == m_SelectedCurve;

            if (isCurrentlySelectedCurve && m_SelectedKeyframeIndex >= length)
                m_SelectedKeyframeIndex = -1;

            // Handles & keys
            for (int k = 0; k < length; k++)
            {
                bool isCurrentlySelectedKeyframe = k == m_SelectedKeyframeIndex;
                var e = Event.current;

                var pos = CurveToCanvas(keys[k]);
                var hitRect = new Rect(pos.x - 8f, pos.y - 8f, 16f, 16f);
                var offset = isCurrentlySelectedCurve
                    ? new RectOffset(5, 5, 5, 5)
                    : new RectOffset(6, 6, 6, 6);

                var outTangent = pos + CurveTangentToCanvas(keys[k].outTangent).normalized * 40f;
                var inTangent = pos - CurveTangentToCanvas(keys[k].inTangent).normalized * 40f;
                var inTangentHitRect = new Rect(inTangent.x - 7f, inTangent.y - 7f, 14f, 14f);
                var outTangentHitrect = new Rect(outTangent.x - 7f, outTangent.y - 7f, 14f, 14f);

                // Draw
                if (state.ShowNonEditableHandles)
                {
                    if (e.type == EventType.Repaint)
                    {
                        var selectedColor = (isCurrentlySelectedCurve && isCurrentlySelectedKeyframe)
                            ? settings.SelectionColor
                            : state.Color;

                        // Keyframe
                        EditorGUI.DrawRect(offset.Remove(hitRect), selectedColor);

                        // Tangents
                        if (isCurrentlySelectedCurve && (!state.OnlyShowHandlesOnSelection || (state.OnlyShowHandlesOnSelection && isCurrentlySelectedKeyframe)))
                        {
                            Handles.color = selectedColor;

                            if (k > 0 || state.LoopInBounds)
                            {
                                Handles.DrawAAPolyLine(state.HandleWidth, pos, inTangent);
                                EditorGUI.DrawRect(offset.Remove(inTangentHitRect), selectedColor);
                            }

                            if (k < length - 1 || state.LoopInBounds)
                            {
                                Handles.DrawAAPolyLine(state.HandleWidth, pos, outTangent);
                                EditorGUI.DrawRect(offset.Remove(outTangentHitrect), selectedColor);
                            }
                        }
                    }
                }

                // Events
                if (state.Editable)
                {
                    // Keyframe move
                    if (m_EditMode == EditMode.Moving && e.type == EventType.MouseDrag && isCurrentlySelectedCurve && isCurrentlySelectedKeyframe)
                    {
                        EditMoveKeyframe(animCurve, keys, k);
                    }

                    // Tangent editing
                    if (m_EditMode == EditMode.TangentEdit && e.type == EventType.MouseDrag && isCurrentlySelectedCurve && isCurrentlySelectedKeyframe)
                    {
                        bool alreadyBroken = !(Mathf.Approximately(keys[k].inTangent, keys[k].outTangent) || (float.IsInfinity(keys[k].inTangent) && float.IsInfinity(keys[k].outTangent)));
                        EditMoveTangent(animCurve, keys, k, m_TangentEditMode, e.shift || !(alreadyBroken || e.control));
                    }

                    // Keyframe selection & context menu
                    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                    {
                        if (hitRect.Contains(e.mousePosition))
                        {
                            if (e.button == 0)
                            {
                                SelectKeyframe(curve, k);
                                m_EditMode = EditMode.Moving;
                                e.Use();
                            }
                            else if (e.button == 1)
                            {
                                // Keyframe context menu
                                var menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Delete Key"), false, (x) =>
                                {
                                    var action = (MenuAction)x;
                                    var curveValue = action.Curve;//.animationCurveValue;
                                    //TUDO Event to update object
                                    //action.curve.serializedObject.Update();
                                    RemoveKeyframe(curveValue, action.Index);
                                    m_SelectedKeyframeIndex = -1;
                                    SaveCurve();
                                    //TUDO Event to update object
                                    //action.curve.serializedObject.ApplyModifiedProperties();
                                }, new MenuAction(curve, k));
                                menu.ShowAsContext();
                                e.Use();
                            }
                        }
                    }

                    // Tangent selection & edit mode
                    if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                    {
                        if (inTangentHitRect.Contains(e.mousePosition) && (k > 0 || state.LoopInBounds))
                        {
                            SelectKeyframe(curve, k);
                            m_EditMode = EditMode.TangentEdit;
                            m_TangentEditMode = Tangent.In;
                            e.Use();
                        }
                        else if (outTangentHitrect.Contains(e.mousePosition) && (k < length - 1 || state.LoopInBounds))
                        {
                            SelectKeyframe(curve, k);
                            m_EditMode = EditMode.TangentEdit;
                            m_TangentEditMode = Tangent.Out;
                            e.Use();
                        }
                    }

                    // Mouse up - clean up states
                    if (e.rawType == EventType.MouseUp && m_EditMode != EditMode.None)
                    {
                        m_EditMode = EditMode.None;
                    }

                    // Set cursors
                    {
                        EditorGUIUtility.AddCursorRect(hitRect, MouseCursor.MoveArrow);

                        if (k > 0 || state.LoopInBounds)
                            EditorGUIUtility.AddCursorRect(inTangentHitRect, MouseCursor.RotateArrow);

                        if (k < length - 1 || state.LoopInBounds)
                            EditorGUIUtility.AddCursorRect(outTangentHitrect, MouseCursor.RotateArrow);
                    }
                }
            }

            Handles.color = Color.white;
            SaveCurve();
        }

        private void OnGeneralUI()
        {
            var e = Event.current;

            // Selection
            if (e.type == EventType.MouseDown)
            {
                GUI.FocusControl(null);
                m_SelectedCurve = null;
                m_SelectedKeyframeIndex = -1;
                bool used = false;

                var hit = CanvasToCurve(e.mousePosition);
                float curvePickValue = CurveToCanvas(hit).y;

                // Try and select a curve
                foreach (var curve in m_Curves)
                {
                    if (!curve.Value.Editable || !curve.Value.Visible)
                        continue;

                    var prop = curve.Key;
                    var state = curve.Value;
                    var animCurve = prop;//.animationCurveValue;
                    float hitY = animCurve.length == 0
                        ? state.ZeroKeyConstantValue
                        : animCurve.Evaluate(hit.x);

                    var curvePos = CurveToCanvas(new Vector3(hit.x, hitY));

                    if (Mathf.Abs(curvePos.y - curvePickValue) < settings.CurvePickingDistance)
                    {
                        m_SelectedCurve = prop;

                        if (e.clickCount == 2 && e.button == 0)
                        {
                            // Create a keyframe on double-click on this curve
                            EditCreateKeyframe(animCurve, hit, true, state.ZeroKeyConstantValue);
                            SaveCurve();
                        }
                        else if (e.button == 1)
                        {
                            // Curve context menu
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add Key"), false, (x) =>
                            {
                                var action = (MenuAction)x;
                                var curveValue = action.Curve;//.animationCurveValue;
                                //TUDO event to update object
                                //action.curve.serializedObject.Update();
                                EditCreateKeyframe(curveValue, hit, true, 0f);
                                SaveCurve();
                                //TUDO event to update object
                                //action.curve.serializedObject.ApplyModifiedProperties();
                            }, new MenuAction(prop, hit));

                           

                            menu.ShowAsContext();
                            e.Use();
                            used = true;
                        }
                    }
                }

                if (e.clickCount == 2 && e.button == 0 && m_SelectedCurve == null)
                {
                    // Create a keyframe on every curve on double-click
                    foreach (var curve in m_Curves)
                    {
                        if (!curve.Value.Editable || !curve.Value.Visible)
                            continue;

                        var prop = curve.Key;
                        var state = curve.Value;
                        var animCurve = prop;//.animationCurveValue;
                        EditCreateKeyframe(animCurve, hit, e.alt, state.ZeroKeyConstantValue);
                        SaveCurve();
                    }
                }
                else if (!used && e.button == 1)
                {
                    // Global context menu
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Key At Position"), false, () => ContextMenuAddKey(hit, false));
                    menu.AddItem(new GUIContent("Add Key On Curves"), false, () => ContextMenuAddKey(hit, true));
                    menu.AddItem(new GUIContent("Copy Curve"), false, () => CopyCurve() );
                    if (VegetationStudioManager.GetAnimationCurveFromClippboard() != null)
                    {
                        menu.AddItem(new GUIContent("Paste Curve"), false, () => PasteCurve());
                    }
                    menu.ShowAsContext();
                }

                e.Use();
            }

           

            // Delete selected key(s)
            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace))
            {
                if (m_SelectedKeyframeIndex != -1 && m_SelectedCurve != null)
                {
                    var animCurve = m_SelectedCurve;//.animationCurveValue;
                    var length = animCurve.length;

                    if (m_Curves[m_SelectedCurve].MinPointCount < length && length >= 0)
                    {
                        EditDeleteKeyframe(animCurve, m_SelectedKeyframeIndex);
                        m_SelectedKeyframeIndex = -1;
                        SaveCurve();
                    }

                    e.Use();
                }
            }
        }

        void CopyCurve()
        {
            if (m_Curves.Count > 0)
            {
                foreach (var curve in m_Curves)
                {
                    VegetationStudioManager.AddAnimationCurveToClipboard(curve.Key);
                    break;
                }
            }
            else
            {
                VegetationStudioManager.AddAnimationCurveToClipboard(null); ;
            }            
        }

        void PasteCurve()
        {
            AnimationCurve animationCurve = VegetationStudioManager.GetAnimationCurveFromClippboard();
            if (animationCurve != null)
            {
                foreach (var curve in m_Curves)
                {
                    Keyframe[] keyframes = animationCurve.keys;
                    curve.Key.keys = new Keyframe[0];                  
                    for (var i = 0; i <= keyframes.Length - 1; i++)
                    {
                        curve.Key.AddKey(keyframes[i].time, keyframes[i].value);
                    }
                    break;
                }
            }
        }

        
        private static void SaveCurve()
        {
            //prop.animationCurveValue = curve;
        }

        void Invalidate()
        {
            m_Dirty = true;
        }

        #endregion

        #region Keyframe manipulations

        void SelectKeyframe(AnimationCurve curve, int keyframeIndex)
        {
            m_SelectedKeyframeIndex = keyframeIndex;
            m_SelectedCurve = curve;
            Invalidate();
        }

        void ContextMenuAddKey(Vector3 hit, bool createOnCurve)
        {
           // SerializedObject serializedObject = null;

            foreach (var curve in m_Curves)
            {
                if (!curve.Value.Editable || !curve.Value.Visible)
                    continue;

                var prop = curve.Key;
                var state = curve.Value;

                //if (serializedObject == null)
                //{
                //    serializedObject = prop.serializedObject;
                //    serializedObject.Update();
                //}

                var animCurve = prop;//.animationCurveValue;
                EditCreateKeyframe(animCurve, hit, createOnCurve, state.ZeroKeyConstantValue);
                SaveCurve();
            }

            //if (serializedObject != null)
            //    serializedObject.ApplyModifiedProperties();

            Invalidate();
        }

        void EditCreateKeyframe(AnimationCurve curve, Vector3 position, bool createOnCurve, float zeroKeyConstantValue)
        {
            float tangent = EvaluateTangent(curve, position.x);

            if (createOnCurve)
            {
                position.y = curve.length == 0
                    ? zeroKeyConstantValue
                    : curve.Evaluate(position.x);
            }

            AddKeyframe(curve, new Keyframe(position.x, position.y, tangent, tangent));
        }

        void EditDeleteKeyframe(AnimationCurve curve, int keyframeIndex)
        {
            RemoveKeyframe(curve, keyframeIndex);
        }

        void AddKeyframe(AnimationCurve curve, Keyframe newValue)
        {
            curve.AddKey(newValue);
            Invalidate();
        }

        void RemoveKeyframe(AnimationCurve curve, int keyframeIndex)
        {
            curve.RemoveKey(keyframeIndex);
            Invalidate();
        }

        void SetKeyframe(AnimationCurve curve, int keyframeIndex, Keyframe newValue)
        {
            var keys = curve.keys;

            if (keyframeIndex > 0)
                newValue.time = Mathf.Max(keys[keyframeIndex - 1].time + settings.KeyTimeClampingDistance, newValue.time);

            if (keyframeIndex < keys.Length - 1)
                newValue.time = Mathf.Min(keys[keyframeIndex + 1].time - settings.KeyTimeClampingDistance, newValue.time);

            curve.MoveKey(keyframeIndex, newValue);
            Invalidate();
        }

        void EditMoveKeyframe(AnimationCurve curve, Keyframe[] keys, int keyframeIndex)
        {
            var key = CanvasToCurve(Event.current.mousePosition);
            float inTgt = keys[keyframeIndex].inTangent;
            float outTgt = keys[keyframeIndex].outTangent;
            SetKeyframe(curve, keyframeIndex, new Keyframe(key.x, key.y, inTgt, outTgt));
        }

        void EditMoveTangent(AnimationCurve curve, Keyframe[] keys, int keyframeIndex, Tangent targetTangent, bool linkTangents)
        {
            var pos = CanvasToCurve(Event.current.mousePosition);

            float time = keys[keyframeIndex].time;
            float value = keys[keyframeIndex].value;

            pos -= new Vector3(time, value);

            if (targetTangent == Tangent.In && pos.x > 0f)
                pos.x = 0f;

            if (targetTangent == Tangent.Out && pos.x < 0f)
                pos.x = 0f;

            float tangent;

            if (Mathf.Approximately(pos.x, 0f))
                tangent = pos.y < 0f ? float.PositiveInfinity : float.NegativeInfinity;
            else
                tangent = pos.y / pos.x;

            float inTangent = keys[keyframeIndex].inTangent;
            float outTangent = keys[keyframeIndex].outTangent;

            if (targetTangent == Tangent.In || linkTangents)
                inTangent = tangent;
            if (targetTangent == Tangent.Out || linkTangents)
                outTangent = tangent;

            SetKeyframe(curve, keyframeIndex, new Keyframe(time, value, inTangent, outTangent));
        }

        #endregion

        #region Maths utilities

        Vector3 CurveToCanvas(Keyframe keyframe)
        {
            return CurveToCanvas(new Vector3(keyframe.time, keyframe.value));
        }

        Vector3 CurveToCanvas(Vector3 position)
        {
            var bounds = settings.Bounds;
            var output = new Vector3((position.x - bounds.x) / (bounds.xMax - bounds.x), (position.y - bounds.y) / (bounds.yMax - bounds.y));
            output.x = output.x * (m_CurveArea.xMax - m_CurveArea.xMin) + m_CurveArea.xMin;
            output.y = (1f - output.y) * (m_CurveArea.yMax - m_CurveArea.yMin) + m_CurveArea.yMin;
            return output;
        }

        Vector3 CanvasToCurve(Vector3 position)
        {
            var bounds = settings.Bounds;
            var output = position;
            output.x = (output.x - m_CurveArea.xMin) / (m_CurveArea.xMax - m_CurveArea.xMin);
            output.y = (output.y - m_CurveArea.yMin) / (m_CurveArea.yMax - m_CurveArea.yMin);
            output.x = Mathf.Lerp(bounds.x, bounds.xMax, output.x);
            output.y = Mathf.Lerp(bounds.yMax, bounds.y, output.y);
            return output;
        }

        Vector3 CurveTangentToCanvas(float tangent)
        {
            if (!float.IsInfinity(tangent))
            {
                var bounds = settings.Bounds;
                float ratio = (m_CurveArea.width / m_CurveArea.height) / ((bounds.xMax - bounds.x) / (bounds.yMax - bounds.y));
                return new Vector3(1f, -tangent / ratio).normalized;
            }

            return float.IsPositiveInfinity(tangent) ? Vector3.up : Vector3.down;
        }

        Vector3[] BezierSegment(Keyframe start, Keyframe end)
        {
            var segment = new Vector3[4];

            segment[0] = CurveToCanvas(new Vector3(start.time, start.value));
            segment[3] = CurveToCanvas(new Vector3(end.time, end.value));

            float middle = start.time + ((end.time - start.time) * 0.333333f);
            float middle2 = start.time + ((end.time - start.time) * 0.666666f);

            segment[1] = CurveToCanvas(new Vector3(middle, ProjectTangent(start.time, start.value, start.outTangent, middle)));
            segment[2] = CurveToCanvas(new Vector3(middle2, ProjectTangent(end.time, end.value, end.inTangent, middle2)));

            return segment;
        }

        Vector3[] HardSegment(Keyframe start, Keyframe end)
        {
            var segment = new Vector3[3];

            segment[0] = CurveToCanvas(start);
            segment[1] = CurveToCanvas(new Vector3(end.time, start.value));
            segment[2] = CurveToCanvas(end);

            return segment;
        }

        float ProjectTangent(float inPosition, float inValue, float inTangent, float projPosition)
        {
            return inValue + ((projPosition - inPosition) * inTangent);
        }

        float EvaluateTangent(AnimationCurve curve, float time)
        {
            int prev = -1, next = 0;
            for (int i = 0; i < curve.keys.Length; i++)
            {
                if (time > curve.keys[i].time)
                {
                    prev = i;
                    next = i + 1;
                }
                else break;
            }

            if (next == 0)
                return 0f;

            if (prev == curve.keys.Length - 1)
                return 0f;

            const float kD = 1e-3f;
            float tp = Mathf.Max(time - kD, curve.keys[prev].time);
            float tn = Mathf.Min(time + kD, curve.keys[next].time);

            float vp = curve.Evaluate(tp);
            float vn = curve.Evaluate(tn);

            if (Mathf.Approximately(tn, tp))
                return (vn - vp > 0f) ? float.PositiveInfinity : float.NegativeInfinity;

            return (vn - vp) / (tn - tp);
        }

        #endregion


        public void DrawCurve(Editor editor)
        {
            var rect = GUILayoutUtility.GetAspectRect(2f);
            var innerRect = settings.Padding.Remove(rect);

            if (Event.current.type == EventType.Repaint)
            {
                // Background
                EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f));

                // Bounds
                Handles.color = Color.white;
                Handles.DrawSolidRectangleWithOutline(innerRect, Color.clear, new Color(0.8f, 0.8f, 0.8f, 0.5f));

                // Grid setup
                Handles.color = new Color(1f, 1f, 1f, 0.05f);
                int hLines = (int)Mathf.Sqrt(innerRect.width);
                int vLines = (int)(hLines / (innerRect.width / innerRect.height));

                // Vertical grid
                int gridOffset = Mathf.FloorToInt(innerRect.width / hLines);
                int gridPadding = ((int)(innerRect.width) % hLines) / 2;

                for (int i = 1; i < hLines; i++)
                {
                    var offset = i * UnityEngine.Vector2.right * gridOffset;
                    offset.x += gridPadding;
                    Handles.DrawLine(innerRect.position + offset, new UnityEngine.Vector2(innerRect.x, innerRect.yMax - 1) + offset);
                }

                // Horizontal grid
                gridOffset = Mathf.FloorToInt(innerRect.height / vLines);
                gridPadding = ((int)(innerRect.height) % vLines) / 2;

                for (int i = 1; i < vLines; i++)
                {
                    var offset = i * UnityEngine.Vector2.up * gridOffset;
                    offset.y += gridPadding;
                    Handles.DrawLine(innerRect.position + offset, new UnityEngine.Vector2(innerRect.xMax - 1, innerRect.y) + offset);
                }
            }

            // Curve editor
            if (OnGUI(rect))
            {
                editor.Repaint();
                GUI.changed = true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                // Borders
                Handles.color = Color.black;

                Handles.DrawLine(new UnityEngine.Vector2(rect.x, rect.y), new UnityEngine.Vector2(rect.xMax, rect.y));
                Handles.DrawLine(new UnityEngine.Vector2(rect.x, rect.y), new UnityEngine.Vector2(rect.x, rect.yMax));
                Handles.DrawLine(new UnityEngine.Vector2(rect.x, rect.yMax), new UnityEngine.Vector2(rect.xMax, rect.yMax));
                Handles.DrawLine(new UnityEngine.Vector2(rect.xMax, rect.yMax), new UnityEngine.Vector2(rect.xMax, rect.y));

                // Selection info
                var selection = GetSelection();

                if (selection.Curve != null && selection.KeyframeIndex > -1)
                {
                    if (selection.Keyframe != null)
                    {
                        var key = selection.Keyframe.Value;
                        var infoRect = innerRect;
                        infoRect.x += 5f;
                        infoRect.width = 150f;
                        infoRect.height = 30f;

                        switch (CurveType)
                        {
                            case InspectorCurveType.Height:
                                GUI.Label(infoRect, string.Format("{0}\n{1}", (key.time * 90).ToString("F2") + " meter above sea level", (key.value * 100).ToString("F0") + "% Density"), FxStyles.PreLabel);
                                break;
                            case InspectorCurveType.Steepness:
                                GUI.Label(infoRect, string.Format("{0}\n{1}", (key.time * 90).ToString("F2") + " Degree steepness", (key.value * 100).ToString("F0") + "% Density"), FxStyles.PreLabel);
                                break;
                        }                      
                    }
                }
            }

            if (CurveType == InspectorCurveType.Height)
            {
                //Handles.color = Color.blue;
                //Handles.DrawLine(new Vector2(rect.x, rect.y + 200), new Vector2(rect.xMax, rect.y + 200));
                //Rect waterRect = new Rect(new Vector2(rect.x, rect.y + 200), new Vector2(rect.xMax - 15, 85));
                //Handles.DrawSolidRectangleWithOutline(waterRect, new Color(0.5f, 0.5f, 0.5f, 0.1f), Color.gray);
            }
        }

        public void AddCurve(AnimationCurve curve, Color color, uint minPointCount, bool loop)
        {
            var state = CurveState.DefaultState;
            state.Color = color;
            state.Visible = false;
            state.MinPointCount = minPointCount;
            state.OnlyShowHandlesOnSelection = true;
            state.ZeroKeyConstantValue = 0.5f;
            state.LoopInBounds = loop;
            state.Visible = true;
            Add(curve, state);
        }

        public bool EditCurve(AnimationCurve animationCurve, Editor editor)
        {
            Keyframe[] keyframes = animationCurve.keys;

            RemoveAll();

            switch(CurveType)
            {
                case InspectorCurveType.Distance:
                    AddCurve(animationCurve, new Color(0f, 1f, 0f), 2, false);
                    break;
                case InspectorCurveType.Height:
                    AddCurve(animationCurve, new Color(1f, 0f, 0f), 2, false);
                    break;
                case InspectorCurveType.Steepness:
                    AddCurve(animationCurve, new Color(0f, 1f, 0f), 2, false);
                    break;
            }
          
            DrawCurve(editor);

            if (animationCurve.keys.Length != keyframes.Length) return true;
            for (int i = 0; i <= animationCurve.keys.Length - 1; i++)
            {
                if (!keyframes[i].Equals(animationCurve.keys[i])) return true;
            }

            return false;
        }
    }
}

