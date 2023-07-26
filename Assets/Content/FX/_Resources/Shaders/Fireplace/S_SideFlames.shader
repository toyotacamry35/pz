// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/FirePlace/S_SideFlames"
{
	Properties
	{
		_noise1Tiling("noise1Tiling", Range( 0 , 15)) = 0
		_noise2Tiling("noise2Tiling", Range( 0 , 15)) = 0
		_noise1PanX("noise1PanX", Range( -2 , 2)) = 0
		_noise1PanY("noise1PanY", Range( -2 , 2)) = 0
		_noise2PanX("noise2PanX", Range( -2 , 2)) = 0
		_noise2PanY("noise2PanY", Range( -2 , 2)) = 0
		_noiseTex1("noiseTex1", 2D) = "white" {}
		_noiseTex2("noiseTex2", 2D) = "white" {}
		_noise1Mix("noise1Mix", Range( 0 , 3)) = 0
		[Toggle(_DYNAMICNOISE1MIXUV2X_ON)] _dynamicNoise1MixUV2X("dynamicNoise1Mix(UV2X)", Float) = 0
		[Toggle(_DYNAMICNOISE2MIXUV1W_ON)] _dynamicNoise2MixUV1W("dynamicNoise2Mix(UV1W)", Float) = 0
		[Toggle(_USEDYNAMICDISTUV1Z_ON)] _useDynamicDistUV1Z("useDynamicDist(UV1Z)", Float) = 0
		_noise2Mix("noise2Mix", Range( 0 , 3)) = 0
		_uvDistGradMaskPow("uvDistGradMaskPow", Range( 0 , 4)) = 1.069
		_uvDistIntensity("uvDistIntensity", Range( -5 , 5)) = 0
		_ColorEdge("ColorEdge", Color) = (0.5,0.5,0.5,1)
		_ColorBase("ColorBase", Color) = (0.5,0.5,0.5,1)
		_MainTex("MainTex", 2D) = "white" {}
		_edgeColorMultiplier("edgeColorMultiplier", Range( 0 , 3)) = 0
		_baseColorMultiplier("baseColorMultiplier", Range( 0 , 3)) = 0
		_finalEmissive("finalEmissive", Range( 0 , 20)) = 0
		_distance("distance", Range( 0 , 1)) = 0
		_finalOpacity("finalOpacity", Range( 0 , 2)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha One
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM



#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
		//only defining to not throw compilation error over Unity 5.5
		#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#pragma shader_feature _DYNAMICNOISE1MIXUV2X_ON
			#pragma shader_feature _DYNAMICNOISE2MIXUV1W_ON
			#pragma shader_feature _USEDYNAMICDISTUV1Z_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
			};

			uniform float4 _ColorEdge;
			uniform float _edgeColorMultiplier;
			uniform sampler2D _MainTex;
			uniform sampler2D _noiseTex1;
			uniform float _noise1Tiling;
			uniform float _noise1PanX;
			uniform float _noise1PanY;
			uniform float _noise1Mix;
			uniform sampler2D _noiseTex2;
			uniform float _noise2PanX;
			uniform float _noise2PanY;
			uniform float _noise2Tiling;
			uniform float _noise2Mix;
			uniform float _uvDistGradMaskPow;
			uniform float4 _MainTex_ST;
			uniform float _uvDistIntensity;
			uniform float4 _ColorBase;
			uniform float _baseColorMultiplier;
			uniform float _finalEmissive;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _distance;
			uniform float _finalOpacity;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord2;
				o.ase_color = v.color;
				float3 vertexValue =  float3(0,0,0) ;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				float2 appendResult8 = (float2(_noise1PanX , _noise1PanY));
				float2 temp_output_15_0 = ( float2( 3,8 ) * i.ase_texcoord1.xy );
				#ifdef _DYNAMICNOISE1MIXUV2X_ON
				float staticSwitch30 = i.ase_texcoord2.x;
				#else
				float staticSwitch30 = 1.0;
				#endif
				float2 appendResult18 = (float2(_noise2PanX , _noise2PanY));
				#ifdef _DYNAMICNOISE2MIXUV1W_ON
				float staticSwitch32 = i.ase_texcoord1.w;
				#else
				float staticSwitch32 = _noise2Mix;
				#endif
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				#ifdef _USEDYNAMICDISTUV1Z_ON
				float staticSwitch53 = i.ase_texcoord1.z;
				#else
				float staticSwitch53 = _uvDistIntensity;
				#endif
				float2 appendResult59 = (float2(i.ase_texcoord.x , ( i.ase_texcoord.y + ( ( ( i.ase_texcoord1.x + ( ( tex2D( _noiseTex1, ( ( _noise1Tiling * i.ase_texcoord.xy ) + ( _Time.y * appendResult8 ) + temp_output_15_0 ) ).r * _noise1Mix * staticSwitch30 ) + ( tex2D( _noiseTex2, ( temp_output_15_0 + ( _Time.y * appendResult18 ) + ( _noise2Tiling * i.ase_texcoord.xy ) ) ).r * staticSwitch32 ) ) ) * ( pow( i.ase_texcoord.y , _uvDistGradMaskPow ) * tex2D( _MainTex, uv_MainTex ).b ) ) * staticSwitch53 ) )));
				float4 tex2DNode58 = tex2D( _MainTex, appendResult59 );
				float4 screenPos = i.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth74 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( screenPos )));
				float distanceDepth74 = abs( ( screenDepth74 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _distance ) );
				float4 appendResult77 = (float4(( ( ( _ColorEdge * _edgeColorMultiplier * tex2DNode58.r ) + ( _ColorBase * _baseColorMultiplier * tex2DNode58.g ) ) * _finalEmissive ).rgb , saturate( ( i.ase_color.a * saturate( ( tex2DNode58.r + tex2DNode58.g ) ) * saturate( distanceDepth74 ) * _finalOpacity ) )));
				
				
				finalColor = appendResult77;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
