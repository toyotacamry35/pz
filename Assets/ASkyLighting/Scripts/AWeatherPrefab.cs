using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AWeatherPrefab : MonoBehaviour 
{
	public AWeatherPreset weatherPreset;
	[HideInInspector]public List<ParticleSystem> effectSystems = new List<ParticleSystem>();
	[HideInInspector]public List<float> effectEmmisionRates = new List<float>();
}

