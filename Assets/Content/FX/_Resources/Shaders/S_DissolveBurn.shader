// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Community/DissolveBurn"
{
	Properties
	{
		_DM("DM", 2D) = "white" {}
		_AM("AM", 2D) = "white" {}
		_NM("NM", 2D) = "bump" {}
		_EM("EM", 2D) = "white" {}
		_BurnRamp("Burn Ramp", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0
		_BurnMultColor("BurnMultColor", Float) = 0
		_NOise("NOise", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		ZTest LEqual
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_TEXTURE_PARAMS(textureName) textureName

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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NM;
		uniform float4 _NM_ST;
		uniform sampler2D _DM;
		uniform float4 _DM_ST;
		uniform float _DissolveAmount;
		uniform sampler2D _NOise;
		uniform sampler2D _BurnRamp;
		uniform float _BurnMultColor;
		uniform sampler2D _EM;
		uniform float4 _EM_ST;
		uniform sampler2D _AM;
		uniform float4 _AM_ST;


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NM = i.uv_texcoord * _NM_ST.xy + _NM_ST.zw;
			o.Normal = UnpackNormal( tex2D( _NM, uv_NM ) );
			float2 uv_DM = i.uv_texcoord * _DM_ST.xy + _DM_ST.zw;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar150 = TriplanarSamplingSF( _NOise, ase_worldPos, ase_worldNormal, 1.76, float2( 1,1 ), 1.0, 0 );
			float4 clampResult113 = clamp( (float4( -2,0,0,0 ) + ((float4( 0,0,0,0 ) + (( (0.41 + (( ( 1.0 - (0.0 + (_DissolveAmount - 0.0) * (0.6968135 - 0.0) / (1.0 - 0.0)) ) * (0.5 + (sin( ( _Time.y * 3.0 ) ) - -1.0) * (1.0 - 0.5) / (1.0 - -1.0)) ) - 0.0) * (1.0 - 0.41) / (1.0 - 0.0)) + triplanar150 ) - float4( 0,0,0,0 )) * (float4( 1,0,0,0 ) - float4( 0,0,0,0 )) / (float4( 1,0,0,0 ) - float4( 0,0,0,0 ))) - float4( 0,0,0,0 )) * (float4( 2,0,0,0 ) - float4( -2,0,0,0 )) / (float4( 1,0,0,0 ) - float4( 0,0,0,0 ))) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			float4 temp_output_130_0 = ( 1.0 - clampResult113 );
			float2 appendResult115 = (float2(temp_output_130_0.xy));
			float4 temp_output_126_0 = ( temp_output_130_0 * tex2D( _BurnRamp, appendResult115 ) * _BurnMultColor );
			float4 blendOpSrc144 = tex2D( _DM, uv_DM );
			float4 blendOpDest144 = temp_output_126_0;
			o.Albedo = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc144 ) * ( 1.0 - blendOpDest144 ) ) )).xyz;
			float2 uv_EM = i.uv_texcoord * _EM_ST.xy + _EM_ST.zw;
			o.Emission = ( tex2D( _EM, uv_EM ) + temp_output_126_0 ).rgb;
			float2 uv_AM = i.uv_texcoord * _AM_ST.xy + _AM_ST.zw;
			float4 tex2DNode78 = tex2D( _AM, uv_AM );
			o.Metallic = tex2DNode78.r;
			o.Smoothness = (1.0 + (tex2DNode78.a - 0.0) * (0.0 - 1.0) / (1.0 - 0.0));
			o.Occlusion = tex2DNode78.g;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
1984;258;1206;768;1971.175;-197.7504;1.3;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;155;-2177.963,698.1144;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;158;-2138.239,813.1634;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;-1972.293,758.4981;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1682.119,523.186;Float;False;Property;_DissolveAmount;Dissolve Amount;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;161;-1293.921,523.5358;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.6968135;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;160;-1734.309,756.939;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;128;-967.3727,510.0833;Float;False;908.2314;498.3652;Dissolve - Opacity Mask;4;71;73;111;148;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;159;-1461.091,720.5807;Float;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;71;-965.8284,563.7526;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;151;-1311.943,944.9036;Float;True;Property;_NOise;NOise;7;0;Create;True;0;0;False;0;None;28c7aad1372ff114b90d330f8a2dd938;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-790.2714,747.1235;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;150;-927.1315,988.7056;Float;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1.76;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;111;-607.5305,565.1279;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.41;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-294.9845,637.9298;Float;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TFHCRemapNode;149;112.7375,611.5286;Float;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;129;-892.9326,49.09825;Float;False;814.5701;432.0292;Burn Effect - Emission;7;113;126;115;114;112;130;141;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;112;-878.1525,280.8961;Float;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,0,0,0;False;3;FLOAT4;-2,0,0,0;False;4;FLOAT4;2,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;113;-828.3207,38.2812;Float;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;130;-530.5283,81.27126;Float;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;115;-549.438,307.1016;Float;False;FLOAT2;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-403.6841,193.4202;Float;False;Property;_BurnMultColor;BurnMultColor;6;0;Create;True;0;0;False;0;0;30;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;114;-313.3001,306.7128;Float;True;Property;_BurnRamp;Burn Ramp;4;0;Create;True;0;0;False;0;None;8973279574b2fbd47929d947056d87ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-231.1784,58.53046;Float;False;3;3;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;136;-769.8271,-137.1955;Float;True;Property;_EM;EM;3;0;Create;True;0;0;False;0;704951c273da0ea4c9724d236f4bd02e;a07d1f15654220549b17f93bf412a597;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;134;-406.2162,-267.855;Float;True;Property;_DM;DM;0;0;Create;True;0;0;False;0;655f1ba9a5fb6634f8ac9c869990f654;8f8c18dac1daf494ca38615ec4511113;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-318.9415,-736.8513;Float;True;Property;_AM;AM;1;0;Create;True;0;0;False;0;None;2fd873c5c03130040a31fef9520d2f2b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;132;728.1154,-18.99257;Float;False;765.1592;493.9802;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0;;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendOpsNode;144;145.0268,-152.1926;Float;False;Screen;True;3;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;146;121.3928,19.09554;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;131;-411.1492,-453.2101;Float;True;Property;_NM;NM;2;0;Create;True;0;0;False;0;None;26f315b6e953073408111942455e84e8;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;140;273.7414,-705.1817;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1234.415,27.10748;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASESampleShaders/Community/DissolveBurn;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;157;0;155;0
WireConnection;157;1;158;0
WireConnection;161;0;4;0
WireConnection;160;0;157;0
WireConnection;159;0;160;0
WireConnection;71;0;161;0
WireConnection;148;0;71;0
WireConnection;148;1;159;0
WireConnection;150;0;151;0
WireConnection;111;0;148;0
WireConnection;73;0;111;0
WireConnection;73;1;150;0
WireConnection;149;0;73;0
WireConnection;112;0;149;0
WireConnection;113;0;112;0
WireConnection;130;0;113;0
WireConnection;115;0;130;0
WireConnection;114;1;115;0
WireConnection;126;0;130;0
WireConnection;126;1;114;0
WireConnection;126;2;141;0
WireConnection;144;0;134;0
WireConnection;144;1;126;0
WireConnection;146;0;136;0
WireConnection;146;1;126;0
WireConnection;140;0;78;4
WireConnection;0;0;144;0
WireConnection;0;1;131;0
WireConnection;0;2;146;0
WireConnection;0;3;78;1
WireConnection;0;4;140;0
WireConnection;0;5;78;2
ASEEND*/
//CHKSM=B3F6F5BB7202A407DA624D8FDB8CC25C75432FA7