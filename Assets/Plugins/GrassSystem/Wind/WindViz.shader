Shader "Wind Visualization" {
	Properties {


		[Enum(Main Wind,0,Turbulence,1,All,2)] _VizMode ("Visualization", Float) = 0
		_Transparency ("Opacity", Range(0,1)) = 0.5
		_Color ("Color", Color) = (1,1,1,1)
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float _VizMode;
		fixed _Transparency;

		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		CBUFFER_START(AtgGrass)
			sampler2D _AtgWindRT;
			float4 _AtgWindDirSize;
			float4 _AtgWindStrength;
			float2 _AtgSinTime;
			float4 _AtgGrassFadeProps;
			float4 _AtgGrassShadowFadeProps;
		CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

//IN.uv_MainTex += _Time.xx * float2(1, _Metallic * 2 - 1);


			fixed4 c = tex2D (_AtgWindRT, IN.worldPos.xz * _AtgWindDirSize.w).rgba; // * _Color;
			o.Albedo = _Color.rgb;
			if (_VizMode == 0) {
				o.Albedo *= c.rrr;	
			}
			else if (_VizMode == 1) {
				o.Albedo *= c.ggg;
			}
			else {
				o.Albedo *= c.rrg;
			}
			o.Alpha = _Transparency;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
