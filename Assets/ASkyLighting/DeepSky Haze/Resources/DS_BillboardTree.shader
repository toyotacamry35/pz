// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TerrainEngine/BillboardTree" {
	Properties{
		_MainTex("Base (RGB) Alpha (A)", 2D) = "white" {}
	}

		SubShader{
		Tags{ "Queue" = "Transparent-100" "IgnoreProjector" = "True" "RenderType" = "TreeBillboard" }

		Pass{
			ColorMask rgb
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "DS_TreeLib.cginc"

			struct v2f {
				fixed4 pos : SV_POSITION;
				fixed4 color : COLOR0;
				fixed2 uv : TEXCOORD0;
				fixed4 fogColour : TEXCOORD1;
			};

			v2f vert(appdata_tree_billboard v) {
				v2f o;
				TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
#if defined (UNITY_5_4_OR_NEWER)
				o.pos = UnityObjectToClipPos(v.vertex.xyz);
#else
				o.pos = UnityObjectToClipPos(v.vertex);
#endif
				o.uv.x = v.texcoord.x;
				o.uv.y = v.texcoord.y > 0;
				o.color = v.color;
				o.fogColour = DS_compute_fog_tree_vtx(v.vertex);
				return o;
			}

			sampler2D _MainTex;
			fixed4 frag(v2f input) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, input.uv);
				col.rgb *= input.color.rgb;
				col.rgb = col.rgb * input.fogColour.a + input.fogColour.rgb;
				clip(col.a);
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}