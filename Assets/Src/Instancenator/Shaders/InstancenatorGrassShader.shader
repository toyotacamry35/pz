Shader "Instancenator/GrassShader"
{
	Properties
	{
		_MainTex("Albedo (RGBA)", 2D) = "white" {}
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
			float3 color : COLOR;
		};


		#include "InstancenatorShaderBase.cginc"
		
		void vert(inout appdata_full v)
		{
			InstanceData data = GetInstanceData();
			v.vertex.xyz *= data.instanceValue.w * instanceTransitionScale;
			//v.color.rgb = data.instanceValue.xyz;
		}

		sampler2D _MainTex;
		half _Glossiness;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			clip(c.a - 0.1);
			o.Albedo = c.rgb;// *IN.color * 2;
			o.Metallic = 0;
			o.Smoothness = _Glossiness;
			o.Emission = 0;// dot(IN.color.rgb, 0.33);
			o.Alpha = 1;			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
