using System;
using JetBrains.Annotations;
using Src.Animation;
using Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
     [CustomEditor(typeof(ExportedAnimationCurve3))]
    public class ExportedAnimationCurve3Inspector : ExportedAnimationCurveInspectorBase
    {
        protected override ClassLayout Layout => new ClassLayout
        {
            Clip = nameof(ExportedAnimationCurve3.Clip),
            Name = nameof(ExportedAnimationCurve3.CurveName),
            Autoupdate = nameof(ExportedAnimationCurve3.AutoUpdate),
            Optimization = nameof(ExportedAnimationCurve3.Optimization),
            OptimizationTolerance = nameof(ExportedAnimationCurve3.OptimizationTolerance),
            Scale = nameof(ExportedAnimationCurve3.Scale),
        };

        public static void UpdateCurve([NotNull] IExportedAnimationCurve ieac)
        {
            if (ieac == null) throw new ArgumentNullException(nameof(ieac));
            var eac = (ExportedAnimationCurve3) ieac;
            var newCurve = eac.Clip != null && eac.CurveName != null ? AnimationCurveExtractor.ExtractCurve3(eac.Clip, eac.CurveName) : null;
            if (newCurve != null)
            {
                eac.curveX = newCurve.Item1;
                eac.curveY = newCurve.Item2;
                eac.curveZ = newCurve.Item3;
                EditorUtility.SetDirty(eac);
            }
            else
                throw new Exception($"Can't extract curve '{eac.CurveName}' for '{eac.name}' from '{eac.Clip}'");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            
            var curveXProperty = serializedObject.FindProperty(nameof(ExportedAnimationCurve3.curveX));
            var curveYProperty = serializedObject.FindProperty(nameof(ExportedAnimationCurve3.curveY));
            var curveZProperty = serializedObject.FindProperty(nameof(ExportedAnimationCurve3.curveZ));

            void Draw()
            {
                EditorGUILayout.PropertyField(curveXProperty, GUIContent.none, GUILayout.Height(80));
                EditorGUILayout.PropertyField(curveYProperty, GUIContent.none, GUILayout.Height(80));
                EditorGUILayout.PropertyField(curveZProperty, GUIContent.none, GUILayout.Height(80));
            }

            void Export(AnimationClip clip, string curveName, bool optimization, float optimizationTolerance, float scale)
            {
                var newCurve = clip != null && !string.IsNullOrEmpty(curveName) ? AnimationCurveExtractor.ExtractCurve3(clip, curveName) : null;
                if (newCurve != null)
                {
                    if (!Mathf.Approximately(scale, 1))
                        newCurve = Tuple.Create(
                            AnimationCurveUtils.Scale(newCurve.Item1, scale),
                            AnimationCurveUtils.Scale(newCurve.Item2, scale),
                            AnimationCurveUtils.Scale(newCurve.Item3, scale)
                            );
                    if (optimization) 
                        newCurve = Tuple.Create(
                            AnimationCurveUtils.SimpleOptimization(newCurve.Item1, optimizationTolerance),
                            AnimationCurveUtils.SimpleOptimization(newCurve.Item2, optimizationTolerance),
                            AnimationCurveUtils.SimpleOptimization(newCurve.Item3, optimizationTolerance)
                        );
                    curveXProperty.animationCurveValue = newCurve.Item1;
                    curveYProperty.animationCurveValue = newCurve.Item2;
                    curveZProperty.animationCurveValue = newCurve.Item3;
                }
            }

            OnInspectorGUIImpl(Draw, Export);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}