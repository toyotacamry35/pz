Shader "DS_Transparent" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _SpecularGloss("Specular (R), Smoothness (A)", 2D) = "black" {}
		[Normal][NoScaleOffset] _Normal("Normal Map", 2D) = "bump" {}
		_Specular("Specular", Range(0, 1)) = 0
		_Gloss ("Gloss", Range(0, 1)) = 0.2
	}
	SubShader{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf StandardSpecular fullforwardshadows exclude_path:deferred nofog alpha:fade
		#pragma target 3.0

		#pragma shader_feature DS_HAZE_APPLY_PER_FRAGMENT

		#define DS_HAZE_FULL
		

		#include "UnityCG.cginc"
		#include "DS_TransparentLib.cginc"

		UNITY_DECLARE_TEX2D(_MainTex);
		fixed4 _Color;


		UNITY_DECLARE_TEX2D(_SpecularGloss);
		UNITY_DECLARE_TEX2D(_Normal);


		struct Input {
			float2 uv_MainTex;
			DEEPSKY_HAZE_DECLARE_INPUT;
		};


		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {

			fixed4 c = UNITY_SAMPLE_TEX2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c;

			fixed4 sg = UNITY_SAMPLE_TEX2D(_SpecularGloss, IN.uv_MainTex);
			o.Specular = sg.r;
			o.Smoothness = sg.a;



			half4 nmt = UNITY_SAMPLE_TEX2D(_Normal, IN.uv_MainTex);
			o.Normal = UnpackNormal(nmt);

			o.Alpha = c.a;
			
			DEEPSKY_HAZE_FINAL_COLOR(IN, o, c);
		}
		ENDCG
	} 
	FallBack "Diffuse"

	CustomEditor "DeepSky.Haze.DS_TransparentGUI"
}
