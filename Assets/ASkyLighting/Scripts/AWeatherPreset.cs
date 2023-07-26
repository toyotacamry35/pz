using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class AWeatherCloudsConfig {
	[Tooltip("Base color of clouds.")]
	public Color BaseColor = Color.white;
	[Tooltip("Light influence from direct lighting.")]
	public float DirectLightInfluence = 10f;
	[Tooltip("Density of clouds generated.")]
	[Range(-0.5f,0.5f)]
	public float Density = 0.1f;
	[Tooltip("Coverage rate of clouds generated.")]
	[Range(-1f,1f)]
	public float Coverage  = 1.0f; // Dense of clouds
	[Tooltip("Clouds alpha modificator.")]
	[Range(0f,10f)]
	public float Alpha = 1f;
}

[System.Serializable]
public class AWeatherEffects {
	public GameObject prefab;
	public Vector3 localPositionOffset;
	public Vector3 localRotationOffset;
}


[System.Serializable]
//[CreateAssetMenu(fileName = "EnviroProfile", menuName = "EnviroProfile",order =1)]
public class AWeatherPreset : ScriptableObject 
{
	public string version;
	public string Name;

	[Header("Cloud Settings")]
	public List<AWeatherCloudsConfig> cloudConfig = new List<AWeatherCloudsConfig>();

	[Range(0,1)][Tooltip("Wind intensity that will applied to wind zone.")]
	public float WindStrenght = 0.5f;

}

public class AWeatherPresetCreation {
#if UNITY_EDITOR
	[MenuItem("Assets/Create/Enviro/WeatherPreset")]

	public static void CreateMyAsset()
	{
		AWeatherPreset wpreset = ScriptableObject.CreateInstance<AWeatherPreset>();
		wpreset.Name = "New Weather Preset " + UnityEngine.Random.Range (0, 999999).ToString();

		wpreset.version = "1.9.0";
		// Create and save the new profile with unique name
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + "Weather Preset" + ".asset");
		AssetDatabase.CreateAsset (wpreset, assetPathAndName);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = wpreset;
	}
#endif
	public static GameObject GetAssetPrefab(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Contains(".prefab"))
			{
				return AssetDatabase.LoadAssetAtPath<GameObject>(path);
			}
		}
		#endif
		return null;
	}

	public static Cubemap GetAssetCubemap(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Contains(".png"))
			{
				return AssetDatabase.LoadAssetAtPath<Cubemap>(path);
			}
		}
		#endif
		return null;
	}

	public static Texture GetAssetTexture(string name)
	{
		#if UNITY_EDITOR
		string[] assets = AssetDatabase.FindAssets(name, null);
		for (int idx = 0; idx < assets.Length; idx++)
		{
			string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
			if (path.Length > 0)
			{
				return AssetDatabase.LoadAssetAtPath<Texture>(path);
			}
		}
		#endif
		return null;
	}
		
	public static Gradient CreateGradient()
	{
		Gradient nG = new Gradient ();
		GradientColorKey[] gClr = new GradientColorKey[2];
		GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

		gClr [0].color = Color.white;
		gClr [0].time = 0f;
		gClr [1].color = Color.white;
		gClr [1].time = 0f;

		gAlpha [0].alpha = 0f;
		gAlpha [0].time = 0f;
		gAlpha [1].alpha = 0f;
		gAlpha [1].time = 1f;

		nG.SetKeys (gClr, gAlpha);

		return nG;
	}
		
	public static Color GetColor (string hex)
	{
		Color clr = new Color ();	
		ColorUtility.TryParseHtmlString (hex, out clr);
		return clr;
	}
	
	public static Keyframe CreateKey (float value, float time)
	{
		Keyframe k = new Keyframe();
		k.value = value;
		k.time = time;
		return k;
	}

	public static Keyframe CreateKey (float value, float time, float inTangent, float outTangent)
	{
		Keyframe k = new Keyframe();
		k.value = value;
		k.time = time;
		k.inTangent = inTangent;
		k.outTangent = outTangent;
		return k;
	}
			
}
