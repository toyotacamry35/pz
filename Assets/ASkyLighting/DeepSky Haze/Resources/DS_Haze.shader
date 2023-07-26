// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/DS_Haze" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "black" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"
#include "DS_Lib.cginc"

		// [START] Helper defines.
		// Defines for dealing with texture sampling at specified mip level, for D3D and OpenGL compatibility.
#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_METAL)
#define DS_SAMPLE_TEX2D_LOD(tex,coord,level) tex.SampleLevel (sampler##tex,coord,level)
#elif defined(SHADER_API_PSSL)
#define DS_SAMPLE_TEX2D_LOD(tex,coord,level) tex.SampleLOD (sampler##tex,coord,level)
#else
#define DS_SAMPLE_TEX2D_LOD(tex,coord,level) tex2Dlod (tex, half4(coord, level))
#endif

// Constants
#define kGaussianDepthFalloff _SamplingParams.x
#define kUpsampleDepthThreshold _SamplingParams.y
#define kTemporalRejectionScale _SamplingParams.z
#define kTemporalBlendFactor _SamplingParams.w

// [START] Shader params.
// Textures/Samplers
UNITY_DECLARE_TEX2D(_MainTex);
	uniform fixed4 _MainTex_TexelSize;
	UNITY_DECLARE_TEX2D(_HalfResDepth);
	uniform fixed4 _HalfResDepth_TexelSize;
	UNITY_DECLARE_SHADOWMAP(_ShadowCascades);

	// Upsampling and temporal reprojection parameters.
	uniform fixed4 _SamplingParams; // (Gaussian Depth Falloff, Upsample Depth Threshold, Temporal Rejection Scale, Temporal Blend Factor)
	// [END] Shader params.

	// [START] Vertex->Fragment structs
	struct v2f_image
	{
		fixed4 pos : SV_POSITION;
		fixed3 uv : TEXCOORD0;
	};

	struct f2mrt_colour_depth
	{
		fixed4 colour : SV_Target0;
		fixed4 depth : SV_Target1;
	};

	struct f2mrt_compose
	{
		fixed4 colour : SV_Target0;
		fixed4 radiance : SV_Target1;
	};
	// [END] Vertex->Fragment structs

	// [START] Shared vertex programs.
	// Vertex program for basic image effect.
	v2f_image DSVP_Image(appdata_img vtx)
	{
		v2f_image output;
#if defined (UNITY_5_4_OR_NEWER)
		output.pos = UnityObjectToClipPos(vtx.vertex.xyz);
#else
		output.pos = UnityObjectToClipPos(vtx.vertex);
#endif
		output.uv = fixed3(vtx.texcoord.xy, 1);

#if defined(UNITY_UV_STARTS_AT_TOP)
		if (_MainTex_TexelSize.y < 0)
		{
			output.uv.y = 1 - output.uv.y;
		}
#endif

		return output;
	}
	// [END] Shared vertex programs.

	// [START] Cascade shadow map helpers. See Internal-PrePassCollectShadows in standard shaders for source.
	// Get the cascade weights when using Close fit projection.
	inline fixed4 getCascadeWeights(fixed3 wpos, fixed wz)
	{
		fixed4 zNear = fixed4(wz >= _LightSplitsNear);
		fixed4 zFar = fixed4(wz < _LightSplitsFar);
		fixed4 weights = zNear * zFar;
		return weights;
	}

	// Get the cascade weights when using Stable fit projection.
	inline fixed4 getCascadeWeights_splitSpheres(fixed3 wpos)
	{
		fixed3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
		fixed3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
		fixed3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
		fixed3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
		fixed4 distances2 = fixed4(dot(fromCenter0, fromCenter0), dot(fromCenter1, fromCenter1), dot(fromCenter2, fromCenter2), dot(fromCenter3, fromCenter3));
		//#if !defined(SHADER_API_D3D11)
		fixed4 weights = fixed4(distances2 < unity_ShadowSplitSqRadii);
		weights.yzw = saturate(weights.yzw - weights.xyz);
		//#else
		//fixed4 weights = fixed4(distances2 >= unity_ShadowSplitSqRadii);
		//#endif
		return weights;
	}
	// [END] Cascade shadow map helpers.

	// [START] Gaussian blur.
	// Separable Gaussian blur weights (7 taps).
	static const fixed kDSGaussWeights[8] = { 0.14446445f, 0.13543542f, 0.11153505f, 0.08055309f, 0.05087564f, 0.02798160f, 0.01332457f, 0.00545096f };

	// Main Depth-aware Gaussian blur. Weights are scaled according to depth differences to prevent
	// blur leaking over large depth discontinuities.
	// blurAxis: axis in which to perform this blur (U or V).
	fixed4 DS_SeparableBlur(v2f_image input, fixed2 blurAxis)
	{
		fixed4 colour = UNITY_SAMPLE_TEX2D(_MainTex, input.uv) * kDSGaussWeights[0];

		fixed accumWeights = kDSGaussWeights[0];
		fixed centerDepth = Linear01Depth(UNITY_SAMPLE_TEX2D(_HalfResDepth, input.uv));
		//fixed centerDepth = Linear01Depth(tex2D(_HalfResDepth, input.uv));

		fixed2 texelOffset = blurAxis * _MainTex_TexelSize.xy;
		fixed3 offset = fixed3(0, 0, 0);
		fixed kernelDepth = 0;
		fixed4 kernelColour = fixed4(0, 0, 0, 0);
		fixed depthDelta = 0;
		fixed depthR = 0;
		fixed depthG = 0;
		fixed depthWeight = 0;

		// Depth aware blur.
		UNITY_UNROLL
			for (int s = 1; s < 8; s++)
			{
				// Positive direction
				offset = fixed3(texelOffset * s, 0);
				kernelDepth = Linear01Depth(DS_SAMPLE_TEX2D_LOD(_HalfResDepth, input.uv + offset, 0).r);
				kernelColour = DS_SAMPLE_TEX2D_LOD(_MainTex, input.uv + offset, 0);

				depthDelta = abs(kernelDepth - centerDepth);
				depthR = depthDelta * kGaussianDepthFalloff;
				depthG = exp(-depthR * depthR);
				depthWeight = kDSGaussWeights[s] * depthG;
				colour += kernelColour * depthWeight;
				accumWeights += depthWeight;

				// Negative direction
				kernelDepth = Linear01Depth(DS_SAMPLE_TEX2D_LOD(_HalfResDepth, input.uv - offset, 0).r);
				kernelColour = DS_SAMPLE_TEX2D_LOD(_MainTex, input.uv - offset, 0);

				depthDelta = abs(kernelDepth - centerDepth);
				depthR = depthDelta * kGaussianDepthFalloff;
				depthG = exp(-depthR * depthR);
				depthWeight = kDSGaussWeights[s] * depthG;
				colour += kernelColour * depthWeight;
				accumWeights += depthWeight;
			}
		return fixed4(colour / accumWeights);
	}
	// [END] Gaussian blur.
	ENDCG

		SubShader{

			Cull Off ZWrite Off ZTest Always

			
			Pass // 1
			{
				Blend One One

				name "DS_RenderLightShafts_24_Half"
				Cull Off ZWrite Off ZTest Always

				CGPROGRAM
				#pragma target 3.0
				#pragma vertex DSVP_RayMarch
				#pragma fragment DSFP_Render
				#pragma shader_feature SHADOW_PROJ_CLOSE
				#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE

				#define kVolumeSamples 24
				#define kDownsampleFactor 2
				#include "DS_RayMarchLib.cginc"

				fixed4 DSFP_Render(v2f_ray input) : SV_Target
				{
					return DS_RenderLightShafts(input);
				}
				ENDCG
			}

			Pass // 2
			{
				Blend One One

				name "DS_RenderLightShafts_32_Half"
				Cull Off ZWrite Off ZTest Always

				CGPROGRAM
				#pragma target 3.0
				#pragma vertex DSVP_RayMarch
				#pragma fragment DSFP_Render
				#pragma shader_feature SHADOW_PROJ_CLOSE
				#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE

				#define kVolumeSamples 32
				#define kDownsampleFactor 2
				#include "DS_RayMarchLib.cginc"

				fixed4 DSFP_Render(v2f_ray input) : SV_Target
				{
					return DS_RenderLightShafts(input);
				}
				ENDCG
			}

			Pass // 3 (new)
				{
					Blend One One

					name "DS_RenderLightShafts_64_Half"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
				#pragma target 3.0
				#pragma vertex DSVP_RayMarch
				#pragma fragment DSFP_Render
				#pragma shader_feature SHADOW_PROJ_CLOSE
				#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE

				#define kVolumeSamples 64
				#define kDownsampleFactor 2
				#include "DS_RayMarchLib.cginc"

					fixed4 DSFP_Render(v2f_ray input) : SV_Target
				{
					return DS_RenderLightShafts(input);
				}
					ENDCG
				}

			
			Pass // 5 (4)
			{
				Blend One One

				name "DS_RenderLightShafts_24_Quarter"
				Cull Off ZWrite Off ZTest Always

				CGPROGRAM
				#pragma target 3.0
				#pragma vertex DSVP_RayMarch
				#pragma fragment DSFP_Render
				#pragma shader_feature SHADOW_PROJ_CLOSE
				#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE

				#define kVolumeSamples 24
				#define kDownsampleFactor 4
				#include "DS_RayMarchLib.cginc"

				fixed4 DSFP_Render(v2f_ray input) : SV_Target
				{
					return DS_RenderLightShafts(input);
				}
				ENDCG
			}

			Pass // 6 (5)
			{
				Blend One One

				name "DS_RenderLightShafts_32_Quarter"
				Cull Off ZWrite Off ZTest Always

				CGPROGRAM
				#pragma target 3.0
				#pragma vertex DSVP_RayMarch
				#pragma fragment DSFP_Render
				#pragma shader_feature SHADOW_PROJ_CLOSE
				#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE

				#define kVolumeSamples 32
				#define kDownsampleFactor 4
				#include "DS_RayMarchLib.cginc"

				fixed4 DSFP_Render(v2f_ray input) : SV_Target
				{
					return DS_RenderLightShafts(input);
				}
				ENDCG
			}

			Pass // 7 (5)
				{
					Blend One One

					name "DS_RenderLightShafts_64_Quarter"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_RayMarch
					#pragma fragment DSFP_Render
					#pragma shader_feature SHADOW_PROJ_CLOSE
					#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE

					#define kVolumeSamples 64
					#define kDownsampleFactor 4
					#include "DS_RayMarchLib.cginc"

					fixed4 DSFP_Render(v2f_ray input) : SV_Target
				{
					return DS_RenderLightShafts(input);
				}
					ENDCG
				}

			Pass // 8 (6)
			{
					// Gaussian blur - X axis.
					name "DS_BilateralBlur_X"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_Image
					#pragma fragment DSFP_BlurXAxis

					fixed4 DSFP_BlurXAxis(v2f_image i) : SV_Target{ return DS_SeparableBlur(i, fixed2(1, 0)); }
					ENDCG
				}

				Pass // 9 (7)
				{
					// Gaussian blur - Y axis.
					name "DS_BilateralBlur_Y"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_Image
					#pragma fragment DSFP_BlurYAxis

					fixed4 DSFP_BlurYAxis(v2f_image i) : SV_Target{ return DS_SeparableBlur(i, fixed2(0, 1)); }
					ENDCG
				}

				Pass // 10 (8)
				{
					// Perform nearest-depth upscaling and temporal reprojection to accumulate the
					// final volumetric radiance contribution this frame.
					// If the camera is rendering to the full screen then we can use MRTs to output
					// the radiance and the final compose at the same time.
					name "DS_TemporalUpscale"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_RayMarch
					#pragma fragment DSFP_UpscaleCompose
					#pragma shader_feature DS_HAZE_HEIGHT_FALLOFF_NONE
					#pragma multi_compile _ DS_HAZE_APPLY_RAYLEIGH
					#pragma multi_compile _ DS_HAZE_APPLY_MIE
					#pragma multi_compile _ DS_HAZE_APPLY_FOG_EXTINCTION
					#pragma multi_compile _ DS_HAZE_APPLY_FOG_RADIANCE
					#pragma multi_compile _ DS_HAZE_TEMPORAL
					#pragma shader_feature SHOW_TEMPORAL_REJECTION
					#pragma shader_feature SHOW_UPSAMPLE_THRESHOLD

					#include "DS_RayMarchLib.cginc"

					UNITY_DECLARE_TEX2D(_RadianceBuffer);

		#if defined(DS_HAZE_TEMPORAL)
					UNITY_DECLARE_TEX2D(_PrevAccumBuffer);
					UNITY_DECLARE_TEX2D(_PrevDepthBuffer);

					uniform fixed4x4 _PreviousViewProjMatrix;
					uniform fixed4x4 _PreviousInvViewProjMatrix;
					uniform fixed _TemporalRejectionScale;

					f2mrt_compose DSFP_UpscaleCompose(v2f_ray input)
					{
						f2mrt_compose output;
		#else
				// Non-temporal version just outputs final radiance.
				fixed4 DSFP_UpscaleCompose(v2f_ray input) : SV_Target
				{
	#endif
					fixed4 radiance = fixed4(0, 0, 0, 0);
	#if defined (SHOW_UPSAMPLE_THRESHOLD) || defined (SHOW_TEMPORAL_REJECTION)
					fixed debugOutput = 0;
	#endif
					fixed depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv);
					fixed linearDepth = Linear01Depth(depth);

					// [START] Nearest-depth upsampling.
	#if defined (SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)
					fixed2 uvTap_0 = input.uv.xy - _HalfResDepth_TexelSize.xy * 0.5f;
					fixed2 uvTap_1 = uvTap_0 + fixed2(1, 0) * _HalfResDepth_TexelSize.xy;
					fixed2 uvTap_2 = uvTap_0 + fixed2(0, 1) * _HalfResDepth_TexelSize.xy;
					fixed2 uvTap_3 = uvTap_0 + fixed2(1, 1) * _HalfResDepth_TexelSize.xy;

					fixed depthTaps[4] = { 0, 0, 0, 0 };
					depthTaps[0] = Linear01Depth(UNITY_SAMPLE_TEX2D(_HalfResDepth, uvTap_0));
					depthTaps[1] = Linear01Depth(UNITY_SAMPLE_TEX2D(_HalfResDepth, uvTap_1));
					depthTaps[2] = Linear01Depth(UNITY_SAMPLE_TEX2D(_HalfResDepth, uvTap_2));
					depthTaps[3] = Linear01Depth(UNITY_SAMPLE_TEX2D(_HalfResDepth, uvTap_3));
					// Sample the radiance taps using the depth buffer's sampler to avoid hardware bilinear filtering.
					fixed4 radianceTaps[4] = { fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0) };
					radianceTaps[0] = UNITY_SAMPLE_TEX2D_SAMPLER(_RadianceBuffer, _HalfResDepth, uvTap_0);
					radianceTaps[1] = UNITY_SAMPLE_TEX2D_SAMPLER(_RadianceBuffer, _HalfResDepth, uvTap_1);
					radianceTaps[2] = UNITY_SAMPLE_TEX2D_SAMPLER(_RadianceBuffer, _HalfResDepth, uvTap_2);
					radianceTaps[3] = UNITY_SAMPLE_TEX2D_SAMPLER(_RadianceBuffer, _HalfResDepth, uvTap_3);

					fixed minDelta = 1.0f;
					uint nearestDepthInd = 0;

					// Sample 1
					fixed depthDelta = abs(depthTaps[0] - linearDepth);
					bool reject = depthDelta < kUpsampleDepthThreshold;

					UNITY_BRANCH
						if (depthDelta < minDelta)
						{
							minDelta = depthDelta;
							nearestDepthInd = 0;
						}

					// Sample 2
					depthDelta = abs(depthTaps[1] - linearDepth);
					reject = reject && depthDelta < kUpsampleDepthThreshold;

					UNITY_BRANCH
						if (depthDelta < minDelta)
						{
							minDelta = depthDelta;
							nearestDepthInd = 1;
						}

					// Sample 3
					depthDelta = abs(depthTaps[2] - linearDepth);
					reject = reject && depthDelta < kUpsampleDepthThreshold;

					UNITY_BRANCH
						if (depthDelta < minDelta)
						{
							minDelta = depthDelta;
							nearestDepthInd = 2;
						}

					// Sample 4
					depthDelta = abs(depthTaps[3] - linearDepth);
					reject = reject && depthDelta < kUpsampleDepthThreshold;

					UNITY_BRANCH
						if (depthDelta < minDelta)
						{
							minDelta = depthDelta;
							nearestDepthInd = 3;
						}

					UNITY_BRANCH
						if (reject)
						{
							// Non-edge pixels use hardware bilinear.
							radiance = UNITY_SAMPLE_TEX2D(_RadianceBuffer, input.uv);
						}
						else
						{
							radiance = radianceTaps[nearestDepthInd];
	#if defined (SHOW_UPSAMPLE_THRESHOLD)
							debugOutput = 1.0f;
	#endif
						}
					radiance = radianceTaps[nearestDepthInd];
	#else
					// Hardware without support for separate samplers always uses bilinear (DX9).
					radiance = UNITY_SAMPLE_TEX2D(_RadianceBuffer, input.uv);
	#endif
					// [END] Nearest-depth upsampling.

					// [START] Temporal reprojection and final radiance accumulation.
					DSHazeData atmospherics;
					InitializeHazeData(atmospherics, input.ray_interp * linearDepth);

	#if defined(DS_HAZE_TEMPORAL)
					fixed3 fragWS = _WorldSpaceCameraPos + atmospherics.ray;
					fixed4 fragPrevClip = mul(_PreviousViewProjMatrix, fixed4(fragWS, 1.0f));
					fragPrevClip.xyz /= fragPrevClip.w;
					fixed2 prevUV = (fragPrevClip.xy + 1.0f) * 0.5f;

	#if defined (UNITY_UV_STARTS_AT_TOP)
					prevUV.y = 1 - prevUV.y;
	#endif
					bool temporalReject = false;
					if (prevUV.x < 0 || prevUV.x > 1 || prevUV.y < 0 || prevUV.y > 1)
					{
						temporalReject = true;
					}
					fixed prevDepth = UNITY_SAMPLE_TEX2D(_PrevDepthBuffer, prevUV);
					fixed4 prevWS = mul(_PreviousInvViewProjMatrix, fixed4(fragPrevClip.xy, prevDepth, 1));
					prevWS.xyz /= prevWS.w;

					if (!temporalReject)
					{
						fixed reuseFactor = exp(-length(fragWS - prevWS) * kTemporalRejectionScale);
	#if !defined (SHOW_UPSAMPLE_THRESHOLD) && !defined (SHOW_TEMPORAL_REJECTION)
						fixed4 temporalColour = UNITY_SAMPLE_TEX2D(_PrevAccumBuffer, prevUV);
						radiance = lerp(radiance, temporalColour, kTemporalBlendFactor * reuseFactor);
	#endif
	#if defined (SHOW_TEMPORAL_REJECTION)
						debugOutput = 1.0f - reuseFactor;
					}
					else
					{
						debugOutput = 1.0f;
	#endif
					}
	#endif
					// [END] Temporal reprojection and final radiance accumulation.

					// [START] Compose for MRT versions.
					fixed4 baseColour = UNITY_SAMPLE_TEX2D(_MainTex, input.uv);
	#if defined (DS_HAZE_APPLY_RAYLEIGH)
					ComputeRayleighScattering(atmospherics);
	#endif
	#if defined (DS_HAZE_APPLY_MIE)
					ComputeMieScattering(atmospherics);
	#endif
	#if defined (DS_HAZE_APPLY_FOG_EXTINCTION) || defined (DS_HAZE_APPLY_FOG_RADIANCE)
					ComputeHeightFog(atmospherics);
	#endif

	#if defined (DS_HAZE_APPLY_RAYLEIGH) && defined (DS_HAZE_APPLY_MIE) && defined (DS_HAZE_APPLY_FOG_EXTINCTION) && defined (DS_HAZE_APPLY_FOG_RADIANCE)
					ComposeAtmosphere(baseColour.rgb, atmospherics);
	#else // some components are not applied to skybox fragments, conditional required.
					UNITY_BRANCH
					if (linearDepth < 0.9999f)
					{
						// This is not a skybox fragment
	#if !defined (DS_HAZE_APPLY_RAYLEIGH)
						ComputeRayleighScattering(atmospherics);
	#endif
	#if !defined (DS_HAZE_APPLY_MIE)
						ComputeMieScattering(atmospherics);
	#endif
	#if !defined (DS_HAZE_APPLY_FOG_EXTINCTION) && !defined (DS_HAZE_APPLY_FOG_RADIANCE)
						ComputeHeightFog(atmospherics);
	#endif
						ComposeAtmosphere(baseColour.rgb, atmospherics);
	#if !defined (DS_HAZE_APPLY_MIE) // Apply volumetric radiance here if skybox fragments are being ignored.
						baseColour.rgb += radiance.rgb;
	#endif
					}
					else
					{
						// This is a skybox fragment.
						ComposeAtmosphereComponents(baseColour.rgb, atmospherics);
					}
	#endif

	#if defined (DS_HAZE_APPLY_MIE)
					baseColour.rgb += radiance.rgb;
	#endif

	#if defined(DS_HAZE_TEMPORAL)
	#if defined (SHOW_UPSAMPLE_THRESHOLD) || defined (SHOW_TEMPORAL_REJECTION)
					baseColour.rgb *= 0.2f;
					baseColour.g += debugOutput;
					output.colour = baseColour;
	#else
					output.colour = baseColour;
	#endif
					output.radiance = radiance;
					return output;
	#else
	#if defined (SHOW_UPSAMPLE_THRESHOLD)
					baseColour.rgb *= 0.2f;
					baseColour.g += debugOutput;
					return  baseColour;
	#else
					return  baseColour;
	#endif
	#endif
					// [END] Compose for MRT versions.
				}
				ENDCG
			}

			Pass // 11 (9)
			{
					// Copy the depth buffer for reprojection next frame.
					name "DS_DepthGrab"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_Image
					#pragma fragment DSFP_DepthGrab

					fixed4 DSFP_DepthGrab(v2f_image input) : SV_Target
					{
						fixed d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv).r;
						return fixed4(d, 0, 0, 0);
					}
					ENDCG
				}

				Pass // 12, Half res. (10)
				{
					name "DS_DepthDownSample_Half"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_Image
					#pragma fragment DSFP_DepthDownSample

					uniform fixed4 _CameraDepthTexture_TexelSize;

					fixed4 DSFP_DepthDownSample(v2f_image input) : SV_Target
					{
						// Down sample depth buffer for later use in upsampling.
						fixed2 halfTexel = _CameraDepthTexture_TexelSize.xy * 0.5f;

						fixed2 uvTap_0 = input.uv.xy + fixed2(-1, -1) * halfTexel;
						fixed2 uvTap_1 = input.uv.xy + fixed2(-1, 1) * halfTexel;
						fixed2 uvTap_2 = input.uv.xy + fixed2(1, -1) * halfTexel;
						fixed2 uvTap_3 = input.uv.xy + fixed2(1, 1) * halfTexel;

						fixed d0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_0);
						fixed d1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_1);
						fixed d2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_2);
						fixed d3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_3);

		#if defined(UNITY_REVERSED_Z)
						fixed depth = min(d0, min(d1, min(d2, d3)));
		#else
						fixed depth = max(d0, max(d1, max(d2, d3)));
		#endif
						return fixed4(depth, 0, 0, 0);
					}
					ENDCG
				}

				Pass // 13, Half res. (11)
				{
					name "DS_DepthDownSample_Quarter"
					Cull Off ZWrite Off ZTest Always

					CGPROGRAM
					#pragma target 3.0
					#pragma vertex DSVP_Image
					#pragma fragment DSFP_DepthDownSample

					uniform fixed4 _CameraDepthTexture_TexelSize;

					fixed4 DSFP_DepthDownSample(v2f_image input) : SV_Target
					{
						// Down sample depth buffer for later use in upsampling.
						fixed2 halfTexel = _CameraDepthTexture_TexelSize.xy;

						fixed2 uvTap_0 = input.uv.xy + fixed2(-1, -1) * halfTexel;
						fixed2 uvTap_1 = input.uv.xy + fixed2(-1, 1) * halfTexel;
						fixed2 uvTap_2 = input.uv.xy + fixed2(1, -1) * halfTexel;
						fixed2 uvTap_3 = input.uv.xy + fixed2(1, 1) * halfTexel;

						fixed d0 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_0);
						fixed d1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_1);
						fixed d2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_2);
						fixed d3 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uvTap_3);

		#if defined(UNITY_REVERSED_Z)
						fixed depth = min(d0, min(d1, min(d2, d3)));
		#else
						fixed depth = max(d0, max(d1, max(d2, d3)));
		#endif
						return fixed4(depth, 0, 0, 0);
					}
						ENDCG
				}
			}
			FallBack off
	}
