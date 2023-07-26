// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "smoke_noise"
{
	Properties
	{
		_diffuse("diffuse", 2D) = "white" {}
		_noise("noise", 2D) = "white" {}
		_distort_tile_speed("distort_tile_speed", Vector) = (1,1,0,0)
		_distortion("distortion", Float) = 0.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _diffuse;
		uniform sampler2D _noise;
		uniform float4 _distort_tile_speed;
		uniform float _distortion;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult18 = (float2(_distort_tile_speed.x , _distort_tile_speed.y));
			float2 appendResult19 = (float2(_distort_tile_speed.z , _distort_tile_speed.w));
			float2 uv_TexCoord16 = i.uv_texcoord * appendResult18 + ( appendResult19 * _Time.y );
			float4 tex2DNode3 = tex2D( _diffuse, ( i.uv_texcoord + ( (-1.0 + (tex2D( _noise, uv_TexCoord16 ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) * 0.01 * _distortion ) ) );
			o.Albedo = ( i.vertexColor * tex2DNode3 ).rgb;
			o.Alpha = ( i.vertexColor.a * tex2DNode3.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
2629;121;1844;738;3937.187;1339.167;3.349054;True;True
Node;AmplifyShaderEditor.Vector4Node;17;-2142.274,-46.16458;Float;False;Property;_distort_tile_speed;distort_tile_speed;2;0;Create;True;0;0;False;0;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;25;-1927.678,210.1121;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;19;-1843.708,63.15308;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1697.349,71.16382;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;-1840.708,-54.84692;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1530.904,-25.24529;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;14;-1269.766,-48.6822;Float;True;Property;_noise;noise;1;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;15;-950.2073,-42.9769;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1025.107,146.3206;Float;False;Property;_distortion;distortion;3;0;Create;True;0;0;False;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-798.3013,-215.1021;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-741.1081,-40.98836;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0.01;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-422.0071,-96.19864;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;3;-215.1493,-126.1625;Float;True;Property;_diffuse;diffuse;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;9;-5.188003,-327.2839;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;201.3635,195.1163;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;229.1182,-155.0027;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;28;289.3177,25.50336;Float;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;728.0126,11.76689;Float;False;True;2;Float;ASEMaterialInspector;0;0;Lambert;smoke_noise;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;19;0;17;3
WireConnection;19;1;17;4
WireConnection;27;0;19;0
WireConnection;27;1;25;0
WireConnection;18;0;17;1
WireConnection;18;1;17;2
WireConnection;16;0;18;0
WireConnection;16;1;27;0
WireConnection;14;1;16;0
WireConnection;15;0;14;1
WireConnection;20;0;15;0
WireConnection;20;2;21;0
WireConnection;24;0;23;0
WireConnection;24;1;20;0
WireConnection;3;1;24;0
WireConnection;11;0;9;4
WireConnection;11;1;3;4
WireConnection;10;0;9;0
WireConnection;10;1;3;0
WireConnection;2;0;10;0
WireConnection;2;9;11;0
ASEEND*/
//CHKSM=D3B15839DAB331CAEFCC71E36EA4E0AF24896E38