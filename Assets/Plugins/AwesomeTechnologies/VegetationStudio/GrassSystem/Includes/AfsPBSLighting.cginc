#ifndef AFS_PBS_LIGHTING_INCLUDED
#define AFS_PBS_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"
#include "../Includes/AfsBRDF.cginc"


//-------------------------------------------------------------------------------------
// Compatibilty settings
// Uncomment either "#define USEALLOY" or "#define USEUBER" to enable deferred lighting support for the given shader package.
// Leave them commented in case you are using the Lux Foliage deferred lighting shader.
// More infos in the docs.

#define USEALLOY
// #define USEUBER

//-------------------------------------------------------------------------------------
// Debug shader lighting

struct SurfaceOutputAFSUnlit
{
	fixed3 Albedo;
	fixed3 Normal;
	half3 Emission;
	half Smoothness;
	half Occlusion;
	fixed Alpha;	
};

inline half4 LightingAFSUnlit (SurfaceOutputAFSUnlit s, half3 viewDir, UnityGI gi)
{
	half4 c;
	c.rgb = s.Albedo;
	c.a = s.Alpha;
	return c;
}
inline void LightingAFSUnlit_GI (
	SurfaceOutputAFSUnlit s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

//-------------------------------------------------------------------------------------

#if defined (VERTEXLIT)
	half _AfsVertexLitHorizonFade;
#else
	half _HorizonFade;
#endif

//-------------------------------------------------------------------------------------
// Surface shader output structure to be used with physically
// based shading model.
struct SurfaceOutputAFSSpecular
{
	fixed3 Albedo;			// diffuse color
	fixed3 Specular;		// specular color
	fixed3 Normal;			// tangent space normal, if written
	half3 Emission;
	half Smoothness;		// 0=rough, 1=smooth
	half Occlusion;
	fixed Alpha;
	fixed2 Translucency;	// x=strength, y=power
	fixed Lighting;

	#if defined (TANGETFREELIGHTING)
		fixed3 WorldNormal;
	#endif

	fixed3 VertexNormal;
};



//-------------------------------------------------------------------------------------
float3x3 GetCotangentFrame( float3 N, float3 p, float2 uv )
{
    // get edge vectors of the pixel triangle
    float3 dp1 = ddx( p );
    float3 dp2 = ddy( p );
    float2 duv1 = ddx( uv );
    float2 duv2 = ddy( uv );

    // solve the linear system
    float3 dp2perp = cross( dp2, N );
    float3 dp1perp = cross( N, dp1 );
    float3 T = -(dp2perp * duv1.x + dp1perp * duv2.x);

    float3 B = -(dp2perp * duv1.y + dp1perp * duv2.y); // * unity_WorldTransformParams.w;
 
    // construct a scale-invariant frame 
    float invmax = rsqrt( max( dot(T,T), dot(B,B) ) );
    return float3x3( T * invmax, B * invmax, N );
}


// https://forum.libcinder.org/topic/calculating-normals-after-displacing-vertices-in-shader
float3x3 GetCotangentFrameNew( float3 N, float3 p, float2 uv )
{
	// calculate tangent and bitangent
	float3 P1 = ddx( p );
	float3 P2 = ddy( p );
	float2 Q1 = ddx( uv );
	float2 Q2 = ddy( uv );
	 
	float3 T = normalize(  P1 * Q2.y - P2 * Q1.y );
	float3 B = normalize(  P2 * Q1.x - P1 * Q2.x );

	// construct tangent space matrix and perturb normal
	return float3x3( T, B, N );
}


// Horizon Occlusion for Normal Mapped Reflections: http://marmosetco.tumblr.com/post/81245981087
float GetHorizonOcclusion(float3 V, float3 normalWS, float3 vertexNormal, float horizonFade)
{
    float3 R = reflect(-V, normalWS);
    float specularOcclusion = saturate(1.0 + horizonFade * dot(R, vertexNormal));
    // smooth it
    return specularOcclusion * specularOcclusion;
}


//-------------------------------------------------------------------------------------
//	This is more or less like the "LightingStandardSpecular" but with translucency added on top ond wrapped around diffuse

inline half4 LightingAFSSpecular (SurfaceOutputAFSSpecular s, half3 viewDir, UnityGI gi)
{
	#if defined (TANGETFREELIGHTING)
		s.Normal = normalize(s.WorldNormal);
	#else
		s.Normal = normalize(s.Normal);
	#endif

	// energy conservation
	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha (s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	// energy conserving wrapped around diffuse lighting
	// http://blog.stevemcauley.com/2011/12/03/energy-conserving-wrapped-diffuse/
	half wrap1 = 0.4;
	half NdotLDirect = saturate( ( dot(s.Normal, gi.light.dir) + wrap1 ) / ( (1 + wrap1) * (1 + wrap1) ) );

	#if defined(ISTREE)
		#if !defined(DIRECTIONAL) && !defined(DIRECTIONAL_COOKIE)
		//	Fade out point and spot lights
			gi.light.color *= s.Lighting;
		#endif
	#endif

//	Horizon Occlusion
	#if defined (VERTEXLIT)
		gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _AfsVertexLitHorizonFade );
	#else
		gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _HorizonFade );
	#endif
	
	half4 c = BRDF1_AFS_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, NdotLDirect, s.Normal, viewDir, gi.light, gi.indirect);
