// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/S_SphereWaveFront"
{
	Properties
	{
		_ScaleDistortion("Scale Distortion", Range( 0 , 1)) = 0.24
		_Speed("Speed", Float) = 0
		_RingScale("RingScale", Float) = 60
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		GrabPass{ }

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
			#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
			#else
			#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
			#endif


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
				float4 ase_texcoord1 : TEXCOORD1;
			};

			ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
			uniform float _RingScale;
			uniform float _Speed;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _ScaleDistortion;
			inline float4 ASE_ComputeGrabScreenPos( float4 pos )
			{
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				float4 o = pos;
				o.y = pos.w * 0.5f;
				o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
				return o;
			}
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
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
				float4 screenPos = i.ase_texcoord;
				float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( screenPos );
				float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
				float2 uv0117 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( -0.5,-0.5 );
				float2 break122 = uv0117;
				float temp_output_109_0 = ( pow( 0.5 , 3.0 ) * 0.1 );
				float2 appendResult125 = (float2(( break122.x + temp_output_109_0 ) , break122.y));
				float2 break147 = appendResult125;
				float temp_output_33_0 = ( _Time.y * -_Speed );
				float temp_output_135_0 = sin( ( ( uv0117.x * uv0117.x * _RingScale ) + ( uv0117.y * uv0117.y * _RingScale ) + temp_output_33_0 ) );
				float3 appendResult116 = (float3(1.0 , 0.0 , ( ( sin( ( ( break147.x * break147.x * _RingScale ) + ( break147.y * break147.y * _RingScale ) + temp_output_33_0 ) ) - temp_output_135_0 ) * 2.0 )));
				float2 appendResult113 = (float2(break122.x , ( break122.y + temp_output_109_0 )));
				float2 break149 = appendResult113;
				float3 appendResult121 = (float3(0.0 , 1.0 , ( ( sin( ( ( break149.x * break149.x * _RingScale ) + ( break149.y * break149.y * _RingScale ) + temp_output_33_0 ) ) - temp_output_135_0 ) * 2.0 )));
				float3 normalizeResult124 = normalize( cross( appendResult116 , appendResult121 ) );
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth59 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( screenPos )));
				float distanceDepth59 = saturate( abs( ( screenDepth59 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 5.0 ) ) );
				float3 lerpResult152 = lerp( float3(0,0,1) , normalizeResult124 , saturate( distanceDepth59 ));
				float4 screenColor24 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( (ase_grabScreenPosNorm).xy + (( lerpResult152 * _ScaleDistortion )).xy ));
				
				
				finalColor = screenColor24;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16900
