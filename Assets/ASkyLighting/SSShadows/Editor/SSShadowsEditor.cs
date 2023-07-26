using UnityEngine;
using UnityEditor;

namespace Assets.ASkyLighting.SSShadows.Editor
{
	[CustomEditor(typeof(SSShadows))]
	public class SSShadowsEditor : UnityEditor.Editor
	{
		SerializedObject serObj;

		SerializedProperty sun;
		SerializedProperty blendStrength;
		SerializedProperty accumulation;
		SerializedProperty lengthFade;
		SerializedProperty range;
		SerializedProperty zThickness;
		SerializedProperty samples;
		SerializedProperty nearSampleQuality;
		SerializedProperty traceBias;
		SerializedProperty stochasticSampling;
		SerializedProperty leverageTemporalAA;
		SerializedProperty bilateralBlur;
		SerializedProperty blurPasses;
		SerializedProperty blurDepthTolerance;

		SSShadows instance;

		void OnEnable()
		{
			serObj = new SerializedObject(target);

			sun = serObj.FindProperty("sun");
			blendStrength = serObj.FindProperty("blendStrength");
			accumulation = serObj.FindProperty("accumulation");
			lengthFade = serObj.FindProperty("lengthFade");
			range = serObj.FindProperty("range");
			zThickness = serObj.FindProperty("zThickness");
			samples = serObj.FindProperty("samples");
			nearSampleQuality = serObj.FindProperty("nearSampleQuality");
			traceBias = serObj.FindProperty("traceBias");
			stochasticSampling = serObj.FindProperty("stochasticSampling");
			leverageTemporalAA = serObj.FindProperty("leverageTemporalAA");
			bilateralBlur = serObj.FindProperty("bilateralBlur");
			blurDepthTolerance = serObj.FindProperty("blurDepthTolerance");
			blurPasses = serObj.FindProperty("blurPasses");

			instance = target as SSShadows;
		}

		public override void OnInspectorGUI()
		{
			serObj.Update();

			EditorGUILayout.PropertyField(sun, new GUIContent("Sun", "The directional light that you want to have screen-space shadows applied to.")); //TODO: Warning when this isn't assigned
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(blendStrength, new GUIContent("Blend Strength", "The opacity of screen-space shadows after they're calculated."));
			EditorGUILayout.PropertyField(accumulation, new GUIContent("Accumulation", "How much each individual trace step accumulates and contributes to the shadow result."));
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(lengthFade, new GUIContent("Length Fade", "How much traced shadows should fade out as they near the maximum trace length."));
			EditorGUILayout.PropertyField(range, new GUIContent("Range", "How far to trace for screen-space shadows in world units."));
			EditorGUILayout.PropertyField(zThickness, new GUIContent("Z Thickness", "How thick objects in the depth buffer are assumed to be while calculating shadows."));
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(samples, new GUIContent("Samples", "How many trace steps to take while tracing for screen-space shadows."));
			EditorGUILayout.PropertyField(nearSampleQuality, new GUIContent("Near Sample Quality", "Higher values allow for more samples per-pixel for areas close to the camera."));
			EditorGUILayout.PropertyField(traceBias, new GUIContent("Trace Bias", "Similar to shadowmap bias, this can help reduce incorrect self-shadowing artifacts."));
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(stochasticSampling, new GUIContent("Stochastic Sampling", "Enables per-pixel random tracing offset in order to trade banding artifacts for noise."));
			EditorGUILayout.PropertyField(leverageTemporalAA, new GUIContent("Leverage Temporal AA", "Requires Stochastic Sampling. This causes the per-pixel random offset to vary for each frame. When used with Temporal Anti-Aliasing, this can greatly improve shadow smoothness."));
			EditorGUILayout.PropertyField(bilateralBlur, new GUIContent("Bilateral Blur", "Applies an edge-aware blur to the screen-space shadows result."));
			EditorGUILayout.PropertyField(blurPasses, new GUIContent("Blur Passes", "Number of bilateral blur passes to perform."));
			EditorGUILayout.PropertyField(blurDepthTolerance, new GUIContent("Blur Depth Tolerance", "The depth difference tolerance for bilateral blur."));

			serObj.ApplyModifiedProperties();
		}

	}
}