//	For indirect lighting we simply use the built in BRDF
	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
	c.a = outputAlpha;

//	Add Translucency – needs light dir and intensity: so real time only
	#if !defined(LIGHTMAP_ON)

	//	Pick grass: spec color r is 0
		half grass = saturate(1 - s.Specular.r * 255);

	//	Best for grass as the normal counts less
	//	//	https://colinbarrebrisebois.com/2012/04/09/approximating-translucency-revisited-with-simplified-spherical-gaussian/
		half transPower = s.Translucency.y * 10.0h;
		half3 transLightDir = gi.light.dir + s.Normal * 0.01h;
		half transDot = dot( -transLightDir, viewDir ); // sign(minus) comes from eyeVec
		transDot = exp2(saturate(transDot) * transPower - transPower);
		half3 lightScattering = transDot * gi.light.color * lerp(1.0h - NdotLDirect, 1.0h, grass);
		c.rgb += s.Albedo * 4.0h * s.Translucency.x * lightScattering;
	#endif

	return c;
}

#if !defined(ALWAYSFORWARD)
inline half4 LightingAFSSpecular_Deferred (SurfaceOutputAFSSpecular s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	// energy conservation
	half oneMinusReflectivity;
	s.Albedo = EnergyConservationBetweenDiffuseAndSpecular (s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

	#if defined (TANGETFREELIGHTING)
		s.Normal = normalize(s.WorldNormal);
	#else
		s.Normal = normalize(s.Normal);
	#endif

//	Horizon Occlusion
	#if defined (VERTEXLIT)
		gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _AfsVertexLitHorizonFade );
	#else
		gi.indirect.specular *= GetHorizonOcclusion( -viewDir, s.Normal, s.VertexNormal, _HorizonFade );
	#endif

//	For indirect lighting we simply use the built in BRDF
	half4 c = UNITY_BRDF_PBS (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);

	outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);
	
	half4 emission;
//	Alloy Support
	#if defined (USEALLOY)
		outSpecSmoothness = half4(s.Specular, s.Smoothness);
		outNormal = half4(s.Normal * 0.5h + 0.5h, 0.75h);
		emission = half4(s.Emission + c.rgb, s.Translucency.x);
//	UBER Support
	#elif defined (USEUBER)
		//outDiffuseOcclusion = half4(half3(1,0,0), s.Occlusion);
		outSpecSmoothness = half4(s.Specular, s.Smoothness);
		float translucency = floor(saturate(s.Translucency.x) * 15) * (-128);
		outNormal = half4(s.Normal * 0.5h + 0.5h, 1);
		emission = half4(s.Emission + c.rgb, translucency);
// 	Lux Support
	#else
		outSpecSmoothness = half4(s.Specular.r, s.Translucency.y, s.Translucency.x, s.Smoothness);
		// Mark as translucent
		outNormal = half4(s.Normal * 0.5h + 0.5h, 0.66h);
		emission = half4(s.Emission + c.rgb, 1);
	#endif

	return emission;
}
#endif


inline void LightingAFSSpecular_GI (
	SurfaceOutputAFSSpecular s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

#endif // AFS_PBS_LIGHTING_INCLUDED
