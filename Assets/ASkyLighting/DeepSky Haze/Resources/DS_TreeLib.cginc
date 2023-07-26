// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef __DS_TREE_LIB
#define __DS_TREE_LIB

#include "UnityCG.cginc"
#include "UnityBuiltin3xTreeLibrary.cginc"
#include "DS_Lib.cginc"

// Compute fog in the vertex shader.
inline fixed4 DS_compute_fog_tree_vtx(fixed4 vtx)
{
	fixed3 wpos = mul(unity_ObjectToWorld, vtx).xyz;

	DSHazeData data;
	InitializeHazeData(data, wpos - _WorldSpaceCameraPos);

	ComputeRayleighScattering(data);
	ComputeMieScatteringUnityTree(data);
	ComputeHeightFog(data);

	// Pre-calculate the fog interpolation here so we can combine the additive scattering with
	// the LERPed fog colour (otherwise we would need to pass both in the output stuct).
	return fixed4((_DS_FogAmbientLight + _DS_FogDirectLight * data.fogR) * data.fogE + (data.rayleigh + _DS_LightColour * data.mie) * (1.0f - data.fogE), 1.0f - data.fogE);
}

// UNITY TREE BILLBOARDS PRE-v1.1
inline float MiePhaseHLegacy(float3 cameraRay)
{
	float cosTheta = dot(_DS_LightDirection, cameraRay);
	float mie = (1 - kMScatteringDirection * kMScatteringDirection) / ((1 - kMScatteringDirection * cosTheta) * (1 - kMScatteringDirection * cosTheta));
	return mie * kSphericalNormFactor;
}

inline float MiePhaseFLegacy(float3 cameraRay)
{
	float cosTheta = dot(_DS_LightDirection, cameraRay);
	float mie = (1 - kFScatteringDirection * kFScatteringDirection) / ((1 - kFScatteringDirection * cosTheta) * (1 - kFScatteringDirection * cosTheta));
	return mie * kSphericalNormFactor;
}

inline float GetFogHeightDensityIntegralLegacy(float3 ray, float distance, float startDensity)
{
	float densityIntegral = distance * startDensity;

	const float threshold = 0.001f;
	if (abs(ray.y) > threshold)
	{
		float t = kFDensityHeightFalloff * ray.y;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}
	return densityIntegral;
}

inline float3 ComputeRayleighScatteringLegacy(float3 ray, float distance)
{
	float densityIntegral = distance * _DS_InitialDensityParams.x;

	const float threshold = 0.001f;
	if (abs(ray.y) > threshold)
	{
		float t = kRDensityHeightFalloff * ray.y;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}
	return _DS_LightColour * kRBetaS * densityIntegral * kSphericalNormFactor;
}

inline float ComputeMieScatteringLegacy(float3 ray, float3 rayN, float distance)
{
	float densityIntegral = distance * _DS_InitialDensityParams.y;

	const float threshold = 0.001f;
	if (abs(ray.y) > threshold)
	{
		float t = kMDensityHeightFalloff * ray.y;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}

	float phase = MiePhaseHLegacy(rayN);
	return phase * kMBetaS * densityIntegral * (1 - kMDirectIntensity * 0.5f);
}

inline float ComputeHeightFogTreeLegacy(float3 ray, float3 rayN, float worldZ, inout float fogE)
{
	// Exponential height fog.
	float phase = MiePhaseFLegacy(rayN);
	float rhoF = GetFogHeightDensityIntegralLegacy(ray, max(worldZ - kFStartDistance * _ProjectionParams.z, 0), _DS_InitialDensityParams.z);
	float extF = exp(-kFBetaE * rhoF);
	fogE = 1.0f - saturate(extF);
	return phase * kFBetaS;
}

inline void DS_compute_fog_tree_vtx_legacy(fixed4 vtx, out fixed3 rayleigh, out fixed3 mieFog)
{
	fixed3 wpos = mul(unity_ObjectToWorld, vtx).xyz;

	float3 ray = wpos - _WorldSpaceCameraPos;
	float dT = length(ray);
	float3 rayNormalized = normalize(ray);
	float cosTheta = dot(_DS_LightDirection, rayNormalized);

	rayleigh = float3(0, 0, 0);
	mieFog = float3(0, 0, 0);

	rayleigh = ComputeRayleighScatteringLegacy(ray, dT);
	mieFog.r = ComputeMieScatteringLegacy(ray, rayNormalized, dT);
	mieFog.g = ComputeHeightFogTreeLegacy(ray, rayNormalized, dT, mieFog.b);
}


#endif //__DS_TREE_LIB