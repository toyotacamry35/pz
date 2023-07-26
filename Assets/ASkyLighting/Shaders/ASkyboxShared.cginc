
#ifndef A_SKYBOX_SHARED_INCLUDED
#define A_SKYBOX_SHARED_INCLUDED

#include "UnityCG.cginc"


// Atmosphere.
uniform half4  _NightColor;
uniform float  _HorizonFade;

uniform float4    _MoonDir;
uniform float4x4  _MoonMatrix;
uniform float4    _MoonHalo;

uniform float4    _MoonDir02;
uniform float4x4  _MoonMatrix02;
uniform float4    _MoonHalo02;

uniform float4    _MoonDir03;
uniform float4x4  _MoonMatrix03;
uniform float4    _MoonHalo03;

uniform float4    _MoonDir04;
uniform float4x4  _MoonMatrix04;
uniform float4    _MoonHalo04;


uniform samplerCUBE _StarsCubemap;
uniform samplerCUBE _StarsNoiseCubemap;

uniform half        _StarsIntensity;
uniform half4       _StarsColor;
uniform float       _StarsTwinkle;
half4               _StarsCubemap_HDR;

uniform float4x4    _StarsMatrix;
uniform float4x4    _StarsNoiseMatrix;



inline half HorizonFade(float dir)
{
	return pow(max(0, dir), _HorizonFade);
}

inline half getRayleighPhase(half eyeCos2)
{
	return 0.75 + 0.75*eyeCos2;
}
		
inline half getRayleighPhase(half3 light, half3 ray)
{
	half eyeCos	= dot(light, ray);
	return getRayleighPhase(eyeCos * eyeCos);
}
		
inline float scale(float inCos)
{
	float x = 1.0 - inCos;

	#if defined(SHADER_API_N3DS)
		return 0.25 * exp(-0.00287 + x*x*x*(-6.80 + x*5.25));
	#else
		return 0.25 * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
	#endif
}
		
inline half getMiePhase(half eyeCos)
{


	half eyeCos2 = eyeCos * eyeCos;
	half temp = 1.0 + MIE_G2 - 2.0 * MIE_G * eyeCos;
	temp = pow(temp, pow(_SunSize * _SunSpotSize,0.65) * 10);
	temp = max(temp,1.0e-4); // prevent division by zero, esp. in half precision
	temp = 1.5 * ((1.0 - MIE_G2) / (2.0 + MIE_G2)) * (1.0 + eyeCos2) / temp;
	#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
		temp = pow(temp, .454545);
	#endif
	return temp;
}
		
inline half calcSunSpot(half3 delta)
{
	half  dist  = length(delta);
	half  spot  = 1.0 - smoothstep(0, _SunSize, dist);
	return  clamp(kSunScale * spot * spot, 0,1) ;
}

inline float CalcLuminance(float3 color)
{
	return dot(color, float3(0.299f, 0.587f, 0.114f));
}

inline half4 flerp(half4 a, half4 b) 
{
	half4 c = 0.0;
	c.rgb = lerp(b.rgb * b.a, a.rgb, a.a) / max(0.0001, lerp(b.a, 1.0, a.a));
	c.a = lerp(b.a, 1.0, a.a);

	return c;
}

inline half MoonColorAdvanced(float4 coords) //colony: spherical bump for correct lighting
{
	half mask = 1.0;

	float scale = 1.3; 
	coords.xy *= scale;
	coords.xy -= 0.5* (scale - 1.0);
	coords.xy = 2.0*coords.xy - 1.0;
	
	float coordsDot = dot(coords.xy, coords.xy);

	if ( coordsDot < 1.0 )
		mask = 0.0;
	return mask;
}

inline half3 StarsColor(float3 coords)
{
	half4 cube  = texCUBE(_StarsCubemap, coords);
	half3 color = DecodeHDR(cube,  _StarsCubemap_HDR)* _StarsColor.rgb;
	color.rgb  *= (unity_ColorSpaceDouble.rgb) * _StarsIntensity;
	return color;
}

inline half3 moonHalo(float4 viewDir, float3 vec, float4 moonHalo, float fade)
{
	float3 delta = viewDir.xyz - vec;
	float dist = (viewDir.w * 0.77)/2;
    float spot;
	if (length(delta) > dist)
		spot   = 1-smoothstep(-3, 1, length(delta) * moonHalo.a);
	else
		spot   = 1-smoothstep(-3, 1, dist * moonHalo.a);
			
    return spot *  moonHalo.rgb;
}

inline half calcSunAttenuation(half3 lightPos, half3 ray, float convergence)
{
    return pow(saturate(dot(lightPos, ray)), convergence);
}


#endif