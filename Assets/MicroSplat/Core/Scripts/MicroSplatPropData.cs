﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// layout
// 0 perTex UV Scale and Offset
// 1 PerTex Tint and interpolation contrast (a)
// 2 Normal Strength, Smoothness, AO, Metallic
// 3 Brightness, Contrast, porosity, foam
// 4 DetailNoiseStrength, distance Noise Strength, Distance Resample, A OPEN
// 5 geoTex, tint strength, normal strength, A OPEN
// 6 displace, bias, offset (A) OPEN
// 7 Noise 0, Noise 1, Noise 2, Wind Particulate Strength
// 8 Snow (R), Glitter (G),  (BA) OPEN
// 9 Triplanar, trplanar contrast (BA) OPEN
// 10 Texture Cluster Contrast, boost, Height Offset, Height Contrast
// 11 Advanced Detail UV Scale/Offset
// 12 Advanced Detail (G)Normal Blend, (B)Tex Overlay (A) OPEN
// 13 Advanced Detail (R)Contrast, (G) AngleContrast, (B)HeightConttast, (A) OPEN
// 14 AntiTileArray (R)Normal Str, (G) Detail Strength, (B) Distance Strength (A) OPEN
// 15 Reserved for initialization marking

// because unity's HDR import pipeline is broke (assumes gamma, so breaks data in textures)
public class MicroSplatPropData : ScriptableObject 
{
   const int sMaxTextures = 32;
   [HideInInspector]
   public Color[] values = new Color[sMaxTextures*16];

   Texture2D tex;

   [HideInInspector]
   public AnimationCurve geoCurve = AnimationCurve.Linear(0, 0.0f, 0, 0.0f);
   Texture2D geoTex;

   void RevisionData()
   {
      // revision from 16 to 32 max textures
      if (values.Length == (16 * 16))
      {
         Color[] c = new Color[sMaxTextures * 16];
         for (int x = 0; x < 16; ++x)
         {
            for (int y = 0; y < 16; ++y)
            {
               c[y * sMaxTextures + x] = values[y * 16 + x];
            }
         }
         values = c;
         #if UNITY_EDITOR
         UnityEditor.EditorUtility.SetDirty(this);
         #endif
      }
   }

   public Color GetValue(int x, int y)
   {
      RevisionData();
      return values[y * sMaxTextures + x];
   }

   public void SetValue(int x, int y, Color c)
   {
      RevisionData();
      #if UNITY_EDITOR
      UnityEditor.Undo.RecordObject(this, "Changed Value");
      #endif

      values[y * sMaxTextures + x] = c;

      #if UNITY_EDITOR
      UnityEditor.EditorUtility.SetDirty(this);
      #endif
   }

   public void SetValue(int x, int y, int channel, float value)
   {
      RevisionData();
      #if UNITY_EDITOR
      UnityEditor.Undo.RecordObject(this, "Changed Value");
      #endif
      int index = y * sMaxTextures + x;
      Color c = values[index];
      c[channel] = value;
      values[index] = c;

      #if UNITY_EDITOR
      UnityEditor.EditorUtility.SetDirty(this);
      #endif
   }

   public Texture2D GetTexture()
   {
      RevisionData();
      if (tex == null)
      {
         if (Application.platform == RuntimePlatform.Switch)
         {
            tex = new Texture2D(sMaxTextures, 16, TextureFormat.RGBAHalf, false, true);
         }
         else
         {
            tex = new Texture2D(sMaxTextures, 16, TextureFormat.RGBAFloat, false, true);
         }
         tex.hideFlags = HideFlags.HideAndDontSave;
         tex.wrapMode = TextureWrapMode.Clamp;
         tex.filterMode = FilterMode.Point;

      }
      tex.SetPixels(values);
      tex.Apply();
      return tex;
   }

   public Texture2D GetGeoCurve()
   {
      if (geoTex == null)
      {
         geoTex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
         geoTex .hideFlags = HideFlags.HideAndDontSave;
      }
      for (int i = 0; i < 256; ++i)
      {
         float v = geoCurve.Evaluate((float)i / 255.0f);
         geoTex.SetPixel(i, 0, new Color(v, v, v, v));
      }
      geoTex.Apply();
      return geoTex;
   }
}

