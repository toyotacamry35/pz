// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "scyphopodWrk"
{
	Properties
	{
		_Main_Texture("Main_Texture", 2D) = "white" {}
		[Normal]_normalMap("normal Map", 2D) = "bump" {}
		_thickness("thickness", Float) = 2.22
		_speed("speed", Float) = 0.4
		_Color("Color", Color) = (0,0,0,0)
		_frenel("frenel", Float) = 0
		_tips_glow("tips_glow", Range( 0 , 10)) = 0
		_stripes_fade("stripes_fade", Range( 0 , 1)) = 0
		_frenel_fade("frenel_fade", Range( 0 , 1)) = 0
		_ScifopodWorker_Mask("ScifopodWorker_Mask", 2D) = "black" {}
		_Noise("Noise", 2D) = "white" {}
		_scyphopodWorker_EM("scyphopodWorker_EM", 2D) = "white" {}
		_scyphopodWorker_AM("scyphopodWorker_AM", 2D) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
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
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv2_texcoord2;
		};

		uniform sampler2D _normalMap;
		uniform float4 _normalMap_ST;
		uniform sampler2D _Main_Texture;
		uniform float4 _Main_Texture_ST;
		uniform float _frenel;
		uniform float4 _Color;
		uniform float _frenel_fade;
		uniform float _speed;
		uniform float _thickness;
		uniform float _stripes_fade;
		uniform sampler2D _ScifopodWorker_Mask;
		uniform float4 _ScifopodWorker_Mask_ST;
		uniform sampler2D _scyphopodWorker_EM;
		uniform float4 _scyphopodWorker_EM_ST;
		uniform sampler2D _Noise;
		uniform float _tips_glow;
		uniform sampler2D _scyphopodWorker_AM;
		uniform float4 _scyphopodWorker_AM_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_normalMap = i.uv_texcoord * _normalMap_ST.xy + _normalMap_ST.zw;
			float3 tex2DNode2 = UnpackNormal( tex2D( _normalMap, uv_normalMap ) );
			o.Normal = tex2DNode2;
			float2 uv_Main_Texture = i.uv_texcoord * _Main_Texture_ST.xy + _Main_Texture_ST.zw;
			o.Albedo = tex2D( _Main_Texture, uv_Main_Texture ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult22 = dot( (WorldNormalVector( i , tex2DNode2 )) , ase_worldViewDir );
			float temp_output_24_0 = pow( ( 1.0 - saturate( dotResult22 ) ) , _frenel );
			float3 temp_output_46_0 = (_Color).rgb;
			float2 uv_ScifopodWorker_Mask = i.uv_texcoord * _ScifopodWorker_Mask_ST.xy + _ScifopodWorker_Mask_ST.zw;
			float4 tex2DNode51 = tex2D( _ScifopodWorker_Mask, uv_ScifopodWorker_Mask );
			float2 uv_scyphopodWorker_EM = i.uv_texcoord * _scyphopodWorker_EM_ST.xy + _scyphopodWorker_EM_ST.zw;
			float2 appendResult59 = (float2(0.01 , -0.13));
			float2 uv_TexCoord57 = i.uv_texcoord + ( _Time.y * appendResult59 );
			float4 lerpResult41 = lerp( ( tex2D( _scyphopodWorker_EM, uv_scyphopodWorker_EM ) * tex2D( _Noise, uv_TexCoord57 ) ) , _Color , saturate( temp_output_24_0 ));
			float4 lerpResult34 = lerp( float4( 0,0,0,0 ) , lerpResult41 , (0.0 + (_tips_glow - 0.0) * (2.61 - 0.0) / (10.0 - 0.0)));
			o.Emission = ( float4( ( temp_output_24_0 * temp_output_46_0 * _Color.a * _frenel_fade ) , 0.0 ) + float4( ( temp_output_46_0 * saturate( ( 1.0 - abs( ( (-1.0 + (frac( ( i.uv2_texcoord2.y + ( _Time.y * _speed ) ) ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * _thickness ) ) ) ) * _Color.a * _stripes_fade * tex2DNode51.g ) , 0.0 ) + ( lerpResult34 * tex2DNode51.r ) ).rgb;
			float2 uv_scyphopodWorker_AM = i.uv_texcoord * _scyphopodWorker_AM_ST.xy + _scyphopodWorker_AM_ST.zw;
			float4 tex2DNode69 = tex2D( _scyphopodWorker_AM, uv_scyphopodWorker_AM );
			o.Metallic = tex2DNode69.r;
			o.Smoothness = ( 1.0 - tex2DNode69.a );
			o.Occlusion = tex2DNode69.g;
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
				float4 customPack1 : TEXCOORD1;
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
				o.customPack1.zw = customInputData.uv2_texcoord2;
				o.customPack1.zw = v.texcoord1;
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
				surfIN.uv2_texcoord2 = IN.customPack1.zw;
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
Version=16301
2053;137;1206;790;437.7099;646.0405;1.3;True;False
Node;AmplifyShaderEditor.RangedFloatNode;13;-1784.985,648.8639;Float;False;Property;_speed;speed;3;0;Create;True;0;0;False;0;0.4;3.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;12;-1789.311,542.9046;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2858.029,-533.504;Float;True;Property;_normalMap;normal Map;1;1;[Normal];Create;True;0;0;False;0;None;4bf22868da80e8341bb7562469d638a8;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;21;-2544.626,-402.2857;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;23;-2604.109,-210.0764;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1821.186,348.8658;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1587.985,571.8639;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-1927.365,160.8425;Float;False;FLOAT2;4;0;FLOAT;0.01;False;1;FLOAT;-0.13;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;56;-2023.417,55.94766;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;22;-2331.413,-284.8996;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-1448.985,485.8638;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1760.365,113.8425;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;29;-2210.646,-283.4802;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;16;-1294.946,494.582;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-2617.123,-15.66651;Float;False;Property;_frenel;frenel;5;0;Create;True;0;0;False;0;0;0.91;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;4;-1093.584,457.2057;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;30;-2064.645,-284.4802;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;57;-1600.365,-31.15754;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1005.608,716.4033;Float;False;Property;_thickness;thickness;2;0;Create;True;0;0;False;0;2.22;1.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;24;-1891.239,-280.3016;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;60;-1382.808,2.115469;Float;True;Property;_Noise;Noise;10;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-1658.607,-417.1736;Float;True;Property;_scyphopodWorker_EM;scyphopodWorker_EM;11;0;Create;True;0;0;False;0;a711220635fae574e9efe94d856ffb5b;a711220635fae574e9efe94d856ffb5b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-822.5026,691.0704;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;4.72;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;17;-1225.662,208.2216;Float;False;Property;_Color;Color;4;0;Create;True;0;0;False;0;0,0,0,0;0.01401744,0.1946838,0.5943396,0.3647059;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-1080.605,-270.681;Float;False;Property;_tips_glow;tips_glow;6;0;Create;True;0;0;False;0;0;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1048.21,-50.77548;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;53;-1190.446,-152.8358;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;5;-775.5026,458.0704;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;41;-824.2816,-79.1317;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;63;-802.3855,-264.0377;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;0;False;4;FLOAT;2.61;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;6;-579.5026,458.0704;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;34;-575.0386,-199.6117;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;10;-419.5026,461.0704;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-312.8635,756.1749;Float;False;Property;_stripes_fade;stripes_fade;7;0;Create;True;0;0;False;0;0;0.646;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;46;-829.4962,220.1078;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;51;-846.0011,-538.6943;Float;True;Property;_ScifopodWorker_Mask;ScifopodWorker_Mask;9;0;Create;True;0;0;False;0;84dea49eb6cdb6e40afcb3ee7e2b0d1b;84dea49eb6cdb6e40afcb3ee7e2b0d1b;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-756.1985,320.9414;Float;False;Property;_frenel_fade;frenel_fade;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;69;118.6901,-409.4403;Float;True;Property;_scyphopodWorker_AM;scyphopodWorker_AM;12;0;Create;True;0;0;False;0;25d23fd104303cb42bf0403902aaa537;25d23fd104303cb42bf0403902aaa537;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-420.9006,127.7075;Float;True;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-88.56606,415.9888;Float;True;5;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-214.7465,-202.1885;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;184.7919,-40.15545;Float;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1097.091,-696.8396;Float;True;Property;_Main_Texture;Main_Texture;0;0;Create;True;0;0;False;0;None;bd5e4e46e3fb2fd4eb70d44855ef5f02;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;70;418.9901,-114.3405;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;668.3201,-204.7256;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;scyphopodWrk;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;2;0
WireConnection;14;0;12;0
WireConnection;14;1;13;0
WireConnection;22;0;21;0
WireConnection;22;1;23;0
WireConnection;15;0;3;2
WireConnection;15;1;14;0
WireConnection;58;0;56;0
WireConnection;58;1;59;0
WireConnection;29;0;22;0
WireConnection;16;0;15;0
WireConnection;4;0;16;0
WireConnection;30;0;29;0
WireConnection;57;1;58;0
WireConnection;24;0;30;0
WireConnection;24;1;25;0
WireConnection;60;1;57;0
WireConnection;9;0;4;0
WireConnection;9;1;11;0
WireConnection;61;0;66;0
WireConnection;61;1;60;0
WireConnection;53;0;24;0
WireConnection;5;0;9;0
WireConnection;41;0;61;0
WireConnection;41;1;17;0
WireConnection;41;2;53;0
WireConnection;63;0;33;0
WireConnection;6;0;5;0
WireConnection;34;1;41;0
WireConnection;34;2;63;0
WireConnection;10;0;6;0
WireConnection;46;0;17;0
WireConnection;44;0;24;0
WireConnection;44;1;46;0
WireConnection;44;2;17;4
WireConnection;44;3;48;0
WireConnection;18;0;46;0
WireConnection;18;1;10;0
WireConnection;18;2;17;4
WireConnection;18;3;47;0
WireConnection;18;4;51;2
WireConnection;35;0;34;0
WireConnection;35;1;51;1
WireConnection;27;0;44;0
WireConnection;27;1;18;0
WireConnection;27;2;35;0
WireConnection;70;0;69;4
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;2;27;0
WireConnection;0;3;69;1
WireConnection;0;4;70;0
WireConnection;0;5;69;2
ASEEND*/
//CHKSM=429F19E9B26A336F337FDDEC52F71E8E780854A3