// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/SphereNoFogEmission"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_HighestCameraPoint("Highest Camera Point", Float) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_ColorTint("Color Tint", Color) = (1,0.6593853,0.01415092,0)
		_EdgeSpread("Edge Spread", Range( 0 , 1)) = 0
		_NoiseTile("Noise Tile", Float) = 0
		_EmissionHDR("EmissionHDR", Float) = 20
		_VertexOffsetClose("VertexOffsetClose", Float) = 0
		_VertexOffsetFar("VertexOffsetFar", Float) = 0
		_HeightCorrection("HeightCorrection", Float) = 17000
		_UpdownCorrection("UpdownCorrection", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha , One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
		};

		uniform float _VertexOffsetClose;
		uniform float _VertexOffsetFar;
		uniform float _HighestCameraPoint;
		uniform float4 _ColorTint;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _EmissionHDR;
		uniform float _NoiseTile;
		uniform float _HeightCorrection;
		uniform float _Opacity;
		uniform float _UpdownCorrection;
		uniform float _EdgeSpread;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 appendResult110 = (float3(ase_worldPos.x , 0.0 , 0.0));
			float3 appendResult107 = (float3(_WorldSpaceCameraPos.x , 0.0 , 0.0));
			float lerpResult111 = lerp( _VertexOffsetClose , _VertexOffsetFar , saturate( pow( (0.0 + (distance( appendResult110 , appendResult107 ) - 0.0) * (1.0 - 0.0) / (_HighestCameraPoint - 0.0)) , 1.0 ) ));
			float3 ase_vertexNormal = v.normal.xyz;
			float3 temp_output_52_0 = ( lerpResult111 * ase_vertexNormal );
			v.vertex.xyz += temp_output_52_0;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode2 = tex2D( _MainTex, uv_MainTex );
			o.Emission = (( ( _ColorTint * tex2DNode2 ) * _EmissionHDR )).rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float simplePerlin3D29 = snoise( ( ase_vertex3Pos * _NoiseTile ) );
			float3 ase_worldPos = i.worldPos;
			float3 appendResult110 = (float3(ase_worldPos.x , 0.0 , 0.0));
			float3 appendResult107 = (float3(_WorldSpaceCameraPos.x , 0.0 , 0.0));
			float lerpResult111 = lerp( _VertexOffsetClose , _VertexOffsetFar , saturate( pow( (0.0 + (distance( appendResult110 , appendResult107 ) - 0.0) * (1.0 - 0.0) / (_HighestCameraPoint - 0.0)) , 1.0 ) ));
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3 temp_output_52_0 = ( lerpResult111 * ase_vertexNormal );
			float temp_output_94_0 = ( saturate( ( ase_vertex3Pos.y + -_HeightCorrection ) ) - (temp_output_52_0).y );
			float temp_output_59_0 = (-_UpdownCorrection + (( 1.0 - _Opacity ) - 0.0) * (( _UpdownCorrection + _EdgeSpread ) - -_UpdownCorrection) / (1.0 - 0.0));
			float smoothstepResult21 = smoothstep( temp_output_94_0 , temp_output_59_0 , ( temp_output_59_0 + (0.0 + (_EdgeSpread - 0.0) * (-2.0 - 0.0) / (1.0 - 0.0)) ));
			float smoothstepResult40 = smoothstep( (0.26 + (simplePerlin3D29 - -1.0) * (1.0 - 0.26) / (1.0 - -1.0)) , 1.0 , ( 1.0 - ( smoothstepResult21 * step( temp_output_94_0 , temp_output_59_0 ) ) ));
			o.Alpha = saturate( ( tex2DNode2.a * smoothstepResult40 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
2065;276;1206;774;4192.891;379.6294;6.199348;True;True
Node;AmplifyShaderEditor.WorldSpaceCameraPos;109;-3570.39,2219.839;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;108;-3525.128,2050.168;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;110;-3279.929,2066.094;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;107;-3276.929,2235.095;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DistanceOpNode;104;-3102.431,2109.095;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-3114.392,2361.944;Float;False;Property;_HighestCameraPoint;Highest Camera Point;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-2727.151,2442.787;Float;False;Constant;_DistancePower;Distance Power;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;105;-2786.61,2217.522;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;2000;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;106;-2487.695,2255.352;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-2477.621,2002.755;Float;False;Property;_VertexOffsetFar;VertexOffsetFar;9;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;100;-2194.362,2274.489;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-2473.043,1875.423;Float;False;Property;_VertexOffsetClose;VertexOffsetClose;8;0;Create;True;0;0;False;0;0;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-2353.158,1057.903;Float;False;Property;_HeightCorrection;HeightCorrection;10;0;Create;True;0;0;False;0;17000;17000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1900.969,532.9661;Float;False;Property;_EdgeSpread;Edge Spread;5;0;Create;True;0;0;False;0;0;0.02;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1922.877,283.6196;Float;False;Property;_Opacity;Opacity;3;0;Create;True;0;0;False;0;0;0.229;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;126;-1990.74,1016.146;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;-1660.492,386.2612;Float;False;Property;_UpdownCorrection;UpdownCorrection;11;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;11;-2217.795,688.6649;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;111;-2025.924,2174.958;Float;False;3;0;FLOAT;0.01;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;51;-2136.756,1822.696;Float;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;123;-1808.354,905.8843;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;131;-1517.492,360.2612;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;129;-1579.085,257.0243;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-1518.315,456.0723;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1842.063,1756.82;Float;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;127;-1620.291,829.3649;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;59;-1333.811,333.0509;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;93;-1675.012,1367.695;Float;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;23;-1340.5,556.4662;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;-2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1399.922,1098.991;Float;False;Property;_NoiseTile;Noise Tile;6;0;Create;True;0;0;False;0;0;0.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;94;-1470.815,797.4774;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1080.928,598.9217;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;25;-689.2391,732.3311;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;21;-718.8826,463.8786;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1204.068,1010.904;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;8;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;29;-937.754,1027.981;Float;True;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-352.9171,648.9111;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-743.2504,140.651;Float;True;Property;_MainTex;Main Tex;0;0;Create;True;0;0;False;0;None;cb79392366cac814a8b9edce80b3e19b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;38;-592.646,1065.666;Float;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.26;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-690.0356,-75.89638;Float;False;Property;_ColorTint;Color Tint;4;0;Create;True;0;0;False;0;1,0.6593853,0.01415092,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;128;-105.0474,638.3951;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;40;72.94846,877.5641;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-265.0227,37.70288;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-185.9245,248.3158;Float;False;Property;_EmissionHDR;EmissionHDR;7;0;Create;True;0;0;False;0;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;193.2996,260.6602;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;516.5408,767.0261;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;18;758.387,730.8798;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;97;651.1041,316.031;Float;True;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;99;1219.745,441.765;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;Custom/SphereNoFogEmission;False;False;False;False;True;True;True;True;True;True;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Overlay;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;110;0;108;1
WireConnection;107;0;109;1
WireConnection;104;0;110;0
WireConnection;104;1;107;0
WireConnection;105;0;104;0
WireConnection;105;2;101;0
WireConnection;106;0;105;0
WireConnection;106;1;103;0
WireConnection;100;0;106;0
WireConnection;126;0;125;0
WireConnection;111;0;112;0
WireConnection;111;1;113;0
WireConnection;111;2;100;0
WireConnection;123;0;11;2
WireConnection;123;1;126;0
WireConnection;131;0;130;0
WireConnection;129;0;20;0
WireConnection;91;0;130;0
WireConnection;91;1;22;0
WireConnection;52;0;111;0
WireConnection;52;1;51;0
WireConnection;127;0;123;0
WireConnection;59;0;129;0
WireConnection;59;3;131;0
WireConnection;59;4;91;0
WireConnection;93;0;52;0
WireConnection;23;0;22;0
WireConnection;94;0;127;0
WireConnection;94;1;93;0
WireConnection;24;0;59;0
WireConnection;24;1;23;0
WireConnection;25;0;94;0
WireConnection;25;1;59;0
WireConnection;21;0;24;0
WireConnection;21;1;94;0
WireConnection;21;2;59;0
WireConnection;30;0;11;0
WireConnection;30;1;41;0
WireConnection;29;0;30;0
WireConnection;26;0;21;0
WireConnection;26;1;25;0
WireConnection;38;0;29;0
WireConnection;128;0;26;0
WireConnection;40;0;128;0
WireConnection;40;1;38;0
WireConnection;3;0;4;0
WireConnection;3;1;2;0
WireConnection;49;0;3;0
WireConnection;49;1;50;0
WireConnection;36;0;2;4
WireConnection;36;1;40;0
WireConnection;18;0;36;0
WireConnection;97;0;49;0
WireConnection;99;2;97;0
WireConnection;99;9;18;0
WireConnection;99;11;52;0
ASEEND*/
//CHKSM=68EC009DEADAB23CCEA1A9AAE8F9D7CC435A1697