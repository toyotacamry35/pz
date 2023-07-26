// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'
// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

#include "UnityCG.cginc"
#include "UnityDeferredLibrary.cginc"
#include "DS_Lib.cginc"

// Prevent issues with auto-upgrade of cginc files in some older versions of Unity.
#if UNITY_VERSION >= 540
#define UNITY_WORLDTOLIGHT unity_WorldToLight
#else
#define UNITY_WORLDTOLIGHT unity_WorldToLight
#endif

#if defined(SAMPLES_4)
#define kVolumeSamples 4
#elif defined(SAMPLES_8)
#define kVolumeSamples 8
#elif defined(SAMPLES_16)
#define kVolumeSamples 16
#elif defined(SAMPLES_32)
#define kVolumeSamples 32
#endif

#if !defined(kDownsampleFactor)
#define kDownsampleFactor 2
#endif

#define kScatteringAmount _DS_HazeLightVolumeScattering.x
#define kSecondaryScatteringAmount _DS_HazeLightVolumeScattering.y
#define kMLightScatteringDirection _DS_HazeLightVolumeScattering.z
#define kSecondaryScatteringScaler _DS_HazeLightVolumeScattering.w
#define kSphericalNormFactor 0.07957747155f //<--- 1/4PI
#define kSampleOffsetCoords _DS_HazeSamplingParams.xy

#define kSpotLightApex _DS_HazeLightVolumeParams0.xyz
#define kLightRange _DS_HazeLightVolumeParams0.w
#define kSpotLightAxis _DS_HazeLightVolumeParams1.xyz
#define kSpotLightCosTheta _DS_HazeLightVolumeParams2.x
#define kSpotLightOneOverCosTheta _DS_HazeLightVolumeParams2.y
#define kSpotLightPlaneD _DS_HazeLightVolumeParams2.z

#define kDensityTextureOffset _DS_HazeDensityParams.xyz
#define kDensityTextureScale _DS_HazeDensityParams.w
#define kDensityTextureConstrast _DS_HazeSamplingParams.z

uniform fixed4 _DS_HazeLightVolumeColour;
uniform fixed4 _DS_HazeLightVolumeScattering;
uniform fixed4 _DS_HazeLightVolumeParams0; // <--- Light position (x, y, z), radius (w)
uniform fixed4 _DS_HazeCameraDirection;

sampler3D _DensityTexture;
sampler3D _SamplingOffsets;
uniform fixed4 _SamplingOffsets_TexelSize;
uniform fixed4 _DS_HazeSamplingParams;

uniform float4x4 _WorldViewProj;
uniform float4x4 _WorldView;

uniform float4 _DS_HazeLightVolumeParams1; // <--- Axis (x, y, z), Unused (w) 
uniform float4 _DS_HazeLightVolumeParams2; // <--- CosTheta (x), 1.0 / CosTheta (y), plane D (z), Unused (w)

#if defined (DENSITY_TEXTURE)
uniform float4 _DS_HazeDensityParams;
#endif

#if defined (SPOT_COOKIE) || defined (POINT_COOKIE)
uniform float4x4 _DS_Haze_WorldToCookie;
#endif
#if defined (SHADOWS_DEPTH)
uniform float4x4 _DS_Haze_WorldToShadow;
#endif

struct appdata
{
    float4 vertex : POSITION;
};

struct v2f
{
    float4 pos : SV_POSITION;
    float3 ray : TEXCOORD0;
    float4 uv : TEXCOORD1;
};

// Shared vertex function.
v2f vert(appdata v)
{
    v2f o;
    o.pos = mul(_WorldViewProj, v.vertex);
    o.ray = mul(_WorldView, v.vertex).xyz * float3(-1, -1, 1);
    o.uv = ComputeScreenPos(o.pos);
    return o;
}

// --------------------------------------------------------------------
//	Scattering and Attenuation.
// --------------------------------------------------------------------
inline fixed ComputeScattering(float3 wpos, float3 vecToLight, float3 cameraRay)
{
    float density = 1;
#if defined (DENSITY_TEXTURE)
    density = tex3D(_DensityTexture, wpos * kDensityTextureScale + kDensityTextureOffset).r;
	density = saturate(kDensityTextureConstrast * (density - 0.5f) + 0.5f);
#endif

    fixed cosTheta = dot(vecToLight, cameraRay);
    fixed mie = (1 - kMLightScatteringDirection * kMLightScatteringDirection) / ((1 - kMLightScatteringDirection * cosTheta) * (1 - kMLightScatteringDirection * cosTheta));
    return density * mie * kSphericalNormFactor * kScatteringAmount;
}

