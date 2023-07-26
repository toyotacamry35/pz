Shader "DeepSky Haze/Skybox Transparent - Full" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Intensity("Intensity", Range(0.0, 8.0)) = 1
	}
		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 200

			CGPROGRAM
			#pragma surface surf SkyboxUnlit noforwardadd noshadow noambient nolightmap nofog vertex:DS_atmospherics_vtx finalcolor:DS_atmospherics_skybox exclude_path:deferred alpha:fade
			#pragma target 3.0	
			#pragma shader_feature DS_HAZE_APPLY_PER_FRAGMENT
			#define DS_HAZE_USE_FULL_DEPTH
			#define DS_HAZE_FULL
			// -------------------------------------------------------------------------- //

		#include "UnityCG.cginc"
		#include "DS_TransparentLib.cginc"

		UNITY_DECLARE_TEX2D(_MainTex);
		fixed4 _Color;
		fixed _Intensity;

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

		inline void DS_atmospherics_skybox(Input IN, SurfaceOutput o, inout fixed4 colour) {
			DEEPSKY_HAZE_FINAL_COLOR(IN, o, colour);
		}

		void surf (Input IN, inout SurfaceOutput o) {

			fixed4 c = UNITY_SAMPLE_TEX2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c * _Intensity;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"

	CustomEditor "DeepSky.Haze.DS_TransparentGUI"
}
