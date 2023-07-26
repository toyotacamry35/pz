// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/S_LitParticles_deep"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_SoftParticle("Soft Particle", Float) = 2
		_TintColor("TintColor", Color) = (1,1,1,1)
		_Float6("Float 6", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 4.0
		#pragma surface surf Unlit alpha:fade keepalpha vertex:vertexDataFunc finalcolor:MyFinalColorFunc
			#define DS_HAZE_FULL
#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_TransparentLib.cginc"

		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPosition87;
			DEEPSKY_HAZE_DECLARE_INPUT;
		};

		uniform float4 _TintColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Float6;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _SoftParticle;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 vertexPos87 = ase_vertex3Pos;
			float4 ase_screenPos87 = ComputeScreenPos( UnityObjectToClipPos( vertexPos87 ) );
			o.screenPosition87 = ase_screenPos87;
			DEEPSKY_HAZE_VERTEX_MOD(v, o);
		}

		inline void MyFinalColorFunc(Input i, SurfaceOutput o, inout fixed4 color) {
			DEEPSKY_HAZE_FINAL_COLOR(i, o, color);
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode11 = tex2D( _MainTex, uv_MainTex );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float grayscale44 = Luminance(ase_lightColor.rgb);
			float4 temp_output_152_0 = ( tex2DNode11 * (0.0 + (grayscale44 - 0.0) * (1.0 - 0.0) / (1.0 - 0.0)) * _Float6 * 3.0 );
			float4 color161 = IsGammaSpace() ? float4(0.4588235,0.5537153,0.6039216,1) : float4(0.1778884,0.2671703,0.3231432,1);
			float4 color159 = IsGammaSpace() ? float4(0.745283,0.6573959,0.4816216,1) : float4(0.5152035,0.3896956,0.197423,1);
			float4 lerpResult162 = lerp( color161 , color159 , temp_output_152_0);
			float temp_output_99_0 = ( _TintColor.a * tex2DNode11.a * 2.0 );
			float AlphaParticle132 = temp_output_99_0;
			float4 lerpResult169 = lerp( ( lerpResult162 * 0.2 ) , ( ( ( AlphaParticle132 * unity_AmbientSky ) * 0.5 ) + ( ( AlphaParticle132 * unity_AmbientEquator ) * 0.5 ) ) , 0.0);
			o.Emission = ( ( ( _TintColor * tex2DNode11 ) * i.vertexColor * 0.1 ) + temp_output_152_0 + lerpResult169 ).rgb;
			float4 ase_screenPos87 = i.screenPosition87;
			float4 ase_screenPosNorm87 = ase_screenPos87 / ase_screenPos87.w;
			ase_screenPosNorm87.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm87.z : ase_screenPosNorm87.z * 0.5 + 0.5;
			float screenDepth87 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos87 )));
			float distanceDepth87 = saturate( abs( ( screenDepth87 - LinearEyeDepth( ase_screenPosNorm87.z ) ) / ( _SoftParticle ) ) );
			o.Alpha = saturate( ( temp_output_99_0 * distanceDepth87 * i.vertexColor.a ) );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
