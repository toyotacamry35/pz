using System;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

[Serializable]
public class GrassMotionRenderer
{
	private static readonly List<GrassMotionZone> _zones = new List<GrassMotionZone>();
    public static List<GrassMotionZone> _zonesToDelete = new List<GrassMotionZone>();
    public RenderTexture _grassMotionRenderTexture;
	private Material _material;
    public int texSize = 256;

	public static void AddZone(GrassMotionZone zone)
	{
		_zones.Add(zone);
	}

	public static void RemoveZone(GrassMotionZone zone)
	{
		_zones.Remove(zone);
	}

	public void OnEnable()
	{
		_grassMotionRenderTexture = new RenderTexture(Mathf.RoundToInt(texSize), Mathf.RoundToInt(texSize), 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

		_material = new Material(Shader.Find("Hidden/GrassMotionZone"));
		Shader.SetGlobalVector("_RenderTargetSize", new Vector2(_grassMotionRenderTexture.width/4, _grassMotionRenderTexture.height/4));
	}

	public void OnDisable()
	{
		Object.DestroyImmediate(_grassMotionRenderTexture);
		Object.DestroyImmediate(_material);
	}



	public void OnRenderObject()
	{
        
		var activeRenderTarget = RenderTexture.active;

		Graphics.SetRenderTarget(_grassMotionRenderTexture);
		GL.Clear(true, true, Color.gray);

        _zonesToDelete = new List<GrassMotionZone>();

        int count = _zones.Count;
        for (int i=0; i< count; i++)
		{
            _zones[i].Render(_material);
		}

        count = _zonesToDelete.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.Destroy(_zonesToDelete[i]);
        }
        RenderTexture.active = activeRenderTarget;

		Shader.SetGlobalTexture("_GrassMotionTexture", _grassMotionRenderTexture);
        Graphics.SetRenderTarget(null);
    }

   
}