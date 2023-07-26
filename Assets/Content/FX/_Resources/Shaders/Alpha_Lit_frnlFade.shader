// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Alpha_Lit_frnlFade"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_Color("Color", Color) = (0,1,0.5778069,1)
		_frnl_pow("frnl_pow", Float) = 5
		[Toggle]_frnl_togle("frnl_togle", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float3 worldNormal;
			float3 viewDir;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform float _frnl_togle;
		uniform float _frnl_pow;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 temp_output_3_0 = ( tex2D( _MainTex, uv_MainTex ) * i.vertexColor * _Color );
			o.Albedo = (temp_output_3_0).rgb;
			float3 ase_worldNormal = i.worldNormal;
			float dotResult14 = dot( ase_worldNormal , i.viewDir );
			float temp_output_17_0 = pow( abs( dotResult14 ) , _frnl_pow );
			o.Alpha = ( (temp_output_3_0).a * lerp(temp_output_17_0,( 1.0 - temp_output_17_0 ),_frnl_togle) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
2629;133;1844;726;908.9633;-9.772743;1.467049;True;True
Node;AmplifyShaderEditor.WorldNormalVector;15;-691.4329,566.7283;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;16;-692.4329,718.7283;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;14;-437.4329,620.7283;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;18;-209.1205,613.6608;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-663.3474,448.7219;Float;False;Property;_frnl_pow;frnl_pow;2;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;2;-735.2172,61.64791;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-807.2151,224.6351;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;0,1,0.5778069,1;0,1,0.5778069,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-789.2602,-137.5681;Float;True;Property;_MainTex;Main Tex;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;17;-58.64092,599.4479;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-229,49;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;20;204.5271,678.7474;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;19;267.6631,450.7061;Float;False;Property;_frnl_togle;frnl_togle;3;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;6;-78.87805,133.1846;Float;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;-73,46;Float;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;143.1952,187.9636;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;570.3983,-59.79656;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Alpha_Lit_frnlFade;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;15;0
WireConnection;14;1;16;0
WireConnection;18;0;14;0
WireConnection;17;0;18;0
WireConnection;17;1;9;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;10;0
WireConnection;20;0;17;0
WireConnection;19;0;17;0
WireConnection;19;1;20;0
WireConnection;6;0;3;0
WireConnection;5;0;3;0
WireConnection;8;0;6;0
WireConnection;8;1;19;0
WireConnection;0;0;5;0
WireConnection;0;9;8;0
ASEEND*/
//CHKSM=8DD8432AAC3951BC4B9E861C1CD40C056A54B7F5