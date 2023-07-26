
///*************************************************************
/// Custom procedural skybox.
/// Added moon, night  color and stars.
/// Notice: this shader is property by Unity Technologies.
///*************************************************************

Shader "ASkybox/Skybox"  
{

Properties
{
	_SunSize("Sun Size", Range(0,1)) = 0.04
	_AtmosphereThickness("Atmoshpere Thickness", Range(0,5)) = 1.0
	_TopSkyColor("Top Sky Color", Color) = (.5, .5, .5, 0.1)
	_GroundColor("Ground", Color) = (.369, .349, .341, 1)
	_Exposure("Exposure", Range(0, 8)) = 1.3
	
	
}

SubShader 
{

	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass 
	{

		CGPROGRAM
		#pragma glsl
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 4.0

		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		

		#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
		#pragma multi_compile __ HORIZONFADE
		#pragma multi_compile __ STARS

		
		uniform half     _SunSize;
		uniform half     _SunSpotSize;
		uniform float3   _SunDir;
		uniform half4    _SunColor;
		uniform float4x4 _SunMatrix;
		
		uniform float4  _SkyTint;
		uniform float4  _TopSkyColor;
		uniform float4  _HorizonSkyColor;
		uniform half   _AtmosphereThickness;
		uniform half   _TopSky;
		uniform float3  _GroundColor;
		
		uniform half  _Exposure;	
		
		#if defined(UNITY_COLORSPACE_GAMMA)
			#define GAMMA 2
			#define COLOR_2_GAMMA(color) color
			#define COLOR_2_LINEAR(color) color*color
			#define LINEAR_2_OUTPUT(color) sqrt(color)
		#else
			#define GAMMA 2.2
			#define COLOR_2_GAMMA(color) ((unity_ColorSpaceDouble.r>2.0) ? pow(color,1.0/GAMMA) : color)
			#define COLOR_2_LINEAR(color) color
			#define LINEAR_2_LINEAR(color) color
		#endif

		static const float3 kDefaultScatteringWavelength = float3(.65, .57, .475);
		static const float3 kVariableRangeForScatteringWavelength = float3(.15, .15, .15);

		#define OUTER_RADIUS 1.025
		static const float kOuterRadius = OUTER_RADIUS;
		static const float kOuterRadius2 = OUTER_RADIUS*OUTER_RADIUS;
		static const float kInnerRadius = 1.0;
		static const float kInnerRadius2 = 1.0;

		static const float kCameraHeight = 0.0001;

		#define kRAYLEIGH (lerp(0, 0.0025, pow(_AtmosphereThickness,2.5)))		// Rayleigh constant
		#define kMIE 0.0010      		
		#define kSUN_BRIGHTNESS 20.0 	

		#define kMAX_SCATTER 50.0 

		static const half  kSunScale = 400.0 * kSUN_BRIGHTNESS;
		static const float kKmESun = kMIE * kSUN_BRIGHTNESS;
		static const float kKm4PI = kMIE * 4.0 * 3.14159265;
		static const float kScale = 1.0 / (OUTER_RADIUS - 1.0);
		static const float kScaleDepth = 0.25;
		static const float kScaleOverScaleDepth = (1.0 / (OUTER_RADIUS - 1.0)) / 0.25;
		static const float kSamples = 2.0; 

		#define MIE_G (-0.990)
		#define MIE_G2  MIE_G * MIE_G //0.9801

		#define SKY_GROUND_THRESHOLD 0.02
		
	
		#ifndef SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
			#if defined(SHADER_API_MOBILE)
				#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 1
			#else
				#define SKYBOX_COLOR_IN_TARGET_COLOR_SPACE 0
			#endif
		#endif
		
		#include "ASkyboxShared.cginc"

		struct appdata_t
        {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
		
		struct Input
		{

			float4	pos : POSITION;
			float3 worldPos            : COLOR;

			// calculate sky colors in vprog
			half3	groundColor		   : NORMAL;
			half3	skyColor		   : TANGENT;
			half3	sunColor		   : TEXCOORD0;
			
			#if defined(STARS) 
			float3  starsCoords        : TEXCOORD1;
			#endif
			
			float4  moonCoords          : TEXCOORD2;
			float4  moonCoords02          : TEXCOORD3;
			float4  moonCoords03          : TEXCOORD4;
			float4  moonCoords04          : TEXCOORD5;

		};



		Input vert (appdata_base v)
		{
			UNITY_SETUP_INSTANCE_ID(v);
			Input o;
			o.pos = UnityObjectToClipPos(v.vertex);

			o.worldPos = mul((float3x3)unity_ObjectToWorld, v.vertex.xyz);
			float3 eyeRay = normalize(o.worldPos);

			o.moonCoords.xyz = mul((float3x3)_MoonMatrix, v.vertex.xyz) / _MoonDir.w + 0.5;
			o.moonCoords.w = saturate(dot(_MoonDir.xyz, o.worldPos));

			o.moonCoords02.xyz = mul((float3x3)_MoonMatrix02, v.vertex.xyz) / _MoonDir02.w + 0.5;
			o.moonCoords02.w = saturate(dot(_MoonDir02.xyz, o.worldPos));
			
			o.moonCoords03.xyz = mul((float3x3)_MoonMatrix03, v.vertex.xyz) / _MoonDir03.w + 0.5;
			o.moonCoords03.w = saturate(dot(_MoonDir03.xyz, o.worldPos));
			
			o.moonCoords04.xyz = mul((float3x3)_MoonMatrix04, v.vertex.xyz) / _MoonDir04.w + 0.5;
			o.moonCoords04.w = saturate(dot(_MoonDir04.xyz, o.worldPos));
			
			#if defined(STARS) 
			float3 sunCoords = mul((float3x3)_SunMatrix, v.vertex.xyz);
			o.starsCoords  = mul((float3x3)_StarsMatrix, sunCoords.xyz);
			#endif

			float3 kSkyTintInGammaSpace = COLOR_2_GAMMA(_HorizonSkyColor.rgb); 

			float3 kScatteringWavelength = lerp 
			(
				kDefaultScatteringWavelength - kVariableRangeForScatteringWavelength,
				kDefaultScatteringWavelength + kVariableRangeForScatteringWavelength,

				// Using Tint in sRGB gamma allows for more visually linear interpolation and to keep (.5) at (128, gray in sRGB) point
				half3(1,1,1) - kSkyTintInGammaSpace //TODO fix this
			); 

			float3 kInvWavelength = 1.0 / pow(kScatteringWavelength, 4);
			float kKrESun         = kRAYLEIGH * kSUN_BRIGHTNESS;
			float kKr4PI          = kRAYLEIGH * 4.0 * 3.14159265;

			float3 cameraPos      = float3(0,kInnerRadius + kCameraHeight,0); 	

			float far = 0.0; half3 cIn, cOut; 


			if(eyeRay.y >= 0.0)
			{
				far = sqrt(kOuterRadius2 + kInnerRadius2 * eyeRay.y * eyeRay.y - kInnerRadius2) - kInnerRadius * eyeRay.y;

				float3 pos = cameraPos + far * eyeRay;

				float height      = kInnerRadius + kCameraHeight;
				float depth       = exp(kScaleOverScaleDepth * (-kCameraHeight));
				float startAngle  = dot(eyeRay, cameraPos) / height;
				float startOffset = depth*scale(startAngle);

				float  sampleLength = far / kSamples;
				float  scaledLength = sampleLength * kScale;
				float3 sampleRay    = eyeRay * sampleLength;
				float3 samplePoint  = cameraPos + sampleRay * 0.5;

				// Now loop through the sample rays
				float3 frontColor = float3(0.0, 0.0, 0.0);

				{
					float  height      = length(samplePoint);
					float  depth       = exp(kScaleOverScaleDepth * (kInnerRadius - height));
					float  lightAngle  = dot(_SunDir.xyz, samplePoint) / height;
					float  cameraAngle = dot(eyeRay, samplePoint) / height;
					float  scatter     = (startOffset + depth*(scale(lightAngle) - scale(cameraAngle)));
					float3 attenuate   = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));

					frontColor  += (attenuate * (depth * scaledLength));
					samplePoint += sampleRay;
				}
				
				{
					float  height      = length(samplePoint);
					float  depth       = exp(kScaleOverScaleDepth * (kInnerRadius - height));
					float  lightAngle  = dot(_SunDir.xyz, samplePoint) / height;
					float  cameraAngle = dot(eyeRay, samplePoint) / height;
					float  scatter     = (startOffset + depth*(scale(lightAngle) - scale(cameraAngle)));
					float3 attenuate   = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));

					frontColor  += attenuate * (depth * scaledLength);
					samplePoint += sampleRay;				
				}

				cIn  = (frontColor * (kInvWavelength * kKrESun));
				cOut = frontColor * kKmESun;

			}
			else
			{

				far = (-kCameraHeight) / (min(-0.001, eyeRay.y));

				float3 pos = cameraPos + far * eyeRay;


				float depth        = exp((-kCameraHeight) * (1.0/kScaleDepth));
				float cameraAngle  = dot(-eyeRay, pos);
				float lightAngle   = dot(_SunDir.xyz, pos);
				float cameraScale  = scale(cameraAngle);
				float lightScale   = scale(lightAngle);
				float cameraOffset = depth*cameraScale;
				float temp         = (lightScale + cameraScale);

				float  sampleLength = far / kSamples;
				float  scaledLength = sampleLength * kScale;
				float3 sampleRay    = eyeRay * sampleLength;
				float3 samplePoint  = cameraPos + sampleRay * 0.5;


				float3 frontColor = float3(0.0, 0.0, 0.0);
				float3 attenuate;

				{
					float height  = length(samplePoint);
					float depth   = exp(kScaleOverScaleDepth * (kInnerRadius - height));
					float scatter = depth*temp - cameraOffset;
					attenuate     = exp(-clamp(scatter, 0.0, kMAX_SCATTER) * (kInvWavelength * kKr4PI + kKm4PI));
					frontColor   += attenuate * (depth * scaledLength);
					samplePoint  += sampleRay;
				}

				cIn  = frontColor * (kInvWavelength * kKrESun + kKmESun);
				cOut = clamp(attenuate, 0.0, 1.0);
			}

		
			o.groundColor	= _Exposure * (cIn + (_SunColor * saturate(dot(_SunDir.xyz, float3(0,1,0)))  + getRayleighPhase(_SunDir.xyz, float3(0,0,-1))) * COLOR_2_LINEAR(_GroundColor) * cOut); //TODO - fix this
			o.skyColor	= _Exposure * (cIn * getRayleighPhase(_SunDir.xyz, -eyeRay));
			o.sunColor	= _Exposure * (cOut *_SunColor.xyz);

			#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
			o.groundColor	= sqrt(OUT.groundColor);
			o.skyColor	= sqrt(OUT.skyColor);
			o.sunColor    = sqrt(OUT.sunColor);
			#endif

			return o;
		}

	
		
		
		float4 frag (Input IN) : COLOR
		{
			float3 col = half3(0.0, 0.0, 0.0);

			half3 ray = normalize(IN.worldPos.xyz);
			half  y   = -ray.y / SKY_GROUND_THRESHOLD;

			half currentHorizonFade = 1;
			
			#if defined (HORIZONFADE)
				half horizonFade = saturate(HorizonFade(ray.y/0.1));
				currentHorizonFade = horizonFade;
			#endif

			half colorMixFactor =  pow(max(0.001, ray.y), _TopSky);
			half skyLuminance = CalcLuminance(IN.skyColor);
			 
			half horizonAlpha = _HorizonSkyColor.a;
			half topSkyAlpha = _TopSkyColor.a;
			_HorizonSkyColor.a *= (1.0 - colorMixFactor);
			_TopSkyColor.a *= colorMixFactor;

			float4 colorMix = float4(8.0 * skyLuminance.xxx, 1.0) * flerp( _HorizonSkyColor, _TopSkyColor);

			IN.skyColor = max(IN.skyColor , 0.0);
			IN.skyColor = lerp(IN.skyColor, colorMix , _HorizonSkyColor.a + _TopSkyColor.a );

			float colorMixFactorGround = pow(0.5, _TopSky);
			float groundLuminance = CalcLuminance(IN.groundColor);
			half4 colorMixGround = half4(8.0 * groundLuminance.xxx, 1.0) * flerp( half4(_HorizonSkyColor.rgb, horizonAlpha * (1.0 - colorMixFactorGround)), half4(_TopSkyColor.rgb, topSkyAlpha * colorMixFactorGround));
			
			IN.groundColor = lerp(IN.groundColor, 0.5 * IN.groundColor + 4.0 *  colorMixGround *  COLOR_2_LINEAR(_GroundColor), colorMixGround.a);

 
			half moonMask = MoonColorAdvanced(IN.moonCoords); 
			half moonMask02 = MoonColorAdvanced(IN.moonCoords02);
			half moonMask03 = MoonColorAdvanced(IN.moonCoords03); 
			half moonMask04 = MoonColorAdvanced(IN.moonCoords04);
						
			IN.skyColor += _Exposure * moonHalo(_MoonDir, - ray, _MoonHalo, _HorizonFade);
			IN.skyColor += _Exposure * moonHalo(_MoonDir02, - ray, _MoonHalo02, _HorizonFade);
			IN.skyColor += _Exposure * moonHalo(_MoonDir03, - ray, _MoonHalo03, _HorizonFade);
			IN.skyColor += _Exposure * moonHalo(_MoonDir04, - ray, _MoonHalo04, _HorizonFade);
			
			#if defined(STARS) 
			half3  starsColor  = StarsColor(IN.starsCoords);

			starsColor.rgb  *=  moonMask;
			starsColor.rgb  *=  moonMask02;
			starsColor.rgb  *=  moonMask03;
			starsColor.rgb  *=  moonMask04;	
			
			//starsColor.rgb *= currentHorizonFade;
			starsColor.rgb *= pow(max(0.001, ray.y), 2);

			IN.skyColor += _Exposure * starsColor.rgb;
			#endif

			col = lerp(IN.skyColor, IN.groundColor, saturate(y));
			
					
			if(y<0.0)
			{
				half focusedEyeCos = pow(saturate(dot(_SunDir, ray)), 5);
				half3 mie = IN.sunColor * getMiePhase(-focusedEyeCos) * 5 + _SunColor * 4 * calcSunSpot(_SunDir.xyz - ray);
				half3 mieFinal = mie * saturate(col);// * col;
				mieFinal *= moonMask;
				mieFinal *= moonMask02;
				mieFinal *= moonMask03;
				mieFinal *= moonMask04; 
				col +=  mieFinal;
			}
						
			#if defined(UNITY_COLORSPACE_GAMMA) && !SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
			col = LINEAR_2_OUTPUT(col);
			#endif

			return float4(col,1);
		}
		ENDCG
	}
}

Fallback Off

}
