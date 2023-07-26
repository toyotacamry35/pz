using DG.DOTweenEditor.Core;
using Src.Animation;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Assets.Src.Editor
{
    [CustomPropertyDrawer(typeof(UnityEngine.AnimationCurve))]
    public class BetterAnimationCurveFieldDrawer : PropertyDrawer
    {
        private static UnityEngine.AnimationCurve _clipBoardAnimationCurve = new UnityEngine.AnimationCurve();

        //menu command context isn't working, so we'll just stash it here...	
        private static SerializedProperty _popupTargetAnimationCurveProperty;

        [MenuItem("CONTEXT/AnimationCurve/Extract From Animation...")]
        private static void ExtractAnimationCurve(MenuCommand inCommand)
        {
            if (_popupTargetAnimationCurveProperty != null)
            {
                var aceWindow = EditorWindow.GetWindow(typeof(AnimationCurveExtractorWindow)) as AnimationCurveExtractorWindow;
                aceWindow.Init(_popupTargetAnimationCurveProperty);
            }
        }

        [MenuItem("CONTEXT/AnimationCurve/Optimize")]
        private static void OptimizeAnimationCurve(MenuCommand inCommand)
        {
            if (_popupTargetAnimationCurveProperty != null)
            {
                _popupTargetAnimationCurveProperty.animationCurveValue = AnimationCurveUtils.SimpleOptimization(_popupTargetAnimationCurveProperty.animationCurveValue, 0.01f);
                _popupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
            }
        }
        
        [MenuItem("CONTEXT/AnimationCurve/Copy Animation Curve")]
        private static void CopyAnimationCurve(MenuCommand inCommand)
        {
            if (_popupTargetAnimationCurveProperty != null)
                _clipBoardAnimationCurve = AnimationCurveUtils.CreateCopy(_popupTargetAnimationCurveProperty.animationCurveValue);
        }

        [MenuItem("CONTEXT/AnimationCurve/Paste Animation Curve")]
        private static void PasteAnimationCurve(MenuCommand inCommand)
        {
            if (_popupTargetAnimationCurveProperty != null)
            {
                _popupTargetAnimationCurveProperty.serializedObject.Update();
                _popupTargetAnimationCurveProperty.animationCurveValue = AnimationCurveUtils.CreateCopy(_clipBoardAnimationCurve);
                _popupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        [MenuItem("CONTEXT/AnimationCurve/Normalize Value")]
        private static void NormalizeValue(MenuCommand inCommand)
        {
            if (_popupTargetAnimationCurveProperty != null)
            {
                _popupTargetAnimationCurveProperty.serializedObject.Update();
                var srcCurve = _popupTargetAnimationCurveProperty.animationCurveValue;
                var curve = new AnimationCurve();
                float max = 0;
                for (int i = 0; i < srcCurve.length; ++i)
                    max = Mathf.Max(max, Mathf.Abs(srcCurve[i].value));
                for (int i = 0; i < srcCurve.length; ++i)
                    //curve.AddKey(new Keyframe(srcCurve[i].time, srcCurve[i].value / max, srcCurve[i].inTangent, srcCurve[i].outTangent, srcCurve[i].inWeight, srcCurve[i].outTangent));
                    curve.AddKey(new Keyframe(srcCurve[i].time, srcCurve[i].value / max));
                for (int i = 0; i < srcCurve.length; ++i)
                    curve.SmoothTangents(i, 1);
                _popupTargetAnimationCurveProperty.animationCurveValue = curve;
                _popupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect inRect, SerializedProperty inProperty, GUIContent inLabel)
        {
            var evt = Event.current;
            if (evt.type == EventType.ContextClick)
            {
                var mousePos = evt.mousePosition;
                if (inRect.Contains(mousePos))
                {
                    _popupTargetAnimationCurveProperty = inProperty.Copy();
                    inProperty.serializedObject.Update();
                    EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "CONTEXT/AnimationCurve/", null);
                }
                Event.current.Use();
            }
            else
            {
                inLabel = EditorGUI.BeginProperty(inRect, inLabel, inProperty);
                EditorGUI.BeginChangeCheck();
                var newCurve = EditorGUI.CurveField(inRect, inLabel, inProperty.animationCurveValue);

                if (EditorGUI.EndChangeCheck()) inProperty.animationCurveValue = newCurve;

                EditorGUI.EndProperty();
            }
        }
    }
}