2075;354;1206;774;-2491.846;-424.1219;1.331133;True;True
Node;AmplifyShaderEditor.SamplerNode;11;-74.18475,466.1626;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;67956e21c74b1d845a65da55cce7a6d8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;98;17.81689,181.547;Float;False;Property;_TintColor;TintColor;2;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;156;156.6887,724.2664;Float;False;Constant;_Float7;Float 7;4;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;414.1645,479.3254;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;22;-769.1289,1143.482;Float;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;133;-398.6112,806.225;Float;False;499.7424;419.1534;Comment;2;54;44;Sun Intensity;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;138;-134.5413,2394.57;Float;False;856.719;504.9458;Sky Ambient;5;110;109;123;124;137;Sky Ambient;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;132;625.1274,478.7957;Float;False;AlphaParticle;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;139;-139.7683,3077.351;Float;False;856.719;504.9458;Sky Ambient;5;144;143;142;141;140;Equator Ambient;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCGrayscale;44;-348.6113,966.3784;Float;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-53.50577,2444.57;Float;False;132;AlphaParticle;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;140;-89.76845,3234.316;Float;False;unity_AmbientEquator;0;1;COLOR;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;110;-84.54135,2551.535;Float;False;unity_AmbientSky;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-58.73293,3127.351;Float;False;132;AlphaParticle;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;165;890.4157,909.4108;Float;False;1004.044;450.7438;Comment;5;162;159;161;163;164;Color correction;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;153;164.9848,983.285;Float;False;Constant;_Float4;Float 4;3;0;Create;True;0;0;False;0;3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;167.967,893.4745;Float;False;Property;_Float6;Float 6;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;54;-104.8688,856.225;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;159;940.4157,1143.63;Float;False;Constant;_Color1;Color 1;4;0;Create;True;0;0;False;0;0.745283,0.6573959,0.4816216,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;161;944.1196,959.4109;Float;False;Constant;_Color2;Color 2;4;0;Create;True;0;0;False;0;0.4588235,0.5537153,0.6039216,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;143;206.4256,3466.297;Float;False;Constant;_Float3;Float 3;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;1081.825,2236.359;Float;False;588.7734;342.9888;Comment;3;89;88;87;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;124;211.6526,2783.516;Float;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;176.4467,2538.564;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;401.9236,812.0825;Float;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;171.2196,3221.346;Float;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;142;481.9507,3251.022;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;487.1776,2568.24;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;162;1398.408,1005.219;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;88;1131.825,2286.359;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;164;1449.318,1244.155;Float;False;Constant;_Float8;Float 8;4;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;1141.34,2463.347;Float;False;Property;_SoftParticle;Soft Particle;1;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;481.5441,206.1972;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;170;1981.236,1238.151;Float;False;Constant;_colortoambient;color to ambient;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;87;1402.599,2331.631;Float;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;163;1659.46,1039.711;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;167;873.0038,2799.148;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;154;1183.943,424.575;Float;False;Constant;_Float5;Float 5;3;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;105;899.7155,339.8927;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;2253.121,1571.157;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;169;2201.162,1139.426;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;1421.955,254.3963;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;134;72.40601,1685.121;Float;False;582.7979;560.2719;Comment;3;117;118;116;Moon Intensity;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;135;-744.8219,1421.225;Float;False;606.5187;304;Comment;2;106;107;Sun Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;108;2468.17,1572.601;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;148;2611.966,669.4687;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-694.8219,1548.724;Float;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;0.09760383;0.02811763;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;234.1864,1986.393;Float;True;Constant;_Float1;Float 1;6;0;Create;True;0;0;False;0;0.2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-373.3032,1471.225;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;116;122.406,1735.121;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;117;448.2039,1886.232;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;104;3199.754,638.7478;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Custom/S_LitParticles_deep;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;8;5;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;99;0;98;4
WireConnection;99;1;11;4
WireConnection;99;2;156;0
WireConnection;132;0;99;0
WireConnection;44;0;22;0
WireConnection;54;0;44;0
WireConnection;109;0;137;0
WireConnection;109;1;110;0
WireConnection;152;0;11;0
WireConnection;152;1;54;0
WireConnection;152;2;155;0
WireConnection;152;3;153;0
WireConnection;141;0;144;0
WireConnection;141;1;140;0
WireConnection;142;0;141;0
WireConnection;142;1;143;0
WireConnection;123;0;109;0
WireConnection;123;1;124;0
WireConnection;162;0;161;0
WireConnection;162;1;159;0
WireConnection;162;2;152;0
WireConnection;52;0;98;0
WireConnection;52;1;11;0
WireConnection;87;1;88;0
WireConnection;87;0;89;0
WireConnection;163;0;162;0
WireConnection;163;1;164;0
WireConnection;167;0;123;0
WireConnection;167;1;142;0
WireConnection;76;0;99;0
WireConnection;76;1;87;0
WireConnection;76;2;105;4
WireConnection;169;0;163;0
WireConnection;169;1;167;0
WireConnection;169;2;170;0
WireConnection;47;0;52;0
WireConnection;47;1;105;0
WireConnection;47;2;154;0
WireConnection;108;0;76;0
WireConnection;148;0;47;0
WireConnection;148;1;152;0
WireConnection;148;2;169;0
WireConnection;106;0;22;1
WireConnection;106;1;107;0
WireConnection;116;0;44;0
WireConnection;117;0;116;0
WireConnection;117;4;118;0
WireConnection;104;2;148;0
WireConnection;104;9;108;0
ASEEND*/
//CHKSM=2E429E34A8280A2153C51AA0443148D07F9E28D2