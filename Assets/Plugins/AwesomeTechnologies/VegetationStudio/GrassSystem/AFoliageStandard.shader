Shader "AwesomeTechnologies/AFoliageStandard" {
	Properties {
	
		[Header(Colors)]
		_Color ("Healthy", Color) = (1,1,1,0.1)
		_ColorB ("Dry", Color) = (1,1,1,0.1)
		_ColorC ("Wave", Color) = (1,1,1,0.1)
		
		[Header(Base)]
		[Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Toggle] NORMAL ("Normal Up ", Float) = 0
		[Toggle] NORMAL_MAP ("Normal Map  ", Float) = 0
		[NoScaleOffset] _BumpMap	("Normal (RGB)", 2D) = "bump" {}
		[NoScaleOffset]_SpecTex("Emission (RGB) Roughness (A)", 2D) = "black" {}
		_CutoffMinDistance("CutoffMinDistance", Float) = 10
		_CutoffMaxDistance("CutoffMaxDistance", Float) = 100
		_CutoffMax("CutoffAtMinDistance", Range(0,1)) = 0.5
		_CutoffMin("CutoffAtMaxDistance", Range(0,1)) = 0.1
		_Smoothness("Roughness", Range(0, 1)) = 0.4
		_SpecularReflectivity				("Specular Reflectivity", Color) = (0.2,0.2,0.2)
		_BackfaceSmoothness 				("Backface Smoothness", Range(0,2)) = 1
		
		_LeafTurbulence 					("Leaf Turbulence", Range(0,1)) = 0.2
		
		[Header(Settings)]
		[Toggle] MULTI_PIVOTS("Multi Pivots", Float) = 1
		[Toggle] GPU_FRUSTUM("Use GPU culling", Float) = 1
		[Toggle] WIND("Use Wind", Float) = 1
		
		[Header(Culling)]
		[Toggle] FAR_CULL("Use Cull", Float) = 1
		_CullFarStart("Cull start" , Range(0,1000)) = 50
		_CullFarDistance("Cull fade distance" , Range(0,200)) = 10
				
		[Header(Grass Emission)]
		[Toggle] EMISSION_GRASS("Enable Emission", Float) = 1
		[LM_Emission] 
		[HDR]
		_EmissionColor ("Tint", Color) = (1,1,1)
		_EmissionTodFrequency("Frequency", Range(0,5)) = 1
		_EmissionTodSize("Size", Range(0,1)) = 0.25
		_EmissionTodMinimum("Minimum", Range(0,1)) = 0.65
		_EmissionTodScale("Scale", Range(0,10)) = 5
		[Enum(TOD.EmissionCurve)] _EmissionCurve ("Emission Curve", Float) = 0

		_Bounds ("_Bounds", Vector) = (0,0,0,0)
		_ConstantScale("ConstantScale", Vector) = (1,1,1)

		
		


	}

		SubShader{
			Tags { "Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="AFSGrass"
			"AfsMode"="Grass"
			"DisableBatching" = "LODFading" }
			LOD 200

			Cull [_Culling]

			CGPROGRAM
			#pragma surface surf AFSSpecular fullforwardshadows alphatest:_Cutoff vertex:vert addshadow 
			#pragma target 4.0 
			#pragma instancing_options procedural:setupScale
			#pragma shader_feature FAR_CULL_ON __
			#pragma shader_feature NORMAL_ON __
			#pragma shader_feature NORMAL_MAP_ON __
			#pragma shader_feature GPU_FRUSTUM_ON __
			#pragma shader_feature EMISSION_GRASS_ON __
			#pragma shader_feature MULTI_PIVOTS_ON __
			#pragma shader_feature WIND_ON __

			#ifdef FAR_CULL_ON
				#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
					#define FAR_CULL_ON_PROCEDURAL_INSTANCING
				#else
					#define FAR_CULL_ON_SIMPLE
				#endif
			#endif

				
		#include "Includes/AfsPBSLighting.cginc"
		#include "waving_tint.cginc"
		#include "a_foliage_vs.cginc"
		#include "a_foliage_fs.cginc"
		#include "a_indirect.cginc"

		
		ENDCG
	}
	
	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
