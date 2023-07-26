#ifndef __DS_LIB
#define __DS_LIB

#if defined (DS_HAZE_FULL)
#define DS_HAZE_APPLY_RAYLEIGH
#define DS_HAZE_APPLY_MIE
#define DS_HAZE_APPLY_FOG_EXTINCTION
#define DS_HAZE_APPLY_FOG_RADIANCE
#endif

#if defined (DS_HAZE_LIGHTING_ONLY)
#if !defined(DS_HAZE_APPLY_MIE)
#define DS_HAZE_APPLY_MIE
#endif
#if !defined(DS_HAZE_APPLY_FOG_RADIANCE)
#define DS_HAZE_APPLY_FOG_RADIANCE
#endif
#endif

#define kSphericalNormFactor 0.07957747155f //<--- 1/4PI
#define kRDensityHeightFalloff _DS_AirHazeParams.x
#define kRBetaS _DS_RBetaS.xyz

// Mie constants
#define kMDensityHeightFalloff _DS_AirHazeParams.y
#define kMScatteringDirection _DS_AirHazeParams.w	
#define kMBetaS _DS_BetaParams.x //0.0022f //<--- 0.000022 * 100
#define kMDirectIntensity _DS_BetaParams.y

// Fog constants
#define kFogOpacity _DS_FogParams.z
#define kFogScale _DS_FogParams.x
#define kFogMin _DS_InitialDensityParams.w
#define kFDensityHeightFalloff _DS_FogParams.y
#define kFConstantHeight _DS_FogParams.z
#define kFStartDistance _DS_FogParams.x
#define kFScatteringDirection _DS_FogParams.w
#define kFBetaS _DS_BetaParams.z
#define kFBetaE _DS_BetaParams.w

// Haze scattering blend factor for Unity Tree billboards (not SpeedTree!).
// Unity renders tree billboards created with the built-in Tree Creator in a slightly odd way,
// as part of the transparent render queue. This means there is no way to properly integrate the
// volumetric Mie radiance correctly, as there is no depth information available for the trees.
// Instead, we just have to blend in a certain amount of analytic Mie scattering. 
// Adjust this value to control the maximum amount of scattering on tree billboards.
#define kUnityTreeHazeBlend 0.5f

// Scattering cross-section params
uniform fixed4 _DS_RBetaS;
uniform fixed4 _DS_BetaParams; // (kMBetaS, Haze secondary scattering ratio, kFBetaS, kFBetaE)

// Air (Rayleigh) and haze (Mie) parameters.
uniform fixed4 _DS_AirHazeParams; // (Air Density Height Falloff, Haze Density Height Falloff, Haze Start Height, Haze Scattering Direction)

// Fog params (density, falloff, start height, scattering direction)
uniform fixed4 _DS_FogParams;

// Initial densities at view point
uniform fixed4 _DS_InitialDensityParams; // (Air Viewpoint Density, Haze Viewpoint Density, Fog Viewpoint Density, [reserved])

// Lighting
uniform fixed3 _DS_LightDirection;
uniform fixed3 _DS_LightColour;
uniform fixed3 _DS_HorizonColour;
uniform fixed3 _DS_FogAmbientLight;
uniform fixed4 _DS_FogDirectLight;

// Struct to hold common data and outputs used by all the scattering functions.
struct DSHazeData {
	fixed cosTheta;
	fixed dT;
	fixed3 ray;
	fixed3 rayNormalized;
	fixed3 rayleigh;
	fixed mie;
	fixed fogE;
	fixed fogR;
};

// Initialize common data from the given view-to-world point ray.
inline void InitializeHazeData(inout DSHazeData data, fixed3 ray) {
	data.rayleigh = fixed3(0, 0, 0);
	data.mie = fixed3(0, 0, 0);
	data.fogR = 0;
	data.fogE = 0;

	data.ray = ray;
	data.dT = length(ray);
	data.rayNormalized = normalize(ray);
	data.cosTheta = dot(_DS_LightDirection, data.rayNormalized);
}

// Mie phase function for volumetric ray-march.
inline fixed MiePhaseH(fixed3 cameraRay)
{
	fixed cosTheta = dot(_DS_LightDirection, cameraRay);
	fixed mie = (1 - kMScatteringDirection * kMScatteringDirection) / ((1 - kMScatteringDirection * cosTheta) * (1 - kMScatteringDirection * cosTheta));
	return mie * kSphericalNormFactor;
}

inline fixed MiePhaseH(DSHazeData data)
{
	fixed mie = (1 - kMScatteringDirection * kMScatteringDirection) / ((1 - kMScatteringDirection * data.cosTheta) * (1 - kMScatteringDirection * data.cosTheta));
	return mie * kSphericalNormFactor;
}

inline fixed MiePhaseF(DSHazeData data)
{
	fixed mie = (1 - kFScatteringDirection * kFScatteringDirection) / ((1 - kFScatteringDirection * data.cosTheta) * (1 - kFScatteringDirection * data.cosTheta));
	return mie * kSphericalNormFactor;
}

inline fixed GetHazeHeightDensity(fixed height)
{
#if defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	return 1.0f;
#else
	fixed rho = exp(-kMDensityHeightFalloff * height);
	return min(rho, 1.0f);
#endif
}

inline fixed GetFogHeightDensity(fixed height)
{
#if defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	return 1.0f;
#else
	return min(exp(-kFDensityHeightFalloff * height), 1.0f);
#endif
}

