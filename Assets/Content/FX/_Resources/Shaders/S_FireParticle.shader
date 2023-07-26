// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "S_FireParticle"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_impact02_dust_FX("impact02_dust_FX", 2D) = "white" {}
		_noise("noise", 2D) = "white" {}
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_ColorMult("ColorMult", Float) = 3
		_dist("dist", Float) = 0.2
		_speedU("speedU", Float) = 0.5
		_speedV("speedV", Float) = 0.5
		_maskScale("maskScale", Float) = 2
		_NoiseTile("NoiseTile", Vector) = (0.2,0.2,0,0)
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
					
				};
				
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif
				uniform float _InvFade;
				uniform float _speedU;
				uniform float _speedV;
				uniform float2 _NoiseTile;
				uniform float _maskScale;
				uniform sampler2D _impact02_dust_FX;
				uniform sampler2D _noise;
				uniform sampler2D _TextureSample0;
				uniform float _dist;
				uniform float _ColorMult;

				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

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

					float2 uv25 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float clampResult48 = clamp( ( ( (1.0 + (uv25.x - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) * _maskScale ) * ( uv25.x * _maskScale ) * ( (1.0 + (uv25.y - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) * _maskScale ) * ( uv25.y * _maskScale ) ) , 0.0 , 1.0 );
					float2 uv5 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 appendResult22 = (float2(( _speedU * _Time.y ) , ( _Time.y * _speedV )));
					float2 uv9 = i.texcoord.xy * _NoiseTile + appendResult22;
					float4 tex2DNode2 = tex2D( _noise, uv9 );
					float2 appendResult61 = (float2(( ( _speedV * 0.3 ) * _Time.y ) , ( ( _speedU * 0.3 ) * _Time.y )));
					float2 uv62 = i.texcoord.xy * ( _NoiseTile * float2( 0.7,0.7 ) ) + appendResult61;
					float4 tex2DNode63 = tex2D( _TextureSample0, uv62 );
					float2 appendResult71 = (float2(tex2DNode2.r , tex2DNode63.r));
					float2 lerpResult6 = lerp( uv5 , appendResult71 , _dist);
					float4 tex2DNode1 = tex2D( _impact02_dust_FX, lerpResult6 );
					float temp_output_69_0 = pow( ( ( clampResult48 * tex2DNode1.r * tex2DNode1.a ) * _ColorMult ) , _ColorMult );
					float clampResult70 = clamp( temp_output_69_0 , 0.0 , 1.0 );
					

					fixed4 col = ( temp_output_69_0 * i.color * ( clampResult70 * i.color.a ) );
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
Version=16301
2018;202;1584;656;3797.512;699.3469;3.099723;True;True
Node;AmplifyShaderEditor.RangedFloatNode;21;-2462.212,473.2761;Float;False;Property;_speedV;speedV;6;0;Create;True;0;0;True;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-2730.755,715.5104;Float;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2533.646,124.5084;Float;False;Property;_speedU;speedU;5;0;Create;True;0;0;True;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;15;-2774.975,264.9175;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-2458.908,878.0709;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2512.186,689.5554;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-2236.767,189.9968;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;51;-2543.763,-34.25983;Float;False;Property;_NoiseTile;NoiseTile;8;0;Create;True;0;0;True;0;0.2,0.2;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-2227.165,404.4568;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-2264.458,687.6382;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-2254.856,902.0982;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2094.327,317.7017;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;61;-2122.018,815.3431;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-2069.917,546.2991;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.7,0.7;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1918.852,196.3991;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.2,0.2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;62;-1946.543,694.0405;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.2,0.2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-2478.11,-392.0286;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;36;-2110.715,-820.0667;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1645.155,246.0487;Float;True;Property;_noise;noise;1;0;Create;True;0;0;False;0;d5bf4d08352180a4484428b7d6ad384e;d5bf4d08352180a4484428b7d6ad384e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;-1913.559,-607.5217;Float;False;Property;_maskScale;maskScale;7;0;Create;True;0;0;False;0;2;2.79;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;30;-2100.51,-345.3751;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;63;-1672.846,743.6901;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;d5bf4d08352180a4484428b7d6ad384e;d5bf4d08352180a4484428b7d6ad384e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1223.825,-116.2372;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-1866.729,-734.8435;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1244.043,276.6799;Float;False;Property;_dist;dist;4;0;Create;True;0;0;False;0;0.2;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2038.749,-152.7519;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2071.953,-551.2688;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-1867.924,-395.052;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-1319.452,448.3065;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;6;-959.0029,228.5064;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1542.255,-495.5654;Float;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-742.3916,-122.0129;Float;True;Property;_impact02_dust_FX;impact02_dust_FX;0;0;Create;True;0;0;False;0;bfd40f75cd8a4d74eb6e7e4d4ff2bbb0;bfd40f75cd8a4d74eb6e7e4d4ff2bbb0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;48;-1224.02,-361.3893;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-407.7589,-58.53121;Float;False;Property;_ColorMult;ColorMult;3;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-396.2271,-187.6246;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-219.3673,-168.4928;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;69;-36.79148,-81.71057;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;70;-53.03611,293.4876;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;11;-467.9536,286.265;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;170.7144,434.06;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;219.7766,136.6587;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-1264.674,708.0937;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;74;552.6545,119.9168;Float;False;True;2;Float;ASEMaterialInspector;0;7;S_FireParticle;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;66;0;19;0
WireConnection;66;1;65;0
WireConnection;64;0;21;0
WireConnection;64;1;65;0
WireConnection;18;0;19;0
WireConnection;18;1;15;0
WireConnection;20;0;15;0
WireConnection;20;1;21;0
WireConnection;60;0;64;0
WireConnection;60;1;15;0
WireConnection;59;0;66;0
WireConnection;59;1;15;0
WireConnection;22;0;18;0
WireConnection;22;1;20;0
WireConnection;61;0;60;0
WireConnection;61;1;59;0
WireConnection;68;0;51;0
WireConnection;9;0;51;0
WireConnection;9;1;22;0
WireConnection;62;0;68;0
WireConnection;62;1;61;0
WireConnection;36;0;25;1
WireConnection;2;1;9;0
WireConnection;30;0;25;2
WireConnection;63;1;62;0
WireConnection;34;0;36;0
WireConnection;34;1;46;0
WireConnection;26;0;25;2
WireConnection;26;1;46;0
WireConnection;32;0;25;1
WireConnection;32;1;46;0
WireConnection;29;0;30;0
WireConnection;29;1;46;0
WireConnection;71;0;2;1
WireConnection;71;1;63;1
WireConnection;6;0;5;0
WireConnection;6;1;71;0
WireConnection;6;2;7;0
WireConnection;45;0;34;0
WireConnection;45;1;32;0
WireConnection;45;2;29;0
WireConnection;45;3;26;0
WireConnection;1;1;6;0
WireConnection;48;0;45;0
WireConnection;47;0;48;0
WireConnection;47;1;1;1
WireConnection;47;2;1;4
WireConnection;13;0;47;0
WireConnection;13;1;14;0
WireConnection;69;0;13;0
WireConnection;69;1;14;0
WireConnection;70;0;69;0
WireConnection;58;0;70;0
WireConnection;58;1;11;4
WireConnection;12;0;69;0
WireConnection;12;1;11;0
WireConnection;12;2;58;0
WireConnection;67;0;2;1
WireConnection;67;1;63;1
WireConnection;74;0;12;0
ASEEND*/
//CHKSM=ADEB0F4AE17B128D180B92F445E469EE36FC2FDB