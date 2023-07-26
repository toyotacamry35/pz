Shader "AwesomeTechnologies/Terrain/TerrainHeatMap"
{
	Properties
	{
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}
		[MaterialToggle] _UseCurves("Use curves to evaluate area", Float) = 0
		_CurveTexture("Curve texture (RGBA)", 2D) = "white" {}
		_BackgroundTexture("Background texture (RGBA)", 2D) = "white" {}
		_SelectedTexture("Selected texture (RGBA)", 2D) = "green" {}
		_Selected("Selected color", Color) = (0.1,0.4,0.2,1)
		_Background("Background color", Color) = (0.6,0.6,0.6,1)
		[MaterialToggle] _UseTexture("Use Texture for fill", Float) = 0
		_MinHeight("MinHeight", float) = 50
		_MaxHeight("MaxHeight", float) = 100

		_MinSteepness("MinSteepness", Range(0, 90)) = 10
		_MaxSteepness("MaxSteepness", Range(0, 90)) = 25

		_TerrainMinHeight("Terrain 0/Sea height", float) = 0
		_TerrainMaxHeight("Terrain max height", float) = 1000
		_TerrainYPosition("Terrain Y position",float) = 0
	}

		SubShader{
		Tags{
		"Queue" = "Geometry-100"
		"RenderType" = "Opaque"
	}

		Lighting Off
		CGPROGRAM
#pragma surface surf SimpleLambert vertex:SplatmapVert fullforwardshadows
#pragma multi_compile_fog
#pragma target 4.0
#pragma exclude_renderers gles vulkan 
#include "UnityPBSLighting.cginc"

#pragma multi_compile __ _TERRAIN_NORMAL_MAP 

#define TERRAIN_STANDARD_SHADER
#define TERRAIN_SURFACE_OUTPUT SurfaceOutputStandard

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _SelectedTexture;
		sampler2D _BackgroundTexture;
		sampler2D _CurveTexture;
		half4 _Selected;
		half4 _Background;
		float _MinHeight;
		float _MaxHeight;
		float _MinSteepness;
		float _MaxSteepness;
		float _UseTexture;
		float _UseCurves;

		float _TerrainMinHeight;
		float _TerrainMaxHeight;
		float _TerrainYPosition;
		

		uniform float _HeightCurve[256];
		uniform float _SteepnessCurve[256];

		struct Input
		{
			float2 tc_Control : TEXCOORD4;
			float3 worldPos;
			float3 worldNormal;
			UNITY_FOG_COORDS(5)
		};

		void SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);	
			float4 pos = UnityObjectToClipPos(v.vertex);//  mul(UNITY_MATRIX_MVP, v.vertex);
			UNITY_TRANSFER_FOG(data, pos);
			v.tangent.xyz = cross(v.normal, float3(0, 0, 1));
			v.tangent.w = -1;
		}

		half4 LightingSimpleLambert(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half4 c;
			//c.rgb = s.Albedo *_LightColor0.rgb * (NdotL * atten * 2);
			c.rgb = s.Albedo * (NdotL * 2);
			c.a = s.Alpha;
			return c;
		}

		float Perlin2D(float2 P)
		{
			//  https://github.com/BrianSharpe/Wombat/blob/master/Perlin2D.glsl
			P = P * 2 + float2(5000, 5000);
			// establish our grid cell and unit position
			float2 Pi = floor(P);
			float4 Pf_Pfmin1 = P.xyxy - float4(Pi, Pi + 1.0);

			// calculate the hash
			float4 Pt = float4(Pi.xy, Pi.xy + 1.0);

			Pt = Pt - floor(Pt * (1.0 / 71.0)) * 71.0;
			Pt += float2(26.0, 161.0).xyxy;
			Pt *= Pt;
			Pt = Pt.xzxz * Pt.yyww;

			float4 hash_x = frac(Pt * (1.0 / 951.135664));
			float4 hash_y = frac(Pt * (1.0 / 642.949883));

			// calculate the gradient results
			float4 grad_x = hash_x - 0.49999;
			float4 grad_y = hash_y - 0.49999;
			float4 grad_results = rsqrt(grad_x * grad_x + grad_y * grad_y) * (grad_x * Pf_Pfmin1.xzxz + grad_y * Pf_Pfmin1.yyww);

			// Classic Perlin Interpolation
			grad_results *= 1.4142135623730950488016887242097;  // scale things to a strict -1.0->1.0 range  *= 1.0/sqrt(0.5)

			float2 blend = Pf_Pfmin1.xy * Pf_Pfmin1.xy * Pf_Pfmin1.xy * (Pf_Pfmin1.xy * (Pf_Pfmin1.xy * 6.0 - 15.0) + 10.0);
			float4 blend2 = float4(blend, float2(1.0 - blend));

			return dot(grad_results, blend2.zxzx * blend2.wwyy);
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float steepness = acos(dot(IN.worldNormal, float3(0, 1, 0))) *  57.29578;
			half4 color = _Background;
			
			if (_UseCurves == 1)
			{
				float height = IN.worldPos.y - _TerrainMinHeight - _TerrainYPosition;
				float normalizedHeight = height / (_TerrainMaxHeight - _TerrainMinHeight);
				float normalizedSteepness = steepness / 90;

				float sampledHeight = _HeightCurve[floor(normalizedHeight * 256)];
				float sampledSteepness = _SteepnessCurve[floor(normalizedSteepness * 256)];
				float combinedValue = sampledHeight * sampledSteepness;
				//combinedValue = combinedValue * Perlin2D(IN.worldPos.xz / 100);

				o.Albedo = lerp(_Background, _Selected, combinedValue);
				o.Alpha = 1;
			}
			else
			{
				float adjustedMinHeight = _MinHeight + _TerrainMinHeight;
				float adjustedMaxHeight = _MaxHeight + _TerrainMinHeight;

				if (_UseTexture == 1)
				{
					color = tex2D(_BackgroundTexture, IN.tc_Control);
				}

				if (IN.worldPos.y > adjustedMinHeight && IN.worldPos.y < adjustedMaxHeight  &&
					steepness >= _MinSteepness && steepness <= _MaxSteepness)
				{
					if (_UseTexture == 1)
					{
						color = tex2D(_SelectedTexture, IN.tc_Control);
					}
					else
					{
						color = _Selected;
					}
				}
			
				o.Albedo = color;
				o.Alpha = 1;
			}
		}
		ENDCG
	}
	Dependency "BaseMapShader" = "AwesomeTechnologies/Terrain/TerrainHeatMap"
	Fallback "Nature/Terrain/Diffuse"
}