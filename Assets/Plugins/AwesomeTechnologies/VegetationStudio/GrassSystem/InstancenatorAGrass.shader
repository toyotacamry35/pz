Shader "Instancenator/InstancenatorAGrass"
{
	Properties
	{
		[Header(Colors)]
		_Color ("Healthy", Color) = (1,1,1,0.1)
		_ColorB ("Dry", Color) = (1,1,1,0.1)
		_ColorC ("Wave", Color) = (1,1,1,0.1)
		
		[Header(Base)]
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset]_SpecTex("Emission (RGB) Roughness (A)", 2D) = "black" {}
		_CutoffMinDistance("CutoffMinDistance", Float) = 10
		_CutoffMaxDistance("CutoffMaxDistance", Float) = 100
		_CutoffMax("CutoffAtMinDistance", Range(0,1)) = 0.5
		_CutoffMin("CutoffAtMaxDistance", Range(0,1)) = 0.1
		_Smootness("Roughness", Range(0, 1)) = 0.4
			
		[Header(Settings)]
		[Toggle] USE_PIVOT("Use Pivot", Float) = 1
		[Toggle] MULTI_PIVOTS("Multi Pivots", Float) = 1
		//[Toggle] TOUCH_BEND("Use touch bend", Float) = 1
		[Toggle] GPU_FRUSTUM("Use GPU culling", Float) = 1
		
		[Header(Culling)]
		[Toggle] FAR_CULL("Use cull", Float) = 1
		_CullFarStart("Cull start" , Range(0,1000)) = 50
		_CullFarDistance("Cull fade distance" , Range(0,200)) = 10
		_WindAffectDistance("Wind affect distance" , Range(0,200)) = 50
				
		[Header(Grass Emission)]
		[Toggle] EMISSION_GRASS("Enable Emission", Float) = 1
		_EmissionTodFrequency("Frequency", Float) = 1
		_EmissionTodSize("Size", Float) = 0.25
		_EmissionTodMinimum("Minimum", Float) = 0.65
		_EmissionTodScale("Scale X", Float) = 5
		_EmissionTodScale2("Scale Z", Float) = 5
		
		
		
		[Enum(TOD.EmissionCurve)] _EmissionCurve ("Emission Curve", Float) = 0
		
		_EmissionWindPower01("Emission Wind Power 1", Range(0.01,5)) = 3
		_EmissionWindSize01("Emission Wind Size 1", Range(-1,1)) = -0.3
		_EmissionWindPower02("Emission Wind Power 2", Range(0.01,5)) = 4
		_EmissionWindSize02("Emission Wind Size 2", Range(-1,1)) = -0.2

		_Bounds ("_Bounds", Vector) = (0,0,0,0)
		_ConstantScale("ConstantScale", Vector) = (1,1,1)
	}
	SubShader{
		Cull off

		Tags { "Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="AFSGrass"
			"AfsMode"="Grass"
			"DisableBatching" = "LODFading" }
		LOD 200

		CGPROGRAM

		#pragma target 4.6			
		#pragma multi_compile_instancing
		#pragma vertex vert
		#pragma surface surf AFSSpecular //alphatest:_Cutoff fullforwardshadows addshadow 
		#pragma instancing_options procedural:setupScale
		
		#pragma multi_compile FAR_CULL_ON __
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma multi_compile EMISSION_GRASS
		#pragma multi_compile USE_PIVOT_ON __
		#pragma multi_compile GRASS_MOTION
		#pragma shader_feature MULTI_PIVOTS_ON __
		
		#define AG_GRASS_MAXIMUM_WIND_WAVE_BEND 0.2
		#define AG_GRASS_SIN_NOISE_BEND 0.02

			// vertex components
		#define AG_PHASE_SHIFT v.color.g
		#define AG_BEND_FORCE v.color.b
		#define AG_AO_MULTIPLIER v.color.r
		
		#define INSTANCENATOR_FOLIAGE
		
		#ifdef FAR_CULL_ON
			#ifndef UNITY_PROCEDURAL_INSTANCING_ENABLED
				#define FAR_CULL_ON_SIMPLE
			#endif
		#endif
			
		#include "Includes/AfsPBSLighting.cginc"
		#include "waving_tint.cginc"
		#include "Assets/Src/Instancenator/Shaders/InstancenatorShaderBase.cginc"
		#include "a_grass.cginc"
		
		void setupScale()
		{
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			setup();
		#endif
		}

		float3 GetInstanceScale()
		{
			return GetInstanceData().instanceValue.xyz * instanceTransitionScale;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
