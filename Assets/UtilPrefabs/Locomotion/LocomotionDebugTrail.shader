// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Locomotion/Trail"
{
	Properties
	{
		_Size ("Size", Float) = 0.1
		_Outline ("Size", Float) = 0.15
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
		    ZTest Always
		    ZWrite Off
		    Cull Off 
		    Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"

            struct Point{
                float3  vertex;
                float4  color;
            };      

            StructuredBuffer<Point> points;

			struct appdata
			{
				float4 vertex : POSITION;
                fixed4 col : COLOR;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
                fixed4 col : COLOR;
			};
			
            struct g2f{
                float4 vertex : SV_POSITION;
                fixed4 col : COLOR;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Size;
			
			 v2g vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
             {
                 v2g o;
                 float4 vertex_position =  float4(points[id].vertex,1.0f);
                 o.vertex = mul(UNITY_MATRIX_VP, vertex_position);
                 o.col = points[id].color;
                 return o;
             }

			fixed4 frag (g2f i) : SV_Target
			{
				return i.col;
			}
			
			[maxvertexcount(4)]
            void geom (point v2g input[1], inout TriangleStream<g2f> outstream){
                g2f o = (g2f)0; 
                o.col = input[0].col;
                o.vertex = input[0].vertex + float4(0,-_Size,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(_Size,0,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(-_Size,0,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(0,_Size,0,0);
                outstream.Append(o);   
            }
			ENDCG
		}
		
		Pass
		{
		    ZTest Always
		    ZWrite Off
		    Cull Off 
		    Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#include "UnityCG.cginc"

            struct Point{
                float3  vertex;
                float4  color;
            };      

            StructuredBuffer<Point> points;

			struct appdata
			{
				float4 vertex : POSITION;
                fixed4 col : COLOR;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
                fixed4 col : COLOR;
			};
			
            struct g2f{
                float4 vertex : SV_POSITION;
                fixed4 col : COLOR;
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Size;
			float _Outline;
			
			 v2g vert (uint id : SV_VertexID, uint inst : SV_InstanceID)
             {
                 v2g o;
                 float4 vertex_position =  float4(points[id].vertex,1.0f);
                 o.vertex = mul(UNITY_MATRIX_VP, vertex_position);
                 o.col = points[id].color;
                 return o;
             }

			fixed4 frag (g2f i) : SV_Target
			{
				return float4(0,0,0,_Outline);
			}
			
			[maxvertexcount(5)]
            void geom (point v2g input[1], inout LineStream<g2f> outstream){
                g2f o = (g2f)0; 
                o.col = input[0].col;
                o.vertex = input[0].vertex + float4(0,-_Size,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(_Size,0,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(0,_Size,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(-_Size,0,0,0);
                outstream.Append(o);   
                o.vertex = input[0].vertex + float4(0,-_Size,0,0);
                outstream.Append(o);   
            }
			ENDCG
		}
	}
}
