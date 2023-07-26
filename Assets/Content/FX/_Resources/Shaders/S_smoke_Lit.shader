// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "S_smoke(soft)_Lit"
{
	Properties
	{
		_mainTex("mainTex", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_ColorMult("ColorMult", Float) = 0
		_mainNormal("mainNormal", 2D) = "bump" {}
		_mask("mask", 2D) = "white" {}
		_deph("deph", Float) = 0
		_Brightness("Brightness", Float) = 0.8608994
		_lightWrap("lightWrap", Float) = 0.8
		_normalPower("normalPower", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float3 worldPos;
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _lightWrap;
		uniform sampler2D _mainNormal;
		uniform float4 _mainNormal_ST;
		uniform float _normalPower;
		uniform sampler2D _mainTex;
		uniform float4 _mainTex_ST;
		uniform float _ColorMult;
		uniform float4 _Color;
		uniform sampler2D _mask;
		uniform float4 _mask_ST;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _deph;
		uniform float _Brightness;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv0_mainTex = i.uv_texcoord * _mainTex_ST.xy + _mainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _mainTex, ( ( uv0_mainTex + i.uv2_texcoord2 ) * float2( 0.5,0.5 ) ) );
			float2 uv_mask = i.uv_texcoord * _mask_ST.xy + _mask_ST.zw;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth63 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float distanceDepth63 = abs( ( screenDepth63 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _deph ) );
			float clampResult102 = clamp( distanceDepth63 , 0.0 , 1.0 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float grayscale121 = Luminance(ase_lightColor.rgb);
			c.rgb = ( (0.0 + (grayscale121 - 0.0) * (_Brightness - 0.0) / (1.0 - 0.0)) * i.vertexColor ).rgb;
			c.a = ( i.vertexColor.a * tex2DNode1.a * tex2D( _mask, uv_mask ).a * clampResult102 * _Color.a );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float3 LightWrapVector47_g1 = (( _lightWrap * 0.5 )).xxx;
			float2 uv0_mainNormal = i.uv_texcoord * _mainNormal_ST.xy + _mainNormal_ST.zw;
			float3 lerpResult111 = lerp( float3(0,0,1) , UnpackNormal( tex2D( _mainNormal, ( ( uv0_mainNormal + i.uv2_texcoord2 ) * float2( 0.5,0.5 ) ) ) ) , _normalPower);
			float3 CurrentNormal23_g1 = normalize( (WorldNormalVector( i , lerpResult111 )) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult20_g1 = dot( CurrentNormal23_g1 , ase_worldlightDir );
			float NDotL21_g1 = dotResult20_g1;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 AttenuationColor8_g1 = ( ase_lightColor.rgb * 1 );
			float2 uv0_mainTex = i.uv_texcoord * _mainTex_ST.xy + _mainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _mainTex, ( ( uv0_mainTex + i.uv2_texcoord2 ) * float2( 0.5,0.5 ) ) );
			float3 DiffuseColor70_g1 = ( ( ( max( ( LightWrapVector47_g1 + ( ( 1.0 - LightWrapVector47_g1 ) * NDotL21_g1 ) ) , float3(0,0,0) ) * AttenuationColor8_g1 ) + (UNITY_LIGHTMODEL_AMBIENT).rgb ) * ( tex2DNode1 * ( i.vertexColor * _ColorMult * _Color ) ).rgb );
			float3 normalizeResult77_g1 = normalize( _WorldSpaceLightPos0.xyz );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult28_g1 = normalize( ( normalizeResult77_g1 + ase_worldViewDir ) );
			float3 HalfDirection29_g1 = normalizeResult28_g1;
			float dotResult32_g1 = dot( HalfDirection29_g1 , CurrentNormal23_g1 );
			float SpecularPower14_g1 = exp2( ( ( 0.0 * 10.0 ) + 1.0 ) );
			float3 specularFinalColor42_g1 = ( AttenuationColor8_g1 * pow( max( dotResult32_g1 , 0.0 ) , SpecularPower14_g1 ) * 0.0 );
			o.Albedo = ( DiffuseColor70_g1 + specularFinalColor42_g1 );
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers xbox360 xboxone ps4 psp2 n3ds wiiu 
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows exclude_path:deferred nofog 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
2288;348;1206;780;-1324.889;211.1378;1.316444;True;True
Node;AmplifyShaderEditor.TexCoordVertexDataNode;28;-1353.804,196.8721;Float;False;1;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1269.778,-218.7773;Float;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;54;-1002.316,94.22112;Float;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-921.3942,-213.7174;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-692.5288,174.9688;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-506.7271,280.555;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-750.9876,-213.6145;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-123.9523,169.5113;Float;False;Property;_deph;deph;5;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;405.7859,-280.1689;Float;False;Property;_ColorMult;ColorMult;2;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;90;77.63265,-333.0063;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0.2509804;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;11;327.1506,-163.6393;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;120;1319.607,240.2454;Float;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;122;1601.614,420.8504;Float;False;Property;_Brightness;Brightness;6;0;Create;True;0;0;False;0;0.8608994;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;121;1604.379,160.539;Float;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;721.8347,576.5275;Float;False;Property;_normalPower;normalPower;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;63;67.5987,151.1027;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;112;771.4284,79.13055;Float;False;Constant;_Vector2;Vector 2;7;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;5;555.2131,251.8806;Float;True;Property;_mainNormal;mainNormal;3;0;Create;True;0;0;False;0;None;a4352d29f1cfaa449a5ab76a0edb8a86;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;880.0055,-254.6899;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-478.5158,-399.9689;Float;True;Property;_mainTex;mainTex;0;0;Create;True;0;0;False;0;None;d4c61bf94e3a861448f4ef35c582aa05;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;109;1124.823,-81.44064;Float;False;Property;_lightWrap;lightWrap;7;0;Create;True;0;0;False;0;0.8;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;124;1785.001,154.6114;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;2.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;1303.794,-481.8289;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;110;1158.596,-209.8925;Float;False;Constant;_Float5;Float 5;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;102;343.0923,158.167;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;111;975.6384,293.5509;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;2;-251.3155,-65.70755;Float;True;Property;_mask;mask;4;0;Create;True;0;0;False;0;None;04ce4faefbf4daa4cb4c507500639600;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;108;1691.454,-403.0505;Float;False;BlinnPhongLightWrap;-1;;1;139fed909c1bc1a42a96c42d8cf09006;0;5;1;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;44;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;119;2153.218,-303.5781;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;629.4376,-37.42428;Float;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;118;2101.218,-403.5781;Float;False;unity_AmbientGround;0;1;COLOR;0
Node;AmplifyShaderEditor.FogParamsNode;117;2076.739,-537.4137;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;2041.699,219.5353;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;94;2151.981,-199.608;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;S_smoke(soft)_Lit;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;False;False;False;False;False;False;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;24;0
WireConnection;53;1;28;0
WireConnection;55;0;54;0
WireConnection;55;1;28;0
WireConnection;106;0;55;0
WireConnection;105;0;53;0
WireConnection;121;0;120;0
WireConnection;63;0;64;0
WireConnection;5;1;106;0
WireConnection;92;0;11;0
WireConnection;92;1;91;0
WireConnection;92;2;90;0
WireConnection;1;1;105;0
WireConnection;124;0;121;0
WireConnection;124;4;122;0
WireConnection;3;0;1;0
WireConnection;3;1;92;0
WireConnection;102;0;63;0
WireConnection;111;0;112;0
WireConnection;111;1;5;0
WireConnection;111;2;113;0
WireConnection;108;1;3;0
WireConnection;108;4;111;0
WireConnection;108;2;110;0
WireConnection;108;3;110;0
WireConnection;108;44;109;0
WireConnection;6;0;11;4
WireConnection;6;1;1;4
WireConnection;6;2;2;4
WireConnection;6;3;102;0
WireConnection;6;4;90;4
WireConnection;125;0;124;0
WireConnection;125;1;11;0
WireConnection;94;0;108;0
WireConnection;94;9;6;0
WireConnection;94;13;125;0
ASEEND*/
//CHKSM=FD9958BDC15C648351F50157C9E2F9E36C7C3617