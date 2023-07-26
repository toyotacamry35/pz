// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Lightning"
{
	Properties
	{
		_noize("noize", 2D) = "white" {}
		_amplitude_1("amplitude_1", Float) = 0
		_tile_1("tile_1", Float) = 0
		[HDR]_Color("Color", Color) = (0,1,0.8830578,0)
		_speed("speed", Float) = 0
		_speed_2("speed_2", Float) = 1
		_blink_freq("blink_freq", Float) = 1
		_blink_power("blink_power", Range( 0 , 0.84)) = 0
		_arc("arc", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_distort("distort", Float) = 0.23
		_distort_speed("distort_speed", Float) = 0
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend One One , One One
		BlendOp Add , Add
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
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			uniform sampler2D _noize;
			uniform float _tile_1;
			uniform float _speed;
			uniform float _speed_2;
			uniform float _amplitude_1;
			uniform float _arc;
			uniform sampler2D _TextureSample0;
			uniform float4 _noize_ST;
			uniform float _distort_speed;
			uniform float _distort;
			uniform float4 _Color;
			uniform float _blink_freq;
			uniform float _blink_power;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 uv91 = v.ase_texcoord * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult92 = (float2(uv91.x , 0.0));
				float2 appendResult27 = (float2(0.0 , ( _speed * _Time.y )));
				float2 break30 = ( 1.0 - abs( (float2( -1,-1 ) + (uv91 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) );
				float temp_output_31_0 = sqrt( break30.x );
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				float3 vertexValue = ( ( (-1.0 + (tex2Dlod( _noize, float4( ( ( appendResult92 * _tile_1 ) + appendResult27 + ( step( frac( ( 1.3 * _Time.y * _speed_2 ) ) , 0.5 ) * 0.3 ) ), 0, 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * float3(0,1,0) * _amplitude_1 * temp_output_31_0 ) + ( float3(1,0,0) * _amplitude_1 * (-1.0 + (tex2Dlod( _noize, float4( ( -appendResult27 + ( ( appendResult92 + float2( 0.5,0.5 ) ) * _tile_1 ) + ( step( frac( ( _Time.y * 0.8 * _speed_2 ) ) , 0.5 ) * 0.3 ) ), 0, 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * temp_output_31_0 ) + ( temp_output_31_0 * _arc * float3(1,1,1) ) + ( (-1.0 + (tex2Dlod( _noize, float4( ( ( ( appendResult92 + float2( 0.3,0.3 ) ) * _tile_1 ) + appendResult27 + ( step( frac( ( _Time.y * 1.3 * _speed_2 ) ) , 0.5 ) * 0.3 ) ), 0, 0.0) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * float3(0,0,1) * _amplitude_1 * temp_output_31_0 ) );
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
				float2 uv_noize = i.ase_texcoord.xy * _noize_ST.xy + _noize_ST.zw;
				float2 appendResult149 = (float2(_distort_speed , 0.0));
				float2 panner143 = ( _Time.y * appendResult149 + uv_noize);
				float2 lerpResult141 = lerp( uv_noize , ( (float2( 0,0 ) + (( uv_noize + ( tex2D( _noize, panner143 ).r * 1.0 ) ) - float2( 0,0 )) * (float2( 1,1 ) - float2( 0,0 )) / (float2( 1,1 ) - float2( 0,0 ))) * 0.5 ) , _distort);
				float2 uv91 = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 break30 = ( 1.0 - abs( (float2( -1,-1 ) + (uv91 - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 ))) ) );
				float2 appendResult66 = (float2(frac( ( _Time.y * _blink_freq ) ) , 0.5));
				
				
				finalColor = ( tex2D( _TextureSample0, lerpResult141 ) * ( _Color * ( break30.y * break30.y * break30.y ) * saturate( (0.0 + (tex2D( _noize, appendResult66 ).r - _blink_power) * (1.0 - 0.0) / (0.85 - _blink_power)) ) ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16301
1920;0;1920;1019;918.6404;-581.4873;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;145;-1885.661,-666.7953;Float;False;Property;_distort_speed;distort_speed;11;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-3353.573,965.2177;Float;False;Property;_speed_2;speed_2;5;0;Create;True;0;0;False;0;1;0.46;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;24;-3490.782,747.9764;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;149;-1598.343,-740.9126;Float;False;FLOAT2;4;0;FLOAT;10;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;63;-2203.011,-448.2933;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;131;-1754.191,-994.149;Float;True;0;32;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-2874.279,1395.289;Float;False;3;3;0;FLOAT;1.18;False;1;FLOAT;1.3;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2185.871,-331.8486;Float;False;Property;_blink_freq;blink_freq;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;91;-2856.092,67.19805;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-2911.99,1048.063;Float;False;3;3;0;FLOAT;1.18;False;1;FLOAT;0.8;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;143;-1300.532,-824.5023;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;32;-2042.742,82.55782;Float;True;Property;_noize;noize;0;0;Create;True;0;0;False;0;125b1e443ce055a4aae6057287221ea4;125b1e443ce055a4aae6057287221ea4;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-2901.597,821.1141;Float;False;3;3;0;FLOAT;1.3;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-3244.362,593.6331;Float;False;Property;_speed;speed;4;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;160;-2712.14,1399.884;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;104;-2749.851,1052.658;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;96;-2734.718,819.7326;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-1970.728,-354.2341;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;-2503.615,219.285;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-2834.554,599.9978;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;133;-1016.023,-705.787;Float;True;Property;_TextureSample3;Texture Sample 3;11;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;101;-2547.585,820.4116;Float;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2231.351,930.9633;Float;False;Property;_tile_1;tile_1;2;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;64;-1805.375,-352.9696;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-1860.331,702.741;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-2567.874,581.8586;Float;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StepOpNode;161;-2514.089,1399.968;Float;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;105;-2551.8,1052.742;Float;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;17;-1207.268,-109.5206;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-658.4543,-678.386;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;163;-1881.421,1173.791;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.3,0.3;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;66;-1626.558,-356.0832;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2312.89,816.5473;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-1688.362,711.3079;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-2275.14,1397.616;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-2312.851,1050.39;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;89;-1607.723,604.6379;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1933.504,319.7318;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;18;-998.9396,-122.9797;Float;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;136;-452.239,-737.3044;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-1604.359,1154.808;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-327.0368,-884.6011;Float;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;139;-177.285,-688.2051;Float;True;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;0,0;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-1330.413,676.7949;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;121;-1409.734,-198.8301;Float;False;Property;_blink_power;blink_power;7;0;Create;True;0;0;False;0;0;0.344;0;0.84;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;19;-796.6381,-114.9469;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;155;-1276.659,1149.008;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;65;-1427.238,-397.1086;Float;True;Property;_TextureSample1;Texture Sample 1;9;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-1535.228,394.0521;Float;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;30;-633.3965,-115.5481;Float;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;71;-1100.186,588.6147;Float;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;False;0;125b1e443ce055a4aae6057287221ea4;125b1e443ce055a4aae6057287221ea4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;69;-1047.596,-371.5771;Float;True;5;0;FLOAT;0;False;1;FLOAT;0.14;False;2;FLOAT;0.85;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-162.9412,-1075.276;Float;False;Property;_distort;distort;10;0;Create;True;0;0;False;0;0.23;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-29.2319,-1001.169;Float;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-1128.55,324.4474;Float;True;Property;_noize_;noize_;0;0;Create;True;0;0;False;0;125b1e443ce055a4aae6057287221ea4;125b1e443ce055a4aae6057287221ea4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;152;-1006.858,976.1078;Float;True;Property;_TextureSample4;Texture Sample 4;12;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;75;-472.6787,844.0026;Float;False;Constant;_Vector2;Vector 2;7;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;156;-558.9131,1041.609;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;158;-205.5855,1187.356;Float;False;Constant;_Vector3;Vector 3;12;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;86;17.18018,261.8106;Float;False;Constant;_Vector1;Vector 1;7;0;Create;True;0;0;False;0;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;20;-97.46921,-359.2145;Float;False;Property;_Color;Color;3;1;[HDR];Create;True;0;0;False;0;0,1,0.8830578,0;216.8471,244.9569,766.9961,0.9921569;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;70;-716.3763,-373.7495;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-203.4829,-111.987;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;3;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;16.96557,185.9246;Float;False;Property;_arc;arc;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;76;-728.287,602.2665;Float;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SqrtOpNode;31;-380.2274,66.33947;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;52;-329.7609,457.2675;Float;False;Constant;_Vector0;Vector 0;7;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;141;254.7856,-1166.92;Float;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-375.7937,648.2195;Float;False;Property;_amplitude_1;amplitude_1;1;0;Create;True;0;0;False;0;0;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;22;-760.0782,371.0589;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;82.96558,1041.609;Float;True;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;82.20245,408.071;Float;True;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;126;768.492,-639.868;Float;True;Property;_TextureSample0;Texture Sample 0;9;0;Create;True;0;0;False;0;None;f3674d34420252541b14b9017cc6bf54;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;486.856,-158.9229;Float;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;333.9983,114.6164;Float;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;65.73035,787.2981;Float;True;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;745.2729,425.4306;Float;True;4;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;929.5423,-184.2509;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;129;1212.848,340.4699;Float;False;True;2;Float;ASEMaterialInspector;0;1;Lightning;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;True;1;False;-1;1;False;-1;True;False;True;2;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;3;False;-1;True;False;0;False;-1;0;False;-1;True;1;RenderType=Transparent=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;149;0;145;0
WireConnection;159;0;24;0
WireConnection;159;2;107;0
WireConnection;103;0;24;0
WireConnection;103;2;107;0
WireConnection;143;0;131;0
WireConnection;143;2;149;0
WireConnection;143;1;63;0
WireConnection;93;1;24;0
WireConnection;93;2;107;0
WireConnection;160;0;159;0
WireConnection;104;0;103;0
WireConnection;96;0;93;0
WireConnection;67;0;63;0
WireConnection;67;1;68;0
WireConnection;92;0;91;1
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;133;0;32;0
WireConnection;133;1;143;0
WireConnection;101;0;96;0
WireConnection;64;0;67;0
WireConnection;72;0;92;0
WireConnection;27;1;26;0
WireConnection;161;0;160;0
WireConnection;105;0;104;0
WireConnection;17;0;91;0
WireConnection;134;0;133;1
WireConnection;163;0;92;0
WireConnection;66;0;64;0
WireConnection;102;0;101;0
WireConnection;73;0;72;0
WireConnection;73;1;46;0
WireConnection;162;0;161;0
WireConnection;106;0;105;0
WireConnection;89;0;27;0
WireConnection;47;0;92;0
WireConnection;47;1;46;0
WireConnection;18;0;17;0
WireConnection;136;0;131;0
WireConnection;136;1;134;0
WireConnection;154;0;163;0
WireConnection;154;1;46;0
WireConnection;139;0;136;0
WireConnection;74;0;89;0
WireConnection;74;1;73;0
WireConnection;74;2;106;0
WireConnection;19;0;18;0
WireConnection;155;0;154;0
WireConnection;155;1;27;0
WireConnection;155;2;162;0
WireConnection;65;0;32;0
WireConnection;65;1;66;0
WireConnection;45;0;47;0
WireConnection;45;1;27;0
WireConnection;45;2;102;0
WireConnection;30;0;19;0
WireConnection;71;0;32;0
WireConnection;71;1;74;0
WireConnection;69;0;65;1
WireConnection;69;1;121;0
WireConnection;132;0;139;0
WireConnection;132;1;140;0
WireConnection;2;0;32;0
WireConnection;2;1;45;0
WireConnection;152;0;32;0
WireConnection;152;1;155;0
WireConnection;156;0;152;1
WireConnection;70;0;69;0
WireConnection;150;0;30;1
WireConnection;150;1;30;1
WireConnection;150;2;30;1
WireConnection;76;0;71;1
WireConnection;31;0;30;0
WireConnection;141;0;131;0
WireConnection;141;1;132;0
WireConnection;141;2;142;0
WireConnection;22;0;2;1
WireConnection;157;0;156;0
WireConnection;157;1;158;0
WireConnection;157;2;12;0
WireConnection;157;3;31;0
WireConnection;7;0;22;0
WireConnection;7;1;52;0
WireConnection;7;2;12;0
WireConnection;7;3;31;0
WireConnection;126;1;141;0
WireConnection;21;0;20;0
WireConnection;21;1;150;0
WireConnection;21;2;70;0
WireConnection;85;0;31;0
WireConnection;85;1;84;0
WireConnection;85;2;86;0
WireConnection;40;0;75;0
WireConnection;40;1;12;0
WireConnection;40;2;76;0
WireConnection;40;3;31;0
WireConnection;39;0;7;0
WireConnection;39;1;40;0
WireConnection;39;2;85;0
WireConnection;39;3;157;0
WireConnection;127;0;126;0
WireConnection;127;1;21;0
WireConnection;129;0;127;0
WireConnection;129;1;39;0
ASEEND*/
//CHKSM=10605CA6EB04828F1E7DD618B426D64DE076FEF8