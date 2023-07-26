Shader "Instancenator/FoliageShader"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Normals("Normals", 2D) = "black" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
	}
	SubShader{
		Cull off

		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#pragma target 4.6			
		#pragma multi_compile_instancing
		#pragma vertex vert
		#pragma surface surf Standard addshadow fullforwardshadows
		#pragma instancing_options procedural:setup


		struct Input {
			float2 uv_MainTex;
			float2 uv2;
			float2 uv3;
			float2 uv4;
			float3 color : COLOR;
		};


		#include "InstancenatorShaderBase.cginc"


		void vert(inout appdata_full v)
		{
			InstanceData data = GetInstanceData();
			/*
			float kWeights = dot(v.color.rgb, 0.33);

			float3 windDir = float3(1, 0, 0);
			
			windDir *= 0.5 + _CosTime.w;

			windDir = mul((float3x3)unity_WorldToObject, windDir * kWeights);

			v.vertex.xyz += windDir;*/

			v.vertex.xyz *= data.instanceValue.w * instanceTransitionScale;

			v.color.rgb = data.instanceValue.xyz;
		}

		sampler2D _MainTex;
		sampler2D _Normals;
		half _Glossiness;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			clip(c.a - 0.1);
			o.Albedo = c.rgb;// *IN.color * 2;
			o.Normal = UnpackNormal(tex2D(_Normals, IN.uv_MainTex));
			o.Metallic = 0;
			o.Smoothness = _Glossiness;
			o.Emission = 0;// IN.color;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
