using System;
using System.Linq;
using Src.Animation;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    public class AnimationCurveExtractorWindow : EditorWindow
    {
        private SerializedProperty _popupTargetAnimationCurveProperty;
        private static AnimationClip _sourceAnimationClip;
        private string[] _curvesNames;
        private int _selectedCurveIndex;
        private AnimationCurve _selectedCurve;
        private static bool _optimize = true;
        private static float _optimizationTolerance = 0.01f;
        private static float _velocityTimeStep = 0.01f;
        
        public void Init(SerializedProperty inTargetProperty)
        {
            //keep the iterator in its current state...
            _popupTargetAnimationCurveProperty = inTargetProperty;
        }

        private void OnGUI()
        {
            {
                var newSourceAnimationClip = EditorGUILayout.ObjectField("Source Animation", _sourceAnimationClip, typeof(AnimationClip), false) as AnimationClip;
                if (newSourceAnimationClip != _sourceAnimationClip)
                {
                    var selectedName = _curvesNames != null && _selectedCurveIndex>=0 && _selectedCurveIndex<_curvesNames.Length ? _curvesNames[_selectedCurveIndex] : null;
                    _sourceAnimationClip = newSourceAnimationClip;
                    _curvesNames = _sourceAnimationClip != null ? AnimationCurveExtractor.GetCurvesNames(_sourceAnimationClip).ToArray() : null;
                    _selectedCurveIndex = _curvesNames != null ? Mathf.Max(Array.IndexOf(_curvesNames, selectedName), 0) : 0;
                    _selectedCurve = null;
                }
            }

            if (_curvesNames != null && _curvesNames.Length > 0)
            {
                var newSelectedCurveIndex = EditorGUILayout.Popup("Source Curve", _selectedCurveIndex, _curvesNames);
                if (newSelectedCurveIndex != _selectedCurveIndex)
                {
                    _selectedCurveIndex = newSelectedCurveIndex;
                    _selectedCurve = _selectedCurveIndex >= 0 && _selectedCurveIndex < _curvesNames.Length
                        ? AnimationCurveExtractor.ExtractCurve(_sourceAnimationClip, _curvesNames[newSelectedCurveIndex], _velocityTimeStep)
                        : null;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    _optimize = EditorGUILayout.Toggle("Optimize", _optimize);
                    using (new EditorGUI.DisabledGroupScope(!_optimize))
                        _optimizationTolerance = Mathf.Max(EditorGUILayout.FloatField("Tolerance", _optimizationTolerance), 0);
                }

                if (_selectedCurveIndex >= 0 && _selectedCurveIndex < _curvesNames.Length && _curvesNames[_selectedCurveIndex].StartsWith("Velocity"))
                {
                    _velocityTimeStep = Mathf.Max(EditorGUILayout.FloatField("Time step", _velocityTimeStep), 0.001f);
                }

                if (_selectedCurve != null)
                {
                    EditorGUILayout.CurveField(GUIContent.none, _selectedCurve, GUILayout.ExpandHeight(true));
                    using (new EditorGUI.DisabledScope(_popupTargetAnimationCurveProperty == null))
                        if (GUILayout.Button("Extract!"))
                        {
                            if (_optimize)
                                _selectedCurve = AnimationCurveUtils.SimpleOptimization(_selectedCurve, _optimizationTolerance);
                            _popupTargetAnimationCurveProperty.animationCurveValue = _selectedCurve;
                            _popupTargetAnimationCurveProperty.serializedObject.ApplyModifiedProperties();
                            Close();
                        }
                }
            }
        }
    }
}
