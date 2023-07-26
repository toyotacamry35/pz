// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/S_MainFlame"
{
	Properties
	{
		_noise1("noise1", 2D) = "white" {}
		_MainTile("MainTile", Vector) = (1,0.1,0,0)
		_PowerColorEdge("PowerColorEdge", Float) = 2.13
		_noise2("noise2", 2D) = "white" {}
		_Noise1Power("Noise1Power", Float) = 0.33
		_Noise1Tile("Noise1Tile", Vector) = (1,1,0,0)
		_Noise2Tile("Noise2Tile", Vector) = (1,1,0,0)
		_WholeOpacity("WholeOpacity", Range( 0 , 1)) = 0
		_Distance("Distance", Range( 0 , 1)) = 0.5870594
		_DissapearenceMax("DissapearenceMax", Float) = 1
		_MainMask("MainMask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha One , SrcAlpha One
		Cull Back
		ColorMask RGBA
		ZWrite On
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


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
			};

			uniform sampler2D _MainMask;
			uniform sampler2D _noise1;
			uniform float2 _Noise1Tile;
			uniform float _Noise1Power;
			uniform float4 _MainMask_ST;
			uniform float2 _MainTile;
			uniform sampler2D _noise2;
			uniform float2 _Noise2Tile;
			uniform float _PowerColorEdge;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _Distance;
			uniform float _WholeOpacity;
			uniform float _DissapearenceMax;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord2.xyz = ase_worldPos;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				o.ase_texcoord2.w = 0;
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
				float4 color3 = IsGammaSpace() ? float4(0.2029192,0.7547169,0.5507547,1) : float4(0.03402161,0.5298999,0.2640623,1);
				float2 appendResult28 = (float2(0.09 , 1.36));
				float2 uv025 = i.ase_texcoord.xy * _Noise1Tile + ( -_Time.y * appendResult28 );
				float2 uv_MainMask = i.ase_texcoord.xy * _MainMask_ST.xy + _MainMask_ST.zw;
				float2 uv010 = i.ase_texcoord.xy * _MainTile + float2( 0,0 );
				float2 appendResult60 = (float2(-0.36 , -2.34));
				float2 uv064 = i.ase_texcoord.xy * _Noise2Tile + ( ( 0.2 * _Time.y ) * appendResult60 );
				float4 tex2DNode2 = tex2D( _MainMask, ( ( tex2D( _noise1, uv025 ).r * _Noise1Power * (-0.79 + (( 1.0 - tex2D( _MainMask, uv_MainMask ).b ) - 0.0) * (1.02 - -0.79) / (1.0 - 0.0)) ) + uv010 + ( (0.04 + (uv010.y - 0.0) * (2.0 - 0.04) / (1.0 - 0.0)) * tex2D( _noise2, uv064 ).r * 0.55 ) ) );
				float4 color4 = IsGammaSpace() ? float4(1,0.5062699,0.1367925,1) : float4(1,0.2198904,0.01671052,1);
				float4 temp_output_34_0 = ( 4.0 * ( ( color3 * tex2DNode2.r * _PowerColorEdge ) + ( color4 * tex2DNode2.g * 1.23 ) ) );
				float4 screenPos = i.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth72 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( screenPos )));
				float distanceDepth72 = abs( ( screenDepth72 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Distance ) );
				float3 ase_worldPos = i.ase_texcoord2.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float dotResult78 = dot( ase_worldViewDir , float3(0,1,0) );
				float lerpResult79 = lerp( 1.0 , 0.0 , saturate( (0.0 + (dotResult78 - 0.0) * (1.0 - 0.0) / (_DissapearenceMax - 0.0)) ));
				float4 appendResult22 = (float4(temp_output_34_0.rgb , ( ( temp_output_34_0.a + ( temp_output_34_0.a * i.ase_color.a ) ) * saturate( distanceDepth72 ) * _WholeOpacity * lerpResult79 * saturate( (-0.61 + (uv010.y - 0.0) * (30.29 - -0.61) / (1.0 - 0.0)) ) )));
				
				
				finalColor = appendResult22;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
