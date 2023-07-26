// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef __DS_TRANSPARENT_LIB
#define __DS_TRANSPARENT_LIB

#include "UnityCG.cginc"
#include "Lighting.cginc" // required for SurfaceOutput only.
#include "UnityPBSLighting.cginc"
#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_Lib.cginc" // use the full path as may be included in user shaders.

// [START] Define the necessary fields in the Input struct.
// Exactly what we need depends on whether rendering per-vertex/per-fragment and at full-depth.
#if defined (DS_HAZE_USE_FULL_DEPTH)
#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
#define DEEPSKY_HAZE_DECLARE_INPUT float3 worldPos; float4 screenPos
#else
#define DEEPSKY_HAZE_DECLARE_INPUT float3 rayleigh; float3 mieFog; float4 screenPos
#endif
#else
#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
#define DEEPSKY_HAZE_DECLARE_INPUT float3 worldPos
#else
#define DEEPSKY_HAZE_DECLARE_INPUT float3 rayleigh; float3 mieFog
#endif
#endif
// [END] Define the necessary fields in the Input struct.

// Declare the radiance buffer with the volumetrics if we're rendering at full-depth.
#if defined (DS_HAZE_USE_FULL_DEPTH) && defined (DS_HAZE_APPLY_MIE)
UNITY_DECLARE_TEX2D(_DS_RadianceBuffer);
#endif

// Unlit (basic pass-through) lighting model for skybox shaders.
half4 LightingSkyboxUnlit(SurfaceOutput s, half3 lightDir, half atten)
{
	return half4(s.Albedo, s.Alpha);
}

// [START] Compute all the atmospheric effects.
// Which components get computed depends on what is defined. This function
// can be called from either the vertex and final colour modifier, depending
// on whether we're computing effects per-vertex or per-fragment.
 inline void DS_compute_atmospherics(inout DSHazeData data)
 {
 #if defined (DS_HAZE_USE_FULL_DEPTH)
	 data.dT = _ProjectionParams.z; //<--- skybox shaders use the camera's full z-depth
	 data.ray = data.rayNormalized * _ProjectionParams.z;
 #if defined (DS_HAZE_APPLY_RAYLEIGH)
	 ComputeRayleighScattering(data);
 #endif
 #if defined (DS_HAZE_APPLY_MIE)
	 ComputeMieScattering(data);
	 data.mie *= 1.0f - kMDirectIntensity;
 #endif
 #if defined (DS_HAZE_APPLY_FOG_EXTINCTION) || defined (DS_HAZE_APPLY_FOG_RADIANCE)
	 ComputeHeightFog(data);
 #endif
 #else
	 ComputeRayleighScattering(data);
	 ComputeMieScattering(data);
	 data.mie *= 1.0f - kMDirectIntensity;
	 ComputeHeightFog(data);
 #endif
 }
 // [END] Compute all the atmospheric effects.

// [START] Vertex modifier macro and function.
#define DEEPSKY_HAZE_VERTEX_MOD(vtx, intype) DS_Vertex_Modifier(vtx, (intype).rayleigh, (intype).mieFog)

inline void DS_Vertex_Modifier(inout appdata_full vtx, inout float3 outRayleigh, inout float3 outMieFog)
{
	float3 wpos = mul(unity_ObjectToWorld, vtx.vertex).xyz;
	float3 ray = wpos - _WorldSpaceCameraPos;

	DSHazeData data;
	InitializeHazeData(data, ray);

	DS_compute_atmospherics(data);

	outRayleigh = data.rayleigh;
	outMieFog = float3(data.mie, data.fogR, data.fogE);
}

// Vertex function for use in vertex/fragment shaders.
inline void DS_Haze_Per_Vertex(float4 vertex, inout float3 outRayleigh, inout float3 outMieFog)
{
	float3 wpos = mul(unity_ObjectToWorld, vertex).xyz;
	float3 ray = wpos - _WorldSpaceCameraPos;

	DSHazeData data;
	InitializeHazeData(data, ray);

	DS_compute_atmospherics(data);

	outRayleigh = data.rayleigh;
	outMieFog = float3(data.mie, data.fogR, data.fogE);
}
// [END] Vertex modifier macro and function.

// [START] Final colour modifier and macro.
// These are highly dependent on whether rendering at full-depth and whether the atmospherics are
// computed per-vertex or per-fragment. When per-vertex, the modifier just performs the final 
// compose of the different elements. If calculating per-fragment then the atmospherics
// are actually calculated here as well.
#if defined (DS_HAZE_USE_FULL_DEPTH)
#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
#define DEEPSKY_HAZE_FINAL_COLOR(intype, outtype, colour) \
	DS_Final_Color_Modifier((intype).worldPos, (intype).screenPos, colour, (outtype).Alpha)
#else
#define DEEPSKY_HAZE_FINAL_COLOR(intype, outtype, colour) \
	DS_Final_Color_Modifier((intype).screenPos, (intype).rayleigh, (intype).mieFog, colour, (outtype).Alpha)
#endif
#else
#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
#define DEEPSKY_HAZE_FINAL_COLOR(intype, outtype, colour) \
	DS_Final_Color_Modifier((intype).worldPos, colour, (outtype).Alpha)
#else
#define DEEPSKY_HAZE_FINAL_COLOR(intype, outtype, colour) \
	DS_Final_Color_Modifier((intype).rayleigh, (intype).mieFog, colour, (outtype).Alpha)
#endif
#endif

// Helper macro for use in frag shaders. Note: the macro is intended for per-vertex, non-skybox shaders.
#define DS_Haze_Apply(rayleigh, mieFog, colour, alpha) \
	DS_Final_Color_Modifier(rayleigh, mieFog, colour, alpha)

#if defined (DS_HAZE_USE_FULL_DEPTH)
#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
inline void DS_Final_Color_Modifier(float3 wPos, float4 screenPos, inout fixed4 colour, fixed alpha)
#else
inline void DS_Final_Color_Modifier(float4 screenPos, fixed3 rayleigh, fixed3 mieFog, inout fixed4 colour, fixed alpha)
#endif
#else
#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
inline void DS_Final_Color_Modifier(float3 wPos, inout fixed4 colour, fixed alpha)
#else
inline void DS_Final_Color_Modifier(fixed3 rayleigh, fixed3 mieFog, inout fixed4 colour, fixed alpha)
#endif
#endif
{
	DSHazeData data;

#if defined (DS_HAZE_APPLY_PER_FRAGMENT)
	float3 ray = wPos - _WorldSpaceCameraPos;

	InitializeHazeData(data, ray);
	DS_compute_atmospherics(data);

#else // Do the final compose for per-vertex atmospherics.
	data.rayleigh = rayleigh;
	data.mie = mieFog.r;
	data.fogR = mieFog.g;
	data.fogE = mieFog.b;
#endif
	ComposeAtmosphereComponents(colour.rgb, data);

	// Add the volumetric rays for skybox shaders.
#if defined (DS_HAZE_USE_FULL_DEPTH) && defined (DS_HAZE_APPLY_MIE)
	float2 screenUV = screenPos.xy / screenPos.w;
	float4 radiance = UNITY_SAMPLE_TEX2D(_DS_RadianceBuffer, screenUV);
	colour.rgb += _DS_LightColour * radiance.rgb;
#endif
}
// [END] Final colour modifier and macro.

#endif //__DS_TRANSPARENT_LIB