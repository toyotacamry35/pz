// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/S_SphereWaveQuasar"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Float0("Float 0", Float) = 0
		_RimPower("RimPower", Range( 0 , 10)) = 0
		_Float1("Float 1", Float) = 0
		_Float2("Float 2", Float) = 0
		_TextureSample1("Texture Sample 1", 2D) = "white" {}
		_Float5("Float 5", Range( 0 , 1)) = 0
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
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
				float4 ase_tangent : TANGENT;
				float3 ase_normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
			};

			uniform sampler2D _TextureSample0;
			uniform float _Float2;
			uniform float _Float0;
			uniform float _Float1;
			uniform sampler2D _TextureSample1;
			uniform float _RimPower;
			uniform float _Float5;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord1.xyz = ase_worldTangent;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord3.xyz = ase_worldBitangent;
				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord4.xyz = ase_worldPos;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
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
				float2 appendResult2 = (float2(0.0 , _Time.y));
				float2 uv01 = i.ase_texcoord.xy * float2( 1,1 ) + appendResult2;
				float4 color34 = IsGammaSpace() ? float4(0,0.9372549,1,0) : float4(0,0.8631573,1,0);
				float2 appendResult24 = (float2(0.0 , _Float2));
				float2 uv021 = i.ase_texcoord.xy * float2( 1,1 ) + appendResult24;
				float temp_output_20_0 = saturate( (_Float0 + (( 1.0 - ( uv021.y * uv021.y ) ) - 0.0) * (_Float1 - _Float0) / (1.0 - 0.0)) );
				float4 color28 = IsGammaSpace() ? float4(0.3338822,0.3393315,0.5754717,1) : float4(0.09115017,0.09424575,0.2906642,1);
				float4 color40 = IsGammaSpace() ? float4(0.7311321,0.8117558,1,0) : float4(0.4936094,0.6239451,1,0);
				float2 appendResult37 = (float2(0.0 , ( 2.0 * _Time.y )));
				float2 uv038 = i.ase_texcoord.xy * float2( 1,1 ) + appendResult37;
				float3 ase_worldTangent = i.ase_texcoord1.xyz;
				float3 ase_worldNormal = i.ase_texcoord2.xyz;
				float3 ase_worldBitangent = i.ase_texcoord3.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 ase_worldPos = i.ase_texcoord4.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_tanViewDir =  tanToWorld0 * ase_worldViewDir.x + tanToWorld1 * ase_worldViewDir.y  + tanToWorld2 * ase_worldViewDir.z;
				ase_tanViewDir = normalize(ase_tanViewDir);
				float3 normalizeResult51 = normalize( ase_tanViewDir );
				float dotResult46 = dot( float3(0,0,1) , normalizeResult51 );
				float temp_output_47_0 = saturate( dotResult46 );
				float temp_output_50_0 = pow( temp_output_47_0 , _RimPower );
				float2 uv06 = i.ase_texcoord.xy * float2( 1,1 ) + float2( -0.5,-0.5 );
				float temp_output_15_0 = ( temp_output_50_0 * saturate( (-5.19 + (( 1.0 - ( uv06.y * uv06.y ) ) - 0.0) * (1.76 - -5.19) / (1.0 - 0.0)) ) );
				float4 appendResult16 = (float4(( ( tex2D( _TextureSample0, uv01 ) * color34 * 3.0 ) + ( temp_output_20_0 * color28 ) + ( color40 * tex2D( _TextureSample1, uv038 ) * 2.0 ) ).rgb , ( saturate( temp_output_15_0 ) * _Float5 )));
				
				
				finalColor = appendResult16;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