1936;215;1645;720;-889.3048;210.6823;1.389733;True;True
Node;AmplifyShaderEditor.RangedFloatNode;6;-695.2684,202.5731;Float;False;Property;_noise1PanX;noise1PanX;2;0;Create;True;0;0;False;0;0;-0.159;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-706.4338,286.0089;Float;False;Property;_noise1PanY;noise1PanY;3;0;Create;True;0;0;False;0;0;-0.695;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-737.9833,1039.943;Float;False;Property;_noise2PanY;noise2PanY;5;0;Create;True;0;0;False;0;0;-0.371;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-716.5084,899.8041;Float;False;Property;_noise2PanX;noise2PanX;4;0;Create;True;0;0;False;0;0;0.206;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;21;-232.0204,1292.72;Float;False;292;352;Comment;1;91;UV set 0;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;14;-222.6417,513.8869;Float;False;292;352;Comment;1;90;UV set 1;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;13;-427.3515,-300.7573;Float;False;292;352;Comment;1;92;UV set 0;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;11;-66.60275,322.743;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;3,8;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode;92;-346.743,-184.6771;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;90;-123.5525,636.2388;Float;False;1;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;2;-435.4296,-411.3232;Float;False;Property;_noise1Tiling;noise1Tiling;0;0;Create;True;0;0;False;0;0;0.89;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;-414.3742,937.7628;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1085.595,73.52109;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-242.6027,1182.154;Float;False;Property;_noise2Tiling;noise2Tiling;1;0;Create;True;0;0;False;0;0;0.86;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;8;-393.1342,243.1091;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;91;-136.6689,1442.209;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-159.0218,941.1281;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-150.0341,145.6091;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;29;447.0908,-634.3441;Float;False;292;352;Comment;1;87;UV set 2;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-77.83868,-336.7573;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;117.4924,1256.72;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;154.427,472.5692;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;35;670.3242,1320.897;Float;False;292;352;Comment;1;39;UV set 1;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;39;734.0934,1430.189;Float;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;20;422.2759,730.6647;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;34;665.0638,1037.555;Float;False;Property;_noise2Mix;noise2Mix;12;0;Create;True;0;0;False;0;0;0.694;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;87;500.5641,-503.8885;Float;False;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;10;316.1445,5.60143;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;645.6983,-217.7719;Float;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;45;1511.57,538.0641;Float;False;292;352;Comment;1;88;UV set 0;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;27;498.072,262.8396;Float;False;Property;_noise1Mix;noise1Mix;8;0;Create;True;0;0;False;0;0;0.384;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;30;957.443,-369.4424;Float;True;Property;_dynamicNoise1MixUV2X;dynamicNoise1Mix(UV2X);9;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;644.2602,711.132;Float;True;Property;_noiseTex2;noiseTex2;7;0;Create;True;0;0;False;0;None;5fab719e593167a4ab695bfbdc97b4d6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;25;600.0812,-25.8227;Float;True;Property;_noiseTex1;noiseTex1;6;0;Create;True;0;0;False;0;None;11254458ffd495a4cac419eca7d33d95;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;32;1166.096,1038.858;Float;True;Property;_dynamicNoise2MixUV1W;dynamicNoise2Mix(UV1W);10;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;41;1565.492,-422.4059;Float;False;270;257;Comment;1;40;UV set 1;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;1336.321,336.3073;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;1581.265,936.0424;Float;False;Property;_uvDistGradMaskPow;uvDistGradMaskPow;13;0;Create;True;0;0;False;0;1.069;1.069;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;1302.648,-42.2015;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;88;1569.809,649.1998;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;61;1084.949,-815.9987;Float;True;Property;_MainTex;MainTex;17;0;Create;True;0;0;False;0;None;b989b8c551ef914479cca53b1b84df53;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;40;1615.492,-372.406;Float;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;38;1597.475,205.8632;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;47;1928.267,592.7319;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;60;1820.033,285.6346;Float;True;Property;_TextureSample1;Texture Sample 1;16;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;2247.961,502.0695;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;2113.661,-477.5724;Float;False;Property;_uvDistIntensity;uvDistIntensity;14;0;Create;True;0;0;False;0;0;-3.931598;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;42;1992.21,-4.708219;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;53;2435.281,-328.9909;Float;True;Property;_useDynamicDistUV1Z;useDynamicDist(UV1Z);11;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;2449.382,98.06439;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;2666.622,-692.909;Float;False;292;352;Comment;1;89;UV set 0;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;2816.039,36.2353;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;89;2707.741,-595.0606;Float;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;54;3170.71,-32.79095;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;3410.757,-206.8246;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;58;3608.795,-346.8517;Float;True;Property;_TextureSample0;Texture Sample 0;16;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;75;3638.019,27.34716;Float;False;Property;_distance;distance;21;0;Create;True;0;0;False;0;0;0.155;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;62;3534.634,-1683.205;Float;False;Property;_ColorEdge;ColorEdge;15;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;0.9811321,0.6626147,0.1897471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;3480.231,-1408.594;Float;False;Property;_edgeColorMultiplier;edgeColorMultiplier;18;0;Create;True;0;0;False;0;0;2.69;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;74;3931.086,-8.564277;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;3477.43,-1029.907;Float;False;Property;_baseColorMultiplier;baseColorMultiplier;19;0;Create;True;0;0;False;0;0;3;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;3968.494,-242.8869;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;65;3530.59,-1304.518;Float;False;Property;_ColorBase;ColorBase;16;0;Create;True;0;0;False;0;0.5,0.5,0.5,1;0.9921568,0.6293204,0.06399991,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;76;4001.528,221.9939;Float;False;Property;_finalOpacity;finalOpacity;22;0;Create;True;0;0;False;0;0;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;3901.979,-1559.322;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;78;4154.657,-199.2623;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;79;4192.319,-22.17799;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;3897.935,-1180.635;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;73;4046.323,-448.4284;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;68;4108.563,-1346.422;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;4427.244,-183.698;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;4099.632,-1193.533;Float;False;Property;_finalEmissive;finalEmissive;20;0;Create;True;0;0;False;0;0;17.3;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;4451.505,-1144.078;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;80;4620.469,-251.2766;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;77;4765.645,-922.0609;Float;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;86;5137.091,-1064.864;Float;False;True;2;Float;ASEMaterialInspector;0;1;Custom/FirePlace/S_SideFlames;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;8;5;False;-1;1;False;-1;0;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Transparent=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;19;0;5;0
WireConnection;19;1;18;0
WireConnection;9;0;5;0
WireConnection;9;1;8;0
WireConnection;4;0;2;0
WireConnection;4;1;92;0
WireConnection;23;0;22;0
WireConnection;23;1;91;0
WireConnection;15;0;11;0
WireConnection;15;1;90;0
WireConnection;20;0;15;0
WireConnection;20;1;19;0
WireConnection;20;2;23;0
WireConnection;10;0;4;0
WireConnection;10;1;9;0
WireConnection;10;2;15;0
WireConnection;30;1;31;0
WireConnection;30;0;87;1
WireConnection;33;1;20;0
WireConnection;25;1;10;0
WireConnection;32;1;34;0
WireConnection;32;0;39;4
WireConnection;37;0;33;1
WireConnection;37;1;32;0
WireConnection;26;0;25;1
WireConnection;26;1;27;0
WireConnection;26;2;30;0
WireConnection;38;0;26;0
WireConnection;38;1;37;0
WireConnection;47;0;88;2
WireConnection;47;1;46;0
WireConnection;60;0;61;0
WireConnection;48;0;47;0
WireConnection;48;1;60;3
WireConnection;42;0;40;1
WireConnection;42;1;38;0
WireConnection;53;1;52;0
WireConnection;53;0;40;3
WireConnection;50;0;42;0
WireConnection;50;1;48;0
WireConnection;51;0;50;0
WireConnection;51;1;53;0
WireConnection;54;0;89;2
WireConnection;54;1;51;0
WireConnection;59;0;89;1
WireConnection;59;1;54;0
WireConnection;58;0;61;0
WireConnection;58;1;59;0
WireConnection;74;0;75;0
WireConnection;71;0;58;1
WireConnection;71;1;58;2
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;64;2;58;1
WireConnection;78;0;71;0
WireConnection;79;0;74;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;67;2;58;2
WireConnection;68;0;64;0
WireConnection;68;1;67;0
WireConnection;72;0;73;4
WireConnection;72;1;78;0
WireConnection;72;2;79;0
WireConnection;72;3;76;0
WireConnection;69;0;68;0
WireConnection;69;1;70;0
WireConnection;80;0;72;0
WireConnection;77;0;69;0
WireConnection;77;3;80;0
WireConnection;86;0;77;0
ASEEND*/
//CHKSM=F4B409EB95B56DF1E681A08F8905E6CA6D6D9D13