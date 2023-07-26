using System;
using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Utils;
using JetBrains.Annotations;
using Src.Animation;
using Src.Tools;
using UnityEditor;
using UnityEngine;

namespace Assets.Src.Editor
{
    [CustomEditor(typeof(ExportedAnimationCurve))]
    public class ExportedAnimationCurveInspector : ExportedAnimationCurveInspectorBase
    {
        protected override ClassLayout Layout => new ClassLayout
        {
            Clip = nameof(ExportedAnimationCurve.Clip),
            Name = nameof(ExportedAnimationCurve.CurveName),
            Autoupdate = nameof(ExportedAnimationCurve.AutoUpdate),
            Optimization = nameof(ExportedAnimationCurve.Optimization),
            OptimizationTolerance = nameof(ExportedAnimationCurve.OptimizationTolerance),
            Scale = nameof(ExportedAnimationCurve.Scale),
        };
        
        public static void UpdateCurve([NotNull] IExportedAnimationCurve ieac)
        {
            if (ieac == null) throw new ArgumentNullException(nameof(ieac));
            var eac = (ExportedAnimationCurve) ieac;
            var newCurve = eac.Clip != null && eac.CurveName != null ? AnimationCurveExtractor.ExtractCurve(eac.Clip, eac.CurveName) : null;
            if (newCurve != null)
            {
                eac.curve = newCurve;
                EditorUtility.SetDirty(eac);
            }
            else
                throw new Exception($"Can't extract curve '{eac.CurveName}' for '{eac.name}' from '{eac.Clip}'");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();
            
            var curveProperty = serializedObject.FindProperty(nameof(ExportedAnimationCurve.curve));

            void Draw()
            {
                EditorGUILayout.PropertyField(curveProperty, GUIContent.none, GUILayout.Height(80));
            }

            void Export(AnimationClip clip, string curveName, bool optimization, float optimizationTolerance, float scale)
            {
                var newCurve = clip != null && !string.IsNullOrEmpty(curveName) ? AnimationCurveExtractor.ExtractCurve(clip, curveName) : null;
                if (newCurve != null)
                {
                    if (!Mathf.Approximately(scale, 1)) 
                        newCurve = AnimationCurveUtils.Scale(newCurve, scale);
                    if (optimization) 
                        newCurve = AnimationCurveUtils.SimpleOptimization(newCurve, optimizationTolerance);
                    curveProperty.animationCurveValue = newCurve;
                }
            }

            OnInspectorGUIImpl(Draw, Export);
            
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Assets/Create/Curves/Convert to Exported Animation Curve", true)]
        public static bool ConvertToExportedAnimationCurveCheck()
        {
            foreach (var obj in Selection.objects)
                if (obj.GetType() == typeof(Curve))
                    return true;
            return false;
        }   
        
        [MenuItem("Assets/Create/Curves/Convert to Exported Animation Curve")]
        public static void ConvertToExportedAnimationCurve()
        {
            var newSelection = new List<UnityEngine.Object>();
            foreach (var obj in Selection.objects)
            {
                if (obj.GetType() == typeof(Curve)) // не "is", так как ExportedAnimationCurve тоже Curve, но её конвертить не надо 
                {
                    var curve = (Curve)obj;
                    var path = AssetDatabase.GetAssetPath(curve);
                    var newCurve = ExportedAnimationCurve.CreateInstance<ExportedAnimationCurve>();
                    newCurve.curve = curve.curve;
                    var oldAssetPath = AssetDatabase.MoveAsset(path, "~" + path);
                    AssetDatabase.Refresh();
                    AssetDatabase.CreateAsset(newCurve, path);
                    AssetDatabase.DeleteAsset(oldAssetPath);
                    AssetDatabase.Refresh();
                    //AssetDatabase.ImportAsset(path);
                    newSelection.Add(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
                }
            }
            Selection.objects = newSelection.ToArray();
        }
    }
}