2065;276;1206;774;612.1417;133.8263;1.530024;True;True
Node;AmplifyShaderEditor.RangedFloatNode;25;-1752.293,665.1051;Float;False;Property;_Float2;Float 2;4;0;Create;True;0;0;False;0;0;-0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1614.592,642.9292;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1139.128,209.3974;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;52;-935.6616,1288.16;Float;False;Tangent;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1409.988,523.4564;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;51;-712.9646,1285.06;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;55;-898.6416,970.0086;Float;True;Constant;_Vector0;Vector 0;8;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-893.6414,270.676;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-1211.548,-96.46527;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1164.502,584.735;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;46;-521.2647,1206.66;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1049.133,-237.9274;Float;False;2;2;0;FLOAT;2;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;8;-671.1414,258.7762;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;2;-714.0168,-53.27473;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;37;-902.6187,-294.8142;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-273.6642,1339.86;Float;False;Property;_RimPower;RimPower;2;0;Create;True;0;0;False;0;0;3.8;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;47;-346.0655,1182.16;Float;False;1;0;FLOAT;1.23;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;-942.0016,572.8352;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-661.8737,795.7468;Float;False;Property;_Float1;Float 1;3;0;Create;True;0;0;False;0;0;1.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-672.68,664.8698;Float;False;Property;_Float0;Float 0;1;0;Create;True;0;0;False;0;0;-66.95;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;11;-479.5612,271.8431;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-5.19;False;4;FLOAT;1.76;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;17;-486.0576,564.371;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;12;-195.0525,275.2559;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;50;17.27729,1243.234;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-520.1888,-127.2547;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;38;-708.7907,-368.7942;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;-335.2174,-349.319;Float;False;Constant;_Color1;Color 1;4;0;Create;True;0;0;False;0;0,0.9372549,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;-365.721,-752.2339;Float;False;Constant;_Color2;Color 2;5;0;Create;True;0;0;False;0;0.7311321,0.8117558,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;-468.1413,-551.1559;Float;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;None;27e0491fe65f08e44b3c98e425a4ffd4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-75.09674,-676.7911;Float;False;Constant;_Float6;Float 6;7;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-192.1418,895.9445;Float;False;Constant;_Color0;Color 0;4;0;Create;True;0;0;False;0;0.3338822,0.3393315,0.5754717,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-311.1022,-175.1223;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;01d2d4a7606fcef4c88cb6c3784f6305;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;20;-195.0537,559.8221;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-97.58511,-408.2025;Float;False;Constant;_Float3;Float 3;4;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;99.96319,100.8049;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;449.5727,352.4617;Float;False;Property;_Float5;Float 5;7;0;Create;True;0;0;False;0;0;0.2897375;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;41.43512,659.9706;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-66.81836,-540.6818;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;53.44387,-334.9229;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;31;397.7668,177.2213;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;547.0731,234.7477;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;206.2562,-194.5381;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;279.6171,537.1157;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;458.4445,-30.34542;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2.497192,387.6266;Float;False;Property;_Float4;Float 4;6;0;Create;True;0;0;False;0;0.6;1.89;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;48;-177.0625,1224.459;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;213.6484,184.8276;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;696.8889,-42.41775;Float;False;True;2;Float;ASEMaterialInspector;0;1;Custom/S_SphereWaveQuasar;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;2;5;False;-1;10;False;-1;2;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Transparent=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;24;1;25;0
WireConnection;21;1;24;0
WireConnection;51;0;52;0
WireConnection;7;0;6;2
WireConnection;7;1;6;2
WireConnection;22;0;21;2
WireConnection;22;1;21;2
WireConnection;46;0;55;0
WireConnection;46;1;51;0
WireConnection;39;1;3;0
WireConnection;8;0;7;0
WireConnection;2;1;3;0
WireConnection;37;1;39;0
WireConnection;47;0;46;0
WireConnection;23;0;22;0
WireConnection;11;0;8;0
WireConnection;17;0;23;0
WireConnection;17;3;18;0
WireConnection;17;4;19;0
WireConnection;12;0;11;0
WireConnection;50;0;47;0
WireConnection;50;1;49;0
WireConnection;1;1;2;0
WireConnection;38;1;37;0
WireConnection;36;1;38;0
WireConnection;4;1;1;0
WireConnection;20;0;17;0
WireConnection;15;0;50;0
WireConnection;15;1;12;0
WireConnection;27;0;20;0
WireConnection;27;1;28;0
WireConnection;41;0;40;0
WireConnection;41;1;36;0
WireConnection;41;2;45;0
WireConnection;33;0;4;0
WireConnection;33;1;34;0
WireConnection;33;2;35;0
WireConnection;31;0;15;0
WireConnection;43;0;31;0
WireConnection;43;1;44;0
WireConnection;26;0;33;0
WireConnection;26;1;27;0
WireConnection;26;2;41;0
WireConnection;32;0;42;0
WireConnection;32;1;20;0
WireConnection;32;2;50;0
WireConnection;16;0;26;0
WireConnection;16;3;43;0
WireConnection;48;0;47;0
WireConnection;30;0;15;0
WireConnection;30;1;32;0
WireConnection;0;0;16;0
ASEEND*/
//CHKSM=44F72CA62A7F3C3821F1E7996CC64AD2320BD74A