inline fixed GetFogHeightDensityIntegral(DSHazeData data)
{
#if defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	return max(data.dT - kFStartDistance * _ProjectionParams.z, 0) * _DS_InitialDensityParams.z;
#else
	fixed param = min((data.dT - kFStartDistance * _ProjectionParams.z) / (kFStartDistance * _ProjectionParams.z),0);
	fixed densityIntegral = lerp(0, data.dT, 1 - abs(param)) * _DS_InitialDensityParams.z;

	const fixed threshold = 0.001f;
	if (abs(data.ray.y) > threshold)
	{
		fixed t = kFDensityHeightFalloff * data.ray.y;
		t = abs(t) < 0.00001 ? 0.00001 : t;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}
	return densityIntegral;
#endif
}

// Fog extinction for volume lights.
inline fixed GetFogExtinctionToPoint(float3 ray, float dT)
{
#if defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
    return max(dT - kFStartDistance * _ProjectionParams.z, 0) * _DS_InitialDensityParams.z;
#else
    fixed densityIntegral = max(dT - kFStartDistance * _ProjectionParams.z, 0) * _DS_InitialDensityParams.z;

    const fixed threshold = 0.001f;
    if (abs(ray.y) > threshold)
    {
        fixed t = kFDensityHeightFalloff * ray.y;
        t = abs(t) < 0.00001 ? 0.00001 : t;
        densityIntegral *= (1.0f - exp(-t)) / t;
    }
    return kFogOpacity * saturate(exp(-kFBetaE * densityIntegral));
#endif
}

inline fixed GetAirHeightDensity(fixed height)
{
#if defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	return 1.0f;
#else
	fixed rho = exp(-kRDensityHeightFalloff * height);
	return min(rho, 1.0f);
#endif
}

inline void ComputeRayleighScattering(inout DSHazeData data)
{
	fixed densityIntegral = data.dT * _DS_InitialDensityParams.x;

#if !defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	const fixed threshold = 0.001f;
	if (abs(data.ray.y) > threshold)
	{
		fixed t = kRDensityHeightFalloff * data.ray.y;
		t = abs(t) < 0.00001 ? 0.00001 : t;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}
#endif
	data.rayleigh += _DS_HorizonColour * kRBetaS *densityIntegral * kSphericalNormFactor;
}

inline void ComputeMieScatteringUnityTree(inout DSHazeData data)
{
	fixed densityIntegral = data.dT * _DS_InitialDensityParams.y;

#if !defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	const fixed threshold = 0.001f;
	if (abs(data.ray.y) > threshold)
	{
		fixed t = kMDensityHeightFalloff * data.ray.y;
		t = abs(t) < 0.00001 ? 0.00001 : t;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}
#endif

	fixed phase = MiePhaseH(data);
	data.mie =  phase * kMBetaS * densityIntegral * lerp(1, kUnityTreeHazeBlend, kMDirectIntensity);
}

inline void ComputeMieScattering(inout DSHazeData data)
{
	fixed densityIntegral = data.dT * _DS_InitialDensityParams.y;

#if !defined(DS_HAZE_HEIGHT_FALLOFF_NONE)
	const fixed threshold = 0.001f;
	if (abs(data.ray.y) > threshold)
	{
		fixed t = kMDensityHeightFalloff * data.ray.y;
		t = abs(t) < 0.00001 ? 0.00001 : t;
		densityIntegral *= (1.0f - exp(-t)) / t;
	}
#endif

	fixed phase = MiePhaseH(data);
	data.mie =  phase * kMBetaS * densityIntegral * (1.0f - kMDirectIntensity);
}

inline void ComputeHeightFog(inout DSHazeData data)
{
	// Exponential height fog.
	fixed phase = MiePhaseF(data);
	fixed rhoF = GetFogHeightDensityIntegral(data);
	data.fogE = kFogOpacity * (1.0f - saturate(exp(-kFBetaE * rhoF)));
	data.fogR = phase * kFBetaS * kFogOpacity;
}

inline void ComposeAtmosphere(inout fixed3 sceneColour, DSHazeData data)
{
	sceneColour = lerp(sceneColour + data.rayleigh + _DS_LightColour * data.mie, _DS_FogAmbientLight + _DS_FogDirectLight * data.fogR, data.fogE);
}

inline void ComposeAtmosphereComponents(inout fixed3 sceneColour, DSHazeData data)
{
#if defined (DS_HAZE_APPLY_FOG_EXTINCTION) && defined (DS_HAZE_APPLY_FOG_RADIANCE)
	sceneColour = lerp(sceneColour + data.rayleigh + _DS_LightColour * data.mie, _DS_FogAmbientLight + _DS_FogDirectLight * data.fogR, data.fogE);
#endif
#if defined (DS_HAZE_APPLY_FOG_EXTINCTION) && !defined (DS_HAZE_APPLY_FOG_RADIANCE)
	sceneColour = lerp(sceneColour + data.rayleigh + _DS_LightColour * data.mie, _DS_FogAmbientLight, data.fogE);
#endif
#if !defined (DS_HAZE_APPLY_FOG_EXTINCTION) && defined (DS_HAZE_APPLY_FOG_RADIANCE)
	sceneColour += data.rayleigh + _DS_LightColour * data.mie +_DS_FogDirectLight * data.fogR;
#endif
#if !defined (DS_HAZE_APPLY_FOG_EXTINCTION) && !defined (DS_HAZE_APPLY_FOG_RADIANCE)
	sceneColour += data.rayleigh + _DS_LightColour * data.mie;
#endif
}

#endif // __DS_LIB