1936;215;1645;720;3775.446;1293.454;3.702662;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;58;-3358.288,-415.3335;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-3343.229,-509.8909;Float;False;Constant;_Noise2Speed;Noise2Speed;8;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2921.478,-739.7391;Float;False;Constant;_noise1OffsetY;noise1OffsetY;3;0;Create;True;0;0;False;0;1.36;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2899.98,-839.1161;Float;False;Constant;_noise1OfsetX;noise1OfsetX;3;0;Create;True;0;0;False;0;0.09;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-3303.95,-235.5781;Float;False;Constant;_Noise2OffsetX;Noise2OffsetX;3;0;Create;True;0;0;False;0;-0.36;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-3325.448,-136.2011;Float;False;Constant;_noise2OffsetY;noise2OffsetY;3;0;Create;True;0;0;False;0;-2.34;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;27;-2888.118,-972.5317;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;30;-2620.918,-924.664;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;60;-3111.615,-227.2938;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-3084.751,-472.0434;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;95;-2534.071,366.9111;Float;True;Property;_MainMask;MainMask;10;0;Create;True;0;0;False;0;None;83c87d9ec94480a41bf906ce9195b56d;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;28;-2707.645,-830.8318;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;54;-2096.563,303.5557;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;83c87d9ec94480a41bf906ce9195b56d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;62;-2766.675,-529.9772;Float;False;Property;_Noise2Tile;Noise2Tile;6;0;Create;True;0;0;False;0;1,1;1.54,0.74;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;50;-2362.704,-1133.515;Float;False;Property;_Noise1Tile;Noise1Tile;5;0;Create;True;0;0;False;0;1,1;2.4,0.56;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-2305.729,-892.4819;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-2709.699,-288.944;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;14;-2675.013,9.343758;Float;False;Property;_MainTile;MainTile;1;0;Create;True;0;0;False;0;1,0.1;1,0.25;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;55;-1741.449,267.2376;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-2520.167,-412.1241;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.82,0.78;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-2129.345,-1053.362;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.82,0.78;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-2415.957,-3.689195;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;24;-2232.135,-368.4914;Float;True;Property;_noise2;noise2;3;0;Create;True;0;0;False;0;5fab719e593167a4ab695bfbdc97b4d6;5fab719e593167a4ab695bfbdc97b4d6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;52;-2015.646,-54.24442;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.04;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;23;-1852.439,-995.5129;Float;True;Property;_noise1;noise1;0;0;Create;True;0;0;False;0;11254458ffd495a4cac419eca7d33d95;11254458ffd495a4cac419eca7d33d95;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-1735.185,-731.4379;Float;False;Property;_Noise1Power;Noise1Power;4;0;Create;True;0;0;False;0;0.33;0.42;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-1863.081,-181.1323;Float;False;Constant;_Noise2Power;Noise2Power;8;0;Create;True;0;0;False;0;0.55;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;56;-1584.326,90.83521;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.79;False;4;FLOAT;1.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1612.966,-363.1372;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-1514.622,-860.4883;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1206.095,-677.3987;Float;True;3;3;0;FLOAT;0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-945.0679,-108.4011;Float;False;Property;_PowerColorEdge;PowerColorEdge;2;0;Create;True;0;0;False;0;2.13;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1257.777,-233.2029;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;None;b989b8c551ef914479cca53b1b84df53;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-955.1754,41.89002;Float;False;Constant;_ColorBase;ColorBase;1;0;Create;True;0;0;False;0;1,0.5062699,0.1367925,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-918.4334,264.1027;Float;False;Constant;_POweColorBase;POweColorBase;1;0;Create;True;0;0;False;0;1.23;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-950.4448,-344.1375;Float;False;Constant;_ColorEdge;ColorEdge;1;0;Create;True;0;0;False;0;0.2029192,0.7547169,0.5507547,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-680.5471,46.19972;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-559.1439,-319.5715;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-221.0271,-140.0359;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;75;-235.6941,943.1329;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;35;-260.9596,-353.2917;Float;False;Constant;_WholeEmissPower;WholeEmissPower;3;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;105;-216.1451,1100.465;Float;False;Constant;_UpVector;UpVector;15;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-10.45917,-363.1865;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;78;-38.71035,1002.065;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;207.9716,1099.29;Float;False;Property;_DissapearenceMax;DissapearenceMax;9;0;Create;True;0;0;False;0;1;0.97;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;90;447.9846,1035.512;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.97;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;269.2056,461.0388;Float;False;Property;_Distance;Distance;8;0;Create;True;0;0;False;0;0.5870594;0.1619221;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;20;277.5143,-261.7103;Float;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.VertexColorNode;19;330.5872,120.3528;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;590.8696,65.97113;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;72;556.6052,451.4537;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;93;648.4679,1024.156;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;106;-1456.962,601.5439;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.61;False;4;FLOAT;30.29;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;79;833.1431,957.5477;Float;True;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;524.5363,764.8552;Float;False;Property;_WholeOpacity;WholeOpacity;7;0;Create;True;0;0;False;0;0;0.6445572;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;85;815.1039,439.0347;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;838.2086,11.79216;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;107;-1143.913,605.8275;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;1125.716,109.7071;Float;True;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;1458.805,-82.84527;Float;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;89;1811.022,-264.6006;Float;False;True;2;Float;ASEMaterialInspector;0;1;Custom/S_MainFlame;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;8;5;False;-1;1;False;-1;8;5;False;-1;1;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;30;0;27;0
WireConnection;60;0;57;0
WireConnection;60;1;59;0
WireConnection;67;0;68;0
WireConnection;67;1;58;0
WireConnection;28;0;46;0
WireConnection;28;1;47;0
WireConnection;54;0;95;0
WireConnection;45;0;30;0
WireConnection;45;1;28;0
WireConnection;63;0;67;0
WireConnection;63;1;60;0
WireConnection;55;0;54;3
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;25;0;50;0
WireConnection;25;1;45;0
WireConnection;10;0;14;0
WireConnection;24;1;64;0
WireConnection;52;0;10;2
WireConnection;23;1;25;0
WireConnection;56;0;55;0
WireConnection;65;0;52;0
WireConnection;65;1;24;1
WireConnection;65;2;66;0
WireConnection;32;0;23;1
WireConnection;32;1;33;0
WireConnection;32;2;56;0
WireConnection;31;0;32;0
WireConnection;31;1;10;0
WireConnection;31;2;65;0
WireConnection;2;0;95;0
WireConnection;2;1;31;0
WireConnection;8;0;4;0
WireConnection;8;1;2;2
WireConnection;8;2;18;0
WireConnection;6;0;3;0
WireConnection;6;1;2;1
WireConnection;6;2;17;0
WireConnection;12;0;6;0
WireConnection;12;1;8;0
WireConnection;34;0;35;0
WireConnection;34;1;12;0
WireConnection;78;0;75;0
WireConnection;78;1;105;0
WireConnection;90;0;78;0
WireConnection;90;2;92;0
WireConnection;20;0;34;0
WireConnection;81;0;20;3
WireConnection;81;1;19;4
WireConnection;72;0;73;0
WireConnection;93;0;90;0
WireConnection;106;0;10;2
WireConnection;79;2;93;0
WireConnection;85;0;72;0
WireConnection;83;0;20;3
WireConnection;83;1;81;0
WireConnection;107;0;106;0
WireConnection;84;0;83;0
WireConnection;84;1;85;0
WireConnection;84;2;71;0
WireConnection;84;3;79;0
WireConnection;84;4;107;0
WireConnection;22;0;34;0
WireConnection;22;3;84;0
WireConnection;89;0;22;0
ASEEND*/
//CHKSM=79B6FC1FD552BF8846BB31F8D7FACDC4C6201EC5