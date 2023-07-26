// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/S_LikeAlloyEmission"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_SpecTex("SpecTex", 2D) = "white" {}
		_Opacity("Opacity", Range( 0.16 , 0.263)) = 0
		_EdgeSpread("Edge Spread", Range( 0 , 1)) = 0
		_NoiseTile("Noise Tile", Float) = 0
		_UpdownCorrection("UpdownCorrection", Float) = 0
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		_HeightControl("HeightControl", Float) = 0.93
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _EmissionColor;
		uniform float _NoiseTile;
		uniform float _HeightControl;
		uniform float _UpdownCorrection;
		uniform float _EdgeSpread;
		uniform float _Opacity;
		uniform sampler2D _SpecTex;
		uniform float4 _SpecTex_ST;
		uniform float _Cutoff = 0.5;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			o.Normal = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = tex2DNode1.rgb;
			float4 color36 = IsGammaSpace() ? float4(0.8980393,0.4901961,0.3803922,1) : float4(0.783538,0.2050788,0.1195384,1);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float simplePerlin3D31 = snoise( ( ase_vertex3Pos * _NoiseTile ) );
			float4 lerpResult35 = lerp( _EmissionColor , color36 , simplePerlin3D31);
			float temp_output_15_0 = ( ase_vertex3Pos.y - 0.0 );
			float temp_output_13_0 = (-_UpdownCorrection + (_HeightControl - 0.0) * (( _UpdownCorrection + _EdgeSpread ) - -_UpdownCorrection) / (1.0 - 0.0));
			float smoothstepResult17 = smoothstep( temp_output_15_0 , temp_output_13_0 , ( temp_output_13_0 + (0.0 + (_EdgeSpread - 0.0) * (-2.0 - 0.0) / (1.0 - 0.0)) ));
			float4 lerpResult27 = lerp( float4( 0,0,0,0 ) , lerpResult35 , ( saturate( ( ( 1.0 - saturate( ( smoothstepResult17 * step( temp_output_15_0 , temp_output_13_0 ) ) ) ) * (0.96 + (simplePerlin3D31 - -1.0) * (1.94 - 0.96) / (1.0 - -1.0)) ) ) * (0.0 + (_Opacity - 0.16) * (1.0 - 0.0) / (0.263 - 0.16)) ));
			o.Emission = lerpResult27.rgb;
			float2 uv_SpecTex = i.uv_texcoord * _SpecTex_ST.xy + _SpecTex_ST.zw;
			float4 tex2DNode3 = tex2D( _SpecTex, uv_SpecTex );
			o.Metallic = tex2DNode3.r;
			o.Smoothness = tex2DNode3.b;
			o.Occlusion = tex2DNode3.g;
			o.Alpha = tex2DNode1.a;
			clip( tex2DNode1.a - _Cutoff );
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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
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
2065;276;1206;774;-176.5409;-1194.267;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;5;-1400.628,739.3109;Float;False;Property;_UpdownCorrection;UpdownCorrection;7;0;Create;True;0;0;False;0;0;238.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1545.571,882.9371;Float;False;Property;_EdgeSpread;Edge Spread;5;0;Create;True;0;0;False;0;0;0.759;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;10;-1127.094,698.2321;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1082.322,517.5983;Float;False;Property;_HeightControl;HeightControl;9;0;Create;True;0;0;False;0;0.93;0.93;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-1127.917,794.0433;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;12;-950.1019,894.4371;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;-2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;-943.4129,672.108;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;4;-1827.397,1026.636;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;15;-1110.951,1289.422;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-663.6685,904.3762;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;16;-298.8411,1070.302;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;17;-322.8296,749.5405;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-688.5933,1543.564;Float;False;Property;_NoiseTile;Noise Tile;6;0;Create;True;0;0;False;0;0;0.07;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;14.30371,980.5215;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-492.7393,1455.477;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;8;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;26;231.8321,1038.006;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;31;-226.4252,1472.554;Float;True;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;32;118.6828,1510.239;Float;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.96;False;4;FLOAT;1.94;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;361.9397,1053.288;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;503.1776,1264.655;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;453.6415,1653.953;Float;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;False;0;0;0.16;0.16;0.263;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;36;31.5575,771.0636;Float;False;Constant;_Color0;Color 0;9;0;Create;True;0;0;False;0;0.8980393,0.4901961,0.3803922,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;39;674.9318,1299.33;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-36.17312,594.057;Float;False;Property;_EmissionColor;EmissionColor;8;0;Create;True;0;0;False;0;0,0,0,0;0.990566,0.8678646,0.3971608,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;41;832.9916,1650.21;Float;False;5;0;FLOAT;0;False;1;FLOAT;0.16;False;2;FLOAT;0.263;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;35;308.6794,714.7123;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;868.9318,1340.33;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-762.8894,127.287;Float;True;Property;_BumpMap;BumpMap;2;0;Create;True;0;0;False;0;None;e4d117bfeeb50184a9f07fa1ebb1b153;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-689.2654,-92.94455;Float;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;None;aa051584905a90843a2153e00f6a7ce1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-827.6875,337.3469;Float;True;Property;_SpecTex;SpecTex;3;0;Create;True;0;0;False;0;None;46ac2fc630d4a91448d424dc223155be;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;27;665.6174,757.4167;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1060.944,145.1187;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/S_LikeAlloyEmission;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;5;0
WireConnection;8;0;5;0
WireConnection;8;1;6;0
WireConnection;12;0;6;0
WireConnection;13;0;37;0
WireConnection;13;3;10;0
WireConnection;13;4;8;0
WireConnection;15;0;4;2
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;16;0;15;0
WireConnection;16;1;13;0
WireConnection;17;0;14;0
WireConnection;17;1;15;0
WireConnection;17;2;13;0
WireConnection;23;0;17;0
WireConnection;23;1;16;0
WireConnection;30;0;4;0
WireConnection;30;1;29;0
WireConnection;26;0;23;0
WireConnection;31;0;30;0
WireConnection;32;0;31;0
WireConnection;25;0;26;0
WireConnection;34;0;25;0
WireConnection;34;1;32;0
WireConnection;39;0;34;0
WireConnection;41;0;7;0
WireConnection;35;0;28;0
WireConnection;35;1;36;0
WireConnection;35;2;31;0
WireConnection;40;0;39;0
WireConnection;40;1;41;0
WireConnection;27;1;35;0
WireConnection;27;2;40;0
WireConnection;0;0;1;0
WireConnection;0;1;2;0
WireConnection;0;2;27;0
WireConnection;0;3;3;1
WireConnection;0;4;3;3
WireConnection;0;5;3;2
WireConnection;0;9;1;4
WireConnection;0;10;1;4
ASEEND*/
//CHKSM=A4F87E1363F459F9A1E780C458AC8D611DDC1E37