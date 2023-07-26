// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/Templates/Unlit"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_smooth("smooth", Float) = 1
		_spec("spec", Float) = 1
		_Float1("Float 1", Range( 0 , 2)) = 2
		_offset("offset", Float) = 0.5
		_strength("strength", Float) = 0.5
		_skin_Color("skin_Color", Color) = (0,0,0,0)
		_internals_Color("internals_Color", Color) = (0.4811321,0.3245372,0.3245372,0)
		[HDR]_skinGlow_Color("skinGlow_Color", Color) = (1.672823,1.413365,1.208529,1)
		_grow_border_lenght("grow_border_lenght", Range( 0.13 , 1)) = 0.13
		_wire_Color("wire_Color", Color) = (0.764151,0.140575,0.140575,0)
		[Toggle]_wire_type("wire_type", Float) = 0
		_skinInside_Color("skinInside_Color", Color) = (0.4339623,0.1261842,0.0798327,0)
		_final_Color("final_Color", Color) = (0.2830189,0.2816839,0.2816839,0)
		[HDR]_disolveBorder_Color("disolveBorder_Color", Color) = (0,0,0,0)
		_noize_texture("noize_texture", 2D) = "white" {}
		_crack("crack", 2D) = "white" {}
		_Wire_texture("Wire_texture", 2D) = "white" {}
		_offset_multiply("offset_multiply", Float) = 1
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			half ASEVFace : VFACE;
			float2 uv2_texcoord2;
		};

		uniform sampler2D _noize_texture;
		uniform float4 _noize_texture_ST;
		uniform float _Float1;
		uniform float _offset_multiply;
		uniform sampler2D _Wire_texture;
		uniform float _offset;
		uniform float _strength;
		uniform sampler2D _crack;
		uniform float4 _skin_Color;
		uniform float4 _internals_Color;
		uniform float4 _wire_Color;
		uniform half _wire_type;
		uniform float4 _Wire_texture_ST;
		uniform float4 _final_Color;
		uniform float4 _skinInside_Color;
		uniform half _grow_border_lenght;
		uniform float4 _skinGlow_Color;
		uniform float4 _disolveBorder_Color;
		uniform float _spec;
		uniform float _smooth;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_noize_texture = v.texcoord * _noize_texture_ST.xy + _noize_texture_ST.zw;
			float4 tex2DNode86 = tex2Dlod( _noize_texture, float4( uv_noize_texture, 0, 0.0) );
			float temp_output_85_0 = ( ( _Float1 - 1.0 ) * 2.0 );
			float temp_output_87_0 = ( temp_output_85_0 - 1.0 );
			float2 appendResult96 = (float2(temp_output_87_0 , ( temp_output_87_0 + 0.08 )));
			float2 break99 = ( ( (tex2DNode86).rg + float2( 1,1 ) ) - saturate( appendResult96 ) );
			float temp_output_103_0 = saturate( ( saturate( break99.x ) - saturate( break99.y ) ) );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( temp_output_103_0 * ase_vertexNormal * _offset_multiply );
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 temp_output_2_0_g3 = i.uv_texcoord;
			float2 break6_g3 = temp_output_2_0_g3;
			float temp_output_25_0_g3 = ( pow( _offset , 3.0 ) * 0.1 );
			float2 appendResult8_g3 = (float2(( break6_g3.x + temp_output_25_0_g3 ) , break6_g3.y));
			float4 tex2DNode14_g3 = tex2D( _Wire_texture, temp_output_2_0_g3 );
			float temp_output_4_0_g3 = _strength;
			float3 appendResult13_g3 = (float3(1.0 , 0.0 , ( ( tex2D( _Wire_texture, appendResult8_g3 ).g - tex2DNode14_g3.g ) * temp_output_4_0_g3 )));
			float2 appendResult9_g3 = (float2(break6_g3.x , ( break6_g3.y + temp_output_25_0_g3 )));
			float3 appendResult16_g3 = (float3(0.0 , 1.0 , ( ( tex2D( _Wire_texture, appendResult9_g3 ).g - tex2DNode14_g3.g ) * temp_output_4_0_g3 )));
			float3 normalizeResult22_g3 = normalize( cross( appendResult13_g3 , appendResult16_g3 ) );
			float3 temp_output_118_0 = normalizeResult22_g3;
			float2 temp_output_125_0 = ( i.uv_texcoord * float2( 5,5 ) );
			float2 temp_output_2_0_g2 = temp_output_125_0;
			float2 break6_g2 = temp_output_2_0_g2;
			float temp_output_25_0_g2 = ( pow( 0.4 , 3.0 ) * 0.1 );
			float2 appendResult8_g2 = (float2(( break6_g2.x + temp_output_25_0_g2 ) , break6_g2.y));
			float4 tex2DNode14_g2 = tex2D( _crack, temp_output_2_0_g2 );
			float temp_output_4_0_g2 = 4.0;
			float3 appendResult13_g2 = (float3(1.0 , 0.0 , ( ( tex2D( _crack, appendResult8_g2 ).g - tex2DNode14_g2.g ) * temp_output_4_0_g2 )));
			float2 appendResult9_g2 = (float2(break6_g2.x , ( break6_g2.y + temp_output_25_0_g2 )));
			float3 appendResult16_g2 = (float3(0.0 , 1.0 , ( ( tex2D( _crack, appendResult9_g2 ).g - tex2DNode14_g2.g ) * temp_output_4_0_g2 )));
			float3 normalizeResult22_g2 = normalize( cross( appendResult13_g2 , appendResult16_g2 ) );
			float2 uv_noize_texture = i.uv_texcoord * _noize_texture_ST.xy + _noize_texture_ST.zw;
			float4 tex2DNode86 = tex2D( _noize_texture, uv_noize_texture );
			float temp_output_143_0 = ( 1.0 - tex2DNode86.r );
			float temp_output_142_0 = saturate( (0.0 + (temp_output_143_0 - saturate( (1.0 + (_Float1 - 1.3) * (0.0 - 1.0) / (1.7 - 1.3)) )) * (1.0 - 0.0) / (1.0 - saturate( (1.0 + (_Float1 - 1.3) * (0.0 - 1.0) / (1.7 - 1.3)) ))) );
			float3 lerpResult126 = lerp( temp_output_118_0 , BlendNormals( temp_output_118_0 , normalizeResult22_g2 ) , temp_output_142_0);
			o.Normal = lerpResult126;
			float4 lerpResult121 = lerp( _skin_Color , _internals_Color , i.vertexColor.r);
			float2 uv_Wire_texture = i.uv_texcoord * _Wire_texture_ST.xy + _Wire_texture_ST.zw;
			float4 tex2DNode1 = tex2D( _Wire_texture, uv_Wire_texture );
			float4 lerpResult73 = lerp( lerpResult121 , _wire_Color , lerp(tex2DNode1.r,tex2DNode1.g,_wire_type));
			float temp_output_85_0 = ( ( _Float1 - 1.0 ) * 2.0 );
			float temp_output_84_0 = saturate( temp_output_85_0 );
			float4 lerpResult82 = lerp( lerpResult73 , _final_Color , temp_output_84_0);
			float4 switchResult6 = (((i.ASEVFace>0)?(lerpResult82):(_skinInside_Color)));
			float4 tex2DNode144 = tex2D( _crack, temp_output_125_0 );
			float4 lerpResult146 = lerp( switchResult6 , ( switchResult6 * tex2DNode144.r ) , temp_output_142_0);
			o.Albedo = lerpResult146.rgb;
			float temp_output_8_0 = ( 1.0 - i.uv2_texcoord2.y );
			float2 appendResult39 = (float2(temp_output_8_0 , temp_output_8_0));
			float temp_output_80_0 = saturate( _Float1 );
			float2 appendResult37 = (float2(temp_output_80_0 , ( temp_output_80_0 - 0.1 )));
			float2 temp_cast_1 = (0.1).xx;
			float2 appendResult40 = (float2(_grow_border_lenght , _grow_border_lenght));
			float2 temp_output_25_0 = (-appendResult40 + (( 1.0 - appendResult37 ) - temp_cast_1) * (float2( 1,1 ) - -appendResult40) / (float2( 1,1 ) - temp_cast_1));
			float2 temp_cast_2 = (0.1).xx;
			float2 break41 = saturate( ( (float2( 0,0 ) + (appendResult39 - temp_output_25_0) * (float2( 1,1 ) - float2( 0,0 )) / (( temp_output_25_0 + appendResult40 ) - temp_output_25_0)) + tex2DNode1.r ) );
			float temp_output_87_0 = ( temp_output_85_0 - 1.0 );
			float2 appendResult96 = (float2(temp_output_87_0 , ( temp_output_87_0 + 0.08 )));
			float2 break99 = ( ( (tex2DNode86).rg + float2( 1,1 ) ) - saturate( appendResult96 ) );
			float temp_output_103_0 = saturate( ( saturate( break99.x ) - saturate( break99.y ) ) );
			float temp_output_91_0 = floor( _Float1 );
			float4 lerpResult101 = lerp( float4( ( ( break41.x - break41.y ) * (_skinGlow_Color).rgb ) , 0.0 ) , ( _disolveBorder_Color * temp_output_103_0 ) , temp_output_91_0);
			o.Emission = lerpResult101.rgb;
			float3 temp_cast_5 = (_spec).xxx;
			o.Specular = temp_cast_5;
			float lerpResult76 = lerp( _smooth , 0.97 , saturate( (0.0 + (tex2DNode1.r - 0.72) * (1.0 - 0.0) / (1.0 - 0.72)) ));
			float lerpResult114 = lerp( lerpResult76 , 0.2 , temp_output_84_0);
			o.Smoothness = lerpResult114;
			o.Alpha = 1;
			float lerpResult89 = lerp( ( break41.x + 0.0 ) , break99.x , temp_output_91_0);
			float lerpResult151 = lerp( lerpResult89 , ( lerpResult89 * tex2DNode144.r ) , saturate( (0.0 + (temp_output_143_0 - saturate( (1.0 + (_Float1 - 1.45) * (0.0 - 1.0) / (1.7 - 1.45)) )) * (1.0 - 0.0) / (1.0 - saturate( (1.0 + (_Float1 - 1.45) * (0.0 - 1.0) / (1.7 - 1.45)) ))) ));
			clip( lerpResult151 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
2154;326;1580;729;-464.3981;378.5762;2.124533;True;True
Node;AmplifyShaderEditor.RangedFloatNode;4;-2689.949,6.454021;Float;False;Property;_Float1;Float 1;5;0;Create;True;0;0;False;0;2;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2254.365,1158.229;Float;False;Constant;_Float2;Float 2;8;0;Create;True;0;0;False;0;0.1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;80;-2134.629,982.062;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-1912.53,1141.31;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-2284.171,1372.783;Half;False;Property;_grow_border_lenght;grow_border_lenght;11;0;Create;True;0;0;False;0;0.13;0;0.13;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;83;-1798.107,-440.2224;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;37;-1708.694,1116.172;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;-1772.368,1342.96;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-1656.644,-440.8002;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;7;-1562.809,891.1102;Float;False;1;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;87;-1557.738,-860.6661;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;26;-1559.595,1316.21;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;33;-1522.249,1141.822;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;25;-1318.595,1213.21;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0.1,0.1;False;2;FLOAT2;1,1;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;8;-1296.163,972.9294;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;-1387.888,-853.4708;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.08;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;86;-473.4143,-1266.568;Float;True;Property;_noize_texture;noize_texture;17;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;96;-1245.987,-960.2584;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-2230.76,155.3394;Float;True;Property;_Wire_texture;Wire_texture;19;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1018.115,1003.076;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-1083.923,1370.13;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;98;166.9087,-1153.358;Float;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;88;-1000.14,-1045.678;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;367.0736,-1064.221;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1889.892,-137.8327;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;12;-898.4083,1180.883;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,0;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-226.7032,1171.477;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;93;663.002,-929.8995;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.VertexColorNode;120;-1203.931,-208.924;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;-1035.138,-159.4621;Float;False;Property;_skin_Color;skin_Color;8;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;119;-1017.547,-364.7152;Float;False;Property;_internals_Color;internals_Color;9;0;Create;True;0;0;False;0;0.4811321,0.3245372,0.3245372,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;117;-1175.09,46.07594;Half;False;Property;_wire_type;wire_type;13;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;15;-58.84612,1153.684;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;121;-684.9305,-90.52399;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;74;-1073.509,324.588;Float;False;Property;_wire_Color;wire_Color;12;0;Create;True;0;0;False;0;0.764151,0.140575,0.140575,0;0.764151,0.140575,0.140575,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;99;832.9283,-698.5956;Float;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;73;-372.3528,-61.50098;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;41;110.5239,1143.396;Float;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexCoordVertexDataNode;30;-2115.928,342.1919;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;77;-452.0013,307.1338;Float;False;Property;_final_Color;final_Color;15;0;Create;True;0;0;False;0;0.2830189,0.2816839,0.2816839,0;0.2830189,0.2816839,0.2816839,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;84;-1468.739,-440.7513;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;149;-1977.073,207.2521;Float;False;5;0;FLOAT;0;False;1;FLOAT;1.45;False;2;FLOAT;1.7;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;133;-1985.787,38.36915;Float;False;5;0;FLOAT;0;False;1;FLOAT;1.3;False;2;FLOAT;1.7;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;106;1161.85,-771.533;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;107;1167.976,-654.0565;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;82;-44.5081,-49.73744;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-1884.688,688.3035;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;5,5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-1736.226,348.7192;Float;False;Property;_offset;offset;6;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;134;-1653.327,86.37235;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1755.309,431.1595;Float;False;Property;_strength;strength;7;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;123;-2233.302,582.455;Float;True;Property;_crack;crack;18;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.OneMinusNode;143;-156.4843,-1010.664;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;51;778.4509,1166.326;Float;False;Property;_skinGlow_Color;skinGlow_Color;10;1;[HDR];Create;True;0;0;False;0;1.672823,1.413365,1.208529,1;1.672823,1.413365,1.208529,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;100;1485.209,-593.9953;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;150;-1664.773,192.4792;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;91;-941.4025,-681.6592;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;408.3167,1108.969;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;71;-26.97382,302.0415;Float;False;Property;_skinInside_Color;skinInside_Color;14;0;Create;True;0;0;False;0;0.4339623,0.1261842,0.0798327,0;0.4339623,0.1261842,0.0798327,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;72;-1514.486,-317.5103;Float;True;5;0;FLOAT;0;False;1;FLOAT;0.72;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;141;198.1169,-829.8238;Float;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;103;1663.802,-514.246;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;109;1499.961,-806.8361;Float;False;Property;_disolveBorder_Color;disolveBorder_Color;16;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;148;215.9722,-653.0976;Float;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;75;-409.5481,-261.2499;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;403.6741,1250.311;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;89;1413.928,264.6497;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;118;-1327.724,203.7423;Float;False;NormalCreate;1;;3;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;2;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;124;-1497.109,610.5522;Float;False;NormalCreate;1;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.4;False;4;FLOAT;4;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwitchByFaceNode;6;306.0072,34.18883;Float;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;2020.993,-614.9977;Float;False;Property;_smooth;smooth;3;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;144;-1269.315,730.3958;Float;True;Property;_TextureSample1;Texture Sample 1;20;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;53;1101.521,317.247;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;76;2190.152,-448.5877;Float;False;3;0;FLOAT;0.5;False;1;FLOAT;0.97;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;142;498.6151,-612.1715;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;147;1479.938,568.2892;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;1451.453,-103.0948;Float;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;326.5066,557.1829;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;111;1720.214,405.3152;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;1843.957,-535.1943;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;152;477.1575,-499.1621;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;135;-671.2969,635.4076;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;153;1704.988,600.1089;Float;False;Property;_offset_multiply;offset_multiply;20;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;2007.981,312.8582;Float;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;126;709.9818,211.4601;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;-938.0498,-563.2542;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;130;-1476.088,487.0903;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;101;2067.859,-73.45621;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;146;776.1975,28.65702;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;129;-1522.031,315.1393;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;2319.555,-112.0549;Float;False;Property;_spec;spec;4;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;151;1681.542,241.7357;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;114;2383.532,-356.3318;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.2;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;2865.098,35.02147;Float;False;True;2;Float;ASEMaterialInspector;0;0;StandardSpecular;Hidden/Templates/Unlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;80;0;4;0
WireConnection;38;0;80;0
WireConnection;38;1;19;0
WireConnection;83;0;4;0
WireConnection;37;0;80;0
WireConnection;37;1;38;0
WireConnection;40;0;66;0
WireConnection;40;1;66;0
WireConnection;85;0;83;0
WireConnection;87;0;85;0
WireConnection;26;0;40;0
WireConnection;33;0;37;0
WireConnection;25;0;33;0
WireConnection;25;1;19;0
WireConnection;25;3;26;0
WireConnection;8;0;7;2
WireConnection;94;0;87;0
WireConnection;96;0;87;0
WireConnection;96;1;94;0
WireConnection;39;0;8;0
WireConnection;39;1;8;0
WireConnection;18;0;25;0
WireConnection;18;1;40;0
WireConnection;98;0;86;0
WireConnection;88;0;96;0
WireConnection;92;0;98;0
WireConnection;1;0;29;0
WireConnection;12;0;39;0
WireConnection;12;1;25;0
WireConnection;12;2;18;0
WireConnection;13;0;12;0
WireConnection;13;1;1;1
WireConnection;93;0;92;0
WireConnection;93;1;88;0
WireConnection;117;0;1;1
WireConnection;117;1;1;2
WireConnection;15;0;13;0
WireConnection;121;0;34;0
WireConnection;121;1;119;0
WireConnection;121;2;120;1
WireConnection;99;0;93;0
WireConnection;73;0;121;0
WireConnection;73;1;74;0
WireConnection;73;2;117;0
WireConnection;41;0;15;0
WireConnection;84;0;85;0
WireConnection;149;0;4;0
WireConnection;133;0;4;0
WireConnection;106;0;99;0
WireConnection;107;0;99;1
WireConnection;82;0;73;0
WireConnection;82;1;77;0
WireConnection;82;2;84;0
WireConnection;125;0;30;0
WireConnection;134;0;133;0
WireConnection;143;0;86;1
WireConnection;100;0;106;0
WireConnection;100;1;107;0
WireConnection;150;0;149;0
WireConnection;91;0;4;0
WireConnection;43;0;41;0
WireConnection;72;0;1;1
WireConnection;141;0;143;0
WireConnection;141;1;134;0
WireConnection;103;0;100;0
WireConnection;148;0;143;0
WireConnection;148;1;150;0
WireConnection;75;0;72;0
WireConnection;42;0;41;0
WireConnection;42;1;41;1
WireConnection;89;0;43;0
WireConnection;89;1;99;0
WireConnection;89;2;91;0
WireConnection;118;1;29;0
WireConnection;118;2;30;0
WireConnection;118;3;31;0
WireConnection;118;4;32;0
WireConnection;124;1;123;0
WireConnection;124;2;125;0
WireConnection;6;0;82;0
WireConnection;6;1;71;0
WireConnection;144;0;123;0
WireConnection;144;1;125;0
WireConnection;53;0;51;0
WireConnection;76;0;35;0
WireConnection;76;2;75;0
WireConnection;142;0;141;0
WireConnection;147;0;89;0
WireConnection;147;1;144;1
WireConnection;50;0;42;0
WireConnection;50;1;53;0
WireConnection;145;0;6;0
WireConnection;145;1;144;1
WireConnection;110;0;109;0
WireConnection;110;1;103;0
WireConnection;152;0;148;0
WireConnection;135;0;118;0
WireConnection;135;1;124;0
WireConnection;112;0;103;0
WireConnection;112;1;111;0
WireConnection;112;2;153;0
WireConnection;126;0;118;0
WireConnection;126;1;135;0
WireConnection;126;2;142;0
WireConnection;131;0;84;0
WireConnection;131;1;84;0
WireConnection;131;2;84;0
WireConnection;131;3;84;0
WireConnection;130;0;32;0
WireConnection;130;2;84;0
WireConnection;101;0;50;0
WireConnection;101;1;110;0
WireConnection;101;2;91;0
WireConnection;146;0;6;0
WireConnection;146;1;145;0
WireConnection;146;2;142;0
WireConnection;129;0;31;0
WireConnection;129;2;84;0
WireConnection;151;0;89;0
WireConnection;151;1;147;0
WireConnection;151;2;152;0
WireConnection;114;0;76;0
WireConnection;114;2;84;0
WireConnection;2;0;146;0
WireConnection;2;1;126;0
WireConnection;2;2;101;0
WireConnection;2;3;3;0
WireConnection;2;4;114;0
WireConnection;2;10;151;0
WireConnection;2;11;112;0
ASEEND*/
//CHKSM=93BC20553EDA94F0B1E69C176373F7451BA13039