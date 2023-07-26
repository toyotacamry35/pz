Shader "DeepSky Haze/Standard Transparent - Full" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _MetallicGloss("Metal (R), Smoothness (A)", 2D) = "black" {}
		[Normal][NoScaleOffset] _Normal("Normal Map", 2D) = "bump" {}
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Gloss("Gloss", Range(0, 1)) = 0.2
	}
	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:DS_atmospherics_vtx finalcolor:DS_atmospherics_standard exclude_path:deferred nofog alpha:fade
		#pragma target 3.0
		#pragma shader_feature _DS_HAZE_METAL_GLOSS_TEX
		#pragma shader_feature _DS_HAZE_NORMAL_TEX
		#pragma shader_feature DS_HAZE_APPLY_PER_FRAGMENT

		#define DS_HAZE_FULL

		#include "UnityCG.cginc"
		#include "DS_TransparentLib.cginc"

		UNITY_DECLARE_TEX2D(_MainTex);
		fixed4 _Color;

#if defined (_DS_HAZE_METAL_GLOSS_TEX)
		UNITY_DECLARE_TEX2D(_MetallicGloss);
#else
		fixed _Metallic;
		fixed _Gloss;
#endif
#if defined (_DS_HAZE_NORMAL_TEX)
		UNITY_DECLARE_TEX2D(_Normal);
#endif

		struct Input {
			float2 uv_MainTex;
			DEEPSKY_HAZE_DECLARE_INPUT;
		};

		inline void DS_atmospherics_vtx(inout appdata_full vtx, out Input output)
		{
			UNITY_INITIALIZE_OUTPUT(Input, output);

			#if !defined (DS_HAZE_APPLY_PER_FRAGMENT)
			DEEPSKY_HAZE_VERTEX_MOD(vtx, output);
			#endif // Do nothing if computing atmospherics per-fragment.
		}

		inline void DS_atmospherics_standard(Input IN, SurfaceOutputStandard o, inout fixed4 colour) {
			DEEPSKY_HAZE_FINAL_COLOR(IN, o, colour);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed4 c = UNITY_SAMPLE_TEX2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c;

#if defined (_DS_HAZE_METAL_GLOSS_TEX)
			fixed4 mg = UNITY_SAMPLE_TEX2D(_MetallicGloss, IN.uv_MainTex);
			o.Metallic = mg.r;
			o.Smoothness = mg.a;
#else
			o.Metallic = _Metallic;
			o.Smoothness = _Gloss;
#endif

#if defined (_DS_HAZE_NORMAL_TEX)
			half4 nmt = UNITY_SAMPLE_TEX2D(_Normal, IN.uv_MainTex);
			o.Normal = UnpackNormal(nmt);
#endif
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"

	CustomEditor "DeepSky.Haze.DS_TransparentGUI"
}
