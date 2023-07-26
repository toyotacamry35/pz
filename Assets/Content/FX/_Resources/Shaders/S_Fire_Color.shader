// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Colony_FX/Unique/S_Fire_Color"
{
	Properties
	{
		_noise("noise", 2D) = "white" {}
		_noise2("noise2", 2D) = "white" {}
		_fire("fire", 2D) = "white" {}
		_noiseScale("noiseScale", Float) = 1
		_noisePanSpeed("noisePanSpeed", Vector) = (0,-0.2,0,0)
		_noiseStrength("noiseStrength", Float) = 1
		_noise2Scale("noise2Scale", Float) = 1
		_noise2PanSpeed("noise2PanSpeed", Vector) = (0,-0.2,0,0)
		_noise2Strength("noise2Strength", Float) = 1
		_distIntensity("distIntensity", Float) = 1
		_emissiveStrength("emissiveStrength", Float) = 1
		_opacity("opacity", Float) = 1
		_Color0("Color 0", Color) = (0,0,0,0)
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , SrcAlpha OneMinusSrcAlpha
		BlendOp Add , Add
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Color0;
		uniform sampler2D _fire;
		uniform sampler2D _noise;
		uniform float _noiseScale;
		uniform float2 _noisePanSpeed;
		uniform float _noiseStrength;
		uniform sampler2D _noise2;
		uniform float _noise2Scale;
		uniform float2 _noise2PanSpeed;
		uniform float _noise2Strength;
		uniform float4 _fire_ST;
		uniform float _distIntensity;
		uniform float _emissiveStrength;
		uniform float _opacity;

		inline fixed4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return fixed4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_TexCoord20 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 uv_TexCoord6 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner4 = ( ( uv_TexCoord6 * _noiseScale ) + 1.0 * _Time.y * ( _noiseScale * _noisePanSpeed ));
			float2 uv_TexCoord41 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner44 = ( ( uv_TexCoord41 * _noise2Scale ) + 1.0 * _Time.y * ( _noise2Scale * _noise2PanSpeed ));
			float clampResult83 = clamp( ( ( tex2D( _noise, panner4 ).r * _noiseStrength ) + ( tex2D( _noise2, panner44 ).a * _noise2Strength ) ) , 0.0 , 1.0 );
			float2 uv2_TexCoord54 = i.uv2_texcoord2 * float2( 1,1 ) + float2( 0,0 );
			float2 uv_TexCoord16 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 uv_fire = i.uv_texcoord * _fire_ST.xy + _fire_ST.zw;
			float4 appendResult25 = (float4(uv_TexCoord20.x , ( uv_TexCoord20.y + ( ( ( clampResult83 + uv2_TexCoord54.x ) * ( uv_TexCoord16.y * tex2D( _fire, uv_fire ).b ) ) * ( _distIntensity * -1.0 ) ) ) , 0.0 , 0.0));
			float4 tex2DNode78 = tex2D( _fire, appendResult25.xy );
			float clampResult87 = clamp( tex2DNode78.r , 0.0 , 1.0 );
			float clampResult74 = clamp( tex2DNode78.g , 0.15 , 1.0 );
			o.Emission = ( ( ( ( _Color0 * 1.3 ) * clampResult87 ) + ( i.vertexColor * clampResult74 ) ) * _emissiveStrength ).rgb;
			float clampResult71 = clamp( ( ( i.vertexColor.a * ( tex2DNode78.g + 0.0 + tex2DNode78.r ) ) * _opacity ) , 0.0 , 1.0 );
			o.Alpha = clampResult71;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13901
2022;287;1614;809;5924.879;2692.966;7.154388;True;True
Node;AmplifyShaderEditor.CommentaryNode;84;-6956.356,-463.245;Float;False;2914.698;1690.906;UV Dist;20;83;8;6;41;39;40;5;9;42;43;7;44;4;47;36;80;82;81;79;37;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-6843.19,-413.245;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-6853.732,512.9136;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;5;-6870.13,365.7338;Float;False;Property;_noisePanSpeed;noisePanSpeed;4;0;0,-0.2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.Vector2Node;39;-6783.952,1066.661;Float;False;Property;_noise2PanSpeed;noise2PanSpeed;7;0;0,-0.2;0;3;FLOAT2;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;40;-6906.356,921.7192;Float;False;Property;_noise2Scale;noise2Scale;6;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-6895.813,-4.439571;Float;False;Property;_noiseScale;noiseScale;3;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-6483.599,-259.9742;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-6462.122,839.7043;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-6480.061,621.5906;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-6526.652,-0.6439621;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT2;0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;44;-6126.209,548.1115;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.PannerNode;4;-6157.679,-277.5868;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.RangedFloatNode;80;-5374.164,100.6957;Float;False;Property;_noiseStrength;noiseStrength;5;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;82;-5363.042,403.2306;Float;False;Property;_noise2Strength;noise2Strength;8;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;36;-5728.074,516.493;Float;True;Property;_noise2;noise2;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;47;-5706.633,-178.6971;Float;True;Property;_noise;noise;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-5170.607,524.1304;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-5205.164,-94.30435;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;86;-3446.282,117.7547;Float;False;1004.09;1220.894;UV Dist Mask;6;19;12;17;76;16;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-4673.335,104.307;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-3396.282,319.7615;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;85;-4003.261,-226.9122;Float;False;393.6599;581.7269;UV Dist Shift;2;48;54;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;21;-2916.274,923.7123;Float;True;Property;_fire;fire;2;0;None;False;white;Auto;0;1;SAMPLER2D
Node;AmplifyShaderEditor.SamplerNode;76;-3292.847,578.9078;Float;True;Property;_TextureSample1;Texture Sample 1;12;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ComponentMaskNode;17;-3139.38,355.0505;Float;False;True;True;True;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;83;-4210.658,109.5874;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;97;-2295.162,-473.9535;Float;False;1446.717;951.09;Dist mix;6;10;38;11;25;24;20;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;54;-3953.261,-176.9122;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-2924.023,451.9857;Float;False;2;2;0;FLOAT;1.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-2274.571,286.3523;Float;False;Property;_distIntensity;distIntensity;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-3844.601,101.8147;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2046.734,194.2974;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;-1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-2721.072,247.4774;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-1692.933,-423.9535;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1822.02,97.40633;Float;True;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1380.889,-77.97314;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.DynamicAppendNode;25;-1188.143,-99.46853;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4
Node;AmplifyShaderEditor.CommentaryNode;95;-95.93484,-447.1171;Float;False;1434.404;708.569;Color addition;10;90;94;93;87;88;91;74;34;45;46;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;94;313.2274,-176.6123;Float;False;Constant;_Float0;Float 0;13;0;1.3;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;78;-593.7772,46.86846;Float;True;Property;_TextureSample0;Texture Sample 0;11;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;96;-34.05834,492.0697;Float;False;1234.898;433.0741;opacity control;5;92;70;69;71;32;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;90;122.6664,-397.1171;Float;False;Property;_Color0;Color 0;12;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;533.6276,-342.0123;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleAddOpNode;92;15.94166,574.317;Float;False;3;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.VertexColorNode;31;135.2408,299.3352;Float;False;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;74;-45.93484,42.71232;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.15;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;87;-42.62006,-134.1865;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;642.1158,16.99565;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;645.5798,-105.1595;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;403.2544,560.5688;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;70;470.5521,810.1438;Float;False;Property;_opacity;opacity;11;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;91;943.8799,-61.78646;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;46;923.3696,146.4519;Float;False;Property;_emissiveStrength;emissiveStrength;10;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;664.9296,573.8858;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ClampOpNode;71;1031.84,542.0697;Float;False;3;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1169.469,-9.7481;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;1410.579,246.8404;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Colony_FX/S_Fire_Color;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;2;SrcAlpha;OneMinusSrcAlpha;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;6;0
WireConnection;9;1;8;0
WireConnection;43;0;40;0
WireConnection;43;1;39;0
WireConnection;42;0;41;0
WireConnection;42;1;40;0
WireConnection;7;0;8;0
WireConnection;7;1;5;0
WireConnection;44;0;42;0
WireConnection;44;2;43;0
WireConnection;4;0;9;0
WireConnection;4;2;7;0
WireConnection;36;1;44;0
WireConnection;47;1;4;0
WireConnection;81;0;36;4
WireConnection;81;1;82;0
WireConnection;79;0;47;1
WireConnection;79;1;80;0
WireConnection;37;0;79;0
WireConnection;37;1;81;0
WireConnection;76;0;21;0
WireConnection;17;0;16;2
WireConnection;83;0;37;0
WireConnection;12;0;17;0
WireConnection;12;1;76;3
WireConnection;48;0;83;0
WireConnection;48;1;54;1
WireConnection;38;0;11;0
WireConnection;19;0;48;0
WireConnection;19;1;12;0
WireConnection;10;0;19;0
WireConnection;10;1;38;0
WireConnection;24;0;20;2
WireConnection;24;1;10;0
WireConnection;25;0;20;1
WireConnection;25;1;24;0
WireConnection;78;0;21;0
WireConnection;78;1;25;0
WireConnection;93;0;90;0
WireConnection;93;1;94;0
WireConnection;92;0;78;2
WireConnection;92;2;78;1
WireConnection;74;0;78;2
WireConnection;87;0;78;1
WireConnection;34;0;31;0
WireConnection;34;1;74;0
WireConnection;88;0;93;0
WireConnection;88;1;87;0
WireConnection;32;0;31;4
WireConnection;32;1;92;0
WireConnection;91;0;88;0
WireConnection;91;1;34;0
WireConnection;69;0;32;0
WireConnection;69;1;70;0
WireConnection;71;0;69;0
WireConnection;45;0;91;0
WireConnection;45;1;46;0
WireConnection;2;2;45;0
WireConnection;2;9;71;0
ASEEND*/
//CHKSM=C38AE4EDF37ECCDC6B5BE22346F9088FE5C815B3