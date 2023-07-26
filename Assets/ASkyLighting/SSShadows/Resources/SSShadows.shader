// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SSShadows" 
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{

		Cull Off ZWrite Off ZTest Always

		CGINCLUDE

		#pragma vertex vert
		#pragma fragment frag
			
		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}


		float fixDepth(float depth)
		{
			#if defined(UNITY_REVERSED_Z)
				return 1.0 - depth;
			#else
				return depth;
			#endif
		}


		ENDCG

		Pass 
		{
			Blend DstColor Zero

			CGPROGRAM
			
			sampler2D SSShadowsTexture;

			fixed4 frag (v2f input) : SV_Target
			{
				fixed4 col = tex2D(SSShadowsTexture, input.uv).xxxx;
				
				return col;
			}
			ENDCG
		}

		Pass 
		{
			CGPROGRAM
			#pragma target 5.0
			
			Texture2D<float> _MainTex;
			sampler2D _CameraGBufferTexture2;

			SamplerState sampler_CameraDepthTexture;
			SamplerState sampler_MainTex;

			Texture2D<float> _CameraDepthTexture;

			float4x4 ProjectionMatrix;
			float4x4 ProjectionMatrixInverse;
			float4x4 WorldToCamera;

			float3 SunlightVector;
			float4 ScreenRes;

			float BlendStrength;
			float Accumulation;
			float Range;
			int Samples;
			float TraceBias;
			float ZThickness;
			float StochasticSampling;
			int TJitter;
			float LengthFade;
			float NearQualityCutoff;

			float4 GetViewSpacePosition(float2 coord)
			{
				float depth = fixDepth(_MainTex.SampleLevel(sampler_MainTex, coord.xy, 0));

				float4 viewPosition = mul(ProjectionMatrixInverse, float4(coord.x * 2.0 - 1.0, coord.y * 2.0 - 1.0, 2.0 * depth - 1.0, 1.0));
				viewPosition /= viewPosition.w;

				return viewPosition;
			}

			float3 ProjectBack(float4 viewPos)
			{
				viewPos = mul(ProjectionMatrix, float4(viewPos.xyz, 0.0));
				viewPos.xyz /= viewPos.w;
				viewPos.xyz = viewPos.xyz * 0.5 + 0.5;
				return viewPos.xyz;
			}

			float rand(float2 coord)
			{
				return saturate(frac(sin(dot(coord, float2(12.9898, 78.223))) * 43758.5453));
			}

			float3 CameraDir;

			fixed4 frag (v2f input) : SV_Target
			{

				float3 viewPos = GetViewSpacePosition(input.uv.xy);
				float3 rayPos = viewPos;
				float3 rayDir = -SunlightVector * float3(1.0, 1.0, -1.0);

				float shadow = 1.0;

				rayPos += rayDir * -viewPos.z * 0.000037 * TraceBias;

				rayDir *= (Range / Samples);
				rayDir *= -viewPos.z * 0.1 + NearQualityCutoff;

				rayPos += rayDir * rand(input.uv.xy + TJitter * 0.1) * StochasticSampling;

				int numSamples = Samples;
				numSamples /= -viewPos.z * 0.125 + NearQualityCutoff;

				for (int i = 0; i < numSamples; i++)
				{
					float fi = (float)i / numSamples;
					rayPos += rayDir;
					float3 rayProjPos = ProjectBack(float4(rayPos, 0.0));

					float sampleDepth = LinearEyeDepth(_MainTex.SampleLevel(sampler_MainTex, rayProjPos.xy, 0));

					float depthDiff = -rayPos.z - sampleDepth - 0.02 * (-viewPos.z) * TraceBias; 

					if (depthDiff > 0.0 && depthDiff < ZThickness)
					{
						float shadowStrength = pow(1.0 - fi, LengthFade);

						shadowStrength *= Accumulation;

						shadow *= 1.0 - shadowStrength;
					}
				}

				shadow = saturate(shadow * 1.0);

				shadow = lerp(1.0, shadow, BlendStrength);

				return shadow.xxxx;

			}
			ENDCG
		}

		Pass // 2 get depth with padding
		{
			CGPROGRAM
			
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			float4 ScreenRes;

			fixed4 frag (v2f input) : SV_Target
			{
				float depth = tex2D(_CameraDepthTexture, input.uv.xy).x;

				if (	input.vertex.x <= 1.0 || input.vertex.x >= ScreenRes.x - 1.0
					||  input.vertex.y <= 1.0 || input.vertex.y >= ScreenRes.y - 1.0)
				{
					#if defined(UNITY_REVERSED_Z)
						depth = 0.0;
					#else
						depth = 1.0;
					#endif
				}

				return depth.xxxx;
			}
			ENDCG
		}

		Pass // 3 bilateral blur
		{
			CGPROGRAM			

			Texture2D<float> _CameraDepthTexture;
			SamplerState sampler_CameraDepthTexture;

			sampler2D _MainTex;
			sampler2D _CameraGBufferTexture2;
			float4x4 ProjectionMatrixInverse;
			float4 ScreenRes;
			float2 SSSBlurKernel;
			float BlurDepthTolerance;

			float4 frag(v2f input) : COLOR0
			{
				float4 blurred = float4(0.0, 0.0, 0.0, 0.0);
				float validWeights = 0.0;
				float depth = LinearEyeDepth(_CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, input.uv.xy, 0));
				float thresh = 0.16 * BlurDepthTolerance;


				for (int i = -1; i <= 1; i++)
				{
					float2 offs = SSSBlurKernel.xy * (i)* ScreenRes.zw * 1.0;
					float sampleDepth = LinearEyeDepth(_CameraDepthTexture.SampleLevel(sampler_CameraDepthTexture, input.uv.xy + offs.xy, 0));

					float weight = saturate(1.0 - abs(depth - sampleDepth) / thresh);

					float4 blurSample = tex2Dlod(_MainTex, float4(input.uv.xy + offs.xy, 0, 0)).rgba;
					blurred += blurSample * weight;
					validWeights += weight;
				}

				blurred /= validWeights + 0.001;

				return blurred;
			}

			ENDCG
		}
	}
	Fallback "Hidden/SSShadowsFallback"
}