// Derived from UnityDeferredCalculateLightParams in UnityDeferredLibrary.cginc
inline half ComputeAttenuation(float3 lightVec, float3 wpos, float fadeDist, float2 uv)
{
	float attenS = 1.0f;
	float attenC = 1.0f;

#if defined (SPOT_COOKIE)
    float4 uvCookie = mul(_DS_Haze_WorldToCookie, float4(wpos, 1));
	attenC *= tex2Dbias(_LightTexture0, float4(uvCookie.xy/uvCookie.w, 0, -8)).w;
	attenC *= uvCookie.w < 0;
#endif // SPOT_COOKIE
#if defined (SHADOWS_DEPTH)
    float fade = fadeDist * _LightShadowData.z + _LightShadowData.w;
    fade = saturate(fade);

    float4 shadowCoord = mul(_DS_Haze_WorldToShadow, float4(wpos, 1));
	attenS *= saturate(UnitySampleShadowmap(shadowCoord) + fade);
#endif // SPOT_SHADOWS

#if defined (SHADOWS_CUBE)
	attenS *= UnitySampleShadowmap(lightVec);
#endif // POINT_SHADOWS
#if defined (POINT_COOKIE)
#if defined (SHADOWS_CUBE)
	attenC *= texCUBE(_LightTexture0, mul(UNITY_WORLDTOLIGHT, half4(wpos, 1)).xyz).w;
#else
	attenC *= texCUBE(_LightTexture0, mul(_DS_Haze_WorldToCookie, half4(wpos, 1)).xyz).w;
#endif
#endif // POINT_COOKIE

	attenS = kSecondaryScatteringAmount + (attenS * kSecondaryScatteringScaler);
	attenS *= attenC;

#if defined (DS_FALLOFF_UNITY)
	float att = dot(lightVec, lightVec) / (kLightRange * kLightRange); // *_LightPos.w; <-- not sure under what situations _LightPos.w isn't 1.
	attenS *= tex2D(_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;
#else
    float l = length(lightVec);
    float att = 1.0f / (l * l);
    attenS *= att;
#endif

    return attenS;
}

// --------------------------------------------------------------------
//	Intersection functions.
// --------------------------------------------------------------------
// For future reference: The version of IntersectRaySphere that returned a bool doesn't work in GLSL (no errors, just outputs black).
// Instead of testing the result and discarding accordingly, moved the discards inside the function. This now works across OpenGL and DX.
inline void IntersectRaySphere(float4 sphere, float3 rayDir, out float t0, out float t1)
{
    bool r = true;
    float3 L = sphere.xyz - _WorldSpaceCameraPos;
    float tca = dot(L, rayDir);
    float d2 = dot(L, L) - tca * tca;
    if (d2 > sphere.w * sphere.w)
    {
        t0 = 0;
        t1 = 0;
	discard;
        //return false;
    }

    float thc = sqrt(sphere.w * sphere.w - d2);
    t0 = tca - thc;
    t1 = tca + thc;

    if (t0 < 0 && t1 < 0)
    {
	discard;
        //return false;
    }

    t0 = max(t0, 0);
    t1 = max(t1, 0);

    t0 = min(t0, t1);
    t1 = max(t0, t1);

    //return true;
}

inline bool IntersectRayPlane(float3 rayDir, out float t)
{
    float NdotR = dot(kSpotLightAxis, rayDir);
    float NdotRO = dot(kSpotLightAxis, _WorldSpaceCameraPos);

    t = -(NdotRO + kSpotLightPlaneD) / NdotR;
    if (t < 0)
    {
        t = 100000.0f;
        return false;
    }
    return true;
}

inline bool IntersectRayCone(float3 rayDir, out float2 t)
{
    float3 rayOrigin = _WorldSpaceCameraPos - kSpotLightApex;
    float a = dot(rayDir, kSpotLightAxis);
    float b = dot(rayDir, rayDir);
    float c = dot(rayOrigin, kSpotLightAxis);
    float d = dot(rayOrigin, rayDir);
    float e = dot(rayOrigin, rayOrigin);

    float cosSqr = kSpotLightCosTheta * kSpotLightCosTheta;

    float A = a * a - b * cosSqr;
    float B = 2 * (c * a - d * cosSqr);
    float C = c * c - e * cosSqr;
    float D = B * B - 4 * A * C;

    t = float2(0, 0);
    if (D > 0)
    {
        D = sqrt(D);
        t = (-B + sign(A) * float2(-D, D)) / (2 * A);
        bool2 useB2 = c + a * t > 0 && t > 0;
        t = t * useB2 + !useB2 * float2(100000.0f, 100000.0f);
        return true;
    }
    return false;
}

// --------------------------------------------------------------------
// Fragment programs.
// --------------------------------------------------------------------

// Point lights.
fixed4 DSFP_PointVolume(v2f i) : SV_Target
{
	float2 uv = i.uv.xy / i.uv.w;

	float3 uv_offset = float3(uv * (_ScreenParams.xy / 2 / _SamplingOffsets_TexelSize.z), kSampleOffsetCoords.x);
	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	depth = Linear01Depth(depth);
	float sampleOffset = tex3D(_SamplingOffsets, uv_offset).r;

	i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	float4 vpos = float4(i.ray * depth, 1);
	float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
	float3 eyeray = wpos - _WorldSpaceCameraPos;
	float3 ray = normalize(eyeray);
	float ws_depth = depth / dot(_DS_HazeCameraDirection.xyz, ray);

	float t0 = 0;
	float t1 = 0;

	IntersectRaySphere(_DS_HazeLightVolumeParams0, ray, t0, t1);

	if (t0 * _ProjectionParams.w > ws_depth)
	{
		discard;
	}

	if (t1 * _ProjectionParams.w > ws_depth)
	{
		t1 = ws_depth * _ProjectionParams.z;
	}

    float3 rayToNear = ray * t0;
    float3 rayNear = _WorldSpaceCameraPos + rayToNear;
	float3 traceRay = ray * (t1 - t0);

	float3 rayDt = traceRay / kVolumeSamples;
	float dT = length(rayDt);
	float3 sampleWS = rayNear + rayDt * sampleOffset;

	float radiance = 0;

	UNITY_UNROLL
		for (int si = 0; si < kVolumeSamples; si++)
		{
			float3 ld = sampleWS - _DS_HazeLightVolumeParams0.xyz;
			float atten = ComputeAttenuation(ld, sampleWS, 0, uv);
			float scatter = ComputeScattering(sampleWS, normalize(-ld), ray);
			radiance += atten * dT * scatter;

			sampleWS += rayDt;
		}

#if defined(DS_LIGHT_USE_FOG)
    radiance *= GetFogExtinctionToPoint(rayToNear, length(rayToNear));
#endif
	return float4(_DS_HazeLightVolumeColour.rgb * radiance, 1);
}

// Spotlights, inside the volume.
fixed4 DSFP_SpotVolumeInterior(v2f i) : SV_Target
{
	float2 uv = i.uv.xy / i.uv.w;
	float3 uv_offset = float3(uv * (_ScreenParams.xy / 2 / _SamplingOffsets_TexelSize.z), kSampleOffsetCoords.x);
	float sampleOffset = tex3D(_SamplingOffsets, uv_offset).r;

	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	depth = Linear01Depth(depth);

	i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	float4 vpos = float4(i.ray * depth, 1);
	float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
	float3 eyeray = wpos - _WorldSpaceCameraPos;
	float3 ray = normalize(eyeray);
	float ws_depth = depth / dot(_DS_HazeCameraDirection.xyz, ray);

	float3 light_to_cam = _WorldSpaceCameraPos - kSpotLightApex;
	bool plane_ptv = false;
	if (dot(light_to_cam, kSpotLightAxis) > kLightRange)
	{
		plane_ptv = true;
	}

	float2 tC = float2(0, 0);
	float tP = 0.0f;
	float dt0 = 0.0f;
	float dt1 = 0.0f;
	bool hitC = IntersectRayCone(ray, tC);
	bool hitP = IntersectRayPlane(ray, tP);

	if (!hitC)
	{
		discard;
	}

	if (plane_ptv)
	{
		dt0 = tP;
		dt1 = min(tC.x, tC.y);
	}
	else
	{
		dt1 = min(tP, min(tC.x, tC.y));
	}

	if (dt0 * _ProjectionParams.w > ws_depth)
	{
		discard;
	}

	dt1 = min(dt1, ws_depth * _ProjectionParams.z);

    float3 rayToNear = ray * dt0;
    float3 rayNear = _WorldSpaceCameraPos + rayToNear;
	float3 traceRay = ray * (dt1 - dt0);

	float3 rayDt = traceRay / kVolumeSamples;
	float dT = length(rayDt);
	float3 sampleWS = rayNear + rayDt * sampleOffset;

	float radiance = 0;

	UNITY_UNROLL
		for (int si = 0; si < kVolumeSamples; si++)
		{
			float3 ld = kSpotLightApex - sampleWS;
			float atten = ComputeAttenuation(ld, sampleWS, 0, uv);
			float scatter = ComputeScattering(sampleWS, normalize(ld), ray);
			radiance += atten * dT * scatter;

			sampleWS += rayDt;
		}

#if defined(DS_LIGHT_USE_FOG)
    radiance *= GetFogExtinctionToPoint(rayToNear, length(rayToNear));
#endif
	return fixed4(_DS_HazeLightVolumeColour.rgb * radiance, 1);
}

// Spot lights, outside the volume.
fixed4 DSFP_SpotVolumeExterior(v2f i) : SV_Target
{
	float2 uv = i.uv.xy / i.uv.w;
	float3 uv_offset = float3(uv * (_ScreenParams.xy / 2 / _SamplingOffsets_TexelSize.z), kSampleOffsetCoords.x);
	float sampleOffset = tex3D(_SamplingOffsets, uv_offset).r;

	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	depth = Linear01Depth(depth);
	i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
	float4 vpos = float4(i.ray * depth, 1);
	float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
	float3 eyeray = wpos - _WorldSpaceCameraPos;
	float3 ray = normalize(eyeray);
	float ws_depth = depth / dot(_DS_HazeCameraDirection.xyz, ray);

	float3 light_to_cam = _WorldSpaceCameraPos - kSpotLightApex;
	bool plane_ptv = false;
	if (dot(light_to_cam, kSpotLightAxis) > kLightRange)
	{
		plane_ptv = true;
	}

	float2 tC = float2(0, 0);
	float tP = 0.0f;
	float dt0 = 0.0f;
	float dt1 = 0.0f;
	bool hitC = IntersectRayCone(ray, tC);
	bool hitP = IntersectRayPlane(ray, tP);

	if (!hitC)
	{
		discard;
	}

	if (plane_ptv)
	{
		dt0 = min(max(tP, tC.x), max(tP, tC.y));
		dt1 = max(tC.x, tC.y);
	}
	else
	{
		dt0 = min(tP, min(tC.x, tC.y));
		dt1 = max(min(tP, tC.x), min(tP, tC.y));
	}

	if (dt0 * _ProjectionParams.w > ws_depth)
	{
		discard;
	}

	dt1 = min(dt1, ws_depth * _ProjectionParams.z);

    float3 rayToNear = ray * dt0;
    float3 rayNear = _WorldSpaceCameraPos + rayToNear;
	float3 traceRay = ray * (dt1 - dt0);

	float3 rayDt = traceRay / kVolumeSamples;
	float dT = length(rayDt);
	float3 sampleWS = rayNear + rayDt * sampleOffset;

	float radiance = 0;

	UNITY_UNROLL
		for (int si = 0; si < kVolumeSamples; si++)
		{
			float3 ld = kSpotLightApex - sampleWS;
			float atten = ComputeAttenuation(ld, sampleWS, 0, uv);
			float scatter = ComputeScattering(sampleWS, normalize(ld), ray);
			radiance += atten * dT * scatter;

			sampleWS += rayDt;
		}

#if defined(DS_LIGHT_USE_FOG)
    radiance *= GetFogExtinctionToPoint(rayToNear, length(rayToNear));
#endif
	return fixed4(_DS_HazeLightVolumeColour.rgb * saturate(radiance), 1);
}