// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "bilboard_add"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (0,0.978663,1,0)
		_Blink_speed("Blink_speed", Float) = 0.72
		_blink_type("blink_type", Int) = 0
		_camera_offset("camera_offset", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		

		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform float _camera_offset;
			uniform sampler2D _TextureSample0;
			uniform float4 _TextureSample0_ST;
			uniform float4 _Color;
			uniform float _Blink_speed;
			uniform int _blink_type;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				//Calculate new billboard vertex position and normal;
				float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 );
				float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
				float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
				float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
				v.ase_normal = normalize( mul( float4( v.ase_normal , 0 ), rotationCamMatrix ));
				//This unfortunately must be made to take non-uniform scaling into account;
				//Transform to world coords, apply rotation and transform back to local;
				v.vertex = mul( v.vertex , unity_ObjectToWorld );
				v.vertex = mul( v.vertex , rotationCamMatrix );
				v.vertex = mul( v.vertex , unity_WorldToObject );
				float4 transform32 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
				float3 normalizeResult37 = normalize( ( _WorldSpaceCameraPos - (transform32).xyz ) );
				float4 transform29 = mul(unity_WorldToObject,float4( ( _camera_offset * normalizeResult37 ) , 0.0 ));
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				
				v.vertex.xyz += ( (transform29).xyz + 0 );
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 uv_TextureSample0 = i.ase_texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float4 tex2DNode1 = tex2D( _TextureSample0, uv_TextureSample0 );
				float temp_output_10_0 = frac( ( _Time.y * _Blink_speed ) );
				float lerpResult14 = lerp( temp_output_10_0 , ( 1.0 - temp_output_10_0 ) , (float)saturate( _blink_type ));
				float lerpResult18 = lerp( lerpResult14 , abs( (-1.0 + (temp_output_10_0 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) , (float)saturate( ( _blink_type - 1 ) ));
				
				
				finalColor = ( tex2DNode1 * _Color * lerpResult18 * _Color.a * tex2DNode1.a );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16200
2559;160;1844;744;555.037;599.387;1.730139;True;True
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;32;415.8127,-246.8572;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;5;-772.7458,137.2853;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;34;500.1981,-413.9264;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;36;607.3828,-253.9065;Float;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-767.7458,206.2853;Float;False;Property;_Blink_speed;Blink_speed;2;0;Create;True;0;0;False;0;0.72;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-568.7458,152.2853;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;35;863.2032,-341.2012;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FractNode;10;-401.7458,136.2853;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;9;-717.7458,336.2853;Float;False;Property;_blink_type;blink_type;3;0;Create;True;0;0;False;0;0;0;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;644.7562,-113.2411;Float;False;Property;_camera_offset;camera_offset;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;37;1059.533,-313.9329;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;16;-528.8587,431.4923;Float;False;2;0;INT;0;False;1;INT;1;False;1;INT;0
Node;AmplifyShaderEditor.SaturateNode;15;-526.1878,341.2557;Float;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.OneMinusNode;11;-212.1775,253.2592;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;1106.165,-194.4954;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;12;-280.1948,488.7158;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;13;-60.2469,539.0511;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;14;122.5359,266.6902;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;29;820.1187,164.5977;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;17;-367.7958,421.0533;Float;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.LerpOp;18;227.2402,395.7008;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BillboardNode;4;788.9106,459.4541;Float;False;Spherical;False;0;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;2;-616,-28;Float;False;Property;_Color;Color;1;1;[HDR];Create;True;0;0;False;0;0,0.978663,1,0;0,0.978663,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;30;1082.87,178.7516;Float;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;-614,-230;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;295.5572,-3.543405;Float;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;1296.181,230.093;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1420.839,-4.964158;Float;False;True;2;Float;ASEMaterialInspector;0;1;bilboard_add;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;36;0;32;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;35;0;34;0
WireConnection;35;1;36;0
WireConnection;10;0;6;0
WireConnection;37;0;35;0
WireConnection;16;0;9;0
WireConnection;15;0;9;0
WireConnection;11;0;10;0
WireConnection;38;0;24;0
WireConnection;38;1;37;0
WireConnection;12;0;10;0
WireConnection;13;0;12;0
WireConnection;14;0;10;0
WireConnection;14;1;11;0
WireConnection;14;2;15;0
WireConnection;29;0;38;0
WireConnection;17;0;16;0
WireConnection;18;0;14;0
WireConnection;18;1;13;0
WireConnection;18;2;17;0
WireConnection;30;0;29;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;18;0
WireConnection;3;3;2;4
WireConnection;3;4;1;4
WireConnection;28;0;30;0
WireConnection;28;1;4;0
WireConnection;0;0;3;0
WireConnection;0;1;28;0
ASEEND*/
//CHKSM=7D1458A725831EA22CB29660F3005D33F391BEEB