2065;276;1206;774;5073.436;2549.447;8.299267;True;True
Node;AmplifyShaderEditor.RangedFloatNode;129;-3679.968,2532.408;Float;False;Constant;_OffsetN;OffsetN;5;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;117;-4144.647,1334.542;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.5,-0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;108;-3549.174,2384.789;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;122;-3550.063,1660.856;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ScaleNode;109;-3367.387,2393.415;Float;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;107;-3157.908,2422.57;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;120;-3234.712,1658.036;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-3832.836,839.1229;Float;False;Property;_Speed;Speed;1;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;-3014.774,2400.952;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;125;-3081.908,1655.475;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;32;-3724.991,715.3168;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;149;-2833.383,2324.843;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;147;-2887.686,1698.276;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;29;-2992.633,1870.003;Float;False;Property;_RingScale;RingScale;3;0;Create;True;0;0;False;0;60;40;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;36;-3639.657,846.0198;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-2480.835,1733.571;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-3489.76,799.8779;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-2471.018,1957.752;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;-2674.01,1122.364;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;-2459.107,2517.576;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-2468.924,2293.395;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-2664.193,1346.545;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;144;-2178.66,2345.347;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-2383.747,1174.316;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;139;-2190.571,1785.523;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;145;-1933.554,2312.483;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;135;-2138.64,1141.452;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;140;-1945.465,1752.659;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-1443.193,1816.532;Float;False;Constant;_Float2;Float 2;16;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;119;-1476.298,1913.145;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;123;-1470.117,1680.311;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;61;-1120.468,698.26;Float;False;599.6792;234.597;Comment;2;59;60;SoftParticles;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;-1326.255,1926.258;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-1319.497,1704.021;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;116;-1092.023,1689.736;Float;False;FLOAT3;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1070.469,816.8572;Float;False;Constant;_Float6;Float 6;6;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;121;-1069.088,1906.571;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CrossProductOpNode;115;-934.84,1779.831;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DepthFade;59;-788.7894,748.26;Float;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;58;-45.79004,543.3544;Float;False;1493.868;479.0077;Comment;6;19;21;23;24;22;25;Normal Screeeen Grab;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalizeNode;124;-758.5928,1777.492;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;150;-588.7487,1271.84;Float;True;Constant;_Vector0;Vector 0;6;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;154;-588.5333,1027.905;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;174.4952,773.8634;Float;False;Property;_ScaleDistortion;Scale Distortion;0;0;Create;True;0;0;False;0;0.24;0.014;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;152;-249.6608,1421.018;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;601.8875,876.8497;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GrabScreenPosition;19;580.6165,593.3544;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;21;864.092,820.256;Float;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;22;840.0617,624.3214;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;56;317.4925,-2061.689;Float;False;932.325;483.3309;Comment;6;50;51;52;53;54;55;ViewDissapear;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;23;1064.702,681.3617;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;74;-1468.822,-1608.376;Float;False;1610.132;612.0602;Comment;9;72;69;70;67;66;65;73;68;71;ColorNoise;1,1,1,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;50;400.9461,-2011.689;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScreenColorNode;24;1234.586,687.4309;Float;False;Global;_ScreenGrab0;Screen Grab 0;-1;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;66;-469.5735,-1554.346;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.65;False;4;FLOAT;0.94;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;677.0966,-1694.358;Float;False;Constant;_Float8;Float 8;6;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;54;1084.818,-1828.205;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-1058.388,-1381.203;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;73;-1302.186,-1113.575;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;53;831.4396,-1874.41;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1071.586,-1131.316;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.46;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;65;-178.6903,-1558.376;Float;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;None;fb2cba6fd842c6543872d7e2e8f6243c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;52;598.5884,-1948.262;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;51;367.4925,-1831.637;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-1357.642,-1253.848;Float;False;Constant;_BubleNoiseScale;BubleNoiseScale;5;0;Create;True;0;0;False;0;0.02;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;67;-645.7772,-1391.747;Float;True;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;72;-1418.822,-1502.636;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;70;-797.1906,-1251.308;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1671.551,721.2192;Float;False;True;2;Float;ASEMaterialInspector;0;1;Custom/S_SphereWaveFront;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;108;0;129;0
WireConnection;122;0;117;0
WireConnection;109;0;108;0
WireConnection;107;0;122;1
WireConnection;107;1;109;0
WireConnection;120;0;122;0
WireConnection;120;1;109;0
WireConnection;113;0;122;0
WireConnection;113;1;107;0
WireConnection;125;0;120;0
WireConnection;125;1;122;1
WireConnection;149;0;113;0
WireConnection;147;0;125;0
WireConnection;36;0;35;0
WireConnection;138;0;147;0
WireConnection;138;1;147;0
WireConnection;138;2;29;0
WireConnection;33;0;32;0
WireConnection;33;1;36;0
WireConnection;137;0;147;1
WireConnection;137;1;147;1
WireConnection;137;2;29;0
WireConnection;133;0;117;1
WireConnection;133;1;117;1
WireConnection;133;2;29;0
WireConnection;142;0;149;1
WireConnection;142;1;149;1
WireConnection;142;2;29;0
WireConnection;143;0;149;0
WireConnection;143;1;149;0
WireConnection;143;2;29;0
WireConnection;132;0;117;2
WireConnection;132;1;117;2
WireConnection;132;2;29;0
WireConnection;144;0;143;0
WireConnection;144;1;142;0
WireConnection;144;2;33;0
WireConnection;134;0;133;0
WireConnection;134;1;132;0
WireConnection;134;2;33;0
WireConnection;139;0;138;0
WireConnection;139;1;137;0
WireConnection;139;2;33;0
WireConnection;145;0;144;0
WireConnection;135;0;134;0
WireConnection;140;0;139;0
WireConnection;119;0;145;0
WireConnection;119;1;135;0
WireConnection;123;0;140;0
WireConnection;123;1;135;0
WireConnection;118;0;119;0
WireConnection;118;1;130;0
WireConnection;114;0;123;0
WireConnection;114;1;130;0
WireConnection;116;2;114;0
WireConnection;121;2;118;0
WireConnection;115;0;116;0
WireConnection;115;1;121;0
WireConnection;59;0;60;0
WireConnection;124;0;115;0
WireConnection;154;0;59;0
WireConnection;152;0;150;0
WireConnection;152;1;124;0
WireConnection;152;2;154;0
WireConnection;95;0;152;0
WireConnection;95;1;25;0
WireConnection;21;0;95;0
WireConnection;22;0;19;0
WireConnection;23;0;22;0
WireConnection;23;1;21;0
WireConnection;24;0;23;0
WireConnection;66;0;67;0
WireConnection;54;0;53;0
WireConnection;68;0;72;0
WireConnection;68;1;71;0
WireConnection;53;0;52;0
WireConnection;53;1;55;0
WireConnection;69;0;73;0
WireConnection;65;1;66;0
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;67;0;70;0
WireConnection;70;0;68;0
WireConnection;70;1;69;0
WireConnection;1;0;24;0
ASEEND*/
//CHKSM=0418990BF8916577554C88BB3A30CA6C8C4E3780