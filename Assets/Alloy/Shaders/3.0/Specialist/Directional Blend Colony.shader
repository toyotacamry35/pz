// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

Shader "Alloy/Directional Blend Colony" {
Properties {
    // Global Settings
    _Mode ("'Rendering Mode' {RenderingMode:{Opaque:{_Cutoff}, Cutout:{}, Fade:{_Cutoff}, Transparent:{_Cutoff}}}", Float) = 0
    _SrcBlend ("__src", Float) = 0
    _DstBlend ("__dst", Float) = 0
    _ZWrite ("__zw", Float) = 1
    [LM_TransparencyCutOff] 
    _Cutoff ("'Opacity Cutoff' {Min:0, Max:1}", Float) = 0.5
    [Toggle(EFFECT_BUMP)]
    _HasBumpMap ("'Normals Source' {Dropdown:{VertexNormals:{_BumpMap,_BumpScale,_DetailNormalMap,_DetailNormalMapScale,_WetNormalMap,_WetNormalMapScale,_BumpMap2,_BumpScale2}, NormalMaps:{}}}", Float) = 1
    [Toggle(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)]
    _MainRoughnessSource ("'Roughness Source' {Dropdown:{PackedMapAlpha:{}, BaseColorAlpha:{_SpecTex,_Occlusion,_MaterialMap2,_Occlusion2}}}", Float) = 0
    
    // Main Textures
    _MainTextures ("'Main Textures' {Section:{Color:0}}", Float) = 0
    [LM_Albedo] [LM_Transparency] 
    _Color ("'Tint' {}", Color) = (1,1,1,1)	
    [LM_MasterTilingOffset] [LM_Albedo] 
    _MainTex ("'Base Color(RGB) Opacity(A)' {Visualize:{RGB, A}}", 2D) = "white" {}
    _MainTexVelocity ("Scroll", Vector) = (0,0,0,0) 
    _MainTexUV ("UV Set", Float) = 0
    [LM_Metallic]
    _SpecTex ("'Metal(R) AO(G) Spec(B) Rough(A)' {Visualize:{R, G, B, A}, Parent:_MainTex}", 2D) = "white" {}
    [LM_NormalMap]
    _BumpMap ("'Normals' {Visualize:{NRM}, Parent:_MainTex}", 2D) = "bump" {}
    _BaseColorVertexTint ("'Vertex Color Tint' {Min:0, Max:1}", Float) = 0
     

    // Main Properties
    _MainPhysicalProperties ("'Main Properties' {Section:{Color:1}}", Float) = 0
    [LM_Metallic]
    _Metal ("'Metallic' {Min:0, Max:1}", Float) = 1
    _Specularity ("'Specularity' {Min:0, Max:1}", Float) = 1
    _SpecularTint ("'Specular Tint' {Min:0, Max:1}", Float) = 0
    _Roughness ("'Roughness' {Min:0, Max:1}", Float) = 1
    _Occlusion ("'Occlusion Strength' {Min:0, Max:1}", Float) = 1
    _BumpScale ("'Normal Strength' {}", Float) = 1
    
    // Parallax
    [Toggle(_PARALLAXMAP)]
    _ParallaxT ("'Parallax' {Feature:{Color:5}}", Float) = 0
    [Toggle(_BUMPMODE_POM)]
    _BumpMode ("'Mode' {Dropdown:{Parallax:{_MinSamples, _MaxSamples}, POM:{}}}", Float) = 0
    _ParallaxMap ("'Heightmap(G)' {Visualize:{RGB}, Parent:_MainTex}", 2D) = "black" {}
    _Parallax ("'Height' {Min:0, Max:0.08}", Float) = 0.02
    _MinSamples ("'Min Samples' {Min:1}", Float) = 4
    _MaxSamples ("'Max Samples' {Min:1}", Float) = 20
    
    // AO2
    [Toggle(_AO2_ON)] 
    _AO2 ("'AO2' {Feature:{Color:6}}", Float) = 0
    _Ao2Map ("'AO2(G)' {Visualize:{RGB}}", 2D) = "white" {} 
    _Ao2MapUV ("UV Set", Float) = 1
    _Ao2Occlusion ("'Occlusion Strength' {Min:0, Max:1}", Float) = 1
    
    // Detail
    [Toggle(_DETAIL_MULX2)] 
    _DetailT ("'Detail' {Feature:{Color:7}}", Float) = 0

    [Toggle(_NORMALMAP)]
    _DetailMaskSource ("'Mask Source' {Dropdown:{TextureAlpha:{}, VertexColorAlpha:{_DetailMask}}}", Float) = 0
    _DetailMask ("'ColorMap(RGB)' {Visualize:{RGB}, Parent:_MainTex}", 2D) = "white" {}
    //_DetailMaskStrength ("'Mask Strength' {Min:0, Max:1}", Float) = 1
	_DetailWeight("'Weight' {Min:0, Max:1}", Float) = 1
    [Enum(Mul, 0, MulX2, 1)] 
    _DetailMode ("'Color Mode' {Dropdown:{Mul:{}, MulX2:{}}}", Float) = 0
    _DetailAlbedoMap ("'Color(RGB) Red' {Visualize:{RGB}}", 2D) = "white" {}
    _DetailAlbedoMapSpin ("Spin", Float) = 0 
    _DetailAlbedoMapUV ("UV Set", Float) = 0
    _DetailNormalMap ("'Normals Red' {Visualize:{NRM}, Parent:_DetailAlbedoMap}", 2D) = "bump" {}
	_DetailNormalMapScale("'Normal Strength' {Min:0, Max:1}", Float) = 1

       
    
    // Team Color
    [Toggle(_TEAMCOLOR_ON)] 
    _TeamColor ("'Team Color' {Feature:{Color:8}}", Float) = 0
    [Enum(Masks, 0, Tint, 1)]
    _TeamColorMasksAsTint ("'Texture Mode' {Dropdown:{Masks:{}, Tint:{_TeamColorMasks, _TeamColor0, _TeamColor1, _TeamColor2, _TeamColor3}}}", Float) = 0
    _TeamColorMaskMap ("'Masks(RGBA)' {Visualize:{R, G, B, A, RGB}, Parent:_MainTex}", 2D) = "black" {}
    _TeamColorMasks ("'Channels' {Vector:Channels}", Vector) = (1,1,1,0)
    _TeamColor0 ("'Tint R' {}", Color) = (1,0,0)
    _TeamColor1 ("'Tint G' {}", Color) = (0,1,0)
    _TeamColor2 ("'Tint B' {}", Color) = (0,0,1)
    _TeamColor3 ("'Tint A' {}", Color) = (0.5,0.5,0.5)
    
    // Decal
    [Toggle(_DECAL_ON)] 
    _Decal ("'Decal' {Feature:{Color:9}}", Float) = 0	
    _DecalColor ("'Tint' {}", Color) = (1,1,1,1)
    _DecalTex ("'Base Color(RGB) Opacity(A)' {Visualize:{RGB, A}}", 2D) = "black" {} 
    _DecalTexUV ("UV Set", Float) = 0
    _DecalWeight ("'Weight' {Min:0, Max:1}", Float) = 1
    _DecalSpecularity ("'Specularity' {Min:0, Max:1}", Float) = 0.5
    _DecalAlphaVertexTint ("'Vertex Alpha Tint' {Min:0, Max:1}", Float) = 0

    // Emission 
    	[Toggle(_EMISSION)]
	_Emission("'Emission' {Feature:{Color:10}}", Float) = 0
		[LM_Emission]
	[HDR]
	_EmissionColor("'Tint' {}", Color) = (1, 1, 1)
		[LM_Emission]
	_EmissionMap("'Mask(RGB)' {Visualize:{RGB}, Parent:_MainTex}", 2D) = "white" {}
	_IncandescenceMap("'Effect(RGB)' {Visualize:{RGB}}", 2D) = "white" {}
	_IncandescenceMapVelocity("Scroll", Vector) = (0, 0, 0, 0)
		_IncandescenceMapUV("UV Set", Float) = 0
		[Gamma]
	_EmissionWeight("'Weight' {Min:0, Max:1}", Float) = 1
		[Toggle(_EMISSION_TOD_TOGGLE)]
	_EmissionTodToggle("'Time Of Day Environment' {Toggle:{On:{}, Off:{}}}", Float) = 1.0
		_EmissionTodFrequency("'Frequency' {Min:0.1, Max:5}", Float) = 1
		_EmissionTodSize("'Size' {Min:0, Max:2}", Float) = 0.25
		_EmissionTodMinimum("'Minimum' {Min:0, Max:2}", Float) = 0.65
		_EmissionTodScale("'Scale' {Min:0.1, Max:20}", Float) = 5


    // Rim Emission 
    [Toggle(_RIM_ON)] 
    _Rim ("'Rim Emission' {Feature:{Color:11}}", Float) = 0
    [HDR]
    _RimColor ("'Tint' {}", Color) = (1,1,1)
    _RimTex ("'Effect(RGB)' {Visualize:{RGB}}", 2D) = "white" {}
    _RimTexVelocity ("Scroll", Vector) = (0,0,0,0) 
    _RimTexUV ("UV Set", Float) = 0
    [Gamma]
    _RimWeight ("'Weight' {Min:0, Max:1}", Float) = 1
    [Gamma]
    _RimBias ("'Fill' {Min:0, Max:1}", Float) = 0
    _RimPower ("'Falloff' {Min:0.01}", Float) = 4

    // Dissolve 
    [Toggle(_DISSOLVE_ON)] 
    _Dissolve ("'Dissolve' {Feature:{Color:12}}", Float) = 0
    [HDR]
    _DissolveGlowColor ("'Glow Tint' {}", Color) = (1,1,1,1)
    _DissolveTex ("'Glow Color(RGB) Opacity(A)' {Visualize:{RGB, A}}", 2D) = "white" {} 
    _DissolveTexUV ("UV Set", Float) = 0
    _DissolveCutoff ("'Cutoff' {Min:0, Max:1}", Float) = 0
    [Gamma]
    _DissolveGlowWeight ("'Glow Weight' {Min:0, Max:1}", Float) = 1
    _DissolveEdgeWidth ("'Glow Width' {Min:0, Max:1}", Float) = 0.01
    
    // Wetness
    [Toggle(_WETNESS_ON)]
    _WetnessProperties ("'Wetness' {Feature:{Color:13}}", Float) = 0
    [Toggle(_WETMASKSOURCE_VERTEXCOLORALPHA)]
    _WetMaskSource ("'Mask Source' {Dropdown:{TextureAlpha:{}, VertexColorAlpha:{_WetMask}}}", Float) = 0
    _WetMask ("'Mask(A)' {Visualize:{A}}", 2D) = "white" {}
    _WetMaskVelocity ("Scroll", Vector) = (0,0,0,0)
    _WetMaskUV ("UV Set", Float) = 0
    _WetMaskStrength ("'Mask Strength' {Min:0, Max:1}", Float) = 1
    _WetTint ("'Tint' {}", Color) = (1,1,1,1)
    _WetNormalMap ("'Normals' {Visualize:{NRM}}", 2D) = "bump" {}
    _WetNormalMapVelocity ("Scroll", Vector) = (0,0,0,0)
    _WetNormalMapUV ("UV Set", Float) = 0
    _WetWeight ("'Weight' {Min:0, Max:1}", Float) = 1
    _WetRoughness ("'Roughness' {Min:0, Max:1}", Float) = 0.2
    _WetNormalMapScale ("'Normal Strength' {}", Float) = 1

    // Directional Blend
    _DirectionalBlendProperties ("'Directional Blend' {Section:{Color:14}}", Float) = 0
    [Toggle(_DIRECTIONALBLENDMODE_OBJECT)]
    _DirectionalBlendMode ("'Mode' {Dropdown:{World:{}, Object:{}}}", Float) = 0
	[PerRendererData]_DirectionalBlendDirection ("'Direction' {Vector:Euler}", Vector) = (0,1,0,0)
    _DirectionalBlendDirectionEulerUI ("'Rotation' {Vector:3}", Vector) = (0,0,0,0)
	[PerRendererData]_OrientedScale ("'Weight' {Min:0, Max:1}", Float) = 0
	[PerRendererData]_OrientedCutoff ("'Cutoff' {Min:0, Max:1}", Float) = 0.25
	[PerRendererData]_OrientedBlend ("'Blend' {Min:0.0001, Max:1}", Float) = 0.1
	[PerRendererData]_DirectionalBlendAlphaVertexTint ("'Vertex Alpha Tint' {Min:0, Max:1}", Float) = 0
    
	[Toggle(_COATING_MASK)]
	_COATING_MASK("'Use Coating Mask' {Toggle:{On:{}, Off:{}}}", Float) = 1.0

	[PerRendererData]_CoatingMask("'Coating Mask' {}", 2D) = "white" {}
	[PerRendererData]_CoatingMaskPower("'Coating Mask Power' {}", Float) = 1
	[PerRendererData]_CoatingMaskSpin("Spin", Float) = 0
	_CoatingMaskUV("UV Set", Float) = 0


    // Secondary Textures 
    _SecondaryTextures ("'Secondary Textures' {Section:{Color:15}}", Float) = 0
    _Color2 ("'Tint' {}", Color) = (1,1,1,1)
	[PerRendererData]_SplatsAlbedo("'Albedo' {Visualize:{RGB}}", 2DArray) = "white" {}
	[PerRendererData]_SplatsNormal("'NormalHeight' {Visualize:{RGB}}", 2DArray) = "white" {}
	[PerRendererData]_MainTex2 ("'Base Color(RGB) Opacity(A)' {Visualize:{RGB, A}}", 2D) = "white" {}
    _MainTex2Velocity ("Scroll", Vector) = (0,0,0,0) 
    _MainTex2UV ("UV Set", Float) = 0
    _MaterialMap2 ("'Metal(R) AO(G) Spec(B) Rough(A)' {Visualize:{R, G, B, A}, Parent:_MainTex2}", 2D) = "white" {}
	[PerRendererData]_BumpMap2 ("'Normals' {Visualize:{NRM}, Parent:_MainTex2}", 2D) = "bump" {}
    _BaseColorVertexTint2 ("'Vertex Color Tint' {Min:0, Max:1}", Float) = 0
    

    // Secondary Properties 
    _SecondaryPhysicalProperties ("'Secondary Properties' {Section:{Color:16}}", Float) = 0
	[PerRendererData]_Metallic2 ("'Metallic' {Min:0, Max:1}", Float) = 0
	[PerRendererData]_Specularity2 ("'Specularity' {Min:0, Max:1}", Float) = 0
	[PerRendererData] _SpecularTint2 ("'Specular Tint' {Min:0, Max:1}", Float) = 0
	[PerRendererData]_Roughness2 ("'Roughness' {Min:0, Max:1}", Float) = 1
    _Occlusion2 ("'Occlusion Strength' {Min:0, Max:1}", Float) = 1
    _BumpScale2 ("'Normal Strength' {}", Float) = 1

	

    // Forward Rendering Options
    _ForwardRenderingOptions ("'Forward Rendering Options' {Section:{Color:19}}", Float) = 0
    [ToggleOff] 
    _SpecularHighlights ("'Specular Highlights' {Toggle:{On:{}, Off:{}}}", Float) = 1.0
    [ToggleOff] 
    _GlossyReflections ("'Glossy Reflections' {Toggle:{On:{}, Off:{}}}", Float) = 1.0

    // Advanced Options
    _AdvancedOptions ("'Advanced Options' {Section:{Color:20}}", Float) = 0
    _Lightmapping ("'GI' {LightmapEmissionProperty:{}}", Float) = 1
    _RenderQueue ("'Render Queue' {RenderQueue:{}}", Float) = 0
    _EnableInstancing ("'Enable Instancing' {EnableInstancing:{}}", Float) = 0

	[PerRendererData]_TexBrightness("Tex Brightness", Range(0, 2)) = 1.0
	[PerRendererData]_TexSaturate("Tex Saturate", Range(0, 2)) = 1.0
	[PerRendererData]_OcclusionStrength("Occlusion Strength", Range(0, 20)) = 0

}

SubShader{
	Tags {
		"RenderType" = "Opaque"
		"PerformanceChecks" = "False"
	//"DisableBatching" = "LODFading"
}
LOD 300

Pass {
	Name "FORWARD"
	Tags { "LightMode" = "ForwardBase" }

	Blend[_SrcBlend][_DstBlend]
	ZWrite[_ZWrite]

	CGPROGRAM
	#pragma target 3.0
	#pragma exclude_renderers gles

	#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
	#pragma shader_feature EFFECT_BUMP
	#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
	#pragma shader_feature _PARALLAXMAP
	#pragma shader_feature _BUMPMODE_POM
	#pragma shader_feature _AO2_ON
	#pragma shader_feature _DETAIL_MULX2
	#pragma shader_feature _NORMALMAP
	#pragma shader_feature _TEAMCOLOR_ON
	#pragma shader_feature _DECAL_ON
	#pragma shader_feature _EMISSION
	#pragma shader_feature _RIM_ON
	#pragma shader_feature _DISSOLVE_ON
	#pragma shader_feature _WETNESS_ON
	#pragma shader_feature _WETMASKSOURCE_VERTEXCOLORALPHA
	#pragma shader_feature _DIRECTIONALBLENDMODE_OBJECT
	#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
	#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF

        //#pragma multi_compile __ LOD_FADE_CROSSFADE
        #pragma multi_compile_fwdbase
        #pragma multi_compile_fog
        #pragma multi_compile_instancing
        //#pragma multi_compile __ VTRANSPARENCY_ON
            
        #pragma vertex aMainVertexShader
        #pragma fragment aMainFragmentShader
        
        #define UNITY_PASS_FORWARDBASE
        
		#include "Assets/Src/TerrainBaker/Shaders/TerrainAtlas.cginc"
        #include "Assets/Alloy/Shaders/Definition/DirectionalBlendColony.cginc"
        #include "Assets/Alloy/Shaders/Forward/Base.cginc"

        ENDCG
    }
    
    Pass {
        Name "FORWARD_DELTA"
        Tags { "LightMode" = "ForwardAdd" }
        
        Blend [_SrcBlend] One
        ZWrite Off

        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers gles
        
        #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
        #pragma shader_feature EFFECT_BUMP
        #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        #pragma shader_feature _PARALLAXMAP
        #pragma shader_feature _BUMPMODE_POM
        #pragma shader_feature _AO2_ON
        #pragma shader_feature _DETAIL_MULX2
        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _TEAMCOLOR_ON
        #pragma shader_feature _DECAL_ON
        #pragma shader_feature _DISSOLVE_ON
        #pragma shader_feature _WETNESS_ON
        #pragma shader_feature _WETMASKSOURCE_VERTEXCOLORALPHA
        #pragma shader_feature _DIRECTIONALBLENDMODE_OBJECT
        #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
        //#pragma multi_compile __ LOD_FADE_CROSSFADE
        #pragma multi_compile_fwdadd_fullshadows
        #pragma multi_compile_fog
        //#pragma multi_compile __ VTRANSPARENCY_ON
        
        #pragma vertex aMainVertexShader
        #pragma fragment aMainFragmentShader

        #define UNITY_PASS_FORWARDADD

		#include "Assets/Src/TerrainBaker/Shaders/TerrainAtlas.cginc"
        #include "Assets/Alloy/Shaders/Definition/DirectionalBlendColony.cginc"
        #include "Assets/Alloy/Shaders/Forward/Add.cginc"

        ENDCG
    }
    
	Pass{
		Name "SHADOWCASTER"
		Tags { "LightMode" = "ShadowCaster" }

		CGPROGRAM
		#pragma target 3.0
		#pragma exclude_renderers gles

		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		#pragma shader_feature _DISSOLVE_ON

		#pragma multi_compile_shadowcaster
		#pragma multi_compile_instancing

		#pragma vertex aMainVertexShader
		#pragma fragment aMainFragmentShader

		#define UNITY_PASS_SHADOWCASTER


		#include "Assets/Src/TerrainBaker/Shaders/TerrainAtlas.cginc"
		#include "Assets/Alloy/Shaders/Definition/DirectionalBlendColony.cginc"
		#include "Assets/Alloy/Shaders/Forward/Shadow.cginc"

		ENDCG
	}

		Pass{
			Name "DEFERRED"
			Tags { "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma target 3.0
			#pragma exclude_renderers nomrt gles

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature EFFECT_BUMP
			#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _PARALLAXMAP
			#pragma shader_feature _BUMPMODE_POM
			#pragma shader_feature _AO2_ON
			#pragma shader_feature _DETAIL_MULX2
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _TEAMCOLOR_ON
			#pragma shader_feature _DECAL_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _RIM_ON
			#pragma shader_feature _DISSOLVE_ON
			#pragma shader_feature _WETNESS_ON
			#pragma shader_feature _WETMASKSOURCE_VERTEXCOLORALPHA
			#pragma shader_feature _DIRECTIONALBLENDMODE_OBJECT
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			#pragma shader_feature _EMISSION_TOD_TOGGLE
			#pragma shader_feature _COATING_MASK
			

		//#pragma multi_compile __ LOD_FADE_CROSSFADE
		#pragma multi_compile_prepassfinal
		#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
		#pragma multi_compile_instancing

		#pragma vertex aMainVertexShader
		#pragma fragment aMainFragmentShader

		#define UNITY_PASS_DEFERRED
#define EFFECT_BUMP




		#include "Assets/Src/TerrainBaker/Shaders/TerrainAtlas.cginc"
        #include "Assets/Alloy/Shaders/Definition/DirectionalBlendColony.cginc"
        #include "Assets/Alloy/Shaders/Forward/Gbuffer.cginc"

        ENDCG
    }
    
    Pass {
        Name "Meta"
        Tags { "LightMode" = "Meta" }

        Cull Off



		CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers nomrt gles
        
        #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        #pragma shader_feature _DETAIL_MULX2
        #pragma shader_feature _NORMALMAP
        #pragma shader_feature _TEAMCOLOR_ON
        #pragma shader_feature _DECAL_ON
        #pragma shader_feature _EMISSION
        #pragma shader_feature _DIRECTIONALBLENDMODE_OBJECT
        
        #pragma vertex aMainVertexShader
        #pragma fragment aMainFragmentShader
        
        #define UNITY_PASS_META
        

		#include "Assets/Src/TerrainBaker/Shaders/TerrainAtlas.cginc"
        #include "Assets/Alloy/Shaders/Definition/DirectionalBlendColony.cginc"
        #include "Assets/Alloy/Shaders/Forward/Meta.cginc"

        ENDCG
    }
}

FallBack "VertexLit"
CustomEditor "AlloyFieldBasedEditor"
}
