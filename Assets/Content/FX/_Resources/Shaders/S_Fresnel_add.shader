// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fresnel_add"
{
	Properties
	{
		_ColorMult("ColorMult", Float) = 1
		_FresnelScale("FresnelScale", Float) = 1.05
		_FresnelPower("FresnelPower", Float) = 0.28
		_FadeIn("FadeIn", Float) = 1
		_Distance2object("Distance2object", Float) = 1
		[Toggle]_InvertFresnel("InvertFresnel", Float) = 1
		[Toggle]_DistanceFade("DistanceFade", Float) = 1
		_SpeedU("SpeedU", Float) = 0
		_SpeedV("SpeedV", Float) = 0
		_Texture0("Texture 0", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_dephFade("dephFade", Float) = 5
		_niosecolor("niose/color", Float) = 0
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One , One One
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
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
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float3 ase_normal : NORMAL;
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
				float4 ase_texcoord3 : TEXCOORD3;
			};

			uniform float _ColorMult;
			uniform sampler2D _Texture0;
			uniform float _SpeedU;
			uniform float _SpeedV;
			uniform float4 _Texture0_ST;
			uniform float4 _Color;
			uniform float _niosecolor;
			uniform float _InvertFresnel;
			uniform float _FresnelScale;
			uniform float _FresnelPower;
			uniform float _DistanceFade;
			uniform float _Distance2object;
			uniform float _FadeIn;
			UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
			uniform float4 _CameraDepthTexture_TexelSize;
			uniform float _dephFade;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord1.xyz = ase_worldPos;
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord2.xyz = ase_worldNormal;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_color = v.color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				o.ase_texcoord1.w = 0;
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
				fixed4 finalColor;
				float2 appendResult50 = (float2(_SpeedU , _SpeedV));
				float2 uv_Texture0 = i.ase_texcoord.xy * _Texture0_ST.xy + _Texture0_ST.zw;
				float2 panner53 = ( _Time.y * ( appendResult50 * float2( 0.4,0.4 ) ) + ( uv_Texture0 + float2( 0.7,0.7 ) ));
				float2 panner43 = ( _Time.y * appendResult50 + uv_Texture0);
				float4 lerpResult62 = lerp( ( tex2D( _Texture0, panner53 ) * tex2D( _Texture0, panner43 ) ) , _Color , _niosecolor);
				float3 ase_worldPos = i.ase_texcoord1.xyz;
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 ase_worldNormal = i.ase_texcoord2.xyz;
				float fresnelNdotV3 = dot( ase_worldNormal, ase_worldViewDir );
				float fresnelNode3 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV3, _FresnelPower ) );
				float clampResult11 = clamp( fresnelNode3 , 0.0 , 1.0 );
				float clampResult34 = clamp( ( distance( ase_worldPos , _WorldSpaceCameraPos ) + ( 1.0 - _Distance2object ) ) , 0.0 , _FadeIn );
				float4 screenPos = i.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth59 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( screenPos ))));
				float distanceDepth59 = abs( ( screenDepth59 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _dephFade ) );
				float clampResult61 = clamp( distanceDepth59 , 0.0 , 1.0 );
				
				
				finalColor = ( ( _ColorMult * lerpResult62 * _Color * i.ase_color ) * lerp(clampResult11,( 1.0 - clampResult11 ),_InvertFresnel) * lerp(1.0,clampResult34,_DistanceFade) * clampResult61 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16301
1923;14;1904;1005;2031.967;974.4335;2.583869;True;True
Node;AmplifyShaderEditor.RangedFloatNode;47;-1672.089,-368.5857;Float;False;Property;_SpeedU;SpeedU;7;0;Create;True;0;0;False;0;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-1677.089,-258.5857;Float;False;Property;_SpeedV;SpeedV;8;0;Create;True;0;0;False;0;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-1640.089,-513.5857;Float;False;0;51;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;50;-1471.089,-340.5857;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1274.563,-630.4395;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.7,0.7;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1267.11,-512.1403;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.4,0.4;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;44;-1540.914,-158.0018;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;51;-1616.537,-937.8406;Float;True;Property;_Texture0;Texture 0;9;0;Create;True;0;0;False;0;4cdaee6b62156a741a76caffeb900dc7;6e4526d327c644f4fba970ade6a19b1d;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;53;-1001.345,-620.9725;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;29;-736.8494,518.947;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;20;-810.4595,661.4636;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;7;-1046.142,224.263;Float;False;Property;_FresnelScale;FresnelScale;1;0;Create;True;0;0;False;0;1.05;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-607.1583,834.7694;Float;False;Property;_Distance2object;Distance2object;4;0;Create;True;0;0;False;0;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1059.142,304.863;Float;False;Property;_FresnelPower;FresnelPower;2;0;Create;True;0;0;False;0;0.28;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;43;-1172.667,-353.7969;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-880.8413,-392.1369;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;36;-369.8221,829.9662;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;3;-837.8477,160.0395;Float;True;Standard;WorldNormal;ViewDir;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;26;-424.6299,641.4333;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;54;-773.8088,-647.4324;Float;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-438.6974,-319.0641;Float;False;Property;_niosecolor;niose/color;12;0;Create;True;0;0;False;0;0;0.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-141.2299,846.778;Float;False;Property;_FadeIn;FadeIn;3;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;11;-481.9416,146.9031;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;57;-518.8436,-221.0797;Float;False;Property;_Color;Color;10;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;60;471.9969,457.2881;Float;False;Property;_dephFade;dephFade;11;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-154.439,659.446;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-363.9091,-450.6298;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;34;20.88434,659.4462;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;62;-30.01953,-308.7888;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;5;-236.3415,-92.03711;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-64.94107,-472.7704;Float;False;Property;_ColorMult;ColorMult;0;0;Create;True;0;0;False;0;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;59;672.8799,442.3371;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-0.5057006,347.1131;Float;False;Constant;_Float1;Float 1;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;9;-297.3415,207.3628;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;242.9989,-255.7502;Float;False;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;61;963.9674,438.3828;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;41;-141.054,132.792;Float;False;Property;_InvertFresnel;InvertFresnel;5;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;42;216.3735,333.7647;Float;False;Property;_DistanceFade;DistanceFade;6;0;Create;True;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;1173.998,160.9435;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1511.582,169.169;Float;False;True;2;Float;ASEMaterialInspector;0;1;Fresnel_add;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;True;0;False;-1;0;False;-1;True;False;True;2;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;False;0;False;-1;0;False;-1;True;1;RenderType=Transparent=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;50;0;47;0
WireConnection;50;1;48;0
WireConnection;56;0;46;0
WireConnection;52;0;50;0
WireConnection;53;0;56;0
WireConnection;53;2;52;0
WireConnection;53;1;44;0
WireConnection;43;0;46;0
WireConnection;43;2;50;0
WireConnection;43;1;44;0
WireConnection;2;0;51;0
WireConnection;2;1;43;0
WireConnection;36;0;33;0
WireConnection;3;2;7;0
WireConnection;3;3;8;0
WireConnection;26;0;29;0
WireConnection;26;1;20;0
WireConnection;54;0;51;0
WireConnection;54;1;53;0
WireConnection;11;0;3;0
WireConnection;32;0;26;0
WireConnection;32;1;36;0
WireConnection;55;0;54;0
WireConnection;55;1;2;0
WireConnection;34;0;32;0
WireConnection;34;2;31;0
WireConnection;62;0;55;0
WireConnection;62;1;57;0
WireConnection;62;2;63;0
WireConnection;59;0;60;0
WireConnection;9;0;11;0
WireConnection;12;0;6;0
WireConnection;12;1;62;0
WireConnection;12;2;57;0
WireConnection;12;3;5;0
WireConnection;61;0;59;0
WireConnection;41;0;11;0
WireConnection;41;1;9;0
WireConnection;42;0;40;0
WireConnection;42;1;34;0
WireConnection;4;0;12;0
WireConnection;4;1;41;0
WireConnection;4;2;42;0
WireConnection;4;3;61;0
WireConnection;1;0;4;0
ASEEND*/
//CHKSM=CF4FC1D705BF33F58317B7632312FBAAAB6A943C