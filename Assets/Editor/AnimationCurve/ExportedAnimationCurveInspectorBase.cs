using System;
using System.Collections.Generic;
using System.Linq;
using Src.Animation;
using Src.Tools;
using Assets.Src.Tools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Src.Editor
{
    public abstract class ExportedAnimationCurveInspectorBase : UnityEditor.Editor
    {
        private static readonly Dictionary<Object,AuxData> _AuxData = new Dictionary<Object,AuxData>();
        
        protected delegate void DrawDelegate();  
        protected delegate void ExportDelegate(AnimationClip clip, string curveName, bool optimization, float optimizationTolerance, float scale);  
        
        protected abstract ClassLayout Layout { get; } 
        
        protected void OnInspectorGUIImpl(DrawDelegate draw, ExportDelegate export)
        {
            var clipProperty = serializedObject.FindProperty(Layout.Clip);
            var nameProperty = serializedObject.FindProperty(Layout.Name);
            var autoupdateProperty = serializedObject.FindProperty(Layout.Autoupdate);
            var optimizationProperty = serializedObject.FindProperty(Layout.Optimization);
            var optimizationToleranceProperty = serializedObject.FindProperty(Layout.OptimizationTolerance);
            var scaleProperty = serializedObject.FindProperty(Layout.Scale);

            var aux = _AuxData.GetOrCreate(serializedObject.targetObject);

            bool updateCurve = false;

            bool autoupdate = autoupdateProperty.boolValue;
            autoupdateProperty.boolValue = EditorGUILayout.ToggleLeft(new GUIContent(" Auto update"), autoupdateProperty.boolValue);
            if (autoupdateProperty.boolValue && !autoupdate)
                updateCurve = true;

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(clipProperty);
                if (scope.changed)
                {
                    var clip = clipProperty.objectReferenceValue as AnimationClip;
                    if (clip != null)
                    {
                        aux.CurvesNames = AnimationCurveExtractor.GetCurvesNames(clip).ToArray();
                        updateCurve = autoupdateProperty.boolValue;
                    }
                    else
                        aux.CurvesNames = null;
                }
            }

            if (aux.CurvesNames != null)
            {
                var selectedCurveIndex = Array.IndexOf(aux.CurvesNames, nameProperty.stringValue);
                var newSelectedCurveIndex = EditorGUILayout.Popup("Curve", selectedCurveIndex, aux.CurvesNames);
                if (newSelectedCurveIndex != selectedCurveIndex && newSelectedCurveIndex >= 0 && newSelectedCurveIndex < aux.CurvesNames.Length)
                {
                    nameProperty.stringValue = aux.CurvesNames[newSelectedCurveIndex];
                    updateCurve = autoupdateProperty.boolValue;
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.LabelField("Curve", nameProperty.stringValue, GUI.skin.GetStyle("popup"));
            }

            using (var scope = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(optimizationProperty);
                    EditorGUILayout.PropertyField(optimizationToleranceProperty, new GUIContent("Tolerance"));
                }
                EditorGUILayout.PropertyField(scaleProperty);
                if (scope.changed)
                    updateCurve = autoupdateProperty.boolValue;
            }
            
            if (!string.IsNullOrWhiteSpace(aux.Error))
                EditorGUILayout.HelpBox(aux.Error, MessageType.Error);

            if (aux.CurvesNames != null && !autoupdateProperty.boolValue)
                using (new EditorGUI.DisabledScope(Array.IndexOf(aux.CurvesNames, nameProperty.stringValue) == -1))
                    if (GUILayout.Button("Export"))
                        updateCurve = true;

            draw();

            if (updateCurve)
            {
                aux.Error = null;
                var clip = clipProperty.objectReferenceValue as AnimationClip;
                var curveName = nameProperty.stringValue;
                try
                {
                    export(clip, curveName, optimizationProperty.boolValue, optimizationToleranceProperty.floatValue, scaleProperty.floatValue);
                }
                catch (Exception e)
                {
                    aux.Error = e.Message;
                }
            }
        }
                
        private void OnEnable()
        {
            serializedObject.Update();
            AuxData aux = _AuxData.GetOrCreate(serializedObject.targetObject);
            var clip = serializedObject.FindProperty(nameof(ExportedAnimationCurve.Clip))?.objectReferenceValue as AnimationClip;
            aux.CurvesNames = clip != null ? AnimationCurveExtractor.GetCurvesNames(clip).ToArray() : null;
            var curveName = serializedObject.FindProperty(nameof(ExportedAnimationCurve.CurveName)).stringValue;
            try
            {
                aux.Error = null;
                if(clip != null && !string.IsNullOrEmpty(curveName))
                    AnimationCurveExtractor.ExtractCurve(clip, curveName);
            }
            catch (Exception e)
            {
                aux.Error = e.Message;
            }
        }

        protected struct ClassLayout
        {
            public string Clip;
            public string Name;
            public string Autoupdate;
            public string Optimization;
            public string OptimizationTolerance;
            public string Scale;
        }
        
        class AuxData
        {
            public string[] CurvesNames;
            public string Error;
        }
    }
}