// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: excluded shader from DX11 because it uses wrong array syntax (type[size] name)
#pragma exclude_renderers d3d11

#ifndef ASKYPLANETSHARED_CGINC
# define ASKYPLANETSHARED_CGINC

	uniform sampler2D _MainTex;
	uniform sampler2D _BumpMap;
	uniform sampler2D _SpecMap;
	uniform sampler2D _LightMap;
	uniform sampler2D _CloudMap;
	uniform sampler2D _CloudMap02;
	uniform float4 _BumpMap_ST;
	uniform float4 _SpecColor; 
	uniform float _Shininess;
	uniform float _LightMapIntensity;
	uniform float	_CloudStrength;
	uniform float	_CloudShadowsStrength;
	uniform float	_CloudSpeed;
	
	uniform float	_CloudStrength02;
	uniform float	_CloudShadowsStrength02;
	uniform float	_CloudSpeed02;
	
	uniform float3 _SunDirection;
	uniform float3 _SunColor;
	uniform float _Magnitude;
	uniform float4 _PlanetsDataArray[4];
	uniform float _PlanetsCount;
	float3 sunColor;
	

	float _DistanceBlending = 0.0025;
	float _HighlightOffset = 0.025;
	
	float	_AtmoThickness;
	float	atmoSize;
	int		_atmosphere;
	float 	invWavelength;
	float 	fOuterRadius;
	float 	fInnerRadius;
	float 	fKrESun;
	float 	fKmESun;
	float	fKr4PI;
	float 	fKm4PI;
	float 	fScale;
	float 	fScaleDepth;
	float 	fScaleOverScaleDepth;

	#define LIGHT_DISTANCE 0.0025
	
	float 	scale(float fCos)
	{
		float x = 1.0 - fCos;
		return fScaleDepth * exp(-0.00287 + x*(0.459 + x*(3.83 + x*(-6.80 + x*5.25))));
	}

	float 	getMiePhase(float fCos, float fCos2)
	{
		return 1.5 * ((0.01) / (3.0)) * (1.0 + fCos2) / pow(2 + 2 * fCos, 1.5);
	}

	
	half DistanceBlend(float3 worldPos)
	{
		float3 mainPivot = mul(unity_ObjectToWorld, float4(0,0,0,1));
		
		float dist = distance(_WorldSpaceCameraPos, worldPos);
		float distToObject =  distance(_WorldSpaceCameraPos, mainPivot);
		float distanceKiller = 1.0f;
		_Magnitude *= _DistanceBlending;
		float min = distToObject - _Magnitude;
		float max = distToObject + _Magnitude;
		if (dist > max) 
			distanceKiller = 0;
		else
			if (dist > min && dist <= max)
		distanceKiller = lerp(1,0, saturate(abs(dist-min)/(_Magnitude*2)));
		
		return distanceKiller;
	}

	float 	getRayleighPhase(float fCos2)
	{
		return (fCos2 + 0.75) * 0.75;
	}
     
	inline float getMask(float3 dir, float3 sphereDir, float scaleDist)
	{
		float d = length(cross(dir, sphereDir)) - scaleDist;
		if (dot(dir, sphereDir) > 0)
			return smoothstep(-1, 1, -d / _DistanceBlending);
		return 0;
	} 
	
	inline float2 getData(float3 posWorld, int i)
	{

		float3 	lightDirection = normalize(_SunDirection);
		float3 	viewDirection = normalize(_WorldSpaceCameraPos - posWorld);
		
		float3 sphereDirection = _PlanetsDataArray[i].xyz - posWorld;
		float sphereDistance = length(sphereDirection);
		sphereDirection = sphereDirection / sphereDistance;
		float scaleDist = _PlanetsDataArray[i].w / sphereDistance;
		
		float2 result = float2(0,0);
		
		result.x = getMask(lightDirection, sphereDirection, scaleDist);
		result.y = getMask(viewDirection, sphereDirection, scaleDist);
		
		return result;
	}
	
	inline float2 GetPlanetShadows(float3 posWorld)
	{
		
		float2 result = float2(0,0);
		for (int i=0; i<_PlanetsCount; i++)
		{
			float2 currentData = float2(0,0);
			currentData = getData(posWorld, i);
			result.x = max(result.x, currentData.x);
			result.y = max(result.y, currentData.y);
		}
	
		return result;
	}
	 
	

	
	
	
	


#endif