// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/SolarProminence"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureMask("Texture Mask", 2D) = "white" {}
		_TextureGradient("Texture Gradient", 2D) = "white" {}
	}


	Category 
	{
		SubShader
		{
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend One One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 ase_texcoord2 : TEXCOORD2;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _TextureGradient;
				uniform sampler2D _TextureMask;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					o.ase_texcoord3.xy = v.ase_texcoord2.xy;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.zw = 0;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 uv245 = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
					float temp_output_28_0 = ( length( ( uv245 - float2( 0.5,0.5 ) ) ) * 21.0 * 0.06014688 );
					float2 appendResult33 = (float2(temp_output_28_0 , temp_output_28_0));
					float temp_output_35_0 = frac( ( _Time.y * ( 0.25 * i.color.r ) ) );
					float temp_output_40_0 = ( tex2D( _TextureMask, ( uv245 + ( appendResult33 * (-2.0 + (temp_output_35_0 - 0.0) * (0.0 - -2.0) / (1.0 - 0.0)) ) ) ).r * ( 1.0 - temp_output_35_0 ) );
					float2 temp_cast_0 = (temp_output_40_0).xx;
					float2 uv073 = i.texcoord.xy * float2( 1,1 ) + float2( -0.5,-0.45 );
					

					fixed4 col = ( tex2D( _TextureGradient, temp_cast_0 ) * i.color.a * ( 3.0 * temp_output_40_0 ) * saturate( pow( ( 1.0 - ( ( uv073.x * uv073.x ) + ( uv073.y * uv073.y ) ) ) , 6.14 ) ) );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
2065;276;1206;774;-1022.758;-620.5954;1.3;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1243.197,980.3842;Float;True;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;70;-397.3221,1527.338;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-311.7375,1341.919;Float;False;Constant;_Speed;Speed;5;0;Create;True;0;0;False;0;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-262.091,1034.617;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-91.59106,1440.394;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;53;-852.2043,993.722;Float;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LengthOpNode;27;-545.661,641.2598;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-89.73755,1257.919;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-571.8273,1144.277;Float;False;Constant;_Float4;Float 4;1;0;Create;True;0;0;False;0;0.06014688;0.06014688;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;35;15.49604,1057.716;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-354.7794,719.14;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;21;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;33;-30.2478,835.2475;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;36;252.3082,1030.808;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-2;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;73;370.4796,1985.521;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.45;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;903.4163,1490.407;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;344.9199,740.5203;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;932.7162,1714.007;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;76;1074.716,1683.007;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;578.2772,663.9063;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;11;994.1707,576.4682;Float;True;Property;_TextureMask;Texture Mask;0;0;Create;True;0;0;False;0;None;b15bbffcf7cc43d43b709ce76acd24d8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;39;224.1012,1310.926;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;77;1341.537,1658.318;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;1152.167,861.1469;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;81;1464.542,1829.054;Float;True;2;0;FLOAT;0;False;1;FLOAT;6.14;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;1330.724,1025.759;Float;False;Constant;_Float8;Float 8;7;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;1442.672,895.6104;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;1323.633,642.8833;Float;True;Property;_TextureGradient;Texture Gradient;1;0;Create;True;0;0;False;0;None;fb2cba6fd842c6543872d7e2e8f6243c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;80;1552.943,1158.82;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;1748.814,841.9988;Float;True;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;69;2300.284,742.5504;Float;False;True;2;Float;ASEMaterialInspector;0;7;Custom/SolarProminence;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;87;0;38;0
WireConnection;87;1;70;1
WireConnection;53;0;45;0
WireConnection;27;0;53;0
WireConnection;37;0;34;0
WireConnection;37;1;87;0
WireConnection;35;0;37;0
WireConnection;28;0;27;0
WireConnection;28;2;29;0
WireConnection;33;0;28;0
WireConnection;33;1;28;0
WireConnection;36;0;35;0
WireConnection;74;0;73;1
WireConnection;74;1;73;1
WireConnection;31;0;33;0
WireConnection;31;1;36;0
WireConnection;75;0;73;2
WireConnection;75;1;73;2
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;30;0;45;0
WireConnection;30;1;31;0
WireConnection;11;1;30;0
WireConnection;39;0;35;0
WireConnection;77;0;76;0
WireConnection;40;0;11;1
WireConnection;40;1;39;0
WireConnection;81;0;77;0
WireConnection;88;0;86;0
WireConnection;88;1;40;0
WireConnection;42;1;40;0
WireConnection;80;0;81;0
WireConnection;43;0;42;0
WireConnection;43;1;70;4
WireConnection;43;2;88;0
WireConnection;43;3;80;0
WireConnection;69;0;43;0
ASEEND*/
//CHKSM=D723B3D5F36A2DEA30D59A771D965BFF6C18AD3F